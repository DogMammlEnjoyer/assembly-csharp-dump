using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[HelpURL("https://developer.oculus.com/documentation/unity/unity-sf-customhands/")]
public class OVRGrabber : MonoBehaviour
{
	public OVRGrabbable grabbedObject
	{
		get
		{
			return this.m_grabbedObj;
		}
	}

	public void ForceRelease(OVRGrabbable grabbable)
	{
		if (this.m_grabbedObj != null && this.m_grabbedObj == grabbable)
		{
			this.GrabEnd();
		}
	}

	protected virtual void Awake()
	{
		this.m_anchorOffsetPosition = base.transform.localPosition;
		this.m_anchorOffsetRotation = base.transform.localRotation;
		if (!this.m_moveHandPosition)
		{
			OVRCameraRig componentInParent = base.transform.GetComponentInParent<OVRCameraRig>();
			if (componentInParent != null)
			{
				componentInParent.UpdatedAnchors += delegate(OVRCameraRig r)
				{
					this.OnUpdatedAnchors();
				};
				this.m_operatingWithoutOVRCameraRig = false;
			}
		}
	}

	protected virtual void Start()
	{
		this.m_lastPos = base.transform.position;
		this.m_lastRot = base.transform.rotation;
		if (this.m_parentTransform == null)
		{
			this.m_parentTransform = base.gameObject.transform;
		}
		this.SetPlayerIgnoreCollision(base.gameObject, true);
	}

	public virtual void Update()
	{
		if (this.m_operatingWithoutOVRCameraRig)
		{
			this.OnUpdatedAnchors();
		}
	}

	private void OnUpdatedAnchors()
	{
		Vector3 vector = this.m_parentTransform.TransformPoint(this.m_anchorOffsetPosition);
		Quaternion rot = this.m_parentTransform.rotation * this.m_anchorOffsetRotation;
		if (this.m_moveHandPosition)
		{
			base.GetComponent<Rigidbody>().MovePosition(vector);
			base.GetComponent<Rigidbody>().MoveRotation(rot);
		}
		if (!this.m_parentHeldObject)
		{
			this.MoveGrabbedObject(vector, rot, false);
		}
		this.m_lastPos = base.transform.position;
		this.m_lastRot = base.transform.rotation;
		float prevFlex = this.m_prevFlex;
		this.m_prevFlex = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, this.m_controller);
		this.CheckForGrabOrRelease(prevFlex);
	}

	private void OnDestroy()
	{
		if (this.m_grabbedObj != null)
		{
			this.GrabEnd();
		}
	}

	private void OnTriggerEnter(Collider otherCollider)
	{
		OVRGrabbable ovrgrabbable = otherCollider.GetComponent<OVRGrabbable>() ?? otherCollider.GetComponentInParent<OVRGrabbable>();
		if (ovrgrabbable == null)
		{
			return;
		}
		int num = 0;
		this.m_grabCandidates.TryGetValue(ovrgrabbable, out num);
		this.m_grabCandidates[ovrgrabbable] = num + 1;
	}

	private void OnTriggerExit(Collider otherCollider)
	{
		OVRGrabbable ovrgrabbable = otherCollider.GetComponent<OVRGrabbable>() ?? otherCollider.GetComponentInParent<OVRGrabbable>();
		if (ovrgrabbable == null)
		{
			return;
		}
		int num = 0;
		if (!this.m_grabCandidates.TryGetValue(ovrgrabbable, out num))
		{
			return;
		}
		if (num > 1)
		{
			this.m_grabCandidates[ovrgrabbable] = num - 1;
			return;
		}
		this.m_grabCandidates.Remove(ovrgrabbable);
	}

	protected void CheckForGrabOrRelease(float prevFlex)
	{
		if (this.m_prevFlex >= this.grabBegin && prevFlex < this.grabBegin)
		{
			this.GrabBegin();
			return;
		}
		if (this.m_prevFlex <= this.grabEnd && prevFlex > this.grabEnd)
		{
			this.GrabEnd();
		}
	}

	protected virtual void GrabBegin()
	{
		float num = float.MaxValue;
		OVRGrabbable ovrgrabbable = null;
		Collider grabPoint = null;
		foreach (OVRGrabbable ovrgrabbable2 in this.m_grabCandidates.Keys)
		{
			if (!ovrgrabbable2.isGrabbed || ovrgrabbable2.allowOffhandGrab)
			{
				for (int i = 0; i < ovrgrabbable2.grabPoints.Length; i++)
				{
					Collider collider = ovrgrabbable2.grabPoints[i];
					Vector3 b = collider.ClosestPointOnBounds(this.m_gripTransform.position);
					float sqrMagnitude = (this.m_gripTransform.position - b).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						ovrgrabbable = ovrgrabbable2;
						grabPoint = collider;
					}
				}
			}
		}
		this.GrabVolumeEnable(false);
		if (ovrgrabbable != null)
		{
			if (ovrgrabbable.isGrabbed)
			{
				ovrgrabbable.grabbedBy.OffhandGrabbed(ovrgrabbable);
			}
			this.m_grabbedObj = ovrgrabbable;
			this.m_grabbedObj.GrabBegin(this, grabPoint);
			this.m_lastPos = base.transform.position;
			this.m_lastRot = base.transform.rotation;
			if (this.m_grabbedObj.snapPosition)
			{
				this.m_grabbedObjectPosOff = this.m_gripTransform.localPosition;
				if (this.m_grabbedObj.snapOffset)
				{
					Vector3 position = this.m_grabbedObj.snapOffset.position;
					if (this.m_controller == OVRInput.Controller.LTouch)
					{
						position.x = -position.x;
					}
					this.m_grabbedObjectPosOff += position;
				}
			}
			else
			{
				Vector3 vector = this.m_grabbedObj.transform.position - base.transform.position;
				vector = Quaternion.Inverse(base.transform.rotation) * vector;
				this.m_grabbedObjectPosOff = vector;
			}
			if (this.m_grabbedObj.snapOrientation)
			{
				this.m_grabbedObjectRotOff = this.m_gripTransform.localRotation;
				if (this.m_grabbedObj.snapOffset)
				{
					this.m_grabbedObjectRotOff = this.m_grabbedObj.snapOffset.rotation * this.m_grabbedObjectRotOff;
				}
			}
			else
			{
				Quaternion grabbedObjectRotOff = Quaternion.Inverse(base.transform.rotation) * this.m_grabbedObj.transform.rotation;
				this.m_grabbedObjectRotOff = grabbedObjectRotOff;
			}
			this.MoveGrabbedObject(this.m_lastPos, this.m_lastRot, true);
			this.SetPlayerIgnoreCollision(this.m_grabbedObj.gameObject, true);
			if (this.m_parentHeldObject)
			{
				this.m_grabbedObj.transform.parent = base.transform;
			}
		}
	}

	protected virtual void MoveGrabbedObject(Vector3 pos, Quaternion rot, bool forceTeleport = false)
	{
		if (this.m_grabbedObj == null)
		{
			return;
		}
		Rigidbody grabbedRigidbody = this.m_grabbedObj.grabbedRigidbody;
		Vector3 position = pos + rot * this.m_grabbedObjectPosOff;
		Quaternion quaternion = rot * this.m_grabbedObjectRotOff;
		if (forceTeleport)
		{
			grabbedRigidbody.transform.position = position;
			grabbedRigidbody.transform.rotation = quaternion;
			return;
		}
		grabbedRigidbody.MovePosition(position);
		grabbedRigidbody.MoveRotation(quaternion);
	}

	protected void GrabEnd()
	{
		if (this.m_grabbedObj != null)
		{
			OVRPose lhs = new OVRPose
			{
				position = OVRInput.GetLocalControllerPosition(this.m_controller),
				orientation = OVRInput.GetLocalControllerRotation(this.m_controller)
			};
			OVRPose rhs = new OVRPose
			{
				position = this.m_anchorOffsetPosition,
				orientation = this.m_anchorOffsetRotation
			};
			lhs *= rhs;
			OVRPose ovrpose = base.transform.ToOVRPose(false) * lhs.Inverse();
			Vector3 linearVelocity = ovrpose.orientation * OVRInput.GetLocalControllerVelocity(this.m_controller);
			Vector3 angularVelocity = ovrpose.orientation * OVRInput.GetLocalControllerAngularVelocity(this.m_controller);
			this.GrabbableRelease(linearVelocity, angularVelocity);
		}
		this.GrabVolumeEnable(true);
	}

	protected void GrabbableRelease(Vector3 linearVelocity, Vector3 angularVelocity)
	{
		this.m_grabbedObj.GrabEnd(linearVelocity, angularVelocity);
		if (this.m_parentHeldObject)
		{
			this.m_grabbedObj.transform.parent = null;
		}
		this.m_grabbedObj = null;
	}

	protected virtual void GrabVolumeEnable(bool enabled)
	{
		if (this.m_grabVolumeEnabled == enabled)
		{
			return;
		}
		this.m_grabVolumeEnabled = enabled;
		for (int i = 0; i < this.m_grabVolumes.Length; i++)
		{
			this.m_grabVolumes[i].enabled = this.m_grabVolumeEnabled;
		}
		if (!this.m_grabVolumeEnabled)
		{
			this.m_grabCandidates.Clear();
		}
	}

	protected virtual void OffhandGrabbed(OVRGrabbable grabbable)
	{
		if (this.m_grabbedObj == grabbable)
		{
			this.GrabbableRelease(Vector3.zero, Vector3.zero);
		}
	}

	protected void SetPlayerIgnoreCollision(GameObject grabbable, bool ignore)
	{
		if (this.m_player != null)
		{
			foreach (Collider collider in this.m_player.GetComponentsInChildren<Collider>())
			{
				foreach (Collider collider2 in grabbable.GetComponentsInChildren<Collider>())
				{
					if (!collider2.isTrigger && !collider.isTrigger)
					{
						Physics.IgnoreCollision(collider2, collider, ignore);
					}
				}
			}
		}
	}

	public float grabBegin = 0.55f;

	public float grabEnd = 0.35f;

	[SerializeField]
	protected bool m_parentHeldObject;

	[SerializeField]
	protected bool m_moveHandPosition;

	[SerializeField]
	protected Transform m_gripTransform;

	[SerializeField]
	protected Collider[] m_grabVolumes;

	[SerializeField]
	protected OVRInput.Controller m_controller;

	[SerializeField]
	protected Transform m_parentTransform;

	[SerializeField]
	protected GameObject m_player;

	protected bool m_grabVolumeEnabled = true;

	protected Vector3 m_lastPos;

	protected Quaternion m_lastRot;

	protected Quaternion m_anchorOffsetRotation;

	protected Vector3 m_anchorOffsetPosition;

	protected float m_prevFlex;

	protected OVRGrabbable m_grabbedObj;

	protected Vector3 m_grabbedObjectPosOff;

	protected Quaternion m_grabbedObjectRotOff;

	protected Dictionary<OVRGrabbable, int> m_grabCandidates = new Dictionary<OVRGrabbable, int>();

	protected bool m_operatingWithoutOVRCameraRig = true;
}
