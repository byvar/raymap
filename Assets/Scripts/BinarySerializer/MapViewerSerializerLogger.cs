using System.IO;
using System.Text;
using BinarySerializer;

namespace Raymap
{
	public class MapViewerSerializerLogger : ISerializerLogger
	{
		public bool IsEnabled => UnitySettings.Log;

		private StreamWriter _logWriter;

		protected StreamWriter LogWriter => _logWriter ??= GetFile();

		public string OverrideLogPath { get; set; }
		public string LogFile => OverrideLogPath ?? UnitySettings.LogFile;
		public int BufferSize => 0x8000000; // 1 GB

		public StreamWriter GetFile()
		{
			return new StreamWriter(File.Open(LogFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8, BufferSize);
		}

		public void Log(object obj)
		{
			if (IsEnabled)
				LogWriter.WriteLine(obj != null ? obj.ToString() : "");
		}

		public void Dispose()
		{
			_logWriter?.Dispose();
			_logWriter = null;
		}
	}
}