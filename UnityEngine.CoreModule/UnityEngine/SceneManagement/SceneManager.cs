using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Events;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.SceneManagement
{
	[RequiredByNativeCode]
	[NativeHeader("Runtime/Export/SceneManager/SceneManager.bindings.h")]
	public class SceneManager
	{
		public static extern int sceneCount { [StaticAccessor("GetSceneManager()", StaticAccessorType.Dot)] [NativeMethod("GetSceneCount")] [NativeHeader("Runtime/SceneManager/SceneManager.h")] [MethodImpl(MethodImplOptions.InternalCall)] get; }

		public static extern int loadedSceneCount { [NativeHeader("Runtime/SceneManager/SceneManager.h")] [StaticAccessor("GetSceneManager()", StaticAccessorType.Dot)] [NativeMethod("GetLoadedSceneCount")] [MethodImpl(MethodImplOptions.InternalCall)] get; }

		public static int sceneCountInBuildSettings
		{
			get
			{
				return SceneManagerAPI.ActiveAPI.GetNumScenesInBuildSettings();
			}
		}

		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		internal static bool CanSetAsActiveScene(Scene scene)
		{
			return SceneManager.CanSetAsActiveScene_Injected(ref scene);
		}

		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		public static Scene GetActiveScene()
		{
			Scene result;
			SceneManager.GetActiveScene_Injected(out result);
			return result;
		}

		[NativeThrows]
		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		public static bool SetActiveScene(Scene scene)
		{
			return SceneManager.SetActiveScene_Injected(ref scene);
		}

		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		public unsafe static Scene GetSceneByPath(string scenePath)
		{
			Scene result;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(scenePath, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = scenePath.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Scene scene;
				SceneManager.GetSceneByPath_Injected(ref managedSpanWrapper, out scene);
			}
			finally
			{
				char* ptr = null;
				Scene scene;
				result = scene;
			}
			return result;
		}

		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		public unsafe static Scene GetSceneByName(string name)
		{
			Scene result;
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
				Scene scene;
				SceneManager.GetSceneByName_Injected(ref managedSpanWrapper, out scene);
			}
			finally
			{
				char* ptr = null;
				Scene scene;
				result = scene;
			}
			return result;
		}

		public static Scene GetSceneByBuildIndex(int buildIndex)
		{
			return SceneManagerAPI.ActiveAPI.GetSceneByBuildIndex(buildIndex);
		}

		[NativeThrows]
		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		public static Scene GetSceneAt(int index)
		{
			Scene result;
			SceneManager.GetSceneAt_Injected(index, out result);
			return result;
		}

		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		[NativeThrows]
		public unsafe static Scene CreateScene([NotNull] string sceneName, CreateSceneParameters parameters)
		{
			if (sceneName == null)
			{
				ThrowHelper.ThrowArgumentNullException(sceneName, "sceneName");
			}
			Scene result;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(sceneName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = sceneName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Scene scene;
				SceneManager.CreateScene_Injected(ref managedSpanWrapper, ref parameters, out scene);
			}
			finally
			{
				char* ptr = null;
				Scene scene;
				result = scene;
			}
			return result;
		}

		[NativeThrows]
		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		private static bool UnloadSceneInternal(Scene scene, UnloadSceneOptions options)
		{
			return SceneManager.UnloadSceneInternal_Injected(ref scene, options);
		}

		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		[NativeThrows]
		private static AsyncOperation UnloadSceneAsyncInternal(Scene scene, UnloadSceneOptions options)
		{
			IntPtr intPtr = SceneManager.UnloadSceneAsyncInternal_Injected(ref scene, options);
			return (intPtr == 0) ? null : AsyncOperation.BindingsMarshaller.ConvertToManaged(intPtr);
		}

		private static AsyncOperation LoadSceneAsyncNameIndexInternal(string sceneName, int sceneBuildIndex, LoadSceneParameters parameters, bool mustCompleteNextFrame)
		{
			bool flag = !SceneManager.s_AllowLoadScene;
			AsyncOperation result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = SceneManagerAPI.ActiveAPI.LoadSceneAsyncByNameOrIndex(sceneName, sceneBuildIndex, parameters, mustCompleteNextFrame);
			}
			return result;
		}

		private static AsyncOperation UnloadSceneNameIndexInternal(string sceneName, int sceneBuildIndex, bool immediately, UnloadSceneOptions options, out bool outSuccess)
		{
			bool flag = !SceneManager.s_AllowLoadScene;
			AsyncOperation result;
			if (flag)
			{
				outSuccess = false;
				result = null;
			}
			else
			{
				result = SceneManagerAPI.ActiveAPI.UnloadSceneAsyncByNameOrIndex(sceneName, sceneBuildIndex, immediately, options, out outSuccess);
			}
			return result;
		}

		[NativeThrows]
		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		public static void MergeScenes(Scene sourceScene, Scene destinationScene)
		{
			SceneManager.MergeScenes_Injected(ref sourceScene, ref destinationScene);
		}

		[NativeThrows]
		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		public static void MoveGameObjectToScene([NotNull] GameObject go, Scene scene)
		{
			if (go == null)
			{
				ThrowHelper.ThrowArgumentNullException(go, "go");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(go);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(go, "go");
			}
			SceneManager.MoveGameObjectToScene_Injected(intPtr, ref scene);
		}

		[NativeThrows]
		[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
		private static void MoveGameObjectsToSceneByInstanceId(IntPtr instanceIds, int instanceCount, Scene scene)
		{
			SceneManager.MoveGameObjectsToSceneByInstanceId_Injected(instanceIds, instanceCount, ref scene);
		}

		public static void MoveGameObjectsToScene(NativeArray<int> instanceIDs, Scene scene)
		{
			bool flag = !instanceIDs.IsCreated;
			if (flag)
			{
				throw new ArgumentException("NativeArray is uninitialized", "instanceIDs");
			}
			bool flag2 = instanceIDs.Length == 0;
			if (!flag2)
			{
				SceneManager.MoveGameObjectsToSceneByInstanceId((IntPtr)instanceIDs.GetUnsafeReadOnlyPtr<int>(), instanceIDs.Length, scene);
			}
		}

		[RequiredByNativeCode]
		internal static AsyncOperation LoadFirstScene_Internal(bool async)
		{
			return SceneManagerAPI.ActiveAPI.LoadFirstScene(async);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event UnityAction<Scene, LoadSceneMode> sceneLoaded;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event UnityAction<Scene> sceneUnloaded;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event UnityAction<Scene, Scene> activeSceneChanged;

		[Obsolete("Use SceneManager.sceneCount and SceneManager.GetSceneAt(int index) to loop the all scenes instead.")]
		public static Scene[] GetAllScenes()
		{
			Scene[] array = new Scene[SceneManager.sceneCount];
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				array[i] = SceneManager.GetSceneAt(i);
			}
			return array;
		}

		public static Scene CreateScene(string sceneName)
		{
			CreateSceneParameters parameters = new CreateSceneParameters(LocalPhysicsMode.None);
			return SceneManager.CreateScene(sceneName, parameters);
		}

		public static void LoadScene(string sceneName, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
		{
			LoadSceneParameters parameters = new LoadSceneParameters(mode);
			SceneManager.LoadScene(sceneName, parameters);
		}

		[ExcludeFromDocs]
		public static void LoadScene(string sceneName)
		{
			LoadSceneParameters parameters = new LoadSceneParameters(LoadSceneMode.Single);
			SceneManager.LoadScene(sceneName, parameters);
		}

		public static Scene LoadScene(string sceneName, LoadSceneParameters parameters)
		{
			SceneManager.LoadSceneAsyncNameIndexInternal(sceneName, -1, parameters, true);
			return SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
		}

		public static void LoadScene(int sceneBuildIndex, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
		{
			LoadSceneParameters parameters = new LoadSceneParameters(mode);
			SceneManager.LoadScene(sceneBuildIndex, parameters);
		}

		[ExcludeFromDocs]
		public static void LoadScene(int sceneBuildIndex)
		{
			LoadSceneParameters parameters = new LoadSceneParameters(LoadSceneMode.Single);
			SceneManager.LoadScene(sceneBuildIndex, parameters);
		}

		public static Scene LoadScene(int sceneBuildIndex, LoadSceneParameters parameters)
		{
			SceneManager.LoadSceneAsyncNameIndexInternal(null, sceneBuildIndex, parameters, true);
			return SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
		}

		public static AsyncOperation LoadSceneAsync(int sceneBuildIndex, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
		{
			LoadSceneParameters parameters = new LoadSceneParameters(mode);
			return SceneManager.LoadSceneAsync(sceneBuildIndex, parameters);
		}

		[ExcludeFromDocs]
		public static AsyncOperation LoadSceneAsync(int sceneBuildIndex)
		{
			LoadSceneParameters parameters = new LoadSceneParameters(LoadSceneMode.Single);
			return SceneManager.LoadSceneAsync(sceneBuildIndex, parameters);
		}

		public static AsyncOperation LoadSceneAsync(int sceneBuildIndex, LoadSceneParameters parameters)
		{
			return SceneManager.LoadSceneAsyncNameIndexInternal(null, sceneBuildIndex, parameters, false);
		}

		public static AsyncOperation LoadSceneAsync(string sceneName, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
		{
			LoadSceneParameters parameters = new LoadSceneParameters(mode);
			return SceneManager.LoadSceneAsync(sceneName, parameters);
		}

		[ExcludeFromDocs]
		public static AsyncOperation LoadSceneAsync(string sceneName)
		{
			LoadSceneParameters parameters = new LoadSceneParameters(LoadSceneMode.Single);
			return SceneManager.LoadSceneAsync(sceneName, parameters);
		}

		public static AsyncOperation LoadSceneAsync(string sceneName, LoadSceneParameters parameters)
		{
			return SceneManager.LoadSceneAsyncNameIndexInternal(sceneName, -1, parameters, false);
		}

		[Obsolete("Use SceneManager.UnloadSceneAsync. This function is not safe to use during triggers and under other circumstances. See Scripting reference for more details.")]
		public static bool UnloadScene(Scene scene)
		{
			return SceneManager.UnloadSceneInternal(scene, UnloadSceneOptions.None);
		}

		[Obsolete("Use SceneManager.UnloadSceneAsync. This function is not safe to use during triggers and under other circumstances. See Scripting reference for more details.")]
		public static bool UnloadScene(int sceneBuildIndex)
		{
			bool result;
			SceneManager.UnloadSceneNameIndexInternal("", sceneBuildIndex, true, UnloadSceneOptions.None, out result);
			return result;
		}

		[Obsolete("Use SceneManager.UnloadSceneAsync. This function is not safe to use during triggers and under other circumstances. See Scripting reference for more details.")]
		public static bool UnloadScene(string sceneName)
		{
			bool result;
			SceneManager.UnloadSceneNameIndexInternal(sceneName, -1, true, UnloadSceneOptions.None, out result);
			return result;
		}

		public static AsyncOperation UnloadSceneAsync(int sceneBuildIndex)
		{
			bool flag;
			return SceneManager.UnloadSceneNameIndexInternal("", sceneBuildIndex, false, UnloadSceneOptions.None, out flag);
		}

		public static AsyncOperation UnloadSceneAsync(string sceneName)
		{
			bool flag;
			return SceneManager.UnloadSceneNameIndexInternal(sceneName, -1, false, UnloadSceneOptions.None, out flag);
		}

		public static AsyncOperation UnloadSceneAsync(Scene scene)
		{
			return SceneManager.UnloadSceneAsyncInternal(scene, UnloadSceneOptions.None);
		}

		public static AsyncOperation UnloadSceneAsync(int sceneBuildIndex, UnloadSceneOptions options)
		{
			bool flag;
			return SceneManager.UnloadSceneNameIndexInternal("", sceneBuildIndex, false, options, out flag);
		}

		public static AsyncOperation UnloadSceneAsync(string sceneName, UnloadSceneOptions options)
		{
			bool flag;
			return SceneManager.UnloadSceneNameIndexInternal(sceneName, -1, false, options, out flag);
		}

		public static AsyncOperation UnloadSceneAsync(Scene scene, UnloadSceneOptions options)
		{
			return SceneManager.UnloadSceneAsyncInternal(scene, options);
		}

		[RequiredByNativeCode]
		private static void Internal_SceneLoaded(Scene scene, LoadSceneMode mode)
		{
			bool flag = SceneManager.sceneLoaded != null;
			if (flag)
			{
				SceneManager.sceneLoaded(scene, mode);
			}
		}

		[RequiredByNativeCode]
		private static void Internal_SceneUnloaded(Scene scene)
		{
			bool flag = SceneManager.sceneUnloaded != null;
			if (flag)
			{
				SceneManager.sceneUnloaded(scene);
			}
		}

		[RequiredByNativeCode]
		private static void Internal_ActiveSceneChanged(Scene previousActiveScene, Scene newActiveScene)
		{
			bool flag = SceneManager.activeSceneChanged != null;
			if (flag)
			{
				SceneManager.activeSceneChanged(previousActiveScene, newActiveScene);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CanSetAsActiveScene_Injected([In] ref Scene scene);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetActiveScene_Injected(out Scene ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetActiveScene_Injected([In] ref Scene scene);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSceneByPath_Injected(ref ManagedSpanWrapper scenePath, out Scene ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSceneByName_Injected(ref ManagedSpanWrapper name, out Scene ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSceneAt_Injected(int index, out Scene ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreateScene_Injected(ref ManagedSpanWrapper sceneName, [In] ref CreateSceneParameters parameters, out Scene ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool UnloadSceneInternal_Injected([In] ref Scene scene, UnloadSceneOptions options);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr UnloadSceneAsyncInternal_Injected([In] ref Scene scene, UnloadSceneOptions options);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MergeScenes_Injected([In] ref Scene sourceScene, [In] ref Scene destinationScene);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MoveGameObjectToScene_Injected(IntPtr go, [In] ref Scene scene);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MoveGameObjectsToSceneByInstanceId_Injected(IntPtr instanceIds, int instanceCount, [In] ref Scene scene);

		internal static bool s_AllowLoadScene = true;
	}
}
