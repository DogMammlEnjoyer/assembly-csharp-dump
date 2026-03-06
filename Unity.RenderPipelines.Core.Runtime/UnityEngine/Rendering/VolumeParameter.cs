using System;

namespace UnityEngine.Rendering
{
	public abstract class VolumeParameter : ICloneable
	{
		public virtual bool overrideState
		{
			get
			{
				return this.m_OverrideState;
			}
			set
			{
				this.m_OverrideState = value;
			}
		}

		internal abstract void Interp(VolumeParameter from, VolumeParameter to, float t);

		public T GetValue<T>()
		{
			return ((VolumeParameter<T>)this).value;
		}

		public abstract void SetValue(VolumeParameter parameter);

		protected internal virtual void OnEnable()
		{
		}

		protected internal virtual void OnDisable()
		{
		}

		public static bool IsObjectParameter(Type type)
		{
			return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ObjectParameter<>)) || (type.BaseType != null && VolumeParameter.IsObjectParameter(type.BaseType));
		}

		public virtual void Release()
		{
		}

		public abstract object Clone();

		public const string k_DebuggerDisplay = "{m_Value} ({m_OverrideState})";

		[SerializeField]
		protected bool m_OverrideState;
	}
}
