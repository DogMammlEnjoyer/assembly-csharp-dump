using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.ResourceManagement.ResourceProviders
{
	[DisplayName("Assets from Bundles Provider")]
	public class BundledAssetProvider : ResourceProviderBase
	{
		public override void Provide(ProvideHandle provideHandle)
		{
			new BundledAssetProvider.InternalOp().Start(provideHandle);
		}

		internal class InternalOp
		{
			internal static T LoadBundleFromDependecies<T>(IList<object> results) where T : class, IAssetBundleResource
			{
				if (results == null || results.Count == 0)
				{
					return default(T);
				}
				IAssetBundleResource assetBundleResource = null;
				bool flag = true;
				for (int i = 0; i < results.Count; i++)
				{
					IAssetBundleResource assetBundleResource2 = results[i] as IAssetBundleResource;
					if (assetBundleResource2 != null)
					{
						assetBundleResource2.GetAssetBundle();
						if (flag)
						{
							assetBundleResource = assetBundleResource2;
						}
						flag = false;
					}
				}
				return assetBundleResource as T;
			}

			internal static bool IsDownloadOnly(IList<object> results)
			{
				foreach (object obj in results)
				{
					AssetBundleResource assetBundleResource = obj as AssetBundleResource;
					if (assetBundleResource != null && assetBundleResource.m_DownloadOnly)
					{
						return true;
					}
				}
				return false;
			}

			public void Start(ProvideHandle provideHandle)
			{
				provideHandle.SetProgressCallback(new Func<float>(this.ProgressCallback));
				provideHandle.SetWaitForCompletionCallback(new Func<bool>(this.WaitForCompletionHandler));
				this.subObjectName = null;
				this.m_ProvideHandle = provideHandle;
				this.m_RequestOperation = null;
				List<object> list = new List<object>();
				this.m_ProvideHandle.GetDependencies(list);
				IAssetBundleResource assetBundleResource = BundledAssetProvider.InternalOp.LoadBundleFromDependecies<IAssetBundleResource>(list);
				if (assetBundleResource == null)
				{
					AssetBundle result = null;
					bool status = false;
					string str = "Unable to load dependent bundle from location ";
					IResourceLocation location = this.m_ProvideHandle.Location;
					this.m_ProvideHandle.Complete<AssetBundle>(result, status, new Exception(str + ((location != null) ? location.ToString() : null)));
					return;
				}
				this.m_AssetBundle = assetBundleResource.GetAssetBundle();
				if (this.m_AssetBundle == null)
				{
					AssetBundle result2 = null;
					bool status2 = false;
					string str2 = "Unable to load dependent bundle from location ";
					IResourceLocation location2 = this.m_ProvideHandle.Location;
					this.m_ProvideHandle.Complete<AssetBundle>(result2, status2, new Exception(str2 + ((location2 != null) ? location2.ToString() : null)));
					return;
				}
				AssetBundleResource assetBundleResource2 = assetBundleResource as AssetBundleResource;
				if (assetBundleResource2 != null)
				{
					this.m_PreloadRequest = assetBundleResource2.GetAssetPreloadRequest();
				}
				if (this.m_PreloadRequest == null || this.m_PreloadRequest.isDone)
				{
					this.BeginAssetLoad();
					return;
				}
				this.m_PreloadRequest.completed += delegate(AsyncOperation operation)
				{
					this.BeginAssetLoad();
				};
			}

			private void BeginAssetLoad()
			{
				if (this.m_AssetBundle == null)
				{
					AssetBundle result = null;
					bool status = false;
					string str = "Unable to load dependent bundle from location ";
					IResourceLocation location = this.m_ProvideHandle.Location;
					this.m_ProvideHandle.Complete<AssetBundle>(result, status, new Exception(str + ((location != null) ? location.ToString() : null)));
					return;
				}
				string text = this.m_ProvideHandle.ResourceManager.TransformInternalId(this.m_ProvideHandle.Location);
				string name;
				string text2;
				if (this.m_ProvideHandle.Type.IsArray)
				{
					this.m_RequestOperation = this.m_AssetBundle.LoadAssetWithSubAssetsAsync(text, this.m_ProvideHandle.Type.GetElementType());
				}
				else if (this.m_ProvideHandle.Type.IsGenericType && typeof(IList<>) == this.m_ProvideHandle.Type.GetGenericTypeDefinition())
				{
					this.m_RequestOperation = this.m_AssetBundle.LoadAssetWithSubAssetsAsync(text, this.m_ProvideHandle.Type.GetGenericArguments()[0]);
				}
				else if (ResourceManagerConfig.ExtractKeyAndSubKey(text, out name, out text2))
				{
					this.subObjectName = text2;
					this.m_RequestOperation = this.m_AssetBundle.LoadAssetWithSubAssetsAsync(name, this.m_ProvideHandle.Type);
				}
				else
				{
					this.m_RequestOperation = this.m_AssetBundle.LoadAssetAsync(text, this.m_ProvideHandle.Type);
				}
				if (this.m_RequestOperation != null)
				{
					if (this.m_RequestOperation.isDone)
					{
						this.ActionComplete(this.m_RequestOperation);
						return;
					}
					this.m_RequestOperation.completed += this.ActionComplete;
				}
			}

			private bool WaitForCompletionHandler()
			{
				if (this.m_PreloadRequest != null && !this.m_PreloadRequest.isDone)
				{
					return this.m_PreloadRequest.asset == null;
				}
				return this.m_Result != null || (this.m_RequestOperation != null && (this.m_RequestOperation.isDone || this.m_RequestOperation.asset != null));
			}

			private void ActionComplete(AsyncOperation obj)
			{
				if (this.m_RequestOperation != null)
				{
					if (this.m_ProvideHandle.Type.IsArray)
					{
						this.GetArrayResult(this.m_RequestOperation.allAssets);
					}
					else if (this.m_ProvideHandle.Type.IsGenericType && typeof(IList<>) == this.m_ProvideHandle.Type.GetGenericTypeDefinition())
					{
						this.GetListResult(this.m_RequestOperation.allAssets);
					}
					else if (string.IsNullOrEmpty(this.subObjectName))
					{
						this.GetAssetResult(this.m_RequestOperation.asset);
					}
					else
					{
						this.GetAssetSubObjectResult(this.m_RequestOperation.allAssets);
					}
				}
				this.CompleteOperation();
			}

			private void GetArrayResult(Object[] allAssets)
			{
				this.m_Result = ResourceManagerConfig.CreateArrayResult(this.m_ProvideHandle.Type, allAssets);
			}

			private void GetListResult(Object[] allAssets)
			{
				this.m_Result = ResourceManagerConfig.CreateListResult(this.m_ProvideHandle.Type, allAssets);
			}

			private void GetAssetResult(Object asset)
			{
				this.m_Result = ((asset != null && this.m_ProvideHandle.Type.IsAssignableFrom(asset.GetType())) ? asset : null);
			}

			private void GetAssetSubObjectResult(Object[] allAssets)
			{
				foreach (Object @object in allAssets)
				{
					if (@object.name == this.subObjectName && this.m_ProvideHandle.Type.IsAssignableFrom(@object.GetType()))
					{
						this.m_Result = @object;
						return;
					}
				}
			}

			private void CompleteOperation()
			{
				Exception exception = (this.m_Result == null) ? new Exception(string.Format("Unable to load asset of type {0} from location {1}.", this.m_ProvideHandle.Type, this.m_ProvideHandle.Location)) : null;
				this.m_ProvideHandle.Complete<object>(this.m_Result, this.m_Result != null, exception);
			}

			public float ProgressCallback()
			{
				if (this.m_RequestOperation == null)
				{
					return 0f;
				}
				return this.m_RequestOperation.progress;
			}

			private AssetBundle m_AssetBundle;

			private AssetBundleRequest m_PreloadRequest;

			private AssetBundleRequest m_RequestOperation;

			private object m_Result;

			private ProvideHandle m_ProvideHandle;

			private string subObjectName;
		}
	}
}
