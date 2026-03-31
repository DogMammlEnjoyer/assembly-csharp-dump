using System;

namespace Liv.Lck.Core.FFI
{
	internal struct GameInfo
	{
		public static GameInfo AllocateFromGameInfo(GameInfo gameInfo)
		{
			return new GameInfo(gameInfo);
		}

		public void Free()
		{
			InteropUtilities.Free(this.GameName);
			InteropUtilities.Free(this.GameVersion);
			InteropUtilities.Free(this.ProjectName);
			InteropUtilities.Free(this.CompanyName);
			InteropUtilities.Free(this.EngineVersion);
			InteropUtilities.Free(this.RenderPipeline);
			InteropUtilities.Free(this.GraphicsAPI);
			InteropUtilities.Free(this.Platform);
			InteropUtilities.Free(this.PersistentDataPath);
			InteropUtilities.Free(this.InteractionSystems);
		}

		private GameInfo(GameInfo gameInfo)
		{
			this.GameName = InteropUtilities.StringToUTF8Pointer(gameInfo.GameName);
			this.GameVersion = InteropUtilities.StringToUTF8Pointer(gameInfo.GameVersion);
			this.ProjectName = InteropUtilities.StringToUTF8Pointer(gameInfo.ProjectName);
			this.CompanyName = InteropUtilities.StringToUTF8Pointer(gameInfo.CompanyName);
			this.EngineVersion = InteropUtilities.StringToUTF8Pointer(gameInfo.EngineVersion);
			this.RenderPipeline = InteropUtilities.StringToUTF8Pointer(gameInfo.RenderPipeline);
			this.GraphicsAPI = InteropUtilities.StringToUTF8Pointer(gameInfo.GraphicsAPI);
			this.Platform = InteropUtilities.StringToUTF8Pointer(gameInfo.Platform);
			this.PersistentDataPath = InteropUtilities.StringToUTF8Pointer(gameInfo.PersistentDataPath);
			this.InteractionSystems = InteropUtilities.StringToUTF8Pointer(gameInfo.InteractionSystems);
		}

		public IntPtr GameName;

		public IntPtr GameVersion;

		public IntPtr ProjectName;

		public IntPtr CompanyName;

		public IntPtr EngineVersion;

		public IntPtr RenderPipeline;

		public IntPtr GraphicsAPI;

		public IntPtr Platform;

		public IntPtr PersistentDataPath;

		public IntPtr InteractionSystems;
	}
}
