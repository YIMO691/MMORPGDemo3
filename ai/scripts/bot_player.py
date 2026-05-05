"""
Phase 2 bot player — simulates a second player for multi-client testing.
Connects via HTTP (login + create role) then WebSocket (enter scene + random movement).

Usage: .venv/Scripts/python.exe ai/scripts/bot_player.py
"""
import asyncio
import json
import random
import time
import websockets
import urllib.request

SERVER = "http://localhost:5000"
WS_URL = "ws://localhost:5000/ws"


def http_post(path, data):
    req = urllib.request.Request(
        f"{SERVER}{path}",
        data=json.dumps(data).encode(),
        headers={"Content-Type": "application/json"}
    )
    with urllib.request.urlopen(req) as resp:
        return json.loads(resp.read())


async def main():
    # ── Phase 1: HTTP login + create role ──
    device = f"bot-{random.randint(1000, 9999)}"
    print(f"[Bot] Logging in as {device}...")
    login = http_post("/api/auth/guest-login", {
        "deviceId": device, "platform": "bot", "appVersion": "0.2.0"
    })
    pid = login["playerId"]
    tok = login["token"]
    print(f"[Bot] Login OK: {pid}")

    # Create a random bot character
    names = ["BotWarrior", "BotMage", "BotArcher", "BotKnight", "BotRogue"]
    name = random.choice(names)
    cls_id = random.randint(1, 3)
    create = http_post("/api/roles/create", {
        "playerId": pid, "token": tok, "name": name, "classId": cls_id
    })
    rid = create["role"]["roleId"]
    print(f"[Bot] Created {name} (role={rid})")

    # ── Phase 2: WebSocket ──
    async with websockets.connect(WS_URL) as ws:
        # Auth
        await ws.send(json.dumps({
            "t": "c2s.auth", "ts": 0,
            "p": {"playerId": pid, "token": tok, "roleId": rid}
        }))
        resp = json.loads(await ws.recv())
        ok = resp.get("p", {}).get("ok", False)
        print(f"[Bot] Auth: {'OK' if ok else 'FAILED'}")

        # Enter scene
        await ws.send(json.dumps({
            "t": "c2s.enter_scene", "ts": 0,
            "p": {"sceneId": "city_001"}
        }))
        resp = json.loads(await ws.recv())
        ok = resp.get("p", {}).get("ok", False)
        ents = len(resp.get("p", {}).get("entities", []))
        print(f"[Bot] Enter scene: {'OK' if ok else 'FAILED'} ({ents} entities)")

        # Random movement loop
        x, z = 0.0, 0.0
        print("[Bot] Moving randomly... (Ctrl+C to stop)")
        dir_x, dir_z = random.uniform(-1, 1), random.uniform(-1, 1)

        for _ in range(500):  # ~50 seconds
            # Change direction occasionally
            if random.random() < 0.05:
                dir_x, dir_z = random.uniform(-1, 1), random.uniform(-1, 1)

            # Move
            x += dir_x * 0.5
            z += dir_z * 0.5

            await ws.send(json.dumps({
                "t": "c2s.move", "ts": 0,
                "p": {
                    "dirX": round(dir_x, 2), "dirY": 0, "dirZ": round(dir_z, 2),
                    "posX": round(x, 2), "posY": 0, "posZ": round(z, 2)
                }
            }))

            # Read any incoming messages (entity snapshots, joins, leaves)
            try:
                while True:
                    msg = json.loads(await asyncio.wait_for(ws.recv(), timeout=0.01))
                    t = msg.get("t", "")
                    if t == "s2c.entity_snapshot":
                        entities = msg.get("p", {}).get("entities", [])
                        for e in entities:
                            print(f"  ↙ {e.get('name','?')} at ({e.get('posX',0):.1f}, {e.get('posZ',0):.1f})")
                    elif t == "s2c.entity_joined":
                        e = msg.get("p", {}).get("entity", {})
                        print(f"  → {e.get('name','?')} joined")
                    elif t == "s2c.entity_left":
                        eid = msg.get("p", {}).get("entityId", "?")
                        print(f"  ← {eid} left")
            except asyncio.TimeoutError:
                pass

            await asyncio.sleep(0.1)

    print("[Bot] Disconnected.")


if __name__ == "__main__":
    asyncio.run(main())
