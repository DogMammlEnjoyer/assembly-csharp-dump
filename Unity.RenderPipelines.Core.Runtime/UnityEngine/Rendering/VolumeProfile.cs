using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
	public sealed class VolumeProfile : ScriptableObject
	{
		[Obsolete("This field was only public for editor access. #from(6000.0)")]
		public bool isDirty
		{
			get
			{
				return this.dirtyState > VolumeProfile.DirtyState.None;
			}
			set
			{
				if (value)
				{
					this.dirtyState |= VolumeProfile.DirtyState.Other;
					return;
				}
				this.dirtyState &= ~VolumeProfile.DirtyState.Other;
			}
		}

		private void OnEnable()
		{
			this.components.RemoveAll((VolumeComponent x) => x == null);
		}

		internal void OnDisable()
		{
			if (this.components == null)
			{
				return;
			}
			for (int i = 0; i < this.components.Count; i++)
			{
				if (this.components[i] != null)
				{
					this.components[i].Release();
				}
			}
		}

		public void Reset()
		{
			this.dirtyState |= VolumeProfile.DirtyState.DirtyByProfileReset;
		}

		public T Add<T>(bool overrides = false) where T : VolumeComponent
		{
			return (T)((object)this.Add(typeof(T), overrides));
		}

		public VolumeComponent Add(Type type, bool overrides = false)
		{
			if (this.Has(type))
			{
				throw new InvalidOperationException("Component already exists in the volume");
			}
			VolumeComponent volumeComponent = (VolumeComponent)ScriptableObject.CreateInstance(type);
			volumeComponent.SetAllOverridesTo(overrides);
			this.components.Add(volumeComponent);
			this.dirtyState |= VolumeProfile.DirtyState.DirtyByComponentChange;
			return volumeComponent;
		}

		public void Remove<T>() where T : VolumeComponent
		{
			this.Remove(typeof(T));
		}

		public void Remove(Type type)
		{
			int num = -1;
			for (int i = 0; i < this.components.Count; i++)
			{
				if (this.components[i].GetType() == type)
				{
					num = i;
					break;
				}
			}
			if (num >= 0)
			{
				this.components.RemoveAt(num);
				this.dirtyState |= VolumeProfile.DirtyState.DirtyByComponentChange;
			}
		}

		public bool Has<T>() where T : VolumeComponent
		{
			return this.Has(typeof(T));
		}

		public bool Has(Type type)
		{
			using (List<VolumeComponent>.Enumerator enumerator = this.components.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.GetType() == type)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool HasSubclassOf(Type type)
		{
			using (List<VolumeComponent>.Enumerator enumerator = this.components.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.GetType().IsSubclassOf(type))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool TryGet<T>(out T component) where T : VolumeComponent
		{
			return this.TryGet<T>(typeof(T), out component);
		}

		public bool TryGet<T>(Type type, out T component) where T : VolumeComponent
		{
			component = default(T);
			foreach (VolumeComponent volumeComponent in this.components)
			{
				if (volumeComponent.GetType() == type)
				{
					component = (T)((object)volumeComponent);
					return true;
				}
			}
			return false;
		}

		public bool TryGetSubclassOf<T>(Type type, out T component) where T : VolumeComponent
		{
			component = default(T);
			foreach (VolumeComponent volumeComponent in this.components)
			{
				if (volumeComponent.GetType().IsSubclassOf(type))
				{
					component = (T)((object)volumeComponent);
					return true;
				}
			}
			return false;
		}

		public bool TryGetAllSubclassOf<T>(Type type, List<T> result) where T : VolumeComponent
		{
			int count = result.Count;
			foreach (VolumeComponent volumeComponent in this.components)
			{
				if (volumeComponent.GetType().IsSubclassOf(type))
				{
					result.Add((T)((object)volumeComponent));
				}
			}
			return count != result.Count;
		}

		public override int GetHashCode()
		{
			int num = 17;
			for (int i = 0; i < this.components.Count; i++)
			{
				num = num * 23 + this.components[i].GetHashCode();
			}
			return num;
		}

		internal int GetComponentListHashCode()
		{
			int num = 17;
			for (int i = 0; i < this.components.Count; i++)
			{
				num = num * 23 + this.components[i].GetType().GetHashCode();
			}
			return num;
		}

		internal void Sanitize()
		{
			for (int i = this.components.Count - 1; i >= 0; i--)
			{
				if (this.components[i] == null)
				{
					this.components.RemoveAt(i);
				}
			}
		}

		public List<VolumeComponent> components = new List<VolumeComponent>();

		internal VolumeProfile.DirtyState dirtyState;

		[Flags]
		internal enum DirtyState
		{
			None = 0,
			DirtyByComponentChange = 1,
			DirtyByProfileReset = 2,
			Other = 4
		}
	}
}
