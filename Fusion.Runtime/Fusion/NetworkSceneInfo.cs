using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

namespace Fusion
{
	[NetworkStructWeaved(13)]
	[StructLayout(LayoutKind.Explicit)]
	public struct NetworkSceneInfo : INetworkStruct, IEquatable<NetworkSceneInfo>
	{
		public FixedArray<SceneRef> Scenes
		{
			get
			{
				return FixedArray.Create<SceneRef>(ref this._scene0, this.SceneCount);
			}
		}

		public FixedArray<NetworkLoadSceneParameters> SceneParams
		{
			get
			{
				return FixedArray.Create<NetworkLoadSceneParameters>(ref this._sceneMeta0, this.SceneCount);
			}
		}

		public unsafe int IndexOf(SceneRef sceneRef, NetworkLoadSceneParameters sceneParams)
		{
			for (int i = 0; i < this.SceneCount; i++)
			{
				bool flag = *this.Scenes[i] == sceneRef && *this.SceneParams[i] == sceneParams;
				if (flag)
				{
					return i;
				}
			}
			return -1;
		}

		public int IndexOf([TupleElementNames(new string[]
		{
			"SceneRef",
			"SceneParams"
		})] ValueTuple<SceneRef, NetworkLoadSceneParameters> scene)
		{
			return this.IndexOf(scene.Item1, scene.Item2);
		}

		public int SceneCount
		{
			get
			{
				return (int)(this._flags & NetworkSceneInfoDefaultFlags.SceneCountMask);
			}
			private set
			{
				this._flags = ((this._flags & ~NetworkSceneInfoDefaultFlags.SceneCountMask) | (NetworkSceneInfoDefaultFlags)(value & 15));
			}
		}

		public int Version
		{
			get
			{
				return (int)((this._flags & NetworkSceneInfoDefaultFlags.ConterMask) >> 4);
			}
			private set
			{
				this._flags = ((this._flags & ~NetworkSceneInfoDefaultFlags.ConterMask) | (NetworkSceneInfoDefaultFlags)(value << 4 & 1048560));
			}
		}

		public int AddSceneRef(SceneRef sceneRef, LoadSceneMode loadSceneMode = LoadSceneMode.Single, LocalPhysicsMode localPhysicsMode = LocalPhysicsMode.None, bool activeOnLoad = false)
		{
			return this.AddSceneRef(sceneRef, ((loadSceneMode == LoadSceneMode.Single) ? NetworkLoadSceneParametersFlags.Single : ((NetworkLoadSceneParametersFlags)0)) | (((localPhysicsMode & LocalPhysicsMode.Physics2D) != LocalPhysicsMode.None) ? NetworkLoadSceneParametersFlags.LocalPhysics2D : ((NetworkLoadSceneParametersFlags)0)) | (((localPhysicsMode & LocalPhysicsMode.Physics3D) != LocalPhysicsMode.None) ? NetworkLoadSceneParametersFlags.LocalPhysics3D : ((NetworkLoadSceneParametersFlags)0)) | (activeOnLoad ? NetworkLoadSceneParametersFlags.ActiveOnLoad : ((NetworkLoadSceneParametersFlags)0)));
		}

		internal unsafe int AddSceneRef(SceneRef sceneRef, NetworkLoadSceneParametersFlags flags)
		{
			bool flag = (flags & NetworkLoadSceneParametersFlags.Single) > (NetworkLoadSceneParametersFlags)0;
			if (flag)
			{
				this.Scenes.Clear();
				this.SceneParams.Clear();
				this.SceneCount = 0;
			}
			bool flag2 = this.SceneCount >= 8;
			int result;
			if (flag2)
			{
				result = -1;
			}
			else
			{
				int num = this.SceneCount;
				this.SceneCount = num + 1;
				int num2 = num;
				*this.SceneParams[num2] = new NetworkLoadSceneParameters(new NetworkSceneLoadId((byte)this.Version), flags);
				*this.Scenes[num2] = sceneRef;
				num = this.Version + 1;
				this.Version = num;
				result = num2;
			}
			return result;
		}

		public unsafe bool RemoveSceneRef(SceneRef sceneRef)
		{
			for (int i = 0; i < this.SceneCount; i++)
			{
				bool flag = *this.Scenes[i] == sceneRef;
				if (flag)
				{
					for (int j = i + 1; j < this.SceneCount; j++)
					{
						*this.Scenes[j - 1] = *this.Scenes[j];
						*this.SceneParams[j - 1] = *this.SceneParams[j];
					}
					*this.Scenes[this.SceneCount - 1] = default(SceneRef);
					*this.SceneParams[this.SceneCount - 1] = default(NetworkLoadSceneParameters);
					int num = this.Version + 1;
					this.Version = num;
					num = this.SceneCount - 1;
					this.SceneCount = num;
					return true;
				}
			}
			return false;
		}

		public override string ToString()
		{
			return "[Scenes: " + string.Join<SceneRef>(", ", this.Scenes) + "]";
		}

		public unsafe bool Equals(NetworkSceneInfo other)
		{
			fixed (NetworkSceneInfo* ptr = &this)
			{
				NetworkSceneInfo* ptr2 = ptr;
				return Native.MemCmp((void*)ptr2, (void*)(&other), 52) == 0;
			}
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is NetworkSceneInfo)
			{
				NetworkSceneInfo other = (NetworkSceneInfo)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public unsafe override int GetHashCode()
		{
			fixed (NetworkSceneInfo* ptr = &this)
			{
				NetworkSceneInfo* data = ptr;
				return HashCodeUtilities.GetHashCodeDeterministic<NetworkSceneInfo>(data, 0);
			}
		}

		public static implicit operator NetworkSceneInfo(SceneRef sceneRef)
		{
			NetworkSceneInfo result = default(NetworkSceneInfo);
			result.AddSceneRef(sceneRef, LoadSceneMode.Single, LocalPhysicsMode.None, false);
			return result;
		}

		public const int WORD_COUNT = 13;

		public const int SIZE = 52;

		public const int MaxScenes = 8;

		[FieldOffset(0)]
		private NetworkSceneInfoDefaultFlags _flags;

		[FieldOffset(4)]
		private SceneRef _scene0;

		[FieldOffset(8)]
		private SceneRef _scene1;

		[FieldOffset(12)]
		private SceneRef _scene2;

		[FieldOffset(16)]
		private SceneRef _scene3;

		[FieldOffset(20)]
		private SceneRef _scene4;

		[FieldOffset(24)]
		private SceneRef _scene5;

		[FieldOffset(28)]
		private SceneRef _scene6;

		[FieldOffset(32)]
		private SceneRef _scene7;

		[FieldOffset(36)]
		private NetworkLoadSceneParameters _sceneMeta0;

		[FieldOffset(38)]
		private NetworkLoadSceneParameters _sceneMeta1;

		[FieldOffset(40)]
		private NetworkLoadSceneParameters _sceneMeta2;

		[FieldOffset(42)]
		private NetworkLoadSceneParameters _sceneMeta3;

		[FieldOffset(44)]
		private NetworkLoadSceneParameters _sceneMeta4;

		[FieldOffset(46)]
		private NetworkLoadSceneParameters _sceneMeta5;

		[FieldOffset(48)]
		private NetworkLoadSceneParameters _sceneMeta6;

		[FieldOffset(50)]
		private NetworkLoadSceneParameters _sceneMeta7;
	}
}
