using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;

namespace UnityEngine.ResourceManagement.ResourceProviders
{
	public class SceneProvider : ISceneProvider2, ISceneProvider
	{
		public AsyncOperationHandle<SceneInstance> ProvideScene(ResourceManager resourceManager, IResourceLocation location, LoadSceneMode loadSceneMode, bool activateOnLoad, int priority)
		{
			return this.ProvideScene(resourceManager, location, new LoadSceneParameters(loadSceneMode), activateOnLoad, priority);
		}

		public AsyncOperationHandle<SceneInstance> ProvideScene(ResourceManager resourceManager, IResourceLocation location, LoadSceneParameters loadSceneParameters, bool activateOnLoad, int priority)
		{
			return this.ProvideScene(resourceManager, location, loadSceneParameters, SceneReleaseMode.ReleaseSceneWhenSceneUnloaded, activateOnLoad, priority);
		}

		public AsyncOperationHandle<SceneInstance> ProvideScene(ResourceManager resourceManager, IResourceLocation location, LoadSceneParameters loadSceneParameters, SceneReleaseMode releaseMode, bool activateOnLoad, int priority)
		{
			AsyncOperationHandle<IList<AsyncOperationHandle>> asyncOperationHandle = default(AsyncOperationHandle<IList<AsyncOperationHandle>>);
			if (location.HasDependencies)
			{
				asyncOperationHandle = resourceManager.ProvideResourceGroupCached(location.Dependencies, location.DependencyHashCode, typeof(IAssetBundleResource), null, true);
			}
			SceneProvider.SceneOp sceneOp = new SceneProvider.SceneOp(resourceManager, this);
			sceneOp.Init(location, loadSceneParameters, releaseMode, activateOnLoad, priority, asyncOperationHandle);
			AsyncOperationHandle<SceneInstance> result = resourceManager.StartOperation<SceneInstance>(sceneOp, asyncOperationHandle);
			if (asyncOperationHandle.IsValid())
			{
				asyncOperationHandle.Release();
			}
			return result;
		}

		public AsyncOperationHandle<SceneInstance> ReleaseScene(ResourceManager resourceManager, AsyncOperationHandle<SceneInstance> sceneLoadHandle)
		{
			return ((ISceneProvider2)this).ReleaseScene(resourceManager, sceneLoadHandle, UnloadSceneOptions.None);
		}

		AsyncOperationHandle<SceneInstance> ISceneProvider2.ReleaseScene(ResourceManager resourceManager, AsyncOperationHandle<SceneInstance> sceneLoadHandle, UnloadSceneOptions unloadOptions)
		{
			SceneProvider.UnloadSceneOp unloadSceneOp = new SceneProvider.UnloadSceneOp();
			unloadSceneOp.Init(sceneLoadHandle, unloadOptions);
			return resourceManager.StartOperation<SceneInstance>(unloadSceneOp, sceneLoadHandle);
		}

		private class SceneOp : AsyncOperationBase<SceneInstance>, IUpdateReceiver
		{
			public SceneOp(ResourceManager rm, ISceneProvider2 provider)
			{
				this.m_ResourceManager = rm;
				this.m_provider = provider;
			}

			internal override DownloadStatus GetDownloadStatus(HashSet<object> visited)
			{
				if (!this.m_DepOp.IsValid())
				{
					return new DownloadStatus
					{
						IsDone = base.IsDone
					};
				}
				return this.m_DepOp.InternalGetDownloadStatus(visited);
			}

			public void Init(IResourceLocation location, LoadSceneMode loadSceneMode, bool activateOnLoad, int priority, AsyncOperationHandle<IList<AsyncOperationHandle>> depOp)
			{
				this.Init(location, new LoadSceneParameters(loadSceneMode), SceneReleaseMode.ReleaseSceneWhenSceneUnloaded, activateOnLoad, priority, depOp);
			}

			public void Init(IResourceLocation location, LoadSceneParameters loadSceneParameters, SceneReleaseMode releaseMode, bool activateOnLoad, int priority, AsyncOperationHandle<IList<AsyncOperationHandle>> depOp)
			{
				this.m_DepOp = (depOp.IsValid() ? depOp.Acquire() : depOp);
				this.m_Location = location;
				this.m_LoadSceneParameters = loadSceneParameters;
				this.m_ReleaseMode = releaseMode;
				this.m_ActivateOnLoad = activateOnLoad;
				this.m_Priority = priority;
			}

			protected override bool InvokeWaitForCompletion()
			{
				if (this.m_DepOp.IsValid() && !this.m_DepOp.IsDone)
				{
					this.m_DepOp.WaitForCompletion();
				}
				ResourceManager rm = this.m_RM;
				if (rm != null)
				{
					rm.Update(Time.unscaledDeltaTime);
				}
				if (!this.HasExecuted)
				{
					base.InvokeExecute();
				}
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				while (!base.IsDone)
				{
					((IUpdateReceiver)this).Update(Time.unscaledDeltaTime);
					if (this.m_Inst.m_Operation.progress == 0f && stopwatch.ElapsedMilliseconds > 5000L)
					{
						throw new Exception("Infinite loop detected within LoadSceneAsync.WaitForCompletion. For more information see the notes under the Scenes section of the \"Synchronous Addressables\" page of the Addressables documentation, or consider using asynchronous scene loading code.");
					}
					if (this.m_Inst.m_Operation.allowSceneActivation && Mathf.Approximately(this.m_Inst.m_Operation.progress, 0.9f))
					{
						base.Result = this.m_Inst;
						return true;
					}
				}
				return base.IsDone;
			}

			public override void GetDependencies(List<AsyncOperationHandle> deps)
			{
				if (this.m_DepOp.IsValid())
				{
					deps.Add(this.m_DepOp);
				}
			}

			protected override string DebugName
			{
				get
				{
					return string.Format("Scene({0})", (this.m_Location == null) ? "Invalid" : AsyncOperationBase<SceneInstance>.ShortenPath(this.m_ResourceManager.TransformInternalId(this.m_Location), false));
				}
			}

			protected override void Execute()
			{
				bool loadingFromBundle = false;
				if (this.m_DepOp.IsValid())
				{
					foreach (AsyncOperationHandle asyncOperationHandle in this.m_DepOp.Result)
					{
						IAssetBundleResource assetBundleResource = asyncOperationHandle.Result as IAssetBundleResource;
						if (assetBundleResource != null && assetBundleResource.GetAssetBundle() != null)
						{
							loadingFromBundle = true;
						}
					}
				}
				if (!this.m_DepOp.IsValid() || this.m_DepOp.OperationException == null)
				{
					this.m_Inst = this.InternalLoadScene(this.m_Location, loadingFromBundle, this.m_LoadSceneParameters, this.m_ActivateOnLoad, this.m_Priority);
					((IUpdateReceiver)this).Update(0f);
				}
				else
				{
					base.Complete(this.m_Inst, false, this.m_DepOp.OperationException, true);
				}
				this.HasExecuted = true;
			}

			internal SceneInstance InternalLoadScene(IResourceLocation location, bool loadingFromBundle, LoadSceneParameters loadSceneParameters, bool activateOnLoad, int priority)
			{
				string path = this.m_ResourceManager.TransformInternalId(location);
				AsyncOperation asyncOperation = this.InternalLoad(path, loadingFromBundle, loadSceneParameters);
				asyncOperation.allowSceneActivation = activateOnLoad;
				asyncOperation.priority = priority;
				return new SceneInstance
				{
					m_Operation = asyncOperation,
					Scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1),
					ReleaseSceneOnSceneUnloaded = (this.m_ReleaseMode == SceneReleaseMode.ReleaseSceneWhenSceneUnloaded)
				};
			}

			private AsyncOperation InternalLoad(string path, bool loadingFromBundle, LoadSceneParameters loadSceneParameters)
			{
				return SceneManager.LoadSceneAsync(path, loadSceneParameters);
			}

			protected override void Destroy()
			{
				if (this.m_Inst.Scene.IsValid())
				{
					this.m_provider.ReleaseScene(this.m_ResourceManager, base.Handle, UnloadSceneOptions.None).ReleaseHandleOnCompletion();
				}
				if (this.m_DepOp.IsValid())
				{
					this.m_DepOp.Release();
				}
				base.Destroy();
			}

			protected override float Progress
			{
				get
				{
					float num = 0.9f;
					float num2 = 0.1f;
					float num3 = 0f;
					if (this.m_Inst.m_Operation != null)
					{
						num3 += this.m_Inst.m_Operation.progress * num2;
					}
					if (!this.m_DepOp.IsDone)
					{
						num3 += this.m_DepOp.PercentComplete * num;
					}
					else
					{
						num3 += num;
					}
					return num3;
				}
			}

			void IUpdateReceiver.Update(float unscaledDeltaTime)
			{
				if (this.m_Inst.m_Operation != null && (this.m_Inst.m_Operation.isDone || (!this.m_Inst.m_Operation.allowSceneActivation && Mathf.Approximately(this.m_Inst.m_Operation.progress, 0.9f))))
				{
					this.m_ResourceManager.RemoveUpdateReciever(this);
					base.Complete(this.m_Inst, true, null);
				}
			}

			private bool m_ActivateOnLoad;

			private SceneInstance m_Inst;

			private IResourceLocation m_Location;

			private LoadSceneParameters m_LoadSceneParameters;

			private SceneReleaseMode m_ReleaseMode;

			private int m_Priority;

			private AsyncOperationHandle<IList<AsyncOperationHandle>> m_DepOp;

			private ResourceManager m_ResourceManager;

			private ISceneProvider2 m_provider;
		}

		private class UnloadSceneOp : AsyncOperationBase<SceneInstance>
		{
			public void Init(AsyncOperationHandle<SceneInstance> sceneLoadHandle, UnloadSceneOptions options)
			{
				if (sceneLoadHandle.IsValid())
				{
					this.m_sceneLoadHandle = sceneLoadHandle;
					this.m_Instance = this.m_sceneLoadHandle.Result;
				}
				this.m_UnloadOptions = options;
			}

			protected override void Execute()
			{
				if (this.m_sceneLoadHandle.IsValid() && this.m_Instance.Scene.isLoaded)
				{
					AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(this.m_Instance.Scene, this.m_UnloadOptions);
					if (asyncOperation == null)
					{
						this.UnloadSceneCompleted(null);
					}
					else
					{
						asyncOperation.completed += this.UnloadSceneCompleted;
					}
				}
				else
				{
					this.UnloadSceneCompleted(null);
				}
				this.HasExecuted = true;
			}

			protected override bool InvokeWaitForCompletion()
			{
				ResourceManager rm = this.m_RM;
				if (rm != null)
				{
					rm.Update(Time.unscaledDeltaTime);
				}
				if (!this.HasExecuted)
				{
					base.InvokeExecute();
				}
				Debug.LogWarning("Cannot unload a Scene with WaitForCompletion. Scenes must be unloaded asynchronously.");
				return true;
			}

			private void UnloadSceneCompleted(AsyncOperation obj)
			{
				base.Complete(this.m_Instance, true, "");
				if (this.m_sceneLoadHandle.IsValid() && this.m_sceneLoadHandle.ReferenceCount > 0)
				{
					this.m_sceneLoadHandle.Release();
				}
			}

			protected override float Progress
			{
				get
				{
					return this.m_sceneLoadHandle.PercentComplete;
				}
			}

			private SceneInstance m_Instance;

			private AsyncOperationHandle<SceneInstance> m_sceneLoadHandle;

			private UnloadSceneOptions m_UnloadOptions;
		}
	}
}
