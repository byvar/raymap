using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace OpenSpace {
    public static class FileSystem {
		public enum Mode {
			Normal, Web
		}
		public static Mode mode = Mode.Normal;
		private struct BigFileEntry {
			public int cacheLength;
			public long fileLength;
			public BigFileEntry(int cacheLength, long fileLength) {
				this.cacheLength = cacheLength;
				this.fileLength = fileLength;
			}
		}

		private static Dictionary<string, byte[]> virtualFiles = new Dictionary<string, byte[]>();
		private static Dictionary<string, BigFileEntry> virtualBigFiles = new Dictionary<string, BigFileEntry>();
		private static Dictionary<string, bool> existingDirectories = new Dictionary<string, bool>();
        private static string serverAddress = "https://getramone.com/raymap/data/";

        public static bool DirectoryExists(string path) {
            if (FileSystem.mode == FileSystem.Mode.Web) { // || (Application.isEditor && UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)) {
				if (existingDirectories.ContainsKey(path) && existingDirectories[path] == true) return true;
				return false;
			} else {
                return Directory.Exists(path);
            }
        }

        public static bool FileExists(string path) {
            if (FileSystem.mode == FileSystem.Mode.Web) { // || (Application.isEditor && UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)) {
                return ((virtualFiles.ContainsKey(path) && virtualFiles[path] != null) || (virtualBigFiles.ContainsKey(path)));
            } else {
                return File.Exists(path);
            }
        }

        public static void AddVirtualFile(string path, byte[] data) {
            virtualFiles[path] = data;
        }
		public static void AddVirtualBigFile(string path, long size, int cacheLength) {
			virtualBigFiles[path] = new BigFileEntry(cacheLength, size);
		}

		public static Stream GetFileReadStream(string path) {
            if (FileSystem.mode == FileSystem.Mode.Web) { // || (Application.isEditor && UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)) {
				if (virtualFiles.ContainsKey(path)) {
					return new MemoryStream(virtualFiles[path]);
				} else if (virtualBigFiles.ContainsKey(path)) {
					return new PartialHttpStream(serverAddress + path, cacheLen: virtualBigFiles[path].cacheLength, length: virtualBigFiles[path].fileLength);
				} else return null;
            } else {
                return File.OpenRead(path);
            }
        }

        public static IEnumerator DownloadFile(string path) {
			Debug.Log("Downloading " + path);
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

		public static IEnumerator CheckDirectory(string path) {
			if (existingDirectories.ContainsKey(path)) yield break;
			UnityWebRequest www = UnityWebRequest.Head(serverAddress + path + "/");
			yield return www.SendWebRequest();
			while (!www.isDone) {
				yield return null;
			}
			if (!www.isHttpError && !www.isNetworkError) {
				existingDirectories.Add(path, true);
			} else {
				existingDirectories.Add(path, false);
			}
		}

		public static IEnumerator InitBigFile(string path, int cacheLength) {
			UnityWebRequest www = UnityWebRequest.Head(serverAddress + path);
			yield return www.SendWebRequest();
			while (!www.isDone) {
				yield return null;
			}
			if (!www.isHttpError && !www.isNetworkError) {
				long contentLength;
				if (long.TryParse(www.GetResponseHeader("Content-Length"), out contentLength)) {
					AddVirtualBigFile(path, contentLength, cacheLength);
				}
			}
			yield return null;
		}

        public static long GetFileLength(string path) {
            if (FileExists(path)) {
                if (FileSystem.mode == FileSystem.Mode.Web) {
					if (virtualFiles.ContainsKey(path)) {
						return virtualFiles[path].Length;
					} else if (virtualBigFiles.ContainsKey(path)) {
						return virtualBigFiles[path].fileLength;
					} else return 0;
                } else {
                    return new FileInfo(path).Length;
                }
            } else {
                return 0;
            }
        }

	}
}
