using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.XR;

public class BatteryChargerCrank : HoldableObject
{
	public bool IsHeld
	{
		get
		{
			return this.isHeld;
		}
	}

	public bool IsHeldLeftHand
	{
		get
		{
			return this.isHeldLeftHand;
		}
	}

	public float CurrentAngle
	{
		get
		{
			return this.currentAngle;
		}
	}

	internal int CrankIndex
	{
		get
		{
			return this.crankIndex;
		}
	}

	private void Awake()
	{
		if (this.rotatingPart == null)
		{
			this.rotatingPart = base.transform;
		}
		Vector3 vector = this.rotatingPart.parent.InverseTransformPoint(this.rotatingPart.TransformPoint(Vector3.right));
		this.lastAngle = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		this.baseLocalAngle = this.rotatingPart.localRotation;
		this.baseLocalAngleInverse = Quaternion.Inverse(this.baseLocalAngle);
		this.crankRadius = new Vector2(this.crankHandleX, this.crankHandleY).magnitude;
		this.crankAngleOffset = Mathf.Atan2(this.crankHandleY, this.crankHandleX) * 57.29578f;
		if (this.crankHandleMaxZ < this.crankHandleMinZ)
		{
			float num = this.crankHandleMaxZ;
			float num2 = this.crankHandleMinZ;
			this.crankHandleMinZ = num;
			this.crankHandleMaxZ = num2;
		}
	}

	private void Start()
	{
		this.crankIndex = this.charger.RegisterCrank(this);
	}

	private void LateUpdate()
	{
		if (!this.isHeld || this.crankIndex < 0)
		{
			return;
		}
		if (!this.charger.IsCrankHeldLocally(this.crankIndex))
		{
			this.DropItemCleanup();
			return;
		}
		Transform controllerTransform = GTPlayer.Instance.GetControllerTransform(this.isHeldLeftHand);
		Vector3 vector = this.rotatingPart.InverseTransformPoint(controllerTransform.position);
		Vector3 position = (vector.xy().normalized * this.crankRadius).WithZ(Mathf.Clamp(vector.z, this.crankHandleMinZ, this.crankHandleMaxZ));
		Vector3 vector2 = this.rotatingPart.TransformPoint(position);
		if (this.maxHandSnapDistance > 0f && (controllerTransform.position - vector2).IsLongerThan(this.maxHandSnapDistance))
		{
			this.OnRelease(null, this.isHeldLeftHand ? EquipmentInteractor.instance.leftHand : EquipmentInteractor.instance.rightHand);
			return;
		}
		controllerTransform.position = vector2;
		float num = this.ComputeAngleFromWorldPos(controllerTransform.position);
		float num2 = Mathf.DeltaAngle(this.lastAngle, num);
		this.lastAngle = num;
		this.currentAngle = num;
		if (num2 != 0f)
		{
			this.charger.OnCrankInput(this.crankIndex, num2);
			GorillaTagger.Instance.DoVibration(this.isHeldLeftHand ? XRNode.LeftHand : XRNode.RightHand, Mathf.Abs(num2 / 30f) * this.vibrationAmplitude, Time.deltaTime);
		}
		this.UpdateCrankSound(num2);
		this.ApplyVisualAngle(num);
	}

	public void UpdateFromRemoteHand(VRRig rig, bool leftHand)
	{
		VRMap vrmap = leftHand ? rig.leftHand : rig.rightHand;
		Vector3 vector = vrmap.GetExtrapolatedControllerPosition();
		vector -= vrmap.rigTarget.rotation * GTPlayer.Instance.GetHandOffset(leftHand) * rig.scaleFactor;
		float angle = this.ComputeAngleFromWorldPos(vector);
		this.currentAngle = angle;
		this.ApplyVisualAngle(angle);
	}

	public void SetVisualAngle(float angle)
	{
		if (this.rotatingPart != null)
		{
			this.currentAngle = angle;
			this.ApplyVisualAngle(angle);
		}
	}

	private float ComputeAngleFromWorldPos(Vector3 worldPos)
	{
		Vector3 vector = this.baseLocalAngleInverse * Quaternion.Inverse(this.rotatingPart.parent.rotation) * (worldPos - this.rotatingPart.position);
		return Mathf.Atan2(vector.y, vector.x) * 57.29578f;
	}

	private void ApplyVisualAngle(float angle)
	{
		this.rotatingPart.localRotation = this.baseLocalAngle * Quaternion.AngleAxis(angle - this.crankAngleOffset, Vector3.forward);
	}

	private void UpdateCrankSound(float crankAmount)
	{
		if (this.crankSound == null)
		{
			return;
		}
		float b = Mathf.Abs(crankAmount / 30f) * this.vibrationAmplitude;
		this.smoothCrankSpeed = Mathf.Lerp(this.smoothCrankSpeed, b, 10f * Time.deltaTime);
		if (this.smoothCrankSpeed > 0.01f)
		{
			if (!this.crankSound.isPlaying)
			{
				this.crankSound.Play();
			}
			float t = Mathf.Clamp01(this.smoothCrankSpeed);
			this.crankSound.pitch = Mathf.Lerp(this.crankSoundMinPitch, this.crankSoundMaxPitch, t);
			return;
		}
		if (this.crankSound.isPlaying)
		{
			this.crankSound.Stop();
			this.smoothCrankSpeed = 0f;
		}
	}

	private void StopCrankSound()
	{
		if (this.crankSound != null && this.crankSound.isPlaying)
		{
			this.crankSound.Stop();
		}
		this.smoothCrankSpeed = 0f;
	}

	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (this.crankIndex < 0)
		{
			return;
		}
		this.isHeldLeftHand = (grabbingHand == EquipmentInteractor.instance.leftHand);
		if (!this.charger.OnCrankGrabbed(this.crankIndex, this.isHeldLeftHand))
		{
			return;
		}
		this.isHeld = true;
		EquipmentInteractor.instance.UpdateHandEquipment(this, this.isHeldLeftHand);
		Transform controllerTransform = GTPlayer.Instance.GetControllerTransform(this.isHeldLeftHand);
		Vector3 vector = this.baseLocalAngleInverse * Quaternion.Inverse(this.rotatingPart.parent.rotation) * (controllerTransform.position - this.rotatingPart.position);
		this.lastAngle = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
	}

	public override void DropItemCleanup()
	{
		if (this.isHeld)
		{
			this.isHeld = false;
			this.StopCrankSound();
			this.charger.OnCrankReleased(this.crankIndex, this.currentAngle);
		}
	}

	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		EquipmentInteractor.instance.UpdateHandEquipment(null, this.isHeldLeftHand);
		if (this.isHeld)
		{
			this.isHeld = false;
			this.charger.OnCrankReleased(this.crankIndex, this.currentAngle);
		}
		return true;
	}

	private void OnDrawGizmosSelected()
	{
		Transform transform = (this.rotatingPart != null) ? this.rotatingPart : base.transform;
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.TransformPoint(new Vector3(this.crankHandleX, this.crankHandleY, this.crankHandleMinZ)), transform.TransformPoint(new Vector3(this.crankHandleX, this.crankHandleY, this.crankHandleMaxZ)));
	}

	[SerializeField]
	private BatteryCharger charger;

	[SerializeField]
	private float crankHandleX;

	[SerializeField]
	private float crankHandleY;

	[SerializeField]
	private float crankHandleMinZ;

	[SerializeField]
	private float crankHandleMaxZ;

	[SerializeField]
	private float maxHandSnapDistance;

	[SerializeField]
	private Transform rotatingPart;

	[SerializeField]
	private float vibrationAmplitude = 0.3f;

	[SerializeField]
	private AudioSource crankSound;

	[SerializeField]
	private float crankSoundMinPitch = 0.6f;

	[SerializeField]
	private float crankSoundMaxPitch = 1.4f;

	private float crankAngleOffset;

	private float crankRadius;

	private float lastAngle;

	private float currentAngle;

	private float smoothCrankSpeed;

	private Quaternion baseLocalAngle;

	private Quaternion baseLocalAngleInverse;

	private int crankIndex = -1;

	private bool isHeld;

	private bool isHeldLeftHand;
}
