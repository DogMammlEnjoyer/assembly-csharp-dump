using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class RenderModelChangerUI : UIElement
	{
		protected override void Awake()
		{
			base.Awake();
			this.ui = base.GetComponentInParent<SkeletonUIOptions>();
		}

		protected override void OnButtonClick()
		{
			base.OnButtonClick();
			if (this.ui != null)
			{
				this.ui.SetRenderModel(this);
			}
		}

		public GameObject leftPrefab;

		public GameObject rightPrefab;

		protected SkeletonUIOptions ui;
	}
}
