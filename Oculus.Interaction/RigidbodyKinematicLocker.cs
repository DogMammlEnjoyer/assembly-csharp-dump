using System;
using UnityEngine;

namespace Oculus.Interaction
{
	[RequireComponent(typeof(Rigidbody))]
	public class RigidbodyKinematicLocker : MonoBehaviour
	{
		public bool IsLocked
		{
			get
			{
				return this._counter != 0;
			}
		}

		private void Awake()
		{
			this._rigidbody = base.GetComponent<Rigidbody>();
		}

		public void LockKinematic()
		{
			if (this._counter == 0)
			{
				this._savedIsKinematicState = this._rigidbody.isKinematic;
			}
			this._counter++;
			this._rigidbody.isKinematic = true;
		}

		public void UnlockKinematic()
		{
			if (this._counter == 0)
			{
				Debug.LogError("Too many calls to UnlockKinematic.Expected calls to LockKinematic to balance the kinematic state.", this);
				return;
			}
			this._counter--;
			if (this._counter == 0)
			{
				this._rigidbody.isKinematic = this._savedIsKinematicState;
			}
		}

		private Rigidbody _rigidbody;

		private int _counter;

		private bool _savedIsKinematicState;
	}
}
