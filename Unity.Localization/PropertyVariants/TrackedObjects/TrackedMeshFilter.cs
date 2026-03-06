using System;
using UnityEngine.Localization.PropertyVariants.TrackedProperties;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization.PropertyVariants.TrackedObjects
{
	[DisplayName("Mesh Filter", null)]
	[CustomTrackedObject(typeof(MeshFilter), false)]
	[Serializable]
	public class TrackedMeshFilter : TrackedObject
	{
		public override bool CanTrackProperty(string propertyPath)
		{
			return propertyPath == "m_Mesh";
		}

		public override AsyncOperationHandle ApplyLocale(Locale variantLocale, Locale defaultLocale)
		{
			if (base.TrackedProperties.Count == 0)
			{
				return default(AsyncOperationHandle);
			}
			if (this.m_CurrentOperation.IsValid())
			{
				if (!this.m_CurrentOperation.IsDone)
				{
					this.m_CurrentOperation.Completed -= this.MeshOperationCompleted;
				}
				AddressablesInterface.SafeRelease(this.m_CurrentOperation);
				this.m_CurrentOperation = default(AsyncOperationHandle<Mesh>);
			}
			ITrackedProperty trackedProperty = base.TrackedProperties[0];
			UnityObjectProperty unityObjectProperty = trackedProperty as UnityObjectProperty;
			if (unityObjectProperty != null)
			{
				LocaleIdentifier fallback = (defaultLocale != null) ? defaultLocale.Identifier : default(LocaleIdentifier);
				Object @object;
				if (unityObjectProperty.GetValue(variantLocale.Identifier, fallback, out @object))
				{
					this.SetMesh(@object as Mesh);
				}
			}
			else
			{
				LocalizedAssetProperty localizedAssetProperty = trackedProperty as LocalizedAssetProperty;
				if (localizedAssetProperty != null && !localizedAssetProperty.LocalizedObject.IsEmpty)
				{
					this.m_CurrentOperation = localizedAssetProperty.LocalizedObject.LoadAssetAsync<Mesh>();
					if (this.m_CurrentOperation.IsDone)
					{
						this.MeshOperationCompleted(this.m_CurrentOperation);
					}
					else
					{
						if (!localizedAssetProperty.LocalizedObject.ForceSynchronous)
						{
							this.m_CurrentOperation.Completed += this.MeshOperationCompleted;
							return this.m_CurrentOperation;
						}
						this.m_CurrentOperation.WaitForCompletion();
						this.MeshOperationCompleted(this.m_CurrentOperation);
					}
				}
			}
			return default(AsyncOperationHandle);
		}

		private void MeshOperationCompleted(AsyncOperationHandle<Mesh> assetOp)
		{
			this.SetMesh(assetOp.Result);
		}

		private void SetMesh(Mesh mesh)
		{
			((MeshFilter)base.Target).sharedMesh = mesh;
		}

		private const string k_MeshProperty = "m_Mesh";

		private AsyncOperationHandle<Mesh> m_CurrentOperation;
	}
}
