using System;
using System.Runtime.CompilerServices;
using Modio.Mods;
using Modio.Unity.UI.Components;
using UnityEngine;

namespace Modio.Unity.UI.Panels.Monetization
{
	public class ModioConfirmPurchasePanel : ModioPanelBase
	{
		protected override void Awake()
		{
			base.Awake();
			this._modioUIMod = base.GetComponent<ModioUIMod>();
		}

		public void OpenPanel(Mod mod)
		{
			base.OpenPanel();
			this._modioUIMod.SetMod(mod);
		}

		public void ConfirmPurchase()
		{
			this.ConfirmPurchaseFlow();
		}

		private void ConfirmPurchaseFlow()
		{
			ModioConfirmPurchasePanel.<ConfirmPurchaseFlow>d__5 <ConfirmPurchaseFlow>d__;
			<ConfirmPurchaseFlow>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<ConfirmPurchaseFlow>d__.<>4__this = this;
			<ConfirmPurchaseFlow>d__.<>1__state = -1;
			<ConfirmPurchaseFlow>d__.<>t__builder.Start<ModioConfirmPurchasePanel.<ConfirmPurchaseFlow>d__5>(ref <ConfirmPurchaseFlow>d__);
		}

		private ModioUIMod _modioUIMod;

		[SerializeField]
		private bool _subscribeOnPurchase = true;
	}
}
