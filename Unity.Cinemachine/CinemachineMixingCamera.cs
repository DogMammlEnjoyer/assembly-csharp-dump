using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[DisallowMultipleComponent]
	[ExecuteAlways]
	[ExcludeFromPreset]
	[AddComponentMenu("Cinemachine/Cinemachine Mixing Camera")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineMixingCamera.html")]
	public class CinemachineMixingCamera : CinemachineCameraManagerBase
	{
		private void OnValidate()
		{
			for (int i = 0; i < 8; i++)
			{
				this.SetWeight(i, Mathf.Max(0f, this.GetWeight(i)));
			}
		}

		protected override void Reset()
		{
			base.Reset();
			for (int i = 0; i < 8; i++)
			{
				this.SetWeight(i, (float)((i == 0) ? 1 : 0));
			}
		}

		public override CameraState State
		{
			get
			{
				return this.m_CameraState;
			}
		}

		public override string Description
		{
			get
			{
				if (base.LiveChild == null)
				{
					return "[(none)]";
				}
				StringBuilder stringBuilder = CinemachineDebug.SBFromPool();
				stringBuilder.Append("[");
				stringBuilder.Append(base.LiveChild.Name);
				stringBuilder.Append(" ");
				stringBuilder.Append(Mathf.RoundToInt(this.m_LiveChildPercent));
				stringBuilder.Append("%]");
				string result = stringBuilder.ToString();
				CinemachineDebug.ReturnToPool(stringBuilder);
				return result;
			}
		}

		public float GetWeight(int index)
		{
			switch (index)
			{
			case 0:
				return this.Weight0;
			case 1:
				return this.Weight1;
			case 2:
				return this.Weight2;
			case 3:
				return this.Weight3;
			case 4:
				return this.Weight4;
			case 5:
				return this.Weight5;
			case 6:
				return this.Weight6;
			case 7:
				return this.Weight7;
			default:
				Debug.LogError("CinemachineMixingCamera: Invalid index: " + index.ToString());
				return 0f;
			}
		}

		public void SetWeight(int index, float w)
		{
			switch (index)
			{
			case 0:
				this.Weight0 = w;
				return;
			case 1:
				this.Weight1 = w;
				return;
			case 2:
				this.Weight2 = w;
				return;
			case 3:
				this.Weight3 = w;
				return;
			case 4:
				this.Weight4 = w;
				return;
			case 5:
				this.Weight5 = w;
				return;
			case 6:
				this.Weight6 = w;
				return;
			case 7:
				this.Weight7 = w;
				return;
			default:
				Debug.LogError("CinemachineMixingCamera: Invalid index: " + index.ToString());
				return;
			}
		}

		public float GetWeight(CinemachineVirtualCameraBase vcam)
		{
			this.UpdateCameraCache();
			int index;
			if (this.m_IndexMap.TryGetValue(vcam, out index))
			{
				return this.GetWeight(index);
			}
			return 0f;
		}

		public void SetWeight(CinemachineVirtualCameraBase vcam, float w)
		{
			this.UpdateCameraCache();
			int index;
			if (this.m_IndexMap.TryGetValue(vcam, out index))
			{
				this.SetWeight(index, w);
				return;
			}
			Debug.LogError("CinemachineMixingCamera: Invalid child: " + ((vcam != null) ? vcam.Name : "(null)"));
		}

		public override bool IsLiveChild(ICinemachineCamera vcam, bool dominantChildOnly = false)
		{
			if (dominantChildOnly)
			{
				return base.LiveChild == vcam;
			}
			List<CinemachineVirtualCameraBase> childCameras = base.ChildCameras;
			int num = 0;
			while (num < 8 && num < childCameras.Count)
			{
				if (childCameras[num] == vcam)
				{
					return this.GetWeight(num) > 0.0001f && childCameras[num].isActiveAndEnabled;
				}
				num++;
			}
			return false;
		}

		protected override bool UpdateCameraCache()
		{
			if (!base.UpdateCameraCache())
			{
				return false;
			}
			this.m_IndexMap = new Dictionary<CinemachineVirtualCameraBase, int>();
			for (int i = 0; i < base.ChildCameras.Count; i++)
			{
				this.m_IndexMap.Add(base.ChildCameras[i], i);
			}
			return true;
		}

		public override void OnTransitionFromCamera(ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
		{
			int num = 0;
			while (num < 8 && num < base.ChildCameras.Count)
			{
				base.ChildCameras[num].OnTransitionFromCamera(fromCam, worldUp, deltaTime);
				num++;
			}
			base.OnTransitionFromCamera(fromCam, worldUp, deltaTime);
		}

		public override void InternalUpdateCameraState(Vector3 worldUp, float deltaTime)
		{
			this.UpdateCameraCache();
			CinemachineVirtualCameraBase activeCamera = null;
			List<CinemachineVirtualCameraBase> childCameras = base.ChildCameras;
			float num = 0f;
			float num2 = 0f;
			int num3 = 0;
			while (num3 < 8 && num3 < childCameras.Count)
			{
				CinemachineVirtualCameraBase cinemachineVirtualCameraBase = childCameras[num3];
				if (cinemachineVirtualCameraBase.isActiveAndEnabled)
				{
					float num4 = Mathf.Max(0f, this.GetWeight(num3));
					if (num4 > 0.0001f)
					{
						num2 += num4;
						if (num2 == num4)
						{
							this.m_CameraState = cinemachineVirtualCameraBase.State;
						}
						else
						{
							CameraState state = cinemachineVirtualCameraBase.State;
							this.m_CameraState = CameraState.Lerp(this.m_CameraState, state, num4 / num2);
						}
						if (num4 > num)
						{
							num = num4;
							activeCamera = cinemachineVirtualCameraBase;
						}
					}
				}
				num3++;
			}
			this.m_LiveChildPercent = ((num2 > 0.001f) ? (num * 100f / num2) : 0f);
			base.SetLiveChild(activeCamera, worldUp, deltaTime);
			base.InvokePostPipelineStageCallback(this, CinemachineCore.Stage.Finalize, ref this.m_CameraState, deltaTime);
			this.PreviousStateIsValid = true;
		}

		protected override CinemachineVirtualCameraBase ChooseCurrentCamera(Vector3 worldUp, float deltaTime)
		{
			return null;
		}

		public const int MaxCameras = 8;

		[Tooltip("The weight of the first tracked camera")]
		[FormerlySerializedAs("m_Weight0")]
		public float Weight0 = 0.5f;

		[Tooltip("The weight of the second tracked camera")]
		[FormerlySerializedAs("m_Weight1")]
		public float Weight1 = 0.5f;

		[Tooltip("The weight of the third tracked camera")]
		[FormerlySerializedAs("m_Weight2")]
		public float Weight2 = 0.5f;

		[Tooltip("The weight of the fourth tracked camera")]
		[FormerlySerializedAs("m_Weight3")]
		public float Weight3 = 0.5f;

		[Tooltip("The weight of the fifth tracked camera")]
		[FormerlySerializedAs("m_Weight4")]
		public float Weight4 = 0.5f;

		[Tooltip("The weight of the sixth tracked camera")]
		[FormerlySerializedAs("m_Weight5")]
		public float Weight5 = 0.5f;

		[Tooltip("The weight of the seventh tracked camera")]
		[FormerlySerializedAs("m_Weight6")]
		public float Weight6 = 0.5f;

		[Tooltip("The weight of the eighth tracked camera")]
		[FormerlySerializedAs("m_Weight7")]
		public float Weight7 = 0.5f;

		private CameraState m_CameraState = CameraState.Default;

		private Dictionary<CinemachineVirtualCameraBase, int> m_IndexMap;

		private float m_LiveChildPercent;
	}
}
