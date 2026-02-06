using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class GorillaIKMgr : MonoBehaviour
{
	public static GorillaIKMgr Instance
	{
		get
		{
			return GorillaIKMgr._instance;
		}
	}

	private void Awake()
	{
		GorillaIKMgr._instance = this;
		this.firstFrame = true;
		this.tAA = new TransformAccessArray(0, -1);
		this.transformList = new List<Transform>();
		this.job = new GorillaIKMgr.IKJob
		{
			constantInput = new NativeArray<GorillaIKMgr.IKConstantInput>(40, Allocator.Persistent, NativeArrayOptions.ClearMemory),
			input = new NativeArray<GorillaIKMgr.IKInput>(40, Allocator.Persistent, NativeArrayOptions.ClearMemory),
			output = new NativeArray<GorillaIKMgr.IKOutput>(40, Allocator.Persistent, NativeArrayOptions.ClearMemory)
		};
		this.jobXform = new GorillaIKMgr.IKTransformJob
		{
			transformRotations = new NativeArray<Quaternion>(160, Allocator.Persistent, NativeArrayOptions.ClearMemory),
			transformPositions = new NativeArray<Vector3>(160, Allocator.Persistent, NativeArrayOptions.ClearMemory)
		};
	}

	private void OnDestroy()
	{
		this.jobHandle.Complete();
		this.jobXformHandle.Complete();
		this.jobXform.transformRotations.Dispose();
		this.jobXform.transformPositions.Dispose();
		this.tAA.Dispose();
		this.job.input.Dispose();
		this.job.constantInput.Dispose();
		this.job.output.Dispose();
	}

	public void RegisterIK(GorillaIK ik)
	{
		this.ikList.Add(ik);
		this.actualListSz += 2;
		this.updatedSinceLastRun = true;
		if (this.job.constantInput.IsCreated)
		{
			this.SetConstantData(ik, this.actualListSz - 2);
		}
	}

	public void DeregisterIK(GorillaIK ik)
	{
		int num = this.ikList.FindIndex((GorillaIK curr) => curr == ik);
		this.updatedSinceLastRun = true;
		this.ikList.RemoveAt(num);
		this.actualListSz -= 2;
		if (this.job.constantInput.IsCreated)
		{
			for (int i = num; i < this.actualListSz; i++)
			{
				this.job.constantInput[i] = this.job.constantInput[i + 2];
			}
		}
	}

	private void SetConstantData(GorillaIK ik, int index)
	{
		this.job.constantInput[index] = new GorillaIKMgr.IKConstantInput
		{
			initRotLower = ik.initialLowerLeft,
			initRotUpper = ik.initialUpperLeft,
			shoulderPosition = new Vector3(-0.018300775f, -0.04206751f, 0.08612572f),
			bodyPivotPos = new Vector3(0f, 0.011406422f, 1.6582015f),
			shoulderRot = new Quaternion(-0.59150106f, 0.3665933f, 0.20795153f, 0.68738055f)
		};
		this.job.constantInput[index + 1] = new GorillaIKMgr.IKConstantInput
		{
			initRotLower = ik.initialLowerRight,
			initRotUpper = ik.initialUpperRight,
			shoulderPosition = new Vector3(0.018300813f, -0.042066876f, 0.08613044f),
			bodyPivotPos = new Vector3(0f, 0.011406422f, 1.6582015f),
			shoulderRot = new Quaternion(-0.591501f, -0.3665933f, -0.20795153f, 0.6873807f)
		};
	}

	private void CopyInput()
	{
		int num = 0;
		int i = 0;
		while (i < this.actualListSz)
		{
			GorillaIK gorillaIK = this.ikList[i / 2];
			bool flag = gorillaIK.usingUpdatedIK && SubscriptionManager.GetSubscriptionDetails(gorillaIK.myRig).active;
			if (gorillaIK != GorillaIKMgr.playerIK)
			{
				gorillaIK.lerpLeftElbowDirection = Vector3.Lerp(gorillaIK.lerpLeftElbowDirection, gorillaIK.leftElbowDirection, this.lerpValue);
				gorillaIK.lerpRightElbowDirection = Vector3.Lerp(gorillaIK.lerpRightElbowDirection, gorillaIK.rightElbowDirection, this.lerpValue);
				gorillaIK.lerpBodyRot = (flag ? Quaternion.Lerp(gorillaIK.lerpBodyRot, gorillaIK.targetBodyRot, this.lerpValue) : gorillaIK.bodyInitialRot);
			}
			else
			{
				gorillaIK.lerpLeftElbowDirection = gorillaIK.leftElbowDirection;
				gorillaIK.lerpRightElbowDirection = gorillaIK.rightElbowDirection;
				gorillaIK.lerpBodyRot = (flag ? gorillaIK.targetBodyRot : gorillaIK.bodyInitialRot);
			}
			this.job.input[i] = new GorillaIKMgr.IKInput
			{
				targetPos = gorillaIK.GetShoulderLocalTargetPos_Left(flag),
				elbowDir = gorillaIK.lerpLeftElbowDirection,
				bodyRot = gorillaIK.lerpBodyRot,
				usingNewIK = flag
			};
			this.job.input[i + 1] = new GorillaIKMgr.IKInput
			{
				targetPos = gorillaIK.GetShoulderLocalTargetPos_Right(flag),
				elbowDir = gorillaIK.lerpRightElbowDirection,
				bodyRot = gorillaIK.lerpBodyRot,
				usingNewIK = flag
			};
			gorillaIK.ClearOverrides();
			i += 2;
			num++;
		}
	}

	private void CopyOutput()
	{
		bool flag = false;
		if (this.updatedSinceLastRun || this.tAA.length != this.ikList.Count * 8)
		{
			flag = true;
			this.tAA.Dispose();
			this.transformList.Clear();
		}
		for (int i = 0; i < this.ikList.Count; i++)
		{
			GorillaIK gorillaIK = this.ikList[i];
			if (flag || this.updatedSinceLastRun)
			{
				this.transformList.Add(gorillaIK.leftUpperArm);
				this.transformList.Add(gorillaIK.leftLowerArm);
				this.transformList.Add(gorillaIK.rightUpperArm);
				this.transformList.Add(gorillaIK.rightLowerArm);
				this.transformList.Add(gorillaIK.bodyBone);
				this.transformList.Add(gorillaIK.headBone);
				this.transformList.Add(gorillaIK.leftHand);
				this.transformList.Add(gorillaIK.rightHand);
			}
			this.jobXform.transformRotations[8 * i] = this.job.output[i * 2].upperArmLocalRot;
			this.jobXform.transformRotations[8 * i + 1] = this.job.output[i * 2].lowerArmLocalRot;
			this.jobXform.transformRotations[8 * i + 2] = this.job.output[i * 2 + 1].upperArmLocalRot;
			this.jobXform.transformRotations[8 * i + 3] = this.job.output[i * 2 + 1].lowerArmLocalRot;
			this.jobXform.transformRotations[8 * i + 4] = gorillaIK.lerpBodyRot;
			this.jobXform.transformRotations[8 * i + 5] = gorillaIK.targetHead.rotation;
			this.jobXform.transformRotations[8 * i + 6] = gorillaIK.targetLeft.rotation;
			this.jobXform.transformRotations[8 * i + 7] = gorillaIK.targetRight.rotation;
			this.jobXform.transformPositions[8 * i + 6] = this.job.output[i * 2].handLocalPosition;
			this.jobXform.transformPositions[8 * i + 7] = this.job.output[i * 2 + 1].handLocalPosition;
		}
		if (flag)
		{
			this.tAA = new TransformAccessArray(this.transformList.ToArray(), -1);
		}
		this.updatedSinceLastRun = false;
	}

	public void LateUpdate()
	{
		GorillaIK gorillaIK = GorillaIKMgr.playerIK;
		if (gorillaIK != null)
		{
			gorillaIK.SkeletonUpdate();
		}
		if (!this.firstFrame)
		{
			this.jobXformHandle.Complete();
		}
		this.CopyInput();
		this.jobHandle = this.job.Schedule(this.actualListSz, 20, default(JobHandle));
		this.jobHandle.Complete();
		this.CopyOutput();
		this.jobXformHandle = this.jobXform.Schedule(this.tAA, default(JobHandle));
		this.firstFrame = false;
	}

	public static void AddPlayerIK(GorillaIK _playerIK)
	{
		GorillaIKMgr.playerIK = _playerIK;
	}

	[OnEnterPlay_SetNull]
	private static GorillaIKMgr _instance;

	private const int MaxSize = 20;

	private List<GorillaIK> ikList = new List<GorillaIK>(20);

	private int actualListSz;

	private JobHandle jobHandle;

	private JobHandle jobXformHandle;

	private bool firstFrame = true;

	private TransformAccessArray tAA;

	private List<Transform> transformList;

	private bool updatedSinceLastRun;

	public const int tFormCount = 8;

	public static GorillaIK playerIK;

	private float lerpValue = 0.155f;

	private GorillaIKMgr.IKJob job;

	private GorillaIKMgr.IKTransformJob jobXform;

	private struct IKConstantInput
	{
		public Quaternion initRotLower;

		public Quaternion initRotUpper;

		public Vector3 shoulderPosition;

		public Vector3 bodyPivotPos;

		public Quaternion bodyStartRot;

		public Quaternion shoulderRot;
	}

	private struct IKInput
	{
		public bool usingNewIK;

		public Vector3 targetPos;

		public Vector3 elbowDir;

		public Quaternion bodyRot;
	}

	private struct IKOutput
	{
		public IKOutput(Quaternion upperArmLocalRot_, Quaternion lowerArmLocalRot_, Vector3 _handLocalPosition)
		{
			this.upperArmLocalRot = upperArmLocalRot_;
			this.lowerArmLocalRot = lowerArmLocalRot_;
			this.handLocalPosition = _handLocalPosition;
		}

		public Quaternion upperArmLocalRot;

		public Quaternion lowerArmLocalRot;

		public Vector3 handLocalPosition;
	}

	[BurstCompile]
	private struct IKJob : IJobParallelFor
	{
		public void Execute(int i)
		{
			Quaternion initRotUpper = this.constantInput[i].initRotUpper;
			Vector3 vector = GorillaIKMgr.IKJob.upperArmLocalPos;
			Quaternion rotation = initRotUpper * this.constantInput[i].initRotLower;
			Vector3 vector2 = vector + initRotUpper * GorillaIKMgr.IKJob.forearmLocalPos;
			Vector3 vector3 = vector2 + rotation * GorillaIKMgr.IKJob.handLocalPos;
			float num = 0.001f;
			float magnitude = (vector - vector2).magnitude;
			float magnitude2 = (vector2 - vector3).magnitude;
			float max = magnitude + magnitude2 - num;
			Vector3 normalized = (vector3 - vector).normalized;
			Vector3 normalized2 = (vector2 - vector).normalized;
			Vector3 normalized3 = (vector3 - vector2).normalized;
			Vector3 normalized4 = (this.input[i].targetPos - vector).normalized;
			float num2 = Mathf.Clamp((this.input[i].targetPos - vector).magnitude, num, max);
			float num3 = Mathf.Acos(Mathf.Clamp(Vector3.Dot(normalized, normalized2), -1f, 1f));
			float num4 = Mathf.Acos(Mathf.Clamp(Vector3.Dot(-normalized2, normalized3), -1f, 1f));
			float num5 = Mathf.Acos(Mathf.Clamp(Vector3.Dot(normalized, normalized4), -1f, 1f));
			float num6 = Mathf.Acos(Mathf.Clamp((magnitude2 * magnitude2 - magnitude * magnitude - num2 * num2) / (-2f * magnitude * num2), -1f, 1f));
			float num7 = Mathf.Acos(Mathf.Clamp((num2 * num2 - magnitude * magnitude - magnitude2 * magnitude2) / (-2f * magnitude * magnitude2), -1f, 1f));
			Vector3 normalized5 = Vector3.Cross(normalized, normalized2).normalized;
			Vector3 normalized6 = Vector3.Cross(normalized, normalized4).normalized;
			Quaternion rhs = Quaternion.AngleAxis((num6 - num3) * 57.29578f, Quaternion.Inverse(initRotUpper) * normalized5);
			Quaternion rhs2 = Quaternion.AngleAxis((num7 - num4) * 57.29578f, Quaternion.Inverse(rotation) * normalized5);
			Quaternion rhs3 = Quaternion.AngleAxis(num5 * 57.29578f, Quaternion.Inverse(initRotUpper) * normalized6);
			Quaternion quaternion = this.constantInput[i].initRotUpper * rhs3 * rhs;
			Quaternion quaternion2 = this.constantInput[i].initRotLower * rhs2;
			Quaternion quaternion3 = this.input[i].bodyRot * this.constantInput[i].shoulderRot;
			Quaternion quaternion4 = quaternion3 * quaternion;
			Quaternion rotation2 = quaternion4 * quaternion2;
			Vector3 handLocalPosition = this.constantInput[i].bodyPivotPos + this.input[i].bodyRot * this.constantInput[i].shoulderPosition + quaternion3 * GorillaIKMgr.IKJob.upperArmLocalPos + quaternion4 * GorillaIKMgr.IKJob.forearmLocalPos + rotation2 * GorillaIKMgr.IKJob.handLocalPos;
			if (!this.input[i].usingNewIK)
			{
				this.output[i] = new GorillaIKMgr.IKOutput(quaternion, quaternion2, handLocalPosition);
				return;
			}
			Vector3 normalized7 = this.input[i].elbowDir.normalized;
			Vector3 normalized8 = (vector + quaternion * GorillaIKMgr.IKJob.forearmLocalPos - vector).normalized;
			Vector3 normalized9 = Vector3.Cross(normalized4, normalized7).normalized;
			quaternion = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.Cross(normalized4, normalized8).normalized, normalized9, normalized4), normalized4) * quaternion;
			this.output[i] = new GorillaIKMgr.IKOutput(quaternion, quaternion2, handLocalPosition);
		}

		public NativeArray<GorillaIKMgr.IKConstantInput> constantInput;

		public NativeArray<GorillaIKMgr.IKInput> input;

		public NativeArray<GorillaIKMgr.IKOutput> output;

		private static readonly Vector3 upperArmLocalPos = new Vector3(0f, 0.1454885f, -0.02598158f);

		private static readonly Vector3 forearmLocalPos = new Vector3(0f, 0.4061671f, 0f);

		private static readonly Vector3 handLocalPos = new Vector3(0f, 0.3816895f, 0f);
	}

	[BurstCompile]
	private struct IKTransformJob : IJobParallelForTransform
	{
		public void Execute(int index, TransformAccess xform)
		{
			if (index % 8 <= 4)
			{
				xform.localRotation = this.transformRotations[index];
			}
			else
			{
				xform.rotation = this.transformRotations[index];
			}
			if (index % 8 >= 6)
			{
				xform.localPosition = this.transformPositions[index];
			}
		}

		public NativeArray<Quaternion> transformRotations;

		public NativeArray<Vector3> transformPositions;
	}
}
