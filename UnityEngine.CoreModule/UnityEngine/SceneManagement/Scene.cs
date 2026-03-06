using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.SceneManagement
{
	[NativeHeader("Runtime/Export/SceneManager/Scene.bindings.h")]
	[Serializable]
	public struct Scene
	{
		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsValidInternal(int sceneHandle);

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		private static string GetPathInternal(int sceneHandle)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpan;
				Scene.GetPathInternal_Injected(sceneHandle, out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		private unsafe static void SetPathAndGUIDInternal(int sceneHandle, string path, string guid)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = path.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(guid, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = guid.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				Scene.SetPathAndGUIDInternal_Injected(sceneHandle, ref managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
		}

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		private static string GetNameInternal(int sceneHandle)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpan;
				Scene.GetNameInternal_Injected(sceneHandle, out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		[NativeThrows]
		private unsafe static void SetNameInternal(int sceneHandle, string name)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Scene.SetNameInternal_Injected(sceneHandle, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		private static string GetGUIDInternal(int sceneHandle)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpan;
				Scene.GetGUIDInternal_Injected(sceneHandle, out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsSubScene(int sceneHandle);

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetIsSubScene(int sceneHandle, bool value);

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetIsLoadedInternal(int sceneHandle);

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Scene.LoadingState GetLoadingStateInternal(int sceneHandle);

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetIsDirtyInternal(int sceneHandle);

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetDirtyID(int sceneHandle);

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetBuildIndexInternal(int sceneHandle);

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetRootCountInternal(int sceneHandle);

		[StaticAccessor("SceneBindings", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRootGameObjectsInternal(int sceneHandle, object resultRootList);

		internal Scene(int handle)
		{
			this.m_Handle = handle;
		}

		public int handle
		{
			get
			{
				return this.m_Handle;
			}
		}

		internal Scene.LoadingState loadingState
		{
			get
			{
				return Scene.GetLoadingStateInternal(this.handle);
			}
		}

		internal string guid
		{
			get
			{
				return Scene.GetGUIDInternal(this.handle);
			}
		}

		public bool IsValid()
		{
			return Scene.IsValidInternal(this.handle);
		}

		public string path
		{
			get
			{
				return Scene.GetPathInternal(this.handle);
			}
		}

		public string name
		{
			get
			{
				return Scene.GetNameInternal(this.handle);
			}
			set
			{
				Scene.SetNameInternal(this.handle, value);
			}
		}

		public bool isLoaded
		{
			get
			{
				return Scene.GetIsLoadedInternal(this.handle);
			}
		}

		public int buildIndex
		{
			get
			{
				return Scene.GetBuildIndexInternal(this.handle);
			}
		}

		public bool isDirty
		{
			get
			{
				return Scene.GetIsDirtyInternal(this.handle);
			}
		}

		internal int dirtyID
		{
			get
			{
				return Scene.GetDirtyID(this.handle);
			}
		}

		public int rootCount
		{
			get
			{
				return Scene.GetRootCountInternal(this.handle);
			}
		}

		public bool isSubScene
		{
			get
			{
				return Scene.IsSubScene(this.handle);
			}
			set
			{
				Scene.SetIsSubScene(this.handle, value);
			}
		}

		public GameObject[] GetRootGameObjects()
		{
			List<GameObject> list = new List<GameObject>(this.rootCount);
			this.GetRootGameObjects(list);
			return list.ToArray();
		}

		public void GetRootGameObjects(List<GameObject> rootGameObjects)
		{
			bool flag = rootGameObjects.Capacity < this.rootCount;
			if (flag)
			{
				rootGameObjects.Capacity = this.rootCount;
			}
			rootGameObjects.Clear();
			bool flag2 = !this.IsValid();
			if (flag2)
			{
				throw new ArgumentException("The scene is invalid.");
			}
			bool flag3 = !Application.isPlaying && !this.isLoaded;
			if (flag3)
			{
				throw new ArgumentException("The scene is not loaded.");
			}
			bool flag4 = this.rootCount == 0;
			if (!flag4)
			{
				Scene.GetRootGameObjectsInternal(this.handle, rootGameObjects);
			}
		}

		public static bool operator ==(Scene lhs, Scene rhs)
		{
			return lhs.handle == rhs.handle;
		}

		public static bool operator !=(Scene lhs, Scene rhs)
		{
			return lhs.handle != rhs.handle;
		}

		public override int GetHashCode()
		{
			return this.m_Handle;
		}

		public override bool Equals(object other)
		{
			bool flag = !(other is Scene);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Scene scene = (Scene)other;
				result = (this.handle == scene.handle);
			}
			return result;
		}

		internal void SetPathAndGuid(string path, string guid)
		{
			Scene.SetPathAndGUIDInternal(this.m_Handle, path, guid);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPathInternal_Injected(int sceneHandle, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPathAndGUIDInternal_Injected(int sceneHandle, ref ManagedSpanWrapper path, ref ManagedSpanWrapper guid);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetNameInternal_Injected(int sceneHandle, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetNameInternal_Injected(int sceneHandle, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGUIDInternal_Injected(int sceneHandle, out ManagedSpanWrapper ret);

		[SerializeField]
		[HideInInspector]
		private int m_Handle;

		internal enum LoadingState
		{
			NotLoaded,
			Loading,
			Loaded,
			Unloading
		}
	}
}
