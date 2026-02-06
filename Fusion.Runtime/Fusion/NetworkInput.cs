using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	public struct NetworkInput
	{
		public int WordCount
		{
			get
			{
				return this._wordCount;
			}
		}

		public unsafe uint* Data
		{
			get
			{
				return (this._ptr == null) ? null : (this._ptr + 1);
			}
		}

		public bool IsValid
		{
			get
			{
				return this._ptr != null;
			}
		}

		internal unsafe uint* Ptr
		{
			get
			{
				return this._ptr;
			}
		}

		internal unsafe int TypeKey
		{
			get
			{
				return (int)((this._ptr == null) ? uint.MaxValue : (*this._ptr));
			}
			set
			{
				bool flag = this._ptr != null;
				if (flag)
				{
					*this._ptr = (uint)value;
				}
			}
		}

		public Type Type
		{
			get
			{
				int typeKey = this.TypeKey;
				bool flag = typeKey == -1;
				Type result;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = NetworkInputUtils.GetType(typeKey);
				}
				return result;
			}
		}

		internal unsafe static NetworkInput FromRaw(uint* ptr, int wordCount)
		{
			return new NetworkInput
			{
				_ptr = ptr,
				_wordCount = wordCount
			};
		}

		internal unsafe static NetworkInput FromRaw(int* ptr, int wordCount)
		{
			return new NetworkInput
			{
				_ptr = (uint*)ptr,
				_wordCount = wordCount
			};
		}

		public unsafe bool TryGet<[IsUnmanaged] T>(out T input) where T : struct, ValueType, INetworkInput
		{
			Assert.Check(this.IsValid);
			bool flag = this._ptr == null || this.TypeKey != NetworkInputUtils.GetTypeKey(typeof(T));
			bool result;
			if (flag)
			{
				input = default(T);
				result = false;
			}
			else
			{
				input = *(T*)this.Data;
				result = true;
			}
			return result;
		}

		public unsafe bool TrySet<[IsUnmanaged] T>(T input) where T : struct, ValueType, INetworkInput
		{
			Assert.Check(this.IsValid);
			bool flag = this._ptr == null || this.TypeKey != NetworkInputUtils.GetTypeKey(typeof(T));
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				*(T*)this.Data = input;
				result = true;
			}
			return result;
		}

		public unsafe T Get<[IsUnmanaged] T>() where T : struct, ValueType, INetworkInput
		{
			Assert.Check(this.IsValid);
			this.Convert<T>();
			return *(T*)this.Data;
		}

		public unsafe bool Set<[IsUnmanaged] T>(T value) where T : struct, ValueType, INetworkInput
		{
			Assert.Check(this.IsValid);
			bool result = this.Convert<T>();
			*(T*)this.Data = value;
			return result;
		}

		internal unsafe bool Set(Type type, void* value)
		{
			Assert.Check(this.IsValid);
			int wordCount = NetworkInputUtils.GetWordCount(type);
			bool flag = wordCount >= this._wordCount;
			if (flag)
			{
				throw new ArgumentException(string.Format("Expected max {0}, got: {1}", this._wordCount, wordCount), "type");
			}
			bool flag2 = this.Convert(type);
			Native.MemCpy((void*)this.Data, value, wordCount * 4);
			return true;
		}

		public bool Convert<[IsUnmanaged] T>() where T : struct, ValueType, INetworkInput
		{
			return this.Convert(typeof(T));
		}

		public unsafe bool Convert(Type type)
		{
			Assert.Check(this.IsValid);
			int typeKey = NetworkInputUtils.GetTypeKey(type);
			bool flag = typeKey != this.TypeKey;
			bool result;
			if (flag)
			{
				Native.MemClear((void*)this._ptr, this._wordCount * 4);
				this.TypeKey = typeKey;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool Is<[IsUnmanaged] T>() where T : struct, ValueType, INetworkInput
		{
			Assert.Check(this.IsValid);
			return this.TypeKey == NetworkInputUtils.GetTypeKey(typeof(T));
		}

		private unsafe uint* _ptr;

		private int _wordCount;
	}
}
