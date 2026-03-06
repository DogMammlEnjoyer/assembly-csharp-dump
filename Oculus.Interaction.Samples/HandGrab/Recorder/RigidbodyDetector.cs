using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.HandGrab.Recorder
{
	public class RigidbodyDetector : MonoBehaviour
	{
		public List<Rigidbody> IntersectingBodies { get; private set; } = new List<Rigidbody>();

		public void IgnoreBody(Rigidbody body)
		{
			if (!this._ignoredBodies.Contains(body))
			{
				this._ignoredBodies.Add(body);
			}
			if (this.IntersectingBodies.Contains(body))
			{
				this.IntersectingBodies.Remove(body);
			}
		}

		public void UnIgnoreBody(Rigidbody body)
		{
			if (this._ignoredBodies.Contains(body))
			{
				this._ignoredBodies.Remove(body);
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			Rigidbody attachedRigidbody = collider.attachedRigidbody;
			if (attachedRigidbody == null || this._ignoredBodies.Contains(attachedRigidbody))
			{
				return;
			}
			if (!this.IntersectingBodies.Contains(attachedRigidbody))
			{
				this.IntersectingBodies.Add(attachedRigidbody);
			}
		}

		private void OnTriggerExit(Collider collider)
		{
			Rigidbody attachedRigidbody = collider.attachedRigidbody;
			if (attachedRigidbody == null)
			{
				return;
			}
			if (this.IntersectingBodies.Contains(attachedRigidbody))
			{
				this.IntersectingBodies.Remove(attachedRigidbody);
			}
		}

		private HashSet<Rigidbody> _ignoredBodies = new HashSet<Rigidbody>();
	}
}
