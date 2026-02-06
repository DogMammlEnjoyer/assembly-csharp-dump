using System;
using GorillaLocomotion;
using UnityEngine;

public class GorillaVelocityEstimator : MonoBehaviour
{
	public Vector3 linearVelocity { get; private set; }

	public Vector3 angularVelocity { get; private set; }

	public Vector3 handPos { get; private set; }

	private void Awake()
	{
		this.history = new GorillaVelocityEstimator.VelocityHistorySample[this.numFrames];
	}

	private void OnEnable()
	{
		this.currentFrame = 0;
		for (int i = 0; i < this.history.Length; i++)
		{
			this.history[i] = default(GorillaVelocityEstimator.VelocityHistorySample);
		}
		this.lastPos = base.transform.position;
		this.lastRotation = base.transform.rotation;
		GorillaVelocityEstimatorManager.Register(this);
	}

	private void OnDisable()
	{
		GorillaVelocityEstimatorManager.Unregister(this);
	}

	private void OnDestroy()
	{
		GorillaVelocityEstimatorManager.Unregister(this);
	}

	public void TriggeredLateUpdate()
	{
		Vector3 vector;
		Quaternion lhs;
		base.transform.GetPositionAndRotation(out vector, out lhs);
		Vector3 b = Vector3.zero;
		if (!this.useGlobalSpace)
		{
			b = GTPlayer.Instance.InstantaneousVelocity;
		}
		Vector3 vector2 = (vector - this.lastPos) / Time.deltaTime - b;
		Vector3 vector3 = (lhs * Quaternion.Inverse(this.lastRotation)).eulerAngles;
		if (vector3.x > 180f)
		{
			vector3.x -= 360f;
		}
		if (vector3.y > 180f)
		{
			vector3.y -= 360f;
		}
		if (vector3.z > 180f)
		{
			vector3.z -= 360f;
		}
		vector3 *= 0.017453292f / Time.fixedDeltaTime;
		this.linearVelocity += (vector2 - this.history[this.currentFrame].linear) / (float)this.numFrames;
		this.angularVelocity += (vector3 - this.history[this.currentFrame].angular) / (float)this.numFrames;
		this.history[this.currentFrame] = new GorillaVelocityEstimator.VelocityHistorySample
		{
			linear = vector2,
			angular = vector3
		};
		this.handPos = vector;
		this.currentFrame = (this.currentFrame + 1) % this.numFrames;
		this.lastPos = vector;
		this.lastRotation = lhs;
	}

	[Min(1f)]
	[SerializeField]
	private int numFrames = 8;

	private GorillaVelocityEstimator.VelocityHistorySample[] history;

	private int currentFrame;

	private Vector3 lastPos;

	private Quaternion lastRotation;

	private Vector3 lastRotationVec;

	public bool useGlobalSpace;

	public struct VelocityHistorySample
	{
		public Vector3 linear;

		public Vector3 angular;
	}
}
