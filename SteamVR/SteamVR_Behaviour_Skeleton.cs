using System;
using System.Collections;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Behaviour_Skeleton : MonoBehaviour
	{
		public bool skeletonAvailable
		{
			get
			{
				return this.skeletonAction.activeBinding;
			}
		}

		public bool isActive
		{
			get
			{
				return this.skeletonAction.GetActive();
			}
		}

		public float[] fingerCurls
		{
			get
			{
				if (this.skeletonAvailable)
				{
					return this.skeletonAction.GetFingerCurls(false);
				}
				float[] array = new float[5];
				for (int i = 0; i < 5; i++)
				{
					array[i] = this.fallbackCurlAction.GetAxis(this.inputSource);
				}
				return array;
			}
		}

		public float thumbCurl
		{
			get
			{
				if (this.skeletonAvailable)
				{
					return this.skeletonAction.GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.thumb);
				}
				return this.fallbackCurlAction.GetAxis(this.inputSource);
			}
		}

		public float indexCurl
		{
			get
			{
				if (this.skeletonAvailable)
				{
					return this.skeletonAction.GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.index);
				}
				return this.fallbackCurlAction.GetAxis(this.inputSource);
			}
		}

		public float middleCurl
		{
			get
			{
				if (this.skeletonAvailable)
				{
					return this.skeletonAction.GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.middle);
				}
				return this.fallbackCurlAction.GetAxis(this.inputSource);
			}
		}

		public float ringCurl
		{
			get
			{
				if (this.skeletonAvailable)
				{
					return this.skeletonAction.GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.ring);
				}
				return this.fallbackCurlAction.GetAxis(this.inputSource);
			}
		}

		public float pinkyCurl
		{
			get
			{
				if (this.skeletonAvailable)
				{
					return this.skeletonAction.GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.pinky);
				}
				return this.fallbackCurlAction.GetAxis(this.inputSource);
			}
		}

		public Transform root
		{
			get
			{
				return this.bones[0];
			}
		}

		public Transform wrist
		{
			get
			{
				return this.bones[1];
			}
		}

		public Transform indexMetacarpal
		{
			get
			{
				return this.bones[6];
			}
		}

		public Transform indexProximal
		{
			get
			{
				return this.bones[7];
			}
		}

		public Transform indexMiddle
		{
			get
			{
				return this.bones[8];
			}
		}

		public Transform indexDistal
		{
			get
			{
				return this.bones[9];
			}
		}

		public Transform indexTip
		{
			get
			{
				return this.bones[10];
			}
		}

		public Transform middleMetacarpal
		{
			get
			{
				return this.bones[11];
			}
		}

		public Transform middleProximal
		{
			get
			{
				return this.bones[12];
			}
		}

		public Transform middleMiddle
		{
			get
			{
				return this.bones[13];
			}
		}

		public Transform middleDistal
		{
			get
			{
				return this.bones[14];
			}
		}

		public Transform middleTip
		{
			get
			{
				return this.bones[15];
			}
		}

		public Transform pinkyMetacarpal
		{
			get
			{
				return this.bones[21];
			}
		}

		public Transform pinkyProximal
		{
			get
			{
				return this.bones[22];
			}
		}

		public Transform pinkyMiddle
		{
			get
			{
				return this.bones[23];
			}
		}

		public Transform pinkyDistal
		{
			get
			{
				return this.bones[24];
			}
		}

		public Transform pinkyTip
		{
			get
			{
				return this.bones[25];
			}
		}

		public Transform ringMetacarpal
		{
			get
			{
				return this.bones[16];
			}
		}

		public Transform ringProximal
		{
			get
			{
				return this.bones[17];
			}
		}

		public Transform ringMiddle
		{
			get
			{
				return this.bones[18];
			}
		}

		public Transform ringDistal
		{
			get
			{
				return this.bones[19];
			}
		}

		public Transform ringTip
		{
			get
			{
				return this.bones[20];
			}
		}

		public Transform thumbMetacarpal
		{
			get
			{
				return this.bones[2];
			}
		}

		public Transform thumbProximal
		{
			get
			{
				return this.bones[2];
			}
		}

		public Transform thumbMiddle
		{
			get
			{
				return this.bones[3];
			}
		}

		public Transform thumbDistal
		{
			get
			{
				return this.bones[4];
			}
		}

		public Transform thumbTip
		{
			get
			{
				return this.bones[5];
			}
		}

		public Transform thumbAux
		{
			get
			{
				return this.bones[26];
			}
		}

		public Transform indexAux
		{
			get
			{
				return this.bones[27];
			}
		}

		public Transform middleAux
		{
			get
			{
				return this.bones[28];
			}
		}

		public Transform ringAux
		{
			get
			{
				return this.bones[29];
			}
		}

		public Transform pinkyAux
		{
			get
			{
				return this.bones[30];
			}
		}

		public Transform[] proximals { get; protected set; }

		public Transform[] middles { get; protected set; }

		public Transform[] distals { get; protected set; }

		public Transform[] tips { get; protected set; }

		public Transform[] auxs { get; protected set; }

		public EVRSkeletalTrackingLevel skeletalTrackingLevel
		{
			get
			{
				if (this.skeletonAvailable)
				{
					return this.skeletonAction.skeletalTrackingLevel;
				}
				return EVRSkeletalTrackingLevel.VRSkeletalTracking_Estimated;
			}
		}

		public bool isBlending
		{
			get
			{
				return this.blendRoutine != null;
			}
		}

		public SteamVR_ActionSet actionSet
		{
			get
			{
				return this.skeletonAction.actionSet;
			}
		}

		public SteamVR_ActionDirections direction
		{
			get
			{
				return this.skeletonAction.direction;
			}
		}

		protected virtual void Awake()
		{
			SteamVR.Initialize(false);
			this.AssignBonesArray();
			this.proximals = new Transform[]
			{
				this.thumbProximal,
				this.indexProximal,
				this.middleProximal,
				this.ringProximal,
				this.pinkyProximal
			};
			this.middles = new Transform[]
			{
				this.thumbMiddle,
				this.indexMiddle,
				this.middleMiddle,
				this.ringMiddle,
				this.pinkyMiddle
			};
			this.distals = new Transform[]
			{
				this.thumbDistal,
				this.indexDistal,
				this.middleDistal,
				this.ringDistal,
				this.pinkyDistal
			};
			this.tips = new Transform[]
			{
				this.thumbTip,
				this.indexTip,
				this.middleTip,
				this.ringTip,
				this.pinkyTip
			};
			this.auxs = new Transform[]
			{
				this.thumbAux,
				this.indexAux,
				this.middleAux,
				this.ringAux,
				this.pinkyAux
			};
			this.CheckSkeletonAction();
		}

		protected virtual void CheckSkeletonAction()
		{
			if (this.skeletonAction == null)
			{
				this.skeletonAction = SteamVR_Input.GetAction<SteamVR_Action_Skeleton>("Skeleton" + this.inputSource.ToString(), false);
			}
		}

		protected virtual void AssignBonesArray()
		{
			this.bones = this.skeletonRoot.GetComponentsInChildren<Transform>();
		}

		protected virtual void OnEnable()
		{
			this.CheckSkeletonAction();
			SteamVR_Input.onSkeletonsUpdated += this.SteamVR_Input_OnSkeletonsUpdated;
			if (this.skeletonAction != null)
			{
				this.skeletonAction.onDeviceConnectedChanged += this.OnDeviceConnectedChanged;
				this.skeletonAction.onTrackingChanged += this.OnTrackingChanged;
			}
		}

		protected virtual void OnDisable()
		{
			SteamVR_Input.onSkeletonsUpdated -= this.SteamVR_Input_OnSkeletonsUpdated;
			if (this.skeletonAction != null)
			{
				this.skeletonAction.onDeviceConnectedChanged -= this.OnDeviceConnectedChanged;
				this.skeletonAction.onTrackingChanged -= this.OnTrackingChanged;
			}
		}

		private void OnDeviceConnectedChanged(SteamVR_Action_Skeleton fromAction, bool deviceConnected)
		{
			if (this.onConnectedChanged != null)
			{
				this.onConnectedChanged.Invoke(this, this.inputSource, deviceConnected);
			}
			if (this.onConnectedChangedEvent != null)
			{
				this.onConnectedChangedEvent(this, this.inputSource, deviceConnected);
			}
		}

		private void OnTrackingChanged(SteamVR_Action_Skeleton fromAction, ETrackingResult trackingState)
		{
			if (this.onTrackingChanged != null)
			{
				this.onTrackingChanged.Invoke(this, this.inputSource, trackingState);
			}
			if (this.onTrackingChangedEvent != null)
			{
				this.onTrackingChangedEvent(this, this.inputSource, trackingState);
			}
		}

		protected virtual void SteamVR_Input_OnSkeletonsUpdated(bool skipSendingEvents)
		{
			this.UpdateSkeleton();
		}

		protected virtual void UpdateSkeleton()
		{
			if (this.skeletonAction == null)
			{
				return;
			}
			if (this.updatePose)
			{
				this.UpdatePose();
			}
			if (this.blendPoser != null && this.skeletonBlend < 1f)
			{
				if (this.blendSnapshot == null)
				{
					this.blendSnapshot = this.blendPoser.GetBlendedPose(this);
				}
				this.blendSnapshot = this.blendPoser.GetBlendedPose(this);
			}
			if (this.rangeOfMotionBlendRoutine == null)
			{
				if (this.temporaryRangeOfMotion != null)
				{
					this.skeletonAction.SetRangeOfMotion(this.temporaryRangeOfMotion.Value);
				}
				else
				{
					this.skeletonAction.SetRangeOfMotion(this.rangeOfMotion);
				}
				this.UpdateSkeletonTransforms();
			}
		}

		public void SetTemporaryRangeOfMotion(EVRSkeletalMotionRange newRangeOfMotion, float blendOverSeconds = 0.1f)
		{
			if (this.rangeOfMotion == newRangeOfMotion)
			{
				EVRSkeletalMotionRange? evrskeletalMotionRange = this.temporaryRangeOfMotion;
				if (evrskeletalMotionRange.GetValueOrDefault() == newRangeOfMotion & evrskeletalMotionRange != null)
				{
					return;
				}
			}
			this.TemporaryRangeOfMotionBlend(newRangeOfMotion, blendOverSeconds);
		}

		public void ResetTemporaryRangeOfMotion(float blendOverSeconds = 0.1f)
		{
			this.ResetTemporaryRangeOfMotionBlend(blendOverSeconds);
		}

		public void SetRangeOfMotion(EVRSkeletalMotionRange newRangeOfMotion, float blendOverSeconds = 0.1f)
		{
			if (this.rangeOfMotion != newRangeOfMotion)
			{
				this.RangeOfMotionBlend(newRangeOfMotion, blendOverSeconds);
			}
		}

		public void BlendToSkeleton(float overTime = 0.1f)
		{
			if (this.blendPoser != null)
			{
				this.blendSnapshot = this.blendPoser.GetBlendedPose(this);
			}
			this.blendPoser = null;
			this.BlendTo(1f, overTime);
		}

		public void BlendToPoser(SteamVR_Skeleton_Poser poser, float overTime = 0.1f)
		{
			if (poser == null)
			{
				return;
			}
			this.blendPoser = poser;
			this.BlendTo(0f, overTime);
		}

		public void BlendToAnimation(float overTime = 0.1f)
		{
			this.BlendTo(0f, overTime);
		}

		public void BlendTo(float blendToAmount, float overTime)
		{
			if (this.blendRoutine != null)
			{
				base.StopCoroutine(this.blendRoutine);
			}
			if (base.gameObject.activeInHierarchy)
			{
				this.blendRoutine = base.StartCoroutine(this.DoBlendRoutine(blendToAmount, overTime));
			}
		}

		protected IEnumerator DoBlendRoutine(float blendToAmount, float overTime)
		{
			float startTime = Time.time;
			float endTime = startTime + overTime;
			float startAmount = this.skeletonBlend;
			while (Time.time < endTime)
			{
				yield return null;
				this.skeletonBlend = Mathf.Lerp(startAmount, blendToAmount, (Time.time - startTime) / overTime);
			}
			this.skeletonBlend = blendToAmount;
			this.blendRoutine = null;
			yield break;
		}

		protected void RangeOfMotionBlend(EVRSkeletalMotionRange newRangeOfMotion, float blendOverSeconds)
		{
			if (this.rangeOfMotionBlendRoutine != null)
			{
				base.StopCoroutine(this.rangeOfMotionBlendRoutine);
			}
			EVRSkeletalMotionRange oldRangeOfMotion = this.rangeOfMotion;
			this.rangeOfMotion = newRangeOfMotion;
			if (base.gameObject.activeInHierarchy)
			{
				this.rangeOfMotionBlendRoutine = base.StartCoroutine(this.DoRangeOfMotionBlend(oldRangeOfMotion, newRangeOfMotion, blendOverSeconds));
			}
		}

		protected void TemporaryRangeOfMotionBlend(EVRSkeletalMotionRange newRangeOfMotion, float blendOverSeconds)
		{
			if (this.rangeOfMotionBlendRoutine != null)
			{
				base.StopCoroutine(this.rangeOfMotionBlendRoutine);
			}
			EVRSkeletalMotionRange value = this.rangeOfMotion;
			if (this.temporaryRangeOfMotion != null)
			{
				value = this.temporaryRangeOfMotion.Value;
			}
			this.temporaryRangeOfMotion = new EVRSkeletalMotionRange?(newRangeOfMotion);
			if (base.gameObject.activeInHierarchy)
			{
				this.rangeOfMotionBlendRoutine = base.StartCoroutine(this.DoRangeOfMotionBlend(value, newRangeOfMotion, blendOverSeconds));
			}
		}

		protected void ResetTemporaryRangeOfMotionBlend(float blendOverSeconds)
		{
			if (this.temporaryRangeOfMotion != null)
			{
				if (this.rangeOfMotionBlendRoutine != null)
				{
					base.StopCoroutine(this.rangeOfMotionBlendRoutine);
				}
				EVRSkeletalMotionRange value = this.temporaryRangeOfMotion.Value;
				EVRSkeletalMotionRange newRangeOfMotion = this.rangeOfMotion;
				this.temporaryRangeOfMotion = null;
				if (base.gameObject.activeInHierarchy)
				{
					this.rangeOfMotionBlendRoutine = base.StartCoroutine(this.DoRangeOfMotionBlend(value, newRangeOfMotion, blendOverSeconds));
				}
			}
		}

		protected IEnumerator DoRangeOfMotionBlend(EVRSkeletalMotionRange oldRangeOfMotion, EVRSkeletalMotionRange newRangeOfMotion, float overTime)
		{
			float startTime = Time.time;
			float endTime = startTime + overTime;
			while (Time.time < endTime)
			{
				yield return null;
				float t = (Time.time - startTime) / overTime;
				if (this.skeletonBlend > 0f)
				{
					this.skeletonAction.SetRangeOfMotion(oldRangeOfMotion);
					this.skeletonAction.UpdateValueWithoutEvents();
					Vector3[] array = (Vector3[])this.GetBonePositions().Clone();
					Quaternion[] array2 = (Quaternion[])this.GetBoneRotations().Clone();
					this.skeletonAction.SetRangeOfMotion(newRangeOfMotion);
					this.skeletonAction.UpdateValueWithoutEvents();
					Vector3[] bonePositions = this.GetBonePositions();
					Quaternion[] boneRotations = this.GetBoneRotations();
					for (int i = 0; i < this.bones.Length; i++)
					{
						if (!(this.bones[i] == null) && SteamVR_Utils.IsValid(boneRotations[i]) && SteamVR_Utils.IsValid(array2[i]))
						{
							Vector3 vector = Vector3.Lerp(array[i], bonePositions[i], t);
							Quaternion quaternion = Quaternion.Lerp(array2[i], boneRotations[i], t);
							if (this.skeletonBlend < 1f)
							{
								if (this.blendPoser != null)
								{
									this.SetBonePosition(i, Vector3.Lerp(this.blendSnapshot.bonePositions[i], vector, this.skeletonBlend));
									this.SetBoneRotation(i, Quaternion.Lerp(this.GetBlendPoseForBone(i, quaternion), quaternion, this.skeletonBlend));
								}
								else
								{
									this.SetBonePosition(i, Vector3.Lerp(this.bones[i].localPosition, vector, this.skeletonBlend));
									this.SetBoneRotation(i, Quaternion.Lerp(this.bones[i].localRotation, quaternion, this.skeletonBlend));
								}
							}
							else
							{
								this.SetBonePosition(i, vector);
								this.SetBoneRotation(i, quaternion);
							}
						}
					}
				}
				if (this.onBoneTransformsUpdated != null)
				{
					this.onBoneTransformsUpdated.Invoke(this, this.inputSource);
				}
				if (this.onBoneTransformsUpdatedEvent != null)
				{
					this.onBoneTransformsUpdatedEvent(this, this.inputSource);
				}
			}
			this.rangeOfMotionBlendRoutine = null;
			yield break;
		}

		protected virtual Quaternion GetBlendPoseForBone(int boneIndex, Quaternion skeletonRotation)
		{
			return this.blendSnapshot.boneRotations[boneIndex];
		}

		public virtual void UpdateSkeletonTransforms()
		{
			Vector3[] bonePositions = this.GetBonePositions();
			Quaternion[] boneRotations = this.GetBoneRotations();
			if (this.skeletonBlend <= 0f)
			{
				if (this.blendPoser != null)
				{
					SteamVR_Skeleton_Pose_Hand hand = this.blendPoser.skeletonMainPose.GetHand(this.inputSource);
					for (int i = 0; i < this.bones.Length; i++)
					{
						if (!(this.bones[i] == null))
						{
							if ((i == 1 && hand.ignoreWristPoseData) || (i == 0 && hand.ignoreRootPoseData))
							{
								this.SetBonePosition(i, bonePositions[i]);
								this.SetBoneRotation(i, boneRotations[i]);
							}
							else
							{
								Quaternion blendPoseForBone = this.GetBlendPoseForBone(i, boneRotations[i]);
								this.SetBonePosition(i, this.blendSnapshot.bonePositions[i]);
								this.SetBoneRotation(i, blendPoseForBone);
							}
						}
					}
				}
				else
				{
					for (int j = 0; j < this.bones.Length; j++)
					{
						Quaternion blendPoseForBone2 = this.GetBlendPoseForBone(j, boneRotations[j]);
						this.SetBonePosition(j, this.blendSnapshot.bonePositions[j]);
						this.SetBoneRotation(j, blendPoseForBone2);
					}
				}
			}
			else if (this.skeletonBlend >= 1f)
			{
				for (int k = 0; k < this.bones.Length; k++)
				{
					if (!(this.bones[k] == null))
					{
						this.SetBonePosition(k, bonePositions[k]);
						this.SetBoneRotation(k, boneRotations[k]);
					}
				}
			}
			else
			{
				for (int l = 0; l < this.bones.Length; l++)
				{
					if (!(this.bones[l] == null))
					{
						if (this.blendPoser != null)
						{
							SteamVR_Skeleton_Pose_Hand hand2 = this.blendPoser.skeletonMainPose.GetHand(this.inputSource);
							if ((l == 1 && hand2.ignoreWristPoseData) || (l == 0 && hand2.ignoreRootPoseData))
							{
								this.SetBonePosition(l, bonePositions[l]);
								this.SetBoneRotation(l, boneRotations[l]);
							}
							else
							{
								this.SetBonePosition(l, Vector3.Lerp(this.blendSnapshot.bonePositions[l], bonePositions[l], this.skeletonBlend));
								this.SetBoneRotation(l, Quaternion.Lerp(this.blendSnapshot.boneRotations[l], boneRotations[l], this.skeletonBlend));
							}
						}
						else if (this.blendSnapshot == null)
						{
							this.SetBonePosition(l, Vector3.Lerp(this.bones[l].localPosition, bonePositions[l], this.skeletonBlend));
							this.SetBoneRotation(l, Quaternion.Lerp(this.bones[l].localRotation, boneRotations[l], this.skeletonBlend));
						}
						else
						{
							this.SetBonePosition(l, Vector3.Lerp(this.blendSnapshot.bonePositions[l], bonePositions[l], this.skeletonBlend));
							this.SetBoneRotation(l, Quaternion.Lerp(this.blendSnapshot.boneRotations[l], boneRotations[l], this.skeletonBlend));
						}
					}
				}
			}
			if (this.onBoneTransformsUpdated != null)
			{
				this.onBoneTransformsUpdated.Invoke(this, this.inputSource);
			}
			if (this.onBoneTransformsUpdatedEvent != null)
			{
				this.onBoneTransformsUpdatedEvent(this, this.inputSource);
			}
		}

		public virtual void SetBonePosition(int boneIndex, Vector3 localPosition)
		{
			if (!this.onlySetRotations)
			{
				this.bones[boneIndex].localPosition = localPosition;
			}
		}

		public virtual void SetBoneRotation(int boneIndex, Quaternion localRotation)
		{
			this.bones[boneIndex].localRotation = localRotation;
		}

		public virtual Transform GetBone(int joint)
		{
			if (this.bones == null || this.bones.Length == 0)
			{
				this.Awake();
			}
			return this.bones[joint];
		}

		public Vector3 GetBonePosition(int joint, bool local = false)
		{
			if (local)
			{
				return this.bones[joint].localPosition;
			}
			return this.bones[joint].position;
		}

		public Quaternion GetBoneRotation(int joint, bool local = false)
		{
			if (local)
			{
				return this.bones[joint].localRotation;
			}
			return this.bones[joint].rotation;
		}

		protected Vector3[] GetBonePositions()
		{
			if (this.skeletonAvailable)
			{
				Vector3[] bonePositions = this.skeletonAction.GetBonePositions(false);
				if (this.mirroring == SteamVR_Behaviour_Skeleton.MirrorType.LeftToRight || this.mirroring == SteamVR_Behaviour_Skeleton.MirrorType.RightToLeft)
				{
					for (int i = 0; i < bonePositions.Length; i++)
					{
						bonePositions[i] = SteamVR_Behaviour_Skeleton.MirrorPosition(i, bonePositions[i]);
					}
				}
				return bonePositions;
			}
			if (this.fallbackPoser != null)
			{
				return this.fallbackPoser.GetBlendedPose(this.skeletonAction, this.inputSource).bonePositions;
			}
			Debug.LogError("Skeleton Action is not bound, and you have not provided a fallback SkeletonPoser. Please create one to drive hand animation when no skeleton data is available.", this);
			return null;
		}

		protected Quaternion[] GetBoneRotations()
		{
			if (this.skeletonAvailable)
			{
				Quaternion[] boneRotations = this.skeletonAction.GetBoneRotations(false);
				if (this.mirroring == SteamVR_Behaviour_Skeleton.MirrorType.LeftToRight || this.mirroring == SteamVR_Behaviour_Skeleton.MirrorType.RightToLeft)
				{
					for (int i = 0; i < boneRotations.Length; i++)
					{
						boneRotations[i] = SteamVR_Behaviour_Skeleton.MirrorRotation(i, boneRotations[i]);
					}
				}
				return boneRotations;
			}
			if (this.fallbackPoser != null)
			{
				return this.fallbackPoser.GetBlendedPose(this.skeletonAction, this.inputSource).boneRotations;
			}
			Debug.LogError("Skeleton Action is not bound, and you have not provided a fallback SkeletonPoser. Please create one to drive hand animation when no skeleton data is available.", this);
			return null;
		}

		public static Vector3 MirrorPosition(int boneIndex, Vector3 rawPosition)
		{
			if (boneIndex == 1 || SteamVR_Behaviour_Skeleton.IsMetacarpal(boneIndex))
			{
				rawPosition.Scale(new Vector3(-1f, 1f, 1f));
			}
			else if (boneIndex != 0)
			{
				rawPosition *= -1f;
			}
			return rawPosition;
		}

		public static Quaternion MirrorRotation(int boneIndex, Quaternion rawRotation)
		{
			if (boneIndex == 1)
			{
				rawRotation.y *= -1f;
				rawRotation.z *= -1f;
			}
			if (SteamVR_Behaviour_Skeleton.IsMetacarpal(boneIndex))
			{
				rawRotation = SteamVR_Behaviour_Skeleton.rightFlipAngle * rawRotation;
			}
			return rawRotation;
		}

		protected virtual void UpdatePose()
		{
			if (this.skeletonAction == null)
			{
				return;
			}
			Vector3 position = this.skeletonAction.GetLocalPosition();
			Quaternion quaternion = this.skeletonAction.GetLocalRotation();
			if (this.origin == null)
			{
				if (base.transform.parent != null)
				{
					position = base.transform.parent.TransformPoint(position);
					quaternion = base.transform.parent.rotation * quaternion;
				}
			}
			else
			{
				position = this.origin.TransformPoint(position);
				quaternion = this.origin.rotation * quaternion;
			}
			if (this.skeletonAction.poseChanged)
			{
				if (this.onTransformChanged != null)
				{
					this.onTransformChanged.Invoke(this, this.inputSource);
				}
				if (this.onTransformChangedEvent != null)
				{
					this.onTransformChangedEvent(this, this.inputSource);
				}
			}
			base.transform.position = position;
			base.transform.rotation = quaternion;
			if (this.onTransformUpdated != null)
			{
				this.onTransformUpdated.Invoke(this, this.inputSource);
			}
		}

		public void ForceToReferencePose(EVRSkeletalReferencePose referencePose)
		{
			bool flag = false;
			if (Application.isEditor && !Application.isPlaying)
			{
				flag = SteamVR.InitializeTemporarySession(true);
				this.Awake();
				this.skeletonAction.actionSet.Activate(SteamVR_Input_Sources.Any, 0, false);
				SteamVR_ActionSet_Manager.UpdateActionStates(true);
				this.skeletonAction.UpdateValueWithoutEvents();
			}
			if (!this.skeletonAction.active)
			{
				Debug.LogError("<b>[SteamVR Input]</b> Please turn on your " + this.inputSource.ToString() + " controller and ensure SteamVR is open.", this);
				return;
			}
			SteamVR_Utils.RigidTransform[] referenceTransforms = this.skeletonAction.GetReferenceTransforms(EVRSkeletalTransformSpace.Parent, referencePose);
			if (referenceTransforms == null || referenceTransforms.Length == 0)
			{
				Debug.LogError("<b>[SteamVR Input]</b> Unable to get the reference transform for " + this.inputSource.ToString() + ". Please make sure SteamVR is open and both controllers are connected.", this);
			}
			if (this.mirroring == SteamVR_Behaviour_Skeleton.MirrorType.LeftToRight || this.mirroring == SteamVR_Behaviour_Skeleton.MirrorType.RightToLeft)
			{
				for (int i = 0; i < referenceTransforms.Length; i++)
				{
					this.bones[i].localPosition = SteamVR_Behaviour_Skeleton.MirrorPosition(i, referenceTransforms[i].pos);
					this.bones[i].localRotation = SteamVR_Behaviour_Skeleton.MirrorRotation(i, referenceTransforms[i].rot);
				}
			}
			else
			{
				for (int j = 0; j < referenceTransforms.Length; j++)
				{
					this.bones[j].localPosition = referenceTransforms[j].pos;
					this.bones[j].localRotation = referenceTransforms[j].rot;
				}
			}
			if (flag)
			{
				SteamVR.ExitTemporarySession();
			}
		}

		protected static bool IsMetacarpal(int boneIndex)
		{
			return boneIndex == 6 || boneIndex == 11 || boneIndex == 16 || boneIndex == 21 || boneIndex == 2;
		}

		[Tooltip("If not set, will try to auto assign this based on 'Skeleton' + inputSource")]
		public SteamVR_Action_Skeleton skeletonAction;

		[Tooltip("The device this action should apply to. Any if the action is not device specific.")]
		public SteamVR_Input_Sources inputSource;

		[Tooltip("The range of motion you'd like the hand to move in. With controller is the best estimate of the fingers wrapped around a controller. Without is from a flat hand to a fist.")]
		public EVRSkeletalMotionRange rangeOfMotion = EVRSkeletalMotionRange.WithoutController;

		[Tooltip("This needs to be in the order of: root -> wrist -> thumb, index, middle, ring, pinky")]
		public Transform skeletonRoot;

		[Tooltip("If not set, relative to parent")]
		public Transform origin;

		[Tooltip("Set to true if you want this script to update its position and rotation. False if this will be handled elsewhere")]
		public bool updatePose = true;

		[Tooltip("Check this to not set the positions of the bones. This is helpful for differently scaled skeletons.")]
		public bool onlySetRotations;

		[Range(0f, 1f)]
		[Tooltip("Modify this to blend between animations setup on the hand")]
		public float skeletonBlend = 1f;

		public SteamVR_Behaviour_SkeletonEvent onBoneTransformsUpdated;

		public SteamVR_Behaviour_SkeletonEvent onTransformUpdated;

		public SteamVR_Behaviour_SkeletonEvent onTransformChanged;

		public SteamVR_Behaviour_Skeleton_ConnectedChangedEvent onConnectedChanged;

		public SteamVR_Behaviour_Skeleton_TrackingChangedEvent onTrackingChanged;

		public SteamVR_Behaviour_Skeleton.UpdateHandler onBoneTransformsUpdatedEvent;

		public SteamVR_Behaviour_Skeleton.UpdateHandler onTransformUpdatedEvent;

		public SteamVR_Behaviour_Skeleton.ChangeHandler onTransformChangedEvent;

		public SteamVR_Behaviour_Skeleton.DeviceConnectedChangeHandler onConnectedChangedEvent;

		public SteamVR_Behaviour_Skeleton.TrackingChangeHandler onTrackingChangedEvent;

		[Tooltip("Is this rendermodel a mirror of another one?")]
		public SteamVR_Behaviour_Skeleton.MirrorType mirroring;

		[Header("No Skeleton - Fallback")]
		[Tooltip("The fallback SkeletonPoser to drive hand animation when no skeleton data is available")]
		public SteamVR_Skeleton_Poser fallbackPoser;

		[Tooltip("The fallback action to drive finger curl values when no skeleton data is available")]
		public SteamVR_Action_Single fallbackCurlAction;

		protected SteamVR_Skeleton_Poser blendPoser;

		protected SteamVR_Skeleton_PoseSnapshot blendSnapshot;

		protected Coroutine blendRoutine;

		protected Coroutine rangeOfMotionBlendRoutine;

		protected Coroutine attachRoutine;

		protected Transform[] bones;

		protected EVRSkeletalMotionRange? temporaryRangeOfMotion;

		protected static readonly Quaternion rightFlipAngle = Quaternion.AngleAxis(180f, Vector3.right);

		public enum MirrorType
		{
			None,
			LeftToRight,
			RightToLeft
		}

		public delegate void ActiveChangeHandler(SteamVR_Behaviour_Skeleton fromAction, SteamVR_Input_Sources inputSource, bool active);

		public delegate void ChangeHandler(SteamVR_Behaviour_Skeleton fromAction, SteamVR_Input_Sources inputSource);

		public delegate void UpdateHandler(SteamVR_Behaviour_Skeleton fromAction, SteamVR_Input_Sources inputSource);

		public delegate void TrackingChangeHandler(SteamVR_Behaviour_Skeleton fromAction, SteamVR_Input_Sources inputSource, ETrackingResult trackingState);

		public delegate void ValidPoseChangeHandler(SteamVR_Behaviour_Skeleton fromAction, SteamVR_Input_Sources inputSource, bool validPose);

		public delegate void DeviceConnectedChangeHandler(SteamVR_Behaviour_Skeleton fromAction, SteamVR_Input_Sources inputSource, bool deviceConnected);
	}
}
