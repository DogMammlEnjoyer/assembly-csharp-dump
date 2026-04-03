using System;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using GorillaTag;
using GorillaTag.Rendering;
using UnityEngine;

public class InfectionLavaController : MonoBehaviour, ITickSystemPost
{
	public static IReadOnlyList<InfectionLavaController> ActiveControllers
	{
		get
		{
			return InfectionLavaController.activeControllers;
		}
	}

	public static InfectionLavaController GetControllerForZone(GTZone zone)
	{
		for (int i = 0; i < InfectionLavaController.activeControllers.Count; i++)
		{
			if (InfectionLavaController.activeControllers[i].zone == zone)
			{
				return InfectionLavaController.activeControllers[i];
			}
		}
		return null;
	}

	public GTZone Zone
	{
		get
		{
			return this.zone;
		}
	}

	private bool IsAuthority
	{
		get
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				return true;
			}
			int zoneAuthorityActorNumber = this.GetZoneAuthorityActorNumber();
			if (zoneAuthorityActorNumber == 2147483647)
			{
				return RoomSystem.AmITheHost;
			}
			return zoneAuthorityActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber;
		}
	}

	public bool LavaCurrentlyActivated
	{
		get
		{
			return this.reliableState.state > InfectionLavaController.RisingLavaState.Drained;
		}
	}

	public Plane LavaPlane
	{
		get
		{
			return new Plane(this.lavaSurfacePlaneTransform.up, this.lavaSurfacePlaneTransform.position);
		}
	}

	public Vector3 SurfaceCenter
	{
		get
		{
			return this.lavaSurfacePlaneTransform.position;
		}
	}

	private int PlayerCount
	{
		get
		{
			int result = 1;
			GorillaGameManager instance = GorillaGameManager.instance;
			if (instance != null && instance.currentNetPlayerArray != null)
			{
				result = instance.currentNetPlayerArray.Length;
			}
			return result;
		}
	}

	private bool InCompetitiveQueue
	{
		get
		{
			return NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString.Contains("COMPETITIVE");
		}
	}

	private void Awake()
	{
		this.lavaActivationMPB = new MaterialPropertyBlock();
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
		RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
		RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerJoinedRoom);
		RoomSystem.OnLavaSyncReceived = (Action<RoomSystem.LavaSyncEventData>)Delegate.Combine(RoomSystem.OnLavaSyncReceived, new Action<RoomSystem.LavaSyncEventData>(this.OnLavaSyncReceived));
	}

	protected void OnEnable()
	{
		InfectionLavaController.activeControllers.Add(this);
		this.VerifyReferences();
		for (int i = 0; i < this.volcanoEffects.Length; i++)
		{
			if (this.volcanoEffects[i] != null)
			{
				this.volcanoEffects[i].PreloadAssets();
			}
		}
		if (this.lavaVolume != null)
		{
			this.lavaVolume.ColliderEnteredWater += this.OnColliderEnteredLava;
		}
		if (this.lavaActivationProjectileHitNotifier != null)
		{
			this.lavaActivationProjectileHitNotifier.OnProjectileHit += this.OnActivationLavaProjectileHit;
		}
		if (this.localPlayerInZone && this.lavaZoneShaderSettings != null && this.reliableState.state != InfectionLavaController.RisingLavaState.Drained)
		{
			this.lavaZoneShaderSettings.BecomeActiveInstance(false);
		}
		TickSystem<object>.AddPostTickCallback(this);
	}

	protected void OnDisable()
	{
		InfectionLavaController.activeControllers.Remove(this);
		TickSystem<object>.RemovePostTickCallback(this);
		if (this.lavaVolume != null)
		{
			this.lavaVolume.ColliderEnteredWater -= this.OnColliderEnteredLava;
		}
		if (this.lavaActivationProjectileHitNotifier != null)
		{
			this.lavaActivationProjectileHitNotifier.OnProjectileHit -= this.OnActivationLavaProjectileHit;
		}
		this.ResetLavaState();
	}

	private void VerifyReferences()
	{
		this.IfNullThenLogAndDisableSelf(this.lavaMeshTransform, "lavaMeshTransform", -1);
		this.IfNullThenLogAndDisableSelf(this.lavaSurfacePlaneTransform, "lavaSurfacePlaneTransform", -1);
		this.IfNullThenLogAndDisableSelf(this.lavaVolume, "lavaVolume", -1);
		this.IfNullThenLogAndDisableSelf(this.lavaActivationRenderer, "lavaActivationRenderer", -1);
		this.IfNullThenLogAndDisableSelf(this.lavaActivationStartPos, "lavaActivationStartPos", -1);
		this.IfNullThenLogAndDisableSelf(this.lavaActivationEndPos, "lavaActivationEndPos", -1);
		this.IfNullThenLogAndDisableSelf(this.lavaActivationProjectileHitNotifier, "lavaActivationProjectileHitNotifier", -1);
		for (int i = 0; i < this.volcanoEffects.Length; i++)
		{
			this.IfNullThenLogAndDisableSelf(this.volcanoEffects[i], "volcanoEffects", i);
		}
	}

	private void IfNullThenLogAndDisableSelf(Object obj, string fieldName, int index = -1)
	{
		if (obj != null)
		{
			return;
		}
		fieldName = ((index != -1) ? string.Format("{0}[{1}]", fieldName, index) : fieldName);
		Debug.LogError("InfectionLavaController: Disabling self because reference `" + fieldName + "` is null.", this);
		base.enabled = false;
	}

	private void OnDestroy()
	{
		RoomSystem.LeftRoomEvent -= new Action(this.OnLeftRoom);
		RoomSystem.PlayerLeftEvent -= new Action<NetPlayer>(this.OnPlayerLeftRoom);
		RoomSystem.PlayerJoinedEvent -= new Action<NetPlayer>(this.OnPlayerJoinedRoom);
		RoomSystem.OnLavaSyncReceived = (Action<RoomSystem.LavaSyncEventData>)Delegate.Remove(RoomSystem.OnLavaSyncReceived, new Action<RoomSystem.LavaSyncEventData>(this.OnLavaSyncReceived));
	}

	private void ResetLavaState()
	{
		this.reliableState = default(InfectionLavaController.LavaSyncData);
		this.lavaProgressLinear = 0f;
		this.lavaProgressSmooth = 0f;
		this.localLagLavaProgressOffset = 0f;
		this.activationProgessSmooth = 0f;
		this.currentTime = 0.0;
		this.prevTime = 0.0;
		this.lastSyncSendTime = 0.0;
		this.residuePlaneY = this.GetMinLavaY();
		Shader.SetGlobalVector(InfectionLavaController._shaderProp_GlobalLavaResidueParams, Vector4.zero);
		for (int i = 0; i < this.lavaActivationVotePlayerIds.Length; i++)
		{
			this.lavaActivationVotePlayerIds[i] = 0;
		}
		this.lavaActivationVoteCount = 0;
		for (int j = 0; j < this.volcanoEffects.Length; j++)
		{
			VolcanoEffects volcanoEffects = this.volcanoEffects[j];
			if (volcanoEffects != null)
			{
				volcanoEffects.SetDrainedState();
			}
		}
		this.UpdateLava(0f);
		ZoneShaderSettings.ActivateDefaultSettings();
		if (this.localPlayerInZone && this.baseZoneShaderSettings != null)
		{
			this.baseZoneShaderSettings.BecomeActiveInstance(false);
		}
	}

	bool ITickSystemPost.PostTickRunning { get; set; }

	void ITickSystemPost.PostTick()
	{
		this.prevTime = this.currentTime;
		this.currentTime = (NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : Time.timeAsDouble);
		bool flag = this.localPlayerInZone;
		this.localPlayerInZone = this.CheckLocalPlayerInZone();
		if (this.IsAuthority)
		{
			InfectionLavaController.RisingLavaState state = this.reliableState.state;
			this.UpdateReliableState(this.currentTime, ref this.reliableState);
			bool flag2 = this.reliableState.state != state;
			bool flag3 = this.reliableState.state != InfectionLavaController.RisingLavaState.Drained && this.currentTime - this.lastSyncSendTime > 2.0;
			if (flag2 || flag3)
			{
				this.SendSyncEvent();
			}
		}
		else
		{
			this.AdvanceLavaPhaseByTime(this.currentTime, ref this.reliableState);
			this.DrainActivationProgressLocally();
		}
		this.UpdateLocalState(this.currentTime, this.reliableState);
		if (this.localPlayerInZone && !flag)
		{
			if (this.reliableState.state != InfectionLavaController.RisingLavaState.Drained)
			{
				if (this.lavaZoneShaderSettings != null)
				{
					this.lavaZoneShaderSettings.BecomeActiveInstance(false);
				}
			}
			else if (this.baseZoneShaderSettings != null)
			{
				this.baseZoneShaderSettings.BecomeActiveInstance(false);
			}
		}
		else if (!this.localPlayerInZone && flag)
		{
			ZoneShaderSettings.ActivateDefaultSettings();
			Shader.SetGlobalVector(InfectionLavaController._shaderProp_GlobalLavaResidueParams, Vector4.zero);
		}
		this.localLagLavaProgressOffset = Mathf.MoveTowards(this.localLagLavaProgressOffset, 0f, this.lagResolutionLavaProgressPerSecond * Time.deltaTime);
		this.UpdateLava(this.lavaProgressSmooth + this.localLagLavaProgressOffset);
		this.UpdateResidueState();
		this.UpdateVolcanoActivationLava(this.reliableState.activationProgress);
		this.CheckLocalPlayerAgainstLava(this.currentTime);
	}

	private void JumpToState(InfectionLavaController.RisingLavaState state)
	{
		this.reliableState.state = state;
		switch (state)
		{
		case InfectionLavaController.RisingLavaState.Drained:
			for (int i = 0; i < this.volcanoEffects.Length; i++)
			{
				VolcanoEffects volcanoEffects = this.volcanoEffects[i];
				if (volcanoEffects != null)
				{
					volcanoEffects.SetDrainedState();
				}
			}
			if (this.localPlayerInZone)
			{
				ZoneShaderSettings.ActivateDefaultSettings();
				if (this.baseZoneShaderSettings != null)
				{
					this.baseZoneShaderSettings.BecomeActiveInstance(false);
					return;
				}
			}
			break;
		case InfectionLavaController.RisingLavaState.Erupting:
			for (int j = 0; j < this.volcanoEffects.Length; j++)
			{
				VolcanoEffects volcanoEffects2 = this.volcanoEffects[j];
				if (volcanoEffects2 != null)
				{
					volcanoEffects2.SetEruptingState();
				}
			}
			if (this.localPlayerInZone && this.lavaZoneShaderSettings != null)
			{
				this.lavaZoneShaderSettings.BecomeActiveInstance(false);
				return;
			}
			break;
		case InfectionLavaController.RisingLavaState.Rising:
			if (this.localPlayerInZone && this.lavaZoneShaderSettings != null)
			{
				this.lavaZoneShaderSettings.BecomeActiveInstance(false);
			}
			for (int k = 0; k < this.volcanoEffects.Length; k++)
			{
				VolcanoEffects volcanoEffects3 = this.volcanoEffects[k];
				if (volcanoEffects3 != null)
				{
					volcanoEffects3.SetRisingState();
				}
			}
			return;
		case InfectionLavaController.RisingLavaState.Full:
			if (this.localPlayerInZone && this.lavaZoneShaderSettings != null)
			{
				this.lavaZoneShaderSettings.BecomeActiveInstance(false);
			}
			for (int l = 0; l < this.volcanoEffects.Length; l++)
			{
				VolcanoEffects volcanoEffects4 = this.volcanoEffects[l];
				if (volcanoEffects4 != null)
				{
					volcanoEffects4.SetFullState();
				}
			}
			break;
		case InfectionLavaController.RisingLavaState.Draining:
			for (int m = 0; m < this.volcanoEffects.Length; m++)
			{
				VolcanoEffects volcanoEffects5 = this.volcanoEffects[m];
				if (volcanoEffects5 != null)
				{
					volcanoEffects5.SetDrainingState();
				}
			}
			if (this.localPlayerInZone && this.lavaZoneShaderSettings != null)
			{
				this.lavaZoneShaderSettings.BecomeActiveInstance(false);
				return;
			}
			break;
		default:
			return;
		}
	}

	private void UpdateReliableState(double currentTime, ref InfectionLavaController.LavaSyncData syncData)
	{
		if (syncData.stateStartTime - currentTime > (double)this.latencyBuffer + 1.0)
		{
			syncData.stateStartTime = currentTime;
		}
		switch (syncData.state)
		{
		default:
			if (syncData.activationProgress > 1f)
			{
				float playerCount = (float)this.PlayerCount;
				float num = this.InCompetitiveQueue ? this.activationVotePercentageCompetitiveQueue : this.activationVotePercentageDefaultQueue;
				int num2 = Mathf.RoundToInt(playerCount * num);
				if (this.lavaActivationVoteCount >= num2)
				{
					for (int i = 0; i < this.lavaActivationVoteCount; i++)
					{
						this.lavaActivationVotePlayerIds[i] = 0;
					}
					this.lavaActivationVoteCount = 0;
					syncData.stateStartTime = currentTime + (double)this.latencyBuffer;
					syncData.activationProgress = 1f;
					this.JumpToState(InfectionLavaController.RisingLavaState.Erupting);
					return;
				}
			}
			else
			{
				float num3 = Mathf.Clamp((float)(currentTime - this.prevTime), 0f, 0.1f);
				float activationProgress = syncData.activationProgress;
				syncData.activationProgress = Mathf.MoveTowards(syncData.activationProgress, 0f, this.lavaActivationDrainRateVsPlayerCount.Evaluate((float)this.PlayerCount) * num3);
				if (activationProgress > 0f && syncData.activationProgress <= 1E-45f)
				{
					VolcanoEffects[] array = this.volcanoEffects;
					for (int j = 0; j < array.Length; j++)
					{
						array[j].OnVolcanoBellyEmpty();
					}
					return;
				}
			}
			break;
		case InfectionLavaController.RisingLavaState.Erupting:
			if (currentTime > syncData.stateStartTime + (double)this.eruptTime)
			{
				syncData.stateStartTime += (double)this.eruptTime;
				this.JumpToState(InfectionLavaController.RisingLavaState.Rising);
				return;
			}
			break;
		case InfectionLavaController.RisingLavaState.Rising:
			if (currentTime > syncData.stateStartTime + (double)this.riseTime)
			{
				syncData.stateStartTime += (double)this.riseTime;
				this.JumpToState(InfectionLavaController.RisingLavaState.Full);
				return;
			}
			break;
		case InfectionLavaController.RisingLavaState.Full:
			if (currentTime > syncData.stateStartTime + (double)this.fullTime)
			{
				syncData.stateStartTime += (double)this.fullTime;
				this.JumpToState(InfectionLavaController.RisingLavaState.Draining);
				return;
			}
			break;
		case InfectionLavaController.RisingLavaState.Draining:
		{
			float num4 = Mathf.Clamp((float)(currentTime - this.prevTime), 0f, 0.1f);
			syncData.activationProgress = Mathf.MoveTowards(syncData.activationProgress, 0f, this.lavaActivationDrainRateVsPlayerCount.Evaluate((float)this.PlayerCount) * num4);
			if (currentTime > syncData.stateStartTime + (double)this.drainTime)
			{
				syncData.stateStartTime += (double)this.drainTime;
				this.JumpToState(InfectionLavaController.RisingLavaState.Drained);
			}
			break;
		}
		}
	}

	private void AdvanceLavaPhaseByTime(double time, ref InfectionLavaController.LavaSyncData syncData)
	{
		if (syncData.stateStartTime - time > (double)this.latencyBuffer + 1.0)
		{
			syncData.stateStartTime = time;
		}
		switch (syncData.state)
		{
		case InfectionLavaController.RisingLavaState.Erupting:
			if (time > syncData.stateStartTime + (double)this.eruptTime)
			{
				syncData.stateStartTime += (double)this.eruptTime;
				this.JumpToState(InfectionLavaController.RisingLavaState.Rising);
				return;
			}
			break;
		case InfectionLavaController.RisingLavaState.Rising:
			if (time > syncData.stateStartTime + (double)this.riseTime)
			{
				syncData.stateStartTime += (double)this.riseTime;
				this.JumpToState(InfectionLavaController.RisingLavaState.Full);
				return;
			}
			break;
		case InfectionLavaController.RisingLavaState.Full:
			if (time > syncData.stateStartTime + (double)this.fullTime)
			{
				syncData.stateStartTime += (double)this.fullTime;
				this.JumpToState(InfectionLavaController.RisingLavaState.Draining);
				return;
			}
			break;
		case InfectionLavaController.RisingLavaState.Draining:
			if (time > syncData.stateStartTime + (double)this.drainTime)
			{
				syncData.stateStartTime += (double)this.drainTime;
				this.JumpToState(InfectionLavaController.RisingLavaState.Drained);
			}
			break;
		default:
			return;
		}
	}

	private void DrainActivationProgressLocally()
	{
		if (this.reliableState.activationProgress <= 0f)
		{
			return;
		}
		if (this.reliableState.state != InfectionLavaController.RisingLavaState.Drained && this.reliableState.state != InfectionLavaController.RisingLavaState.Draining)
		{
			return;
		}
		float num = Mathf.Clamp((float)(this.currentTime - this.prevTime), 0f, 0.1f);
		float activationProgress = this.reliableState.activationProgress;
		this.reliableState.activationProgress = Mathf.MoveTowards(this.reliableState.activationProgress, 0f, this.lavaActivationDrainRateVsPlayerCount.Evaluate((float)this.PlayerCount) * num);
		if (activationProgress > 0f && this.reliableState.activationProgress <= 1E-45f)
		{
			for (int i = 0; i < this.volcanoEffects.Length; i++)
			{
				VolcanoEffects volcanoEffects = this.volcanoEffects[i];
				if (volcanoEffects != null)
				{
					volcanoEffects.OnVolcanoBellyEmpty();
				}
			}
		}
	}

	private void UpdateLocalState(double currentTime, InfectionLavaController.LavaSyncData syncData)
	{
		switch (syncData.state)
		{
		default:
		{
			this.lavaProgressLinear = 0f;
			this.lavaProgressSmooth = 0f;
			float time = Mathf.Max(0f, (float)(currentTime - syncData.stateStartTime));
			foreach (VolcanoEffects volcanoEffects in this.volcanoEffects)
			{
				if (volcanoEffects != null)
				{
					volcanoEffects.UpdateDrainedState(time);
				}
			}
			return;
		}
		case InfectionLavaController.RisingLavaState.Erupting:
		{
			this.lavaProgressLinear = 0f;
			this.lavaProgressSmooth = 0f;
			float num = Mathf.Max(0f, (float)(currentTime - syncData.stateStartTime));
			float progress = Mathf.Clamp01(num / this.eruptTime);
			foreach (VolcanoEffects volcanoEffects2 in this.volcanoEffects)
			{
				if (volcanoEffects2 != null)
				{
					volcanoEffects2.UpdateEruptingState(num, this.eruptTime - num, progress);
				}
			}
			return;
		}
		case InfectionLavaController.RisingLavaState.Rising:
		{
			float num2 = Mathf.Max(0f, (float)(currentTime - syncData.stateStartTime));
			float value = num2 / this.riseTime;
			this.lavaProgressLinear = Mathf.Clamp01(value);
			this.lavaProgressSmooth = this.lavaProgressAnimationCurve.Evaluate(this.lavaProgressLinear);
			foreach (VolcanoEffects volcanoEffects3 in this.volcanoEffects)
			{
				if (volcanoEffects3 != null)
				{
					volcanoEffects3.UpdateRisingState(num2, this.riseTime - num2, this.lavaProgressLinear);
				}
			}
			return;
		}
		case InfectionLavaController.RisingLavaState.Full:
		{
			this.lavaProgressLinear = 1f;
			this.lavaProgressSmooth = 1f;
			float num3 = Mathf.Max(0f, (float)(currentTime - syncData.stateStartTime));
			float progress2 = Mathf.Clamp01(num3 / this.fullTime);
			foreach (VolcanoEffects volcanoEffects4 in this.volcanoEffects)
			{
				if (volcanoEffects4 != null)
				{
					volcanoEffects4.UpdateFullState(num3, this.fullTime - num3, progress2);
				}
			}
			return;
		}
		case InfectionLavaController.RisingLavaState.Draining:
		{
			float num4 = Mathf.Max(0f, (float)(currentTime - syncData.stateStartTime));
			float num5 = Mathf.Clamp01(num4 / this.drainTime);
			this.lavaProgressLinear = 1f - num5;
			this.lavaProgressSmooth = this.lavaProgressAnimationCurve.Evaluate(this.lavaProgressLinear);
			foreach (VolcanoEffects volcanoEffects5 in this.volcanoEffects)
			{
				if (volcanoEffects5 != null)
				{
					volcanoEffects5.UpdateDrainingState(num4, this.riseTime - num4, num5);
				}
			}
			return;
		}
		}
	}

	private void UpdateLava(float fillProgress)
	{
		this.lavaScale = Mathf.Lerp(this.lavaMeshMinScale, this.lavaMeshMaxScale, fillProgress);
		if (this.lavaMeshTransform != null)
		{
			this.lavaMeshTransform.localScale = new Vector3(this.lavaMeshTransform.localScale.x, this.lavaMeshTransform.localScale.y, this.lavaScale);
		}
	}

	private float GetMinLavaY()
	{
		if (this.lavaSurfacePlaneTransform == null || this.lavaMeshTransform == null)
		{
			return 0f;
		}
		float z = this.lavaMeshTransform.localScale.z;
		if (z < 0.001f)
		{
			return this.lavaSurfacePlaneTransform.position.y;
		}
		float y = this.lavaMeshTransform.position.y;
		float num = (this.lavaSurfacePlaneTransform.position.y - y) * (this.lavaMeshMinScale / z);
		return y + num;
	}

	private void UpdateResidueState()
	{
		float num = (this.lavaSurfacePlaneTransform != null) ? this.lavaSurfacePlaneTransform.position.y : 0f;
		switch (this.reliableState.state)
		{
		case InfectionLavaController.RisingLavaState.Drained:
		{
			float minLavaY = this.GetMinLavaY();
			this.residuePlaneY = Mathf.MoveTowards(this.residuePlaneY, minLavaY, this.residueDrainSpeed * Time.deltaTime);
			break;
		}
		case InfectionLavaController.RisingLavaState.Erupting:
		case InfectionLavaController.RisingLavaState.Rising:
		case InfectionLavaController.RisingLavaState.Full:
			this.residuePlaneY = num;
			break;
		case InfectionLavaController.RisingLavaState.Draining:
			this.residuePlaneY = Mathf.MoveTowards(this.residuePlaneY, num, this.residueDrainSpeed * Time.deltaTime);
			this.residuePlaneY = Mathf.Max(this.residuePlaneY, num);
			break;
		}
		if (this.localPlayerInZone)
		{
			float minLavaY2 = this.GetMinLavaY();
			float y = (this.reliableState.state != InfectionLavaController.RisingLavaState.Drained || this.residuePlaneY > minLavaY2 + 0.01f) ? this.residueIntensity : 0f;
			Shader.SetGlobalVector(InfectionLavaController._shaderProp_GlobalLavaResidueParams, new Vector4(this.residuePlaneY + this.residueOffset, y, this.residueUVScale, 0f));
		}
	}

	private void UpdateVolcanoActivationLava(float activationProgress)
	{
		if (this.lavaActivationRenderer == null)
		{
			return;
		}
		this.activationProgessSmooth = Mathf.MoveTowards(this.activationProgessSmooth, activationProgress, this.lavaActivationVisualMovementProgressPerSecond * Time.deltaTime);
		this.lavaActivationMPB.SetColor(ShaderProps._BaseColor, this.lavaActivationGradient.Evaluate(this.activationProgessSmooth));
		this.lavaActivationRenderer.SetPropertyBlock(this.lavaActivationMPB);
		this.lavaActivationRenderer.transform.position = Vector3.Lerp(this.lavaActivationStartPos.position, this.lavaActivationEndPos.position, this.activationProgessSmooth);
	}

	private void CheckLocalPlayerAgainstLava(double currentTime)
	{
		if (GTPlayer.Instance.InWater && GTPlayer.Instance.CurrentWaterVolume == this.lavaVolume)
		{
			this.LocalPlayerInLava(currentTime, false);
		}
	}

	private void OnColliderEnteredLava(WaterVolume volume, Collider collider)
	{
		if (collider == GTPlayer.Instance.bodyCollider)
		{
			this.LocalPlayerInLava(NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : Time.timeAsDouble, true);
		}
	}

	private void LocalPlayerInLava(double currentTime, bool enteredLavaThisFrame)
	{
		GorillaGameManager instance = GorillaGameManager.instance;
		if (instance != null && instance.CanAffectPlayer(NetworkSystem.Instance.LocalPlayer, enteredLavaThisFrame) && (currentTime - this.lastTagSelfRPCTime > 0.5 || enteredLavaThisFrame))
		{
			this.lastTagSelfRPCTime = currentTime;
			GameMode.ReportHit();
		}
	}

	public void OnActivationLavaProjectileHit(SlingshotProjectile projectile, Collision collision)
	{
		if (!projectile.gameObject.CompareTag("LavaRockProjectile"))
		{
			return;
		}
		if (this.reliableState.state != InfectionLavaController.RisingLavaState.Drained)
		{
			return;
		}
		if (this.IsAuthority)
		{
			this.AddLavaRock(projectile.projectileOwner.ActorNumber);
			return;
		}
		this.reliableState.activationProgress = this.reliableState.activationProgress + this.lavaActivationRockProgressVsPlayerCount.Evaluate((float)this.PlayerCount);
		for (int i = 0; i < this.volcanoEffects.Length; i++)
		{
			this.volcanoEffects[i].OnStoneAccepted(this.reliableState.activationProgress);
		}
	}

	private void AddLavaRock(int playerId)
	{
		float num = this.lavaActivationRockProgressVsPlayerCount.Evaluate((float)this.PlayerCount);
		this.reliableState.activationProgress = this.reliableState.activationProgress + num;
		this.AddVoteForVolcanoActivation(playerId);
		for (int i = 0; i < this.volcanoEffects.Length; i++)
		{
			this.volcanoEffects[i].OnStoneAccepted(this.reliableState.activationProgress);
		}
		this.SendSyncEvent();
	}

	private void AddVoteForVolcanoActivation(int playerId)
	{
		if (this.IsAuthority && this.lavaActivationVoteCount < 20)
		{
			bool flag = false;
			for (int i = 0; i < this.lavaActivationVoteCount; i++)
			{
				if (this.lavaActivationVotePlayerIds[i] == playerId)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				this.lavaActivationVotePlayerIds[this.lavaActivationVoteCount] = playerId;
				this.lavaActivationVoteCount++;
			}
		}
	}

	private void RemoveVoteForVolcanoActivation(int playerId)
	{
		if (this.IsAuthority)
		{
			for (int i = 0; i < this.lavaActivationVoteCount; i++)
			{
				if (this.lavaActivationVotePlayerIds[i] == playerId)
				{
					this.lavaActivationVotePlayerIds[i] = this.lavaActivationVotePlayerIds[this.lavaActivationVoteCount - 1];
					this.lavaActivationVoteCount--;
					return;
				}
			}
		}
	}

	private void SendSyncEvent()
	{
		this.lastSyncSendTime = this.currentTime;
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		RoomSystem.SendLavaSync((byte)this.zone, (byte)this.reliableState.state, this.reliableState.stateStartTime, this.reliableState.activationProgress, this.lavaActivationVoteCount, this.lavaActivationVotePlayerIds);
	}

	private void SendSyncEventToPlayer(NetPlayer target)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		RoomSystem.SendLavaSyncToPlayer((byte)this.zone, (byte)this.reliableState.state, this.reliableState.stateStartTime, this.reliableState.activationProgress, this.lavaActivationVoteCount, this.lavaActivationVotePlayerIds, target);
	}

	private unsafe void OnLavaSyncReceived(RoomSystem.LavaSyncEventData data)
	{
		if (data.zone != (byte)this.zone || this.IsAuthority)
		{
			return;
		}
		int zoneAuthorityActorNumber = this.GetZoneAuthorityActorNumber();
		if (zoneAuthorityActorNumber != 2147483647 && data.senderActorNumber != zoneAuthorityActorNumber)
		{
			return;
		}
		InfectionLavaController.RisingLavaState state = (InfectionLavaController.RisingLavaState)data.state;
		float num = this.lavaProgressSmooth;
		this.reliableState.stateStartTime = data.stateStartTime;
		this.reliableState.activationProgress = data.activationProgress;
		this.lavaActivationVoteCount = data.voteCount;
		for (int i = 0; i < 20; i++)
		{
			this.lavaActivationVotePlayerIds[i] = *(ref data.votes.FixedElementField + (IntPtr)i * 4);
		}
		if (state != this.reliableState.state)
		{
			this.JumpToState(state);
		}
		this.UpdateLocalState(NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : Time.timeAsDouble, this.reliableState);
		this.localLagLavaProgressOffset = num - this.lavaProgressSmooth;
	}

	private void OnPlayerJoinedRoom(NetPlayer player)
	{
		if (!this.IsAuthority)
		{
			return;
		}
		this.SendSyncEventToPlayer(player);
	}

	public void OnPlayerLeftRoom(NetPlayer otherNetPlayer)
	{
		this.RemoveVoteForVolcanoActivation(otherNetPlayer.ActorNumber);
		if (this.reliableState.state != InfectionLavaController.RisingLavaState.Drained)
		{
			if (this.localPlayerInZone && this.lavaZoneShaderSettings != null)
			{
				this.lavaZoneShaderSettings.BecomeActiveInstance(false);
			}
			if (this.IsAuthority)
			{
				this.SendSyncEvent();
			}
		}
	}

	private void OnLeftRoom()
	{
		if (this.reliableState.state != InfectionLavaController.RisingLavaState.Drained)
		{
			double num = this.currentTime - this.reliableState.stateStartTime;
			double timeAsDouble = Time.timeAsDouble;
			this.reliableState.stateStartTime = timeAsDouble - num;
			this.currentTime = timeAsDouble;
			this.prevTime = timeAsDouble;
			this.lastSyncSendTime = 0.0;
			for (int i = 0; i < this.lavaActivationVotePlayerIds.Length; i++)
			{
				this.lavaActivationVotePlayerIds[i] = 0;
			}
			this.lavaActivationVoteCount = 0;
			return;
		}
		ZoneShaderSettings.ActivateDefaultSettings();
		if (this.baseZoneShaderSettings != null)
		{
			this.baseZoneShaderSettings.BecomeActiveInstance(false);
		}
		this.ResetLavaState();
	}

	private int CountRigsInZone()
	{
		int num = 0;
		IReadOnlyList<VRRig> activeRigs = VRRigCache.ActiveRigs;
		for (int i = 0; i < activeRigs.Count; i++)
		{
			if (activeRigs[i] != null && activeRigs[i].zoneEntity.currentZone == this.zone)
			{
				num++;
			}
		}
		return num;
	}

	private bool CheckLocalPlayerInZone()
	{
		IReadOnlyList<VRRig> activeRigs = VRRigCache.ActiveRigs;
		for (int i = 0; i < activeRigs.Count; i++)
		{
			if (activeRigs[i] != null && activeRigs[i].isLocal)
			{
				return activeRigs[i].zoneEntity.currentZone == this.zone;
			}
		}
		return false;
	}

	private int GetZoneAuthorityActorNumber()
	{
		int num = int.MaxValue;
		IReadOnlyList<VRRig> activeRigs = VRRigCache.ActiveRigs;
		for (int i = 0; i < activeRigs.Count; i++)
		{
			VRRig vrrig = activeRigs[i];
			if (!(vrrig == null) && vrrig.zoneEntity.currentZone == this.zone)
			{
				int actorNumber;
				if (vrrig.isLocal)
				{
					actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
				}
				else
				{
					NetPlayer creator = vrrig.Creator;
					if (creator == null)
					{
						goto IL_6C;
					}
					actorNumber = creator.ActorNumber;
				}
				if (actorNumber < num)
				{
					num = actorNumber;
				}
			}
			IL_6C:;
		}
		return num;
	}

	[OnEnterPlay_SetNew]
	private static readonly List<InfectionLavaController> activeControllers = new List<InfectionLavaController>();

	[SerializeField]
	private GTZone zone;

	[SerializeField]
	private float lavaMeshMinScale = 3.17f;

	[Tooltip("If you throw rocks into the volcano quickly enough, then it will raise to this height.")]
	[SerializeField]
	private float lavaMeshMaxScale = 8.941086f;

	[SerializeField]
	private float eruptTime = 3f;

	[SerializeField]
	private float riseTime = 10f;

	[SerializeField]
	private float fullTime = 240f;

	[SerializeField]
	private float drainTime = 10f;

	[Tooltip("Delay added when starting the eruption cycle so the sync event has time to reach other clients before visuals begin.")]
	[SerializeField]
	private float latencyBuffer = 0.5f;

	[SerializeField]
	private float lagResolutionLavaProgressPerSecond = 0.2f;

	[SerializeField]
	private AnimationCurve lavaProgressAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[Header("Volcano Activation")]
	[SerializeField]
	[Range(0f, 1f)]
	private float activationVotePercentageDefaultQueue = 0.42f;

	[SerializeField]
	[Range(0f, 1f)]
	private float activationVotePercentageCompetitiveQueue = 0.6f;

	[SerializeField]
	private Gradient lavaActivationGradient;

	[SerializeField]
	private AnimationCurve lavaActivationRockProgressVsPlayerCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private AnimationCurve lavaActivationDrainRateVsPlayerCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private float lavaActivationVisualMovementProgressPerSecond = 1f;

	[SerializeField]
	private bool debugLavaActivationVotes;

	[Header("Scene References")]
	[SerializeField]
	private Transform lavaMeshTransform;

	[SerializeField]
	private Transform lavaSurfacePlaneTransform;

	[SerializeField]
	private WaterVolume lavaVolume;

	[SerializeField]
	private MeshRenderer lavaActivationRenderer;

	[SerializeField]
	private Transform lavaActivationStartPos;

	[SerializeField]
	private Transform lavaActivationEndPos;

	[SerializeField]
	private SlingshotProjectileHitNotifier lavaActivationProjectileHitNotifier;

	[SerializeField]
	private VolcanoEffects[] volcanoEffects;

	[SerializeField]
	private ZoneShaderSettings lavaZoneShaderSettings;

	[SerializeField]
	private ZoneShaderSettings baseZoneShaderSettings;

	[DebugReadout]
	private InfectionLavaController.LavaSyncData reliableState;

	private readonly int[] lavaActivationVotePlayerIds = new int[20];

	private int lavaActivationVoteCount;

	private float localLagLavaProgressOffset;

	[DebugReadout]
	private float lavaProgressLinear;

	[DebugReadout]
	private float lavaProgressSmooth;

	private double lastTagSelfRPCTime;

	private const string lavaRockProjectileTag = "LavaRockProjectile";

	private double currentTime;

	private double prevTime;

	private float activationProgessSmooth;

	private float lavaScale;

	private MaterialPropertyBlock lavaActivationMPB;

	private double lastSyncSendTime;

	private const double syncInterval = 2.0;

	private bool localPlayerInZone;

	private static readonly int _shaderProp_GlobalMainWaterSurfacePlane = Shader.PropertyToID("_GlobalMainWaterSurfacePlane");

	private static readonly int _shaderProp_GlobalLavaResidueParams = Shader.PropertyToID("_GlobalLavaResidueParams");

	[Header("Lava Residue")]
	[SerializeField]
	[Range(0f, 1f)]
	private float residueIntensity = 0.85f;

	[Tooltip("How fast the residue plane trails behind the lava when draining (world units/sec).")]
	[SerializeField]
	private float residueDrainSpeed = 1.5f;

	[Tooltip("UV scale for the residue texture in world space.")]
	[SerializeField]
	private float residueUVScale = 0.25f;

	[SerializeField]
	private float residueOffset = 2f;

	private float residuePlaneY;

	public enum RisingLavaState
	{
		Drained,
		Erupting,
		Rising,
		Full,
		Draining
	}

	private struct LavaSyncData
	{
		public InfectionLavaController.RisingLavaState state;

		public double stateStartTime;

		public float activationProgress;
	}
}
