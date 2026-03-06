using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RigEventGate : MonoBehaviour, IBuildValidation
{
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
		RigEventVolumeTrigger rigEventVolumeTrigger = null;
		for (int i = 0; i < this.gameObjects.Count; i++)
		{
			if (this.gameObjects[i].Rig == rc.Rig)
			{
				rigEventVolumeTrigger = this.gameObjects[i];
			}
		}
		int num = (this.rigCollection == null) ? 1 : this.rigCollection.Rigs.Count;
		if (rigEventVolumeTrigger != null)
		{
			this.gameObjects.Remove(rigEventVolumeTrigger);
			this.countChanged(this.gameObjects.Count + 1, this.gameObjects.Count, num + 1, num, null);
			return;
		}
		this.countChanged(this.gameObjects.Count, this.gameObjects.Count, num + 1, num, null);
	}

	private void OnTriggerEnter(Collider other)
	{
		RigEventVolumeTrigger rigEventVolumeTrigger;
		if (!other.gameObject.TryGetComponent<RigEventVolumeTrigger>(out rigEventVolumeTrigger) || base.transform.InverseTransformPoint(rigEventVolumeTrigger.transform.position).z < 0f)
		{
			return;
		}
		if (this.gameObjects.Contains(rigEventVolumeTrigger))
		{
			int num = (this.rigCollection == null) ? 1 : this.rigCollection.Rigs.Count;
			int count = this.gameObjects.Count;
			this.gameObjects.Remove(rigEventVolumeTrigger);
			this.countChanged(count, this.gameObjects.Count, num, num, rigEventVolumeTrigger);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		RigEventVolumeTrigger rigEventVolumeTrigger;
		if (!other.gameObject.TryGetComponent<RigEventVolumeTrigger>(out rigEventVolumeTrigger) || base.transform.InverseTransformPoint(rigEventVolumeTrigger.transform.position).z < 0f)
		{
			return;
		}
		if (!this.gameObjects.Contains(rigEventVolumeTrigger))
		{
			int num = (this.rigCollection == null) ? 1 : this.rigCollection.Rigs.Count;
			int count = this.gameObjects.Count;
			this.gameObjects.Add(rigEventVolumeTrigger);
			this.countChanged(count, this.gameObjects.Count, num, num, rigEventVolumeTrigger);
		}
	}

	private void countChanged(int oldValue, int newValue, int oldPlayerCount, int newPlayerCount, RigEventVolumeTrigger rig)
	{
		if (newValue > oldValue)
		{
			if (rig != null)
			{
				UnityEvent<VRRig> rigExits = this.RigExits;
				if (rigExits != null)
				{
					rigExits.Invoke(rig.Rig);
				}
			}
			if ((this.mode == RigEventGate.Mode.RELATIVE && (float)newValue / (float)newPlayerCount >= this.relThreshold && (float)oldValue / (float)oldPlayerCount < this.relThreshold) || (this.mode == RigEventGate.Mode.ABSOLUTE && newValue >= this.absThreshold && oldValue < this.absThreshold))
			{
				UnityEvent goesOverThreshold = this.GoesOverThreshold;
				if (goesOverThreshold == null)
				{
					return;
				}
				goesOverThreshold.Invoke();
			}
		}
	}

	bool IBuildValidation.BuildValidationCheck()
	{
		if (this.mode == RigEventGate.Mode.RELATIVE && this.rigCollection == null)
		{
			Debug.Log("RigEventGate on " + base.name + " is set to RELATIVE mode but has no Player Count Source. This will crash!");
			return false;
		}
		return true;
	}

	private List<RigEventVolumeTrigger> gameObjects = new List<RigEventVolumeTrigger>();

	[SerializeField]
	private RigEventGate.Mode mode = RigEventGate.Mode.ABSOLUTE;

	[Range(0.05f, 1f)]
	[SerializeField]
	private float relThreshold = 0.05f;

	[SerializeField]
	private VRRigCollection rigCollection;

	[Range(1f, 20f)]
	[SerializeField]
	private int absThreshold = 1;

	[SerializeField]
	private UnityEvent<VRRig> RigExits;

	[SerializeField]
	private UnityEvent GoesOverThreshold;

	private enum Mode
	{
		RELATIVE,
		ABSOLUTE
	}
}
