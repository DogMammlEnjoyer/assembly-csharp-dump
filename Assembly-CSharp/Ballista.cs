using System;
using System.Collections;
using CjLib;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

public class Ballista : MonoBehaviourPun
{
	public void TriggerLoad()
	{
		this.animator.SetTrigger(this.loadTriggerHash);
	}

	public void TriggerFire()
	{
		this.animator.SetTrigger(this.fireTriggerHash);
	}

	private float LaunchSpeed
	{
		get
		{
			if (!this.useSpeedOptions)
			{
				return this.launchSpeed;
			}
			return this.speedOptions[this.currentSpeedIndex];
		}
	}

	private void Awake()
	{
		this.launchDirection = this.launchEnd.position - this.launchStart.position;
		this.launchRampDistance = this.launchDirection.magnitude;
		this.launchDirection /= this.launchRampDistance;
		this.collidingLayer = LayerMask.NameToLayer("Default");
		this.notCollidingLayer = LayerMask.NameToLayer("Prop");
		this.playerPullInRate = Mathf.Exp(this.playerMagnetismStrength);
		this.animator.SetFloat(this.pitchParamHash, this.pitch);
		this.appliedAnimatorPitch = this.pitch;
		this.RefreshButtonColors();
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		AnimatorStateInfo currentAnimatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.shortNameHash == this.idleStateHash)
		{
			if (this.prevStateHash == this.fireStateHash)
			{
				this.fireCompleteTime = Time.time;
			}
			if (Time.time - this.fireCompleteTime > this.reloadDelay)
			{
				this.animator.SetTrigger(this.loadTriggerHash);
				this.loadStartTime = Time.time;
			}
		}
		else if (currentAnimatorStateInfo.shortNameHash == this.loadStateHash)
		{
			if (Time.time - this.loadStartTime > this.loadTime)
			{
				if (this.playerInTrigger)
				{
					GTPlayer instance = GTPlayer.Instance;
					Vector3 playerBodyCenterPosition = this.GetPlayerBodyCenterPosition(instance);
					Vector3 b = Vector3.Dot(playerBodyCenterPosition - this.launchStart.position, this.launchDirection) * this.launchDirection + this.launchStart.position;
					Vector3 b2 = playerBodyCenterPosition - b;
					Vector3 a = Vector3.Lerp(Vector3.zero, b2, Mathf.Exp(-this.playerPullInRate * deltaTime));
					instance.transform.position = instance.transform.position + (a - b2);
					this.playerReadyToFire = (a.sqrMagnitude < this.playerReadyToFireDist * this.playerReadyToFireDist);
				}
				else
				{
					this.playerReadyToFire = false;
				}
				if (this.playerReadyToFire)
				{
					if (PhotonNetwork.InRoom)
					{
						base.photonView.RPC("FireBallistaRPC", RpcTarget.Others, Array.Empty<object>());
					}
					this.FireLocal();
				}
			}
		}
		else if (currentAnimatorStateInfo.shortNameHash == this.fireStateHash && !this.playerLaunched && (this.playerReadyToFire || this.playerInTrigger))
		{
			float num = Vector3.Dot(this.launchBone.position - this.launchStart.position, this.launchDirection) / this.launchRampDistance;
			GTPlayer instance2 = GTPlayer.Instance;
			Vector3 playerBodyCenterPosition2 = this.GetPlayerBodyCenterPosition(instance2);
			float b3 = Vector3.Dot(playerBodyCenterPosition2 - this.launchStart.position, this.launchDirection) / this.launchRampDistance;
			float num2 = 0.25f / this.launchRampDistance;
			float num3 = Mathf.Max(num + num2, b3);
			float d = num3 * this.launchRampDistance;
			Vector3 a2 = this.launchDirection * d + this.launchStart.position;
			instance2.transform.position + (a2 - playerBodyCenterPosition2);
			instance2.transform.position = instance2.transform.position + (a2 - playerBodyCenterPosition2);
			instance2.SetPlayerVelocity(Vector3.zero);
			if (num3 >= 1f)
			{
				this.playerLaunched = true;
				instance2.SetPlayerVelocity(this.LaunchSpeed * this.launchDirection);
				instance2.SetMaximumSlipThisFrame();
			}
		}
		this.prevStateHash = currentAnimatorStateInfo.shortNameHash;
	}

	private void FireLocal()
	{
		this.animator.SetTrigger(this.fireTriggerHash);
		this.playerLaunched = false;
		if (this.debugDrawTrajectoryOnLaunch)
		{
			this.DebugDrawTrajectory(8f);
		}
	}

	private Vector3 GetPlayerBodyCenterPosition(GTPlayer player)
	{
		return player.headCollider.transform.position + Quaternion.Euler(0f, player.headCollider.transform.rotation.eulerAngles.y, 0f) * new Vector3(0f, 0f, -0.15f) + Vector3.down * 0.4f;
	}

	private void OnTriggerEnter(Collider other)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && instance.bodyCollider == other)
		{
			this.playerInTrigger = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && instance.bodyCollider == other)
		{
			this.playerInTrigger = false;
		}
	}

	[PunRPC]
	public void FireBallistaRPC(PhotonMessageInfo info)
	{
		this.FireLocal();
	}

	private void UpdatePredictionLine()
	{
		float d = 0.033333335f;
		Vector3 vector = this.launchEnd.position;
		Vector3 a = (this.launchEnd.position - this.launchStart.position).normalized * this.LaunchSpeed;
		for (int i = 0; i < 240; i++)
		{
			this.predictionLinePoints[i] = vector;
			vector += a * d;
			a += Vector3.down * 9.8f * d;
		}
	}

	private IEnumerator DebugDrawTrajectory(float duration)
	{
		this.UpdatePredictionLine();
		float startTime = Time.time;
		while (Time.time < startTime + duration)
		{
			DebugUtil.DrawLine(this.launchStart.position, this.launchEnd.position, Color.yellow, true);
			DebugUtil.DrawLines(this.predictionLinePoints, Color.yellow, true);
			yield return null;
		}
		yield break;
	}

	private void OnDrawGizmosSelected()
	{
		if (this.launchStart != null && this.launchEnd != null)
		{
			this.UpdatePredictionLine();
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(this.launchStart.position, this.launchEnd.position);
			Gizmos.DrawLineList(this.predictionLinePoints);
		}
	}

	public void RefreshButtonColors()
	{
		this.speedZeroButton.isOn = (this.currentSpeedIndex == 0);
		this.speedZeroButton.UpdateColor();
		this.speedOneButton.isOn = (this.currentSpeedIndex == 1);
		this.speedOneButton.UpdateColor();
		this.speedTwoButton.isOn = (this.currentSpeedIndex == 2);
		this.speedTwoButton.UpdateColor();
		this.speedThreeButton.isOn = (this.currentSpeedIndex == 3);
		this.speedThreeButton.UpdateColor();
	}

	public void SetSpeedIndex(int index)
	{
		this.currentSpeedIndex = index;
		this.RefreshButtonColors();
	}

	public Animator animator;

	public Transform launchStart;

	public Transform launchEnd;

	public Transform launchBone;

	public float reloadDelay = 1f;

	public float loadTime = 1.933f;

	public float playerMagnetismStrength = 3f;

	public float launchSpeed = 20f;

	[Range(0f, 1f)]
	public float pitch;

	private bool useSpeedOptions;

	public float[] speedOptions = new float[]
	{
		10f,
		15f,
		20f,
		25f
	};

	public int currentSpeedIndex;

	public GorillaPressableButton speedZeroButton;

	public GorillaPressableButton speedOneButton;

	public GorillaPressableButton speedTwoButton;

	public GorillaPressableButton speedThreeButton;

	private bool debugDrawTrajectoryOnLaunch;

	private int loadTriggerHash = Animator.StringToHash("Load");

	private int fireTriggerHash = Animator.StringToHash("Fire");

	private int pitchParamHash = Animator.StringToHash("Pitch");

	private int idleStateHash = Animator.StringToHash("Idle");

	private int loadStateHash = Animator.StringToHash("Load");

	private int fireStateHash = Animator.StringToHash("Fire");

	private int prevStateHash = Animator.StringToHash("Idle");

	private float fireCompleteTime;

	private float loadStartTime;

	private bool playerInTrigger;

	private bool playerReadyToFire;

	private bool playerLaunched;

	private float playerReadyToFireDist = 0.1f;

	private Vector3 playerBodyOffsetFromHead = new Vector3(0f, -0.4f, -0.15f);

	private Vector3 launchDirection;

	private float launchRampDistance;

	private int collidingLayer;

	private int notCollidingLayer;

	private float playerPullInRate;

	private float appliedAnimatorPitch;

	private const int predictionLineSamples = 240;

	private Vector3[] predictionLinePoints = new Vector3[240];
}
