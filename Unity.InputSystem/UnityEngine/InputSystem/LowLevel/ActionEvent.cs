using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 37)]
	internal struct ActionEvent : IInputEventTypeInfo
	{
		public static FourCC Type
		{
			get
			{
				return new FourCC('A', 'C', 'T', 'N');
			}
		}

		public double startTime
		{
			get
			{
				return this.m_StartTime;
			}
			set
			{
				this.m_StartTime = value;
			}
		}

		public InputActionPhase phase
		{
			get
			{
				return (InputActionPhase)this.m_Phase;
			}
			set
			{
				this.m_Phase = (byte)value;
			}
		}

		public unsafe byte* valueData
		{
			get
			{
				fixed (byte* ptr = &this.m_ValueData.FixedElementField)
				{
					return ptr;
				}
			}
		}

		public int valueSizeInBytes
		{
			get
			{
				return (int)(this.baseEvent.sizeInBytes - 20U - 16U);
			}
		}

		public int stateIndex
		{
			get
			{
				return (int)this.m_StateIndex;
			}
			set
			{
				if (value < 0 || value > 255)
				{
					throw new NotSupportedException("State count cannot exceed byte.MaxValue");
				}
				this.m_StateIndex = (byte)value;
			}
		}

		public int controlIndex
		{
			get
			{
				return (int)this.m_ControlIndex;
			}
			set
			{
				if (value < 0 || value > 65535)
				{
					throw new NotSupportedException("Control count cannot exceed ushort.MaxValue");
				}
				this.m_ControlIndex = (ushort)value;
			}
		}

		public int bindingIndex
		{
			get
			{
				return (int)this.m_BindingIndex;
			}
			set
			{
				if (value < 0 || value > 65535)
				{
					throw new NotSupportedException("Binding count cannot exceed ushort.MaxValue");
				}
				this.m_BindingIndex = (ushort)value;
			}
		}

		public int interactionIndex
		{
			get
			{
				if (this.m_InteractionIndex == 65535)
				{
					return -1;
				}
				return (int)this.m_InteractionIndex;
			}
			set
			{
				if (value == -1)
				{
					this.m_InteractionIndex = ushort.MaxValue;
					return;
				}
				if (value < 0 || value >= 65535)
				{
					throw new NotSupportedException("Interaction count cannot exceed ushort.MaxValue-1");
				}
				this.m_InteractionIndex = (ushort)value;
			}
		}

		public unsafe InputEventPtr ToEventPtr()
		{
			fixed (ActionEvent* ptr = &this)
			{
				return new InputEventPtr((InputEvent*)ptr);
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return ActionEvent.Type;
			}
		}

		public static int GetEventSizeWithValueSize(int valueSizeInBytes)
		{
			return 36 + valueSizeInBytes;
		}

		public unsafe static ActionEvent* From(InputEventPtr ptr)
		{
			if (!ptr.valid)
			{
				throw new ArgumentNullException("ptr");
			}
			if (!ptr.IsA<ActionEvent>())
			{
				throw new InvalidCastException(string.Format("Cannot cast event with type '{0}' into ActionEvent", ptr.type));
			}
			return (ActionEvent*)ptr.data;
		}

		[FieldOffset(0)]
		public InputEvent baseEvent;

		[FieldOffset(20)]
		private ushort m_ControlIndex;

		[FieldOffset(22)]
		private ushort m_BindingIndex;

		[FieldOffset(24)]
		private ushort m_InteractionIndex;

		[FieldOffset(26)]
		private byte m_StateIndex;

		[FieldOffset(27)]
		private byte m_Phase;

		[FieldOffset(28)]
		private double m_StartTime;

		[FixedBuffer(typeof(byte), 1)]
		[FieldOffset(36)]
		public ActionEvent.<m_ValueData>e__FixedBuffer m_ValueData;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct <m_ValueData>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
