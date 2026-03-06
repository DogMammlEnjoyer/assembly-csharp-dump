using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Drawing.Examples
{
	public class GizmoSphereExample : MonoBehaviourGizmos
	{
		public override void DrawGizmos()
		{
			using (Draw.InLocalSpace(base.transform))
			{
				Draw.WireSphere(Vector3.zero, 0.5f, this.gizmoColor);
				foreach (GizmoSphereExample.Contact contact in this.contactForces.Values)
				{
					Draw.Circle(contact.lastPoint, contact.lastNormal, 0.1f * contact.impulse, this.gizmoColor2);
					Draw.SolidCircle(contact.lastPoint, contact.lastNormal, 0.1f * contact.impulse, this.gizmoColor2);
				}
			}
		}

		private void FixedUpdate()
		{
			foreach (Collider key in this.contactForces.Keys.ToList<Collider>())
			{
				GizmoSphereExample.Contact contact = this.contactForces[key];
				if (contact.impulse > 0.1f)
				{
					contact.impulse = Mathf.Lerp(contact.impulse, 0f, 10f * Time.fixedDeltaTime);
					contact.smoothImpulse = Mathf.Lerp(contact.impulse, contact.smoothImpulse, 20f * Time.fixedDeltaTime);
					this.contactForces[key] = contact;
				}
				else
				{
					this.contactForces.Remove(key);
				}
			}
		}

		private void OnCollisionStay(Collision collision)
		{
			ContactPoint[] contacts = collision.contacts;
			int num = 0;
			if (num >= contacts.Length)
			{
				return;
			}
			ContactPoint contactPoint = contacts[num];
			if (!this.contactForces.ContainsKey(collision.collider))
			{
				this.contactForces.Add(collision.collider, new GizmoSphereExample.Contact
				{
					impulse = 2f
				});
			}
			GizmoSphereExample.Contact contact = this.contactForces[collision.collider];
			contact.impulse = Mathf.Max(contact.impulse, 1f);
			contact.lastPoint = base.transform.InverseTransformPoint(contactPoint.point);
			contact.lastNormal = base.transform.InverseTransformVector(contactPoint.normal);
			this.contactForces[collision.collider] = contact;
		}

		public Color gizmoColor = new Color(1f, 0.34509805f, 0.33333334f);

		public Color gizmoColor2 = new Color(0.30980393f, 0.8f, 0.92941177f);

		private Dictionary<Collider, GizmoSphereExample.Contact> contactForces = new Dictionary<Collider, GizmoSphereExample.Contact>();

		private struct Contact
		{
			public float impulse;

			public float smoothImpulse;

			public Vector3 lastPoint;

			public Vector3 lastNormal;
		}
	}
}
