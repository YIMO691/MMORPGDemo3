using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace MmoDemo.Client
{
    /// <summary>
    /// HTTP client wrapper for Phase 1 REST API calls.
    /// Uses UnityWebRequest with coroutines for Unity compatibility.
    /// </summary>
    public class NetworkManager
    {
        private readonly string _serverBaseUrl;
        private string _playerId;
        private string _token;

        public string PlayerId => _playerId;
        public string Token => _token;

        public NetworkManager(string serverBaseUrl)
        {
            _serverBaseUrl = serverBaseUrl.TrimEnd('/');
        }

        public IEnumerator CheckHealth(Action<bool> callback)
        {
            using var req = UnityWebRequest.Get(_serverBaseUrl + "/health");
            yield return req.SendWebRequest();
            var ok = req.result == UnityWebRequest.Result.Success;
            Debug.Log(ok ? $"[Network] Health OK" : $"[Network] Health FAIL: {req.error}");
            callback(ok);
        }

        public IEnumerator GuestLogin(string deviceId, string platform, string appVersion,
            Action<LoginResult> onSuccess, Action<string> onError)
        {
            var json = $"{{\"deviceId\":\"{Esc(deviceId)}\",\"platform\":\"{Esc(platform)}\",\"appVersion\":\"{Esc(appVersion)}\"}}";
            yield return Post("/api/auth/guest-login", json, (raw) =>
            {
                var r = JsonUtility.FromJson<LoginResult>(raw);
                if (r.code == 0)
                {
                    _playerId = r.playerId;
                    _token = r.token;
                    Debug.Log($"[Network] Login OK: {_playerId}");
                    onSuccess(r);
                }
                else onError(r.message ?? "Login failed");
            }, onError);
        }

        public IEnumerator GetRoleList(Action<RoleListResult> onSuccess, Action<string> onError)
        {
            var json = $"{{\"playerId\":\"{Esc(_playerId)}\",\"token\":\"{Esc(_token)}\"}}";
            yield return Post("/api/roles/list", json,
                raw => onSuccess(JsonUtility.FromJson<RoleListResult>(raw)), onError);
        }

        public IEnumerator CreateRole(string name, int classId,
            Action<CreateRoleResult> onSuccess, Action<string> onError)
        {
            var json = $"{{\"playerId\":\"{Esc(_playerId)}\",\"token\":\"{Esc(_token)}\",\"name\":\"{Esc(name)}\",\"classId\":{classId}}}";
            yield return Post("/api/roles/create", json,
                raw => onSuccess(JsonUtility.FromJson<CreateRoleResult>(raw)), onError);
        }

        public IEnumerator SelectRole(string roleId,
            Action<SelectRoleResult> onSuccess, Action<string> onError)
        {
            var json = $"{{\"playerId\":\"{Esc(_playerId)}\",\"token\":\"{Esc(_token)}\",\"roleId\":\"{Esc(roleId)}\"}}";
            yield return Post("/api/roles/select", json,
                raw => onSuccess(JsonUtility.FromJson<SelectRoleResult>(raw)), onError);
        }

        public IEnumerator EnterCity(string roleId,
            Action<EnterCityResult> onSuccess, Action<string> onError)
        {
            var json = $"{{\"playerId\":\"{Esc(_playerId)}\",\"token\":\"{Esc(_token)}\",\"roleId\":\"{Esc(roleId)}\"}}";
            yield return Post("/api/scene/enter-city", json,
                raw => onSuccess(JsonUtility.FromJson<EnterCityResult>(raw)), onError);
        }

        private IEnumerator Post(string path, string body,
            Action<string> onSuccess, Action<string> onError)
        {
            using var req = new UnityWebRequest(_serverBaseUrl + path, "POST");
            var bytes = Encoding.UTF8.GetBytes(body);
            req.uploadHandler = new UploadHandlerRaw(bytes);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
                onSuccess(req.downloadHandler.text);
            else
            {
                Debug.LogError($"[Network] POST {path} failed: {req.error}");
                onError(req.error);
            }
        }

        private static string Esc(string s)
            => (s ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    // ── DTOs ──

    [Serializable] public class LoginResult
    { public int code; public string message; public string playerId; public string token; public long serverTime; }

    [Serializable] public class RoleInfo
    { public string roleId; public string name; public int level; public int classId; public int sceneId; public long gold; }

    [Serializable] public class RoleListResult
    { public int code; public string message; public RoleInfo[] roles; }

    [Serializable] public class CreateRoleResult
    { public int code; public string message; public RoleInfo role; }

    [Serializable] public class SelectRoleResult
    { public int code; public string message; public RoleInfo role; }

    [Serializable] public class EnterCityResult
    { public int code; public string message; public RoleInfo role; public long serverTime; }
}
