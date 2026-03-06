using System;
using Liv.Lck.GorillaTag;
using Liv.Lck.Smoothing;
using UnityEngine;

public class GtThirdPersonCameraBehaviour : MonoBehaviour
{
	private void OnEnable()
	{
		Transform transform;
		if (GtTag.TryGetTransform(GtTagType.HMD, out transform))
		{
			this._positionFilter = new KalmanFilterVector3(transform.position, 1f);
			this._rotationFilter = new KalmanFilterQuaternion(transform.rotation, 1f);
		}
	}

	private void LateUpdate()
	{
		this.UpdateCamera(true);
	}

	private void UpdateCamera(bool useLerp)
	{
		Transform transform;
		if (!GtTag.TryGetTransform(GtTagType.HMD, out transform))
		{
			return;
		}
		Quaternion quaternion = this._rotationFilter.Update(transform.rotation, Time.deltaTime, useLerp ? this.rotationalSmoothness : 0f);
		Vector3 vector = transform.TransformPoint(Vector3.right * this.shoulderOffset);
		Vector3 eulerAngles = quaternion.eulerAngles;
		eulerAngles.x += (this.front ? (-this.heightOffsetAngle) : this.heightOffsetAngle);
		Vector3 point = this.front ? Vector3.forward : Vector3.back;
		Vector3 vector2 = Quaternion.Euler(eulerAngles.x, eulerAngles.y, 0f) * point;
		Vector3 lossyScale = transform.lossyScale;
		float num = (lossyScale.x + lossyScale.y + lossyScale.z) * 0.333333f;
		float num2 = this.distance * num;
		float radius = this.cameraRadius * num;
		float d = useLerp ? GtThirdPersonCameraBehaviour.Lerp(Vector3.Distance(vector, base.transform.position), num2, Time.deltaTime * 0.1f) : num2;
		RaycastHit raycastHit;
		if (Physics.SphereCast(new Ray(vector, vector2), radius, out raycastHit, 10f, this.cameraCollisionMask, QueryTriggerInteraction.Ignore))
		{
			d = Mathf.Min(num2, raycastHit.distance);
		}
		Vector3 vector3 = vector + vector2 * d;
		Quaternion rotation = Quaternion.LookRotation((vector - vector3).normalized, Vector3.up);
		base.transform.SetPositionAndRotation(vector3, rotation);
	}

	public void UpdateCameraWithoutSmoothing()
	{
		this.UpdateCamera(false);
	}

	public static float Lerp(float a, float b, float t)
	{
		return b + (a - b) * Mathf.Exp(-16f * t);
	}

	public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
	{
		return b + (a - b) * Mathf.Exp(-16f * t);
	}

	public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
	{
		return b + (a - b) * Mathf.Exp(-16f * t);
	}

	public static Vector4 Lerp(Vector4 a, Vector4 b, float t)
	{
		return b + (a - b) * Mathf.Exp(-16f * t);
	}

	public bool front = true;

	public float distance = 2f;

	public float heightOffsetAngle;

	public float shoulderOffset = 0.25f;

	public float positionalSmoothness;

	public float rotationalSmoothness;

	public LayerMask cameraCollisionMask;

	public float cameraRadius = 0.1f;

	private KalmanFilterVector3 _positionFilter;

	private KalmanFilterQuaternion _rotationFilter;

	private const float DECAY = 16f;
}
