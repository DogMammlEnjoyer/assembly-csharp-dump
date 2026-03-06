using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Liv.Lck.GorillaTag
{
	public class GtTabletFollower : MonoBehaviour
	{
		private void OnEnable()
		{
			this._isFollowingToggle.onValueChanged.AddListener(new UnityAction<bool>(this.SetIsFollowing));
			this._minDistanceCounter.onValueChanged.AddListener(new UnityAction<int>(this.SetMinDistance));
			this._smoothingCounter.onValueChanged.AddListener(new UnityAction<int>(this.SetSmoothing));
			if (this._controller != null)
			{
				this._controller.OnCameraModeChanged += this.OnCameraModeChanged;
			}
		}

		private void OnCameraModeChanged(CameraMode mode, ILckCamera camera)
		{
			this._currentCameraMode = mode;
			if (mode == CameraMode.Selfie)
			{
				this._isEnabled = true;
				return;
			}
			this._isEnabled = false;
		}

		private void OnDisable()
		{
			GtToggle isFollowingToggle = this._isFollowingToggle;
			if (isFollowingToggle != null)
			{
				isFollowingToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.SetIsFollowing));
			}
			GtCounter minDistanceCounter = this._minDistanceCounter;
			if (minDistanceCounter != null)
			{
				minDistanceCounter.onValueChanged.RemoveListener(new UnityAction<int>(this.SetMinDistance));
			}
			GtCounter smoothingCounter = this._smoothingCounter;
			if (smoothingCounter != null)
			{
				smoothingCounter.onValueChanged.RemoveListener(new UnityAction<int>(this.SetSmoothing));
			}
			if (this._controller != null)
			{
				this._controller.OnCameraModeChanged -= this.OnCameraModeChanged;
			}
		}

		private void Start()
		{
			this._isEnabled = true;
			this._canUpdate = true;
			this._targetPosition = base.transform.position;
			this._playerHead = this.FindPlayerHeadTransform();
		}

		private void Update()
		{
			this.ProcessTabletFollowing();
		}

		public void SetPlayerSizeModifier(bool isDefaultScale, float modifier)
		{
			if (isDefaultScale)
			{
				this._playerSizeOffset = Vector3.down;
				this._playerSizeModifier = 1f;
				return;
			}
			this._playerSizeOffset = Vector3.down * modifier;
			this._playerSizeModifier = modifier;
		}

		public float GetPlayerSizeModifier()
		{
			return this._playerSizeModifier;
		}

		public void SetCanUpdate(bool value)
		{
			this._canUpdate = value;
		}

		public void RepositionNearPlayer()
		{
			if (!this._isEnabled)
			{
				return;
			}
			if (!this._playerHead)
			{
				this._playerHead = this.FindPlayerHeadTransform();
			}
			if (!this._playerHead)
			{
				return;
			}
			Vector3 vector;
			if (this._repositionInFrontOfPlayer)
			{
				Vector3 forward = this._playerHead.forward;
				forward.y = 0f;
				forward.Normalize();
				vector = this._playerHead.position + forward + Vector3.down * this._heightOffsetForPlayerHead;
			}
			else
			{
				Vector3 vector2 = this._playerHead.position + Vector3.down * this._heightOffsetForPlayerHead;
				Vector3 b = base.transform.position - vector2;
				b.y = 0f;
				b.Normalize();
				vector = vector2 + b;
			}
			if (this._smoothRepositioning)
			{
				if (this._repositioningAnimation != null)
				{
					base.StopCoroutine(this._repositioningAnimation);
				}
				this._repositioningAnimation = base.StartCoroutine(this.RepositioningAnimation(vector));
				return;
			}
			base.transform.position = vector;
		}

		public void ResetFollowTarget()
		{
			if (!this._isEnabled)
			{
				return;
			}
			this._targetPosition = base.transform.position;
			if (this._isFollowing && this._playerHead)
			{
				base.transform.LookAt(this._playerHead.position);
			}
			this._followVelocity = Vector3.zero;
		}

		public void IsEnabled(bool value)
		{
			if (this._currentCameraMode != CameraMode.Selfie)
			{
				return;
			}
			this._isEnabled = value;
		}

		private void ProcessTabletFollowing()
		{
			if (!this._isEnabled)
			{
				return;
			}
			if (!this._canUpdate)
			{
				return;
			}
			if (!this._playerHead)
			{
				return;
			}
			if (!this._isFollowing)
			{
				return;
			}
			Vector3 vector = this._playerHead.position + this._playerSizeOffset * this._heightOffsetForPlayerHead;
			Vector3 position = base.transform.position;
			Vector3 vector2 = position - vector;
			float num = this._minDistance;
			if (this._playerSizeModifier != 1f)
			{
				num *= this._playerSizeModifier;
			}
			bool flag = vector2.magnitude < num;
			this._targetPosition = vector + vector2.normalized * num;
			Vector3 position2 = Vector3.SmoothDamp(position, flag ? position : this._targetPosition, ref this._followVelocity, this._smoothing);
			if (this._smoothing > 0f)
			{
				Quaternion b = Quaternion.LookRotation(vector - base.transform.position);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * (this._rotationSpeed * this.InvertSmoothingValue(this._smoothing)));
			}
			else
			{
				base.transform.LookAt(vector);
			}
			base.transform.position = position2;
		}

		private float InvertSmoothingValue(float originalValue)
		{
			return Mathf.Clamp(1.1f - originalValue, 0.1f, 1f);
		}

		private Transform FindPlayerHeadTransform()
		{
			Transform result = null;
			this._playerCamera = Camera.main;
			if (this._playerCamera != null)
			{
				Transform transform;
				result = (GtTag.TryGetTransform(GtTagType.Player, out transform) ? transform : this._playerCamera.transform);
			}
			return result;
		}

		private IEnumerator RepositioningAnimation(Vector3 targetPosition)
		{
			float time = 0f;
			Vector3 startPosition = base.transform.position;
			while (time < this._repositioningDuration)
			{
				time += Time.deltaTime;
				float t = this._repositioningCurve.Evaluate(time / this._repositioningDuration);
				base.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
				yield return null;
			}
			yield break;
		}

		private void SetIsFollowing(bool value)
		{
			this._isFollowing = value;
		}

		private void SetMinDistance(int value)
		{
			this._minDistance = (float)value;
		}

		private void SetSmoothing(int value)
		{
			this._smoothing = (float)value / 1000f;
		}

		[Header("Settings")]
		[SerializeField]
		private float _heightOffsetForPlayerHead;

		[SerializeField]
		private float _rotationSpeed = 17f;

		[SerializeField]
		private bool _smoothRepositioning;

		[SerializeField]
		private bool _repositionInFrontOfPlayer;

		[SerializeField]
		private float _repositioningDuration;

		[SerializeField]
		private AnimationCurve _repositioningCurve;

		[Header("Elements")]
		[SerializeField]
		private GtToggle _isFollowingToggle;

		[SerializeField]
		private GtCounter _minDistanceCounter;

		[SerializeField]
		private GtCounter _smoothingCounter;

		[SerializeField]
		private GTLckController _controller;

		private bool _canUpdate;

		private bool _isEnabled;

		private bool _isFollowing;

		private float _minDistance;

		private float _smoothing;

		private Coroutine _repositioningAnimation;

		private Vector3 _followVelocity;

		private Vector3 _targetPosition;

		private Camera _playerCamera;

		private Transform _playerHead;

		private CameraMode _currentCameraMode;

		private Vector3 _playerSizeOffset = Vector3.down;

		private float _playerSizeModifier = 1f;
	}
}
