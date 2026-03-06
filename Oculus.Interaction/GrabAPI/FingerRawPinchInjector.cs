using System;
using UnityEngine;

namespace Oculus.Interaction.GrabAPI
{
	public class FingerRawPinchInjector : MonoBehaviour
	{
		protected virtual void Awake()
		{
			this._handGrabAPI.InjectOptionalFingerPinchAPI(new FingerRawPinchAPI());
			this._handGrabAPI.InjectOptionalFingerGrabAPI(new FingerRawPinchAPI());
		}

		[SerializeField]
		private HandGrabAPI _handGrabAPI;
	}
}
