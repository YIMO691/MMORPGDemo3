using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

namespace MmoDemo.Client
{
    public class LuaManager : IDisposable
    {
        private Script _script;
        private readonly Dictionary<string, object> _bridges = new();

        public void Start()
        {
            _script = new Script();
            foreach (var kv in _bridges)
                _script.Globals[kv.Key] = kv.Value;
            Debug.Log("[Lua] MoonSharp VM started");
        }

        public void RegisterBridge(string name, object instance)
        {
            _bridges[name] = instance;
            if (_script != null)
                _script.Globals[name] = instance;
            Debug.Log($"[Lua] Registered bridge: {name}");
        }

        public DynValue DoString(string script)
        {
            if (_script == null) return DynValue.Nil;
            try { return _script.DoString(script); }
            catch (Exception e) { Debug.LogError($"[Lua] Error: {e.Message}"); return DynValue.Nil; }
        }

        public DynValue DoFile(string path)
        {
            if (_script == null) return DynValue.Nil;
            try { return _script.DoFile(path); }
            catch (Exception e) { Debug.LogError($"[Lua] Error: {e.Message}"); return DynValue.Nil; }
        }

        public object Call(string funcName, params object[] args)
        {
            if (_script == null) return null;
            try
            {
                var fn = _script.Globals.Get(funcName);
                if (fn.IsNil()) return null;
                return _script.Call(fn, args);
            }
            catch (Exception e) { Debug.LogError($"[Lua] Call '{funcName}' error: {e.Message}"); return null; }
        }

        public void Reload()
        {
            // Re-run Start to clear globals and re-register bridges
            _script = new Script();
            foreach (var kv in _bridges)
                _script.Globals[kv.Key] = kv.Value;
            Debug.Log("[Lua] VM reloaded (hotfix applied)");
        }

        public void Dispose()
        {
            Debug.Log("[Lua] VM disposed");
            _bridges.Clear();
            _script = null;
        }
    }
}
