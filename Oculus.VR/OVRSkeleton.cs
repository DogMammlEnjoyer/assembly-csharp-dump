using System;
using System.Collections.Generic;
using UnityEngine;

public class OVRSkeleton : MonoBehaviour
{
	public bool IsInitialized { get; private set; }

	public bool IsDataValid { get; private set; }

	public bool IsDataHighConfidence { get; private set; }

	public IList<OVRBone> Bones { get; protected set; }

	public IList<OVRBone> BindPoses { get; private set; }

	public IList<OVRBoneCapsule> Capsules { get; private set; }

	public OVRSkeleton.SkeletonType GetSkeletonType()
	{
		return this._skeletonType;
	}

	internal virtual void SetSkeletonType(OVRSkeleton.SkeletonType type)
	{
		bool flag = this.IsInitialized && type != this._skeletonType;
		this._skeletonType = type;
		if (flag)
		{
			this.Initialize();
			OVRMeshRenderer component = base.GetComponent<OVRMeshRenderer>();
			if (component != null)
			{
				component.ForceRebind();
			}
		}
	}

	internal OVRPlugin.BodyJointSet GetRequiredBodyJointSet()
	{
		OVRSkeleton.SkeletonType skeletonType = this._skeletonType;
		OVRPlugin.BodyJointSet result;
		if (skeletonType != OVRSkeleton.SkeletonType.Body)
		{
			if (skeletonType != OVRSkeleton.SkeletonType.FullBody)
			{
				result = OVRPlugin.BodyJointSet.None;
			}
			else
			{
				result = OVRPlugin.BodyJointSet.FullBody;
			}
		}
		else
		{
			result = OVRPlugin.BodyJointSet.UpperBody;
		}
		return result;
	}

	public bool IsValidBone(OVRSkeleton.BoneId bone)
	{
		return OVRPlugin.IsValidBone((OVRPlugin.BoneId)bone, (OVRPlugin.SkeletonType)this._skeletonType);
	}

	public int SkeletonChangedCount { get; private set; }

	protected virtual void Awake()
	{
		if (this._dataProvider == null)
		{
			OVRSkeleton.IOVRSkeletonDataProvider iovrskeletonDataProvider = this.SearchSkeletonDataProvider();
			if (iovrskeletonDataProvider != null)
			{
				this._dataProvider = iovrskeletonDataProvider;
				MonoBehaviour monoBehaviour = this._dataProvider as MonoBehaviour;
				if (monoBehaviour != null)
				{
					Debug.Log("Found IOVRSkeletonDataProvider reference in " + monoBehaviour.name + " due to unassigned field.");
				}
			}
		}
		this._bones = new List<OVRBone>();
		this.Bones = this._bones.AsReadOnly();
		this._bindPoses = new List<OVRBone>();
		this.BindPoses = this._bindPoses.AsReadOnly();
		this._capsules = new List<OVRBoneCapsule>();
		this.Capsules = this._capsules.AsReadOnly();
	}

	internal OVRSkeleton.IOVRSkeletonDataProvider SearchSkeletonDataProvider()
	{
		foreach (OVRSkeleton.IOVRSkeletonDataProvider iovrskeletonDataProvider in base.gameObject.GetComponentsInParent<OVRSkeleton.IOVRSkeletonDataProvider>(true))
		{
			if (iovrskeletonDataProvider.GetSkeletonType() == this._skeletonType)
			{
				return iovrskeletonDataProvider;
			}
		}
		return null;
	}

	protected virtual void Start()
	{
		if (this._dataProvider == null && this._skeletonType == OVRSkeleton.SkeletonType.Body)
		{
			Debug.LogWarning("OVRSkeleton and its subclasses requires OVRBody to function.");
		}
		if (this.ShouldInitialize())
		{
			this.Initialize();
		}
	}

	private bool ShouldInitialize()
	{
		if (this.IsInitialized)
		{
			return false;
		}
		if (this._dataProvider != null && !this._dataProvider.enabled)
		{
			return false;
		}
		if (this._skeletonType == OVRSkeleton.SkeletonType.None)
		{
			return false;
		}
		OVRSkeleton.IsHandSkeleton(this._skeletonType);
		return true;
	}

	private void Initialize()
	{
		if (OVRPlugin.GetSkeleton2((OVRPlugin.SkeletonType)this._skeletonType, ref this._skeleton))
		{
			this.InitializeBones();
			this.InitializeBindPose();
			this.InitializeCapsules();
			this.IsInitialized = true;
		}
	}

	protected virtual Transform GetBoneTransform(OVRSkeleton.BoneId boneId)
	{
		return null;
	}

	protected virtual void InitializeBones()
	{
		bool flag = this._skeletonType.IsOVRHandSkeleton();
		if (!this._bonesGO)
		{
			this._bonesGO = new GameObject("Bones");
			this._bonesGO.transform.SetParent(base.transform, false);
			this._bonesGO.transform.localPosition = Vector3.zero;
			this._bonesGO.transform.localRotation = Quaternion.identity;
		}
		if (this._bones == null || (long)this._bones.Count != (long)((ulong)this._skeleton.NumBones))
		{
			if (this._bones != null)
			{
				for (int i = 0; i < this._bones.Count; i++)
				{
					this._bones[i].Dispose();
				}
				this._bones.Clear();
			}
			this._bones = new List<OVRBone>(new OVRBone[this._skeleton.NumBones]);
			this.Bones = this._bones.AsReadOnly();
		}
		bool flag2 = false;
		for (int j = 0; j < this._bones.Count; j++)
		{
			OVRBone ovrbone;
			if ((ovrbone = this._bones[j]) == null)
			{
				ovrbone = (this._bones[j] = new OVRBone());
			}
			OVRBone ovrbone2 = ovrbone;
			ovrbone2.Id = (OVRSkeleton.BoneId)this._skeleton.Bones[j].Id;
			ovrbone2.ParentBoneIndex = this._skeleton.Bones[j].ParentBoneIndex;
			if (ovrbone2.Transform == null)
			{
				flag2 = true;
				ovrbone2.Transform = this.GetBoneTransform(ovrbone2.Id);
				if (ovrbone2.Transform == null)
				{
					ovrbone2.Transform = new GameObject(OVRSkeleton.BoneLabelFromBoneId(this._skeletonType, ovrbone2.Id)).transform;
				}
			}
			if (this.GetBoneTransform(ovrbone2.Id) == null)
			{
				ovrbone2.Transform.name = OVRSkeleton.BoneLabelFromBoneId(this._skeletonType, ovrbone2.Id);
			}
		}
		if (flag2)
		{
			for (int k = 0; k < this._bones.Count; k++)
			{
				if (!this.IsValidBone((OVRSkeleton.BoneId)this._bones[k].ParentBoneIndex) || OVRSkeleton.IsBodySkeleton(this._skeletonType))
				{
					this._bones[k].Transform.SetParent(this._bonesGO.transform, false);
				}
				else
				{
					this._bones[k].Transform.SetParent(this._bones[(int)this._bones[k].ParentBoneIndex].Transform, false);
				}
			}
		}
		for (int l = 0; l < this._bones.Count; l++)
		{
			OVRBone ovrbone3 = this._bones[l];
			OVRPlugin.Posef pose = this._skeleton.Bones[l].Pose;
			if (this._applyBoneTranslations)
			{
				ovrbone3.Transform.localPosition = (flag ? pose.Position.FromFlippedXVector3f() : pose.Position.FromFlippedZVector3f());
			}
			ovrbone3.Transform.localRotation = (flag ? pose.Orientation.FromFlippedXQuatf() : pose.Orientation.FromFlippedZQuatf());
		}
	}

	protected virtual void InitializeBindPose()
	{
		if (!this._bindPosesGO)
		{
			this._bindPosesGO = new GameObject("BindPoses");
			this._bindPosesGO.transform.SetParent(base.transform, false);
			this._bindPosesGO.transform.localPosition = Vector3.zero;
			this._bindPosesGO.transform.localRotation = Quaternion.identity;
		}
		if (this._bindPoses != null)
		{
			for (int i = 0; i < this._bindPoses.Count; i++)
			{
				this._bindPoses[i].Dispose();
			}
			this._bindPoses.Clear();
		}
		if (this._bindPosesGO != null)
		{
			List<Transform> list = new List<Transform>();
			for (int j = 0; j < this._bindPosesGO.transform.childCount; j++)
			{
				list.Add(this._bindPosesGO.transform.GetChild(j));
			}
			for (int k = 0; k < list.Count; k++)
			{
				Object.Destroy(list[k].gameObject);
			}
		}
		if (this._bindPoses == null || this._bindPoses.Count != this._bones.Count)
		{
			this._bindPoses = new List<OVRBone>(new OVRBone[this._bones.Count]);
			this.BindPoses = this._bindPoses.AsReadOnly();
		}
		for (int l = 0; l < this._bindPoses.Count; l++)
		{
			OVRBone ovrbone = this._bones[l];
			OVRBone ovrbone2;
			if ((ovrbone2 = this._bindPoses[l]) == null)
			{
				ovrbone2 = (this._bindPoses[l] = new OVRBone());
			}
			OVRBone ovrbone3 = ovrbone2;
			ovrbone3.Id = ovrbone.Id;
			ovrbone3.ParentBoneIndex = ovrbone.ParentBoneIndex;
			Transform transform = ovrbone3.Transform ? ovrbone3.Transform : (ovrbone3.Transform = new GameObject(OVRSkeleton.BoneLabelFromBoneId(this._skeletonType, ovrbone3.Id)).transform);
			transform.localPosition = ovrbone.Transform.localPosition;
			transform.localRotation = ovrbone.Transform.localRotation;
		}
		for (int m = 0; m < this._bindPoses.Count; m++)
		{
			if (!this.IsValidBone((OVRSkeleton.BoneId)this._bindPoses[m].ParentBoneIndex) || OVRSkeleton.IsBodySkeleton(this._skeletonType))
			{
				this._bindPoses[m].Transform.SetParent(this._bindPosesGO.transform, false);
			}
			else
			{
				this._bindPoses[m].Transform.SetParent(this._bindPoses[(int)this._bindPoses[m].ParentBoneIndex].Transform, false);
			}
		}
	}

	private void InitializeCapsules()
	{
		bool flag = this._skeletonType.IsOVRHandSkeleton();
		if (this._enablePhysicsCapsules)
		{
			if (!this._capsulesGO)
			{
				this._capsulesGO = new GameObject("Capsules");
				this._capsulesGO.transform.SetParent(base.transform, false);
				this._capsulesGO.transform.localPosition = Vector3.zero;
				this._capsulesGO.transform.localRotation = Quaternion.identity;
			}
			if (this._capsules != null)
			{
				for (int i = 0; i < this._capsules.Count; i++)
				{
					this._capsules[i].Cleanup();
				}
				this._capsules.Clear();
			}
			if (this._capsules == null || (long)this._capsules.Count != (long)((ulong)this._skeleton.NumBoneCapsules))
			{
				this._capsules = new List<OVRBoneCapsule>(new OVRBoneCapsule[this._skeleton.NumBoneCapsules]);
				this.Capsules = this._capsules.AsReadOnly();
			}
			for (int j = 0; j < this._capsules.Count; j++)
			{
				OVRBone ovrbone = this._bones[(int)this._skeleton.BoneCapsules[j].BoneIndex];
				OVRBoneCapsule ovrboneCapsule;
				if ((ovrboneCapsule = this._capsules[j]) == null)
				{
					ovrboneCapsule = (this._capsules[j] = new OVRBoneCapsule());
				}
				OVRBoneCapsule ovrboneCapsule2 = ovrboneCapsule;
				ovrboneCapsule2.BoneIndex = this._skeleton.BoneCapsules[j].BoneIndex;
				if (ovrboneCapsule2.CapsuleRigidbody == null)
				{
					ovrboneCapsule2.CapsuleRigidbody = new GameObject(OVRSkeleton.BoneLabelFromBoneId(this._skeletonType, ovrbone.Id) + "_CapsuleRigidbody").AddComponent<Rigidbody>();
					ovrboneCapsule2.CapsuleRigidbody.mass = 1f;
					ovrboneCapsule2.CapsuleRigidbody.isKinematic = true;
					ovrboneCapsule2.CapsuleRigidbody.useGravity = false;
					ovrboneCapsule2.CapsuleRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
				}
				GameObject gameObject = ovrboneCapsule2.CapsuleRigidbody.gameObject;
				gameObject.transform.SetParent(this._capsulesGO.transform, false);
				gameObject.transform.position = ovrbone.Transform.position;
				gameObject.transform.rotation = ovrbone.Transform.rotation;
				if (ovrboneCapsule2.CapsuleCollider == null)
				{
					ovrboneCapsule2.CapsuleCollider = new GameObject(OVRSkeleton.BoneLabelFromBoneId(this._skeletonType, ovrbone.Id) + "_CapsuleCollider").AddComponent<CapsuleCollider>();
					ovrboneCapsule2.CapsuleCollider.isTrigger = false;
				}
				Vector3 vector = flag ? this._skeleton.BoneCapsules[j].StartPoint.FromFlippedXVector3f() : this._skeleton.BoneCapsules[j].StartPoint.FromFlippedZVector3f();
				Vector3 toDirection = (flag ? this._skeleton.BoneCapsules[j].EndPoint.FromFlippedXVector3f() : this._skeleton.BoneCapsules[j].EndPoint.FromFlippedZVector3f()) - vector;
				float magnitude = toDirection.magnitude;
				Quaternion localRotation = Quaternion.FromToRotation(Vector3.right, toDirection);
				ovrboneCapsule2.CapsuleCollider.radius = this._skeleton.BoneCapsules[j].Radius;
				ovrboneCapsule2.CapsuleCollider.height = magnitude + this._skeleton.BoneCapsules[j].Radius * 2f;
				ovrboneCapsule2.CapsuleCollider.direction = 0;
				ovrboneCapsule2.CapsuleCollider.center = Vector3.right * magnitude * 0.5f;
				GameObject gameObject2 = ovrboneCapsule2.CapsuleCollider.gameObject;
				gameObject2.transform.SetParent(gameObject.transform, false);
				gameObject2.transform.localPosition = vector;
				gameObject2.transform.localRotation = localRotation;
			}
		}
	}

	protected virtual void Update()
	{
		this.UpdateSkeleton();
	}

	protected void UpdateSkeleton()
	{
		if (this.ShouldInitialize())
		{
			this.Initialize();
		}
		if (!this.IsInitialized || this._dataProvider == null)
		{
			this.IsDataValid = false;
			this.IsDataHighConfidence = false;
			return;
		}
		OVRSkeleton.SkeletonPoseData skeletonPoseData = this._dataProvider.GetSkeletonPoseData();
		this.IsDataValid = skeletonPoseData.IsDataValid;
		if (!skeletonPoseData.IsDataValid)
		{
			return;
		}
		if (this.SkeletonChangedCount != skeletonPoseData.SkeletonChangedCount)
		{
			this.SkeletonChangedCount = skeletonPoseData.SkeletonChangedCount;
			this.IsInitialized = false;
			this.Initialize();
		}
		this.IsDataHighConfidence = skeletonPoseData.IsDataHighConfidence;
		if (this._updateRootPose)
		{
			base.transform.localPosition = skeletonPoseData.RootPose.Position.FromFlippedZVector3f();
			base.transform.localRotation = skeletonPoseData.RootPose.Orientation.FromFlippedZQuatf();
		}
		if (this._updateRootScale)
		{
			base.transform.localScale = new Vector3(skeletonPoseData.RootScale, skeletonPoseData.RootScale, skeletonPoseData.RootScale);
		}
		for (int i = 0; i < this._bones.Count; i++)
		{
			Transform transform = this._bones[i].Transform;
			if (!(transform == null))
			{
				if (OVRSkeleton.IsBodySkeleton(this._skeletonType))
				{
					transform.localPosition = skeletonPoseData.BoneTranslations[i].FromFlippedZVector3f();
					transform.localRotation = skeletonPoseData.BoneRotations[i].FromFlippedZQuatf();
				}
				else if (OVRSkeleton.IsHandSkeleton(this._skeletonType))
				{
					if (this._skeletonType.IsOVRHandSkeleton())
					{
						transform.localRotation = skeletonPoseData.BoneRotations[i].FromFlippedXQuatf();
						if (this._bones[i].Id == OVRSkeleton.BoneId.Hand_Start)
						{
							transform.localRotation *= this.wristFixupRotation;
						}
					}
					else if (this._skeletonType.IsOpenXRHandSkeleton())
					{
						Vector3 a = skeletonPoseData.BoneTranslations[i].FromFlippedZVector3f();
						Quaternion rhs = skeletonPoseData.BoneRotations[i].FromFlippedZQuatf();
						int parentBoneIndex = (int)this._bones[i].ParentBoneIndex;
						bool flag = this.IsValidBone((OVRSkeleton.BoneId)parentBoneIndex);
						Vector3 b = (flag ? skeletonPoseData.BoneTranslations[parentBoneIndex] : skeletonPoseData.RootPose.Position).FromFlippedZVector3f();
						Quaternion rotation = (flag ? skeletonPoseData.BoneRotations[parentBoneIndex] : skeletonPoseData.RootPose.Orientation).FromFlippedZQuatf();
						float d = (skeletonPoseData.RootScale > 0f) ? (1f / skeletonPoseData.RootScale) : 1f;
						Quaternion quaternion = Quaternion.Inverse(rotation);
						transform.localPosition = quaternion * (d * (a - b));
						transform.localRotation = quaternion * rhs;
					}
				}
				else
				{
					transform.localRotation = skeletonPoseData.BoneRotations[i].FromFlippedZQuatf();
				}
			}
		}
	}

	protected void FixedUpdate()
	{
		if (!this.IsInitialized || this._dataProvider == null)
		{
			this.IsDataValid = false;
			this.IsDataHighConfidence = false;
			return;
		}
		this.Update();
		if (this._enablePhysicsCapsules)
		{
			OVRSkeleton.SkeletonPoseData skeletonPoseData = this._dataProvider.GetSkeletonPoseData();
			this.IsDataValid = skeletonPoseData.IsDataValid;
			this.IsDataHighConfidence = skeletonPoseData.IsDataHighConfidence;
			for (int i = 0; i < this._capsules.Count; i++)
			{
				OVRBoneCapsule ovrboneCapsule = this._capsules[i];
				GameObject gameObject = ovrboneCapsule.CapsuleRigidbody.gameObject;
				if (skeletonPoseData.IsDataValid && skeletonPoseData.IsDataHighConfidence)
				{
					Transform transform = this._bones[(int)ovrboneCapsule.BoneIndex].Transform;
					if (gameObject.activeSelf)
					{
						ovrboneCapsule.CapsuleRigidbody.MovePosition(transform.position);
						ovrboneCapsule.CapsuleRigidbody.MoveRotation(transform.rotation);
					}
					else
					{
						gameObject.SetActive(true);
						ovrboneCapsule.CapsuleRigidbody.position = transform.position;
						ovrboneCapsule.CapsuleRigidbody.rotation = transform.rotation;
					}
				}
				else if (gameObject.activeSelf)
				{
					gameObject.SetActive(false);
				}
			}
		}
	}

	public OVRSkeleton.BoneId GetCurrentStartBoneId()
	{
		switch (this._skeletonType)
		{
		case OVRSkeleton.SkeletonType.HandLeft:
		case OVRSkeleton.SkeletonType.HandRight:
			return OVRSkeleton.BoneId.Hand_Start;
		case OVRSkeleton.SkeletonType.Body:
			return OVRSkeleton.BoneId.Hand_Start;
		case OVRSkeleton.SkeletonType.FullBody:
			return OVRSkeleton.BoneId.Hand_Start;
		case OVRSkeleton.SkeletonType.XRHandLeft:
		case OVRSkeleton.SkeletonType.XRHandRight:
			return OVRSkeleton.BoneId.Hand_Start;
		}
		return OVRSkeleton.BoneId.Invalid;
	}

	public OVRSkeleton.BoneId GetCurrentEndBoneId()
	{
		switch (this._skeletonType)
		{
		case OVRSkeleton.SkeletonType.HandLeft:
		case OVRSkeleton.SkeletonType.HandRight:
			return OVRSkeleton.BoneId.Hand_End;
		case OVRSkeleton.SkeletonType.Body:
			return OVRSkeleton.BoneId.Body_End;
		case OVRSkeleton.SkeletonType.FullBody:
			return OVRSkeleton.BoneId.FullBody_End;
		case OVRSkeleton.SkeletonType.XRHandLeft:
		case OVRSkeleton.SkeletonType.XRHandRight:
			return OVRSkeleton.BoneId.XRHand_Max;
		}
		return OVRSkeleton.BoneId.Invalid;
	}

	private OVRSkeleton.BoneId GetCurrentMaxSkinnableBoneId()
	{
		switch (this._skeletonType)
		{
		case OVRSkeleton.SkeletonType.HandLeft:
		case OVRSkeleton.SkeletonType.HandRight:
			return OVRSkeleton.BoneId.Hand_MaxSkinnable;
		case OVRSkeleton.SkeletonType.Body:
			return OVRSkeleton.BoneId.Body_End;
		case OVRSkeleton.SkeletonType.FullBody:
			return OVRSkeleton.BoneId.FullBody_End;
		case OVRSkeleton.SkeletonType.XRHandLeft:
		case OVRSkeleton.SkeletonType.XRHandRight:
			return OVRSkeleton.BoneId.XRHand_Max;
		}
		return OVRSkeleton.BoneId.Invalid;
	}

	public int GetCurrentNumBones()
	{
		OVRSkeleton.SkeletonType skeletonType = this._skeletonType;
		if (skeletonType != OVRSkeleton.SkeletonType.None && skeletonType <= OVRSkeleton.SkeletonType.FullBody)
		{
			return this.GetCurrentEndBoneId() - this.GetCurrentStartBoneId();
		}
		return 0;
	}

	public int GetCurrentNumSkinnableBones()
	{
		OVRSkeleton.SkeletonType skeletonType = this._skeletonType;
		if (skeletonType != OVRSkeleton.SkeletonType.None && skeletonType <= OVRSkeleton.SkeletonType.XRHandRight)
		{
			return this.GetCurrentMaxSkinnableBoneId() - this.GetCurrentStartBoneId();
		}
		return 0;
	}

	public static string BoneLabelFromBoneId(OVRSkeleton.SkeletonType skeletonType, OVRSkeleton.BoneId boneId)
	{
		if (skeletonType == OVRSkeleton.SkeletonType.Body)
		{
			switch (boneId)
			{
			case OVRSkeleton.BoneId.Hand_Start:
				return "Body_Root";
			case OVRSkeleton.BoneId.Hand_ForearmStub:
				return "Body_Hips";
			case OVRSkeleton.BoneId.Hand_Thumb0:
				return "Body_SpineLower";
			case OVRSkeleton.BoneId.Hand_Thumb1:
				return "Body_SpineMiddle";
			case OVRSkeleton.BoneId.Hand_Thumb2:
				return "Body_SpineUpper";
			case OVRSkeleton.BoneId.Hand_Thumb3:
				return "Body_Chest";
			case OVRSkeleton.BoneId.Hand_Index1:
				return "Body_Neck";
			case OVRSkeleton.BoneId.Hand_Index2:
				return "Body_Head";
			case OVRSkeleton.BoneId.Hand_Index3:
				return "Body_LeftShoulder";
			case OVRSkeleton.BoneId.Hand_Middle1:
				return "Body_LeftScapula";
			case OVRSkeleton.BoneId.Hand_Middle2:
				return "Body_LeftArmUpper";
			case OVRSkeleton.BoneId.Hand_Middle3:
				return "Body_LeftArmLower";
			case OVRSkeleton.BoneId.Hand_Ring1:
				return "Body_LeftHandWristTwist";
			case OVRSkeleton.BoneId.Hand_Ring2:
				return "Body_RightShoulder";
			case OVRSkeleton.BoneId.Hand_Ring3:
				return "Body_RightScapula";
			case OVRSkeleton.BoneId.Hand_Pinky0:
				return "Body_RightArmUpper";
			case OVRSkeleton.BoneId.Hand_Pinky1:
				return "Body_RightArmLower";
			case OVRSkeleton.BoneId.Hand_Pinky2:
				return "Body_RightHandWristTwist";
			case OVRSkeleton.BoneId.Hand_Pinky3:
				return "Body_LeftHandPalm";
			case OVRSkeleton.BoneId.Hand_MaxSkinnable:
				return "Body_LeftHandWrist";
			case OVRSkeleton.BoneId.Hand_IndexTip:
				return "Body_LeftHandThumbMetacarpal";
			case OVRSkeleton.BoneId.Hand_MiddleTip:
				return "Body_LeftHandThumbProximal";
			case OVRSkeleton.BoneId.Hand_RingTip:
				return "Body_LeftHandThumbDistal";
			case OVRSkeleton.BoneId.Hand_PinkyTip:
				return "Body_LeftHandThumbTip";
			case OVRSkeleton.BoneId.Hand_End:
				return "Body_LeftHandIndexMetacarpal";
			case OVRSkeleton.BoneId.XRHand_LittleTip:
				return "Body_LeftHandIndexProximal";
			case OVRSkeleton.BoneId.XRHand_Max:
				return "Body_LeftHandIndexIntermediate";
			case OVRSkeleton.BoneId.Body_LeftHandIndexDistal:
				return "Body_LeftHandIndexDistal";
			case OVRSkeleton.BoneId.Body_LeftHandIndexTip:
				return "Body_LeftHandIndexTip";
			case OVRSkeleton.BoneId.Body_LeftHandMiddleMetacarpal:
				return "Body_LeftHandMiddleMetacarpal";
			case OVRSkeleton.BoneId.Body_LeftHandMiddleProximal:
				return "Body_LeftHandMiddleProximal";
			case OVRSkeleton.BoneId.Body_LeftHandMiddleIntermediate:
				return "Body_LeftHandMiddleIntermediate";
			case OVRSkeleton.BoneId.Body_LeftHandMiddleDistal:
				return "Body_LeftHandMiddleDistal";
			case OVRSkeleton.BoneId.Body_LeftHandMiddleTip:
				return "Body_LeftHandMiddleTip";
			case OVRSkeleton.BoneId.Body_LeftHandRingMetacarpal:
				return "Body_LeftHandRingMetacarpal";
			case OVRSkeleton.BoneId.Body_LeftHandRingProximal:
				return "Body_LeftHandRingProximal";
			case OVRSkeleton.BoneId.Body_LeftHandRingIntermediate:
				return "Body_LeftHandRingIntermediate";
			case OVRSkeleton.BoneId.Body_LeftHandRingDistal:
				return "Body_LeftHandRingDistal";
			case OVRSkeleton.BoneId.Body_LeftHandRingTip:
				return "Body_LeftHandRingTip";
			case OVRSkeleton.BoneId.Body_LeftHandLittleMetacarpal:
				return "Body_LeftHandLittleMetacarpal";
			case OVRSkeleton.BoneId.Body_LeftHandLittleProximal:
				return "Body_LeftHandLittleProximal";
			case OVRSkeleton.BoneId.Body_LeftHandLittleIntermediate:
				return "Body_LeftHandLittleIntermediate";
			case OVRSkeleton.BoneId.Body_LeftHandLittleDistal:
				return "Body_LeftHandLittleDistal";
			case OVRSkeleton.BoneId.Body_LeftHandLittleTip:
				return "Body_LeftHandLittleTip";
			case OVRSkeleton.BoneId.Body_RightHandPalm:
				return "Body_RightHandPalm";
			case OVRSkeleton.BoneId.Body_RightHandWrist:
				return "Body_RightHandWrist";
			case OVRSkeleton.BoneId.Body_RightHandThumbMetacarpal:
				return "Body_RightHandThumbMetacarpal";
			case OVRSkeleton.BoneId.Body_RightHandThumbProximal:
				return "Body_RightHandThumbProximal";
			case OVRSkeleton.BoneId.Body_RightHandThumbDistal:
				return "Body_RightHandThumbDistal";
			case OVRSkeleton.BoneId.Body_RightHandThumbTip:
				return "Body_RightHandThumbTip";
			case OVRSkeleton.BoneId.Body_RightHandIndexMetacarpal:
				return "Body_RightHandIndexMetacarpal";
			case OVRSkeleton.BoneId.Body_RightHandIndexProximal:
				return "Body_RightHandIndexProximal";
			case OVRSkeleton.BoneId.Body_RightHandIndexIntermediate:
				return "Body_RightHandIndexIntermediate";
			case OVRSkeleton.BoneId.Body_RightHandIndexDistal:
				return "Body_RightHandIndexDistal";
			case OVRSkeleton.BoneId.Body_RightHandIndexTip:
				return "Body_RightHandIndexTip";
			case OVRSkeleton.BoneId.Body_RightHandMiddleMetacarpal:
				return "Body_RightHandMiddleMetacarpal";
			case OVRSkeleton.BoneId.Body_RightHandMiddleProximal:
				return "Body_RightHandMiddleProximal";
			case OVRSkeleton.BoneId.Body_RightHandMiddleIntermediate:
				return "Body_RightHandMiddleIntermediate";
			case OVRSkeleton.BoneId.Body_RightHandMiddleDistal:
				return "Body_RightHandMiddleDistal";
			case OVRSkeleton.BoneId.Body_RightHandMiddleTip:
				return "Body_RightHandMiddleTip";
			case OVRSkeleton.BoneId.Body_RightHandRingMetacarpal:
				return "Body_RightHandRingMetacarpal";
			case OVRSkeleton.BoneId.Body_RightHandRingProximal:
				return "Body_RightHandRingProximal";
			case OVRSkeleton.BoneId.Body_RightHandRingIntermediate:
				return "Body_RightHandRingIntermediate";
			case OVRSkeleton.BoneId.Body_RightHandRingDistal:
				return "Body_RightHandRingDistal";
			case OVRSkeleton.BoneId.Body_RightHandRingTip:
				return "Body_RightHandRingTip";
			case OVRSkeleton.BoneId.Body_RightHandLittleMetacarpal:
				return "Body_RightHandLittleMetacarpal";
			case OVRSkeleton.BoneId.Body_RightHandLittleProximal:
				return "Body_RightHandLittleProximal";
			case OVRSkeleton.BoneId.Body_RightHandLittleIntermediate:
				return "Body_RightHandLittleIntermediate";
			case OVRSkeleton.BoneId.Body_RightHandLittleDistal:
				return "Body_RightHandLittleDistal";
			case OVRSkeleton.BoneId.Body_RightHandLittleTip:
				return "Body_RightHandLittleTip";
			default:
				return "Body_Unknown";
			}
		}
		else if (skeletonType == OVRSkeleton.SkeletonType.FullBody)
		{
			switch (boneId)
			{
			case OVRSkeleton.BoneId.Hand_Start:
				return "FullBody_Root";
			case OVRSkeleton.BoneId.Hand_ForearmStub:
				return "FullBody_Hips";
			case OVRSkeleton.BoneId.Hand_Thumb0:
				return "FullBody_SpineLower";
			case OVRSkeleton.BoneId.Hand_Thumb1:
				return "FullBody_SpineMiddle";
			case OVRSkeleton.BoneId.Hand_Thumb2:
				return "FullBody_SpineUpper";
			case OVRSkeleton.BoneId.Hand_Thumb3:
				return "FullBody_Chest";
			case OVRSkeleton.BoneId.Hand_Index1:
				return "FullBody_Neck";
			case OVRSkeleton.BoneId.Hand_Index2:
				return "FullBody_Head";
			case OVRSkeleton.BoneId.Hand_Index3:
				return "FullBody_LeftShoulder";
			case OVRSkeleton.BoneId.Hand_Middle1:
				return "FullBody_LeftScapula";
			case OVRSkeleton.BoneId.Hand_Middle2:
				return "FullBody_LeftArmUpper";
			case OVRSkeleton.BoneId.Hand_Middle3:
				return "FullBody_LeftArmLower";
			case OVRSkeleton.BoneId.Hand_Ring1:
				return "FullBody_LeftHandWristTwist";
			case OVRSkeleton.BoneId.Hand_Ring2:
				return "FullBody_RightShoulder";
			case OVRSkeleton.BoneId.Hand_Ring3:
				return "FullBody_RightScapula";
			case OVRSkeleton.BoneId.Hand_Pinky0:
				return "FullBody_RightArmUpper";
			case OVRSkeleton.BoneId.Hand_Pinky1:
				return "FullBody_RightArmLower";
			case OVRSkeleton.BoneId.Hand_Pinky2:
				return "FullBody_RightHandWristTwist";
			case OVRSkeleton.BoneId.Hand_Pinky3:
				return "FullBody_LeftHandPalm";
			case OVRSkeleton.BoneId.Hand_MaxSkinnable:
				return "FullBody_LeftHandWrist";
			case OVRSkeleton.BoneId.Hand_IndexTip:
				return "FullBody_LeftHandThumbMetacarpal";
			case OVRSkeleton.BoneId.Hand_MiddleTip:
				return "FullBody_LeftHandThumbProximal";
			case OVRSkeleton.BoneId.Hand_RingTip:
				return "FullBody_LeftHandThumbDistal";
			case OVRSkeleton.BoneId.Hand_PinkyTip:
				return "FullBody_LeftHandThumbTip";
			case OVRSkeleton.BoneId.Hand_End:
				return "FullBody_LeftHandIndexMetacarpal";
			case OVRSkeleton.BoneId.XRHand_LittleTip:
				return "FullBody_LeftHandIndexProximal";
			case OVRSkeleton.BoneId.XRHand_Max:
				return "FullBody_LeftHandIndexIntermediate";
			case OVRSkeleton.BoneId.Body_LeftHandIndexDistal:
				return "FullBody_LeftHandIndexDistal";
			case OVRSkeleton.BoneId.Body_LeftHandIndexTip:
				return "FullBody_LeftHandIndexTip";
			case OVRSkeleton.BoneId.Body_LeftHandMiddleMetacarpal:
				return "FullBody_LeftHandMiddleMetacarpal";
			case OVRSkeleton.BoneId.Body_LeftHandMiddleProximal:
				return "FullBody_LeftHandMiddleProximal";
			case OVRSkeleton.BoneId.Body_LeftHandMiddleIntermediate:
				return "FullBody_LeftHandMiddleIntermediate";
			case OVRSkeleton.BoneId.Body_LeftHandMiddleDistal:
				return "FullBody_LeftHandMiddleDistal";
			case OVRSkeleton.BoneId.Body_LeftHandMiddleTip:
				return "FullBody_LeftHandMiddleTip";
			case OVRSkeleton.BoneId.Body_LeftHandRingMetacarpal:
				return "FullBody_LeftHandRingMetacarpal";
			case OVRSkeleton.BoneId.Body_LeftHandRingProximal:
				return "FullBody_LeftHandRingProximal";
			case OVRSkeleton.BoneId.Body_LeftHandRingIntermediate:
				return "FullBody_LeftHandRingIntermediate";
			case OVRSkeleton.BoneId.Body_LeftHandRingDistal:
				return "FullBody_LeftHandRingDistal";
			case OVRSkeleton.BoneId.Body_LeftHandRingTip:
				return "FullBody_LeftHandRingTip";
			case OVRSkeleton.BoneId.Body_LeftHandLittleMetacarpal:
				return "FullBody_LeftHandLittleMetacarpal";
			case OVRSkeleton.BoneId.Body_LeftHandLittleProximal:
				return "FullBody_LeftHandLittleProximal";
			case OVRSkeleton.BoneId.Body_LeftHandLittleIntermediate:
				return "FullBody_LeftHandLittleIntermediate";
			case OVRSkeleton.BoneId.Body_LeftHandLittleDistal:
				return "FullBody_LeftHandLittleDistal";
			case OVRSkeleton.BoneId.Body_LeftHandLittleTip:
				return "FullBody_LeftHandLittleTip";
			case OVRSkeleton.BoneId.Body_RightHandPalm:
				return "FullBody_RightHandPalm";
			case OVRSkeleton.BoneId.Body_RightHandWrist:
				return "FullBody_RightHandWrist";
			case OVRSkeleton.BoneId.Body_RightHandThumbMetacarpal:
				return "FullBody_RightHandThumbMetacarpal";
			case OVRSkeleton.BoneId.Body_RightHandThumbProximal:
				return "FullBody_RightHandThumbProximal";
			case OVRSkeleton.BoneId.Body_RightHandThumbDistal:
				return "FullBody_RightHandThumbDistal";
			case OVRSkeleton.BoneId.Body_RightHandThumbTip:
				return "FullBody_RightHandThumbTip";
			case OVRSkeleton.BoneId.Body_RightHandIndexMetacarpal:
				return "FullBody_RightHandIndexMetacarpal";
			case OVRSkeleton.BoneId.Body_RightHandIndexProximal:
				return "FullBody_RightHandIndexProximal";
			case OVRSkeleton.BoneId.Body_RightHandIndexIntermediate:
				return "FullBody_RightHandIndexIntermediate";
			case OVRSkeleton.BoneId.Body_RightHandIndexDistal:
				return "FullBody_RightHandIndexDistal";
			case OVRSkeleton.BoneId.Body_RightHandIndexTip:
				return "FullBody_RightHandIndexTip";
			case OVRSkeleton.BoneId.Body_RightHandMiddleMetacarpal:
				return "FullBody_RightHandMiddleMetacarpal";
			case OVRSkeleton.BoneId.Body_RightHandMiddleProximal:
				return "FullBody_RightHandMiddleProximal";
			case OVRSkeleton.BoneId.Body_RightHandMiddleIntermediate:
				return "FullBody_RightHandMiddleIntermediate";
			case OVRSkeleton.BoneId.Body_RightHandMiddleDistal:
				return "FullBody_RightHandMiddleDistal";
			case OVRSkeleton.BoneId.Body_RightHandMiddleTip:
				return "FullBody_RightHandMiddleTip";
			case OVRSkeleton.BoneId.Body_RightHandRingMetacarpal:
				return "FullBody_RightHandRingMetacarpal";
			case OVRSkeleton.BoneId.Body_RightHandRingProximal:
				return "FullBody_RightHandRingProximal";
			case OVRSkeleton.BoneId.Body_RightHandRingIntermediate:
				return "FullBody_RightHandRingIntermediate";
			case OVRSkeleton.BoneId.Body_RightHandRingDistal:
				return "FullBody_RightHandRingDistal";
			case OVRSkeleton.BoneId.Body_RightHandRingTip:
				return "FullBody_RightHandRingTip";
			case OVRSkeleton.BoneId.Body_RightHandLittleMetacarpal:
				return "FullBody_RightHandLittleMetacarpal";
			case OVRSkeleton.BoneId.Body_RightHandLittleProximal:
				return "FullBody_RightHandLittleProximal";
			case OVRSkeleton.BoneId.Body_RightHandLittleIntermediate:
				return "FullBody_RightHandLittleIntermediate";
			case OVRSkeleton.BoneId.Body_RightHandLittleDistal:
				return "FullBody_RightHandLittleDistal";
			case OVRSkeleton.BoneId.Body_RightHandLittleTip:
				return "FullBody_RightHandLittleTip";
			case OVRSkeleton.BoneId.Body_End:
				return "FullBody_LeftUpperLeg";
			case OVRSkeleton.BoneId.FullBody_LeftLowerLeg:
				return "FullBody_LeftLowerLeg";
			case OVRSkeleton.BoneId.FullBody_LeftFootAnkleTwist:
				return "FullBody_LeftFootAnkleTwist";
			case OVRSkeleton.BoneId.FullBody_LeftFootAnkle:
				return "FullBody_LeftFootAnkle";
			case OVRSkeleton.BoneId.FullBody_LeftFootSubtalar:
				return "FullBody_LeftFootSubtalar";
			case OVRSkeleton.BoneId.FullBody_LeftFootTransverse:
				return "FullBody_LeftFootTransverse";
			case OVRSkeleton.BoneId.FullBody_LeftFootBall:
				return "FullBody_LeftFootBall";
			case OVRSkeleton.BoneId.FullBody_RightUpperLeg:
				return "FullBody_RightUpperLeg";
			case OVRSkeleton.BoneId.FullBody_RightLowerLeg:
				return "FullBody_RightLowerLeg";
			case OVRSkeleton.BoneId.FullBody_RightFootAnkleTwist:
				return "FullBody_RightFootAnkleTwist";
			case OVRSkeleton.BoneId.FullBody_RightFootAnkle:
				return "FullBody_RightFootAnkle";
			case OVRSkeleton.BoneId.FullBody_RightFootSubtalar:
				return "FullBody_RightFootSubtalar";
			case OVRSkeleton.BoneId.FullBody_RightFootTransverse:
				return "FullBody_RightFootTransverse";
			case OVRSkeleton.BoneId.FullBody_RightFootBall:
				return "FullBody_RightFootBall";
			default:
				return "FullBody_Unknown";
			}
		}
		else
		{
			if (!OVRSkeleton.IsHandSkeleton(skeletonType))
			{
				return "Skeleton_Unknown";
			}
			if (skeletonType == OVRSkeleton.SkeletonType.HandLeft || skeletonType == OVRSkeleton.SkeletonType.HandRight)
			{
				switch (boneId)
				{
				case OVRSkeleton.BoneId.Hand_Start:
					return "Hand_WristRoot";
				case OVRSkeleton.BoneId.Hand_ForearmStub:
					return "Hand_ForearmStub";
				case OVRSkeleton.BoneId.Hand_Thumb0:
					return "Hand_Thumb0";
				case OVRSkeleton.BoneId.Hand_Thumb1:
					return "Hand_Thumb1";
				case OVRSkeleton.BoneId.Hand_Thumb2:
					return "Hand_Thumb2";
				case OVRSkeleton.BoneId.Hand_Thumb3:
					return "Hand_Thumb3";
				case OVRSkeleton.BoneId.Hand_Index1:
					return "Hand_Index1";
				case OVRSkeleton.BoneId.Hand_Index2:
					return "Hand_Index2";
				case OVRSkeleton.BoneId.Hand_Index3:
					return "Hand_Index3";
				case OVRSkeleton.BoneId.Hand_Middle1:
					return "Hand_Middle1";
				case OVRSkeleton.BoneId.Hand_Middle2:
					return "Hand_Middle2";
				case OVRSkeleton.BoneId.Hand_Middle3:
					return "Hand_Middle3";
				case OVRSkeleton.BoneId.Hand_Ring1:
					return "Hand_Ring1";
				case OVRSkeleton.BoneId.Hand_Ring2:
					return "Hand_Ring2";
				case OVRSkeleton.BoneId.Hand_Ring3:
					return "Hand_Ring3";
				case OVRSkeleton.BoneId.Hand_Pinky0:
					return "Hand_Pinky0";
				case OVRSkeleton.BoneId.Hand_Pinky1:
					return "Hand_Pinky1";
				case OVRSkeleton.BoneId.Hand_Pinky2:
					return "Hand_Pinky2";
				case OVRSkeleton.BoneId.Hand_Pinky3:
					return "Hand_Pinky3";
				case OVRSkeleton.BoneId.Hand_MaxSkinnable:
					return "Hand_ThumbTip";
				case OVRSkeleton.BoneId.Hand_IndexTip:
					return "Hand_IndexTip";
				case OVRSkeleton.BoneId.Hand_MiddleTip:
					return "Hand_MiddleTip";
				case OVRSkeleton.BoneId.Hand_RingTip:
					return "Hand_RingTip";
				case OVRSkeleton.BoneId.Hand_PinkyTip:
					return "Hand_PinkyTip";
				default:
					return "Hand_Unknown";
				}
			}
			else
			{
				switch (boneId)
				{
				case OVRSkeleton.BoneId.Hand_Start:
					return "XRHand_Palm";
				case OVRSkeleton.BoneId.Hand_ForearmStub:
					return "XRHand_Wrist";
				case OVRSkeleton.BoneId.Hand_Thumb0:
					return "XRHand_ThumbMetacarpal";
				case OVRSkeleton.BoneId.Hand_Thumb1:
					return "XRHand_ThumbProximal";
				case OVRSkeleton.BoneId.Hand_Thumb2:
					return "XRHand_ThumbDistal";
				case OVRSkeleton.BoneId.Hand_Thumb3:
					return "XRHand_ThumbTip";
				case OVRSkeleton.BoneId.Hand_Index1:
					return "XRHand_IndexMetacarpal";
				case OVRSkeleton.BoneId.Hand_Index2:
					return "XRHand_IndexProximal";
				case OVRSkeleton.BoneId.Hand_Index3:
					return "XRHand_IndexIntermediate";
				case OVRSkeleton.BoneId.Hand_Middle1:
					return "XRHand_IndexDistal";
				case OVRSkeleton.BoneId.Hand_Middle2:
					return "XRHand_IndexTip";
				case OVRSkeleton.BoneId.Hand_Middle3:
					return "XRHand_MiddleMetacarpal";
				case OVRSkeleton.BoneId.Hand_Ring1:
					return "XRHand_MiddleProximal";
				case OVRSkeleton.BoneId.Hand_Ring2:
					return "XRHand_MiddleIntermediate";
				case OVRSkeleton.BoneId.Hand_Ring3:
					return "XRHand_MiddleDistal";
				case OVRSkeleton.BoneId.Hand_Pinky0:
					return "XRHand_MiddleTip";
				case OVRSkeleton.BoneId.Hand_Pinky1:
					return "XRHand_RingMetacarpal";
				case OVRSkeleton.BoneId.Hand_Pinky2:
					return "XRHand_RingProximal";
				case OVRSkeleton.BoneId.Hand_Pinky3:
					return "XRHand_RingIntermediate";
				case OVRSkeleton.BoneId.Hand_MaxSkinnable:
					return "XRHand_RingDistal";
				case OVRSkeleton.BoneId.Hand_IndexTip:
					return "XRHand_RingTip";
				case OVRSkeleton.BoneId.Hand_MiddleTip:
					return "XRHand_LittleMetacarpal";
				case OVRSkeleton.BoneId.Hand_RingTip:
					return "XRHand_LittleProximal";
				case OVRSkeleton.BoneId.Hand_PinkyTip:
					return "XRHand_LittleIntermediate";
				case OVRSkeleton.BoneId.Hand_End:
					return "XRHand_LittleDistal";
				case OVRSkeleton.BoneId.XRHand_LittleTip:
					return "XRHand_LittleTip";
				default:
					return "XRHand_Unknown";
				}
			}
		}
	}

	internal static bool IsBodySkeleton(OVRSkeleton.SkeletonType type)
	{
		return type == OVRSkeleton.SkeletonType.Body || type == OVRSkeleton.SkeletonType.FullBody;
	}

	private static bool IsHandSkeleton(OVRSkeleton.SkeletonType type)
	{
		return type.IsHand();
	}

	[SerializeField]
	protected OVRSkeleton.SkeletonType _skeletonType = OVRSkeleton.SkeletonType.None;

	[SerializeField]
	private OVRSkeleton.IOVRSkeletonDataProvider _dataProvider;

	[SerializeField]
	private bool _updateRootPose;

	[SerializeField]
	private bool _updateRootScale;

	[SerializeField]
	private bool _enablePhysicsCapsules;

	[SerializeField]
	private bool _applyBoneTranslations = true;

	private GameObject _bonesGO;

	private GameObject _bindPosesGO;

	private GameObject _capsulesGO;

	protected List<OVRBone> _bones;

	private List<OVRBone> _bindPoses;

	private List<OVRBoneCapsule> _capsules;

	protected OVRPlugin.Skeleton2 _skeleton;

	private readonly Quaternion wristFixupRotation = new Quaternion(0f, 1f, 0f, 0f);

	public interface IOVRSkeletonDataProvider
	{
		OVRSkeleton.SkeletonType GetSkeletonType();

		OVRSkeleton.SkeletonPoseData GetSkeletonPoseData();

		bool enabled { get; }
	}

	public struct SkeletonPoseData
	{
		public OVRPlugin.Posef RootPose { readonly get; set; }

		public float RootScale { readonly get; set; }

		public OVRPlugin.Quatf[] BoneRotations { readonly get; set; }

		public bool IsDataValid { readonly get; set; }

		public bool IsDataHighConfidence { readonly get; set; }

		public OVRPlugin.Vector3f[] BoneTranslations { readonly get; set; }

		public int SkeletonChangedCount { readonly get; set; }
	}

	public enum SkeletonType
	{
		None = -1,
		[InspectorName("OVR Hand (Left)")]
		HandLeft,
		[InspectorName("OVR Hand (Right)")]
		HandRight,
		Body,
		FullBody,
		[InspectorName("OpenXR Hand (Left)")]
		XRHandLeft,
		[InspectorName("OpenXR Hand (Right)")]
		XRHandRight
	}

	public enum BoneId
	{
		Invalid = -1,
		Hand_Start,
		Hand_WristRoot = 0,
		Hand_ForearmStub,
		Hand_Thumb0,
		Hand_Thumb1,
		Hand_Thumb2,
		Hand_Thumb3,
		Hand_Index1,
		Hand_Index2,
		Hand_Index3,
		Hand_Middle1,
		Hand_Middle2,
		Hand_Middle3,
		Hand_Ring1,
		Hand_Ring2,
		Hand_Ring3,
		Hand_Pinky0,
		Hand_Pinky1,
		Hand_Pinky2,
		Hand_Pinky3,
		Hand_MaxSkinnable,
		Hand_ThumbTip = 19,
		Hand_IndexTip,
		Hand_MiddleTip,
		Hand_RingTip,
		Hand_PinkyTip,
		Hand_End,
		XRHand_Start = 0,
		XRHand_Palm = 0,
		XRHand_Wrist,
		XRHand_ThumbMetacarpal,
		XRHand_ThumbProximal,
		XRHand_ThumbDistal,
		XRHand_ThumbTip,
		XRHand_IndexMetacarpal,
		XRHand_IndexProximal,
		XRHand_IndexIntermediate,
		XRHand_IndexDistal,
		XRHand_IndexTip,
		XRHand_MiddleMetacarpal,
		XRHand_MiddleProximal,
		XRHand_MiddleIntermediate,
		XRHand_MiddleDistal,
		XRHand_MiddleTip,
		XRHand_RingMetacarpal,
		XRHand_RingProximal,
		XRHand_RingIntermediate,
		XRHand_RingDistal,
		XRHand_RingTip,
		XRHand_LittleMetacarpal,
		XRHand_LittleProximal,
		XRHand_LittleIntermediate,
		XRHand_LittleDistal,
		XRHand_LittleTip,
		XRHand_Max,
		XRHand_End = 26,
		Body_Start = 0,
		Body_Root = 0,
		Body_Hips,
		Body_SpineLower,
		Body_SpineMiddle,
		Body_SpineUpper,
		Body_Chest,
		Body_Neck,
		Body_Head,
		Body_LeftShoulder,
		Body_LeftScapula,
		Body_LeftArmUpper,
		Body_LeftArmLower,
		Body_LeftHandWristTwist,
		Body_RightShoulder,
		Body_RightScapula,
		Body_RightArmUpper,
		Body_RightArmLower,
		Body_RightHandWristTwist,
		Body_LeftHandPalm,
		Body_LeftHandWrist,
		Body_LeftHandThumbMetacarpal,
		Body_LeftHandThumbProximal,
		Body_LeftHandThumbDistal,
		Body_LeftHandThumbTip,
		Body_LeftHandIndexMetacarpal,
		Body_LeftHandIndexProximal,
		Body_LeftHandIndexIntermediate,
		Body_LeftHandIndexDistal,
		Body_LeftHandIndexTip,
		Body_LeftHandMiddleMetacarpal,
		Body_LeftHandMiddleProximal,
		Body_LeftHandMiddleIntermediate,
		Body_LeftHandMiddleDistal,
		Body_LeftHandMiddleTip,
		Body_LeftHandRingMetacarpal,
		Body_LeftHandRingProximal,
		Body_LeftHandRingIntermediate,
		Body_LeftHandRingDistal,
		Body_LeftHandRingTip,
		Body_LeftHandLittleMetacarpal,
		Body_LeftHandLittleProximal,
		Body_LeftHandLittleIntermediate,
		Body_LeftHandLittleDistal,
		Body_LeftHandLittleTip,
		Body_RightHandPalm,
		Body_RightHandWrist,
		Body_RightHandThumbMetacarpal,
		Body_RightHandThumbProximal,
		Body_RightHandThumbDistal,
		Body_RightHandThumbTip,
		Body_RightHandIndexMetacarpal,
		Body_RightHandIndexProximal,
		Body_RightHandIndexIntermediate,
		Body_RightHandIndexDistal,
		Body_RightHandIndexTip,
		Body_RightHandMiddleMetacarpal,
		Body_RightHandMiddleProximal,
		Body_RightHandMiddleIntermediate,
		Body_RightHandMiddleDistal,
		Body_RightHandMiddleTip,
		Body_RightHandRingMetacarpal,
		Body_RightHandRingProximal,
		Body_RightHandRingIntermediate,
		Body_RightHandRingDistal,
		Body_RightHandRingTip,
		Body_RightHandLittleMetacarpal,
		Body_RightHandLittleProximal,
		Body_RightHandLittleIntermediate,
		Body_RightHandLittleDistal,
		Body_RightHandLittleTip,
		Body_End,
		FullBody_Start = 0,
		FullBody_Root = 0,
		FullBody_Hips,
		FullBody_SpineLower,
		FullBody_SpineMiddle,
		FullBody_SpineUpper,
		FullBody_Chest,
		FullBody_Neck,
		FullBody_Head,
		FullBody_LeftShoulder,
		FullBody_LeftScapula,
		FullBody_LeftArmUpper,
		FullBody_LeftArmLower,
		FullBody_LeftHandWristTwist,
		FullBody_RightShoulder,
		FullBody_RightScapula,
		FullBody_RightArmUpper,
		FullBody_RightArmLower,
		FullBody_RightHandWristTwist,
		FullBody_LeftHandPalm,
		FullBody_LeftHandWrist,
		FullBody_LeftHandThumbMetacarpal,
		FullBody_LeftHandThumbProximal,
		FullBody_LeftHandThumbDistal,
		FullBody_LeftHandThumbTip,
		FullBody_LeftHandIndexMetacarpal,
		FullBody_LeftHandIndexProximal,
		FullBody_LeftHandIndexIntermediate,
		FullBody_LeftHandIndexDistal,
		FullBody_LeftHandIndexTip,
		FullBody_LeftHandMiddleMetacarpal,
		FullBody_LeftHandMiddleProximal,
		FullBody_LeftHandMiddleIntermediate,
		FullBody_LeftHandMiddleDistal,
		FullBody_LeftHandMiddleTip,
		FullBody_LeftHandRingMetacarpal,
		FullBody_LeftHandRingProximal,
		FullBody_LeftHandRingIntermediate,
		FullBody_LeftHandRingDistal,
		FullBody_LeftHandRingTip,
		FullBody_LeftHandLittleMetacarpal,
		FullBody_LeftHandLittleProximal,
		FullBody_LeftHandLittleIntermediate,
		FullBody_LeftHandLittleDistal,
		FullBody_LeftHandLittleTip,
		FullBody_RightHandPalm,
		FullBody_RightHandWrist,
		FullBody_RightHandThumbMetacarpal,
		FullBody_RightHandThumbProximal,
		FullBody_RightHandThumbDistal,
		FullBody_RightHandThumbTip,
		FullBody_RightHandIndexMetacarpal,
		FullBody_RightHandIndexProximal,
		FullBody_RightHandIndexIntermediate,
		FullBody_RightHandIndexDistal,
		FullBody_RightHandIndexTip,
		FullBody_RightHandMiddleMetacarpal,
		FullBody_RightHandMiddleProximal,
		FullBody_RightHandMiddleIntermediate,
		FullBody_RightHandMiddleDistal,
		FullBody_RightHandMiddleTip,
		FullBody_RightHandRingMetacarpal,
		FullBody_RightHandRingProximal,
		FullBody_RightHandRingIntermediate,
		FullBody_RightHandRingDistal,
		FullBody_RightHandRingTip,
		FullBody_RightHandLittleMetacarpal,
		FullBody_RightHandLittleProximal,
		FullBody_RightHandLittleIntermediate,
		FullBody_RightHandLittleDistal,
		FullBody_RightHandLittleTip,
		FullBody_LeftUpperLeg,
		FullBody_LeftLowerLeg,
		FullBody_LeftFootAnkleTwist,
		FullBody_LeftFootAnkle,
		FullBody_LeftFootSubtalar,
		FullBody_LeftFootTransverse,
		FullBody_LeftFootBall,
		FullBody_RightUpperLeg,
		FullBody_RightLowerLeg,
		FullBody_RightFootAnkleTwist,
		FullBody_RightFootAnkle,
		FullBody_RightFootSubtalar,
		FullBody_RightFootTransverse,
		FullBody_RightFootBall,
		FullBody_End,
		Max = 84
	}
}
