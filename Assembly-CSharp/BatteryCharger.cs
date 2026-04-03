using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class BatteryCharger : MonoBehaviour
{
	public int CurrentEventPhase
	{
		get
		{
			if (!(this.state != null))
			{
				return -1;
			}
			return this.state.EventPhase;
		}
	}

	internal int RegisterCrank(BatteryChargerCrank crank)
	{
		if (this.crankCount >= 20)
		{
			Debug.LogError(string.Format("BatteryCharger: too many cranks (max {0})", 20), this);
			return -1;
		}
		int num = this.crankCount;
		this.cranks[num] = crank;
		this.crankCount++;
		return num;
	}

	private int LocalActorNr
	{
		get
		{
			if (PhotonNetwork.LocalPlayer == null)
			{
				return -1;
			}
			return PhotonNetwork.LocalPlayer.ActorNumber;
		}
	}

	private void OnEnable()
	{
		BatteryChargerState newState;
		if (this.stateRef.TryResolve<BatteryChargerState>(out newState))
		{
			this.Bind(newState);
			return;
		}
		this.stateRef.AddCallbackOnLoad(new Action(this.OnStateSceneLoaded));
	}

	private void OnDisable()
	{
		this.stateRef.RemoveCallbackOnLoad(new Action(this.OnStateSceneLoaded));
		this.Unbind();
	}

	private void OnStateSceneLoaded()
	{
		BatteryChargerState newState;
		if (this.stateRef.TryResolve<BatteryChargerState>(out newState))
		{
			this.Bind(newState);
		}
	}

	private void Bind(BatteryChargerState newState)
	{
		if (this.state == newState)
		{
			return;
		}
		this.Unbind();
		this.state = newState;
		if (this.state == null)
		{
			return;
		}
		this.state.onChargeChanged += this.OnChargeChanged;
		this.state.onFullyCharged += this.OnFullyCharged;
		this.state.onEventPhaseChanged += this.OnEventPhaseChanged;
		this.previousCharge = this.state.CurrentCharge;
		this.ApplyChargeVisuals();
		this.OnEventPhaseChanged(this.state.EventPhase);
	}

	private void Unbind()
	{
		if (this.state == null)
		{
			return;
		}
		this.state.onChargeChanged -= this.OnChargeChanged;
		this.state.onFullyCharged -= this.OnFullyCharged;
		this.state.onEventPhaseChanged -= this.OnEventPhaseChanged;
		this.state = null;
	}

	private void LateUpdate()
	{
		if (this.state == null)
		{
			return;
		}
		int localActorNr = this.LocalActorNr;
		for (int i = 0; i < this.crankCount; i++)
		{
			if (!(this.cranks[i] == null))
			{
				if (this.state.crankSyncs[i].holderActorNr == localActorNr)
				{
					this.state.UpdateLocalCrankState(i, this.cranks[i].IsHeldLeftHand, this.cranks[i].CurrentAngle);
				}
				this.UpdateRemoteCrankVisual(this.cranks[i], this.state.crankSyncs[i], localActorNr);
			}
		}
		if (this.chargingLoopSound != null)
		{
			bool flag = false;
			for (int j = 0; j < 20; j++)
			{
				if (this.state.crankSyncs[j].holderActorNr != -1)
				{
					flag = true;
					break;
				}
			}
			if (flag && !this.chargingLoopSound.isPlaying)
			{
				this.chargingLoopSound.Play();
				return;
			}
			if (!flag && this.chargingLoopSound.isPlaying)
			{
				this.chargingLoopSound.Stop();
			}
		}
	}

	private void UpdateRemoteCrankVisual(BatteryChargerCrank crank, BatteryChargerState.CrankSyncState syncState, int localActor)
	{
		if (crank == null || syncState.holderActorNr == localActor)
		{
			return;
		}
		if (syncState.holderActorNr != -1)
		{
			VRRig vrrig = BatteryChargerState.FindRigForActor(syncState.holderActorNr);
			if (vrrig != null)
			{
				crank.UpdateFromRemoteHand(vrrig, syncState.isLeftHand);
				return;
			}
		}
		crank.SetVisualAngle(syncState.angle);
	}

	internal bool IsCrankHeldLocally(int crankIndex)
	{
		return !(this.state == null) && crankIndex >= 0 && crankIndex < 20 && this.state.crankSyncs[crankIndex].holderActorNr == this.LocalActorNr;
	}

	public void SetEventPhase(int phase)
	{
		this.state.SetEventPhase(phase);
	}

	public void SetChargePerCrankDegree(float chargeRate)
	{
		this.state.SetChargePerCrankDegree(chargeRate);
	}

	internal bool OnCrankGrabbed(int crankIndex, bool isLeftHand)
	{
		return this.state.NotifyCrankGrabbed(crankIndex, isLeftHand);
	}

	internal void OnCrankReleased(int crankIndex, float finalAngle)
	{
		this.state.NotifyCrankReleased(crankIndex, finalAngle);
	}

	internal void OnCrankInput(int crankIndex, float degrees)
	{
		this.state.NotifyCrankInput(crankIndex, degrees);
		this.ApplyChargeVisuals();
	}

	private void OnChargeChanged()
	{
		for (int i = 0; i < this.actions.Length; i++)
		{
			if ((this.actions[i].Direction == BatteryCharger.BatteryChargerEvent.VDirection.Up && this.previousCharge < this.state.CurrentCharge && this.previousCharge < this.actions[i].Value && this.state.CurrentCharge >= this.state.CurrentCharge) || (this.actions[i].Direction == BatteryCharger.BatteryChargerEvent.VDirection.Down && this.previousCharge > this.state.CurrentCharge && this.previousCharge > this.actions[i].Value && this.state.CurrentCharge <= this.state.CurrentCharge))
			{
				UnityEvent action = this.actions[i].Action;
				if (action != null)
				{
					action.Invoke();
				}
			}
		}
		this.previousCharge = this.state.CurrentCharge;
		this.ApplyChargeVisuals();
	}

	private void OnFullyCharged()
	{
		if (this.fullyChargedSound != null)
		{
			this.fullyChargedSound.GTPlay();
		}
	}

	private void OnEventPhaseChanged(int phase)
	{
		for (int i = 0; i < this.eventPhases.Length; i++)
		{
			BatteryCharger.EventPhaseObjects eventPhaseObjects = this.eventPhases[i];
			if (((eventPhaseObjects != null) ? eventPhaseObjects.objects : null) != null)
			{
				bool active = i == phase;
				for (int j = 0; j < this.eventPhases[i].objects.Length; j++)
				{
					if (this.eventPhases[i].objects[j] != null)
					{
						this.eventPhases[i].objects[j].SetActive(active);
					}
				}
			}
		}
	}

	private void ApplyChargeVisuals()
	{
		if (this.state == null)
		{
			return;
		}
		float chargePercent = this.state.ChargePercent;
		if (this.chargeFillTransform != null)
		{
			this.chargeFillTransform.localRotation = Quaternion.Euler(0f, 0f, chargePercent * this.chargeFullRollAngle);
		}
		if (this.chargeFillRenderer != null)
		{
			this.chargeFillRenderer.material.color = Color.Lerp(this.emptyColor, this.fullColor, chargePercent);
		}
	}

	[Header("Network State")]
	[SerializeField]
	private XSceneRef stateRef;

	[Header("Charge Visuals")]
	[Tooltip("Transform rotated on its local Z axis to show charge level")]
	[SerializeField]
	private Transform chargeFillTransform;

	[Tooltip("Local Z rotation in degrees when fully charged")]
	[SerializeField]
	private float chargeFullRollAngle = -180f;

	[Tooltip("Renderer whose material color lerps with charge")]
	[SerializeField]
	private Renderer chargeFillRenderer;

	[SerializeField]
	private Color emptyColor = Color.red;

	[SerializeField]
	private Color fullColor = Color.green;

	[Header("Audio")]
	[SerializeField]
	private AudioSource chargingLoopSound;

	[SerializeField]
	private AudioSource fullyChargedSound;

	[Header("Event Phases")]
	[SerializeField]
	private BatteryCharger.EventPhaseObjects[] eventPhases;

	private BatteryChargerState state;

	private BatteryChargerCrank[] cranks = new BatteryChargerCrank[20];

	private int crankCount;

	[SerializeField]
	private BatteryCharger.BatteryChargerEvent[] actions;

	private float previousCharge;

	[Serializable]
	private class EventPhaseObjects
	{
		public string friendlyName;

		public GameObject[] objects;
	}

	[Serializable]
	private class BatteryChargerEvent
	{
		public BatteryCharger.BatteryChargerEvent.VDirection Direction
		{
			get
			{
				return this.direction;
			}
		}

		public float Value
		{
			get
			{
				return this.value;
			}
		}

		public UnityEvent Action
		{
			get
			{
				return this.action;
			}
		}

		[SerializeField]
		private BatteryCharger.BatteryChargerEvent.VDirection direction;

		[SerializeField]
		private float value;

		[SerializeField]
		private UnityEvent action;

		public enum VDirection
		{
			Up,
			Down
		}
	}
}
