using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Meta.XR.ImmersiveDebugger.Utils
{
	[Serializable]
	internal readonly struct InstanceHandle : IEquatable<InstanceHandle>
	{
		public Object Instance { get; }

		public Type Type { get; }

		public int InstanceId { get; }

		public bool IsStatic
		{
			get
			{
				return this.InstanceId == 0;
			}
		}

		public bool Valid
		{
			get
			{
				return this.Type != null && (this.IsStatic || this.Instance != null || this.Type == typeof(Scene));
			}
		}

		public InstanceHandle(Type type, Object instance)
		{
			this.Type = type;
			this.Instance = instance;
			this.InstanceId = ((instance != null) ? instance.GetInstanceID() : 0);
		}

		public InstanceHandle(Scene scene)
		{
			this.Type = typeof(Scene);
			this.Instance = null;
			this.InstanceId = scene.handle;
		}

		public static InstanceHandle Static(Type type)
		{
			return new InstanceHandle(type, null);
		}

		public bool Equals(InstanceHandle other)
		{
			return this.InstanceId == other.InstanceId && this.Type == other.Type;
		}

		public override bool Equals(object obj)
		{
			if (obj is InstanceHandle)
			{
				InstanceHandle other = (InstanceHandle)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			int num = this.InstanceId.GetHashCode() * 486187739;
			Type type = this.Type;
			return (num + ((type != null) ? new int?(type.GetHashCode()) : null)).GetValueOrDefault();
		}
	}
}
