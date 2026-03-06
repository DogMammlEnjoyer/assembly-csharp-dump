using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnityEngine.Rendering
{
	[Obsolete("This is not longer supported Please use DebugDisplaySettingsVolume. #from(6000.2)", false)]
	public abstract class VolumeDebugSettings<T> : IVolumeDebugSettings where T : MonoBehaviour, IAdditionalData
	{
		public int selectedComponent { get; set; }

		public Camera selectedCamera
		{
			get
			{
				if (this.selectedCameraIndex >= 0)
				{
					return this.cameras.ElementAt(this.selectedCameraIndex);
				}
				return null;
			}
		}

		public int selectedCameraIndex
		{
			get
			{
				int num = this.cameras.Count<Camera>();
				if (num <= 0)
				{
					return -1;
				}
				return Math.Clamp(this.m_SelectedCameraIndex, 0, num - 1);
			}
			set
			{
				int num = this.cameras.Count<Camera>();
				this.m_SelectedCameraIndex = Math.Clamp(value, 0, num - 1);
			}
		}

		public IEnumerable<Camera> cameras
		{
			get
			{
				this.m_Cameras.Clear();
				if (this.m_CamerasArray == null || this.m_CamerasArray.Length != Camera.allCamerasCount)
				{
					this.m_CamerasArray = new Camera[Camera.allCamerasCount];
				}
				Camera.GetAllCameras(this.m_CamerasArray);
				foreach (Camera camera in this.m_CamerasArray)
				{
					if (!(camera == null) && camera.cameraType != CameraType.Preview && camera.cameraType != CameraType.Reflection)
					{
						T t;
						if (!camera.TryGetComponent<T>(out t))
						{
							t = camera.gameObject.AddComponent<T>();
						}
						if (t != null)
						{
							this.m_Cameras.Add(camera);
						}
					}
				}
				return this.m_Cameras;
			}
		}

		public abstract VolumeStack selectedCameraVolumeStack { get; }

		public abstract LayerMask selectedCameraLayerMask { get; }

		public abstract Vector3 selectedCameraPosition { get; }

		public Type selectedComponentType
		{
			get
			{
				if (this.selectedComponent <= 0)
				{
					return null;
				}
				return this.volumeComponentsPathAndType[this.selectedComponent - 1].Item2;
			}
			set
			{
				int num = this.volumeComponentsPathAndType.FindIndex((ValueTuple<string, Type> t) => t.Item2 == value);
				if (num != -1)
				{
					this.selectedComponent = num + 1;
				}
			}
		}

		public List<ValueTuple<string, Type>> volumeComponentsPathAndType
		{
			get
			{
				return VolumeManager.instance.GetVolumeComponentsForDisplay(GraphicsSettings.currentRenderPipelineAssetType);
			}
		}

		[Obsolete("This property is obsolete and kept only for not breaking user code. VolumeDebugSettings will use current pipeline when it needs to gather volume component types and paths. #from(23.2)", false)]
		public virtual Type targetRenderPipeline { get; }

		internal VolumeParameter GetParameter(VolumeComponent component, FieldInfo field)
		{
			return (VolumeParameter)field.GetValue(component);
		}

		internal VolumeParameter GetParameter(FieldInfo field)
		{
			VolumeStack selectedCameraVolumeStack = this.selectedCameraVolumeStack;
			if (selectedCameraVolumeStack != null)
			{
				return this.GetParameter(selectedCameraVolumeStack.GetComponent(this.selectedComponentType), field);
			}
			return null;
		}

		internal VolumeParameter GetParameter(Volume volume, FieldInfo field)
		{
			VolumeComponent component;
			if (!(volume.HasInstantiatedProfile() ? volume.profile : volume.sharedProfile).TryGet<VolumeComponent>(this.selectedComponentType, out component))
			{
				return null;
			}
			VolumeParameter parameter = this.GetParameter(component, field);
			if (!parameter.overrideState)
			{
				return null;
			}
			return parameter;
		}

		private float ComputeWeight(Volume volume, Vector3 triggerPos)
		{
			if (volume == null)
			{
				return 0f;
			}
			VolumeProfile volumeProfile = volume.HasInstantiatedProfile() ? volume.profile : volume.sharedProfile;
			if (!volume.gameObject.activeInHierarchy)
			{
				return 0f;
			}
			if (!volume.enabled || volumeProfile == null || volume.weight <= 0f)
			{
				return 0f;
			}
			VolumeComponent volumeComponent;
			if (!volumeProfile.TryGet<VolumeComponent>(this.selectedComponentType, out volumeComponent))
			{
				return 0f;
			}
			if (!volumeComponent.active)
			{
				return 0f;
			}
			float num = Mathf.Clamp01(volume.weight);
			if (!volume.isGlobal)
			{
				List<Collider> colliders = volume.colliders;
				float num2 = float.PositiveInfinity;
				foreach (Collider collider in colliders)
				{
					if (collider.enabled)
					{
						float sqrMagnitude = (collider.ClosestPoint(triggerPos) - triggerPos).sqrMagnitude;
						if (sqrMagnitude < num2)
						{
							num2 = sqrMagnitude;
						}
					}
				}
				float num3 = volume.blendDistance * volume.blendDistance;
				if (num2 > num3)
				{
					num = 0f;
				}
				else if (num3 > 0f)
				{
					num *= 1f - num2 / num3;
				}
			}
			return num;
		}

		public Volume[] GetVolumes()
		{
			return (from v in VolumeManager.instance.GetVolumes(this.selectedCameraLayerMask)
			where v.sharedProfile != null
			select v).Reverse<Volume>().ToArray<Volume>();
		}

		private VolumeParameter[,] GetStates()
		{
			FieldInfo[] array = (from t in this.selectedComponentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			where t.FieldType.IsSubclassOf(typeof(VolumeParameter))
			select t).ToArray<FieldInfo>();
			VolumeParameter[,] array2 = new VolumeParameter[this.volumes.Length, array.Length];
			for (int i = 0; i < this.volumes.Length; i++)
			{
				VolumeComponent component;
				if ((this.volumes[i].HasInstantiatedProfile() ? this.volumes[i].profile : this.volumes[i].sharedProfile).TryGet<VolumeComponent>(this.selectedComponentType, out component))
				{
					for (int j = 0; j < array.Length; j++)
					{
						VolumeParameter parameter = this.GetParameter(component, array[j]);
						array2[i, j] = (parameter.overrideState ? parameter : null);
					}
				}
			}
			return array2;
		}

		private bool ChangedStates(VolumeParameter[,] newStates)
		{
			if (this.savedStates.GetLength(1) != newStates.GetLength(1))
			{
				return true;
			}
			for (int i = 0; i < this.savedStates.GetLength(0); i++)
			{
				for (int j = 0; j < this.savedStates.GetLength(1); j++)
				{
					if (this.savedStates[i, j] == null != (newStates[i, j] == null))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool RefreshVolumes(Volume[] newVolumes)
		{
			bool result = false;
			if (this.volumes == null || !newVolumes.SequenceEqual(this.volumes))
			{
				this.volumes = (Volume[])newVolumes.Clone();
				this.savedStates = this.GetStates();
				result = true;
			}
			else
			{
				VolumeParameter[,] states = this.GetStates();
				if (this.savedStates == null || this.ChangedStates(states))
				{
					this.savedStates = states;
					result = true;
				}
			}
			Vector3 selectedCameraPosition = this.selectedCameraPosition;
			this.weights = new float[this.volumes.Length];
			for (int i = 0; i < this.volumes.Length; i++)
			{
				this.weights[i] = this.ComputeWeight(this.volumes[i], selectedCameraPosition);
			}
			return result;
		}

		public float GetVolumeWeight(Volume volume)
		{
			Vector3 selectedCameraPosition = this.selectedCameraPosition;
			return this.ComputeWeight(volume, selectedCameraPosition);
		}

		public bool VolumeHasInfluence(Volume volume)
		{
			Vector3 selectedCameraPosition = this.selectedCameraPosition;
			return this.ComputeWeight(volume, selectedCameraPosition) > 0f;
		}

		[Obsolete("Please use volumeComponentsPathAndType instead, and get the second element of the tuple", false)]
		public static List<Type> componentTypes
		{
			get
			{
				if (VolumeDebugSettings<T>.s_ComponentTypes == null)
				{
					VolumeDebugSettings<T>.s_ComponentTypes = (from t in VolumeManager.instance.baseComponentTypeArray
					where !t.IsDefined(typeof(HideInInspector), false)
					where !t.IsDefined(typeof(ObsoleteAttribute), false)
					orderby VolumeDebugSettings<T>.ComponentDisplayName(t)
					select t).ToList<Type>();
				}
				return VolumeDebugSettings<T>.s_ComponentTypes;
			}
		}

		[Obsolete("Please use componentPathAndType instead, and get the first element of the tuple", false)]
		public static string ComponentDisplayName(Type component)
		{
			VolumeComponentMenuForRenderPipeline volumeComponentMenuForRenderPipeline = component.GetCustomAttribute(typeof(VolumeComponentMenuForRenderPipeline), false) as VolumeComponentMenuForRenderPipeline;
			if (volumeComponentMenuForRenderPipeline != null)
			{
				return volumeComponentMenuForRenderPipeline.menu;
			}
			VolumeComponentMenuForRenderPipeline volumeComponentMenuForRenderPipeline2 = component.GetCustomAttribute(typeof(VolumeComponentMenu), false) as VolumeComponentMenuForRenderPipeline;
			if (volumeComponentMenuForRenderPipeline2 != null)
			{
				return volumeComponentMenuForRenderPipeline2.menu;
			}
			return component.Name;
		}

		[Obsolete("Cameras are auto registered/unregistered, use property cameras", false)]
		private protected static List<T> additionalCameraDatas { protected get; private set; } = new List<T>();

		[Obsolete("Cameras are auto registered/unregistered", false)]
		public static void RegisterCamera(T additionalCamera)
		{
			if (!VolumeDebugSettings<T>.additionalCameraDatas.Contains(additionalCamera))
			{
				VolumeDebugSettings<T>.additionalCameraDatas.Add(additionalCamera);
			}
		}

		[Obsolete("Cameras are auto registered/unregistered", false)]
		public static void UnRegisterCamera(T additionalCamera)
		{
			if (VolumeDebugSettings<T>.additionalCameraDatas.Contains(additionalCamera))
			{
				VolumeDebugSettings<T>.additionalCameraDatas.Remove(additionalCamera);
			}
		}

		protected int m_SelectedCameraIndex = -1;

		private Camera[] m_CamerasArray;

		private List<Camera> m_Cameras = new List<Camera>();

		private float[] weights;

		private Volume[] volumes;

		private VolumeParameter[,] savedStates;

		private static List<Type> s_ComponentTypes;
	}
}
