using System;
using Modio.Errors;
using Modio.Mods;
using Modio.Unity.UI.Components.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.ModProperties
{
	[Serializable]
	public class ModPropertyFileOperations : IModProperty
	{
		public void OnModUpdate(Mod mod)
		{
			ModPropertyFileOperations.Operation operation;
			switch (mod.File.State)
			{
			case ModFileState.Queued:
				operation = ModPropertyFileOperations.Operation.Queued;
				goto IL_66;
			case ModFileState.Downloading:
				operation = ModPropertyFileOperations.Operation.Downloading;
				goto IL_66;
			case ModFileState.Installing:
				operation = ModPropertyFileOperations.Operation.Installing;
				goto IL_66;
			case ModFileState.Installed:
				operation = (mod.IsSubscribed ? ModPropertyFileOperations.Operation.Installed : ModPropertyFileOperations.Operation.InstalledByOtherUser);
				goto IL_66;
			case ModFileState.Updating:
				operation = ModPropertyFileOperations.Operation.Updating;
				goto IL_66;
			case ModFileState.Uninstalling:
				operation = ModPropertyFileOperations.Operation.Uninstalling;
				goto IL_66;
			case ModFileState.FileOperationFailed:
				operation = ModPropertyFileOperations.Operation.FileOperationFailed;
				goto IL_66;
			}
			operation = ModPropertyFileOperations.Operation.None;
			IL_66:
			ModPropertyFileOperations.Operation operation2 = operation;
			bool flag = operation2 != ModPropertyFileOperations.Operation.None && this._operations.HasFlag(operation2);
			if (this._noOperationActive != null)
			{
				this._noOperationActive.gameObject.SetActive(!flag);
			}
			if (this._operationActive != null)
			{
				this._operationActive.gameObject.SetActive(flag);
			}
			if (!flag)
			{
				return;
			}
			if (this._operationName != null)
			{
				this._operationName.text = operation2.ToString();
			}
			if (this._operationNameLocalised != null)
			{
				string text;
				if (operation2 <= ModPropertyFileOperations.Operation.Updating)
				{
					switch (operation2)
					{
					case ModPropertyFileOperations.Operation.None:
						text = "";
						goto IL_1C4;
					case ModPropertyFileOperations.Operation.Queued:
						text = "modio_modstate_queued";
						goto IL_1C4;
					case ModPropertyFileOperations.Operation.Downloading:
						text = "modio_modstate_downloading";
						goto IL_1C4;
					case ModPropertyFileOperations.Operation.Queued | ModPropertyFileOperations.Operation.Downloading:
					case ModPropertyFileOperations.Operation.Queued | ModPropertyFileOperations.Operation.Installing:
					case ModPropertyFileOperations.Operation.Downloading | ModPropertyFileOperations.Operation.Installing:
					case ModPropertyFileOperations.Operation.Queued | ModPropertyFileOperations.Operation.Downloading | ModPropertyFileOperations.Operation.Installing:
						break;
					case ModPropertyFileOperations.Operation.Installing:
						text = "modio_modstate_installing";
						goto IL_1C4;
					case ModPropertyFileOperations.Operation.Installed:
						text = "modio_modstate_installed";
						goto IL_1C4;
					default:
						if (operation2 == ModPropertyFileOperations.Operation.Updating)
						{
							text = "modio_modstate_updating";
							goto IL_1C4;
						}
						break;
					}
				}
				else
				{
					if (operation2 == ModPropertyFileOperations.Operation.Uninstalling)
					{
						text = "modio_modstate_uninstalling";
						goto IL_1C4;
					}
					if (operation2 == ModPropertyFileOperations.Operation.FileOperationFailed)
					{
						text = ((mod.File.FileStateErrorCause.Code == ErrorCode.INSUFFICIENT_SPACE) ? "modio_error_storage_header" : "modio_modstate_error");
						goto IL_1C4;
					}
					if (operation2 == ModPropertyFileOperations.Operation.InstalledByOtherUser)
					{
						text = "modio_modstate_installed";
						goto IL_1C4;
					}
				}
				throw new ArgumentOutOfRangeException();
				IL_1C4:
				string key = text;
				this._operationNameLocalised.SetKey(key);
			}
			if (this._progressPercent != null)
			{
				this._progressPercent.text = mod.File.FileStateProgress.ToString("P0", ModioUILocalizationManager.CultureInfo);
			}
			if (this._progressFill != null)
			{
				this._progressFill.fillAmount = (this._invertProgressFill ? (1f - mod.File.FileStateProgress) : mod.File.FileStateProgress);
			}
			if (this._downloadSpeed != null)
			{
				bool flag2 = operation2 == ModPropertyFileOperations.Operation.Downloading;
				if (flag2)
				{
					this._downloadSpeed.text = StringFormat.BytesSuffix(mod.File.DownloadingBytesPerSecond, true) + "/s";
				}
				this._downloadSpeed.gameObject.SetActive(flag2);
			}
		}

		[SerializeField]
		private ModPropertyFileOperations.Operation _operations = ~ModPropertyFileOperations.Operation.Installed;

		[Space]
		[SerializeField]
		private GameObject _noOperationActive;

		[SerializeField]
		private GameObject _operationActive;

		[Space]
		[SerializeField]
		private TMP_Text _operationName;

		[SerializeField]
		private ModioUILocalizedText _operationNameLocalised;

		[SerializeField]
		private TMP_Text _progressPercent;

		[SerializeField]
		private Image _progressFill;

		[SerializeField]
		private bool _invertProgressFill;

		[SerializeField]
		private TMP_Text _downloadSpeed;

		[Flags]
		private enum Operation
		{
			None = 0,
			Queued = 1,
			Downloading = 2,
			Installing = 4,
			Installed = 8,
			Updating = 16,
			Uninstalling = 32,
			FileOperationFailed = 64,
			InstalledByOtherUser = 128
		}
	}
}
