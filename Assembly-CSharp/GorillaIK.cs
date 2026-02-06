using System;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Android;

public class GorillaIK : MonoBehaviour
{
	private void Awake()
	{
		this.bodyInitialRot = this.bodyBone.localRotation;
		this.myRig = base.GetComponent<VRRig>();
		this.ResetIKData();
	}

	private void OnEnable()
	{
		GorillaIKMgr.Instance.RegisterIK(this);
		if (this.skeleton == null)
		{
			return;
		}
		GorillaIK.playerIK = this;
	}

	private void OnDisable()
	{
		GorillaIKMgr.Instance.DeregisterIK(this);
		this.ResetIKData();
	}

	public void ResetIKData()
	{
		this.leftElbowDirection = Vector3.zero;
		this.lerpLeftElbowDirection = Vector3.zero;
		this.rightElbowDirection = Vector3.zero;
		this.lerpRightElbowDirection = Vector3.zero;
		this.targetBodyRot = this.bodyInitialRot;
		this.lerpBodyRot = this.targetBodyRot;
		if (this.projectedBodyRotation != null)
		{
			this.projectedBodyRotation.localRotation = this.targetBodyRot;
		}
		this.usingUpdatedIK = false;
	}

	public bool TickRunning { get; set; }

	public void OverrideTargetPos(bool isLeftHand, Vector3 targetWorldPos)
	{
		if (isLeftHand)
		{
			this.hasLeftOverride = true;
			this.leftOverrideWorldPos = targetWorldPos;
			return;
		}
		this.hasRightOverride = true;
		this.rightOverrideWorldPos = targetWorldPos;
	}

	public Vector3 GetShoulderLocalTargetPos_Left(bool updatedIK)
	{
		if (this.projectedBodyRotation != null && updatedIK)
		{
			return this.projectedLeftShoulderPosition.InverseTransformPoint(this.hasLeftOverride ? this.leftOverrideWorldPos : this.targetLeft.position);
		}
		return this.leftUpperArm.parent.InverseTransformPoint(this.hasLeftOverride ? this.leftOverrideWorldPos : this.targetLeft.position);
	}

	public Vector3 GetShoulderLocalTargetPos_Right(bool updatedIK)
	{
		if (this.projectedBodyRotation != null && updatedIK)
		{
			return this.projectedRightShoulderPosition.InverseTransformPoint(this.hasRightOverride ? this.rightOverrideWorldPos : this.targetRight.position);
		}
		return this.rightUpperArm.parent.InverseTransformPoint(this.hasRightOverride ? this.rightOverrideWorldPos : this.targetRight.position);
	}

	public void ClearOverrides()
	{
		this.hasLeftOverride = false;
		this.hasRightOverride = false;
	}

	public void SkeletonUpdate()
	{
		if (!this.canUseUpdatedIK)
		{
			return;
		}
		if (!SubscriptionManager.IsLocalSubscribed())
		{
			return;
		}
		bool flag = SubscriptionManager.GetSubscriptionSettingValue("SMKEYPREFIXIOBT_ENABLE_KEY") >= 1;
		if (flag != this.skeleton.gameObject.activeSelf)
		{
			this.skeleton.gameObject.SetActive(flag);
			this.usingUpdatedIK = flag;
			if (!flag)
			{
				this.ResetIKData();
			}
			return;
		}
		if (!flag)
		{
			return;
		}
		if (this.skeleton == null || this.skeleton.Bones == null || this.skeleton.Bones.Count == 0)
		{
			return;
		}
		if (this.boneXforms[0] == null || this.body == null || this.leftArmUpper == null || this.leftArmLower == null || this.rightArmUpper == null || this.rightArmLower == null)
		{
			foreach (OVRBone ovrbone in this.skeleton.Bones)
			{
				this.boneXforms[(int)ovrbone.Id] = ovrbone.Transform;
			}
			this.body = this.boneXforms[5];
			this.leftArmUpper = this.boneXforms[10];
			this.leftArmLower = this.boneXforms[11];
			this.rightArmUpper = this.boneXforms[15];
			this.rightArmLower = this.boneXforms[16];
			return;
		}
		this.usingUpdatedIK = true;
		this.targetBodyRot = Quaternion.Inverse(this.bodyBone.parent.rotation) * this.skeleton.transform.rotation * this.body.localRotation * this.bodyOffsetRotation;
		this.projectedBodyRotation.localRotation = this.targetBodyRot;
		this.leftElbowDirection = this.projectedLeftShoulderPosition.InverseTransformDirection((this.leftArmLower.position - this.leftArmLower.up * this.biasDistance - this.targetLeft.position).normalized).normalized;
		this.rightElbowDirection = this.projectedRightShoulderPosition.InverseTransformDirection((this.rightArmLower.position + this.rightArmLower.up * this.biasDistance - this.targetRight.position).normalized).normalized;
	}

	private void CheckPermissions()
	{
		if (!Permission.HasUserAuthorizedPermission("com.oculus.permission.BODY_TRACKING"))
		{
			PermissionCallbacks permissionCallbacks = new PermissionCallbacks();
			permissionCallbacks.PermissionGranted += this.PermissionGranted;
			Permission.RequestUserPermission("com.oculus.permission.BODY_TRACKING", permissionCallbacks);
			return;
		}
		this.PermissionGranted("");
	}

	private void PermissionGranted(string permissionName)
	{
		GorillaIKMgr.AddPlayerIK(this);
		this.boneXforms = new Transform[84];
		this.leftElbowDirection = Vector3.zero;
		this.rightElbowDirection = Vector3.zero;
		this.targetBodyRot = this.bodyInitialRot;
		this.canUseUpdatedIK = true;
	}

	public Transform headBone;

	public Transform bodyBone;

	public Transform leftUpperArm;

	public Transform leftLowerArm;

	public Transform leftHand;

	public Transform rightUpperArm;

	public Transform rightLowerArm;

	public Transform rightHand;

	public Transform targetLeft;

	public Transform targetRight;

	public Transform targetHead;

	public Quaternion initialUpperLeft;

	public Quaternion initialLowerLeft;

	public Quaternion initialUpperRight;

	public Quaternion initialLowerRight;

	[NonSerialized]
	public Quaternion targetBodyRot;

	[NonSerialized]
	public Quaternion lerpBodyRot;

	[NonSerialized]
	public Vector3 leftElbowDirection;

	[NonSerialized]
	public Vector3 lerpLeftElbowDirection;

	[NonSerialized]
	public Vector3 rightElbowDirection;

	[NonSerialized]
	public Vector3 lerpRightElbowDirection;

	public bool usingUpdatedIK;

	public bool canUseUpdatedIK;

	public Quaternion bodyOffsetRotation;

	public OVRSkeleton skeleton;

	private Transform[] boneXforms;

	[NonSerialized]
	public Quaternion bodyInitialRot;

	public Transform projectedBodyRotation;

	public Transform projectedLeftShoulderPosition;

	public Transform projectedRightShoulderPosition;

	[NonSerialized]
	public VRRig myRig;

	public static GorillaIK playerIK;

	public float biasDistance = 0.2f;

	private bool hasLeftOverride;

	private Vector3 leftOverrideWorldPos;

	private bool hasRightOverride;

	private Vector3 rightOverrideWorldPos;

	private Transform body;

	private Transform leftArmUpper;

	private Transform leftArmLower;

	private Transform rightArmUpper;

	private Transform rightArmLower;
}
