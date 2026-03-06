using System;
using Meta.WitAi.TTS.Data;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.WitAi.TTS.LipSync
{
	public class VisemeBlendShapeLipSync : BaseVisemeBlendShapeLipSync
	{
		public override SkinnedMeshRenderer SkinnedMeshRenderer
		{
			get
			{
				return this.meshRenderer;
			}
		}

		protected override void Awake()
		{
			if (!this._lipsyncAnimator)
			{
				this._lipsyncAnimator = base.GetComponent<VisemeLipSyncAnimator>();
			}
			base.Awake();
		}

		protected virtual void OnEnable()
		{
			this._lipsyncAnimator.OnVisemeLerp.AddListener(new UnityAction<Viseme, Viseme, float>(this.OnVisemeLerp));
		}

		protected void OnDisable()
		{
			this._lipsyncAnimator.OnVisemeLerp.RemoveListener(new UnityAction<Viseme, Viseme, float>(this.OnVisemeLerp));
		}

		public SkinnedMeshRenderer meshRenderer;

		[SerializeField]
		private VisemeLipSyncAnimator _lipsyncAnimator;
	}
}
