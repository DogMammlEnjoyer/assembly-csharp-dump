using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.GrabAPI
{
	public class HandGrabAPI : MonoBehaviour
	{
		public IHand Hand { get; private set; }

		public IHmd Hmd { get; private set; }

		protected virtual void Awake()
		{
			this.Hand = (this._hand as IHand);
			this.Hmd = (this._hmd as IHmd);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			if (this._fingerPinchGrabAPI == null)
			{
				this._fingerPinchGrabAPI = new PinchGrabAPI(this.Hmd);
			}
			if (this._fingerPalmGrabAPI == null)
			{
				this._fingerPalmGrabAPI = new PalmGrabAPI();
			}
			this.EndStart(ref this._started);
		}

		private void OnEnable()
		{
			if (this._started)
			{
				this.Hand.WhenHandUpdated += this.OnHandUpdated;
			}
		}

		private void OnDisable()
		{
			if (this._started)
			{
				this.Hand.WhenHandUpdated -= this.OnHandUpdated;
			}
		}

		private void OnHandUpdated()
		{
			this._fingerPinchGrabAPI.Update(this.Hand);
			this._fingerPalmGrabAPI.Update(this.Hand);
		}

		public HandFingerFlags HandPinchGrabbingFingers()
		{
			return this.HandGrabbingFingers(this._fingerPinchGrabAPI);
		}

		public HandFingerFlags HandPalmGrabbingFingers()
		{
			return this.HandGrabbingFingers(this._fingerPalmGrabAPI);
		}

		private HandFingerFlags HandGrabbingFingers(IFingerAPI fingerAPI)
		{
			HandFingerFlags handFingerFlags = HandFingerFlags.None;
			for (int i = 0; i < 5; i++)
			{
				HandFinger finger = (HandFinger)i;
				if (fingerAPI.GetFingerIsGrabbing(finger))
				{
					handFingerFlags |= (HandFingerFlags)(1 << i);
				}
			}
			return handFingerFlags;
		}

		public bool IsHandPinchGrabbing(in GrabbingRule fingers)
		{
			HandFingerFlags grabbingFingers = this.HandPinchGrabbingFingers();
			return this.IsSustainingGrab(fingers, grabbingFingers);
		}

		public bool IsHandPalmGrabbing(in GrabbingRule fingers)
		{
			HandFingerFlags grabbingFingers = this.HandPalmGrabbingFingers();
			return this.IsSustainingGrab(fingers, grabbingFingers);
		}

		public bool IsSustainingGrab(in GrabbingRule fingers, HandFingerFlags grabbingFingers)
		{
			bool flag = false;
			for (int i = 0; i < 5; i++)
			{
				HandFinger fingerID = (HandFinger)i;
				HandFingerFlags handFingerFlags = (HandFingerFlags)(1 << i);
				bool flag2 = (grabbingFingers & handFingerFlags) > HandFingerFlags.None;
				GrabbingRule grabbingRule = fingers;
				if (grabbingRule[fingerID] == FingerRequirement.Required)
				{
					flag = (flag || flag2);
					grabbingRule = fingers;
					if (grabbingRule.UnselectMode == FingerUnselectMode.AnyReleased && !flag2)
					{
						return false;
					}
					grabbingRule = fingers;
					if (grabbingRule.UnselectMode == FingerUnselectMode.AllReleased && flag2)
					{
						return true;
					}
				}
				else
				{
					grabbingRule = fingers;
					if (grabbingRule[fingerID] == FingerRequirement.Optional)
					{
						flag = (flag || flag2);
					}
				}
			}
			return flag;
		}

		public bool IsHandSelectPinchFingersChanged(in GrabbingRule fingers)
		{
			return this.IsHandSelectFingersChanged(fingers, this._fingerPinchGrabAPI);
		}

		public bool IsHandSelectPalmFingersChanged(in GrabbingRule fingers)
		{
			return this.IsHandSelectFingersChanged(fingers, this._fingerPalmGrabAPI);
		}

		public bool IsHandUnselectPinchFingersChanged(in GrabbingRule fingers)
		{
			return this.IsHandUnselectFingersChanged(fingers, this._fingerPinchGrabAPI);
		}

		public bool IsHandUnselectPalmFingersChanged(in GrabbingRule fingers)
		{
			return this.IsHandUnselectFingersChanged(fingers, this._fingerPalmGrabAPI);
		}

		private bool IsHandSelectFingersChanged(in GrabbingRule fingers, IFingerAPI fingerAPI)
		{
			GrabbingRule grabbingRule = fingers;
			bool selectsWithOptionals = grabbingRule.SelectsWithOptionals;
			bool result = false;
			for (int i = 0; i < 5; i++)
			{
				HandFinger handFinger = (HandFinger)i;
				grabbingRule = fingers;
				if (grabbingRule[handFinger] == FingerRequirement.Required)
				{
					if (!fingerAPI.GetFingerIsGrabbing(handFinger))
					{
						return false;
					}
					if (fingerAPI.GetFingerIsGrabbingChanged(handFinger, true))
					{
						result = true;
					}
				}
				else if (selectsWithOptionals)
				{
					grabbingRule = fingers;
					if (grabbingRule[handFinger] == FingerRequirement.Optional && fingerAPI.GetFingerIsGrabbingChanged(handFinger, true))
					{
						return true;
					}
				}
			}
			return result;
		}

		private bool IsHandUnselectFingersChanged(in GrabbingRule fingers, IFingerAPI fingerAPI)
		{
			bool flag = false;
			bool flag2 = false;
			GrabbingRule grabbingRule = fingers;
			bool selectsWithOptionals = grabbingRule.SelectsWithOptionals;
			for (int i = 0; i < 5; i++)
			{
				HandFinger handFinger = (HandFinger)i;
				grabbingRule = fingers;
				if (grabbingRule[handFinger] != FingerRequirement.Ignored)
				{
					flag |= fingerAPI.GetFingerIsGrabbing(handFinger);
					grabbingRule = fingers;
					if (grabbingRule[handFinger] == FingerRequirement.Required)
					{
						if (fingerAPI.GetFingerIsGrabbingChanged(handFinger, false))
						{
							flag2 = true;
							grabbingRule = fingers;
							if (grabbingRule.UnselectMode == FingerUnselectMode.AnyReleased)
							{
								return true;
							}
						}
					}
					else
					{
						grabbingRule = fingers;
						if (grabbingRule[handFinger] == FingerRequirement.Optional && fingerAPI.GetFingerIsGrabbingChanged(handFinger, false))
						{
							flag2 = true;
							grabbingRule = fingers;
							if (grabbingRule.UnselectMode == FingerUnselectMode.AnyReleased && selectsWithOptionals)
							{
								return true;
							}
						}
					}
				}
			}
			return !flag && flag2;
		}

		public Vector3 GetPinchCenter()
		{
			Vector3 localOffset = Vector3.zero;
			if (this._fingerPinchGrabAPI != null)
			{
				localOffset = this._fingerPinchGrabAPI.GetWristOffsetLocal();
			}
			return this.WristOffsetToWorldPoint(localOffset);
		}

		public Vector3 GetPalmCenter()
		{
			Vector3 localOffset = Vector3.zero;
			if (this._fingerPalmGrabAPI != null)
			{
				localOffset = this._fingerPalmGrabAPI.GetWristOffsetLocal();
			}
			return this.WristOffsetToWorldPoint(localOffset);
		}

		private Vector3 WristOffsetToWorldPoint(Vector3 localOffset)
		{
			Pose pose;
			if (!this.Hand.GetJointPose(HandJointId.HandWristRoot, out pose))
			{
				return localOffset * this.Hand.Scale;
			}
			return pose.position + pose.rotation * localOffset * this.Hand.Scale;
		}

		public float GetHandPinchScore(in GrabbingRule fingers, bool includePinching = true)
		{
			return this.GetHandGrabScore(fingers, includePinching, this._fingerPinchGrabAPI);
		}

		public float GetHandPalmScore(in GrabbingRule fingers, bool includeGrabbing = true)
		{
			return this.GetHandGrabScore(fingers, includeGrabbing, this._fingerPalmGrabAPI);
		}

		public float GetFingerPinchStrength(HandFinger finger)
		{
			return this._fingerPinchGrabAPI.GetFingerGrabScore(finger);
		}

		public float GetFingerPinchPercent(HandFinger finger)
		{
			if (this._fingerPinchGrabAPI is FingerPinchGrabAPI)
			{
				return (this._fingerPinchGrabAPI as FingerPinchGrabAPI).GetFingerPinchPercent(finger);
			}
			Debug.LogWarning("GetFingerPinchPercent is not applicable");
			return -1f;
		}

		public float GetFingerPinchDistance(HandFinger finger)
		{
			if (this._fingerPinchGrabAPI is FingerPinchGrabAPI)
			{
				return (this._fingerPinchGrabAPI as FingerPinchGrabAPI).GetFingerPinchDistance(finger);
			}
			Debug.LogWarning("GetFingerPinchDistance is not applicable");
			return -1f;
		}

		public float GetFingerPalmStrength(HandFinger finger)
		{
			return this._fingerPalmGrabAPI.GetFingerGrabScore(finger);
		}

		private float GetHandGrabScore(in GrabbingRule fingers, bool includeGrabbing, IFingerAPI fingerAPI)
		{
			float num = 1f;
			float num2 = 0f;
			bool flag = false;
			GrabbingRule grabbingRule = fingers;
			bool selectsWithOptionals = grabbingRule.SelectsWithOptionals;
			for (int i = 0; i < 5; i++)
			{
				HandFinger handFinger = (HandFinger)i;
				if (includeGrabbing || !fingerAPI.GetFingerIsGrabbing((HandFinger)i))
				{
					grabbingRule = fingers;
					if (grabbingRule[handFinger] != FingerRequirement.Ignored)
					{
						grabbingRule = fingers;
						if (grabbingRule[handFinger] == FingerRequirement.Optional)
						{
							num2 = Mathf.Max(num2, fingerAPI.GetFingerGrabScore(handFinger));
						}
						else
						{
							grabbingRule = fingers;
							if (grabbingRule[handFinger] == FingerRequirement.Required)
							{
								flag = true;
								num = Mathf.Min(num, fingerAPI.GetFingerGrabScore(handFinger));
							}
						}
					}
				}
			}
			if (selectsWithOptionals)
			{
				return num2;
			}
			if (!flag)
			{
				return 0f;
			}
			return num;
		}

		public void SetPinchGrabParam(PinchGrabParam paramId, float paramVal)
		{
			FingerPinchGrabAPI fingerPinchGrabAPI = this._fingerPinchGrabAPI as FingerPinchGrabAPI;
			if (fingerPinchGrabAPI != null)
			{
				fingerPinchGrabAPI.SetPinchGrabParam(paramId, paramVal);
			}
		}

		public float GetPinchGrabParam(PinchGrabParam paramId)
		{
			FingerPinchGrabAPI fingerPinchGrabAPI = this._fingerPinchGrabAPI as FingerPinchGrabAPI;
			if (fingerPinchGrabAPI != null)
			{
				return fingerPinchGrabAPI.GetPinchGrabParam(paramId);
			}
			return 0f;
		}

		public bool GetFingerIsGrabbing(HandFinger finger)
		{
			return this._fingerPinchGrabAPI.GetFingerIsGrabbing(finger);
		}

		public bool GetFingerIsPalmGrabbing(HandFinger finger)
		{
			return this._fingerPalmGrabAPI.GetFingerIsGrabbing(finger);
		}

		public void InjectAllHandGrabAPI(IHand hand)
		{
			this.InjectHand(hand);
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		public void InjectOptionalHmd(IHmd hmd)
		{
			this.Hmd = hmd;
			this._hmd = (hmd as Object);
		}

		public void InjectOptionalFingerPinchAPI(IFingerAPI fingerPinchAPI)
		{
			this._fingerPinchGrabAPI = fingerPinchAPI;
		}

		public void InjectOptionalFingerGrabAPI(IFingerAPI fingerGrabAPI)
		{
			this._fingerPalmGrabAPI = fingerGrabAPI;
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		[SerializeField]
		[Interface(typeof(IHmd), new Type[]
		{

		})]
		[Optional]
		private Object _hmd;

		private IFingerAPI _fingerPinchGrabAPI;

		private IFingerAPI _fingerPalmGrabAPI;

		private bool _started;
	}
}
