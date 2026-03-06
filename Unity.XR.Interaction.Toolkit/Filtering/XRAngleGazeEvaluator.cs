using System;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Filtering
{
	[Serializable]
	public class XRAngleGazeEvaluator : XRTargetEvaluator
	{
		private static Camera GetXROriginCamera()
		{
			XROrigin xrorigin;
			if (!ComponentLocatorUtility<XROrigin>.TryFindComponent(out xrorigin))
			{
				return null;
			}
			return xrorigin.Camera;
		}

		public Transform gazeTransform
		{
			get
			{
				return this.m_GazeTransform;
			}
			set
			{
				this.m_GazeTransform = value;
			}
		}

		public float maxAngle
		{
			get
			{
				return this.m_MaxAngle;
			}
			set
			{
				this.m_MaxAngle = Mathf.Clamp(value, 0f, 180f);
			}
		}

		private void InitializeGazeTransform()
		{
			Camera xroriginCamera = XRAngleGazeEvaluator.GetXROriginCamera();
			if (xroriginCamera != null && xroriginCamera.enabled && xroriginCamera.gameObject.activeInHierarchy)
			{
				this.m_GazeTransform = xroriginCamera.transform;
				return;
			}
			Debug.LogWarning("Couldn't find an active XROrigin Camera for XRAngleGazeEvaluator", base.filter);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.m_GazeTransform == null)
			{
				this.InitializeGazeTransform();
			}
		}

		public override void Reset()
		{
			base.Reset();
			this.InitializeGazeTransform();
		}

		protected override float CalculateNormalizedScore(IXRInteractor interactor, IXRInteractable target)
		{
			Transform gazeTransform = this.gazeTransform;
			if (gazeTransform == null || this.m_MaxAngle <= 0f)
			{
				return 0f;
			}
			XRBaseInteractable xrbaseInteractable = target as XRBaseInteractable;
			Vector3 a;
			if (xrbaseInteractable != null)
			{
				a = xrbaseInteractable.GetDistance(gazeTransform.position).point;
			}
			else
			{
				Transform transform = target.transform;
				if (transform == null)
				{
					return 0f;
				}
				a = transform.position;
			}
			float num = Vector3.Angle(a - gazeTransform.position, gazeTransform.forward) * 2f / this.m_MaxAngle;
			return 1f - num;
		}

		[Tooltip("The Transform whose forward direction is used to evaluate the target Interactable angle. If none is specified, during OnEnable this property is initialized with the XROrigin Camera.")]
		[SerializeField]
		private Transform m_GazeTransform;

		[Tooltip("The maximum value an angle can be evaluated as before the Interactor receives a normalized score of 0. Think of it as a field-of-view angle.")]
		[SerializeField]
		[Range(0f, 180f)]
		private float m_MaxAngle = 60f;
	}
}
