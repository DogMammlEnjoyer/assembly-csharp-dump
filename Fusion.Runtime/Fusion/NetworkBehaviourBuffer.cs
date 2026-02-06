using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion
{
	public ref struct NetworkBehaviourBuffer
	{
		public Tick Tick
		{
			get
			{
				return this._tick;
			}
		}

		public int Length
		{
			get
			{
				return this._length;
			}
		}

		public bool Valid
		{
			get
			{
				return this._ptr != null && this._length > 0;
			}
		}

		public unsafe T ReinterpretState<[IsUnmanaged] T>(int offset = 0) where T : struct, ValueType
		{
			Assert.Check(this.Valid);
			Assert.Check<int, int>(offset < this._length, offset, this._length);
			Assert.Check(offset + Native.WordCount(sizeof(T), 4) <= this._length);
			return *(T*)(this._ptr + offset);
		}

		public unsafe int this[int index]
		{
			get
			{
				Assert.Check(index < this._length);
				return this._ptr[index];
			}
		}

		internal unsafe NetworkBehaviourBuffer(Tick tick, int* ptr, int length)
		{
			this._ptr = ptr;
			this._tick = tick;
			this._length = length;
		}

		public T Read<T>(NetworkBehaviour.BehaviourReader<T> reader) where T : NetworkBehaviour
		{
			Assert.Check(this.Valid);
			return reader.Read(this);
		}

		public unsafe T Read<[IsUnmanaged] T>(NetworkBehaviour.PropertyReader<T> reader) where T : struct, ValueType
		{
			Assert.Check(this.Valid);
			Assert.Check(reader.Data.Offset < this._length);
			Assert.Check(reader.Data.Offset + Native.WordCount(sizeof(T), 4) <= this._length);
			return *(T*)(this._ptr + reader.Data.Offset);
		}

		public float Read(NetworkBehaviour.PropertyReader<float> reader)
		{
			Assert.Check(this.Valid);
			Assert.Check(reader.Data.Offset < this._length);
			Assert.Check(reader.Data.Offset + 1 <= this._length);
			return ReadWriteUtils.ReadFloat(this._ptr + reader.Data.Offset);
		}

		public Vector2 Read(NetworkBehaviour.PropertyReader<Vector2> reader)
		{
			Assert.Check(this.Valid);
			Assert.Check(reader.Data.Offset < this._length);
			Assert.Check(reader.Data.Offset + 2 <= this._length);
			return ReadWriteUtils.ReadVector2(this._ptr + reader.Data.Offset);
		}

		public Vector3 Read(NetworkBehaviour.PropertyReader<Vector3> reader)
		{
			Assert.Check(this.Valid);
			Assert.Check(reader.Data.Offset < this._length);
			Assert.Check(reader.Data.Offset + 3 <= this._length);
			return ReadWriteUtils.ReadVector3(this._ptr + reader.Data.Offset);
		}

		public Vector4 Read(NetworkBehaviour.PropertyReader<Vector4> reader)
		{
			Assert.Check(this.Valid);
			Assert.Check(reader.Data.Offset < this._length);
			Assert.Check(reader.Data.Offset + 4 <= this._length);
			return ReadWriteUtils.ReadVector4(this._ptr + reader.Data.Offset);
		}

		public Quaternion Read(NetworkBehaviour.PropertyReader<Quaternion> reader)
		{
			Assert.Check(this.Valid);
			Assert.Check(reader.Data.Offset < this._length);
			Assert.Check(reader.Data.Offset + 4 <= this._length);
			return ReadWriteUtils.ReadQuaternion(this._ptr + reader.Data.Offset);
		}

		public static implicit operator bool(NetworkBehaviourBuffer buffer)
		{
			return buffer.Valid;
		}

		internal unsafe int* _ptr;

		internal int _length;

		internal Tick _tick;
	}
}
