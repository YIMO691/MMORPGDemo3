using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace MmoDemo.Client
{
    [Serializable]
    public class ResourceEntry
    {
        public string name;
        public string hash;
        public long size;
    }

    [Serializable]
    public class ManifestWrapper
    {
        public ResourceEntry[] files;
    }

    public class ResourceManager
    {
        private readonly string _serverBaseUrl;
        private string CacheDir => Application.persistentDataPath + "/resources/";

        public ResourceManager(string serverBaseUrl)
        {
            _serverBaseUrl = serverBaseUrl.TrimEnd('/');
        }

        public IEnumerator CheckForUpdates(Action<int, int> onProgress, Action<bool> onComplete)
        {
            // Download manifest
            var manifest = new List<ResourceEntry>();
            using (var req = UnityWebRequest.Get(_serverBaseUrl + "/api/resources/manifest"))
            {
                yield return req.SendWebRequest();
                if (req.result != UnityWebRequest.Result.Success)
                {
                    onComplete?.Invoke(false);
                    yield break;
                }
                var json = req.downloadHandler.text;
                var wrapper = JsonUtility.FromJson<ManifestWrapper>(json);
                if (wrapper?.files != null)
                    manifest.AddRange(wrapper.files);
            }

            if (manifest.Count == 0)
            {
                onComplete?.Invoke(true);
                yield break;
            }

            // Download changed files
            var total = manifest.Count;
            var downloaded = 0;
            Directory.CreateDirectory(CacheDir);

            foreach (var entry in manifest)
            {
                var localPath = CacheDir + entry.name;
                var localHash = "";
                if (File.Exists(localPath))
                    localHash = ComputeHash(localPath);

                if (localHash != entry.hash)
                {
                    using var req = UnityWebRequest.Get(_serverBaseUrl + "/api/resources/" + entry.name);
                    yield return req.SendWebRequest();
                    if (req.result == UnityWebRequest.Result.Success)
                    {
                        var dir = Path.GetDirectoryName(localPath);
                        if (dir != null) Directory.CreateDirectory(dir);
                        File.WriteAllBytes(localPath, req.downloadHandler.data);
                    }
                }
                downloaded++;
                onProgress?.Invoke(downloaded, total);
            }

            onComplete?.Invoke(true);
        }

        public string GetCachedPath(string name)
        {
            var path = CacheDir + name;
            return File.Exists(path) ? path : null;
        }

        public string ReadCachedText(string name)
        {
            var path = GetCachedPath(name);
            return path != null ? File.ReadAllText(path) : null;
        }

        private static string ComputeHash(string filePath)
        {
            using var sha = SHA256.Create();
            using var stream = File.OpenRead(filePath);
            var hash = sha.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
