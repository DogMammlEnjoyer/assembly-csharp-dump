using System;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	public struct InputEventPtr : IEquatable<InputEventPtr>
	{
		public unsafe InputEventPtr(InputEvent* eventPtr)
		{
			this.m_EventPtr = eventPtr;
		}

		public bool valid
		{
			get
			{
				return this.m_EventPtr != null;
			}
		}

		public unsafe bool handled
		{
			get
			{
				return this.valid && this.m_EventPtr->handled;
			}
			set
			{
				if (!this.valid)
				{
					throw new InvalidOperationException("The InputEventPtr is not valid.");
				}
				this.m_EventPtr->handled = value;
			}
		}

		public unsafe int id
		{
			get
			{
				if (!this.valid)
				{
					return 0;
				}
				return this.m_EventPtr->eventId;
			}
			set
			{
				if (!this.valid)
				{
					throw new InvalidOperationException("The InputEventPtr is not valid.");
				}
				this.m_EventPtr->eventId = value;
			}
		}

		public unsafe FourCC type
		{
			get
			{
				if (!this.valid)
				{
					return default(FourCC);
				}
				return this.m_EventPtr->type;
			}
		}

		public unsafe uint sizeInBytes
		{
			get
			{
				if (!this.valid)
				{
					return 0U;
				}
				return this.m_EventPtr->sizeInBytes;
			}
		}

		public unsafe int deviceId
		{
			get
			{
				if (!this.valid)
				{
					return 0;
				}
				return this.m_EventPtr->deviceId;
			}
			set
			{
				if (!this.valid)
				{
					throw new InvalidOperationException("The InputEventPtr is not valid.");
				}
				this.m_EventPtr->deviceId = value;
			}
		}

		public unsafe double time
		{
			get
			{
				if (!this.valid)
				{
					return 0.0;
				}
				return this.m_EventPtr->time;
			}
			set
			{
				if (!this.valid)
				{
					throw new InvalidOperationException("The InputEventPtr is not valid.");
				}
				this.m_EventPtr->time = value;
			}
		}

		internal unsafe double internalTime
		{
			get
			{
				if (!this.valid)
				{
					return 0.0;
				}
				return this.m_EventPtr->internalTime;
			}
			set
			{
				if (!this.valid)
				{
					throw new InvalidOperationException("The InputEventPtr is not valid.");
				}
				this.m_EventPtr->internalTime = value;
			}
		}

		public unsafe InputEvent* data
		{
			get
			{
				return this.m_EventPtr;
			}
		}

		internal unsafe FourCC stateFormat
		{
			get
			{
				FourCC type = this.type;
				if (type == 1398030676)
				{
					return StateEvent.FromUnchecked(this)->stateFormat;
				}
				if (type == 1145852993)
				{
					return DeltaStateEvent.FromUnchecked(this)->stateFormat;
				}
				string str = "Event must be a StateEvent or DeltaStateEvent but is ";
				InputEventPtr inputEventPtr = this;
				throw new InvalidOperationException(str + inputEventPtr.ToString());
			}
		}

		internal unsafe uint stateSizeInBytes
		{
			get
			{
				if (this.IsA<StateEvent>())
				{
					return StateEvent.From(this)->stateSizeInBytes;
				}
				if (this.IsA<DeltaStateEvent>())
				{
					return DeltaStateEvent.From(this)->deltaStateSizeInBytes;
				}
				string str = "Event must be a StateEvent or DeltaStateEvent but is ";
				InputEventPtr inputEventPtr = this;
				throw new InvalidOperationException(str + inputEventPtr.ToString());
			}
		}

		internal unsafe uint stateOffset
		{
			get
			{
				if (this.IsA<DeltaStateEvent>())
				{
					return DeltaStateEvent.From(this)->stateOffset;
				}
				string str = "Event must be a DeltaStateEvent but is ";
				InputEventPtr inputEventPtr = this;
				throw new InvalidOperationException(str + inputEventPtr.ToString());
			}
		}

		public unsafe bool IsA<TOtherEvent>() where TOtherEvent : struct, IInputEventTypeInfo
		{
			if (this.m_EventPtr == null)
			{
				return false;
			}
			TOtherEvent totherEvent = default(TOtherEvent);
			return this.m_EventPtr->type == totherEvent.typeStatic;
		}

		public InputEventPtr Next()
		{
			if (!this.valid)
			{
				return default(InputEventPtr);
			}
			return new InputEventPtr(InputEvent.GetNextInMemory(this.m_EventPtr));
		}

		public unsafe override string ToString()
		{
			if (!this.valid)
			{
				return "null";
			}
			InputEvent inputEvent = *this.m_EventPtr;
			return inputEvent.ToString();
		}

		public unsafe InputEvent* ToPointer()
		{
			return this;
		}

		public bool Equals(InputEventPtr other)
		{
			return this.m_EventPtr == other.m_EventPtr || InputEvent.Equals(this.m_EventPtr, other.m_EventPtr);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is InputEventPtr)
			{
				InputEventPtr other = (InputEventPtr)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.m_EventPtr;
		}

		public static bool operator ==(InputEventPtr left, InputEventPtr right)
		{
			return left.m_EventPtr == right.m_EventPtr;
		}

		public static bool operator !=(InputEventPtr left, InputEventPtr right)
		{
			return left.m_EventPtr != right.m_EventPtr;
		}

		public unsafe static implicit operator InputEventPtr(InputEvent* eventPtr)
		{
			return new InputEventPtr(eventPtr);
		}

		public unsafe static InputEventPtr From(InputEvent* eventPtr)
		{
			return new InputEventPtr(eventPtr);
		}

		public unsafe static implicit operator InputEvent*(InputEventPtr eventPtr)
		{
			return eventPtr.data;
		}

		public unsafe static InputEvent* FromInputEventPtr(InputEventPtr eventPtr)
		{
			return eventPtr.data;
		}

		private unsafe readonly InputEvent* m_EventPtr;
	}
}
