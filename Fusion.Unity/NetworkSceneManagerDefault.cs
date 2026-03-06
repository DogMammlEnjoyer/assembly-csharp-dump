using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Fusion
{
	public class NetworkSceneManagerDefault : Behaviour, INetworkSceneManager
	{
		public Scene MultiPeerScene { get; private set; }

		public Transform MultiPeerDontDestroyOnLoadRoot { get; private set; }

		public NetworkRunner Runner { get; private set; }

		private bool IsMultiplePeer
		{
			get
			{
				return this.Runner.Config.PeerMode == NetworkProjectConfig.PeerModes.Multiple;
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ClearStatics()
		{
			NetworkSceneManagerDefault._allOwnedScenes.Clear();
		}

		static NetworkSceneManagerDefault()
		{
			SceneManager.sceneUnloaded += delegate(Scene s)
			{
				NetworkSceneManagerDefault._allOwnedScenes.Remove(s);
			};
		}

		public virtual void Initialize(NetworkRunner runner)
		{
			this.LoadAddressableScenePathsAsync();
			this.Runner = runner;
			if (this.IsMultiplePeer)
			{
				Scene multiPeerScene = SceneManager.CreateScene(string.Format("{0}_{1}", runner.name, runner.LocalPlayer), new CreateSceneParameters(LocalPhysicsMode.Physics2D | LocalPhysicsMode.Physics3D));
				this.MultiPeerScene = multiPeerScene;
				this.MultiPeerDontDestroyOnLoadRoot = new GameObject("[DontDestroyOnLoad]").transform;
				SceneManager.MoveGameObjectToScene(this.MultiPeerDontDestroyOnLoadRoot.gameObject, this.MultiPeerScene);
			}
		}

		public virtual void Shutdown()
		{
			this.Runner = null;
			foreach (Scene key in (from x in NetworkSceneManagerDefault._allOwnedScenes
			where x.Value == this
			select x.Key).ToList<Scene>())
			{
				NetworkSceneManagerDefault._allOwnedScenes.Remove(key);
			}
			this._multiPeerSceneRoots.Clear();
			this._multiPeerActiveRoot = null;
			this.MultiPeerDontDestroyOnLoadRoot = null;
			Scene multiPeerScene = this.MultiPeerScene;
			this.MultiPeerScene = default(Scene);
			if (multiPeerScene.isLoaded)
			{
				if (!multiPeerScene.CanBeUnloaded())
				{
					SceneManager.CreateScene("FusionSceneManager_TempEmptyScene");
				}
				SceneManager.UnloadSceneAsync(multiPeerScene);
			}
		}

		public virtual bool IsBusy
		{
			get
			{
				return this._isLoading || (this.IsMultiplePeer && this._multiPeerSceneRoots.Count == 0);
			}
		}

		public virtual Scene MainRunnerScene
		{
			get
			{
				if (this.IsMultiplePeer)
				{
					return this.MultiPeerScene;
				}
				return SceneManager.GetActiveScene();
			}
		}

		public virtual bool IsRunnerScene(Scene scene)
		{
			return !this.IsMultiplePeer || scene == this.MultiPeerScene;
		}

		public virtual bool TryGetPhysicsScene2D(out PhysicsScene2D scene2D)
		{
			Scene mainRunnerScene = this.MainRunnerScene;
			if (mainRunnerScene.IsValid())
			{
				scene2D = mainRunnerScene.GetPhysicsScene2D();
				return true;
			}
			scene2D = default(PhysicsScene2D);
			return false;
		}

		public virtual bool TryGetPhysicsScene3D(out PhysicsScene scene3D)
		{
			Scene mainRunnerScene = this.MainRunnerScene;
			if (mainRunnerScene.IsValid())
			{
				scene3D = mainRunnerScene.GetPhysicsScene();
				return true;
			}
			scene3D = default(PhysicsScene);
			return false;
		}

		public virtual void MakeDontDestroyOnLoad(GameObject obj)
		{
			if (this.IsMultiplePeer)
			{
				obj.transform.SetParent(this.MultiPeerDontDestroyOnLoadRoot, true);
				return;
			}
			Object.DontDestroyOnLoad(obj);
		}

		public bool MoveGameObjectToScene(GameObject gameObject, SceneRef sceneRef)
		{
			if (this.IsMultiplePeer)
			{
				foreach (NetworkSceneManagerDefault.MultiPeerSceneRoot multiPeerSceneRoot in this._multiPeerSceneRoots)
				{
					if ((!(sceneRef != default(SceneRef)) || !(multiPeerSceneRoot.SceneRef != sceneRef)) && (!(sceneRef == default(SceneRef)) || !this._multiPeerActiveRoot || !(multiPeerSceneRoot != this._multiPeerActiveRoot)))
					{
						if (gameObject.scene != this.MultiPeerScene)
						{
							gameObject.transform.SetParent(null, true);
							SceneManager.MoveGameObjectToScene(gameObject, this.MultiPeerScene);
							if (!Application.isBatchMode)
							{
								this.Runner.AddVisibilityNodes(gameObject);
							}
						}
						gameObject.transform.SetParent(multiPeerSceneRoot.transform, true);
						return true;
					}
				}
				return false;
			}
			if (sceneRef == default(SceneRef))
			{
				return true;
			}
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				Scene sceneAt = SceneManager.GetSceneAt(i);
				if (sceneAt.isLoaded && this.GetSceneRef(sceneAt.path) == sceneRef)
				{
					SceneManager.MoveGameObjectToScene(gameObject, sceneAt);
					return true;
				}
			}
			return false;
		}

		public virtual NetworkSceneAsyncOp LoadScene(SceneRef sceneRef, NetworkLoadSceneParameters parameters)
		{
			return NetworkSceneAsyncOp.FromCoroutine(sceneRef, this.StartTracedCoroutine(this.LoadSceneCoroutine(sceneRef, parameters)));
		}

		public virtual NetworkSceneAsyncOp UnloadScene(SceneRef sceneRef)
		{
			return NetworkSceneAsyncOp.FromCoroutine(sceneRef, this.StartTracedCoroutine(this.UnloadSceneCoroutine(sceneRef)));
		}

		public virtual SceneRef GetSceneRef(string sceneNameOrPath)
		{
			int sceneBuildIndex = FusionUnitySceneManagerUtils.GetSceneBuildIndex(sceneNameOrPath);
			if (sceneBuildIndex >= 0)
			{
				return SceneRef.FromIndex(sceneBuildIndex);
			}
			string[] array;
			if (!this.TryGetAddressableScenes(out array))
			{
				Log.Error(this, "Failed to resolve addressable scene paths, won't be able to resolve " + sceneNameOrPath + " or any other addressable scene.");
				array = Array.Empty<string>();
			}
			int sceneIndex = FusionUnitySceneManagerUtils.GetSceneIndex(array, sceneNameOrPath);
			if (sceneIndex >= 0)
			{
				return SceneRef.FromPath(array[sceneIndex]);
			}
			return SceneRef.None;
		}

		public SceneRef GetSceneRef(GameObject gameObject)
		{
			if (!this.IsMultiplePeer)
			{
				return this.GetSceneRef(gameObject.scene.path);
			}
			if (gameObject.scene != this.MultiPeerScene)
			{
				return default(SceneRef);
			}
			Transform root = gameObject.transform.root;
			foreach (NetworkSceneManagerDefault.MultiPeerSceneRoot multiPeerSceneRoot in this._multiPeerSceneRoots)
			{
				if (multiPeerSceneRoot.transform == root)
				{
					return multiPeerSceneRoot.SceneRef;
				}
			}
			return default(SceneRef);
		}

		public bool OnSceneInfoChanged(NetworkSceneInfo sceneInfo, NetworkSceneInfoChangeSource changeSource)
		{
			return false;
		}

		protected virtual IEnumerator LoadSceneCoroutine(SceneRef sceneRef, NetworkLoadSceneParameters sceneParams)
		{
			this.Runner.InvokeSceneLoadStart(sceneRef);
			Scene scene = default(Scene);
			using (this.MakeLoadingScope())
			{
				LocalPhysicsMode localPhysicsMode = sceneParams.LocalPhysicsMode;
				LoadSceneMode loadSceneMode = sceneParams.LoadSceneMode;
				if (this.IsMultiplePeer)
				{
					if (localPhysicsMode != LocalPhysicsMode.None)
					{
						throw new ArgumentException("Local physics mode is not supported in multiple peer mode", "sceneParams");
					}
					if (loadSceneMode == LoadSceneMode.Single)
					{
						loadSceneMode = LoadSceneMode.Additive;
						try
						{
							foreach (NetworkSceneManagerDefault.MultiPeerSceneRoot multiPeerSceneRoot in this._multiPeerSceneRoots)
							{
								Object.Destroy(multiPeerSceneRoot.gameObject);
							}
							foreach (NetworkSceneManagerDefault.MultiPeerSceneRoot root in this._multiPeerSceneRoots)
							{
								while (root != null)
								{
									yield return null;
								}
								root = null;
							}
							List<NetworkSceneManagerDefault.MultiPeerSceneRoot>.Enumerator enumerator2 = default(List<NetworkSceneManagerDefault.MultiPeerSceneRoot>.Enumerator);
						}
						finally
						{
							this._multiPeerSceneRoots.Clear();
						}
					}
				}
				else if (this.DestroySpawnedPrefabsOnSceneUnload && loadSceneMode == LoadSceneMode.Single)
				{
					for (int j = 0; j < SceneManager.sceneCount; j++)
					{
						Scene sceneAt = SceneManager.GetSceneAt(j);
						SceneRef sceneRef2 = this.GetSceneRef(sceneAt.path);
						if (sceneRef2 != SceneRef.None)
						{
							this.DestroyAllRuntimeSpawnedObjectsInScene(sceneAt, sceneRef2);
						}
					}
				}
				if (this.IsSceneTakeOverEnabled)
				{
					Scene candidate = this.FindSceneToTakeOver(sceneRef);
					if (candidate.IsValid())
					{
						if (candidate.GetLocalPhysicsMode() != localPhysicsMode)
						{
							throw new InvalidOperationException(string.Format("Tried to take over {0} for {1}, but physics mode were different: {2} != {3}", new object[]
							{
								candidate.Dump(),
								sceneRef,
								candidate.GetLocalPhysicsMode(),
								localPhysicsMode
							}));
						}
						scene = candidate;
						this.MarkSceneAsOwned(sceneRef, candidate);
						if (loadSceneMode == LoadSceneMode.Single && !this.IsMultiplePeer)
						{
							int k;
							for (int i = 0; i < SceneManager.sceneCount; i = k + 1)
							{
								Scene sceneAt2 = SceneManager.GetSceneAt(i);
								if (sceneAt2 != candidate)
								{
									yield return SceneManager.UnloadSceneAsync(sceneAt2);
								}
								k = i;
							}
						}
					}
					candidate = default(Scene);
				}
				if (!scene.IsValid())
				{
					if (loadSceneMode == LoadSceneMode.Single)
					{
						this._addressableOperations.Clear();
					}
					if (sceneRef.IsIndex)
					{
						AsyncOperation op3 = SceneManager.LoadSceneAsync(sceneRef.AsIndex, new LoadSceneParameters(loadSceneMode, localPhysicsMode));
						if (op3 == null)
						{
							throw new InvalidOperationException(string.Format("Scene not found: {0}", sceneRef.AsIndex));
						}
						scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
						this.MarkSceneAsOwned(sceneRef, scene);
						while (!op3.isDone)
						{
							this.OnLoadSceneProgress(sceneRef, op3.progress);
							yield return null;
						}
						op3 = null;
					}
					else
					{
						string[] array;
						if (!this.TryGetAddressableScenes(out array))
						{
							Log.Error(this, string.Format("Failed to resolve addressable scene paths, won't be able to resolve {0}", sceneRef));
							array = Array.Empty<string>();
						}
						string sceneAddress = null;
						foreach (string text in array)
						{
							if (sceneRef.IsPath(text))
							{
								sceneAddress = text;
								break;
							}
						}
						if (sceneAddress == null)
						{
							throw new InvalidOperationException(string.Format("Unable to find addressable scene path for {0}", sceneRef));
						}
						LoadSceneParameters loadSceneParameters = new LoadSceneParameters(loadSceneMode, localPhysicsMode);
						AsyncOperationHandle<SceneInstance> op2 = Addressables.LoadSceneAsync(sceneAddress, loadSceneParameters, true, 100);
						scene = default(Scene);
						op2.Completed += delegate(AsyncOperationHandle<SceneInstance> op)
						{
							if (op.Status == AsyncOperationStatus.Succeeded)
							{
								scene = op.Result.Scene;
								this.MarkSceneAsOwned(sceneRef, scene);
							}
						};
						op2.Destroyed += delegate(AsyncOperationHandle _)
						{
							this._addressableOperations.Remove(sceneRef);
						};
						this._addressableOperations.Add(sceneRef, op2);
						while (!op2.IsDone)
						{
							this.OnLoadSceneProgress(sceneRef, op2.PercentComplete);
							yield return null;
						}
						if (!op2.IsValid())
						{
							throw new InvalidOperationException(string.Format("Loading operation for {0} has been destroyed", sceneRef));
						}
						if (op2.Status == AsyncOperationStatus.Failed)
						{
							Addressables.Release<SceneInstance>(op2);
							throw new InvalidOperationException("Failed to load scene from addressable: " + sceneAddress);
						}
						sceneAddress = null;
						op2 = default(AsyncOperationHandle<SceneInstance>);
					}
				}
			}
			NetworkSceneManagerDefault.LoadingScope loadingScope = default(NetworkSceneManagerDefault.LoadingScope);
			yield return base.StartCoroutine(this.OnSceneLoaded(sceneRef, scene, sceneParams));
			yield break;
			yield break;
		}

		protected virtual IEnumerator UnloadSceneCoroutine(SceneRef sceneRef)
		{
			using (this.MakeLoadingScope())
			{
				if (this.IsMultiplePeer)
				{
					for (int i = 0; i < this._multiPeerSceneRoots.Count; i++)
					{
						NetworkSceneManagerDefault.MultiPeerSceneRoot root = this._multiPeerSceneRoots[i];
						if (root.SceneRef == sceneRef)
						{
							if (root == this._multiPeerActiveRoot)
							{
								this._multiPeerActiveRoot = null;
							}
							this._multiPeerSceneRoots.RemoveAt(i);
							Object.Destroy(root.gameObject);
							while (root != null)
							{
								yield return null;
							}
							yield break;
						}
						root = null;
					}
					throw new ArgumentOutOfRangeException(string.Format("Did not find a scene to unload: {0}", sceneRef), "sceneRef");
				}
				Scene scene = default(Scene);
				for (int j = 0; j < SceneManager.sceneCount; j++)
				{
					Scene sceneAt = SceneManager.GetSceneAt(j);
					if (this.GetSceneRef(sceneAt.path) == sceneRef)
					{
						scene = sceneAt;
						break;
					}
				}
				if (!scene.IsValid())
				{
					throw new ArgumentOutOfRangeException(string.Format("Did not find a scene to unload: {0}", sceneRef), "sceneRef");
				}
				if (this.DestroySpawnedPrefabsOnSceneUnload)
				{
					this.DestroyAllRuntimeSpawnedObjectsInScene(scene, sceneRef);
				}
				if (!scene.CanBeUnloaded())
				{
					Log.Warn(this.Runner, string.Format("Scene {0} can't be unloaded for {1}, creating a temporary scene to unload it", scene.Dump(), sceneRef));
					this._tempUnloadScene = SceneManager.CreateScene("FusionSceneManager_TempEmptyScene");
				}
				AsyncOperationHandle<SceneInstance> handle;
				if (this._addressableOperations.TryGetValue(sceneRef, out handle))
				{
					yield return Addressables.UnloadSceneAsync(handle, true);
				}
				else
				{
					AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(scene);
					if (asyncOperation == null)
					{
						throw new InvalidOperationException("Failed to unload " + scene.Dump());
					}
					yield return asyncOperation;
				}
			}
			NetworkSceneManagerDefault.LoadingScope loadingScope = default(NetworkSceneManagerDefault.LoadingScope);
			yield break;
			yield break;
		}

		protected virtual IEnumerator OnSceneLoaded(SceneRef sceneRef, Scene scene, NetworkLoadSceneParameters sceneParams)
		{
			GameObject[] array;
			NetworkObject[] components = scene.GetComponents(true, out array);
			Array.Sort<NetworkObject>(components, NetworkObjectSortKeyComparer.Instance);
			if (this.IsMultiplePeer)
			{
				NetworkSceneManagerDefault.MultiPeerSceneRoot multiPeerSceneRoot = new GameObject("[" + scene.name + "]").AddComponent<NetworkSceneManagerDefault.MultiPeerSceneRoot>();
				multiPeerSceneRoot.SceneRef = sceneRef;
				multiPeerSceneRoot.SceneHandle = scene.handle;
				multiPeerSceneRoot.Scene = scene;
				multiPeerSceneRoot.ScenePath = scene.path;
				SceneManager.MoveGameObjectToScene(multiPeerSceneRoot.gameObject, scene);
				GameObject[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].transform.SetParent(multiPeerSceneRoot.transform, true);
				}
				this._multiPeerSceneRoots.Add(multiPeerSceneRoot);
				SceneManager.MergeScenes(scene, this.MultiPeerScene);
				if (sceneParams.IsActiveOnLoad)
				{
					this._multiPeerActiveRoot = multiPeerSceneRoot;
				}
			}
			else if (sceneParams.IsActiveOnLoad)
			{
				SceneManager.SetActiveScene(scene);
			}
			this.Runner.RegisterSceneObjects(sceneRef, components, sceneParams.LoadId);
			NetworkRunner runner = this.Runner;
			SceneLoadDoneArgs sceneLoadDoneArgs = new SceneLoadDoneArgs(sceneRef, components, scene, array);
			runner.InvokeSceneLoadDone(sceneLoadDoneArgs);
			yield break;
		}

		protected virtual void OnLoadSceneProgress(SceneRef sceneRef, float progress)
		{
		}

		private void DestroyAllRuntimeSpawnedObjectsInScene(Scene scene, SceneRef sceneRef)
		{
			foreach (NetworkObject networkObject in this.Runner.GetAllNetworkObjects())
			{
				if (networkObject.gameObject.scene == scene && !networkObject.NetworkTypeId.IsSceneObject)
				{
					if (networkObject.HasStateAuthority)
					{
						this.Runner.Despawn(networkObject);
					}
					else
					{
						Object.Destroy(networkObject.gameObject);
					}
				}
			}
		}

		private Scene FindSceneToTakeOver(SceneRef sceneRef)
		{
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				Scene sceneAt = SceneManager.GetSceneAt(i);
				if (sceneAt.isLoaded && !(this.GetSceneRef(sceneAt.path) != sceneRef) && !NetworkSceneManagerDefault._allOwnedScenes.ContainsKey(sceneAt))
				{
					return sceneAt;
				}
			}
			return default(Scene);
		}

		private ICoroutine StartTracedCoroutine(IEnumerator inner)
		{
			FusionCoroutine fusionCoroutine = new FusionCoroutine(inner);
			this._runningCoroutines.Add(fusionCoroutine);
			fusionCoroutine.Completed += delegate(IAsyncOperation x)
			{
				if (this.LogSceneLoadErrors && x.Error != null)
				{
					Log.Error(this.Runner, string.Format("Failed async op: {0}", x.Error.SourceException));
				}
				int num = this._runningCoroutines.IndexOf((ICoroutine)x);
				this._runningCoroutines.RemoveAt(num);
				if (num < this._runningCoroutines.Count)
				{
					base.StartCoroutine(this._runningCoroutines[num]);
				}
			};
			if (this._runningCoroutines.Count == 1)
			{
				base.StartCoroutine(fusionCoroutine);
			}
			return fusionCoroutine;
		}

		protected NetworkSceneManagerDefault.LoadingScope MakeLoadingScope()
		{
			return new NetworkSceneManagerDefault.LoadingScope(this);
		}

		protected void MarkSceneAsOwned(SceneRef sceneRef, Scene scene)
		{
			NetworkSceneManagerDefault arg;
			if (NetworkSceneManagerDefault._allOwnedScenes.TryGetValue(scene, out arg))
			{
				Log.Warn(this.Runner, string.Format("Scene {0} (for {1}) already owned by {2}", scene.Dump(), sceneRef, arg));
				return;
			}
			NetworkSceneManagerDefault._allOwnedScenes.Add(scene, this);
		}

		private NetworkSceneAsyncOp FailOp(SceneRef sceneRef, Exception exception)
		{
			if (this.LogSceneLoadErrors)
			{
				Log.Error(this.Runner, string.Format("Failed with: {0}", exception));
			}
			return NetworkSceneAsyncOp.FromError(sceneRef, exception);
		}

		public NetworkSceneManagerDefault()
		{
			this._addressableScenesTask = new Lazy<NetworkSceneManagerDefault.GetAddressableScenesResult>(() => this.GetAddressableScenes());
		}

		public Task LoadAddressableScenePathsAsync()
		{
			return this._addressableScenesTask.Value.Task;
		}

		protected virtual NetworkSceneManagerDefault.GetAddressableScenesResult GetAddressableScenes()
		{
			TaskCompletionSource<string[]> tcs = new TaskCompletionSource<string[]>();
			AsyncOperationHandle<IList<IResourceLocation>> result = Addressables.LoadResourceLocationsAsync(this.AddressableScenesLabel, typeof(SceneInstance));
			result.Completed += delegate(AsyncOperationHandle<IList<IResourceLocation>> op)
			{
				try
				{
					if (op.Status == AsyncOperationStatus.Failed)
					{
						tcs.SetException(op.OperationException);
					}
					else
					{
						string[] result = (from x in op.Result
						select x.PrimaryKey).ToArray<string>();
						tcs.SetResult(result);
					}
				}
				finally
				{
					Addressables.Release<IList<IResourceLocation>>(op);
				}
			};
			return new NetworkSceneManagerDefault.GetAddressableScenesResult
			{
				Task = tcs.Task,
				BeforeWaitForCompletion = delegate()
				{
					if (result.IsValid())
					{
						result.WaitForCompletion();
					}
				}
			};
		}

		protected virtual TimeSpan GetAddressableScenePathsTimeout()
		{
			return TimeSpan.FromSeconds(10.0);
		}

		private bool TryGetAddressableScenes(out string[] addressableScenes)
		{
			if (!this._addressableScenesTask.IsValueCreated)
			{
				Log.Warn(this.Runner, "Going to block the thread in wait for addressable scene paths being resolved, call and await LoadAddressableScenePathsAsync to avoid this.");
			}
			NetworkSceneManagerDefault.GetAddressableScenesResult value = this._addressableScenesTask.Value;
			if (!value.Task.IsCompleted)
			{
				Action beforeWaitForCompletion = value.BeforeWaitForCompletion;
				if (beforeWaitForCompletion != null)
				{
					beforeWaitForCompletion();
				}
				if (!value.Task.Wait(this.GetAddressableScenePathsTimeout()))
				{
					addressableScenes = null;
					return false;
				}
			}
			addressableScenes = value.Task.Result;
			return true;
		}

		[InlineHelp]
		[ToggleLeft]
		public bool IsSceneTakeOverEnabled = true;

		[InlineHelp]
		[ToggleLeft]
		public bool LogSceneLoadErrors = true;

		[InlineHelp]
		[ToggleLeft]
		public bool DestroySpawnedPrefabsOnSceneUnload = true;

		private static Dictionary<Scene, NetworkSceneManagerDefault> _allOwnedScenes = new Dictionary<Scene, NetworkSceneManagerDefault>(new FusionUnitySceneManagerUtils.SceneEqualityComparer());

		private List<NetworkSceneManagerDefault.MultiPeerSceneRoot> _multiPeerSceneRoots = new List<NetworkSceneManagerDefault.MultiPeerSceneRoot>();

		private NetworkSceneManagerDefault.MultiPeerSceneRoot _multiPeerActiveRoot;

		private List<ICoroutine> _runningCoroutines = new List<ICoroutine>();

		private Scene _tempUnloadScene;

		private bool _isLoading;

		[InlineHelp]
		public string AddressableScenesLabel = "FusionScenes";

		private Lazy<NetworkSceneManagerDefault.GetAddressableScenesResult> _addressableScenesTask;

		private Dictionary<SceneRef, AsyncOperationHandle<SceneInstance>> _addressableOperations = new Dictionary<SceneRef, AsyncOperationHandle<SceneInstance>>();

		protected struct GetAddressableScenesResult
		{
			public static implicit operator NetworkSceneManagerDefault.GetAddressableScenesResult(Task<string[]> task)
			{
				return new NetworkSceneManagerDefault.GetAddressableScenesResult
				{
					Task = task
				};
			}

			public Task<string[]> Task;

			public Action BeforeWaitForCompletion;
		}

		protected sealed class MultiPeerSceneRoot : MonoBehaviour
		{
			public SceneRef SceneRef;

			public string ScenePath;

			public int SceneHandle;

			public Scene Scene;
		}

		protected struct LoadingScope : IDisposable
		{
			public LoadingScope(NetworkSceneManagerDefault manager)
			{
				this._manager = manager;
				this._manager._isLoading = true;
			}

			public void Dispose()
			{
				this._manager._isLoading = false;
			}

			private readonly NetworkSceneManagerDefault _manager;
		}
	}
}
