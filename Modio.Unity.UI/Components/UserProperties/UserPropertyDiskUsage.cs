using System;
using System.Runtime.CompilerServices;
using Modio.Mods;
using Modio.Users;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.UserProperties
{
	[Serializable]
	public class UserPropertyDiskUsage : IUserProperty, IPropertyMonoBehaviourEvents
	{
		public void Start()
		{
			if (this._text != null)
			{
				this._text.text = "";
			}
		}

		public void OnDestroy()
		{
		}

		public void OnEnable()
		{
			ModioClient.OnInitialized += this.UpdateUsage;
			Mod.AddChangeListener(ModChangeType.FileState, new Action<Mod, ModChangeType>(this.OnModFileStateChanged));
		}

		public void OnDisable()
		{
			ModioClient.OnInitialized -= this.UpdateUsage;
			Mod.RemoveChangeListener(ModChangeType.FileState, new Action<Mod, ModChangeType>(this.OnModFileStateChanged));
		}

		private void OnModFileStateChanged(Mod _, ModChangeType __)
		{
			this.UpdateUsage();
		}

		public void OnUserUpdate(UserProfile user)
		{
			if (!this._isUpdatingUsage)
			{
				this.UpdateUsage();
			}
		}

		private void UpdateUsage()
		{
			UserPropertyDiskUsage.<UpdateUsage>d__11 <UpdateUsage>d__;
			<UpdateUsage>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<UpdateUsage>d__.<>4__this = this;
			<UpdateUsage>d__.<>1__state = -1;
			<UpdateUsage>d__.<>t__builder.Start<UserPropertyDiskUsage.<UpdateUsage>d__11>(ref <UpdateUsage>d__);
		}

		[SerializeField]
		private TMP_Text _text;

		[SerializeField]
		private Image _fillImage;

		[SerializeField]
		private GameObject _enableIfAvailableSpaceSupported;

		[SerializeField]
		private GameObject _disableIfAvailableSpaceSupported;

		private bool _isUpdatingUsage;
	}
}
