using System;
using Meta.WitAi.TTS.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Meta.WitAi.TTS.LipSync
{
	public class VisemeTextureFlipLipSync : BaseTextureFlipLipSync
	{
		public override Renderer Renderer
		{
			get
			{
				return this.visemeRenderer;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			if (!this._lipSyncAnimator)
			{
				this._lipSyncAnimator = base.GetComponent<VisemeLipSyncAnimator>();
			}
			if (!this.visemeRenderer)
			{
				this.visemeRenderer = base.GetComponent<Renderer>();
			}
		}

		protected virtual void OnEnable()
		{
			if (!this.visemeRenderer)
			{
				VLog.E("No renderer has been set on " + base.name + ". Viseme texture flipping will not be visible.", null);
				base.enabled = false;
				return;
			}
			VisemeChangedEvent onVisemeStarted = this._lipSyncAnimator.OnVisemeStarted;
			if (onVisemeStarted == null)
			{
				return;
			}
			onVisemeStarted.AddListener(new UnityAction<Viseme>(this.OnVisemeStarted));
		}

		protected virtual void OnDisable()
		{
			VisemeChangedEvent onVisemeStarted = this._lipSyncAnimator.OnVisemeStarted;
			if (onVisemeStarted == null)
			{
				return;
			}
			onVisemeStarted.RemoveListener(new UnityAction<Viseme>(this.OnVisemeStarted));
		}

		[FormerlySerializedAs("renderer")]
		[SerializeField]
		private Renderer visemeRenderer;

		[SerializeField]
		private VisemeLipSyncAnimator _lipSyncAnimator;
	}
}
