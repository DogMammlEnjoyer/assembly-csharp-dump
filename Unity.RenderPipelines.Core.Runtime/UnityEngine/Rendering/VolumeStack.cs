using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
	public sealed class VolumeStack : IDisposable
	{
		internal VolumeStack()
		{
		}

		internal void Clear()
		{
			foreach (KeyValuePair<Type, VolumeComponent> keyValuePair in this.components)
			{
				CoreUtils.Destroy(keyValuePair.Value);
			}
			this.components.Clear();
			this.parameters = null;
		}

		internal void Reload(Type[] componentTypes)
		{
			this.Clear();
			this.requiresReset = true;
			this.requiresResetForAllProperties = true;
			List<VolumeParameter> list = new List<VolumeParameter>();
			foreach (Type type in componentTypes)
			{
				VolumeComponent volumeComponent = (VolumeComponent)ScriptableObject.CreateInstance(type);
				this.components.Add(type, volumeComponent);
				list.AddRange(volumeComponent.parameters);
			}
			this.parameters = list.ToArray();
			this.isValid = true;
		}

		public T GetComponent<T>() where T : VolumeComponent
		{
			return (T)((object)this.GetComponent(typeof(T)));
		}

		public VolumeComponent GetComponent(Type type)
		{
			VolumeComponent result;
			this.components.TryGetValue(type, out result);
			return result;
		}

		public void Dispose()
		{
			this.Clear();
			this.isValid = false;
		}

		public bool isValid { get; private set; }

		internal readonly Dictionary<Type, VolumeComponent> components = new Dictionary<Type, VolumeComponent>();

		internal VolumeParameter[] parameters;

		internal bool requiresReset = true;

		internal bool requiresResetForAllProperties = true;
	}
}
