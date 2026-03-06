using System;
using Meta.WitAi.Data;
using Meta.WitAi.Json;
using Meta.WitAi.Requests;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Meta.WitAi.CallbackHandlers
{
	public abstract class WitResponseHandler : MonoBehaviour
	{
		private void OnValidate()
		{
			if (!this.Voice)
			{
				this.Voice = Object.FindAnyObjectByType<VoiceService>();
			}
		}

		protected virtual void OnEnable()
		{
			if (!this.Voice)
			{
				this.Voice = Object.FindAnyObjectByType<VoiceService>();
			}
			if (!this.Voice)
			{
				VLog.E("VoiceService not found in scene.\nDisabling " + base.GetType().Name + " on " + base.gameObject.name, null);
				base.enabled = false;
				return;
			}
			this.Voice.VoiceEvents.OnSend.AddListener(new UnityAction<VoiceServiceRequest>(this.OnRequestSend));
			this.Voice.VoiceEvents.OnValidatePartialResponse.AddListener(new UnityAction<VoiceSession>(this.HandleValidateEarlyResponse));
			this.Voice.VoiceEvents.OnResponse.AddListener(new UnityAction<WitResponseNode>(this.HandleFinalResponse));
		}

		protected virtual void OnDisable()
		{
			if (this.Voice)
			{
				this.Voice.VoiceEvents.OnSend.RemoveListener(new UnityAction<VoiceServiceRequest>(this.OnRequestSend));
				this.Voice.VoiceEvents.OnValidatePartialResponse.RemoveListener(new UnityAction<VoiceSession>(this.HandleValidateEarlyResponse));
				this.Voice.VoiceEvents.OnResponse.RemoveListener(new UnityAction<WitResponseNode>(this.HandleFinalResponse));
			}
		}

		protected virtual void OnRequestSend(VoiceServiceRequest request)
		{
			this._validated = false;
		}

		protected virtual void HandleValidateEarlyResponse(VoiceSession session)
		{
			if (this.ValidateEarly && !this._validated && string.IsNullOrEmpty(this.OnValidateResponse(session.response, true)))
			{
				this._validated = true;
				this.OnResponseSuccess(session.response);
				session.validResponse = true;
			}
		}

		protected virtual void HandleFinalResponse(WitResponseNode response)
		{
			if (!this._validated)
			{
				string text = this.OnValidateResponse(response, false);
				if (!string.IsNullOrEmpty(text))
				{
					this.OnResponseInvalid(response, text);
				}
				else
				{
					this.OnResponseSuccess(response);
				}
				this._validated = true;
			}
		}

		protected abstract string OnValidateResponse(WitResponseNode response, bool isEarlyResponse);

		protected abstract void OnResponseInvalid(WitResponseNode response, string error);

		protected abstract void OnResponseSuccess(WitResponseNode response);

		public static bool RefreshConfidenceRange(float confidence, ConfidenceRange[] confidenceRanges, bool allowConfidenceOverlap)
		{
			bool flag = false;
			bool flag2 = false;
			int num = 0;
			while (confidenceRanges != null && num < confidenceRanges.Length)
			{
				ConfidenceRange confidenceRange = confidenceRanges[num];
				if (confidence >= confidenceRange.minConfidence && confidence <= confidenceRange.maxConfidence)
				{
					if (allowConfidenceOverlap || !flag)
					{
						UnityEvent onWithinConfidenceRange = confidenceRange.onWithinConfidenceRange;
						if (onWithinConfidenceRange != null)
						{
							onWithinConfidenceRange.Invoke();
						}
						flag = true;
					}
				}
				else if (allowConfidenceOverlap || !flag2)
				{
					UnityEvent onOutsideConfidenceRange = confidenceRange.onOutsideConfidenceRange;
					if (onOutsideConfidenceRange != null)
					{
						onOutsideConfidenceRange.Invoke();
					}
					flag2 = true;
				}
				num++;
			}
			return flag;
		}

		[FormerlySerializedAs("wit")]
		[SerializeField]
		public VoiceService Voice;

		[SerializeField]
		public bool ValidateEarly;

		private bool _validated;
	}
}
