using System;
using System.Collections;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

public class GhostPal : MonoBehaviour
{
	private void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this.animator = base.GetComponentInChildren<Animator>();
		this.trailingPosition = base.transform.position;
		this.triggerAudioClipIndex = this.triggerAudioClips.GetRandomIndex<AudioClip>();
	}

	private IEnumerator BounceOnTrigger()
	{
		float startTime = Time.time;
		while (Time.time - startTime < this.bounceOnTrigger[this.bounceOnTrigger.length - 1].time)
		{
			this.bounceHeight = this.bounceOnTrigger.Evaluate(Time.time - startTime);
			yield return null;
		}
		this.bounceHeight = 0f;
		yield break;
	}

	private void LateUpdate()
	{
		Vector3 position = this.rig.bodyTransform.position;
		Vector3 vector = base.transform.parent.position - position;
		float num = vector.y * 0.5f + this.orbitHeight;
		vector.y = 0f;
		float d = vector.magnitude + this.minDistanceFromPlayer;
		vector = vector.normalized * d;
		vector.y = num + this.bounceHeight;
		double num2 = (double)this.orbitSpeed * (PhotonNetwork.InRoom ? ((PhotonNetwork.Time - (double)this.rig.OwningNetPlayer.UserId.GetStaticHash()) * (double)((this.rig.OwningNetPlayer.ActorNumber % 2 == 0) ? 1 : -1)) : Time.timeAsDouble);
		Vector3 b = new Vector3(this.orbitRadius * (float)Math.Cos(num2), 0f, this.orbitRadius * (float)Math.Sin(num2));
		Vector3 vector2 = position + vector + b;
		Vector3 a = vector2 - this.rig.head.rigTarget.position;
		if (Vector3.Dot(this.rig.head.rigTarget.forward, a.normalized) >= this.lookAtDotProductMin)
		{
			this.lookAtTime = Mathf.Min(this.lookAtTime + Time.deltaTime, Mathf.Max(this.rotateTowardsPlayerFromLookTime[this.rotateTowardsPlayerFromLookTime.length - 1].time, this.minLookTimeToTrigger));
			if (this.lookAtTime >= this.minLookTimeToTrigger && !this.hasTriggered && this.bounceHeight == 0f)
			{
				this.animator.SetTrigger(this.friendlyAnimID);
				this.bounceCoroutine = base.StartCoroutine(this.BounceOnTrigger());
				this.triggerAudioSource.pitch = Random.Range(this.triggerAudioPitchMinMax.x, this.triggerAudioPitchMinMax.y);
				this.triggerAudioSource.clip = this.triggerAudioClips[this.triggerAudioClipIndex];
				this.triggerAudioSource.GTPlay();
				this.triggerAudioClipIndex = (this.triggerAudioClipIndex + Random.Range(0, this.triggerAudioClips.Length - 1)) % this.triggerAudioClips.Length;
				this.hasTriggered = true;
			}
		}
		else
		{
			this.lookAtTime = Mathf.Max(this.lookAtTime - Time.deltaTime, 0f);
			if (this.lookAtTime < this.minLookTimeToTrigger && this.hasTriggered && this.bounceHeight == 0f)
			{
				this.animator.SetTrigger(this.neutralAnimID);
				this.hasTriggered = false;
			}
		}
		if ((vector2 - this.trailingPosition).sqrMagnitude > 0.1f)
		{
			float t = 1f - Mathf.Exp(-this.faceMovementDirectionStrength * Time.deltaTime);
			this.trailingPosition = Vector3.Lerp(this.trailingPosition, vector2, t);
		}
		Quaternion rotation = Quaternion.Slerp(Quaternion.LookRotation(vector2 - this.trailingPosition, Vector3.up), Quaternion.LookRotation(-a, Vector3.up), this.rotateTowardsPlayerFromLookTime.Evaluate(this.lookAtTime));
		base.transform.SetPositionAndRotation(vector2, rotation);
	}

	[SerializeField]
	private float minDistanceFromPlayer = 1f;

	[SerializeField]
	private float orbitRadius = 1f;

	[SerializeField]
	private float orbitHeight = 1f;

	[SerializeField]
	private float orbitSpeed = 0.1f;

	[SerializeField]
	private float faceMovementDirectionStrength = 1f;

	[Space]
	[SerializeField]
	private float lookAtDotProductMin = 0.95f;

	[SerializeField]
	private AnimationCurve rotateTowardsPlayerFromLookTime;

	[SerializeField]
	private float minLookTimeToTrigger = 2f;

	[SerializeField]
	private AnimationCurve bounceOnTrigger;

	[SerializeField]
	private AudioSource triggerAudioSource;

	[SerializeField]
	private Vector2 triggerAudioPitchMinMax = new Vector2(0.9f, 1.1f);

	[SerializeField]
	private AudioClip[] triggerAudioClips;

	private VRRig rig;

	private Animator animator;

	private float lookAtTime;

	private bool hasTriggered;

	private Coroutine bounceCoroutine;

	private float bounceHeight;

	private Vector3 trailingPosition;

	private int triggerAudioClipIndex;

	private int neutralAnimID = Animator.StringToHash("Neutral");

	private int friendlyAnimID = Animator.StringToHash("Friendly");
}
