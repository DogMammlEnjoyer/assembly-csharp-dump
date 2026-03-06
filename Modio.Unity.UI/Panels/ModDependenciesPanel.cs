using System;
using System.Runtime.CompilerServices;
using Modio.Mods;
using Modio.Unity.UI.Components;
using UnityEngine;

namespace Modio.Unity.UI.Panels
{
	public class ModDependenciesPanel : ModioPanelBase
	{
		protected override void Awake()
		{
			base.Awake();
			this._modioUIMod = base.GetComponent<ModioUIMod>();
			this.IsSubscribeFlow(false);
		}

		public void OpenPanel(ModioUIMod mod)
		{
			this.OpenPanel(mod.Mod);
		}

		public void OpenPanel(Mod mod)
		{
			base.OpenPanel();
			this._modioUIMod.SetMod(mod);
		}

		public void IsSubscribeFlow(bool isSubscribe)
		{
			GameObject[] array = this._showForSubscribeFlowOnly;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(isSubscribe);
			}
			array = this._hideForSubscribeFlow;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(!isSubscribe);
			}
		}

		public void ConfirmPressed()
		{
			this.SubscribeWithDependenciesAndHandleResult();
		}

		private void SubscribeWithDependenciesAndHandleResult()
		{
			ModDependenciesPanel.<SubscribeWithDependenciesAndHandleResult>d__8 <SubscribeWithDependenciesAndHandleResult>d__;
			<SubscribeWithDependenciesAndHandleResult>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<SubscribeWithDependenciesAndHandleResult>d__.<>4__this = this;
			<SubscribeWithDependenciesAndHandleResult>d__.<>1__state = -1;
			<SubscribeWithDependenciesAndHandleResult>d__.<>t__builder.Start<ModDependenciesPanel.<SubscribeWithDependenciesAndHandleResult>d__8>(ref <SubscribeWithDependenciesAndHandleResult>d__);
		}

		private ModioUIMod _modioUIMod;

		[SerializeField]
		private GameObject[] _showForSubscribeFlowOnly;

		[SerializeField]
		private GameObject[] _hideForSubscribeFlow;
	}
}
