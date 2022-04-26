using System.IO;
using System.Threading.Tasks;
using BinarySerializer;
using BinarySerializer.Unity;

namespace Raymap
{
	public class MapViewerFileManager : IFileManager
	{
		public PathSeparatorChar SeparatorCharacter => PathSeparatorChar.ForwardSlash;

		public bool DirectoryExists(string path) => FileSystem.DirectoryExists(path);

		public bool FileExists(string path) => FileSystem.FileExists(path);

		public Stream GetFileReadStream(string path) => FileSystem.GetFileReadStream(path);

		public Stream GetFileWriteStream(string path, bool recreateOnWrite = true) => FileSystem.GetFileWriteStream(path, recreateOnWrite);

		public async Task FillCacheForReadAsync(long length, Reader reader)
		{
			if (reader.BaseStream.InnerStream is PartialHttpStream httpStream)
				await httpStream.FillCacheForRead(length);
		}
	}
}