using System;
using UnityEngine;

namespace Fusion
{
	internal class NetworkTransformSerializer : NetworkBufferSerializer
	{
		public unsafe override int Write(Simulation.SendContext sc, NetworkObjectMeta meta, NetworkBufferSerializerInfo info, Span<int> ptr, int word, int prev)
		{
			word -= info.Offset;
			Assert.Check<int, int, int>(prev < word, prev, word, info.Offset);
			Assert.Check<int, int, short>(word + 6 <= (int)meta.WordCount, word, 6, meta.WordCount);
			sc.Buffer->WriteInt32VarLength(word - prev, 4);
			ref Span<int> ptr2 = ref ptr;
			int num = word;
			Vector3 vector = ptr2.Slice(num, ptr2.Length - num).Read<Vector3>();
			ptr2 = ref ptr;
			num = word + 3;
			Quaternion rot = ptr2.Slice(num, ptr2.Length - num).Read<Quaternion>();
			sc.Buffer->WriteInt32VarLength(FloatUtils.Compress(vector.x, 1024), 4);
			sc.Buffer->WriteInt32VarLength(FloatUtils.Compress(vector.y, 1024), 4);
			sc.Buffer->WriteInt32VarLength(FloatUtils.Compress(vector.z, 1024), 4);
			sc.Buffer->WriteUInt32(Maths.QuaternionCompress(rot), 32);
			return word + 6;
		}

		public unsafe override int Skip(Simulation.RecvContext rc, int word)
		{
			rc.Buffer->ReadInt32VarLength(4);
			rc.Buffer->ReadInt32VarLength(4);
			rc.Buffer->ReadInt32VarLength(4);
			rc.Buffer->ReadUInt32(32);
			return word + 6;
		}

		public unsafe override int Read(Simulation.RecvContext rc, NetworkObjectMeta meta, NetworkBufferSerializerInfo info, Span<int> ptr, int word)
		{
			Assert.Check<int, int>(info.Offset == 0, info.Offset, word);
			ref Span<int> ptr2 = ref ptr;
			ref Vector3 ptr3 = ref ptr2.Slice(word, ptr2.Length - word).AsRef<Vector3>();
			ptr2 = ref ptr;
			int num = word + 3;
			ref Quaternion ptr4 = ref ptr2.Slice(num, ptr2.Length - num).AsRef<Quaternion>();
			ptr3.x = FloatUtils.Decompress(rc.Buffer->ReadInt32VarLength(4), 1024f);
			ptr3.y = FloatUtils.Decompress(rc.Buffer->ReadInt32VarLength(4), 1024f);
			ptr3.z = FloatUtils.Decompress(rc.Buffer->ReadInt32VarLength(4), 1024f);
			ptr4 = Maths.QuaternionDecompress(rc.Buffer->ReadUInt32(32));
			ptr4 = ptr4.normalized;
			return word + 6;
		}

		private const int POSITION_ACCURACY = 1024;

		private const int POSITION_BLOCK_SIZE = 4;

		private const int JUMP_OFFSET = 6;

		public static NetworkTransformSerializer Instance = new NetworkTransformSerializer();
	}
}
