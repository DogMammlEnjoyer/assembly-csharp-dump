using System;
using UnityEngine;

public class RigDuplicationZone : MonoBehaviour
{
	public static event RigDuplicationZone.RigDuplicationZoneAction OnEnabled;

	public string Id
	{
		get
		{
			return this.id;
		}
	}

	private void OnEnable()
	{
		RigDuplicationZone.OnEnabled += this.RigDuplicationZone_OnEnabled;
		if (RigDuplicationZone.OnEnabled != null)
		{
			RigDuplicationZone.OnEnabled(this);
		}
	}

	private void OnDisable()
	{
		RigDuplicationZone.OnEnabled -= this.RigDuplicationZone_OnEnabled;
	}

	private void RigDuplicationZone_OnEnabled(RigDuplicationZone z)
	{
		if (z == this)
		{
			return;
		}
		if (z.id != this.id)
		{
			return;
		}
		this.SetOtherZone(z);
		z.SetOtherZone(this);
	}

	private void SetOtherZone(RigDuplicationZone z)
	{
		this.otherZone = z;
		this.offsetToOtherZone = z.transform.position - base.transform.position;
	}

	private void OnTriggerEnter(Collider other)
	{
		VRRig component = other.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		if (component.isLocal)
		{
			this.playerInZone = true;
			return;
		}
		component.SetDuplicationZone(this);
	}

	private void OnTriggerExit(Collider other)
	{
		VRRig component = other.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		if (component.isLocal)
		{
			this.playerInZone = false;
			return;
		}
		component.ClearDuplicationZone(this);
	}

	public Vector3 GetVisualOffsetForRigs(Vector3 cachedOffset)
	{
		if (!this.otherZone.playerInZone)
		{
			return cachedOffset;
		}
		return this.offsetToOtherZone + cachedOffset;
	}

	public bool IsApplyingDisplacement
	{
		get
		{
			return this.otherZone.playerInZone;
		}
	}

	private RigDuplicationZone otherZone;

	[SerializeField]
	private string id;

	private bool playerInZone;

	private Vector3 offsetToOtherZone;

	public delegate void RigDuplicationZoneAction(RigDuplicationZone z);
}
