using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	[Serializable]
	public class VolumeParameter<T> : VolumeParameter, IEquatable<VolumeParameter<T>>
	{
		public virtual T value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				this.m_Value = value;
			}
		}

		public VolumeParameter() : this(default(T), false)
		{
		}

		protected VolumeParameter(T value, bool overrideState = false)
		{
			this.m_Value = value;
			this.overrideState = overrideState;
		}

		internal override void Interp(VolumeParameter from, VolumeParameter to, float t)
		{
			this.Interp((from as VolumeParameter<T>).value, (to as VolumeParameter<T>).value, t);
		}

		public virtual void Interp(T from, T to, float t)
		{
			this.m_Value = ((t > 0f) ? to : from);
		}

		public void Override(T x)
		{
			this.overrideState = true;
			this.m_Value = x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void SetValue(VolumeParameter parameter)
		{
			this.m_Value = ((VolumeParameter<T>)parameter).m_Value;
		}

		public override int GetHashCode()
		{
			int num = 17;
			num = num * 23 + this.overrideState.GetHashCode();
			if (!EqualityComparer<T>.Default.Equals(this.value, default(T)))
			{
				int num2 = num * 23;
				T value = this.value;
				num = num2 + value.GetHashCode();
			}
			return num;
		}

		public override string ToString()
		{
			return string.Format("{0} ({1})", this.value, this.overrideState);
		}

		public static bool operator ==(VolumeParameter<T> lhs, T rhs)
		{
			if (lhs != null && lhs.value != null)
			{
				T value = lhs.value;
				return value.Equals(rhs);
			}
			return false;
		}

		public static bool operator !=(VolumeParameter<T> lhs, T rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(VolumeParameter<T> other)
		{
			return other != null && (this == other || EqualityComparer<T>.Default.Equals(this.m_Value, other.m_Value));
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as VolumeParameter<T>);
		}

		public override object Clone()
		{
			return new VolumeParameter<T>(base.GetValue<T>(), this.overrideState);
		}

		public static explicit operator T(VolumeParameter<T> prop)
		{
			return prop.m_Value;
		}

		[SerializeField]
		protected T m_Value;
	}
}
