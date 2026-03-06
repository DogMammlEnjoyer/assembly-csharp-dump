using System;

namespace UnityEngine.VFX.Utility
{
	[ExecuteAlways]
	[RequireComponent(typeof(VFXPropertyBinder))]
	public abstract class VFXBinderBase : MonoBehaviour
	{
		public abstract bool IsValid(VisualEffect component);

		public virtual void Reset()
		{
		}

		protected virtual void Awake()
		{
			this.binder = base.GetComponent<VFXPropertyBinder>();
		}

		protected virtual void OnEnable()
		{
			if (!this.binder.m_Bindings.Contains(this))
			{
				this.binder.m_Bindings.Add(this);
			}
			base.hideFlags = HideFlags.HideInInspector;
		}

		protected virtual void OnDisable()
		{
			if (this.binder.m_Bindings.Contains(this))
			{
				this.binder.m_Bindings.Remove(this);
			}
		}

		public abstract void UpdateBinding(VisualEffect component);

		public override string ToString()
		{
			return base.GetType().ToString();
		}

		protected VFXPropertyBinder binder;
	}
}
