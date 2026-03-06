using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.Extensions;
using Modio.Monetization;
using UnityEngine;

namespace Modio.Unity.UI.Components
{
	public class ModioUITokenPurchase : MonoBehaviour
	{
		private void Start()
		{
			this._referencePack.gameObject.SetActive(false);
			ModioClient.OnInitialized += this.OnPluginInitialized;
		}

		private void OnDestroy()
		{
			ModioClient.OnInitialized -= this.OnPluginInitialized;
		}

		private void OnPluginInitialized()
		{
			this.GetCurrencyPacks().ForgetTaskSafely();
		}

		private Task GetCurrencyPacks()
		{
			ModioUITokenPurchase.<GetCurrencyPacks>d__5 <GetCurrencyPacks>d__;
			<GetCurrencyPacks>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<GetCurrencyPacks>d__.<>4__this = this;
			<GetCurrencyPacks>d__.<>1__state = -1;
			<GetCurrencyPacks>d__.<>t__builder.Start<ModioUITokenPurchase.<GetCurrencyPacks>d__5>(ref <GetCurrencyPacks>d__);
			return <GetCurrencyPacks>d__.<>t__builder.Task;
		}

		private void ShowTokenPacks(PortalSku[] sku)
		{
			foreach (PortalSku pack in sku)
			{
				ModioUITokenPack modioUITokenPack;
				if (this._currentPacks.Count > 0)
				{
					modioUITokenPack = Object.Instantiate<ModioUITokenPack>(this._referencePack, this._referencePack.transform.parent);
				}
				else
				{
					modioUITokenPack = this._referencePack;
					this._referencePack.gameObject.SetActive(true);
				}
				modioUITokenPack.SetPack(pack);
				this._currentPacks.Add(modioUITokenPack);
			}
			if (this._currentPacks.Count == 0)
			{
				Debug.LogError("Unable to find any token packs for the current platform. They must be setup on the Game Admin Settings ");
				this._referencePack.gameObject.SetActive(false);
			}
		}

		[SerializeField]
		private ModioUITokenPack _referencePack;

		private readonly List<ModioUITokenPack> _currentPacks = new List<ModioUITokenPack>();
	}
}
