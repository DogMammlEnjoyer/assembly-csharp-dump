using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 56)]
	public struct TouchState : IInputStateTypeInfo
	{
		public static FourCC Format
		{
			get
			{
				return new FourCC('T', 'O', 'U', 'C');
			}
		}

		public TouchPhase phase
		{
			get
			{
				return (TouchPhase)this.phaseId;
			}
			set
			{
				this.phaseId = (byte)value;
			}
		}

		public bool isNoneEndedOrCanceled
		{
			get
			{
				return this.phase == TouchPhase.None || this.phase == TouchPhase.Ended || this.phase == TouchPhase.Canceled;
			}
		}

		public bool isInProgress
		{
			get
			{
				return this.phase == TouchPhase.Began || this.phase == TouchPhase.Moved || this.phase == TouchPhase.Stationary;
			}
		}

		public bool isPrimaryTouch
		{
			get
			{
				return (this.flags & 8) > 0;
			}
			set
			{
				if (value)
				{
					this.flags |= 8;
					return;
				}
				this.flags &= 247;
			}
		}

		internal bool isOrphanedPrimaryTouch
		{
			get
			{
				return (this.flags & 64) > 0;
			}
			set
			{
				if (value)
				{
					this.flags |= 64;
					return;
				}
				this.flags &= 191;
			}
		}

		public bool isIndirectTouch
		{
			get
			{
				return (this.flags & 1) > 0;
			}
			set
			{
				if (value)
				{
					this.flags |= 1;
					return;
				}
				this.flags &= 254;
			}
		}

		public bool isTap
		{
			get
			{
				return this.isTapPress;
			}
			set
			{
				this.isTapPress = value;
			}
		}

		internal bool isTapPress
		{
			get
			{
				return (this.flags & 16) > 0;
			}
			set
			{
				if (value)
				{
					this.flags |= 16;
					return;
				}
				this.flags &= 239;
			}
		}

		internal bool isTapRelease
		{
			get
			{
				return (this.flags & 32) > 0;
			}
			set
			{
				if (value)
				{
					this.flags |= 32;
					return;
				}
				this.flags &= 223;
			}
		}

		internal bool beganInSameFrame
		{
			get
			{
				return (this.flags & 128) > 0;
			}
			set
			{
				if (value)
				{
					this.flags |= 128;
					return;
				}
				this.flags &= 127;
			}
		}

		public FourCC format
		{
			get
			{
				return TouchState.Format;
			}
		}

		public override string ToString()
		{
			return string.Format("{{ id={0} phase={1} pos={2} delta={3} pressure={4} radius={5} primary={6} }}", new object[]
			{
				this.touchId,
				this.phase,
				this.position,
				this.delta,
				this.pressure,
				this.radius,
				this.isPrimaryTouch
			});
		}

		internal const int kSizeInBytes = 56;

		[InputControl(displayName = "Touch ID", layout = "Integer", synthetic = true, dontReset = true)]
		[FieldOffset(0)]
		public int touchId;

		[InputControl(displayName = "Position", dontReset = true)]
		[FieldOffset(4)]
		public Vector2 position;

		[InputControl(displayName = "Delta", layout = "Delta")]
		[FieldOffset(12)]
		public Vector2 delta;

		[InputControl(displayName = "Pressure", layout = "Axis")]
		[FieldOffset(20)]
		public float pressure;

		[InputControl(displayName = "Radius")]
		[FieldOffset(24)]
		public Vector2 radius;

		[InputControl(name = "phase", displayName = "Touch Phase", layout = "TouchPhase", synthetic = true)]
		[InputControl(name = "press", displayName = "Touch Contact?", layout = "TouchPress", useStateFrom = "phase")]
		[FieldOffset(32)]
		public byte phaseId;

		[InputControl(name = "tapCount", displayName = "Tap Count", layout = "Integer")]
		[FieldOffset(33)]
		public byte tapCount;

		[InputControl(name = "displayIndex", displayName = "Display Index", layout = "Integer")]
		[FieldOffset(34)]
		public byte displayIndex;

		[InputControl(name = "indirectTouch", displayName = "Indirect Touch?", layout = "Button", bit = 0U, synthetic = true)]
		[InputControl(name = "tap", displayName = "Tap", layout = "Button", bit = 4U)]
		[FieldOffset(35)]
		public byte flags;

		[FieldOffset(36)]
		internal uint updateStepCount;

		[InputControl(displayName = "Start Time", layout = "Double", synthetic = true)]
		[FieldOffset(40)]
		public double startTime;

		[InputControl(displayName = "Start Position", synthetic = true)]
		[FieldOffset(48)]
		public Vector2 startPosition;
	}
}
