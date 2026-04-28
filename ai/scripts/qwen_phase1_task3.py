import json
import os
import re
from pathlib import Path

from dotenv import load_dotenv
from openai import OpenAI


ROOT = Path(__file__).resolve().parents[2]

ALLOWED_FILES = {
    "proto/auth.proto",
    "docs/19_Login_Interface_Design.md",
    "docs/20_Login_Sequence.md",
    "client/docs/ClientModuleBoundary.md",
    "server/README.md",
    "docs/11_Changelog.md",
}

READ_CONTEXT_FILES = [
    "AGENTS.md",
    "MMORPG_AI_Agent_Workflow.md",
    "docs/16_Phase_1_Execution_Plan.md",
    "docs/05_Protocol_Rules.md",
    "proto/auth.proto",
    "client/docs/ClientModuleBoundary.md",
    "server/README.md",
    "docs/11_Changelog.md",
]


SYSTEM_PROMPT = """你是 Qwen-Coder，现在作为本地代码生成助手为 Unity MMORPG Demo 项目生成文件内容。

你必须严格遵守：
1. 只处理 Phase 1 Task 3：登录协议和接口设计。
2. 只允许写入白名单文件。
3. 不实现真实登录业务代码。
4. 不修改 server/src/。
5. 不修改 server/tests/。
6. 不创建真实 Unity 工程。
7. 不读取、输出或要求用户提供 .env。
8. 不输出 API Key、secret、password。
9. 客户端只上报意图，服务端保持权威。
10. 只返回 JSON，不要 Markdown 包裹，不要解释。

本次输出必须是 JSON，格式如下：
{
  "files": [
    {
      "path": "proto/auth.proto",
      "content": "完整文件内容"
    }
  ],
  "summary": [
    "一句话说明"
  ]
}

你必须输出完整文件内容，不要输出 diff，不要省略。
你必须一次性返回全部 6 个必需文件：proto/auth.proto、docs/19_Login_Interface_Design.md、docs/20_Login_Sequence.md、client/docs/ClientModuleBoundary.md、server/README.md、docs/11_Changelog.md。
如果你只返回其中一部分，输出将被判定为失败。
"""


USER_PROMPT = """请生成 Phase 1 Task 3：登录协议和接口设计 的所有文件内容。

任务目标：
1. 检查并完善 proto/auth.proto
2. 创建 docs/19_Login_Interface_Design.md
3. 创建 docs/20_Login_Sequence.md
4. 补充 client/docs/ClientModuleBoundary.md 中 Login 模块职责
5. 补充 server/README.md，明确登录接口尚未实现
6. 更新 docs/11_Changelog.md
7. 不实现登录业务
8. 不接数据库
9. 不接 Redis
10. 不实现 JWT
11. 不实现复杂鉴权
12. 不创建真实 Unity UI

允许写入的文件：
- proto/auth.proto
- docs/19_Login_Interface_Design.md
- docs/20_Login_Sequence.md
- client/docs/ClientModuleBoundary.md
- server/README.md
- docs/11_Changelog.md

禁止修改：
- .env
- .env.example
- .venv/
- .tools/
- ai/
- server/src/
- server/tests/
- client/UnityProject/
- configs/
- deploy/
- assets_source/

auth.proto 至少包含：

syntax = "proto3";
package mmo.auth;

message C2S_LoginReq {
  string device_id = 1;
  string platform = 2;
  string app_version = 3;
}

message S2C_LoginRes {
  int32 code = 1;
  string message = 2;
  string player_id = 3;
  string token = 4;
  int64 server_time = 5;
}

登录接口建议：
POST /api/auth/guest-login

请求 JSON 示例：
{
  "deviceId": "dev-local-001",
  "platform": "editor",
  "appVersion": "0.1.0"
}

响应 JSON 示例：
{
  "code": 0,
  "message": "OK",
  "playerId": "player_demo_001",
  "token": "dev_token_demo_001",
  "serverTime": 1730000000000
}

docs/20_Login_Sequence.md 必须包含 Mermaid 时序图。

Changelog 追加：
Phase 1 Task 3：完成登录协议和接口设计，补充 auth.proto、登录接口文档和登录时序说明，暂不实现登录业务。

请基于下面提供的上下文生成完整文件。

重要：最终 JSON 的 files 数组必须包含且只能包含以下 6 个 path：
1. proto/auth.proto
2. docs/19_Login_Interface_Design.md
3. docs/20_Login_Sequence.md
4. client/docs/ClientModuleBoundary.md
5. server/README.md
6. docs/11_Changelog.md
"""


def read_file(path: str) -> str:
    full = ROOT / path
    if not full.exists():
        return ""
    try:
        return full.read_text(encoding="utf-8")
    except UnicodeDecodeError:
        return full.read_text(encoding="utf-8-sig")


def build_context() -> str:
    chunks = []
    for path in READ_CONTEXT_FILES:
        full = ROOT / path
        if full.exists():
            chunks.append(f"\n\n--- FILE: {path} ---\n{read_file(path)}")
        else:
            chunks.append(f"\n\n--- FILE MISSING: {path} ---\n")
    return "\n".join(chunks)


def extract_json(text: str) -> dict:
    text = text.strip()

    # Remove accidental code fences.
    text = re.sub(r"^```(?:json)?\s*", "", text)
    text = re.sub(r"\s*```$", "", text)

    try:
        return json.loads(text)
    except json.JSONDecodeError:
        start = text.find("{")
        end = text.rfind("}")
        if start >= 0 and end > start:
            return json.loads(text[start : end + 1])
        raise


def validate_payload(payload: dict) -> None:
    if not isinstance(payload, dict):
        raise ValueError("Qwen output is not a JSON object.")

    files = payload.get("files")
    if not isinstance(files, list) or not files:
        raise ValueError("Qwen output must contain non-empty files list.")

    for item in files:
        path = item.get("path")
        content = item.get("content")

        if path not in ALLOWED_FILES:
            raise ValueError(f"File is not allowed: {path}")

        if not isinstance(content, str) or not content.strip():
            raise ValueError(f"Empty content for file: {path}")

        lowered = content.lower()
        forbidden = [
            "dashscope" + "_api_key=",
            "openai" + "_api_key=",
            "moonshot" + "_api_key=",
            "deepseek" + "_api_key=",
            "xiaomi" + "_api_key=",
            "sk" + "-",
            "api key",
            "apikey",
            "password" + "=",
            "secret" + "=",
        ]
        for token in forbidden:
            if token in lowered:
                raise ValueError(f"Potential secret detected in generated content for {path}: {token}")

    paths = {item["path"] for item in files}
    required = {
        "proto/auth.proto",
        "docs/19_Login_Interface_Design.md",
        "docs/20_Login_Sequence.md",
        "client/docs/ClientModuleBoundary.md",
        "server/README.md",
        "docs/11_Changelog.md",
    }
    missing = required - paths
    if missing:
        raise ValueError(f"Qwen did not return required files: {sorted(missing)}")


def write_files(payload: dict) -> None:
    for item in payload["files"]:
        path = item["path"]
        content = item["content"].rstrip() + "\n"

        target = ROOT / path
        target.parent.mkdir(parents=True, exist_ok=True)
        target.write_text(content, encoding="utf-8")
        print(f"[WRITE] {path}")


def main() -> None:
    load_dotenv(ROOT / ".env")

    api_key = os.getenv("DASHSCOPE_API_KEY")
    base_url = os.getenv("QWEN_BASE_URL", "https://dashscope-intl.aliyuncs.com/compatible-mode/v1")
    model = os.getenv("QWEN_CODER_MODEL", "qwen3-coder-plus")

    if not api_key:
        raise RuntimeError("Missing DASHSCOPE_API_KEY in .env or environment variables.")

    client = OpenAI(api_key=api_key, base_url=base_url)

    context = build_context()
    prompt = USER_PROMPT + "\n\n项目上下文：" + context

    print(f"[QWEN] model={model}")
    print("[QWEN] generating Phase 1 Task 3 files...")

    completion = client.chat.completions.create(
        model=model,
        messages=[
            {"role": "system", "content": SYSTEM_PROMPT},
            {"role": "user", "content": prompt},
        ],
        temperature=0.2,
        max_tokens=12000,
    )

    content = completion.choices[0].message.content
    output_dir = ROOT / "ai" / "outputs"
    output_dir.mkdir(parents=True, exist_ok=True)
    raw_path = output_dir / "qwen_phase1_task3_raw.json"
    raw_path.write_text(content, encoding="utf-8")
    print(f"[RAW] {raw_path}")

    payload = extract_json(content)
    validate_payload(payload)
    write_files(payload)

    print("\n[DONE] Qwen Phase 1 Task 3 generation completed.")
    print("Next commands:")
    print("  git status")
    print("  Select-String -Path proto/auth.proto -Pattern \"C2S_LoginReq\"")
    print("  Select-String -Path proto/auth.proto -Pattern \"S2C_LoginRes\"")
    print("  Test-Path docs/19_Login_Interface_Design.md")
    print("  Test-Path docs/20_Login_Sequence.md")


if __name__ == "__main__":
    main()
