using System;
using Meta.WitAi.Attributes;
using Meta.WitAi.TTS.Interfaces;
using UnityEngine;

namespace Meta.WitAi.TTS.Utilities
{
	public class TTSSpeakerBodyAnimator : MonoBehaviour
	{
		public ISpeaker Speaker
		{
			get
			{
				return this._speaker as ISpeaker;
			}
		}

		protected virtual void Awake()
		{
			if (this.Speaker == null)
			{
				this._speaker = base.gameObject.GetComponentInChildren(typeof(ISpeaker));
			}
			if (this.Animator == null)
			{
				this.Animator = base.gameObject.GetComponentInChildren<Animator>();
			}
		}

		private void Update()
		{
			this.RefreshPausing();
			this.RefreshSpeaking();
		}

		public void RefreshSpeaking()
		{
			bool flag = this.Speaker != null && this.Speaker.IsSpeaking;
			if (this._speaking == flag)
			{
				return;
			}
			this._speaking = flag;
			if (this.Animator != null)
			{
				this.Animator.SetBool(this.AnimatorSpeakKey, this._speaking);
			}
		}

		public void RefreshPausing()
		{
			bool flag = this.Speaker != null && this.Speaker.IsPaused;
			if (this._pausing == flag)
			{
				return;
			}
			this._pausing = flag;
			if (this.Animator != null)
			{
				this.Animator.speed = (this._pausing ? 0f : 1f);
			}
		}

		[SerializeField]
		[ObjectType(typeof(ISpeaker), new Type[]
		{

		})]
		private Object _speaker;

		public Animator Animator;

		[DropDown("GetAnimatorKeys", false, false, true, true, null, false)]
		public string AnimatorSpeakKey = "SPEAKING";

		private bool _speaking;

		private bool _pausing;
	}
}
