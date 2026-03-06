using System;
using Liv.Lck.Tablet;
using Liv.Lck.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Liv.Lck
{
	public class LckTabletFollow : MonoBehaviour
	{
		private void OnEnable()
		{
			this._isFollowToggleOn = this._isFollowingToggle.isOn;
			this._isFollowingToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnIsFollowToggled));
			this._followDistanceDoubleButton.OnValueChanged += this.OnFollowDistanceChanged;
			this._smoothingDoubleButton.OnValueChanged += this.OnSmoothingChanged;
			LCKCameraController controller = this._controller;
			controller.OnCameraModeChanged = (Action<CameraMode>)Delegate.Combine(controller.OnCameraModeChanged, new Action<CameraMode>(this.OnCameraModeChanged));
		}

		private void OnDisable()
		{
			this._isFollowingToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnIsFollowToggled));
			this._followDistanceDoubleButton.OnValueChanged -= this.OnFollowDistanceChanged;
			this._smoothingDoubleButton.OnValueChanged -= this.OnSmoothingChanged;
			LCKCameraController controller = this._controller;
			controller.OnCameraModeChanged = (Action<CameraMode>)Delegate.Remove(controller.OnCameraModeChanged, new Action<CameraMode>(this.OnCameraModeChanged));
		}

		private void Start()
		{
			this.SetInitialValuesFromDoubleButtons();
			this._isInCorrectCameraMode = true;
			this._targetPosition = base.transform.position;
			if (this._rigidbodyRoot != null)
			{
				this._defaultInterpolation = this._rigidbodyRoot.interpolation;
			}
		}

		private void SetInitialValuesFromDoubleButtons()
		{
			this._minFollowDistance = this._followDistanceDoubleButton.Value * this._minFollowDistanceMultiplier;
			this._followSmoothing = this.CalculateFollowSmoothing(this._smoothingDoubleButton.Value);
		}

		private void FixedUpdate()
		{
			this.ProcessTabletFollowingWithRigidbody();
		}

		public void SetFollowTarget(Transform target)
		{
			this._followTarget = target;
		}

		private void ProcessTabletFollowingWithRigidbody()
		{
			if (!this._isInCorrectCameraMode || !this._isFollowToggleOn)
			{
				return;
			}
			Vector3 vector = (!this._followTarget) ? (this._controller.HmdTransform.position + Vector3.down * this._heightOffsetForPlayerHead) : this._followTarget.position;
			Vector3 position = this._rigidbodyRoot.position;
			Vector3 vector2 = position - vector;
			bool flag = vector2.magnitude < this._minFollowDistance;
			this._targetPosition = vector + vector2.normalized * this._minFollowDistance;
			Vector3 position2 = Vector3.SmoothDamp(position, flag ? position : this._targetPosition, ref this._followVelocity, this._followSmoothing);
			this._rigidbodyRoot.MovePosition(position2);
			Vector3 position3 = this._selfieCamera.transform.position;
			this._rigidbodyRoot.LookAtFromPivotPoint(position3, vector - position3, position2, this._rigidbodyRoot.rotation);
		}

		private void OnCameraModeChanged(CameraMode mode)
		{
			this._isInCorrectCameraMode = (mode == CameraMode.Selfie);
		}

		private void OnIsFollowToggled(bool value)
		{
			this._isFollowToggleOn = value;
			if (this._rigidbodyRoot != null)
			{
				this._rigidbodyRoot.interpolation = (this._isFollowToggleOn ? RigidbodyInterpolation.Interpolate : this._defaultInterpolation);
			}
		}

		private void OnSmoothingChanged(float value)
		{
			this._followSmoothing = this.CalculateFollowSmoothing(value);
		}

		private float CalculateFollowSmoothing(float value)
		{
			return Mathf.Max(value / 10f, this._minFollowSmoothing);
		}

		private void OnFollowDistanceChanged(float value)
		{
			this._minFollowDistance = value * this._minFollowDistanceMultiplier;
		}

		[Header("Settings")]
		[Tooltip("An offset applied to the HMD's position to estimate the player's head/neck position. A small downward offset is typical.")]
		[SerializeField]
		private float _heightOffsetForPlayerHead;

		[Tooltip("The minimum smoothing value, preventing the tablet from becoming too rigid even at the lowest user setting.")]
		[SerializeField]
		private float _minFollowSmoothing = 0.2f;

		[Tooltip("A multiplier applied to the value from the follow distance UI button to determine the actual follow distance in world units.")]
		[SerializeField]
		private float _minFollowDistanceMultiplier = 0.75f;

		[Header("References")]
		[Tooltip("A reference to the main camera controller to get access to the HMD transform.")]
		[SerializeField]
		private LCKCameraController _controller;

		[Tooltip("The UI toggle that enables or disables the follow behavior.")]
		[SerializeField]
		private Toggle _isFollowingToggle;

		[Tooltip("A reference to the transform of the virtual selfie camera. The tablet will orient itself based on this camera's position.")]
		[SerializeField]
		private Transform _selfieCamera;

		[Tooltip("An optional, specific transform for the tablet to follow. If this is not set, it will default to following the user's HMD (player head).")]
		[SerializeField]
		private Transform _followTarget;

		[Tooltip("The UI button used to adjust the follow smoothing.")]
		[SerializeField]
		private LckDoubleButton _smoothingDoubleButton;

		[Tooltip("The UI button used to adjust the minimum follow distance.")]
		[SerializeField]
		private LckDoubleButton _followDistanceDoubleButton;

		[Tooltip("The root Rigidbody of the tablet. All movement is applied to this component.")]
		[SerializeField]
		private Rigidbody _rigidbodyRoot;

		private bool _isInCorrectCameraMode = true;

		private bool _isFollowToggleOn;

		private Vector3 _followVelocity;

		private Vector3 _targetPosition;

		private float _minFollowDistance;

		private float _followSmoothing;

		private RigidbodyInterpolation _defaultInterpolation;
	}
}
