using System;
using System.Collections.Generic;
using UnityEngine;

namespace MmoDemo.Client
{
    /// <summary>
    /// Minimal Lua VM wrapper for Phase 1.
    /// Registers C# bridge objects so Lua scripts can call into native code.
    /// The actual xLua integration happens when the Unity project is created.
    /// </summary>
    public class LuaManager : IDisposable
    {
        private readonly Dictionary<string, object> _bridges = new();

        public void RegisterBridge(string name, object instance)
        {
            _bridges[name] = instance;
            Debug.Log($"[Lua] Registered bridge: {name}");
        }

        public void Start()
        {
            Debug.Log("[Lua] VM started (placeholder — xLua not yet integrated)");

            // In real xLua setup:
            //   var env = new LuaTable();
            //   foreach (var kv in _bridges)
            //       env.Set(kv.Key, kv.Value);
        }

        public void DoString(string script)
        {
            Debug.Log($"[Lua] Execute: {script[..Math.Min(script.Length, 80)]}");
            // In real xLua: LuaEnv.DoString(script);
        }

        public void Dispose()
        {
            Debug.Log("[Lua] VM disposed");
            _bridges.Clear();
        }
    }
}
