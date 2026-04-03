using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

public class ArtilleryCrank : HoldableObject
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

	private int CrankIndex
	{
		get
		{
			if (this.crankType != ArtilleryCrankType.Pitch)
			{
				return 1;
			}
			return 0;
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

	private void LateUpdate()
	{
		if (!this.isHeld)
		{
			return;
		}
		if (!this.cannon.IsCrankHeldLocally(this.CrankIndex))
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
			this.cannon.OnCrankInput(this.CrankIndex, num2);
		}
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

	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		this.isHeldLeftHand = (grabbingHand == EquipmentInteractor.instance.leftHand);
		if (!this.cannon.OnCrankGrabbed(this.CrankIndex, this.isHeldLeftHand))
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
			this.cannon.OnCrankReleased(this.CrankIndex, this.currentAngle);
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
			this.cannon.OnCrankReleased(this.CrankIndex, this.currentAngle);
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
	private ArtilleryCannon cannon;

	[SerializeField]
	private ArtilleryCrankType crankType;

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

	private float crankAngleOffset;

	private float crankRadius;

	private float lastAngle;

	private float currentAngle;

	private Quaternion baseLocalAngle;

	private Quaternion baseLocalAngleInverse;

	private bool isHeld;

	private bool isHeldLeftHand;
}
