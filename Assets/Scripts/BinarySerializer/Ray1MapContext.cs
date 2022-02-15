using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;
using BinarySerializer.Unity;
using OpenSpace;
using UnityEngine;
using ILogger = BinarySerializer.ILogger;
using Reader = BinarySerializer.Reader;

namespace Raymap {
    public class Ray1MapContext : Context {
        public Ray1MapContext(string basePath, MapViewerSettings settings) : base(
            basePath: basePath, // Pass in the base path
            settings: new R1SerializerSettings(), // Pass in the settings
            serializerLog: new R1SerializerLog(), // Use R1 serializer log for logging to a file
            fileManager: new R1FileManager(), // Use R1 file manager for use with FileSystem
            logger: new UnityLogger()) // Use Unity logger
        {
            // Add the game settings
            AddSettings(settings);

            settings.GetGameManager.AddContextSettings(this);
            settings.GetGameManager.AddContextPointers(this);
        }
        public Ray1MapContext(MapViewerSettings settings) : this(settings.GameDirectory, settings) { }

        public MapViewerSettings GameSettings => GetSettings<MapViewerSettings>();

        public class R1SerializerSettings : ISerializerSettings {
            /// <summary>
            /// The default string encoding to use when none is specified
            /// </summary>
            public Encoding DefaultStringEncoding => Encoding.GetEncoding(1252);

            /// <summary>
            /// Indicates if a backup file should be created when writing to a file
            /// </summary>
            public bool CreateBackupOnWrite => UnitySettings.BackupFiles;

            /// <summary>
            /// Indicates if pointers should be saved in the Memory Map for relocation
            /// </summary>
            public bool SavePointersForRelocation => false;

            /// <summary>
            /// Indicates if caching read objects should be ignored
            /// </summary>
            public bool IgnoreCacheOnRead => false;

            /// <summary>
            /// The pointer size to use when logging a <see cref="LegacyPointer"/>. Set to <see langword="null"/> to dynamically determine the appropriate size.
            /// </summary>
            public PointerSize? LoggingPointerSize => PointerSize.Pointer32;

            public Endian DefaultEndianness => Endian.Little;
        }

        public class R1FileManager : IFileManager {
            public PathSeparatorChar SeparatorCharacter => PathSeparatorChar.ForwardSlash;

            public bool DirectoryExists(string path) => FileSystem.DirectoryExists(path);

            public bool FileExists(string path) => FileSystem.FileExists(path);

            public Stream GetFileReadStream(string path) => FileSystem.GetFileReadStream(path);

            public Stream GetFileWriteStream(string path, bool recreateOnWrite = true) => FileSystem.GetFileWriteStream(path, recreateOnWrite);

            public async Task FillCacheForReadAsync(long length, Reader reader) {
                if (reader.BaseStream.InnerStream is PartialHttpStream httpStream)
                    await httpStream.FillCacheForRead(length);
            }
		}

        public class UnityLogger : ILogger {
            public void Log(object log, params object[] args) => Debug.Log(String.Format(log?.ToString() ?? String.Empty, args));
            public void LogWarning(object log, params object[] args) => Debug.LogWarning(String.Format(log?.ToString() ?? String.Empty, args));
            public void LogError(object log, params object[] args) => Debug.LogError(String.Format(log?.ToString() ?? String.Empty, args));
        }

        public class R1SerializerLog : ISerializerLog {
            public bool IsEnabled => UnitySettings.Log;

            private StreamWriter _logWriter;

            protected StreamWriter LogWriter => _logWriter ??= GetFile();

            public string OverrideLogPath { get; set; }
            public string LogFile => OverrideLogPath ?? UnitySettings.LogFile;
            public int BufferSize => 0x8000000; // 1 GB

            public StreamWriter GetFile() {
                return new StreamWriter(File.Open(LogFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8, BufferSize);
            }

            public void Log(object obj) {
                if (IsEnabled)
                    LogWriter.WriteLine(obj != null ? obj.ToString() : "");
            }

            public void Dispose() {
                _logWriter?.Dispose();
                _logWriter = null;
            }
        }
    }
}