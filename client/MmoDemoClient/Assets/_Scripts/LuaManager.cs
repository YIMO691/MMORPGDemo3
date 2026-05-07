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
            {
                try { _script.Globals[kv.Key] = DynValue.FromObject(_script, kv.Value); }
                catch (Exception e) { Debug.LogWarning($"[Lua] Bridge '{kv.Key}' failed: {e.Message}"); }
            }
            Debug.Log("[Lua] MoonSharp VM started");
        }

        public void RegisterBridge(string name, object instance)
        {
            _bridges[name] = instance;
            if (_script != null)
            {
                try { _script.Globals[name] = DynValue.FromObject(_script, instance); }
                catch (Exception e) { Debug.LogWarning($"[Lua] Bridge '{name}' failed: {e.Message}"); }
            }
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
            _script = new Script();
            foreach (var kv in _bridges)
            {
                try { _script.Globals[kv.Key] = DynValue.FromObject(_script, kv.Value); }
                catch (Exception e) { Debug.LogWarning($"[Lua] Bridge '{kv.Key}' failed: {e.Message}"); }
            }
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
