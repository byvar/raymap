using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace OpenSpace {
    public static class FileSystem {
        private static Dictionary<string, byte[]> virtualFiles = new Dictionary<string, byte[]>();
        private static string serverAddress = "https://getramone.com/raymap/data/";

        public static bool DirectoryExists(string path) {
            if (Application.platform == RuntimePlatform.WebGLPlayer) { // || (Application.isEditor && UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)) {
                return true;
            } else {
                return Directory.Exists(path);
            }
        }

        public static bool FileExists(string path) {
            if (Application.platform == RuntimePlatform.WebGLPlayer) { // || (Application.isEditor && UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)) {
                return (virtualFiles.ContainsKey(path) && virtualFiles[path] != null);
            } else {
                return File.Exists(path);
            }
        }

        public static void AddVirtualFile(string path, byte[] data) {
            virtualFiles[path] = data;
        }

        public static Stream GetFileReadStream(string path) {
            if (Application.platform == RuntimePlatform.WebGLPlayer) { // || (Application.isEditor && UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)) {
                return new MemoryStream(virtualFiles[path]);
            } else {
                return File.OpenRead(path);
            }
        }

        public static IEnumerator DownloadFile(string path) {
            UnityWebRequest www = UnityWebRequest.Get(serverAddress + path);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
                virtualFiles[path] = null;
            } else {
                // Or retrieve results as binary data
                AddVirtualFile(path, www.downloadHandler.data);
            }
        }

        public static long GetFileLength(string path) {
            if (FileExists(path)) {
                if (Application.platform == RuntimePlatform.WebGLPlayer) {
                    return virtualFiles[path].Length;
                } else {
                    return new FileInfo(path).Length;
                }
            } else {
                return 0;
            }
        }

    }
}
