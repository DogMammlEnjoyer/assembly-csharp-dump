using System;
using System.Runtime.InteropServices;

namespace Liv.Lck.Core
{
	public struct GameInfo
	{
		[MarshalAs(UnmanagedType.LPUTF8Str)]
		public string GameName;

		[MarshalAs(UnmanagedType.LPUTF8Str)]
		public string GameVersion;

		[MarshalAs(UnmanagedType.LPUTF8Str)]
		public string ProjectName;

		[MarshalAs(UnmanagedType.LPUTF8Str)]
		public string CompanyName;

		[MarshalAs(UnmanagedType.LPUTF8Str)]
		public string EngineVersion;

		[MarshalAs(UnmanagedType.LPUTF8Str)]
		public string RenderPipeline;

		[MarshalAs(UnmanagedType.LPUTF8Str)]
		public string GraphicsAPI;

		[MarshalAs(UnmanagedType.LPUTF8Str)]
		public string Platform;

		[MarshalAs(UnmanagedType.LPUTF8Str)]
		public string PersistentDataPath;
	}
}
