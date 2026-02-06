using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps;
using UnityEngine;

public class SIGadgetTentacleArm : SIGadget, ICallBack, IEnergyGadget
{
	public bool isAnchored { get; private set; }

	public bool isHoldingHand { get; private set; }

	private void Awake()
	{
		this._fps_holding_base = this.FuelPerSecond_Holding;
		this._fps_recharging_base = this.FuelPerSecond_Recharging;
		this._grabCost_base = this.FuelCost_Grab;
		this._jumpCost_base = this.FuelCost_JumpSpeed;
		this._jumpSpeed_base = this.MaxTentacleJumpSpeed;
		this._grabAngle_base = this.MaxGrabAngle;
		this._wall_angle_dot = Mathf.Cos(0.017453292f * this.WallAngle);
		this.tentacleMat = new Material(this.tentacleRenderer.sharedMaterial);
		this.tentacleRenderer.sharedMaterial = this.tentacleMat;
		if (this.tentacleRenderer2 != null)
		{
			this.hasTentacle2 = true;
			this.tentacleMat2 = new Material(this.tentacleRenderer2.sharedMaterial);
			this.tentacleRenderer2.sharedMaterial = this.tentacleMat2;
		}
		this._gaugeMatPropBlock = new MaterialPropertyBlock();
		if (this.m_gaugeMatSlots == null)
		{
			this.m_gaugeMatSlots = Array.Empty<GTRendererMatSlot>();
		}
		int num = 0;
		for (int i = 0; i < this.m_gaugeMatSlots.Length; i++)
		{
			if (this.m_gaugeMatSlots[i].TryInitialize())
			{
				this.m_gaugeMatSlots[num] = this.m_gaugeMatSlots[i];
				num++;
			}
		}
		if (num != this.m_gaugeMatSlots.Length)
		{
			Array.Resize<GTRendererMatSlot>(ref this.m_gaugeMatSlots, num);
		}
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this.OnSnapped));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this.OnReleased));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.OnUnsnapped));
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
		this.heldPlayerCallback = new SIGadgetTentacleArm.HeldPlayerCallback(this);
	}

	private void Start()
	{
		this.clawVisualPos = this.claw.transform.position;
		this.clawVisualRot = this.claw.transform.rotation;
		this.clawReleasedVisual.SetActive(false);
		this.CallBack();
	}

	private void OnDestroy()
	{
		if (this.hasRigCallback)
		{
			this.hasRigCallback = false;
			this.rigForCallback.RemoveLateUpdateCallback(this);
		}
		if (this.hasGravityOverride)
		{
			GTPlayer.Instance.UnsetGravityOverride(this);
			this.hasGravityOverride = false;
		}
		this.heldPlayerCallback.Unregister();
	}

	private void OnGrabbed()
	{
		this.isLeftHanded = (this.gameEntity.heldByHandIndex == 0);
		GamePlayer gamePlayer;
		if (GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			this.hasRigCallback = true;
			this.rigForCallback = gamePlayer.rig;
			this.rigForCallback.AddLateUpdateCallback(this);
		}
	}

	private void OnSnapped()
	{
		this.isLeftHanded = (this.gameEntity.snappedJoint == SnapJointType.HandL);
		GamePlayer gamePlayer;
		if (GamePlayer.TryGetGamePlayer(this.gameEntity.snappedByActorNumber, out gamePlayer))
		{
			this.hasRigCallback = true;
			this.rigForCallback = gamePlayer.rig;
			this.rigForCallback.AddLateUpdateCallback(this);
		}
	}

	private void OnReleased()
	{
		this.ClearClawAnchor();
		if (this.hasRigCallback)
		{
			this.hasRigCallback = false;
			this.rigForCallback.RemoveLateUpdateCallback(this);
		}
	}

	private void OnUnsnapped()
	{
		if (this.hasRigCallback)
		{
			this.hasRigCallback = false;
			this.rigForCallback.RemoveLateUpdateCallback(this);
		}
	}

	private bool CheckInput()
	{
		return this.buttonActivatable.CheckInput(true, true, 0.25f, true, true);
	}

	private Vector3 GetIdealClawPosition(VRRig rig)
	{
		Vector3 position = rig.bodyTransform.position;
		position.y += 0.05f;
		Vector3 position2 = base.transform.position;
		Vector3 a = position2 - position;
		return position2 + a * this.LengthFactor + base.transform.forward * this.tentacleForwardAdjustment;
	}

	protected override void OnUpdateAuthority(float dt)
	{
		bool flag = this.CheckInput();
		if (this.isGripBroken)
		{
			if (flag)
			{
				flag = false;
			}
			else
			{
				this.isGripBroken = false;
			}
		}
		Vector3 position = base.transform.position;
		Vector3 idealClawPosition = this.GetIdealClawPosition(VRRig.LocalRig);
		Quaternion rotation = base.transform.rotation;
		float num = 0.15f;
		bool flag2 = this.isLowFuel;
		if ((this.knownSafePosition - idealClawPosition).IsLongerThan(1f))
		{
			Vector3 position2 = GTPlayer.Instance.headCollider.transform.position;
			Ray ray = new Ray(position2, idealClawPosition - position2);
			RaycastHit raycastHit;
			if (Physics.SphereCast(ray, num, out raycastHit, (idealClawPosition - position2).magnitude, this.worldCollisionLayers))
			{
				this.knownSafePosition = ray.origin + ray.direction * (raycastHit.distance - num * 2.01f);
			}
			else
			{
				this.knownSafePosition = position;
			}
		}
		if ((this.isAnchored || this.isHoldingHand) && !flag)
		{
			GorillaTagger.Instance.StartVibration(this.isLeftHanded, this.hapticStrengthOnRelease, this.hapticDurationOnRelease);
			this.ClearClawAnchor();
		}
		else
		{
			if (this.isAnchored)
			{
				this.currentFuel = Mathf.Max(0f, this.currentFuel - dt * this._current_grab_fps);
				this.isLowFuel = (this.currentFuel < this._lowFuelThreshold);
				if (this.isLowFuel && !flag2)
				{
					this.lowFuelSound.Play();
				}
				this.UpdateFuelGauge();
				if (this.currentFuel == 0f)
				{
					this.isGripBroken = true;
					flag = false;
					this.ClearClawAnchor();
					this.detachFailSound.Play();
				}
				else
				{
					Vector3 position3 = GTPlayer.Instance.transform.position;
					this.clawHoldAdjustment -= position3 - this.lastRequestedPlayerPosition;
					Vector3 vector = this.clawAnchorPosition - (idealClawPosition + this.clawHoldAdjustment);
					ref vector.ClampThisMagnitudeSafe(this.MaxTentacleJumpSpeed * dt);
					GTPlayer.Instance.RequestTentacleMove(this.isLeftHanded, vector);
					GTPlayer.Instance.TentacleActiveAtFrame = Time.frameCount + 1;
					this.lastRequestedPlayerPosition = position3 + vector;
					if ((this.clawAnchorPosition - base.transform.position).IsLongerThan(this.maxTentacleLength))
					{
						this.isGripBroken = true;
						this.ClearClawAnchor();
						this.detachFailSound.Play();
					}
					else
					{
						this.clawVisualPos = this.clawAnchorPosition;
						this.clawVisualRot = this.clawAnchorRotation;
					}
				}
				this.wasGrabPressed = flag;
				return;
			}
			if (this.isHoldingHand)
			{
				TakeMyHand_HandLink takeMyHand_HandLink = this.isLeftHanded ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink;
				if (takeMyHand_HandLink.IsLinkActive())
				{
					Vector3 position4 = (this.isLeftHanded ? VRRig.LocalRig.leftHand : VRRig.LocalRig.rightHand).overrideTarget.position;
					takeMyHand_HandLink.TentacleOffset = idealClawPosition - position4;
				}
				else
				{
					this.isGripBroken = true;
					this.ClearClawAnchor();
					this.detachFailSound.Play();
				}
				this.wasGrabPressed = flag;
				return;
			}
		}
		RaycastHit raycastHit2;
		bool flag3 = Physics.SphereCast(new Ray(this.knownSafePosition, idealClawPosition - this.knownSafePosition), num, out raycastHit2, (idealClawPosition - this.knownSafePosition).magnitude, this.worldCollisionLayers);
		Vector3 vector2 = idealClawPosition;
		Quaternion quaternion = rotation;
		bool flag4 = false;
		bool flag5 = this.currentFuel < this.FuelCost_Grab + this.FuelPerSecond_Holding;
		if (flag5 && flag)
		{
			this.isGripBroken = true;
			flag = false;
			this.attachFailSound.Play();
		}
		if (flag3)
		{
			if (!flag5)
			{
				float num2 = Vector3.Dot(raycastHit2.normal, Vector3.up);
				if (num2 >= this._min_grab_dot)
				{
					this._current_grab_fps = ((num2 >= this._wall_angle_dot) ? this.FuelPerSecond_Holding : (this.FuelPerSecond_Holding * this.FuelCost_Wall_Multiplier));
					flag4 = true;
					if (GTPlayer.Instance.GetSlidePercentage(raycastHit2) > 0.5f)
					{
						if (!this.canHoldSlipperyWalls)
						{
							flag4 = false;
							if (flag && !this.hasFailedToGrab)
							{
								this.attachFailSound.Play();
								this.hasFailedToGrab = true;
							}
						}
						else
						{
							this._current_grab_fps *= this.FuelCost_Slippery_Multiplier;
						}
					}
				}
				else if (flag && !this.hasFailedToGrab)
				{
					this.attachFailSound.Play();
					this.hasFailedToGrab = true;
				}
			}
			this.knownSafePosition += (idealClawPosition - this.knownSafePosition).normalized * (raycastHit2.distance - num * 2.01f);
			vector2 = raycastHit2.point + raycastHit2.normal * 0.1f;
		}
		else
		{
			this.knownSafePosition = idealClawPosition;
		}
		if (flag && flag4)
		{
			vector2 = raycastHit2.point + raycastHit2.normal * 0.01f;
			quaternion = Quaternion.LookRotation(-raycastHit2.normal, rotation * Vector3.up);
			this.SetClawAnchor(vector2, quaternion, vector2 - idealClawPosition);
			GorillaTagger.Instance.StartVibration(this.isLeftHanded, this.hapticStrengthOnGrab, this.hapticDurationOnGrab);
			this.currentFuel -= this.FuelCost_Grab;
		}
		else
		{
			if (flag && !this.wasGrabPressed && (!GorillaComputer.instance.IsPlayerInVirtualStump() || !CustomMapManager.WantsHoldingHandsDisabled()))
			{
				TakeMyHand_HandLink takeMyHand_HandLink2 = this.isLeftHanded ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink;
				Vector3 position5 = (this.isLeftHanded ? VRRig.LocalRig.leftHand : VRRig.LocalRig.rightHand).overrideTarget.position;
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (!vrrig.isLocal)
					{
						if (vrrig.leftHandLink.interactionPoint.OverlapCheck(vector2) && vrrig.leftHandLink.CanBeGrabbed())
						{
							if (takeMyHand_HandLink2.TentacleTryCreateLink(vrrig.leftHandLink))
							{
								this.isHoldingHand = true;
								this.clawHoldingVisual.SetActive(true);
								this.clawReleasedVisual.SetActive(false);
								takeMyHand_HandLink2.TentacleOffset = idealClawPosition - position5;
								this.heldPlayerCallback.Register(vrrig, vrrig.leftHandLink);
								this.gameEntity.RequestState(this.gameEntity.id, this.GetStateLong());
								break;
							}
						}
						else if (vrrig.rightHandLink.interactionPoint.OverlapCheck(vector2) && vrrig.rightHandLink.CanBeGrabbed() && takeMyHand_HandLink2.TentacleTryCreateLink(vrrig.rightHandLink))
						{
							this.isHoldingHand = true;
							this.clawHoldingVisual.SetActive(true);
							this.clawReleasedVisual.SetActive(false);
							takeMyHand_HandLink2.TentacleOffset = idealClawPosition - position5;
							this.heldPlayerCallback.Register(vrrig, vrrig.rightHandLink);
							this.gameEntity.RequestState(this.gameEntity.id, this.GetStateLong());
							break;
						}
					}
				}
			}
			Vector3 axis = Quaternion.AngleAxis(Time.time * 180f, Vector3.forward) * Vector3.up;
			if (flag4)
			{
				quaternion = Quaternion.Lerp(rotation, Quaternion.LookRotation(-raycastHit2.normal, rotation * Vector3.up), 0.75f);
				quaternion *= Quaternion.AngleAxis(5f, axis);
			}
			else
			{
				quaternion *= Quaternion.AngleAxis(20f, axis);
				vector2.y += 0.05f * Mathf.Cos(Time.time * 2f);
			}
		}
		this.clawVisualPos = vector2;
		this.clawVisualRot = quaternion;
		if (!this.isAnchored)
		{
			this.isLowFuel = (this.currentFuel < this.FuelCost_Grab);
		}
		this.wasGrabPressed = flag;
		this.UpdateFuelGauge();
	}

	private void UpdateFuelGauge()
	{
		float value = this.currentFuel / this.fuelSize;
		for (int i = 0; i < this.m_gaugeMatSlots.Length; i++)
		{
			this._gaugeMatPropBlock.SetFloat(ShaderProps._EmissionDissolveProgress, value);
			this.m_gaugeMatSlots[i].renderer.SetPropertyBlock(this._gaugeMatPropBlock, this.m_gaugeMatSlots[i].slot);
		}
	}

	protected override void OnUpdateRemote(float dt)
	{
		if (this.isAnchored)
		{
			return;
		}
		int attachedPlayerActorNumber = base.GetAttachedPlayerActorNumber();
		GamePlayer gamePlayer;
		if (attachedPlayerActorNumber < 1 || !GamePlayer.TryGetGamePlayer(attachedPlayerActorNumber, out gamePlayer))
		{
			return;
		}
		VRRig rig = gamePlayer.rig;
		Vector3 idealClawPosition = this.GetIdealClawPosition(rig);
		Quaternion rotation = base.transform.rotation;
		Vector3 position = base.transform.position;
		if ((this.knownSafePosition - idealClawPosition).IsLongerThan(1f))
		{
			this.knownSafePosition = position;
		}
		if (this.isHoldingHand)
		{
			TakeMyHand_HandLink takeMyHand_HandLink = this.isLeftHanded ? rig.leftHandLink : rig.rightHandLink;
			Vector3 position2 = (this.isLeftHanded ? rig.leftHand : rig.rightHand).rigTarget.position;
			takeMyHand_HandLink.TentacleOffset = idealClawPosition - position2;
			return;
		}
		float num = 0.15f;
		RaycastHit raycastHit;
		bool flag = Physics.SphereCast(new Ray(this.knownSafePosition, idealClawPosition - this.knownSafePosition), num, out raycastHit, (idealClawPosition - this.knownSafePosition).magnitude, this.worldCollisionLayers);
		Vector3 axis = Quaternion.AngleAxis(Time.time * 180f, Vector3.forward) * Vector3.up;
		Vector3 vector = idealClawPosition;
		Quaternion lhs = rotation;
		if (flag)
		{
			this.knownSafePosition += (idealClawPosition - this.knownSafePosition).normalized * (raycastHit.distance - num * 2.01f);
			vector = raycastHit.point + raycastHit.normal * 0.1f;
			lhs *= Quaternion.AngleAxis(5f, axis);
		}
		else
		{
			this.knownSafePosition = idealClawPosition;
			lhs *= Quaternion.AngleAxis(20f, axis);
			vector.y += 0.05f * Mathf.Cos(Time.time * 2f);
		}
		this.clawVisualPos = vector;
		this.clawVisualRot = lhs;
	}

	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this.FuelPerSecond_Holding = this._fps_holding_base * (withUpgrades.Contains(SIUpgradeType.Tentacle_Efficiency) ? 0.8f : 1f);
		this.FuelPerSecond_Recharging = this._fps_recharging_base * (withUpgrades.Contains(SIUpgradeType.Tentacle_Charge_Rate) ? 1.2f : 1f);
		this.FuelCost_Grab = this._grabCost_base * (withUpgrades.Contains(SIUpgradeType.Tentacle_Efficiency) ? 0.8f : 1f);
		this.FuelCost_JumpSpeed = this._jumpCost_base * (withUpgrades.Contains(SIUpgradeType.Tentacle_Efficiency) ? 0.8f : 1f);
		this.MaxGrabAngle = (withUpgrades.Contains(SIUpgradeType.Tentacle_Power_Claw) ? 180f : this._grabAngle_base);
		this.MaxTentacleJumpSpeed = this._jumpSpeed_base;
		this._min_grab_dot = Mathf.Cos(0.017453292f * this.MaxGrabAngle);
		this._lowFuelThreshold = this.FuelCost_Grab + this.FuelPerSecond_Holding;
	}

	private long GetStateLong()
	{
		if (this.isAnchored)
		{
			return 4611686018427387904L | BitPackUtils.PackAnchoredPosRotForNetwork(this.clawAnchorPosition, this.clawAnchorRotation);
		}
		if (this.isHoldingHand)
		{
			TakeMyHand_HandLink takeMyHand_HandLink = this.isLeftHanded ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink;
			NetPlayer grabbedPlayer = takeMyHand_HandLink.grabbedPlayer;
			int num = (grabbedPlayer != null) ? grabbedPlayer.ActorNumber : 0;
			return long.MinValue | (takeMyHand_HandLink.grabbedHandIsLeft ? 2305843009213693952L : 0L) | (long)num;
		}
		return 0L;
	}

	private void SetClawAnchor(Vector3 clawPosition, Quaternion clawRotation, Vector3 adjustment)
	{
		if (!this.isAnchored)
		{
			this.attachSound.Play();
		}
		this.hasFailedToGrab = false;
		this.isAnchored = true;
		this.clawHoldAdjustment = adjustment;
		this.clawAnchorPosition = clawPosition;
		this.clawAnchorRotation = clawRotation;
		this.clawHoldingVisual.SetActive(true);
		this.clawReleasedVisual.SetActive(false);
		if (this.IsEquippedLocal())
		{
			this.lastRequestedPlayerPosition = GTPlayer.Instance.transform.position;
			GTPlayer.Instance.SetGravityOverride(this, new Action<GTPlayer>(this.GravityOverrideFunction));
			this.hasGravityOverride = true;
			SIPlayer.LocalPlayer.OnKnockback += this.OnKnockback;
			this.gameEntity.RequestState(this.gameEntity.id, this.GetStateLong());
		}
	}

	private void ClearClawAnchor()
	{
		if (this.isAnchored || this.isHoldingHand)
		{
			this.detachSound.Play();
		}
		this.hasFailedToGrab = false;
		this.isAnchored = false;
		this.clawHoldingVisual.SetActive(false);
		this.clawReleasedVisual.SetActive(true);
		if (this.isHoldingHand && this.IsEquippedLocal())
		{
			(this.isLeftHanded ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink).BreakLink();
		}
		this.isHoldingHand = false;
		if (this.hasGravityOverride)
		{
			GTPlayer.Instance.UnsetGravityOverride(this);
			this.hasGravityOverride = false;
		}
		if (this.IsEquippedLocal())
		{
			Vector3 averagedVelocity = GTPlayer.Instance.AveragedVelocity;
			float num = averagedVelocity.magnitude;
			if (this.FuelCost_JumpSpeed > 0f)
			{
				num = Mathf.Min(num, this.currentFuel / this.FuelCost_JumpSpeed * this.MaxTentacleJumpSpeed);
			}
			num = Mathf.Min(num, this.MaxTentacleJumpSpeed);
			this.currentFuel -= num / this.MaxTentacleJumpSpeed * this.FuelCost_JumpSpeed;
			if (averagedVelocity.IsLongerThan(num))
			{
				GTPlayer.Instance.SetVelocity(averagedVelocity.normalized * num);
			}
			else
			{
				GTPlayer.Instance.SetVelocity(averagedVelocity);
			}
			SIPlayer.LocalPlayer.OnKnockback -= this.OnKnockback;
			this.gameEntity.RequestState(this.gameEntity.id, this.GetStateLong());
		}
	}

	private void OnKnockback(Vector3 knockbackVector)
	{
		if (this.isAnchored)
		{
			this.isGripBroken = true;
			this.ClearClawAnchor();
		}
	}

	private void GravityOverrideFunction(GTPlayer player)
	{
	}

	private void OnEntityStateChanged(long oldState, long newState)
	{
		if (this.IsEquippedLocal() || this.activatedLocally)
		{
			return;
		}
		if ((newState & -9223372036854775808L) != 0L)
		{
			this.isHoldingHand = true;
			this.clawHoldingVisual.SetActive(true);
			this.clawReleasedVisual.SetActive(false);
			GamePlayer gamePlayer;
			if (GamePlayer.TryGetGamePlayer((int)newState, out gamePlayer))
			{
				this.heldPlayerCallback.Register(gamePlayer.rig, ((newState & 2305843009213693952L) != 0L) ? gamePlayer.rig.leftHandLink : gamePlayer.rig.rightHandLink);
				return;
			}
		}
		else if (newState != 0L)
		{
			int attachedPlayerActorNumber = base.GetAttachedPlayerActorNumber();
			GamePlayer gamePlayer2;
			if (attachedPlayerActorNumber >= 1 && GamePlayer.TryGetGamePlayer(attachedPlayerActorNumber, out gamePlayer2))
			{
				Vector3 clawPosition;
				Quaternion clawRotation;
				BitPackUtils.UnpackAnchoredPosRotForNetwork(newState, gamePlayer2.rig.transform.position, out clawPosition, out clawRotation);
				this.SetClawAnchor(clawPosition, clawRotation, Vector3.zero);
				this.clawVisualPos = this.clawAnchorPosition;
				this.clawVisualRot = this.clawAnchorRotation;
				return;
			}
		}
		else
		{
			this.ClearClawAnchor();
		}
	}

	public override void OnEntityInit()
	{
		this.currentFuel = 10f;
	}

	public static Vector3 GetPlaneIntersection(Vector3 p1Pos, Vector3 p1Norm, Vector3 p2Pos, Vector3 p2Norm, Vector3 refPoint)
	{
		Vector3 normalized = Vector3.Cross(p1Norm, p2Norm).normalized;
		float num = Vector3.Dot(p1Pos, p1Norm);
		float num2 = Vector3.Dot(p2Pos, p2Norm);
		float num3 = Vector3.Dot(p1Norm, p2Norm);
		float num4 = 1f - num3 * num3;
		if (Mathf.Abs(num4) < 0.001f)
		{
			return refPoint;
		}
		float d = (num - num2 * num3) / num4;
		float d2 = (num2 - num * num3) / num4;
		Vector3 vector = d * p1Norm + d2 * p2Norm;
		return vector + Vector3.Project(refPoint - vector, normalized);
	}

	public static Vector3 SplineSample(float theta, Vector3 startDir, Vector3 endPos, Vector3 endDir)
	{
		float num = 1f - theta;
		float t = Mathf.Lerp(theta * theta, 1f - num * num, theta);
		Vector3 a = startDir * theta;
		Vector3 b = endPos + endDir * num;
		return Vector3.Lerp(a, b, t);
	}

	private void UpdateTentacle(Material material, Transform tentacle, Transform anchor)
	{
		Vector3 vector = Vector3.forward * this.LengthFactor;
		material.SetVector(this.tentacleStartDir_HASH, vector);
		Vector3 vector2 = tentacle.InverseTransformPoint(anchor.position);
		material.SetVector(this.tentacleEnd_HASH, vector2);
		Vector3 vector3 = -tentacle.InverseTransformDirection(anchor.forward) * this.LengthFactor;
		material.SetVector(this.tentacleEndDir_HASH, vector3);
		Vector3 vector4 = SIGadgetTentacleArm.SplineSample(0.25f, vector, vector2, vector3);
		Vector3 a = SIGadgetTentacleArm.SplineSample(0.26f, vector, vector2, vector3);
		Vector3 vector5 = SIGadgetTentacleArm.SplineSample(0.75f, vector, vector2, vector3);
		Vector3 a2 = SIGadgetTentacleArm.SplineSample(0.76f, vector, vector2, vector3);
		Vector3 planeIntersection = SIGadgetTentacleArm.GetPlaneIntersection(vector4, (a - vector4).normalized, vector5, (a2 - vector5).normalized, Quaternion.AngleAxis(90f, Vector3.forward) * vector2.WithZ(0f).normalized);
		material.SetVector(this.tentacleRingOrigin_HASH, planeIntersection);
	}

	public void CallBack()
	{
		this.lastCallbackFrame = Time.frameCount;
		if (this.isHoldingHand && this.lastHeldCallbackFrame != this.lastCallbackFrame)
		{
			return;
		}
		this.claw.transform.localPosition = Vector3.MoveTowards(this.claw.transform.localPosition, this.claw.transform.parent.InverseTransformPoint(this.clawVisualPos), this.ClawMaxBlendSpeed * Time.deltaTime);
		this.claw.transform.localRotation = Quaternion.RotateTowards(this.claw.transform.localRotation, this.claw.transform.parent.InverseTransformRotation(this.clawVisualRot), this.ClawMaxRotBlendSpeed * Time.deltaTime);
		this.UpdateTentacle(this.tentacleMat, this.tentacleRenderer.transform, this.tentacleAnchor);
		if (this.hasTentacle2)
		{
			this.UpdateTentacle(this.tentacleMat2, this.tentacleRenderer2.transform, this.tentacleAnchor2);
		}
	}

	private void UpdateTentacleHoldingHandPos(TakeMyHand_HandLink heldHandLink)
	{
		if (!this.isHoldingHand)
		{
			this.heldPlayerCallback.Unregister();
			return;
		}
		this.lastHeldCallbackFrame = Time.frameCount;
		this.clawVisualPos = heldHandLink.LinkPosition;
		this.clawVisualRot = heldHandLink.transform.rotation * Quaternion.AngleAxis(90f, Vector3.right);
		if (this.lastHeldCallbackFrame == this.lastCallbackFrame)
		{
			this.CallBack();
		}
	}

	public bool UsesEnergy
	{
		get
		{
			return true;
		}
	}

	public bool IsFull
	{
		get
		{
			return this.currentFuel >= this.fuelSize;
		}
	}

	public void UpdateRecharge(float dt)
	{
		if (!this.isAnchored)
		{
			this.currentFuel = Mathf.Clamp(this.currentFuel + dt * this.FuelPerSecond_Recharging, 0f, this.fuelSize);
		}
	}

	private const string preLog = "[SIGadgetWristJet]  ";

	private const string preErr = "[SIGadgetWristJet]  ERROR!!!  ";

	private const string preErrBeta = "[SIGadgetWristJet]  ERROR!!!  (beta only log)  ";

	[SerializeField]
	private GameObject claw;

	[SerializeField]
	private GameObject clawHoldingVisual;

	[SerializeField]
	private GameObject clawReleasedVisual;

	[SerializeField]
	private LayerMask worldCollisionLayers;

	[SerializeField]
	private Transform marker;

	[SerializeField]
	private float maxTentacleLength;

	[SerializeField]
	private float tentacleForwardAdjustment;

	[SerializeField]
	private MeshRenderer tentacleRenderer;

	[SerializeField]
	private Transform tentacleAnchor;

	[SerializeField]
	private MeshRenderer tentacleRenderer2;

	[SerializeField]
	private Transform tentacleAnchor2;

	[SerializeField]
	private SoundBankPlayer attachSound;

	[SerializeField]
	private SoundBankPlayer detachSound;

	[SerializeField]
	private SoundBankPlayer attachFailSound;

	[SerializeField]
	private SoundBankPlayer detachFailSound;

	[SerializeField]
	private SoundBankPlayer lowFuelSound;

	[SerializeField]
	private float hapticStrengthOnGrab = 0.5f;

	[SerializeField]
	private float hapticDurationOnGrab = 0.2f;

	[SerializeField]
	private float hapticStrengthOnRelease = 0.5f;

	[SerializeField]
	private float hapticDurationOnRelease = 0.2f;

	[SerializeField]
	private GameButtonActivatable buttonActivatable;

	[SerializeField]
	private float ClawMaxBlendSpeed = 10f;

	[SerializeField]
	private float ClawMaxRotBlendSpeed = 1000f;

	private MaterialPropertyBlock _gaugeMatPropBlock;

	[SerializeField]
	private GTRendererMatSlot[] m_gaugeMatSlots;

	private const float kFUEL_CAPACITY = 10f;

	private float fuelSize = 10f;

	private float currentFuel;

	public float FuelPerSecond_Holding = 1f;

	public float FuelCost_Wall_Multiplier = 2f;

	public float FuelCost_Slippery_Multiplier = 2f;

	public float FuelPerSecond_Recharging = 1f;

	public float FuelCost_Grab = 1f;

	public float FuelCost_JumpSpeed = 1f;

	public float MaxTentacleJumpSpeed = 8f;

	public float LengthFactor = 1.5f;

	public float MaxGrabAngle = 60f;

	public float WallAngle = 60f;

	public bool canHoldSlipperyWalls;

	private bool hasTentacle2;

	private Material tentacleMat;

	private Material tentacleMat2;

	private ShaderHashId tentacleStartDir_HASH = "_TentacleStartDir";

	private ShaderHashId tentacleEnd_HASH = "_TentacleEndPos";

	private ShaderHashId tentacleEndDir_HASH = "_TentacleEndDir";

	private ShaderHashId tentacleRingOrigin_HASH = "_TentacleRingOrigin";

	private bool isLeftHanded;

	private Vector3 knownSafePosition;

	private Vector3 clawHoldAdjustment;

	private Vector3 clawAnchorPosition;

	private Vector3 lastRequestedPlayerPosition;

	private Quaternion clawAnchorRotation;

	private bool isGripBroken;

	private bool hasGravityOverride;

	private bool isLowFuel;

	private bool hasFailedToGrab;

	private float _fps_holding_base;

	private float _fps_recharging_base;

	private float _grabCost_base;

	private float _jumpCost_base;

	private float _jumpSpeed_base;

	private float _grabAngle_base;

	private float _min_grab_dot;

	private float _wall_angle_dot;

	private float _current_grab_fps;

	private float _lowFuelThreshold;

	private SIGadgetTentacleArm.HeldPlayerCallback heldPlayerCallback;

	private bool hasRigCallback;

	private VRRig rigForCallback;

	private Vector3 clawVisualPos;

	private Quaternion clawVisualRot;

	private bool wasGrabPressed;

	private const long HoldingLeftHand_Bit = 2305843009213693952L;

	private const long Anchored_Bit = 4611686018427387904L;

	private const long HoldingHand_Bit = -9223372036854775808L;

	private int lastCallbackFrame;

	private int lastHeldCallbackFrame;

	private class HeldPlayerCallback : ICallBack
	{
		public HeldPlayerCallback(SIGadgetTentacleArm parent)
		{
			this.parent = parent;
		}

		public void Register(VRRig heldPlayer, TakeMyHand_HandLink heldHandLink)
		{
			this.Unregister();
			this.heldRig = heldPlayer;
			this.heldHandLink = heldHandLink;
			heldPlayer.AddLateUpdateCallback(this);
		}

		public void Unregister()
		{
			if (this.heldRig != null)
			{
				this.heldRig.RemoveLateUpdateCallback(this);
			}
			this.heldRig = null;
		}

		public void CallBack()
		{
			this.parent.UpdateTentacleHoldingHandPos(this.heldHandLink);
		}

		private SIGadgetTentacleArm parent;

		private VRRig heldRig;

		private TakeMyHand_HandLink heldHandLink;
	}
}
