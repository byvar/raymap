using Cysharp.Threading.Tasks;
using System.IO;

namespace BinarySerializer.Unity {
    public static class ContextExtensions {
        public static async UniTask<LinearFile> AddLinearFileAsync(this Context context, string filePath, Endian endianness = Endian.Little, bool recreateOnWrite = true, int? bigFileCacheLength = null) {
            var absolutePath = context.GetAbsoluteFilePath(filePath);

            if (bigFileCacheLength.HasValue) {
                await FileSystem.PrepareBigFile(absolutePath, bigFileCacheLength.Value);
            } else {
                await FileSystem.PrepareFile(absolutePath);
            }

            if (!FileSystem.FileExists(absolutePath))
                return null;

            var file = new LinearFile(context, filePath, endianness) {
                RecreateOnWrite = recreateOnWrite
            };

            context.AddFile(file);

            return file;
        }
        public static async UniTask<MemoryMappedFile> AddMemoryMappedFile(this Context context, string filePath, uint baseAddress, Endian endianness = Endian.Little, bool recreateOnWrite = true, long memoryMappedPriority = -1) {
            var absolutePath = context.GetAbsoluteFilePath(filePath);

            await FileSystem.PrepareFile(absolutePath);

            if (!FileSystem.FileExists(absolutePath))
                return null;

            var file = new MemoryMappedFile(context, filePath, baseAddress, endianness, memoryMappedPriority: memoryMappedPriority) {
                RecreateOnWrite = recreateOnWrite
            };

            context.AddFile(file);

            return file;
        }
        public static StreamFile AddStreamFile(this Context context, string name, Stream stream, Endian endianness = Endian.Little, bool allowLocalPointers = false) {
            var file = new StreamFile(context, name, stream, endianness, allowLocalPointers);

            context.AddFile(file);

            return file;
        }
        public static StreamFile AddStreamFile(this Context context, string name, byte[] bytes, Endian endianness = Endian.Little, bool allowLocalPointers = false) {
            var file = new StreamFile(context, name, new MemoryStream(bytes), endianness, allowLocalPointers);

            context.AddFile(file);

            return file;
        }
        public static MemoryMappedStreamFile AddMemoryMappedStreamFile(this Context context, string name, byte[] bytes, uint baseAddress, Endian endianness = Endian.Little) {
            var file = new MemoryMappedStreamFile(context, name, baseAddress, bytes, endianness);

            context.AddFile(file);

            return file;
        }
	}
}