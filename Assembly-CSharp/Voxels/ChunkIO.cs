using System;
using System.IO;
using K4os.Compression.LZ4;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Voxels
{
	public static class ChunkIO
	{
		public static string PathFor(int3 id)
		{
			return Path.Combine(ChunkIO.Root, string.Format("{0}_{1}_{2}.vox", id.x, id.y, id.z));
		}

		public static void SaveChunk(ChunkDTO dto)
		{
			string text = ChunkIO.PathFor(dto.Id);
			Debug.Log(string.Format("Saving chunk {0} to {1}", dto.Id, text));
			ChunkIO.Save(text, dto);
		}

		public static bool TryLoadChunk(int3 id, out ChunkDTO dto)
		{
			string arg = ChunkIO.PathFor(id);
			if (!File.Exists(ChunkIO.PathFor(id)))
			{
				dto = default(ChunkDTO);
				return false;
			}
			dto = ChunkIO.Load(ChunkIO.PathFor(id), Allocator.Persistent);
			if (dto.IsValid)
			{
				Debug.Log(string.Format("Loaded chunk {0} from {1}", id, arg));
			}
			else
			{
				Debug.Log(string.Format("Chunk {0} at {1} magic or version mismatch.", id, arg));
			}
			return dto.IsValid;
		}

		public static void Save(string path, in ChunkDTO chunk)
		{
			if (!Directory.Exists(ChunkIO.Root))
			{
				Directory.CreateDirectory(ChunkIO.Root);
			}
			using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, false))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
				{
					binaryWriter.Write(1448040524U);
					binaryWriter.Write(5);
					binaryWriter.Write(chunk.Id.x);
					binaryWriter.Write(chunk.Id.y);
					binaryWriter.Write(chunk.Id.z);
					binaryWriter.Write(chunk.Size.x);
					binaryWriter.Write(chunk.Size.y);
					binaryWriter.Write(chunk.Size.z);
					binaryWriter.Write(chunk.Dimensions.x);
					binaryWriter.Write(chunk.Dimensions.y);
					binaryWriter.Write(chunk.Dimensions.z);
					ChunkIO.WriteNativeArray(binaryWriter, chunk.Density);
					ChunkIO.WriteNativeArray(binaryWriter, chunk.Material);
				}
			}
		}

		public static ChunkDTO Load(string path, Allocator alloc = Allocator.Persistent)
		{
			ChunkDTO chunkDTO;
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, false))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					int num = (int)binaryReader.ReadUInt32();
					int num2 = binaryReader.ReadInt32();
					if (num != 1448040524 || num2 != 5)
					{
						chunkDTO = default(ChunkDTO);
						chunkDTO = chunkDTO;
					}
					else
					{
						int3 id = new int3(binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32());
						int3 size = new int3(binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32());
						int3 dimensions = new int3(binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32());
						NativeArray<byte> density = ChunkIO.ReadNativeArray(binaryReader, alloc);
						NativeArray<byte> material = ChunkIO.ReadNativeArray(binaryReader, alloc);
						chunkDTO = new ChunkDTO
						{
							Id = id,
							Size = size,
							Dimensions = dimensions,
							Density = density,
							Material = material
						};
					}
				}
			}
			return chunkDTO;
		}

		private static void WriteNativeArray(BinaryWriter bw, NativeArray<byte> src)
		{
			byte[] array = LZ4Pickler.Pickle(src.ToArray(), LZ4Level.L00_FAST);
			bw.Write(array.Length);
			bw.Write(array);
		}

		private static NativeArray<byte> ReadNativeArray(BinaryReader br, Allocator alloc = Allocator.Persistent)
		{
			int count = br.ReadInt32();
			byte[] array = LZ4Pickler.Unpickle(br.ReadBytes(count));
			int length = array.Length;
			NativeArray<byte> nativeArray = new NativeArray<byte>(length, alloc, NativeArrayOptions.ClearMemory);
			NativeArray<byte>.Copy(array, nativeArray, length);
			return nativeArray;
		}

		public static void DeleteWorld()
		{
			if (Directory.Exists(ChunkIO.Root))
			{
				Directory.Delete(ChunkIO.Root, true);
			}
			Debug.Log("All chunks deleted.");
		}

		public const uint MAGIC = 1448040524U;

		public const int VERSION = 5;

		private static readonly string Root = Path.Combine(Application.persistentDataPath, "WorldSaves");
	}
}
