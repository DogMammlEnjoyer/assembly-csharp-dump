using System;
using UnityEngine;

public class GREnemyBossMoonColliderHelper : MonoBehaviour
{
	public void Awake()
	{
		if (this.ResizeOnAwake)
		{
			base.transform.localScale = this.ResizeCollider;
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("GorillaPlayer"))
		{
			VRRig component = other.attachedRigidbody.GetComponent<VRRig>();
			if (component != null && component == VRRigCache.Instance.localRig.Rig && Time.time - this.lastTriggered > 0.5f)
			{
				if (this.localPlayer == null)
				{
					this.localPlayer = VRRig.LocalRig.GetComponent<GRPlayer>();
				}
				this.lastTriggered = Time.time;
				this.boss.HitPlayer(this.localPlayer, true);
				this.boss.ShockPlayer();
			}
		}
	}

	public bool ResizeOnAwake = true;

	public Vector3 ResizeCollider = new Vector3(1.025f, 1.025f, 1.025f);

	[SerializeField]
	private GREnemyBossMoon boss;

	[SerializeField]
	private GRPlayer localPlayer;

	private float lastTriggered;
}
