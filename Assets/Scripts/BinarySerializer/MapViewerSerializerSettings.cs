﻿using System.Text;
using BinarySerializer;
using BinarySerializer.Unity;
using OpenSpace;

namespace Raymap
{
	public class MapViewerSerializerSettings : ISerializerSettings
	{
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
		/// Indicates if the default should be to check and log if bytes skipped for an alignment are not null
		/// </summary>
		public bool LogAlignIfNotNull => FileSystem.mode != FileSystem.Mode.Web;

		/// <summary>
		/// The pointer size to use when logging a <see cref="LegacyPointer"/>. Set to <see langword="null"/> to dynamically determine the appropriate size.
		/// </summary>
		public PointerSize? LoggingPointerSize => PointerSize.Pointer32;

		public Endian DefaultEndianness => Endian.Little;

		public bool AutoInitReadMap => UnitySettings.ExportReadMaps;

		public bool AutoExportReadMap => UnitySettings.ExportReadMaps;
	}
}