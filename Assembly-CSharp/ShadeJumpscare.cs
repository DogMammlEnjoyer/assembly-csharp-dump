using System;
using GorillaExtensions;
using UnityEngine;

public class ShadeJumpscare : MonoBehaviour
{
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	private void OnEnable()
	{
		this.startTime = Time.time;
		this.startAngle = Random.value * 360f;
		this.audioSource.clip = this.audioClips.GetRandomItem<AudioClip>();
		this.audioSource.GTPlay();
	}

	private void Update()
	{
		float num = Time.time - this.startTime;
		float time = num / this.animationTime;
		this.shadeTransform.SetPositionAndRotation(base.transform.position + new Vector3(0f, this.shadeHeightFunction.Evaluate(time), 0f), Quaternion.Euler(0f, this.startAngle + num * this.shadeRotationSpeed, 0f));
		float num2 = this.shadeScaleFunction.Evaluate(time);
		this.shadeTransform.localScale = new Vector3(num2, num2 * this.shadeYScaleMultFunction.Evaluate(time), num2);
		this.audioSource.volume = this.soundVolumeFunction.Evaluate(time);
	}

	[SerializeField]
	private Transform shadeTransform;

	[SerializeField]
	private float animationTime;

	[SerializeField]
	private float shadeRotationSpeed = 1f;

	[SerializeField]
	private AnimationCurve shadeHeightFunction;

	[SerializeField]
	private AnimationCurve shadeScaleFunction;

	[SerializeField]
	private AnimationCurve shadeYScaleMultFunction;

	[SerializeField]
	private AnimationCurve soundVolumeFunction;

	[SerializeField]
	private AudioClip[] audioClips;

	private AudioSource audioSource;

	private float startTime;

	private float startAngle;
}
