using System;
using UnityEngine;

namespace Valve.VR.Extras
{
	public class SteamVR_LaserPointer : MonoBehaviour
	{
		public event PointerEventHandler PointerIn;

		public event PointerEventHandler PointerOut;

		public event PointerEventHandler PointerClick;

		private void Start()
		{
			if (this.pose == null)
			{
				this.pose = base.GetComponent<SteamVR_Behaviour_Pose>();
			}
			if (this.pose == null)
			{
				Debug.LogError("No SteamVR_Behaviour_Pose component found on this object", this);
			}
			if (this.interactWithUI == null)
			{
				Debug.LogError("No ui interaction action has been set on this component.", this);
			}
			this.holder = new GameObject();
			this.holder.transform.parent = base.transform;
			this.holder.transform.localPosition = Vector3.zero;
			this.holder.transform.localRotation = Quaternion.identity;
			this.pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
			this.pointer.transform.parent = this.holder.transform;
			this.pointer.transform.localScale = new Vector3(this.thickness, this.thickness, 100f);
			this.pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
			this.pointer.transform.localRotation = Quaternion.identity;
			BoxCollider component = this.pointer.GetComponent<BoxCollider>();
			if (this.addRigidBody)
			{
				if (component)
				{
					component.isTrigger = true;
				}
				this.pointer.AddComponent<Rigidbody>().isKinematic = true;
			}
			else if (component)
			{
				Object.Destroy(component);
			}
			Material material = new Material(Shader.Find("Unlit/Color"));
			material.SetColor("_Color", this.color);
			this.pointer.GetComponent<MeshRenderer>().material = material;
		}

		public virtual void OnPointerIn(PointerEventArgs e)
		{
			if (this.PointerIn != null)
			{
				this.PointerIn(this, e);
			}
		}

		public virtual void OnPointerClick(PointerEventArgs e)
		{
			if (this.PointerClick != null)
			{
				this.PointerClick(this, e);
			}
		}

		public virtual void OnPointerOut(PointerEventArgs e)
		{
			if (this.PointerOut != null)
			{
				this.PointerOut(this, e);
			}
		}

		private void Update()
		{
			if (!this.isActive)
			{
				this.isActive = true;
				base.transform.GetChild(0).gameObject.SetActive(true);
			}
			float num = 100f;
			RaycastHit raycastHit;
			bool flag = Physics.Raycast(new Ray(base.transform.position, base.transform.forward), out raycastHit);
			if (this.previousContact && this.previousContact != raycastHit.transform)
			{
				this.OnPointerOut(new PointerEventArgs
				{
					fromInputSource = this.pose.inputSource,
					distance = 0f,
					flags = 0U,
					target = this.previousContact
				});
				this.previousContact = null;
			}
			if (flag && this.previousContact != raycastHit.transform)
			{
				this.OnPointerIn(new PointerEventArgs
				{
					fromInputSource = this.pose.inputSource,
					distance = raycastHit.distance,
					flags = 0U,
					target = raycastHit.transform
				});
				this.previousContact = raycastHit.transform;
			}
			if (!flag)
			{
				this.previousContact = null;
			}
			if (flag && raycastHit.distance < 100f)
			{
				num = raycastHit.distance;
			}
			if (flag && this.interactWithUI.GetStateUp(this.pose.inputSource))
			{
				this.OnPointerClick(new PointerEventArgs
				{
					fromInputSource = this.pose.inputSource,
					distance = raycastHit.distance,
					flags = 0U,
					target = raycastHit.transform
				});
			}
			if (this.interactWithUI != null && this.interactWithUI.GetState(this.pose.inputSource))
			{
				this.pointer.transform.localScale = new Vector3(this.thickness * 5f, this.thickness * 5f, num);
				this.pointer.GetComponent<MeshRenderer>().material.color = this.clickColor;
			}
			else
			{
				this.pointer.transform.localScale = new Vector3(this.thickness, this.thickness, num);
				this.pointer.GetComponent<MeshRenderer>().material.color = this.color;
			}
			this.pointer.transform.localPosition = new Vector3(0f, 0f, num / 2f);
		}

		public SteamVR_Behaviour_Pose pose;

		public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI", false);

		public bool active = true;

		public Color color;

		public float thickness = 0.002f;

		public Color clickColor = Color.green;

		public GameObject holder;

		public GameObject pointer;

		private bool isActive;

		public bool addRigidBody;

		public Transform reference;

		private Transform previousContact;
	}
}
