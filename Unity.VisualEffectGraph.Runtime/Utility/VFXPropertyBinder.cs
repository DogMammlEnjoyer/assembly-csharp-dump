using System;
using System.Collections.Generic;

namespace UnityEngine.VFX.Utility
{
	[RequireComponent(typeof(VisualEffect))]
	[DefaultExecutionOrder(1)]
	[DisallowMultipleComponent]
	[ExecuteAlways]
	public class VFXPropertyBinder : MonoBehaviour
	{
		private void OnEnable()
		{
			this.Reload();
		}

		private void OnValidate()
		{
			this.Reload();
		}

		private static void SafeDestroy(Object toDelete)
		{
			Object.Destroy(toDelete);
		}

		private void Reload()
		{
			this.m_VisualEffect = base.GetComponent<VisualEffect>();
			this.m_Bindings = new List<VFXBinderBase>();
			this.m_Bindings.AddRange(base.gameObject.GetComponents<VFXBinderBase>());
		}

		private void Reset()
		{
			this.Reload();
			this.ClearPropertyBinders();
		}

		private void LateUpdate()
		{
			if (!this.m_ExecuteInEditor && Application.isEditor && !Application.isPlaying)
			{
				return;
			}
			for (int i = 0; i < this.m_Bindings.Count; i++)
			{
				VFXBinderBase vfxbinderBase = this.m_Bindings[i];
				if (vfxbinderBase == null)
				{
					Debug.LogWarning(string.Format("Parameter binder at index {0} of GameObject {1} is null or missing", i, base.gameObject.name));
				}
				else if (vfxbinderBase.IsValid(this.m_VisualEffect))
				{
					vfxbinderBase.UpdateBinding(this.m_VisualEffect);
				}
			}
		}

		public T AddPropertyBinder<T>() where T : VFXBinderBase
		{
			return base.gameObject.AddComponent<T>();
		}

		[Obsolete("Use AddPropertyBinder<T>() instead")]
		public T AddParameterBinder<T>() where T : VFXBinderBase
		{
			return this.AddPropertyBinder<T>();
		}

		public void ClearPropertyBinders()
		{
			VFXBinderBase[] components = base.GetComponents<VFXBinderBase>();
			for (int i = 0; i < components.Length; i++)
			{
				VFXPropertyBinder.SafeDestroy(components[i]);
			}
		}

		[Obsolete("Please use ClearPropertyBinders() instead")]
		public void ClearParameterBinders()
		{
			this.ClearPropertyBinders();
		}

		public void RemovePropertyBinder(VFXBinderBase binder)
		{
			if (binder.gameObject == base.gameObject)
			{
				VFXPropertyBinder.SafeDestroy(binder);
			}
		}

		[Obsolete("Please use RemovePropertyBinder() instead")]
		public void RemoveParameterBinder(VFXBinderBase binder)
		{
			this.RemovePropertyBinder(binder);
		}

		public void RemovePropertyBinders<T>() where T : VFXBinderBase
		{
			foreach (VFXBinderBase vfxbinderBase in base.GetComponents<VFXBinderBase>())
			{
				if (vfxbinderBase is T)
				{
					VFXPropertyBinder.SafeDestroy(vfxbinderBase);
				}
			}
		}

		[Obsolete("Please use RemovePropertyBinders<T>() instead")]
		public void RemoveParameterBinders<T>() where T : VFXBinderBase
		{
			this.RemovePropertyBinders<T>();
		}

		public IEnumerable<T> GetPropertyBinders<T>() where T : VFXBinderBase
		{
			foreach (VFXBinderBase vfxbinderBase in this.m_Bindings)
			{
				if (vfxbinderBase is T)
				{
					yield return vfxbinderBase as T;
				}
			}
			List<VFXBinderBase>.Enumerator enumerator = default(List<VFXBinderBase>.Enumerator);
			yield break;
			yield break;
		}

		[Obsolete("Please use GetPropertyBinders<T>() instead")]
		public IEnumerable<T> GetParameterBinders<T>() where T : VFXBinderBase
		{
			return this.GetPropertyBinders<T>();
		}

		[SerializeField]
		protected bool m_ExecuteInEditor = true;

		public List<VFXBinderBase> m_Bindings = new List<VFXBinderBase>();

		[SerializeField]
		protected VisualEffect m_VisualEffect;
	}
}
