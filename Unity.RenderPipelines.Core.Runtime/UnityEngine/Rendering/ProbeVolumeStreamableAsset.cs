using System;
using System.IO;
using Unity.IO.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering
{
	[MovedFrom(false, "UnityEngine.Rendering", "Unity.RenderPipelines.Core.Runtime", "ProbeVolumeBakingSet.StreamableAsset")]
	[Serializable]
	internal class ProbeVolumeStreamableAsset
	{
		public string assetGUID
		{
			get
			{
				return this.m_AssetGUID;
			}
		}

		public TextAsset asset
		{
			get
			{
				return this.m_Asset;
			}
		}

		public int elementSize
		{
			get
			{
				return this.m_ElementSize;
			}
		}

		public SerializedDictionary<int, ProbeVolumeStreamableAsset.StreamableCellDesc> streamableCellDescs
		{
			get
			{
				return this.m_StreamableCellDescs;
			}
		}

		public ProbeVolumeStreamableAsset(string apvStreamingAssetsPath, SerializedDictionary<int, ProbeVolumeStreamableAsset.StreamableCellDesc> cellDescs, int elementSize, string bakingSetGUID, string assetGUID)
		{
			this.m_AssetGUID = assetGUID;
			this.m_StreamableCellDescs = cellDescs;
			this.m_ElementSize = elementSize;
			this.m_StreamableAssetPath = Path.Combine(Path.Combine(apvStreamingAssetsPath, bakingSetGUID), this.m_AssetGUID + ".bytes");
		}

		internal void RefreshAssetPath()
		{
			this.m_FinalAssetPath = Path.Combine(Application.streamingAssetsPath, this.m_StreamableAssetPath);
		}

		public string GetAssetPath()
		{
			if (string.IsNullOrEmpty(this.m_FinalAssetPath))
			{
				this.RefreshAssetPath();
			}
			return this.m_FinalAssetPath;
		}

		internal bool HasValidAssetReference()
		{
			return this.m_Asset != null && this.m_Asset.bytes != null;
		}

		public unsafe bool FileExists()
		{
			if (this.m_Asset != null)
			{
				return true;
			}
			FileInfoResult fileInfoResult;
			AsyncReadManager.GetFileInfo(this.GetAssetPath(), &fileInfoResult).JobHandle.Complete();
			return fileInfoResult.FileState == FileState.Exists;
		}

		public long GetFileSize()
		{
			return new FileInfo(this.GetAssetPath()).Length;
		}

		public bool IsOpen()
		{
			return this.m_AssetFileHandle.IsValid();
		}

		public FileHandle OpenFile()
		{
			if (this.m_AssetFileHandle.IsValid())
			{
				return this.m_AssetFileHandle;
			}
			this.m_AssetFileHandle = AsyncReadManager.OpenFileAsync(this.GetAssetPath());
			return this.m_AssetFileHandle;
		}

		public void CloseFile()
		{
			if (this.m_AssetFileHandle.IsValid() && this.m_AssetFileHandle.JobHandle.IsCompleted)
			{
				this.m_AssetFileHandle.Close(default(JobHandle));
			}
			this.m_AssetFileHandle = default(FileHandle);
		}

		public bool IsValid()
		{
			return !string.IsNullOrEmpty(this.m_AssetGUID);
		}

		public void Dispose()
		{
			if (this.m_AssetFileHandle.IsValid())
			{
				this.m_AssetFileHandle.Close(default(JobHandle)).Complete();
				this.m_AssetFileHandle = default(FileHandle);
			}
		}

		[SerializeField]
		[FormerlySerializedAs("assetGUID")]
		private string m_AssetGUID = "";

		[SerializeField]
		[FormerlySerializedAs("streamableAssetPath")]
		private string m_StreamableAssetPath = "";

		[SerializeField]
		[FormerlySerializedAs("elementSize")]
		private int m_ElementSize;

		[SerializeField]
		[FormerlySerializedAs("streamableCellDescs")]
		private SerializedDictionary<int, ProbeVolumeStreamableAsset.StreamableCellDesc> m_StreamableCellDescs = new SerializedDictionary<int, ProbeVolumeStreamableAsset.StreamableCellDesc>();

		[SerializeField]
		private TextAsset m_Asset;

		private string m_FinalAssetPath;

		private FileHandle m_AssetFileHandle;

		[MovedFrom(false, "UnityEngine.Rendering", "Unity.RenderPipelines.Core.Runtime", "ProbeVolumeBakingSet.StreamableAsset.StreamableCellDesc")]
		[Serializable]
		public struct StreamableCellDesc
		{
			public int offset;

			public int elementCount;
		}
	}
}
