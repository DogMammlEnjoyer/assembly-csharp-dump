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
	public class UserPropertyDownloadQueue : IUserProperty, IPropertyMonoBehaviourEvents
	{
		public void OnUserUpdate(UserProfile user)
		{
		}

		public void Start()
		{
		}

		public void OnDestroy()
		{
		}

		public void OnEnable()
		{
			if (this._disableIfNoOperations != null)
			{
				this._disableIfNoOperations.SetActive(false);
			}
			this.SetInstallOrDownloadState(false);
			Mod.AddChangeListener(ModChangeType.FileState, new Action<Mod, ModChangeType>(this.OnModChangeEvent));
		}

		public void OnDisable()
		{
			Mod.RemoveChangeListener(ModChangeType.FileState, new Action<Mod, ModChangeType>(this.OnModChangeEvent));
			if (this._mod != null)
			{
				this._mod.OnModUpdated -= this.OnModUpdated;
			}
		}

		private void OnModChangeEvent(Mod mod, ModChangeType modChangeType)
		{
			ModFileState state = mod.File.State;
			if (state == ModFileState.Downloading || state == ModFileState.Installing || state == ModFileState.Updating || state == ModFileState.Uninstalling)
			{
				if (this._mod != null)
				{
					this._mod.OnModUpdated -= this.OnModUpdated;
				}
				this._mod = mod;
				this._mod.OnModUpdated += this.OnModUpdated;
				if (this._disableIfNoOperations != null)
				{
					this._disableIfNoOperations.SetActive(true);
				}
				this.OnModUpdated();
				return;
			}
			if (this._mod != mod)
			{
				return;
			}
			this._completedOperationCount++;
			this._mod.OnModUpdated -= this.OnModUpdated;
			this.OnModUpdated();
			this._mod = null;
		}

		private void OnModUpdated()
		{
			float num = this._mod.File.FileStateProgress;
			bool flag = this._mod.File.State == ModFileState.None || this._mod.File.State == ModFileState.Installed;
			bool flag2 = this._mod.File.State == ModFileState.FileOperationFailed;
			if (flag)
			{
				num = 1f;
			}
			if (!flag2)
			{
				Image[] progressBars = this._progressBars;
				for (int i = 0; i < progressBars.Length; i++)
				{
					progressBars[i].fillAmount = num;
				}
				if (this._progressPercentText)
				{
					this._progressPercentText.text = string.Format("{0:P0}", num);
				}
				if (this._progressSizesText)
				{
					long num2 = (this._mod.File.State == ModFileState.Downloading) ? this._mod.File.ArchiveFileSize : this._mod.File.FileSize;
					string str = StringFormat.Bytes(StringFormatBytes.Suffix, (long)((float)num2 * num), null, true);
					string str2 = StringFormat.Bytes(StringFormatBytes.Suffix, num2, null, true);
					this._progressSizesText.text = str + " / " + str2;
				}
				if (this._speedText)
				{
					this._speedText.text = ((this._mod.File.DownloadingBytesPerSecond <= 0L) ? string.Empty : ("(" + StringFormat.Bytes(StringFormatBytes.Suffix, this._mod.File.DownloadingBytesPerSecond, null, true) + "/s)"));
				}
			}
			int pendingModOperationCount = ModInstallationManagement.PendingModOperationCount;
			if (this._mod.File.State == ModFileState.Downloading)
			{
				this.SetInstallOrDownloadState(true);
			}
			else if (this._mod.File.State == ModFileState.Installing || this._mod.File.State == ModFileState.Uninstalling || this._mod.File.State == ModFileState.Updating)
			{
				this.SetInstallOrDownloadState(false);
			}
			if (this._operationCountText != null)
			{
				this._operationCountText.text = string.Format("{0}", pendingModOperationCount);
			}
			if ((flag || flag2) && pendingModOperationCount <= 1)
			{
				this.HideAfterDelay();
			}
		}

		private void HideAfterDelay()
		{
			UserPropertyDownloadQueue.<HideAfterDelay>d__18 <HideAfterDelay>d__;
			<HideAfterDelay>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<HideAfterDelay>d__.<>4__this = this;
			<HideAfterDelay>d__.<>1__state = -1;
			<HideAfterDelay>d__.<>t__builder.Start<UserPropertyDownloadQueue.<HideAfterDelay>d__18>(ref <HideAfterDelay>d__);
		}

		private void SetInstallOrDownloadState(bool isDownloading)
		{
			if (this._showForDownloadOnly != null)
			{
				this._showForDownloadOnly.SetActive(isDownloading);
			}
			if (this._showForInstallOnly != null)
			{
				this._showForInstallOnly.SetActive(!isDownloading);
			}
		}

		[SerializeField]
		private Image[] _progressBars;

		[SerializeField]
		private TMP_Text _progressPercentText;

		[SerializeField]
		private TMP_Text _progressSizesText;

		[SerializeField]
		private TMP_Text _operationCountText;

		[SerializeField]
		private TMP_Text _speedText;

		[SerializeField]
		private GameObject _disableIfNoOperations;

		[SerializeField]
		private GameObject _showForDownloadOnly;

		[SerializeField]
		private GameObject _showForInstallOnly;

		[SerializeField]
		private float _hideAfterSecondsOfInactivity = 2f;

		private int _completedOperationCount;

		private Mod _mod;
	}
}
