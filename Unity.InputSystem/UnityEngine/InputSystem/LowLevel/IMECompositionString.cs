using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 132)]
	public struct IMECompositionString : IEnumerable<char>, IEnumerable
	{
		public int Count
		{
			get
			{
				return this.size;
			}
		}

		public unsafe char this[int index]
		{
			get
			{
				if (index >= this.Count || index < 0)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				fixed (char* ptr = &this.buffer.FixedElementField)
				{
					return ptr[index];
				}
			}
		}

		public unsafe IMECompositionString(string characters)
		{
			if (string.IsNullOrEmpty(characters))
			{
				this.size = 0;
				return;
			}
			this.size = characters.Length;
			for (int i = 0; i < this.size; i++)
			{
				*(ref this.buffer.FixedElementField + (IntPtr)i * 2) = characters[i];
			}
		}

		public unsafe override string ToString()
		{
			fixed (char* ptr = &this.buffer.FixedElementField)
			{
				return new string(ptr, 0, this.size);
			}
		}

		public IEnumerator<char> GetEnumerator()
		{
			return new IMECompositionString.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		[FieldOffset(0)]
		private int size;

		[FixedBuffer(typeof(char), 64)]
		[FieldOffset(4)]
		private IMECompositionString.<buffer>e__FixedBuffer buffer;

		internal struct Enumerator : IEnumerator<char>, IEnumerator, IDisposable
		{
			public Enumerator(IMECompositionString compositionString)
			{
				this.m_CompositionString = compositionString;
				this.m_CurrentCharacter = '\0';
				this.m_CurrentIndex = -1;
			}

			public unsafe bool MoveNext()
			{
				int count = this.m_CompositionString.Count;
				this.m_CurrentIndex++;
				if (this.m_CurrentIndex == count)
				{
					return false;
				}
				fixed (char* ptr = &this.m_CompositionString.buffer.FixedElementField)
				{
					char* ptr2 = ptr;
					this.m_CurrentCharacter = ptr2[this.m_CurrentIndex];
				}
				return true;
			}

			public void Reset()
			{
				this.m_CurrentIndex = -1;
			}

			public void Dispose()
			{
			}

			public char Current
			{
				get
				{
					return this.m_CurrentCharacter;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			private IMECompositionString m_CompositionString;

			private char m_CurrentCharacter;

			private int m_CurrentIndex;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 128)]
		public struct <buffer>e__FixedBuffer
		{
			public char FixedElementField;
		}
	}
}
