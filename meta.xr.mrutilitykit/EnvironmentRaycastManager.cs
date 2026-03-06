using System;
using System.Runtime.CompilerServices;
using Meta.XR.EnvironmentDepth;
using UnityEngine;

namespace Meta.XR
{
	public class EnvironmentRaycastManager : MonoBehaviour
	{
		private static IEnvironmentRaycastProvider CreateProvider()
		{
			return new EnvironmentRaycastManager.EnvironmentRaycastProviderDepthManager();
		}

		private void Awake()
		{
			if (!EnvironmentRaycastManager.IsSupported)
			{
				Debug.LogError("EnvironmentRaycastManager is not supported. Please check the 'IsSupported' property before enabling this component.");
			}
			EnvironmentRaycastManager._instance = this;
		}

		private void OnDestroy()
		{
			EnvironmentRaycastManager._instance = null;
		}

		private void Start()
		{
			OVRTelemetry.Start(651891190, 0, -1L).Send();
		}

		private void OnEnable()
		{
			EnvironmentRaycastManager.SetProviderEnabled(true);
		}

		private void OnDisable()
		{
			EnvironmentRaycastManager.SetProviderEnabled(false);
		}

		private static void SetProviderEnabled(bool isEnabled)
		{
			if (EnvironmentRaycastManager.IsSupported)
			{
				EnvironmentRaycastManager._provider.SetEnabled(isEnabled);
			}
		}

		public static bool IsSupported
		{
			get
			{
				bool value = EnvironmentRaycastManager._isSupported.GetValueOrDefault();
				if (EnvironmentRaycastManager._isSupported == null)
				{
					value = EnvironmentRaycastManager._provider.IsSupported;
					EnvironmentRaycastManager._isSupported = new bool?(value);
				}
				return EnvironmentRaycastManager._isSupported.Value;
			}
		}

		public bool Raycast(Ray ray, out EnvironmentRaycastHit hit, float maxDistance = 100f)
		{
			if (!this.IsReady)
			{
				hit = new EnvironmentRaycastHit
				{
					status = EnvironmentRaycastHitStatus.NotReady
				};
				return false;
			}
			if (!EnvironmentRaycastManager.IsSupported)
			{
				hit = new EnvironmentRaycastHit
				{
					status = EnvironmentRaycastHitStatus.NotSupported
				};
				return false;
			}
			return EnvironmentRaycastManager._provider.Raycast(ray, out hit, maxDistance, true, true);
		}

		private static EnvironmentRaycastHit ToEnvRaycastHit(DepthRaycastHit depthHit)
		{
			return new EnvironmentRaycastHit
			{
				status = EnvironmentRaycastManager.<ToEnvRaycastHit>g__ToStatus|14_0(depthHit.result),
				point = depthHit.point,
				normal = depthHit.normal,
				normalConfidence = depthHit.normalConfidence
			};
		}

		public bool PlaceBox(Ray ray, Vector3 boxSize, Vector3 upwards, out EnvironmentRaycastHit hit)
		{
			if (!this.IsReady)
			{
				hit = new EnvironmentRaycastHit
				{
					status = EnvironmentRaycastHitStatus.NotReady
				};
				return false;
			}
			if (!EnvironmentRaycastManager.IsSupported)
			{
				hit = new EnvironmentRaycastHit
				{
					status = EnvironmentRaycastHitStatus.NotSupported
				};
				return false;
			}
			return EnvironmentRaycastManager._provider.PlaceBox(ray, boxSize, upwards, out hit, 100f);
		}

		public bool CheckBox(Vector3 center, Vector3 halfExtents, Quaternion orientation)
		{
			return this.IsReady && EnvironmentRaycastManager.IsSupported && EnvironmentRaycastManager._provider.CheckBox(center, halfExtents, orientation);
		}

		private bool IsReady
		{
			get
			{
				if (!base.enabled || !base.gameObject.activeInHierarchy)
				{
					Debug.LogError("Please enable the 'EnvironmentRaycastManager' component and its GameObject.", this);
					return false;
				}
				return EnvironmentRaycastManager._provider.IsReady;
			}
		}

		[CompilerGenerated]
		internal static EnvironmentRaycastHitStatus <ToEnvRaycastHit>g__ToStatus|14_0(DepthRaycastResult depthHitResult)
		{
			switch (depthHitResult)
			{
			case DepthRaycastResult.Success:
				return EnvironmentRaycastHitStatus.Hit;
			case DepthRaycastResult.HitPointOccluded:
				return EnvironmentRaycastHitStatus.HitPointOccluded;
			case DepthRaycastResult.NotReady:
				return EnvironmentRaycastHitStatus.NotReady;
			case DepthRaycastResult.RayOutsideOfDepthCameraFrustum:
				return EnvironmentRaycastHitStatus.HitPointOutsideOfCameraFrustum;
			case DepthRaycastResult.RayOccluded:
				return EnvironmentRaycastHitStatus.RayOccluded;
			case DepthRaycastResult.NoHit:
				return EnvironmentRaycastHitStatus.NoHit;
			default:
				throw new Exception(string.Format("Invalid result type: {0}.", depthHitResult));
			}
		}

		private static EnvironmentRaycastManager _instance;

		private static readonly IEnvironmentRaycastProvider _provider = EnvironmentRaycastManager.CreateProvider();

		private static bool? _isSupported;

		private class EnvironmentRaycastProviderDepthManager : IEnvironmentRaycastProvider
		{
			bool IEnvironmentRaycastProvider.IsReady
			{
				get
				{
					this.EnsureDepthManagerIsPresent();
					if (!this._depthManager.enabled || !this._depthManager.gameObject.activeInHierarchy)
					{
						Debug.LogError("Please enable the 'EnvironmentDepthManager' component and its GameObject.", this._depthManager);
						return false;
					}
					return true;
				}
			}

			private void EnsureDepthManagerIsPresent()
			{
				if (this._depthManager == null)
				{
					this._depthManager = Object.FindAnyObjectByType<EnvironmentDepthManager>(FindObjectsInactive.Include);
					if (this._depthManager == null)
					{
						this._depthManager = new GameObject("EnvironmentDepthManager").AddComponent<EnvironmentDepthManager>();
						Debug.LogWarning("EnvironmentDepthManager was added to the scene by EnvironmentRaycastManager. Please add EnvironmentDepthManager to prevent this warning.");
					}
				}
			}

			void IEnvironmentRaycastProvider.SetEnabled(bool isEnabled)
			{
				if (!isEnabled)
				{
					if (EnvironmentRaycastManager.IsSupported && this._depthManager != null)
					{
						this._depthManager.SetRaycastWarmUpEnabled(false);
					}
					return;
				}
				if (EnvironmentRaycastManager.IsSupported)
				{
					this.EnsureDepthManagerIsPresent();
					this._depthManager.SetRaycastWarmUpEnabled(true);
					return;
				}
				string text = "EnvironmentRaycastManager is not supported. Requirements: Quest 3 or newer, Unity >= 2022.3.\n";
				if (Application.isEditor)
				{
					text += "To run the EnvironmentRaycastManager in Editor, please use Meta Quest Link.\n";
				}
				Debug.LogError(text);
			}

			bool IEnvironmentRaycastProvider.IsSupported
			{
				get
				{
					return EnvironmentDepthManager.IsSupported;
				}
			}

			bool IEnvironmentRaycastProvider.Raycast(Ray ray, out EnvironmentRaycastHit hit, float maxDistance, bool reconstructNormal, bool allowOccludedRayOrigin)
			{
				DepthRaycastHit depthHit;
				bool result = this._depthManager.Raycast(ray, out depthHit, maxDistance, Eye.Both, reconstructNormal, allowOccludedRayOrigin);
				hit = EnvironmentRaycastManager.ToEnvRaycastHit(depthHit);
				return result;
			}

			private EnvironmentDepthManager _depthManager;
		}
	}
}
