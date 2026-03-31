using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RigEventVolume : MonoBehaviour, IBuildValidation
{
	public VRRig[] Rigs
	{
		get
		{
			return this.rigs.ToArray();
		}
	}

	public int RigCount
	{
		get
		{
			return this.gameObjects.Keys.Count;
		}
	}

	public event Action OnCountChanged;

	private void OnEnable()
	{
		if (this.rigCollection == null)
		{
			return;
		}
		VRRigCollection vrrigCollection = this.rigCollection;
		vrrigCollection.playerEnteredCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection.playerEnteredCollection, new Action<RigContainer>(this.OnJoined));
		VRRigCollection vrrigCollection2 = this.rigCollection;
		vrrigCollection2.playerLeftCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection2.playerLeftCollection, new Action<RigContainer>(this.OnLeft));
	}

	private void OnDisable()
	{
		if (this.rigCollection == null)
		{
			return;
		}
		VRRigCollection vrrigCollection = this.rigCollection;
		vrrigCollection.playerEnteredCollection = (Action<RigContainer>)Delegate.Remove(vrrigCollection.playerEnteredCollection, new Action<RigContainer>(this.OnJoined));
		VRRigCollection vrrigCollection2 = this.rigCollection;
		vrrigCollection2.playerLeftCollection = (Action<RigContainer>)Delegate.Remove(vrrigCollection2.playerLeftCollection, new Action<RigContainer>(this.OnLeft));
	}

	private void OnDestroy()
	{
		if (this.rigCollection == null)
		{
			return;
		}
		VRRigCollection vrrigCollection = this.rigCollection;
		vrrigCollection.playerEnteredCollection = (Action<RigContainer>)Delegate.Remove(vrrigCollection.playerEnteredCollection, new Action<RigContainer>(this.OnJoined));
		VRRigCollection vrrigCollection2 = this.rigCollection;
		vrrigCollection2.playerLeftCollection = (Action<RigContainer>)Delegate.Remove(vrrigCollection2.playerLeftCollection, new Action<RigContainer>(this.OnLeft));
	}

	private void OnJoined(RigContainer rc)
	{
		int num = (this.rigCollection == null) ? 1 : this.rigCollection.Rigs.Count;
		this.countChanged(this.gameObjects.Count, this.gameObjects.Count, num - 1, num, null);
	}

	private void OnLeft(RigContainer rc)
	{
		int num = (this.rigCollection == null) ? 1 : this.rigCollection.Rigs.Count;
		this.countChanged(this.gameObjects.Count, this.gameObjects.Count, num + 1, num, null);
	}

	private void OnTriggerEnter(Collider other)
	{
		RigEventVolumeTrigger rigEventVolumeTrigger;
		if (other.gameObject.TryGetComponent<RigEventVolumeTrigger>(out rigEventVolumeTrigger))
		{
			if (!this.gameObjects.ContainsKey(rigEventVolumeTrigger))
			{
				this.gameObjects.Add(rigEventVolumeTrigger, 0);
				this.rigs.Add(rigEventVolumeTrigger.Rig);
				int num = (this.rigCollection == null) ? 1 : this.rigCollection.Rigs.Count;
				this.countChanged(this.gameObjects.Count - 1, this.gameObjects.Count, num, num, rigEventVolumeTrigger);
				if (rigEventVolumeTrigger.Rig == VRRig.LocalRig)
				{
					UnityEvent<VRRig> localRigEnters = this.LocalRigEnters;
					if (localRigEnters == null)
					{
						return;
					}
					localRigEnters.Invoke(rigEventVolumeTrigger.Rig);
					return;
				}
			}
			else
			{
				Dictionary<RigEventVolumeTrigger, int> dictionary = this.gameObjects;
				RigEventVolumeTrigger key = rigEventVolumeTrigger;
				int num2 = dictionary[key];
				dictionary[key] = num2 + 1;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		RigEventVolumeTrigger rigEventVolumeTrigger;
		if (other.gameObject.TryGetComponent<RigEventVolumeTrigger>(out rigEventVolumeTrigger))
		{
			Dictionary<RigEventVolumeTrigger, int> dictionary = this.gameObjects;
			RigEventVolumeTrigger key = rigEventVolumeTrigger;
			int num = dictionary[key];
			dictionary[key] = num - 1;
			if (this.gameObjects[rigEventVolumeTrigger] < 0)
			{
				this.gameObjects.Remove(rigEventVolumeTrigger);
				this.rigs.Remove(rigEventVolumeTrigger.Rig);
				int num2 = (this.rigCollection == null) ? 1 : this.rigCollection.Rigs.Count;
				this.countChanged(this.gameObjects.Count + 1, this.gameObjects.Count, num2, num2, rigEventVolumeTrigger);
				if (rigEventVolumeTrigger.Rig == VRRig.LocalRig)
				{
					UnityEvent<VRRig> localRigExits = this.LocalRigExits;
					if (localRigExits == null)
					{
						return;
					}
					localRigExits.Invoke(rigEventVolumeTrigger.Rig);
				}
			}
		}
	}

	private void countChanged(int oldValue, int newValue, int oldPlayerCount, int newPlayerCount, RigEventVolumeTrigger rig)
	{
		if (newValue > oldValue)
		{
			if (rig != null)
			{
				UnityEvent<VRRig> rigEnters = this.RigEnters;
				if (rigEnters != null)
				{
					rigEnters.Invoke(rig.Rig);
				}
			}
			if ((this.mode == RigEventVolume.Mode.RELATIVE && (float)newValue / (float)newPlayerCount >= this.relThreshold && (float)oldValue / (float)oldPlayerCount < this.relThreshold) || (this.mode == RigEventVolume.Mode.ABSOLUTE && newValue >= this.absThreshold && oldValue < this.absThreshold))
			{
				UnityEvent goesOverThreshold = this.GoesOverThreshold;
				if (goesOverThreshold != null)
				{
					goesOverThreshold.Invoke();
				}
			}
		}
		else if (newValue < oldValue)
		{
			if (rig != null)
			{
				UnityEvent<VRRig> rigExits = this.RigExits;
				if (rigExits != null)
				{
					rigExits.Invoke(rig.Rig);
				}
			}
			if ((this.mode == RigEventVolume.Mode.RELATIVE && (float)newValue / (float)newPlayerCount < this.relThreshold && (float)oldValue / (float)oldPlayerCount >= this.relThreshold) || (this.mode == RigEventVolume.Mode.ABSOLUTE && newValue < this.absThreshold && oldValue >= this.absThreshold))
			{
				UnityEvent goesUnderThreshold = this.GoesUnderThreshold;
				if (goesUnderThreshold != null)
				{
					goesUnderThreshold.Invoke();
				}
			}
		}
		Action onCountChanged = this.OnCountChanged;
		if (onCountChanged == null)
		{
			return;
		}
		onCountChanged();
	}

	bool IBuildValidation.BuildValidationCheck()
	{
		if (this.mode == RigEventVolume.Mode.RELATIVE && this.rigCollection == null)
		{
			Debug.Log("RigEventVolume on " + base.name + " is set to RELATIVE mode but has no Player Count Source. This will crash!");
			return false;
		}
		return true;
	}

	private Dictionary<RigEventVolumeTrigger, int> gameObjects = new Dictionary<RigEventVolumeTrigger, int>();

	[SerializeField]
	private RigEventVolume.Mode mode = RigEventVolume.Mode.ABSOLUTE;

	[Range(0.05f, 1f)]
	[SerializeField]
	private float relThreshold = 0.05f;

	[SerializeField]
	private VRRigCollection rigCollection;

	[Range(1f, 20f)]
	[SerializeField]
	private int absThreshold = 1;

	[SerializeField]
	private UnityEvent<VRRig> RigEnters;

	[SerializeField]
	private UnityEvent<VRRig> RigExits;

	[SerializeField]
	private UnityEvent GoesOverThreshold;

	[SerializeField]
	private UnityEvent GoesUnderThreshold;

	[SerializeField]
	private UnityEvent<VRRig> LocalRigEnters;

	[SerializeField]
	private UnityEvent<VRRig> LocalRigExits;

	private List<VRRig> rigs = new List<VRRig>();

	private enum Mode
	{
		RELATIVE,
		ABSOLUTE
	}
}
