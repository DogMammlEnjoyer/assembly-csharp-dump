using System;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

namespace Fusion
{
	[StructLayout(LayoutKind.Explicit, Size = 2)]
	public readonly struct NetworkLoadSceneParameters : IEquatable<NetworkLoadSceneParameters>
	{
		internal NetworkLoadSceneParameters(NetworkSceneLoadId loadId, NetworkLoadSceneParametersFlags flags)
		{
			this.LoadId = loadId;
			this._flags = flags;
		}

		public LoadSceneMode LoadSceneMode
		{
			get
			{
				return ((this._flags & NetworkLoadSceneParametersFlags.Single) != (NetworkLoadSceneParametersFlags)0) ? LoadSceneMode.Single : LoadSceneMode.Additive;
			}
		}

		public LocalPhysicsMode LocalPhysicsMode
		{
			get
			{
				return (((this._flags & NetworkLoadSceneParametersFlags.LocalPhysics3D) != (NetworkLoadSceneParametersFlags)0) ? LocalPhysicsMode.Physics3D : LocalPhysicsMode.None) | (((this._flags & NetworkLoadSceneParametersFlags.LocalPhysics2D) != (NetworkLoadSceneParametersFlags)0) ? LocalPhysicsMode.Physics2D : LocalPhysicsMode.None);
			}
		}

		public LoadSceneParameters LoadSceneParameters
		{
			get
			{
				return new LoadSceneParameters(this.LoadSceneMode, this.LocalPhysicsMode);
			}
		}

		public bool IsActiveOnLoad
		{
			get
			{
				return (this._flags & NetworkLoadSceneParametersFlags.ActiveOnLoad) > (NetworkLoadSceneParametersFlags)0;
			}
		}

		public bool IsSingleLoad
		{
			get
			{
				return (this._flags & NetworkLoadSceneParametersFlags.Single) > (NetworkLoadSceneParametersFlags)0;
			}
		}

		public bool IsLocalPhysics2D
		{
			get
			{
				return (this._flags & NetworkLoadSceneParametersFlags.LocalPhysics2D) > (NetworkLoadSceneParametersFlags)0;
			}
		}

		public bool IsLocalPhysics3D
		{
			get
			{
				return (this._flags & NetworkLoadSceneParametersFlags.LocalPhysics3D) > (NetworkLoadSceneParametersFlags)0;
			}
		}

		public bool Equals(NetworkLoadSceneParameters other)
		{
			return this._flags == other._flags && this.LoadId.Equals(other.LoadId);
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is NetworkLoadSceneParameters)
			{
				NetworkLoadSceneParameters other = (NetworkLoadSceneParameters)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return (int)this._flags * 397 ^ this.LoadId.GetHashCode();
		}

		public static bool operator ==(NetworkLoadSceneParameters left, NetworkLoadSceneParameters right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(NetworkLoadSceneParameters left, NetworkLoadSceneParameters right)
		{
			return !left.Equals(right);
		}

		public override string ToString()
		{
			return string.Format("[Flags: {0}, LoadId: {1}]", this._flags, this.LoadId);
		}

		[FieldOffset(0)]
		public readonly NetworkSceneLoadId LoadId;

		[FieldOffset(1)]
		private readonly NetworkLoadSceneParametersFlags _flags;
	}
}
