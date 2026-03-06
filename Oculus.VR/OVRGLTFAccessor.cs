using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using OVRSimpleJSON;
using UnityEngine;

public class OVRGLTFAccessor : IDisposable
{
	public static bool TryCreate(JSONNode accessorsRoot, JSONNode bufferViewsRoot, JSONNode buffersRoot, Stream binaryChunk, out OVRGLTFAccessor dataAccessor)
	{
		BinaryReader binaryReader = new BinaryReader(binaryChunk, Encoding.UTF8, true);
		uint binaryChunkLength = binaryReader.ReadUInt32();
		if (binaryReader.ReadUInt32() != 5130562U)
		{
			Debug.LogError("Read chunk does not match type.");
			dataAccessor = null;
			return false;
		}
		dataAccessor = new OVRGLTFAccessor(accessorsRoot, bufferViewsRoot, buffersRoot, binaryReader, (int)binaryChunk.Position, (int)binaryChunkLength);
		return true;
	}

	private OVRGLTFAccessor(JSONNode accessorsRoot, JSONNode bufferViewsRoot, JSONNode buffersRoot, BinaryReader binaryChunkReader, int binaryChinkStart, int binaryChunkLength)
	{
		this._reader = binaryChunkReader;
		this._binaryChunk = binaryChunkReader.BaseStream;
		this._binaryChunkLength = binaryChunkLength;
		this._binaryChunkStart = binaryChinkStart;
		foreach (JSONNode jsonnode in accessorsRoot.Children)
		{
			OVRGLTFAccessor.GLTFAccessor gltfaccessor = default(OVRGLTFAccessor.GLTFAccessor);
			foreach (KeyValuePair<string, JSONNode> keyValuePair in jsonnode)
			{
				string key = keyValuePair.Key;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(key);
				if (num <= 1852717166U)
				{
					if (num <= 1361572173U)
					{
						if (num != 967958004U)
						{
							if (num == 1361572173U)
							{
								if (key == "type")
								{
									gltfaccessor.Type = OVRGLTFAccessor.ToOVRType(keyValuePair.Value.Value);
								}
							}
						}
						else if (key == "count")
						{
							gltfaccessor.Count = keyValuePair.Value.AsInt;
						}
					}
					else if (num != 1578195204U)
					{
						if (num == 1852717166U)
						{
							if (key == "bufferView")
							{
								gltfaccessor.BufferViewIndex = keyValuePair.Value.AsInt;
							}
						}
					}
					else if (key == "byteOffset")
					{
						gltfaccessor.ByteOffset = keyValuePair.Value.AsInt;
					}
				}
				else if (num <= 2878967080U)
				{
					if (num != 1984302117U)
					{
						if (num == 2878967080U)
						{
							if (key == "componentType")
							{
								gltfaccessor.ComponentType = (OVRGLTFComponentType)keyValuePair.Value.AsInt;
								gltfaccessor.ComponentTypeStride = this.GetStrideForType(gltfaccessor.ComponentType);
							}
						}
					}
					else if (key == "sparse")
					{
						Debug.LogWarning("Sparse accessors unsupported");
					}
				}
				else if (num != 3381609815U)
				{
					if (num == 3617776409U)
					{
						if (key == "max")
						{
							gltfaccessor.Max = keyValuePair.Value;
						}
					}
				}
				else if (key == "min")
				{
					gltfaccessor.Min = keyValuePair.Value;
				}
			}
			this._accessors.Add(gltfaccessor);
		}
		foreach (JSONNode jsonnode2 in bufferViewsRoot.Children)
		{
			OVRGLTFAccessor.GLTFBufferView item = default(OVRGLTFAccessor.GLTFBufferView);
			foreach (KeyValuePair<string, JSONNode> keyValuePair2 in jsonnode2)
			{
				string key = keyValuePair2.Key;
				if (!(key == "bufferIndex"))
				{
					if (!(key == "byteOffset"))
					{
						if (!(key == "byteLength"))
						{
							if (key == "byteStride")
							{
								item.ByteStride = keyValuePair2.Value.AsInt;
							}
						}
						else
						{
							item.ByteLength = keyValuePair2.Value.AsInt;
						}
					}
					else
					{
						item.ByteOffset = keyValuePair2.Value.AsInt;
					}
				}
				else
				{
					item.BufferIndex = keyValuePair2.Value.AsInt;
				}
			}
			this._bufferViews.Add(item);
		}
		foreach (JSONNode jsonnode3 in buffersRoot.Children)
		{
			OVRGLTFAccessor.GLTFBuffer item2 = default(OVRGLTFAccessor.GLTFBuffer);
			foreach (KeyValuePair<string, JSONNode> keyValuePair3 in jsonnode3)
			{
				if (keyValuePair3.Key == "byteLength")
				{
					item2.ByteLength = keyValuePair3.Value.AsInt;
				}
			}
			this._buffers.Add(item2);
		}
	}

	private static OVRGLTFType ToOVRType(string type)
	{
		if (type == "SCALAR")
		{
			return OVRGLTFType.SCALAR;
		}
		if (type == "VEC2")
		{
			return OVRGLTFType.VEC2;
		}
		if (type == "VEC3")
		{
			return OVRGLTFType.VEC3;
		}
		if (type == "VEC4")
		{
			return OVRGLTFType.VEC4;
		}
		if (!(type == "MAT4"))
		{
			Debug.LogError("Unsupported accessor type.");
			return OVRGLTFType.NONE;
		}
		return OVRGLTFType.MAT4;
	}

	public void Seek(int accessorIndex, bool onlyBufferView = false)
	{
		if (accessorIndex >= this._accessors.Count)
		{
			return;
		}
		this._activeGltfAccessor = this._accessors[accessorIndex];
		this._activeBufferView = this._bufferViews[this._activeGltfAccessor.BufferViewIndex];
		this._activeBuffer = this._buffers[this._activeBufferView.BufferIndex];
		this._requireStrideSeek = (this._activeBufferView.ByteStride != 0 && this._activeBufferView.ByteStride != this._activeGltfAccessor.ComponentTypeStride);
		if (this._binaryChunkLength != this._activeBuffer.ByteLength)
		{
			Debug.LogError("Chunk length is not equal to buffer length.");
			return;
		}
		this._activeBufferOffset = this._binaryChunkStart + this._activeBufferView.ByteOffset;
		if (!onlyBufferView)
		{
			this._activeBufferOffset += this._activeGltfAccessor.ByteOffset;
		}
		this._binaryChunk.Seek((long)this._activeBufferOffset, SeekOrigin.Begin);
	}

	private void SeekStride(int strideIndex)
	{
		if (!this._requireStrideSeek || strideIndex == 0)
		{
			return;
		}
		if (strideIndex >= this._activeGltfAccessor.Count)
		{
			Debug.LogError("Invalid seek index for data");
			return;
		}
		int byteStride = this._activeBufferView.ByteStride;
		this._binaryChunk.Seek((long)(this._activeBufferOffset + byteStride * strideIndex), SeekOrigin.Begin);
	}

	public float[] ReadFloat()
	{
		float[] array = new float[this._activeGltfAccessor.Count];
		if (this._activeGltfAccessor.ComponentType == OVRGLTFComponentType.FLOAT)
		{
			this._binaryChunk.Read(MemoryMarshal.AsBytes<float>(array.AsSpan<float>()));
		}
		else
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = OVRGLTFAccessor.ReadAsFloat(this._reader, this._activeGltfAccessor.ComponentType);
			}
		}
		return array;
	}

	public int[] ReadInt()
	{
		int[] array = new int[this._activeGltfAccessor.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = OVRGLTFAccessor.ReadAsInt(this._reader, this._activeGltfAccessor.ComponentType);
		}
		return array;
	}

	public Vector2[] ReadVector2()
	{
		Vector2[] array = new Vector2[this._activeGltfAccessor.Count];
		if (!this._requireStrideSeek && this._activeGltfAccessor.ComponentType == OVRGLTFComponentType.FLOAT)
		{
			this._binaryChunk.Read(MemoryMarshal.AsBytes<Vector2>(array.AsSpan<Vector2>()));
		}
		else
		{
			for (int i = 0; i < array.Length; i++)
			{
				this.SeekStride(i);
				array[i].x = OVRGLTFAccessor.ReadAsFloat(this._reader, this._activeGltfAccessor.ComponentType);
				array[i].y = OVRGLTFAccessor.ReadAsFloat(this._reader, this._activeGltfAccessor.ComponentType);
			}
		}
		return array;
	}

	public Vector3[] ReadVector3(Vector3 conversionScale)
	{
		Vector3[] array = new Vector3[this._activeGltfAccessor.Count];
		if (!this._requireStrideSeek && this._activeGltfAccessor.ComponentType == OVRGLTFComponentType.FLOAT)
		{
			this._binaryChunk.Read(MemoryMarshal.AsBytes<Vector3>(array.AsSpan<Vector3>()));
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Scale(conversionScale);
			}
		}
		else
		{
			for (int j = 0; j < array.Length; j++)
			{
				this.SeekStride(j);
				array[j].x = OVRGLTFAccessor.ReadAsFloat(this._reader, this._activeGltfAccessor.ComponentType);
				array[j].y = OVRGLTFAccessor.ReadAsFloat(this._reader, this._activeGltfAccessor.ComponentType);
				array[j].z = OVRGLTFAccessor.ReadAsFloat(this._reader, this._activeGltfAccessor.ComponentType);
				array[j].Scale(conversionScale);
			}
		}
		return array;
	}

	public Vector4[] ReadVector4(Vector4 conversionScale)
	{
		Vector4[] array = new Vector4[this._activeGltfAccessor.Count];
		if (!this._requireStrideSeek && this._activeGltfAccessor.ComponentType == OVRGLTFComponentType.FLOAT)
		{
			this._binaryChunk.Read(MemoryMarshal.AsBytes<Vector4>(array.AsSpan<Vector4>()));
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Scale(conversionScale);
			}
		}
		else
		{
			for (int j = 0; j < array.Length; j++)
			{
				this.SeekStride(j);
				array[j].x = OVRGLTFAccessor.ReadAsFloat(this._reader, this._activeGltfAccessor.ComponentType);
				array[j].y = OVRGLTFAccessor.ReadAsFloat(this._reader, this._activeGltfAccessor.ComponentType);
				array[j].z = OVRGLTFAccessor.ReadAsFloat(this._reader, this._activeGltfAccessor.ComponentType);
				array[j].w = OVRGLTFAccessor.ReadAsFloat(this._reader, this._activeGltfAccessor.ComponentType);
				array[j].Scale(conversionScale);
			}
		}
		return array;
	}

	private static int ReadAsInt(BinaryReader reader, OVRGLTFComponentType type)
	{
		if (type != OVRGLTFComponentType.NONE)
		{
			switch (type)
			{
			case OVRGLTFComponentType.BYTE:
				return (int)reader.ReadSByte();
			case OVRGLTFComponentType.UNSIGNED_BYTE:
				return (int)reader.ReadByte();
			case OVRGLTFComponentType.SHORT:
				return (int)reader.ReadInt16();
			case OVRGLTFComponentType.UNSIGNED_SHORT:
				return (int)reader.ReadUInt16();
			case OVRGLTFComponentType.UNSIGNED_INT:
				return (int)reader.ReadUInt32();
			case OVRGLTFComponentType.FLOAT:
				return (int)reader.ReadSingle();
			}
			throw new ArgumentOutOfRangeException("type", type, null);
		}
		return 0;
	}

	private static float ReadAsFloat(BinaryReader reader, OVRGLTFComponentType type)
	{
		if (type != OVRGLTFComponentType.NONE)
		{
			switch (type)
			{
			case OVRGLTFComponentType.BYTE:
				return (float)reader.ReadSByte();
			case OVRGLTFComponentType.UNSIGNED_BYTE:
				return (float)reader.ReadByte();
			case OVRGLTFComponentType.SHORT:
				return (float)reader.ReadInt16();
			case OVRGLTFComponentType.UNSIGNED_SHORT:
				return (float)reader.ReadUInt16();
			case OVRGLTFComponentType.UNSIGNED_INT:
				return reader.ReadUInt32();
			case OVRGLTFComponentType.FLOAT:
				return reader.ReadSingle();
			}
			throw new ArgumentOutOfRangeException("type", type, null);
		}
		return 0f;
	}

	public Color[] ReadColor()
	{
		if (this._activeGltfAccessor.Type != OVRGLTFType.VEC4 && this._activeGltfAccessor.Type != OVRGLTFType.VEC3)
		{
			Debug.LogError("Tried to read non-color type as a color array." + this._activeGltfAccessor.Type.ToString());
			return Array.Empty<Color>();
		}
		Color[] array = new Color[this._activeGltfAccessor.Count];
		if (!this._requireStrideSeek && this._activeGltfAccessor.ComponentType == OVRGLTFComponentType.FLOAT && this._activeGltfAccessor.Type == OVRGLTFType.VEC4)
		{
			this._binaryChunk.Read(MemoryMarshal.AsBytes<Color>(array.AsSpan<Color>()));
		}
		else
		{
			for (int i = 0; i < array.Length; i++)
			{
				this.SeekStride(i);
				if (this._activeGltfAccessor.ComponentType == OVRGLTFComponentType.FLOAT)
				{
					array[i].r = this._reader.ReadSingle();
					array[i].g = this._reader.ReadSingle();
					array[i].b = this._reader.ReadSingle();
					array[i].a = ((this._activeGltfAccessor.Type == OVRGLTFType.VEC4) ? this._reader.ReadSingle() : 1f);
				}
				else
				{
					float maxValueForType = this.GetMaxValueForType(this._activeGltfAccessor.ComponentType);
					array[i].r = (float)OVRGLTFAccessor.ReadAsInt(this._reader, this._activeGltfAccessor.ComponentType) / maxValueForType;
					array[i].g = (float)OVRGLTFAccessor.ReadAsInt(this._reader, this._activeGltfAccessor.ComponentType) / maxValueForType;
					array[i].b = (float)OVRGLTFAccessor.ReadAsInt(this._reader, this._activeGltfAccessor.ComponentType) / maxValueForType;
					array[i].a = ((this._activeGltfAccessor.Type == OVRGLTFType.VEC4) ? ((float)OVRGLTFAccessor.ReadAsInt(this._reader, this._activeGltfAccessor.ComponentType) / maxValueForType) : 1f);
				}
			}
		}
		return array;
	}

	public void ReadWeights(ref BoneWeight[] resultsBoneWeights)
	{
		if (this._activeGltfAccessor.Type != OVRGLTFType.VEC4)
		{
			Debug.LogError("Tried to read bone weights data as a non-vec4 array.");
			return;
		}
		if (resultsBoneWeights == null)
		{
			resultsBoneWeights = new BoneWeight[this._activeGltfAccessor.Count];
		}
		for (int i = 0; i < resultsBoneWeights.Length; i++)
		{
			this.SeekStride(i);
			resultsBoneWeights[i].weight0 = this._reader.ReadSingle();
			resultsBoneWeights[i].weight1 = this._reader.ReadSingle();
			resultsBoneWeights[i].weight2 = this._reader.ReadSingle();
			resultsBoneWeights[i].weight3 = this._reader.ReadSingle();
			float num = resultsBoneWeights[i].weight0 + resultsBoneWeights[i].weight1 + resultsBoneWeights[i].weight2 + resultsBoneWeights[i].weight3;
			if (!Mathf.Approximately(num, 0f))
			{
				BoneWeight[] array = resultsBoneWeights;
				int num2 = i;
				array[num2].weight0 = array[num2].weight0 / num;
				BoneWeight[] array2 = resultsBoneWeights;
				int num3 = i;
				array2[num3].weight1 = array2[num3].weight1 / num;
				BoneWeight[] array3 = resultsBoneWeights;
				int num4 = i;
				array3[num4].weight2 = array3[num4].weight2 / num;
				BoneWeight[] array4 = resultsBoneWeights;
				int num5 = i;
				array4[num5].weight3 = array4[num5].weight3 / num;
			}
		}
	}

	public void ReadJoints(ref BoneWeight[] resultsBoneWeights)
	{
		if (this._activeGltfAccessor.Type != OVRGLTFType.VEC4)
		{
			Debug.LogError("Tried to read bone weights data as a non-vec4 array.");
			return;
		}
		if (resultsBoneWeights == null)
		{
			resultsBoneWeights = new BoneWeight[this._activeGltfAccessor.Count];
		}
		for (int i = 0; i < resultsBoneWeights.Length; i++)
		{
			this.SeekStride(i);
			resultsBoneWeights[i].boneIndex0 = OVRGLTFAccessor.ReadAsInt(this._reader, this._activeGltfAccessor.ComponentType);
			resultsBoneWeights[i].boneIndex1 = OVRGLTFAccessor.ReadAsInt(this._reader, this._activeGltfAccessor.ComponentType);
			resultsBoneWeights[i].boneIndex2 = OVRGLTFAccessor.ReadAsInt(this._reader, this._activeGltfAccessor.ComponentType);
			resultsBoneWeights[i].boneIndex3 = OVRGLTFAccessor.ReadAsInt(this._reader, this._activeGltfAccessor.ComponentType);
		}
	}

	public Quaternion[] ReadQuaterion(Vector4 gltfToUnitySpaceRotation)
	{
		if (this._activeGltfAccessor.Type != OVRGLTFType.VEC4)
		{
			Debug.LogError("Tried to read bone weights data as a non-vec4 array.");
			return Array.Empty<Quaternion>();
		}
		Quaternion[] array = new Quaternion[this._activeGltfAccessor.Count];
		if (!this._requireStrideSeek && this._activeGltfAccessor.ComponentType == OVRGLTFComponentType.FLOAT)
		{
			this._binaryChunk.Read(MemoryMarshal.AsBytes<Quaternion>(array.AsSpan<Quaternion>()));
			for (int i = 0; i < array.Length; i++)
			{
				Quaternion[] array2 = array;
				int num = i;
				array2[num].x = array2[num].x * gltfToUnitySpaceRotation.x;
				Quaternion[] array3 = array;
				int num2 = i;
				array3[num2].y = array3[num2].y * gltfToUnitySpaceRotation.y;
				Quaternion[] array4 = array;
				int num3 = i;
				array4[num3].z = array4[num3].z * gltfToUnitySpaceRotation.z;
				Quaternion[] array5 = array;
				int num4 = i;
				array5[num4].w = array5[num4].w * gltfToUnitySpaceRotation.w;
			}
		}
		else
		{
			for (int j = 0; j < array.Length; j++)
			{
				this.SeekStride(j);
				array[j].x = OVRGLTFAccessor.ReadAsFloat(this._reader, this._activeGltfAccessor.ComponentType) * gltfToUnitySpaceRotation.x;
				array[j].y = OVRGLTFAccessor.ReadAsFloat(this._reader, this._activeGltfAccessor.ComponentType) * gltfToUnitySpaceRotation.y;
				array[j].z = OVRGLTFAccessor.ReadAsFloat(this._reader, this._activeGltfAccessor.ComponentType) * gltfToUnitySpaceRotation.z;
				array[j].w = OVRGLTFAccessor.ReadAsFloat(this._reader, this._activeGltfAccessor.ComponentType) * gltfToUnitySpaceRotation.w;
			}
		}
		return array;
	}

	public Matrix4x4[] ReadMatrix4x4(Vector3 conversionScale)
	{
		if (this._activeGltfAccessor.Type != OVRGLTFType.MAT4)
		{
			Debug.LogError("Tried to read non-vec3 data as a vec3 array.");
			return Array.Empty<Matrix4x4>();
		}
		Matrix4x4 matrix4x = Matrix4x4.Scale(conversionScale);
		Matrix4x4[] array = new Matrix4x4[this._activeGltfAccessor.Count];
		if (!this._requireStrideSeek && this._activeGltfAccessor.ComponentType == OVRGLTFComponentType.FLOAT)
		{
			this._binaryChunk.Read(MemoryMarshal.AsBytes<Matrix4x4>(array.AsSpan<Matrix4x4>()));
			for (int i = 0; i < this._activeGltfAccessor.Count; i++)
			{
				array[i] = matrix4x * array[i] * matrix4x;
			}
		}
		else
		{
			for (int j = 0; j < this._activeGltfAccessor.Count; j++)
			{
				this.SeekStride(j);
				for (int k = 0; k < 16; k++)
				{
					array[j][k] = OVRGLTFAccessor.ReadAsFloat(this._reader, this._activeGltfAccessor.ComponentType);
				}
				array[j] = matrix4x * array[j] * matrix4x;
			}
		}
		return array;
	}

	private int GetStrideForType(OVRGLTFComponentType type)
	{
		switch (type)
		{
		case OVRGLTFComponentType.BYTE:
			return 1;
		case OVRGLTFComponentType.UNSIGNED_BYTE:
			return 1;
		case OVRGLTFComponentType.SHORT:
			return 2;
		case OVRGLTFComponentType.UNSIGNED_SHORT:
			return 2;
		case OVRGLTFComponentType.UNSIGNED_INT:
			return 4;
		case OVRGLTFComponentType.FLOAT:
			return 4;
		}
		Debug.LogWarning("GetStrideForType called with unsupported component type " + type.ToString());
		return 0;
	}

	private float GetMaxValueForType(OVRGLTFComponentType type)
	{
		switch (type)
		{
		case OVRGLTFComponentType.BYTE:
			return 127f;
		case OVRGLTFComponentType.UNSIGNED_BYTE:
			return 255f;
		case OVRGLTFComponentType.SHORT:
			return 32767f;
		case OVRGLTFComponentType.UNSIGNED_SHORT:
			return 65535f;
		case OVRGLTFComponentType.UNSIGNED_INT:
			return 4.2949673E+09f;
		case OVRGLTFComponentType.FLOAT:
			return float.MaxValue;
		}
		Debug.LogWarning("GetMaxValueForType called with unsupported component type " + type.ToString());
		return 1f;
	}

	public byte[] ReadBuffer(int bufferViewIndex)
	{
		this._activeBufferView = this._bufferViews[bufferViewIndex];
		this._activeBuffer = this._buffers[this._activeBufferView.BufferIndex];
		this._binaryChunk.Seek((long)this._binaryChunkStart, SeekOrigin.Begin);
		this._binaryChunk.Seek((long)this._activeBufferView.ByteOffset, SeekOrigin.Current);
		return this._reader.ReadBytes(this._activeBufferView.ByteLength);
	}

	public void Dispose()
	{
		this._reader.Dispose();
	}

	public int GetDataCount()
	{
		return this._activeGltfAccessor.Count;
	}

	private readonly List<OVRGLTFAccessor.GLTFAccessor> _accessors = new List<OVRGLTFAccessor.GLTFAccessor>();

	private readonly List<OVRGLTFAccessor.GLTFBufferView> _bufferViews = new List<OVRGLTFAccessor.GLTFBufferView>();

	private readonly List<OVRGLTFAccessor.GLTFBuffer> _buffers = new List<OVRGLTFAccessor.GLTFBuffer>();

	private readonly Stream _binaryChunk;

	private readonly int _binaryChunkLength;

	private readonly int _binaryChunkStart;

	private readonly BinaryReader _reader;

	private OVRGLTFAccessor.GLTFAccessor _activeGltfAccessor;

	private OVRGLTFAccessor.GLTFBufferView _activeBufferView;

	private OVRGLTFAccessor.GLTFBuffer _activeBuffer;

	private int _activeBufferOffset;

	private bool _requireStrideSeek;

	private struct GLTFAccessor
	{
		public OVRGLTFType Type;

		public OVRGLTFComponentType ComponentType;

		public int ComponentTypeStride;

		public int BufferViewIndex;

		public int ByteOffset;

		public int Count;

		public JSONNode Min;

		public JSONNode Max;
	}

	private struct GLTFBufferView
	{
		public int BufferIndex;

		public int ByteOffset;

		public int ByteLength;

		public int ByteStride;
	}

	private struct GLTFBuffer
	{
		public int ByteLength;
	}
}
