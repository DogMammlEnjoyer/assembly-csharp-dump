using System;
using GorillaTag.Cosmetics;
using UnityEngine;

public class VoiceLoudnessReactor : MonoBehaviour
{
	private void Start()
	{
		for (int i = 0; i < this.transformPositionTargets.Length; i++)
		{
			this.transformPositionTargets[i].Initial = this.transformPositionTargets[i].transform.localPosition;
		}
		for (int j = 0; j < this.transformScaleTargets.Length; j++)
		{
			this.transformScaleTargets[j].Initial = this.transformScaleTargets[j].transform.localScale;
		}
		for (int k = 0; k < this.transformRotationTargets.Length; k++)
		{
			this.transformRotationTargets[k].Initial = this.transformRotationTargets[k].transform.localRotation;
		}
		for (int l = 0; l < this.particleTargets.Length; l++)
		{
			this.particleTargets[l].Main = this.particleTargets[l].particleSystem.main;
			this.particleTargets[l].InitialSpeed = this.particleTargets[l].Main.startSpeedMultiplier;
			this.particleTargets[l].InitialSize = this.particleTargets[l].Main.startSizeMultiplier;
			this.particleTargets[l].Emission = this.particleTargets[l].particleSystem.emission;
			this.particleTargets[l].InitialRate = this.particleTargets[l].Emission.rateOverTimeMultiplier;
			this.particleTargets[l].Main.startSpeedMultiplier = 0f;
			this.particleTargets[l].Main.startSizeMultiplier = 0f;
			this.particleTargets[l].Emission.rateOverTimeMultiplier = 0f;
		}
		for (int m = 0; m < this.gameObjectEnableTargets.Length; m++)
		{
			this.gameObjectEnableTargets[m].GameObject.SetActive(!this.gameObjectEnableTargets[m].TurnOnAtThreshhold);
		}
		for (int n = 0; n < this.rendererColorTargets.Length; n++)
		{
			this.rendererColorTargets[n].Inititialize();
		}
		this.hasContinuousProperties = (this.continuousProperties != null && this.continuousProperties.Count > 0);
	}

	private void OnEnable()
	{
		if (this.loudness != null)
		{
			return;
		}
		this.loudness = base.GetComponentInParent<GorillaSpeakerLoudness>(true);
		if (this.loudness == null)
		{
			GorillaTagger componentInParent = base.GetComponentInParent<GorillaTagger>();
			if (componentInParent != null)
			{
				this.loudness = componentInParent.offlineVRRig.GetComponent<GorillaSpeakerLoudness>();
			}
		}
		if (this.loudness != null)
		{
			this.frameLoudness = this.loudness.Loudness;
			this.frameSmoothedLoudness = this.loudness.SmoothedLoudness;
		}
	}

	private void Update()
	{
		if (this.loudness == null)
		{
			return;
		}
		float num = this.loudness.Loudness;
		float num2 = this.frameLoudness;
		float num3 = this.loudness.Loudness;
		float num4 = this.frameLoudness;
		if (this.attack > 0f && this.loudness.Loudness > this.frameLoudness)
		{
			this.frameLoudness = Mathf.MoveTowards(this.frameLoudness, this.loudness.Loudness, Time.deltaTime / this.attack);
		}
		else if (this.decay > 0f && this.loudness.Loudness < this.frameLoudness)
		{
			this.frameLoudness = Mathf.MoveTowards(this.frameLoudness, this.loudness.Loudness, Time.deltaTime / this.decay);
		}
		else
		{
			this.frameLoudness = this.loudness.Loudness;
		}
		if (this.attack > 0f && this.loudness.SmoothedLoudness > this.frameSmoothedLoudness)
		{
			this.frameSmoothedLoudness = Mathf.MoveTowards(this.frameLoudness, this.loudness.SmoothedLoudness, Time.deltaTime * this.attack);
		}
		else if (this.decay > 0f && this.loudness.SmoothedLoudness < this.frameSmoothedLoudness)
		{
			this.frameSmoothedLoudness = Mathf.MoveTowards(this.frameLoudness, this.loudness.SmoothedLoudness, Time.deltaTime * this.decay);
		}
		else
		{
			this.frameSmoothedLoudness = this.loudness.SmoothedLoudness;
		}
		for (int i = 0; i < this.blendShapeTargets.Length; i++)
		{
			float t = this.blendShapeTargets[i].UseSmoothedLoudness ? this.frameSmoothedLoudness : this.frameLoudness;
			this.blendShapeTargets[i].SkinnedMeshRenderer.SetBlendShapeWeight(this.blendShapeTargets[i].BlendShapeIndex, Mathf.Lerp(this.blendShapeTargets[i].minValue, this.blendShapeTargets[i].maxValue, t));
		}
		for (int j = 0; j < this.transformPositionTargets.Length; j++)
		{
			float t2 = (this.transformPositionTargets[j].UseSmoothedLoudness ? this.frameSmoothedLoudness : this.frameLoudness) * this.transformPositionTargets[j].Scale;
			this.transformPositionTargets[j].transform.localPosition = Vector3.Lerp(this.transformPositionTargets[j].Initial, this.transformPositionTargets[j].Max, t2);
		}
		for (int k = 0; k < this.transformScaleTargets.Length; k++)
		{
			float t3 = (this.transformScaleTargets[k].UseSmoothedLoudness ? this.frameSmoothedLoudness : this.frameLoudness) * this.transformScaleTargets[k].Scale;
			this.transformScaleTargets[k].transform.localScale = Vector3.Lerp(this.transformScaleTargets[k].Initial, this.transformScaleTargets[k].Max, t3);
		}
		for (int l = 0; l < this.transformRotationTargets.Length; l++)
		{
			float t4 = (this.transformRotationTargets[l].UseSmoothedLoudness ? this.frameSmoothedLoudness : this.frameLoudness) * this.transformRotationTargets[l].Scale;
			this.transformRotationTargets[l].transform.localRotation = Quaternion.Slerp(this.transformRotationTargets[l].Initial, this.transformRotationTargets[l].Max, t4);
		}
		for (int m = 0; m < this.particleTargets.Length; m++)
		{
			float time = (this.particleTargets[m].UseSmoothedLoudness ? this.frameSmoothedLoudness : this.frameLoudness) * this.particleTargets[m].Scale;
			this.particleTargets[m].Main.startSpeedMultiplier = this.particleTargets[m].InitialSpeed * this.particleTargets[m].speed.Evaluate(time);
			this.particleTargets[m].Main.startSizeMultiplier = this.particleTargets[m].InitialSize * this.particleTargets[m].size.Evaluate(time);
			this.particleTargets[m].Emission.rateOverTimeMultiplier = this.particleTargets[m].InitialRate * this.particleTargets[m].rate.Evaluate(time);
		}
		for (int n = 0; n < this.gameObjectEnableTargets.Length; n++)
		{
			bool flag = (this.gameObjectEnableTargets[n].UseSmoothedLoudness ? this.frameSmoothedLoudness : (this.frameLoudness * this.gameObjectEnableTargets[n].Scale)) >= this.gameObjectEnableTargets[n].Threshold;
			if (!this.gameObjectEnableTargets[n].TurnOnAtThreshhold)
			{
				flag = !flag;
			}
			if (this.gameObjectEnableTargets[n].GameObject.activeInHierarchy != flag)
			{
				this.gameObjectEnableTargets[n].GameObject.SetActive(flag);
			}
		}
		for (int num5 = 0; num5 < this.rendererColorTargets.Length; num5++)
		{
			VoiceLoudnessReactorRendererColorTarget voiceLoudnessReactorRendererColorTarget = this.rendererColorTargets[num5];
			float level = voiceLoudnessReactorRendererColorTarget.useSmoothedLoudness ? this.frameSmoothedLoudness : (this.frameLoudness * voiceLoudnessReactorRendererColorTarget.scale);
			voiceLoudnessReactorRendererColorTarget.UpdateMaterialColor(level);
		}
		for (int num6 = 0; num6 < this.animatorTargets.Length; num6++)
		{
			VoiceLoudnessReactorAnimatorTarget voiceLoudnessReactorAnimatorTarget = this.animatorTargets[num6];
			float num7 = voiceLoudnessReactorAnimatorTarget.useSmoothedLoudness ? this.frameSmoothedLoudness : this.frameLoudness;
			if (voiceLoudnessReactorAnimatorTarget.animatorSpeedToLoudness < 0f)
			{
				voiceLoudnessReactorAnimatorTarget.animator.speed = Mathf.Max(0f, (1f - num7) * -voiceLoudnessReactorAnimatorTarget.animatorSpeedToLoudness);
			}
			else
			{
				voiceLoudnessReactorAnimatorTarget.animator.speed = Mathf.Max(0f, num7 * voiceLoudnessReactorAnimatorTarget.animatorSpeedToLoudness);
			}
		}
		if (this.hasContinuousProperties)
		{
			float f = this.smoothLoudnessForContinuousProperties ? this.frameSmoothedLoudness : this.frameLoudness;
			this.continuousProperties.ApplyAll(f);
		}
	}

	private GorillaSpeakerLoudness loudness;

	[SerializeField]
	private VoiceLoudnessReactorBlendShapeTarget[] blendShapeTargets = new VoiceLoudnessReactorBlendShapeTarget[0];

	[SerializeField]
	private VoiceLoudnessReactorTransformTarget[] transformPositionTargets = new VoiceLoudnessReactorTransformTarget[0];

	[SerializeField]
	private VoiceLoudnessReactorTransformRotationTarget[] transformRotationTargets = new VoiceLoudnessReactorTransformRotationTarget[0];

	[SerializeField]
	private VoiceLoudnessReactorTransformTarget[] transformScaleTargets = new VoiceLoudnessReactorTransformTarget[0];

	[SerializeField]
	private VoiceLoudnessReactorParticleSystemTarget[] particleTargets = new VoiceLoudnessReactorParticleSystemTarget[0];

	[SerializeField]
	private VoiceLoudnessReactorGameObjectEnableTarget[] gameObjectEnableTargets = new VoiceLoudnessReactorGameObjectEnableTarget[0];

	[SerializeField]
	private VoiceLoudnessReactorRendererColorTarget[] rendererColorTargets = new VoiceLoudnessReactorRendererColorTarget[0];

	[SerializeField]
	private VoiceLoudnessReactorAnimatorTarget[] animatorTargets = new VoiceLoudnessReactorAnimatorTarget[0];

	[SerializeField]
	private bool smoothLoudnessForContinuousProperties = true;

	[SerializeField]
	private ContinuousPropertyArray continuousProperties;

	private bool hasContinuousProperties;

	private float frameLoudness;

	private float frameSmoothedLoudness;

	[Tooltip("If > 0, The rate that the volume gets louder = deltaTime/attack")]
	[SerializeField]
	private float attack;

	[Tooltip("If > 0, The rate that the volume gets quieter = deltaTime/decay")]
	[SerializeField]
	private float decay;
}
