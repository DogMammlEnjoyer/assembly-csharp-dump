using System;
using System.Collections.Generic;
using Meta.XR.Util;
using UnityEngine;

[Feature(Feature.BodyTracking)]
public class OVRUnityHumanoidSkeletonRetargeter : OVRSkeleton
{
	protected OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata SourceSkeletonData
	{
		get
		{
			return this._sourceSkeletonData;
		}
	}

	protected OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata SourceSkeletonTPoseData
	{
		get
		{
			return this._sourceSkeletonTPoseData;
		}
	}

	protected OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata TargetSkeletonData
	{
		get
		{
			return this._targetSkeletonData;
		}
	}

	protected Animator AnimatorTargetSkeleton
	{
		get
		{
			return this._animatorTargetSkeleton;
		}
	}

	protected Dictionary<OVRSkeleton.BoneId, HumanBodyBones> CustomBoneIdToHumanBodyBone
	{
		get
		{
			return this._customBoneIdToHumanBodyBone;
		}
	}

	protected Dictionary<HumanBodyBones, Quaternion> TargetTPoseRotations
	{
		get
		{
			return this._targetTPoseRotations;
		}
	}

	public OVRUnityHumanoidSkeletonRetargeter()
	{
		this._skeletonType = OVRSkeleton.SkeletonType.Body;
	}

	protected OVRUnityHumanoidSkeletonRetargeter.JointAdjustment[] Adjustments
	{
		get
		{
			return this._adjustments;
		}
	}

	protected OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection[] FullBodySectionsToAlign
	{
		get
		{
			return this._fullBodySectionsToAlign;
		}
	}

	protected OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection[] BodySectionsToAlign
	{
		get
		{
			return this._bodySectionsToAlign;
		}
	}

	protected OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection[] FullBodySectionToPosition
	{
		get
		{
			return this._fullBodySectionToPosition;
		}
	}

	protected OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection[] BodySectionToPosition
	{
		get
		{
			return this._bodySectionToPosition;
		}
	}

	public OVRHumanBodyBonesMappingsInterface BodyBoneMappingsInterface
	{
		get
		{
			return this._bodyBonesMappingInterface;
		}
		set
		{
			this._bodyBonesMappingInterface = value;
		}
	}

	protected override void Start()
	{
		base.Start();
		this._lastTrackedScale = base.transform.lossyScale;
		OVRUnityHumanoidSkeletonRetargeter.ValidateGameObjectForUnityHumanoidRetargeting(base.gameObject);
		this._animatorTargetSkeleton = base.gameObject.GetComponent<Animator>();
		this.CreateCustomBoneIdToHumanBodyBoneMapping();
		this.StoreTTargetPoseRotations();
		this._targetSkeletonData = new OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata(this._animatorTargetSkeleton, this._bodyBonesMappingInterface);
		this._targetSkeletonData.BuildCoordinateAxesForAllBones();
		this.PrecomputeAllRotationTweaks();
	}

	private void PrecomputeAllRotationTweaks()
	{
		if (this._adjustments == null || this._adjustments.Length == 0)
		{
			return;
		}
		OVRUnityHumanoidSkeletonRetargeter.JointAdjustment[] adjustments = this._adjustments;
		for (int i = 0; i < adjustments.Length; i++)
		{
			adjustments[i].PrecomputeRotationTweaks();
		}
	}

	protected virtual void OnValidate()
	{
		this.PrecomputeAllRotationTweaks();
	}

	internal static void ValidateGameObjectForUnityHumanoidRetargeting(GameObject go)
	{
		if (go.GetComponent<Animator>() == null)
		{
			throw new InvalidOperationException("Retargeting to Unity Humanoid requires an Animator component with a humanoid avatar on T-Pose");
		}
	}

	private void StoreTTargetPoseRotations()
	{
		for (HumanBodyBones humanBodyBones = HumanBodyBones.Hips; humanBodyBones < HumanBodyBones.LastBone; humanBodyBones++)
		{
			Transform boneTransform = this._animatorTargetSkeleton.GetBoneTransform(humanBodyBones);
			this._targetTPoseRotations[humanBodyBones] = (boneTransform ? boneTransform.rotation : Quaternion.identity);
		}
		Transform transform = this.CreateDuplicateTransformHierarchy(this._animatorTargetSkeleton.GetBoneTransform(HumanBodyBones.Hips));
		transform.name = base.name + "-tPose";
		transform.SetParent(base.transform, false);
	}

	private Transform CreateDuplicateTransformHierarchy(Transform transformFromOriginalHierarchy)
	{
		Transform transform = new GameObject(transformFromOriginalHierarchy.name + "-tPose").transform;
		transform.localPosition = transformFromOriginalHierarchy.localPosition;
		transform.localRotation = transformFromOriginalHierarchy.localRotation;
		transform.localScale = transformFromOriginalHierarchy.localScale;
		HumanBodyBones humanBodyBones = this.FindHumanBodyBoneFromTransform(transformFromOriginalHierarchy);
		if (humanBodyBones != HumanBodyBones.LastBone)
		{
			this._targetTPoseTransformDup[humanBodyBones] = transform;
		}
		foreach (object obj in transformFromOriginalHierarchy)
		{
			Transform transformFromOriginalHierarchy2 = (Transform)obj;
			this.CreateDuplicateTransformHierarchy(transformFromOriginalHierarchy2).SetParent(transform, false);
		}
		return transform;
	}

	private HumanBodyBones FindHumanBodyBoneFromTransform(Transform candidateTransform)
	{
		for (HumanBodyBones humanBodyBones = HumanBodyBones.Hips; humanBodyBones < HumanBodyBones.LastBone; humanBodyBones++)
		{
			if (this._animatorTargetSkeleton.GetBoneTransform(humanBodyBones) == candidateTransform)
			{
				return humanBodyBones;
			}
		}
		return HumanBodyBones.LastBone;
	}

	private void AlignHierarchies(Transform transformToAlign, Transform referenceTransform)
	{
		transformToAlign.localRotation = referenceTransform.localRotation;
		transformToAlign.localPosition = referenceTransform.localPosition;
		transformToAlign.localScale = referenceTransform.localScale;
		for (int i = 0; i < referenceTransform.childCount; i++)
		{
			this.AlignHierarchies(transformToAlign.GetChild(i), referenceTransform.GetChild(i));
		}
	}

	private void CreateCustomBoneIdToHumanBodyBoneMapping()
	{
		this.CopyBoneIdToHumanBodyBoneMapping();
		this.AdjustCustomBoneIdToHumanBodyBoneMapping();
	}

	private void CopyBoneIdToHumanBodyBoneMapping()
	{
		this._customBoneIdToHumanBodyBone.Clear();
		if (this._skeletonType == OVRSkeleton.SkeletonType.FullBody)
		{
			using (Dictionary<OVRSkeleton.BoneId, HumanBodyBones>.Enumerator enumerator = this._bodyBonesMappingInterface.GetFullBodyBoneIdToHumanBodyBone.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<OVRSkeleton.BoneId, HumanBodyBones> keyValuePair = enumerator.Current;
					this._customBoneIdToHumanBodyBone.Add(keyValuePair.Key, keyValuePair.Value);
				}
				return;
			}
		}
		foreach (KeyValuePair<OVRSkeleton.BoneId, HumanBodyBones> keyValuePair2 in this._bodyBonesMappingInterface.GetBoneIdToHumanBodyBone)
		{
			this._customBoneIdToHumanBodyBone.Add(keyValuePair2.Key, keyValuePair2.Value);
		}
	}

	private void AdjustCustomBoneIdToHumanBodyBoneMapping()
	{
		foreach (OVRUnityHumanoidSkeletonRetargeter.JointAdjustment jointAdjustment in this._adjustments)
		{
			bool flag = this._skeletonType == OVRSkeleton.SkeletonType.FullBody;
			if ((!flag || jointAdjustment.FullBodyBoneIdOverrideValue != OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.FullBodyTrackingBoneId.NoOverride) && jointAdjustment.BoneIdOverrideValue != OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodyTrackingBoneId.NoOverride)
			{
				if ((flag && jointAdjustment.FullBodyBoneIdOverrideValue == OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.FullBodyTrackingBoneId.Remove) || jointAdjustment.BoneIdOverrideValue == OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodyTrackingBoneId.Remove)
				{
					this.RemoveMappingCorrespondingToHumanBodyBone(jointAdjustment.Joint);
				}
				else if (flag)
				{
					this._customBoneIdToHumanBodyBone[(OVRSkeleton.BoneId)jointAdjustment.FullBodyBoneIdOverrideValue] = jointAdjustment.Joint;
				}
				else
				{
					this._customBoneIdToHumanBodyBone[(OVRSkeleton.BoneId)jointAdjustment.BoneIdOverrideValue] = jointAdjustment.Joint;
				}
			}
		}
	}

	private void RemoveMappingCorrespondingToHumanBodyBone(HumanBodyBones boneId)
	{
		foreach (OVRSkeleton.BoneId key in this._customBoneIdToHumanBodyBone.Keys)
		{
			if (this._customBoneIdToHumanBodyBone[key] == boneId)
			{
				this._customBoneIdToHumanBodyBone.Remove(key);
				break;
			}
		}
	}

	protected override void Update()
	{
		if (!this.ShouldRunUpdateThisFrame())
		{
			return;
		}
		base.UpdateSkeleton();
		this.RecomputeSkeletalOffsetsIfNecessary();
		this.AlignTargetWithSource();
	}

	protected bool ShouldRunUpdateThisFrame()
	{
		bool inFixedTimeStep = Time.inFixedTimeStep;
		OVRUnityHumanoidSkeletonRetargeter.UpdateType updateType = this._updateType;
		if (updateType != OVRUnityHumanoidSkeletonRetargeter.UpdateType.FixedUpdateOnly)
		{
			return updateType != OVRUnityHumanoidSkeletonRetargeter.UpdateType.UpdateOnly || !inFixedTimeStep;
		}
		return inFixedTimeStep;
	}

	protected void RecomputeSkeletalOffsetsIfNecessary()
	{
		if (this.OffsetComputationNeededThisFrame())
		{
			this.ComputeOffsetsUsingSkeletonComponent();
		}
	}

	protected bool OffsetComputationNeededThisFrame()
	{
		if (!base.IsInitialized || base.BindPoses == null || base.BindPoses.Count == 0)
		{
			return false;
		}
		bool flag = this._lastSkelChangeCount != base.SkeletonChangedCount;
		bool flag2 = (base.transform.lossyScale - this._lastTrackedScale).sqrMagnitude > Mathf.Epsilon;
		return flag || flag2;
	}

	protected void ComputeOffsetsUsingSkeletonComponent()
	{
		if (!base.IsInitialized || base.BindPoses == null || base.BindPoses.Count == 0)
		{
			return;
		}
		if (this._sourceSkeletonData == null)
		{
			this._sourceSkeletonData = new OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata(this, false, this._customBoneIdToHumanBodyBone, this._skeletonType == OVRSkeleton.SkeletonType.FullBody, this._bodyBonesMappingInterface);
		}
		else if (this._skeletonType == OVRSkeleton.SkeletonType.FullBody)
		{
			this._sourceSkeletonData.BuildBoneDataSkeletonFullBody(this, false, this._customBoneIdToHumanBodyBone, this._bodyBonesMappingInterface);
		}
		else
		{
			this._sourceSkeletonData.BuildBoneDataSkeleton(this, false, this._customBoneIdToHumanBodyBone, this._bodyBonesMappingInterface);
		}
		this._sourceSkeletonData.BuildCoordinateAxesForAllBones();
		if (this._sourceSkeletonTPoseData == null)
		{
			this._sourceSkeletonTPoseData = new OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata(this, true, this._customBoneIdToHumanBodyBone, this._skeletonType == OVRSkeleton.SkeletonType.FullBody, this._bodyBonesMappingInterface);
		}
		else if (this._skeletonType == OVRSkeleton.SkeletonType.FullBody)
		{
			this._sourceSkeletonTPoseData.BuildBoneDataSkeletonFullBody(this, true, this._customBoneIdToHumanBodyBone, this._bodyBonesMappingInterface);
		}
		else
		{
			this._sourceSkeletonTPoseData.BuildBoneDataSkeleton(this, true, this._customBoneIdToHumanBodyBone, this._bodyBonesMappingInterface);
		}
		this._sourceSkeletonTPoseData.BuildCoordinateAxesForAllBones();
		this.AlignHierarchies(this._animatorTargetSkeleton.GetBoneTransform(HumanBodyBones.Hips), this._targetTPoseTransformDup[HumanBodyBones.Hips]);
		this._targetSkeletonData.BuildCoordinateAxesForAllBones();
		for (int i = 0; i < base.BindPoses.Count; i++)
		{
			HumanBodyBones humanBodyBones;
			OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata.BoneData boneData;
			OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata.BoneData boneData2;
			OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata.BoneData boneData3;
			if (this._customBoneIdToHumanBodyBone.TryGetValue(base.BindPoses[i].Id, out humanBodyBones) && this._targetSkeletonData.BodyToBoneData.TryGetValue(humanBodyBones, out boneData) && OVRUnityHumanoidSkeletonRetargeter.IsBodySectionInArray(this._bodyBonesMappingInterface.GetBoneToBodySection[humanBodyBones], (this._skeletonType == OVRSkeleton.SkeletonType.FullBody) ? this._fullBodySectionsToAlign : this._bodySectionsToAlign) && this._sourceSkeletonTPoseData.BodyToBoneData.TryGetValue(humanBodyBones, out boneData2) && this._sourceSkeletonData.BodyToBoneData.TryGetValue(humanBodyBones, out boneData3))
			{
				if (boneData2.DegenerateJoint || boneData3.DegenerateJoint)
				{
					boneData.CorrectionQuaternion = null;
				}
				else
				{
					Vector3 toDirection = boneData2.JointPairOrientation * Vector3.forward;
					Quaternion rhs = Quaternion.FromToRotation(boneData.JointPairOrientation * Vector3.forward, toDirection);
					Quaternion lhs = Quaternion.Inverse(base.BindPoses[i].Transform.rotation);
					boneData.CorrectionQuaternion = new Quaternion?(lhs * rhs * this._animatorTargetSkeleton.GetBoneTransform(humanBodyBones).rotation);
				}
			}
		}
		this._lastSkelChangeCount = base.SkeletonChangedCount;
		this._lastTrackedScale = base.transform.lossyScale;
	}

	protected static bool IsBodySectionInArray(OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection bodySectionToCheck, OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection[] sectionArrayToCheck)
	{
		for (int i = 0; i < sectionArrayToCheck.Length; i++)
		{
			if (sectionArrayToCheck[i] == bodySectionToCheck)
			{
				return true;
			}
		}
		return false;
	}

	private void AlignTargetWithSource()
	{
		if (!base.IsInitialized || base.Bones == null || base.Bones.Count == 0)
		{
			return;
		}
		for (int i = 0; i < base.Bones.Count; i++)
		{
			HumanBodyBones humanBodyBones;
			OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata.BoneData boneData;
			if (this._customBoneIdToHumanBodyBone.TryGetValue(base.Bones[i].Id, out humanBodyBones) && this._targetSkeletonData.BodyToBoneData.TryGetValue(humanBodyBones, out boneData) && boneData.CorrectionQuaternion != null)
			{
				Transform originalJoint = boneData.OriginalJoint;
				Quaternion value = boneData.CorrectionQuaternion.Value;
				OVRUnityHumanoidSkeletonRetargeter.JointAdjustment jointAdjustment = this.FindAdjustment(humanBodyBones);
				bool flag = OVRUnityHumanoidSkeletonRetargeter.IsBodySectionInArray(this._bodyBonesMappingInterface.GetBoneToBodySection[humanBodyBones], (this._skeletonType == OVRSkeleton.SkeletonType.FullBody) ? this._fullBodySectionToPosition : this._bodySectionToPosition);
				if (jointAdjustment == null)
				{
					originalJoint.rotation = base.Bones[i].Transform.rotation * value;
					if (flag)
					{
						originalJoint.position = base.Bones[i].Transform.position;
					}
				}
				else
				{
					if (!jointAdjustment.DisableRotationTransform)
					{
						originalJoint.rotation = base.Bones[i].Transform.rotation * value;
					}
					originalJoint.rotation *= jointAdjustment.RotationChange;
					originalJoint.rotation *= jointAdjustment.PrecomputedRotationTweaks;
					if (!jointAdjustment.DisablePositionTransform && flag)
					{
						originalJoint.position = base.Bones[i].Transform.position;
					}
					originalJoint.position += jointAdjustment.PositionChange;
				}
			}
		}
	}

	protected OVRUnityHumanoidSkeletonRetargeter.JointAdjustment FindAdjustment(HumanBodyBones boneId)
	{
		foreach (OVRUnityHumanoidSkeletonRetargeter.JointAdjustment jointAdjustment in this._adjustments)
		{
			if (jointAdjustment.Joint == boneId)
			{
				return jointAdjustment;
			}
		}
		return null;
	}

	private OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata _sourceSkeletonData;

	private OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata _sourceSkeletonTPoseData;

	private OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata _targetSkeletonData;

	private Animator _animatorTargetSkeleton;

	private Dictionary<OVRSkeleton.BoneId, HumanBodyBones> _customBoneIdToHumanBodyBone = new Dictionary<OVRSkeleton.BoneId, HumanBodyBones>();

	private readonly Dictionary<HumanBodyBones, Quaternion> _targetTPoseRotations = new Dictionary<HumanBodyBones, Quaternion>();

	private Dictionary<HumanBodyBones, Transform> _targetTPoseTransformDup = new Dictionary<HumanBodyBones, Transform>();

	private int _lastSkelChangeCount = -1;

	private Vector3 _lastTrackedScale;

	[SerializeField]
	protected OVRUnityHumanoidSkeletonRetargeter.JointAdjustment[] _adjustments = new OVRUnityHumanoidSkeletonRetargeter.JointAdjustment[]
	{
		new OVRUnityHumanoidSkeletonRetargeter.JointAdjustment
		{
			Joint = HumanBodyBones.Hips,
			RotationChange = Quaternion.Euler(60f, 0f, 0f)
		}
	};

	[SerializeField]
	protected OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection[] _fullBodySectionsToAlign = new OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection[]
	{
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftArm,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightArm,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftHand,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightHand,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.Hips,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.Back,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.Neck,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.Head,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftLeg,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftFoot,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightLeg,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightFoot
	};

	[SerializeField]
	protected OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection[] _bodySectionsToAlign = new OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection[]
	{
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftArm,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightArm,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftHand,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightHand,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.Hips,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.Back,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.Neck,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.Head
	};

	[SerializeField]
	protected OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection[] _fullBodySectionToPosition = new OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection[]
	{
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftArm,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightArm,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftHand,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightHand,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.Hips,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.Neck,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.Head,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftLeg,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftFoot,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightLeg,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightFoot
	};

	[SerializeField]
	protected OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection[] _bodySectionToPosition = new OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection[]
	{
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftArm,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightArm,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftHand,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightHand,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.Hips,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.Neck,
		OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.Head
	};

	[SerializeField]
	[Tooltip("Controls if we run retargeting from FixedUpdate, Update, or both.")]
	protected OVRUnityHumanoidSkeletonRetargeter.UpdateType _updateType = OVRUnityHumanoidSkeletonRetargeter.UpdateType.UpdateOnly;

	private OVRHumanBodyBonesMappingsInterface _bodyBonesMappingInterface = new OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings();

	public class OVRHumanBodyBonesMappings : OVRHumanBodyBonesMappingsInterface
	{
		public Dictionary<HumanBodyBones, Tuple<HumanBodyBones, HumanBodyBones>> GetBoneToJointPair
		{
			get
			{
				return OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BoneToJointPair;
			}
		}

		public Dictionary<HumanBodyBones, OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection> GetBoneToBodySection
		{
			get
			{
				return OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BoneToBodySection;
			}
		}

		public Dictionary<OVRSkeleton.BoneId, HumanBodyBones> GetFullBodyBoneIdToHumanBodyBone
		{
			get
			{
				return OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.FullBodyBoneIdToHumanBodyBone;
			}
		}

		public Dictionary<OVRSkeleton.BoneId, HumanBodyBones> GetBoneIdToHumanBodyBone
		{
			get
			{
				return OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BoneIdToHumanBodyBone;
			}
		}

		public Dictionary<OVRSkeleton.BoneId, Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>> GetFullBodyBoneIdToJointPair
		{
			get
			{
				return OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.FullBoneIdToJointPair;
			}
		}

		public Dictionary<OVRSkeleton.BoneId, Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>> GetBoneIdToJointPair
		{
			get
			{
				return OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BoneIdToJointPair;
			}
		}

		public static readonly Dictionary<HumanBodyBones, Tuple<HumanBodyBones, HumanBodyBones>> BoneToJointPair = new Dictionary<HumanBodyBones, Tuple<HumanBodyBones, HumanBodyBones>>
		{
			{
				HumanBodyBones.Neck,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.Neck, HumanBodyBones.Head)
			},
			{
				HumanBodyBones.Head,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.Neck, HumanBodyBones.Head)
			},
			{
				HumanBodyBones.LeftEye,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.Head, HumanBodyBones.LeftEye)
			},
			{
				HumanBodyBones.RightEye,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.Head, HumanBodyBones.RightEye)
			},
			{
				HumanBodyBones.Jaw,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.Head, HumanBodyBones.Jaw)
			},
			{
				HumanBodyBones.Hips,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.Hips, HumanBodyBones.Spine)
			},
			{
				HumanBodyBones.Spine,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.Spine, HumanBodyBones.Chest)
			},
			{
				HumanBodyBones.Chest,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.Chest, HumanBodyBones.UpperChest)
			},
			{
				HumanBodyBones.UpperChest,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.UpperChest, HumanBodyBones.Neck)
			},
			{
				HumanBodyBones.LeftShoulder,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.LeftShoulder, HumanBodyBones.LeftUpperArm)
			},
			{
				HumanBodyBones.LeftUpperArm,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm)
			},
			{
				HumanBodyBones.LeftLowerArm,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand)
			},
			{
				HumanBodyBones.LeftHand,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.LeftHand, HumanBodyBones.LeftMiddleProximal)
			},
			{
				HumanBodyBones.RightShoulder,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.RightShoulder, HumanBodyBones.RightUpperArm)
			},
			{
				HumanBodyBones.RightUpperArm,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm)
			},
			{
				HumanBodyBones.RightLowerArm,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand)
			},
			{
				HumanBodyBones.RightHand,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.RightHand, HumanBodyBones.RightMiddleProximal)
			},
			{
				HumanBodyBones.RightUpperLeg,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg)
			},
			{
				HumanBodyBones.RightLowerLeg,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot)
			},
			{
				HumanBodyBones.RightFoot,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.RightFoot, HumanBodyBones.RightToes)
			},
			{
				HumanBodyBones.RightToes,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.RightFoot, HumanBodyBones.RightToes)
			},
			{
				HumanBodyBones.LeftUpperLeg,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg)
			},
			{
				HumanBodyBones.LeftLowerLeg,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot)
			},
			{
				HumanBodyBones.LeftFoot,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.LeftFoot, HumanBodyBones.LeftToes)
			},
			{
				HumanBodyBones.LeftToes,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.LeftFoot, HumanBodyBones.LeftToes)
			},
			{
				HumanBodyBones.LeftThumbProximal,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.LeftThumbProximal, HumanBodyBones.LeftThumbIntermediate)
			},
			{
				HumanBodyBones.LeftThumbIntermediate,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.LeftThumbIntermediate, HumanBodyBones.LeftThumbDistal)
			},
			{
				HumanBodyBones.LeftThumbDistal,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.LeftThumbDistal, HumanBodyBones.LastBone)
			},
			{
				HumanBodyBones.LeftIndexProximal,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.LeftIndexProximal, HumanBodyBones.LeftIndexIntermediate)
			},
			{
				HumanBodyBones.LeftIndexIntermediate,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.LeftIndexIntermediate, HumanBodyBones.LeftIndexDistal)
			},
			{
				HumanBodyBones.LeftIndexDistal,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.LeftIndexDistal, HumanBodyBones.LastBone)
			},
			{
				HumanBodyBones.LeftMiddleProximal,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.LeftMiddleProximal, HumanBodyBones.LeftMiddleIntermediate)
			},
			{
				HumanBodyBones.LeftMiddleIntermediate,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.LeftMiddleIntermediate, HumanBodyBones.LeftMiddleDistal)
			},
			{
				HumanBodyBones.LeftMiddleDistal,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.LeftMiddleDistal, HumanBodyBones.LastBone)
			},
			{
				HumanBodyBones.LeftRingProximal,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.LeftRingProximal, HumanBodyBones.LeftRingIntermediate)
			},
			{
				HumanBodyBones.LeftRingIntermediate,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.LeftRingIntermediate, HumanBodyBones.LeftRingDistal)
			},
			{
				HumanBodyBones.LeftRingDistal,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.LeftRingDistal, HumanBodyBones.LastBone)
			},
			{
				HumanBodyBones.LeftLittleProximal,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.LeftLittleProximal, HumanBodyBones.LeftLittleIntermediate)
			},
			{
				HumanBodyBones.LeftLittleIntermediate,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.LeftLittleIntermediate, HumanBodyBones.LeftLittleDistal)
			},
			{
				HumanBodyBones.LeftLittleDistal,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.LeftLittleDistal, HumanBodyBones.LastBone)
			},
			{
				HumanBodyBones.RightThumbProximal,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.RightThumbProximal, HumanBodyBones.RightThumbIntermediate)
			},
			{
				HumanBodyBones.RightThumbIntermediate,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.RightThumbIntermediate, HumanBodyBones.RightThumbDistal)
			},
			{
				HumanBodyBones.RightThumbDistal,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.RightThumbDistal, HumanBodyBones.LastBone)
			},
			{
				HumanBodyBones.RightIndexProximal,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.RightIndexProximal, HumanBodyBones.RightIndexIntermediate)
			},
			{
				HumanBodyBones.RightIndexIntermediate,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.RightIndexIntermediate, HumanBodyBones.RightIndexDistal)
			},
			{
				HumanBodyBones.RightIndexDistal,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.RightIndexDistal, HumanBodyBones.LastBone)
			},
			{
				HumanBodyBones.RightMiddleProximal,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.RightMiddleProximal, HumanBodyBones.RightMiddleIntermediate)
			},
			{
				HumanBodyBones.RightMiddleIntermediate,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.RightMiddleIntermediate, HumanBodyBones.RightMiddleDistal)
			},
			{
				HumanBodyBones.RightMiddleDistal,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.RightMiddleDistal, HumanBodyBones.LastBone)
			},
			{
				HumanBodyBones.RightRingProximal,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.RightRingProximal, HumanBodyBones.RightRingIntermediate)
			},
			{
				HumanBodyBones.RightRingIntermediate,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.RightRingIntermediate, HumanBodyBones.RightRingDistal)
			},
			{
				HumanBodyBones.RightRingDistal,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.RightRingDistal, HumanBodyBones.LastBone)
			},
			{
				HumanBodyBones.RightLittleProximal,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.RightLittleProximal, HumanBodyBones.RightLittleIntermediate)
			},
			{
				HumanBodyBones.RightLittleIntermediate,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.RightLittleIntermediate, HumanBodyBones.RightLittleDistal)
			},
			{
				HumanBodyBones.RightLittleDistal,
				new Tuple<HumanBodyBones, HumanBodyBones>(HumanBodyBones.RightLittleDistal, HumanBodyBones.LastBone)
			}
		};

		public static readonly Dictionary<HumanBodyBones, OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection> BoneToBodySection = new Dictionary<HumanBodyBones, OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection>
		{
			{
				HumanBodyBones.Neck,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.Neck
			},
			{
				HumanBodyBones.Head,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.Head
			},
			{
				HumanBodyBones.LeftEye,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.Head
			},
			{
				HumanBodyBones.RightEye,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.Head
			},
			{
				HumanBodyBones.Jaw,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.Head
			},
			{
				HumanBodyBones.Hips,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.Hips
			},
			{
				HumanBodyBones.Spine,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.Back
			},
			{
				HumanBodyBones.Chest,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.Back
			},
			{
				HumanBodyBones.UpperChest,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.Back
			},
			{
				HumanBodyBones.RightShoulder,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightArm
			},
			{
				HumanBodyBones.RightUpperArm,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightArm
			},
			{
				HumanBodyBones.RightLowerArm,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightArm
			},
			{
				HumanBodyBones.RightHand,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightArm
			},
			{
				HumanBodyBones.LeftShoulder,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftArm
			},
			{
				HumanBodyBones.LeftUpperArm,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftArm
			},
			{
				HumanBodyBones.LeftLowerArm,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftArm
			},
			{
				HumanBodyBones.LeftHand,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftArm
			},
			{
				HumanBodyBones.LeftUpperLeg,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftLeg
			},
			{
				HumanBodyBones.LeftLowerLeg,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftLeg
			},
			{
				HumanBodyBones.LeftFoot,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftFoot
			},
			{
				HumanBodyBones.LeftToes,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftFoot
			},
			{
				HumanBodyBones.RightUpperLeg,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightLeg
			},
			{
				HumanBodyBones.RightLowerLeg,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightLeg
			},
			{
				HumanBodyBones.RightFoot,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightFoot
			},
			{
				HumanBodyBones.RightToes,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightFoot
			},
			{
				HumanBodyBones.LeftThumbProximal,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftHand
			},
			{
				HumanBodyBones.LeftThumbIntermediate,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftHand
			},
			{
				HumanBodyBones.LeftThumbDistal,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftHand
			},
			{
				HumanBodyBones.LeftIndexProximal,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftHand
			},
			{
				HumanBodyBones.LeftIndexIntermediate,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftHand
			},
			{
				HumanBodyBones.LeftIndexDistal,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftHand
			},
			{
				HumanBodyBones.LeftMiddleProximal,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftHand
			},
			{
				HumanBodyBones.LeftMiddleIntermediate,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftHand
			},
			{
				HumanBodyBones.LeftMiddleDistal,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftHand
			},
			{
				HumanBodyBones.LeftRingProximal,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftHand
			},
			{
				HumanBodyBones.LeftRingIntermediate,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftHand
			},
			{
				HumanBodyBones.LeftRingDistal,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftHand
			},
			{
				HumanBodyBones.LeftLittleProximal,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftHand
			},
			{
				HumanBodyBones.LeftLittleIntermediate,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftHand
			},
			{
				HumanBodyBones.LeftLittleDistal,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.LeftHand
			},
			{
				HumanBodyBones.RightThumbProximal,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightHand
			},
			{
				HumanBodyBones.RightThumbIntermediate,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightHand
			},
			{
				HumanBodyBones.RightThumbDistal,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightHand
			},
			{
				HumanBodyBones.RightIndexProximal,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightHand
			},
			{
				HumanBodyBones.RightIndexIntermediate,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightHand
			},
			{
				HumanBodyBones.RightIndexDistal,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightHand
			},
			{
				HumanBodyBones.RightMiddleProximal,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightHand
			},
			{
				HumanBodyBones.RightMiddleIntermediate,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightHand
			},
			{
				HumanBodyBones.RightMiddleDistal,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightHand
			},
			{
				HumanBodyBones.RightRingProximal,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightHand
			},
			{
				HumanBodyBones.RightRingIntermediate,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightHand
			},
			{
				HumanBodyBones.RightRingDistal,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightHand
			},
			{
				HumanBodyBones.RightLittleProximal,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightHand
			},
			{
				HumanBodyBones.RightLittleIntermediate,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightHand
			},
			{
				HumanBodyBones.RightLittleDistal,
				OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodySection.RightHand
			}
		};

		public static readonly Dictionary<OVRSkeleton.BoneId, HumanBodyBones> FullBodyBoneIdToHumanBodyBone = new Dictionary<OVRSkeleton.BoneId, HumanBodyBones>
		{
			{
				OVRSkeleton.BoneId.Hand_ForearmStub,
				HumanBodyBones.Hips
			},
			{
				OVRSkeleton.BoneId.Hand_Thumb0,
				HumanBodyBones.Spine
			},
			{
				OVRSkeleton.BoneId.Hand_Thumb2,
				HumanBodyBones.Chest
			},
			{
				OVRSkeleton.BoneId.Hand_Thumb3,
				HumanBodyBones.UpperChest
			},
			{
				OVRSkeleton.BoneId.Hand_Index1,
				HumanBodyBones.Neck
			},
			{
				OVRSkeleton.BoneId.Hand_Index2,
				HumanBodyBones.Head
			},
			{
				OVRSkeleton.BoneId.Hand_Index3,
				HumanBodyBones.LeftShoulder
			},
			{
				OVRSkeleton.BoneId.Hand_Middle2,
				HumanBodyBones.LeftUpperArm
			},
			{
				OVRSkeleton.BoneId.Hand_Middle3,
				HumanBodyBones.LeftLowerArm
			},
			{
				OVRSkeleton.BoneId.Hand_MaxSkinnable,
				HumanBodyBones.LeftHand
			},
			{
				OVRSkeleton.BoneId.Hand_Ring2,
				HumanBodyBones.RightShoulder
			},
			{
				OVRSkeleton.BoneId.Hand_Pinky0,
				HumanBodyBones.RightUpperArm
			},
			{
				OVRSkeleton.BoneId.Hand_Pinky1,
				HumanBodyBones.RightLowerArm
			},
			{
				OVRSkeleton.BoneId.Body_RightHandWrist,
				HumanBodyBones.RightHand
			},
			{
				OVRSkeleton.BoneId.Hand_IndexTip,
				HumanBodyBones.LeftThumbProximal
			},
			{
				OVRSkeleton.BoneId.Hand_MiddleTip,
				HumanBodyBones.LeftThumbIntermediate
			},
			{
				OVRSkeleton.BoneId.Hand_RingTip,
				HumanBodyBones.LeftThumbDistal
			},
			{
				OVRSkeleton.BoneId.XRHand_LittleTip,
				HumanBodyBones.LeftIndexProximal
			},
			{
				OVRSkeleton.BoneId.XRHand_Max,
				HumanBodyBones.LeftIndexIntermediate
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandIndexDistal,
				HumanBodyBones.LeftIndexDistal
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandMiddleProximal,
				HumanBodyBones.LeftMiddleProximal
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandMiddleIntermediate,
				HumanBodyBones.LeftMiddleIntermediate
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandMiddleDistal,
				HumanBodyBones.LeftMiddleDistal
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandRingProximal,
				HumanBodyBones.LeftRingProximal
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandRingIntermediate,
				HumanBodyBones.LeftRingIntermediate
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandRingDistal,
				HumanBodyBones.LeftRingDistal
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandLittleProximal,
				HumanBodyBones.LeftLittleProximal
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandLittleIntermediate,
				HumanBodyBones.LeftLittleIntermediate
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandLittleDistal,
				HumanBodyBones.LeftLittleDistal
			},
			{
				OVRSkeleton.BoneId.Body_RightHandThumbMetacarpal,
				HumanBodyBones.RightThumbProximal
			},
			{
				OVRSkeleton.BoneId.Body_RightHandThumbProximal,
				HumanBodyBones.RightThumbIntermediate
			},
			{
				OVRSkeleton.BoneId.Body_RightHandThumbDistal,
				HumanBodyBones.RightThumbDistal
			},
			{
				OVRSkeleton.BoneId.Body_RightHandIndexProximal,
				HumanBodyBones.RightIndexProximal
			},
			{
				OVRSkeleton.BoneId.Body_RightHandIndexIntermediate,
				HumanBodyBones.RightIndexIntermediate
			},
			{
				OVRSkeleton.BoneId.Body_RightHandIndexDistal,
				HumanBodyBones.RightIndexDistal
			},
			{
				OVRSkeleton.BoneId.Body_RightHandMiddleProximal,
				HumanBodyBones.RightMiddleProximal
			},
			{
				OVRSkeleton.BoneId.Body_RightHandMiddleIntermediate,
				HumanBodyBones.RightMiddleIntermediate
			},
			{
				OVRSkeleton.BoneId.Body_RightHandMiddleDistal,
				HumanBodyBones.RightMiddleDistal
			},
			{
				OVRSkeleton.BoneId.Body_RightHandRingProximal,
				HumanBodyBones.RightRingProximal
			},
			{
				OVRSkeleton.BoneId.Body_RightHandRingIntermediate,
				HumanBodyBones.RightRingIntermediate
			},
			{
				OVRSkeleton.BoneId.Body_RightHandRingDistal,
				HumanBodyBones.RightRingDistal
			},
			{
				OVRSkeleton.BoneId.Body_RightHandLittleProximal,
				HumanBodyBones.RightLittleProximal
			},
			{
				OVRSkeleton.BoneId.Body_RightHandLittleIntermediate,
				HumanBodyBones.RightLittleIntermediate
			},
			{
				OVRSkeleton.BoneId.Body_RightHandLittleDistal,
				HumanBodyBones.RightLittleDistal
			},
			{
				OVRSkeleton.BoneId.Body_End,
				HumanBodyBones.LeftUpperLeg
			},
			{
				OVRSkeleton.BoneId.FullBody_LeftLowerLeg,
				HumanBodyBones.LeftLowerLeg
			},
			{
				OVRSkeleton.BoneId.FullBody_LeftFootAnkle,
				HumanBodyBones.LeftFoot
			},
			{
				OVRSkeleton.BoneId.FullBody_LeftFootBall,
				HumanBodyBones.LeftToes
			},
			{
				OVRSkeleton.BoneId.FullBody_RightUpperLeg,
				HumanBodyBones.RightUpperLeg
			},
			{
				OVRSkeleton.BoneId.FullBody_RightLowerLeg,
				HumanBodyBones.RightLowerLeg
			},
			{
				OVRSkeleton.BoneId.FullBody_RightFootAnkle,
				HumanBodyBones.RightFoot
			},
			{
				OVRSkeleton.BoneId.FullBody_RightFootBall,
				HumanBodyBones.RightToes
			}
		};

		public static readonly Dictionary<OVRSkeleton.BoneId, HumanBodyBones> BoneIdToHumanBodyBone = new Dictionary<OVRSkeleton.BoneId, HumanBodyBones>
		{
			{
				OVRSkeleton.BoneId.Hand_ForearmStub,
				HumanBodyBones.Hips
			},
			{
				OVRSkeleton.BoneId.Hand_Thumb0,
				HumanBodyBones.Spine
			},
			{
				OVRSkeleton.BoneId.Hand_Thumb2,
				HumanBodyBones.Chest
			},
			{
				OVRSkeleton.BoneId.Hand_Thumb3,
				HumanBodyBones.UpperChest
			},
			{
				OVRSkeleton.BoneId.Hand_Index1,
				HumanBodyBones.Neck
			},
			{
				OVRSkeleton.BoneId.Hand_Index2,
				HumanBodyBones.Head
			},
			{
				OVRSkeleton.BoneId.Hand_Index3,
				HumanBodyBones.LeftShoulder
			},
			{
				OVRSkeleton.BoneId.Hand_Middle2,
				HumanBodyBones.LeftUpperArm
			},
			{
				OVRSkeleton.BoneId.Hand_Middle3,
				HumanBodyBones.LeftLowerArm
			},
			{
				OVRSkeleton.BoneId.Hand_MaxSkinnable,
				HumanBodyBones.LeftHand
			},
			{
				OVRSkeleton.BoneId.Hand_Ring2,
				HumanBodyBones.RightShoulder
			},
			{
				OVRSkeleton.BoneId.Hand_Pinky0,
				HumanBodyBones.RightUpperArm
			},
			{
				OVRSkeleton.BoneId.Hand_Pinky1,
				HumanBodyBones.RightLowerArm
			},
			{
				OVRSkeleton.BoneId.Body_RightHandWrist,
				HumanBodyBones.RightHand
			},
			{
				OVRSkeleton.BoneId.Hand_IndexTip,
				HumanBodyBones.LeftThumbProximal
			},
			{
				OVRSkeleton.BoneId.Hand_MiddleTip,
				HumanBodyBones.LeftThumbIntermediate
			},
			{
				OVRSkeleton.BoneId.Hand_RingTip,
				HumanBodyBones.LeftThumbDistal
			},
			{
				OVRSkeleton.BoneId.XRHand_LittleTip,
				HumanBodyBones.LeftIndexProximal
			},
			{
				OVRSkeleton.BoneId.XRHand_Max,
				HumanBodyBones.LeftIndexIntermediate
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandIndexDistal,
				HumanBodyBones.LeftIndexDistal
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandMiddleProximal,
				HumanBodyBones.LeftMiddleProximal
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandMiddleIntermediate,
				HumanBodyBones.LeftMiddleIntermediate
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandMiddleDistal,
				HumanBodyBones.LeftMiddleDistal
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandRingProximal,
				HumanBodyBones.LeftRingProximal
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandRingIntermediate,
				HumanBodyBones.LeftRingIntermediate
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandRingDistal,
				HumanBodyBones.LeftRingDistal
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandLittleProximal,
				HumanBodyBones.LeftLittleProximal
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandLittleIntermediate,
				HumanBodyBones.LeftLittleIntermediate
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandLittleDistal,
				HumanBodyBones.LeftLittleDistal
			},
			{
				OVRSkeleton.BoneId.Body_RightHandThumbMetacarpal,
				HumanBodyBones.RightThumbProximal
			},
			{
				OVRSkeleton.BoneId.Body_RightHandThumbProximal,
				HumanBodyBones.RightThumbIntermediate
			},
			{
				OVRSkeleton.BoneId.Body_RightHandThumbDistal,
				HumanBodyBones.RightThumbDistal
			},
			{
				OVRSkeleton.BoneId.Body_RightHandIndexProximal,
				HumanBodyBones.RightIndexProximal
			},
			{
				OVRSkeleton.BoneId.Body_RightHandIndexIntermediate,
				HumanBodyBones.RightIndexIntermediate
			},
			{
				OVRSkeleton.BoneId.Body_RightHandIndexDistal,
				HumanBodyBones.RightIndexDistal
			},
			{
				OVRSkeleton.BoneId.Body_RightHandMiddleProximal,
				HumanBodyBones.RightMiddleProximal
			},
			{
				OVRSkeleton.BoneId.Body_RightHandMiddleIntermediate,
				HumanBodyBones.RightMiddleIntermediate
			},
			{
				OVRSkeleton.BoneId.Body_RightHandMiddleDistal,
				HumanBodyBones.RightMiddleDistal
			},
			{
				OVRSkeleton.BoneId.Body_RightHandRingProximal,
				HumanBodyBones.RightRingProximal
			},
			{
				OVRSkeleton.BoneId.Body_RightHandRingIntermediate,
				HumanBodyBones.RightRingIntermediate
			},
			{
				OVRSkeleton.BoneId.Body_RightHandRingDistal,
				HumanBodyBones.RightRingDistal
			},
			{
				OVRSkeleton.BoneId.Body_RightHandLittleProximal,
				HumanBodyBones.RightLittleProximal
			},
			{
				OVRSkeleton.BoneId.Body_RightHandLittleIntermediate,
				HumanBodyBones.RightLittleIntermediate
			},
			{
				OVRSkeleton.BoneId.Body_RightHandLittleDistal,
				HumanBodyBones.RightLittleDistal
			}
		};

		public static readonly Dictionary<OVRSkeleton.BoneId, Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>> FullBoneIdToJointPair = new Dictionary<OVRSkeleton.BoneId, Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>>
		{
			{
				OVRSkeleton.BoneId.Hand_Index1,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Index1, OVRSkeleton.BoneId.Hand_Index2)
			},
			{
				OVRSkeleton.BoneId.Hand_Index2,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Index2, OVRSkeleton.BoneId.Invalid)
			},
			{
				OVRSkeleton.BoneId.Hand_Start,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Start, OVRSkeleton.BoneId.Hand_ForearmStub)
			},
			{
				OVRSkeleton.BoneId.Hand_ForearmStub,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_ForearmStub, OVRSkeleton.BoneId.Hand_Thumb0)
			},
			{
				OVRSkeleton.BoneId.Hand_Thumb0,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Thumb0, OVRSkeleton.BoneId.Hand_Thumb1)
			},
			{
				OVRSkeleton.BoneId.Hand_Thumb1,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Thumb1, OVRSkeleton.BoneId.Hand_Thumb2)
			},
			{
				OVRSkeleton.BoneId.Hand_Thumb2,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Thumb2, OVRSkeleton.BoneId.Hand_Thumb3)
			},
			{
				OVRSkeleton.BoneId.Hand_Thumb3,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Thumb3, OVRSkeleton.BoneId.Hand_Index1)
			},
			{
				OVRSkeleton.BoneId.Hand_Index3,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Index3, OVRSkeleton.BoneId.Hand_Middle2)
			},
			{
				OVRSkeleton.BoneId.Hand_Middle1,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Middle1, OVRSkeleton.BoneId.Hand_Middle2)
			},
			{
				OVRSkeleton.BoneId.Hand_Middle2,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Middle2, OVRSkeleton.BoneId.Hand_Middle3)
			},
			{
				OVRSkeleton.BoneId.Hand_Middle3,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Middle3, OVRSkeleton.BoneId.Hand_MaxSkinnable)
			},
			{
				OVRSkeleton.BoneId.Hand_MaxSkinnable,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_MaxSkinnable, OVRSkeleton.BoneId.Body_LeftHandMiddleMetacarpal)
			},
			{
				OVRSkeleton.BoneId.Hand_Pinky3,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Pinky3, OVRSkeleton.BoneId.Body_LeftHandMiddleMetacarpal)
			},
			{
				OVRSkeleton.BoneId.Hand_Ring1,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Ring1, OVRSkeleton.BoneId.Body_LeftHandMiddleMetacarpal)
			},
			{
				OVRSkeleton.BoneId.Hand_IndexTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_IndexTip, OVRSkeleton.BoneId.Hand_MiddleTip)
			},
			{
				OVRSkeleton.BoneId.Hand_MiddleTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_MiddleTip, OVRSkeleton.BoneId.Hand_RingTip)
			},
			{
				OVRSkeleton.BoneId.Hand_RingTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_RingTip, OVRSkeleton.BoneId.Hand_PinkyTip)
			},
			{
				OVRSkeleton.BoneId.Hand_PinkyTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_RingTip, OVRSkeleton.BoneId.Hand_PinkyTip)
			},
			{
				OVRSkeleton.BoneId.Hand_End,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_End, OVRSkeleton.BoneId.XRHand_LittleTip)
			},
			{
				OVRSkeleton.BoneId.XRHand_LittleTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.XRHand_LittleTip, OVRSkeleton.BoneId.XRHand_Max)
			},
			{
				OVRSkeleton.BoneId.XRHand_Max,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.XRHand_Max, OVRSkeleton.BoneId.Body_LeftHandIndexDistal)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandIndexDistal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandIndexDistal, OVRSkeleton.BoneId.Body_LeftHandIndexTip)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandIndexTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandIndexDistal, OVRSkeleton.BoneId.Body_LeftHandIndexTip)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandMiddleMetacarpal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandMiddleMetacarpal, OVRSkeleton.BoneId.Body_LeftHandMiddleProximal)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandMiddleProximal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandMiddleProximal, OVRSkeleton.BoneId.Body_LeftHandMiddleIntermediate)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandMiddleIntermediate,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandMiddleIntermediate, OVRSkeleton.BoneId.Body_LeftHandMiddleDistal)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandMiddleDistal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandMiddleDistal, OVRSkeleton.BoneId.Body_LeftHandMiddleTip)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandMiddleTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandMiddleDistal, OVRSkeleton.BoneId.Body_LeftHandMiddleTip)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandRingMetacarpal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandRingMetacarpal, OVRSkeleton.BoneId.Body_LeftHandRingProximal)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandRingProximal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandRingProximal, OVRSkeleton.BoneId.Body_LeftHandRingIntermediate)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandRingIntermediate,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandRingIntermediate, OVRSkeleton.BoneId.Body_LeftHandRingDistal)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandRingDistal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandRingDistal, OVRSkeleton.BoneId.Body_LeftHandRingTip)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandRingTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandRingDistal, OVRSkeleton.BoneId.Body_LeftHandRingTip)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandLittleMetacarpal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandLittleMetacarpal, OVRSkeleton.BoneId.Body_LeftHandLittleProximal)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandLittleProximal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandLittleProximal, OVRSkeleton.BoneId.Body_LeftHandLittleIntermediate)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandLittleIntermediate,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandLittleIntermediate, OVRSkeleton.BoneId.Body_LeftHandLittleDistal)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandLittleDistal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandLittleDistal, OVRSkeleton.BoneId.Body_LeftHandLittleTip)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandLittleTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandLittleDistal, OVRSkeleton.BoneId.Body_LeftHandLittleTip)
			},
			{
				OVRSkeleton.BoneId.Hand_Ring2,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Ring2, OVRSkeleton.BoneId.Hand_Pinky0)
			},
			{
				OVRSkeleton.BoneId.Hand_Ring3,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Ring3, OVRSkeleton.BoneId.Hand_Pinky0)
			},
			{
				OVRSkeleton.BoneId.Hand_Pinky0,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Pinky0, OVRSkeleton.BoneId.Hand_Pinky1)
			},
			{
				OVRSkeleton.BoneId.Hand_Pinky1,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Pinky1, OVRSkeleton.BoneId.Body_RightHandWrist)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandWrist,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandWrist, OVRSkeleton.BoneId.Body_RightHandMiddleMetacarpal)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandPalm,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandPalm, OVRSkeleton.BoneId.Body_RightHandMiddleMetacarpal)
			},
			{
				OVRSkeleton.BoneId.Hand_Pinky2,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Pinky2, OVRSkeleton.BoneId.Body_RightHandMiddleMetacarpal)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandThumbMetacarpal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandThumbMetacarpal, OVRSkeleton.BoneId.Body_RightHandThumbProximal)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandThumbProximal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandThumbProximal, OVRSkeleton.BoneId.Body_RightHandThumbDistal)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandThumbDistal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandThumbDistal, OVRSkeleton.BoneId.Body_RightHandThumbTip)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandThumbTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandThumbDistal, OVRSkeleton.BoneId.Body_RightHandThumbTip)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandIndexMetacarpal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandIndexMetacarpal, OVRSkeleton.BoneId.Body_RightHandIndexProximal)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandIndexProximal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandIndexProximal, OVRSkeleton.BoneId.Body_RightHandIndexIntermediate)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandIndexIntermediate,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandIndexIntermediate, OVRSkeleton.BoneId.Body_RightHandIndexDistal)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandIndexDistal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandIndexDistal, OVRSkeleton.BoneId.Body_RightHandIndexTip)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandIndexTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandIndexDistal, OVRSkeleton.BoneId.Body_RightHandIndexTip)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandMiddleMetacarpal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandMiddleMetacarpal, OVRSkeleton.BoneId.Body_RightHandMiddleProximal)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandMiddleProximal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandMiddleProximal, OVRSkeleton.BoneId.Body_RightHandMiddleIntermediate)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandMiddleIntermediate,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandMiddleIntermediate, OVRSkeleton.BoneId.Body_RightHandMiddleDistal)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandMiddleDistal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandMiddleDistal, OVRSkeleton.BoneId.Body_RightHandMiddleTip)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandMiddleTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandMiddleDistal, OVRSkeleton.BoneId.Body_RightHandMiddleTip)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandRingMetacarpal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandRingMetacarpal, OVRSkeleton.BoneId.Body_RightHandRingProximal)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandRingProximal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandRingProximal, OVRSkeleton.BoneId.Body_RightHandRingIntermediate)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandRingIntermediate,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandRingIntermediate, OVRSkeleton.BoneId.Body_RightHandRingDistal)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandRingDistal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandRingDistal, OVRSkeleton.BoneId.Body_RightHandRingTip)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandRingTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandRingDistal, OVRSkeleton.BoneId.Body_RightHandRingTip)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandLittleMetacarpal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandLittleMetacarpal, OVRSkeleton.BoneId.Body_RightHandLittleProximal)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandLittleProximal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandLittleProximal, OVRSkeleton.BoneId.Body_RightHandLittleIntermediate)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandLittleIntermediate,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandLittleIntermediate, OVRSkeleton.BoneId.Body_RightHandLittleDistal)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandLittleDistal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandLittleDistal, OVRSkeleton.BoneId.Body_RightHandLittleTip)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandLittleTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandLittleDistal, OVRSkeleton.BoneId.Body_RightHandLittleTip)
			},
			{
				OVRSkeleton.BoneId.Body_End,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_End, OVRSkeleton.BoneId.FullBody_LeftLowerLeg)
			},
			{
				OVRSkeleton.BoneId.FullBody_LeftLowerLeg,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.FullBody_LeftLowerLeg, OVRSkeleton.BoneId.FullBody_LeftFootAnkle)
			},
			{
				OVRSkeleton.BoneId.FullBody_LeftFootAnkle,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.FullBody_LeftFootAnkle, OVRSkeleton.BoneId.FullBody_LeftFootBall)
			},
			{
				OVRSkeleton.BoneId.FullBody_LeftFootBall,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.FullBody_LeftFootAnkle, OVRSkeleton.BoneId.FullBody_LeftFootBall)
			},
			{
				OVRSkeleton.BoneId.FullBody_RightUpperLeg,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.FullBody_RightUpperLeg, OVRSkeleton.BoneId.FullBody_RightLowerLeg)
			},
			{
				OVRSkeleton.BoneId.FullBody_RightLowerLeg,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.FullBody_RightLowerLeg, OVRSkeleton.BoneId.FullBody_RightFootAnkle)
			},
			{
				OVRSkeleton.BoneId.FullBody_RightFootAnkle,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.FullBody_RightFootAnkle, OVRSkeleton.BoneId.FullBody_RightFootBall)
			},
			{
				OVRSkeleton.BoneId.FullBody_RightFootBall,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.FullBody_RightFootAnkle, OVRSkeleton.BoneId.FullBody_RightFootBall)
			}
		};

		public static readonly Dictionary<OVRSkeleton.BoneId, Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>> BoneIdToJointPair = new Dictionary<OVRSkeleton.BoneId, Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>>
		{
			{
				OVRSkeleton.BoneId.Hand_Index1,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Index1, OVRSkeleton.BoneId.Hand_Index2)
			},
			{
				OVRSkeleton.BoneId.Hand_Index2,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Index2, OVRSkeleton.BoneId.Invalid)
			},
			{
				OVRSkeleton.BoneId.Hand_Start,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Start, OVRSkeleton.BoneId.Hand_ForearmStub)
			},
			{
				OVRSkeleton.BoneId.Hand_ForearmStub,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_ForearmStub, OVRSkeleton.BoneId.Hand_Thumb0)
			},
			{
				OVRSkeleton.BoneId.Hand_Thumb0,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Thumb0, OVRSkeleton.BoneId.Hand_Thumb1)
			},
			{
				OVRSkeleton.BoneId.Hand_Thumb1,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Thumb1, OVRSkeleton.BoneId.Hand_Thumb2)
			},
			{
				OVRSkeleton.BoneId.Hand_Thumb2,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Thumb2, OVRSkeleton.BoneId.Hand_Thumb3)
			},
			{
				OVRSkeleton.BoneId.Hand_Thumb3,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Thumb3, OVRSkeleton.BoneId.Hand_Index1)
			},
			{
				OVRSkeleton.BoneId.Hand_Index3,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Index3, OVRSkeleton.BoneId.Hand_Middle2)
			},
			{
				OVRSkeleton.BoneId.Hand_Middle1,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Middle1, OVRSkeleton.BoneId.Hand_Middle2)
			},
			{
				OVRSkeleton.BoneId.Hand_Middle2,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Middle2, OVRSkeleton.BoneId.Hand_Middle3)
			},
			{
				OVRSkeleton.BoneId.Hand_Middle3,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Middle3, OVRSkeleton.BoneId.Hand_MaxSkinnable)
			},
			{
				OVRSkeleton.BoneId.Hand_MaxSkinnable,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_MaxSkinnable, OVRSkeleton.BoneId.Body_LeftHandMiddleMetacarpal)
			},
			{
				OVRSkeleton.BoneId.Hand_Pinky3,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Pinky3, OVRSkeleton.BoneId.Body_LeftHandMiddleMetacarpal)
			},
			{
				OVRSkeleton.BoneId.Hand_Ring1,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Ring1, OVRSkeleton.BoneId.Body_LeftHandMiddleMetacarpal)
			},
			{
				OVRSkeleton.BoneId.Hand_IndexTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_IndexTip, OVRSkeleton.BoneId.Hand_MiddleTip)
			},
			{
				OVRSkeleton.BoneId.Hand_MiddleTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_MiddleTip, OVRSkeleton.BoneId.Hand_RingTip)
			},
			{
				OVRSkeleton.BoneId.Hand_RingTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_RingTip, OVRSkeleton.BoneId.Hand_PinkyTip)
			},
			{
				OVRSkeleton.BoneId.Hand_PinkyTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_RingTip, OVRSkeleton.BoneId.Hand_PinkyTip)
			},
			{
				OVRSkeleton.BoneId.Hand_End,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_End, OVRSkeleton.BoneId.XRHand_LittleTip)
			},
			{
				OVRSkeleton.BoneId.XRHand_LittleTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.XRHand_LittleTip, OVRSkeleton.BoneId.XRHand_Max)
			},
			{
				OVRSkeleton.BoneId.XRHand_Max,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.XRHand_Max, OVRSkeleton.BoneId.Body_LeftHandIndexDistal)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandIndexDistal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandIndexDistal, OVRSkeleton.BoneId.Body_LeftHandIndexTip)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandIndexTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandIndexDistal, OVRSkeleton.BoneId.Body_LeftHandIndexTip)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandMiddleMetacarpal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandMiddleMetacarpal, OVRSkeleton.BoneId.Body_LeftHandMiddleProximal)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandMiddleProximal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandMiddleProximal, OVRSkeleton.BoneId.Body_LeftHandMiddleIntermediate)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandMiddleIntermediate,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandMiddleIntermediate, OVRSkeleton.BoneId.Body_LeftHandMiddleDistal)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandMiddleDistal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandMiddleDistal, OVRSkeleton.BoneId.Body_LeftHandMiddleTip)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandMiddleTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandMiddleDistal, OVRSkeleton.BoneId.Body_LeftHandMiddleTip)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandRingMetacarpal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandRingMetacarpal, OVRSkeleton.BoneId.Body_LeftHandRingProximal)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandRingProximal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandRingProximal, OVRSkeleton.BoneId.Body_LeftHandRingIntermediate)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandRingIntermediate,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandRingIntermediate, OVRSkeleton.BoneId.Body_LeftHandRingDistal)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandRingDistal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandRingDistal, OVRSkeleton.BoneId.Body_LeftHandRingTip)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandRingTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandRingDistal, OVRSkeleton.BoneId.Body_LeftHandRingTip)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandLittleMetacarpal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandLittleMetacarpal, OVRSkeleton.BoneId.Body_LeftHandLittleProximal)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandLittleProximal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandLittleProximal, OVRSkeleton.BoneId.Body_LeftHandLittleIntermediate)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandLittleIntermediate,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandLittleIntermediate, OVRSkeleton.BoneId.Body_LeftHandLittleDistal)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandLittleDistal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandLittleDistal, OVRSkeleton.BoneId.Body_LeftHandLittleTip)
			},
			{
				OVRSkeleton.BoneId.Body_LeftHandLittleTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_LeftHandLittleDistal, OVRSkeleton.BoneId.Body_LeftHandLittleTip)
			},
			{
				OVRSkeleton.BoneId.Hand_Ring2,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Ring2, OVRSkeleton.BoneId.Hand_Pinky0)
			},
			{
				OVRSkeleton.BoneId.Hand_Ring3,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Ring3, OVRSkeleton.BoneId.Hand_Pinky0)
			},
			{
				OVRSkeleton.BoneId.Hand_Pinky0,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Pinky0, OVRSkeleton.BoneId.Hand_Pinky1)
			},
			{
				OVRSkeleton.BoneId.Hand_Pinky1,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Pinky1, OVRSkeleton.BoneId.Body_RightHandWrist)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandWrist,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandWrist, OVRSkeleton.BoneId.Body_RightHandMiddleMetacarpal)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandPalm,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandPalm, OVRSkeleton.BoneId.Body_RightHandMiddleMetacarpal)
			},
			{
				OVRSkeleton.BoneId.Hand_Pinky2,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Hand_Pinky2, OVRSkeleton.BoneId.Body_RightHandMiddleMetacarpal)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandThumbMetacarpal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandThumbMetacarpal, OVRSkeleton.BoneId.Body_RightHandThumbProximal)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandThumbProximal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandThumbProximal, OVRSkeleton.BoneId.Body_RightHandThumbDistal)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandThumbDistal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandThumbDistal, OVRSkeleton.BoneId.Body_RightHandThumbTip)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandThumbTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandThumbDistal, OVRSkeleton.BoneId.Body_RightHandThumbTip)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandIndexMetacarpal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandIndexMetacarpal, OVRSkeleton.BoneId.Body_RightHandIndexProximal)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandIndexProximal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandIndexProximal, OVRSkeleton.BoneId.Body_RightHandIndexIntermediate)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandIndexIntermediate,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandIndexIntermediate, OVRSkeleton.BoneId.Body_RightHandIndexDistal)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandIndexDistal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandIndexDistal, OVRSkeleton.BoneId.Body_RightHandIndexTip)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandIndexTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandIndexDistal, OVRSkeleton.BoneId.Body_RightHandIndexTip)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandMiddleMetacarpal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandMiddleMetacarpal, OVRSkeleton.BoneId.Body_RightHandMiddleProximal)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandMiddleProximal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandMiddleProximal, OVRSkeleton.BoneId.Body_RightHandMiddleIntermediate)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandMiddleIntermediate,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandMiddleIntermediate, OVRSkeleton.BoneId.Body_RightHandMiddleDistal)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandMiddleDistal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandMiddleDistal, OVRSkeleton.BoneId.Body_RightHandMiddleTip)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandMiddleTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandMiddleDistal, OVRSkeleton.BoneId.Body_RightHandMiddleTip)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandRingMetacarpal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandRingMetacarpal, OVRSkeleton.BoneId.Body_RightHandRingProximal)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandRingProximal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandRingProximal, OVRSkeleton.BoneId.Body_RightHandRingIntermediate)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandRingIntermediate,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandRingIntermediate, OVRSkeleton.BoneId.Body_RightHandRingDistal)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandRingDistal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandRingDistal, OVRSkeleton.BoneId.Body_RightHandRingTip)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandRingTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandRingDistal, OVRSkeleton.BoneId.Body_RightHandRingTip)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandLittleMetacarpal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandLittleMetacarpal, OVRSkeleton.BoneId.Body_RightHandLittleProximal)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandLittleProximal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandLittleProximal, OVRSkeleton.BoneId.Body_RightHandLittleIntermediate)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandLittleIntermediate,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandLittleIntermediate, OVRSkeleton.BoneId.Body_RightHandLittleDistal)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandLittleDistal,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandLittleDistal, OVRSkeleton.BoneId.Body_RightHandLittleTip)
			},
			{
				OVRSkeleton.BoneId.Body_RightHandLittleTip,
				new Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId>(OVRSkeleton.BoneId.Body_RightHandLittleDistal, OVRSkeleton.BoneId.Body_RightHandLittleTip)
			}
		};

		public enum BodySection
		{
			LeftLeg,
			LeftFoot,
			RightLeg,
			RightFoot,
			LeftArm,
			LeftHand,
			RightArm,
			RightHand,
			Hips,
			Back,
			Neck,
			Head
		}

		public enum FullBodyTrackingBoneId
		{
			FullBody_Start,
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
			NoOverride,
			Remove
		}

		public enum BodyTrackingBoneId
		{
			Body_Start,
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
			NoOverride,
			Remove
		}
	}

	protected class OVRSkeletonMetadata
	{
		public Dictionary<HumanBodyBones, OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata.BoneData> BodyToBoneData { get; } = new Dictionary<HumanBodyBones, OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata.BoneData>();

		public OVRSkeletonMetadata(OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata otherSkeletonMetaData)
		{
			this.BodyToBoneData = new Dictionary<HumanBodyBones, OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata.BoneData>();
			foreach (HumanBodyBones key in otherSkeletonMetaData.BodyToBoneData.Keys)
			{
				OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata.BoneData otherBoneData = otherSkeletonMetaData.BodyToBoneData[key];
				this.BodyToBoneData[key] = new OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata.BoneData(otherBoneData);
			}
		}

		public OVRSkeletonMetadata(Animator animator, OVRHumanBodyBonesMappingsInterface bodyBonesMappingInterface = null)
		{
			this.BuildBoneData(animator, bodyBonesMappingInterface);
		}

		public OVRSkeletonMetadata(OVRSkeleton skeleton, bool useBindPose, Dictionary<OVRSkeleton.BoneId, HumanBodyBones> customBoneIdToHumanBodyBone, OVRHumanBodyBonesMappingsInterface bodyBonesMappingInterface)
		{
			this.BuildBoneDataSkeleton(skeleton, useBindPose, customBoneIdToHumanBodyBone, bodyBonesMappingInterface);
		}

		public OVRSkeletonMetadata(OVRSkeleton skeleton, bool useBindPose, Dictionary<OVRSkeleton.BoneId, HumanBodyBones> customBoneIdToHumanBodyBone, bool useFullBody, OVRHumanBodyBonesMappingsInterface bodyBonesMappingInterface)
		{
			if (useFullBody)
			{
				this.BuildBoneDataSkeletonFullBody(skeleton, useBindPose, customBoneIdToHumanBodyBone, bodyBonesMappingInterface);
				return;
			}
			this.BuildBoneDataSkeleton(skeleton, useBindPose, customBoneIdToHumanBodyBone, bodyBonesMappingInterface);
		}

		public void BuildBoneDataSkeleton(OVRSkeleton skeleton, bool useBindPose, Dictionary<OVRSkeleton.BoneId, HumanBodyBones> customBoneIdToHumanBodyBone, OVRHumanBodyBonesMappingsInterface bodyBonesMappingInterface)
		{
			this.AssembleSkeleton(skeleton, useBindPose, customBoneIdToHumanBodyBone, bodyBonesMappingInterface, false);
		}

		public void BuildBoneDataSkeletonFullBody(OVRSkeleton skeleton, bool useBindPose, Dictionary<OVRSkeleton.BoneId, HumanBodyBones> customBoneIdToHumanBodyBone, OVRHumanBodyBonesMappingsInterface bodyBonesMappingInterface)
		{
			this.AssembleSkeleton(skeleton, useBindPose, customBoneIdToHumanBodyBone, bodyBonesMappingInterface, true);
		}

		private void AssembleSkeleton(OVRSkeleton skeleton, bool useBindPose, Dictionary<OVRSkeleton.BoneId, HumanBodyBones> customBoneIdToHumanBodyBone, OVRHumanBodyBonesMappingsInterface bodyBonesMappingInterface, bool useFullBody = false)
		{
			if (this.BodyToBoneData.Count != 0)
			{
				this.BodyToBoneData.Clear();
			}
			IList<OVRBone> list = useBindPose ? skeleton.BindPoses : skeleton.Bones;
			for (int i = 0; i < list.Count; i++)
			{
				OVRBone ovrbone = list[i];
				if (customBoneIdToHumanBodyBone.ContainsKey(ovrbone.Id))
				{
					HumanBodyBones key = customBoneIdToHumanBodyBone[ovrbone.Id];
					OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata.BoneData boneData = new OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata.BoneData();
					boneData.OriginalJoint = ovrbone.Transform;
					if (useFullBody)
					{
						if (!bodyBonesMappingInterface.GetFullBodyBoneIdToJointPair.ContainsKey(ovrbone.Id))
						{
							Debug.LogError(string.Format("Can't find {0} in bone Id to joint pair map!", ovrbone.Id));
							goto IL_1CE;
						}
					}
					else if (!bodyBonesMappingInterface.GetBoneIdToJointPair.ContainsKey(ovrbone.Id))
					{
						Debug.LogError(string.Format("Can't find {0} in bone Id to joint pair map!", ovrbone.Id));
						goto IL_1CE;
					}
					Tuple<OVRSkeleton.BoneId, OVRSkeleton.BoneId> tuple = useFullBody ? bodyBonesMappingInterface.GetFullBodyBoneIdToJointPair[ovrbone.Id] : bodyBonesMappingInterface.GetBoneIdToJointPair[ovrbone.Id];
					OVRSkeleton.BoneId item = tuple.Item1;
					OVRSkeleton.BoneId item2 = tuple.Item2;
					boneData.JointPairStart = ((item == ovrbone.Id) ? ovrbone.Transform : OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata.FindBoneWithBoneId(list, item).Transform);
					boneData.JointPairEnd = ((item2 != OVRSkeleton.BoneId.Invalid) ? OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata.FindBoneWithBoneId(list, item2).Transform : boneData.JointPairStart);
					boneData.ParentTransform = list[(int)ovrbone.ParentBoneIndex].Transform;
					if (boneData.JointPairStart == null)
					{
						Debug.LogWarning(string.Format("{0} has invalid start joint.", ovrbone.Id));
					}
					if (boneData.JointPairEnd == null)
					{
						Debug.LogWarning(string.Format("{0} has invalid end joint.", ovrbone.Id));
					}
					this.BodyToBoneData.Add(key, boneData);
				}
				IL_1CE:;
			}
		}

		private static OVRBone FindBoneWithBoneId(IList<OVRBone> bones, OVRSkeleton.BoneId boneId)
		{
			for (int i = 0; i < bones.Count; i++)
			{
				if (bones[i].Id == boneId)
				{
					return bones[i];
				}
			}
			return null;
		}

		private void BuildBoneData(Animator animator, OVRHumanBodyBonesMappingsInterface bodyBonesMappingInterface)
		{
			if (this.BodyToBoneData.Count != 0)
			{
				this.BodyToBoneData.Clear();
			}
			foreach (HumanBodyBones humanBodyBones in this._boneEnumValues)
			{
				if (humanBodyBones != HumanBodyBones.LastBone)
				{
					if (animator.avatar == null)
					{
						Debug.LogWarning(string.Format("{0} has no avatar.", animator));
					}
					if (animator.avatar != null && !animator.avatar.isHuman)
					{
						Debug.LogWarning(string.Format("{0} does not have have a ", animator) + "valid human description!");
					}
					Transform boneTransform = animator.GetBoneTransform(humanBodyBones);
					if (!(boneTransform == null))
					{
						OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata.BoneData boneData = new OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata.BoneData();
						boneData.OriginalJoint = boneTransform;
						this.BodyToBoneData.Add(humanBodyBones, boneData);
					}
				}
			}
			foreach (HumanBodyBones humanBodyBones2 in this.BodyToBoneData.Keys)
			{
				OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata.BoneData boneData2 = this.BodyToBoneData[humanBodyBones2];
				Tuple<HumanBodyBones, HumanBodyBones> tuple = bodyBonesMappingInterface.GetBoneToJointPair[humanBodyBones2];
				boneData2.JointPairStart = ((tuple.Item1 != HumanBodyBones.LastBone) ? animator.GetBoneTransform(tuple.Item1) : boneData2.OriginalJoint);
				boneData2.JointPairEnd = ((tuple.Item2 != HumanBodyBones.LastBone) ? animator.GetBoneTransform(tuple.Item2) : OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata.FindFirstChild(boneData2.OriginalJoint, boneData2.OriginalJoint));
				boneData2.ParentTransform = boneData2.OriginalJoint.parent;
				if (boneData2.JointPairStart == null)
				{
					Debug.LogWarning(string.Format("{0} has invalid start joint, setting to {1}.", humanBodyBones2, boneData2.OriginalJoint));
					boneData2.JointPairStart = boneData2.OriginalJoint;
				}
				if (boneData2.JointPairEnd == null)
				{
					Debug.LogWarning(string.Format("{0} has invalid end joint.", humanBodyBones2));
				}
			}
		}

		public void BuildCoordinateAxesForAllBones()
		{
			foreach (HumanBodyBones humanBodyBones in this.BodyToBoneData.Keys)
			{
				OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata.BoneData boneData = this.BodyToBoneData[humanBodyBones];
				Vector3 position = boneData.JointPairStart.position;
				Vector3 vector;
				if (boneData.JointPairEnd == null || boneData.JointPairEnd == boneData.JointPairStart || (boneData.JointPairEnd.position - boneData.JointPairStart.position).magnitude < Mathf.Epsilon)
				{
					Transform parentTransform = boneData.ParentTransform;
					Transform jointPairStart = boneData.JointPairStart;
					position = parentTransform.position;
					vector = jointPairStart.position;
					boneData.DegenerateJoint = true;
				}
				else
				{
					vector = boneData.JointPairEnd.position;
					boneData.DegenerateJoint = false;
				}
				if (humanBodyBones == HumanBodyBones.LeftHand || humanBodyBones == HumanBodyBones.RightHand)
				{
					vector = this.FixJointPairEndPositionHand(vector, humanBodyBones);
					HumanBodyBones humanBodyBones2 = (humanBodyBones == HumanBodyBones.LeftHand) ? HumanBodyBones.LeftThumbIntermediate : HumanBodyBones.RightThumbIntermediate;
					if (!this.BodyToBoneData.ContainsKey(humanBodyBones2))
					{
						Debug.LogWarning(string.Format("Character is missing bone corresponding to {0},", humanBodyBones2) + " used for creating right vector. Using backup approach.");
						boneData.JointPairOrientation = OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata.CreateQuaternionForBoneData(position, vector);
					}
					else
					{
						Vector3 rightVector = this.BodyToBoneData[humanBodyBones2].OriginalJoint.position - position;
						boneData.JointPairOrientation = OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata.CreateQuaternionForBoneDataWithRightVec(position, vector, rightVector);
					}
				}
				else
				{
					boneData.JointPairOrientation = OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata.CreateQuaternionForBoneData(position, vector);
				}
				Vector3 position2 = boneData.OriginalJoint.position;
				boneData.FromPosition = position2;
				boneData.ToPosition = position2 + (vector - position);
			}
		}

		private Vector3 FixJointPairEndPositionHand(Vector3 jointPairEndPosition, HumanBodyBones humanBodyBone)
		{
			Vector3 result = jointPairEndPosition;
			if (humanBodyBone == HumanBodyBones.LeftHand && this.BodyToBoneData.ContainsKey(HumanBodyBones.LeftThumbProximal) && this.BodyToBoneData.ContainsKey(HumanBodyBones.LeftIndexProximal) && this.BodyToBoneData.ContainsKey(HumanBodyBones.LeftMiddleProximal) && this.BodyToBoneData.ContainsKey(HumanBodyBones.LeftRingProximal) && this.BodyToBoneData.ContainsKey(HumanBodyBones.LeftLittleProximal))
			{
				Vector3 position = this.BodyToBoneData[HumanBodyBones.LeftThumbProximal].OriginalJoint.position;
				Vector3 position2 = this.BodyToBoneData[HumanBodyBones.LeftIndexProximal].OriginalJoint.position;
				Vector3 position3 = this.BodyToBoneData[HumanBodyBones.LeftMiddleProximal].OriginalJoint.position;
				Vector3 position4 = this.BodyToBoneData[HumanBodyBones.LeftRingProximal].OriginalJoint.position;
				Vector3 position5 = this.BodyToBoneData[HumanBodyBones.LeftLittleProximal].OriginalJoint.position;
				result = (position + position2 + position3 + position4 + position5) / 5f;
			}
			if (humanBodyBone == HumanBodyBones.RightHand && this.BodyToBoneData.ContainsKey(HumanBodyBones.RightThumbProximal) && this.BodyToBoneData.ContainsKey(HumanBodyBones.RightIndexProximal) && this.BodyToBoneData.ContainsKey(HumanBodyBones.RightMiddleProximal) && this.BodyToBoneData.ContainsKey(HumanBodyBones.RightRingProximal) && this.BodyToBoneData.ContainsKey(HumanBodyBones.RightLittleProximal))
			{
				Vector3 position6 = this.BodyToBoneData[HumanBodyBones.RightThumbProximal].OriginalJoint.position;
				Vector3 position7 = this.BodyToBoneData[HumanBodyBones.RightIndexProximal].OriginalJoint.position;
				Vector3 position8 = this.BodyToBoneData[HumanBodyBones.RightMiddleProximal].OriginalJoint.position;
				Vector3 position9 = this.BodyToBoneData[HumanBodyBones.RightRingProximal].OriginalJoint.position;
				Vector3 position10 = this.BodyToBoneData[HumanBodyBones.RightLittleProximal].OriginalJoint.position;
				result = (position6 + position7 + position8 + position9 + position10) / 5f;
			}
			return result;
		}

		private static Transform FindFirstChild(Transform startTransform, Transform currTransform)
		{
			if (startTransform != currTransform)
			{
				return currTransform;
			}
			if (currTransform.childCount == 0)
			{
				return null;
			}
			Transform result = null;
			for (int i = 0; i < currTransform.childCount; i++)
			{
				Transform transform = OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata.FindFirstChild(startTransform, currTransform.GetChild(i));
				if (transform != null)
				{
					result = transform;
					break;
				}
			}
			return result;
		}

		private static Quaternion CreateQuaternionForBoneDataWithRightVec(Vector3 fromPosition, Vector3 toPosition, Vector3 rightVector)
		{
			Vector3 vector = (toPosition - fromPosition).normalized;
			if (vector.sqrMagnitude < Mathf.Epsilon)
			{
				vector = Vector3.forward;
			}
			Vector3 upwards = Vector3.Cross(vector, rightVector);
			return Quaternion.LookRotation(vector, upwards);
		}

		private static Quaternion CreateQuaternionForBoneData(Vector3 fromPosition, Vector3 toPosition)
		{
			Vector3 forward = (toPosition - fromPosition).normalized;
			if (forward.sqrMagnitude < Mathf.Epsilon)
			{
				forward = Vector3.forward;
			}
			return Quaternion.LookRotation(forward);
		}

		private readonly HumanBodyBones[] _boneEnumValues = (HumanBodyBones[])Enum.GetValues(typeof(HumanBodyBones));

		public class BoneData
		{
			public BoneData()
			{
			}

			public BoneData(OVRUnityHumanoidSkeletonRetargeter.OVRSkeletonMetadata.BoneData otherBoneData)
			{
				this.OriginalJoint = otherBoneData.OriginalJoint;
				this.FromPosition = otherBoneData.FromPosition;
				this.ToPosition = otherBoneData.ToPosition;
				this.JointPairStart = otherBoneData.JointPairStart;
				this.JointPairEnd = otherBoneData.JointPairEnd;
				this.JointPairOrientation = otherBoneData.JointPairOrientation;
				this.CorrectionQuaternion = otherBoneData.CorrectionQuaternion;
				this.ParentTransform = otherBoneData.ParentTransform;
				this.DegenerateJoint = otherBoneData.DegenerateJoint;
			}

			public Transform OriginalJoint;

			public Vector3 FromPosition;

			public Vector3 ToPosition;

			public Transform JointPairStart;

			public Transform JointPairEnd;

			public Quaternion JointPairOrientation;

			public Quaternion? CorrectionQuaternion;

			public Transform ParentTransform;

			public bool DegenerateJoint;
		}
	}

	[Serializable]
	public class JointAdjustment
	{
		public Quaternion PrecomputedRotationTweaks { get; private set; }

		public void PrecomputeRotationTweaks()
		{
			this.PrecomputedRotationTweaks = Quaternion.identity;
			if (this.RotationTweaks == null || this.RotationTweaks.Length == 0)
			{
				return;
			}
			foreach (Quaternion quaternion in this.RotationTweaks)
			{
				if (quaternion.w >= Mathf.Epsilon || quaternion.x >= Mathf.Epsilon || quaternion.y >= Mathf.Epsilon || quaternion.z >= Mathf.Epsilon)
				{
					this.PrecomputedRotationTweaks *= quaternion;
				}
			}
		}

		public HumanBodyBones Joint;

		public Vector3 PositionChange = Vector3.zero;

		public Quaternion RotationChange = Quaternion.identity;

		public Quaternion[] RotationTweaks;

		public bool DisableRotationTransform;

		public bool DisablePositionTransform;

		public OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.FullBodyTrackingBoneId FullBodyBoneIdOverrideValue = OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.FullBodyTrackingBoneId.NoOverride;

		public OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodyTrackingBoneId BoneIdOverrideValue = OVRUnityHumanoidSkeletonRetargeter.OVRHumanBodyBonesMappings.BodyTrackingBoneId.NoOverride;
	}

	public enum UpdateType
	{
		FixedUpdateOnly,
		UpdateOnly,
		FixedUpdateAndUpdate
	}
}
