using System;
using System.Collections.Generic;
using Liv.Lck.DependencyInjection;
using UnityEngine;
using UnityEngine.Events;

namespace Liv.Lck.UI
{
	public class LckQualitySelector : MonoBehaviour
	{
		public void InitializeOptions(List<QualityOption> qualityOptions)
		{
			this._qualityOptions = qualityOptions;
			int num = this._qualityOptions.FindIndex((QualityOption x) => x.IsDefault);
			if (num != -1)
			{
				this._currentQualityIndex = num;
			}
			else
			{
				this._currentQualityIndex = 0;
			}
			this.UpdateCurrentTrackDescriptor(this._currentQualityIndex);
		}

		public void GoToNextOption()
		{
			if (this._currentQualityIndex == this._qualityOptions.Count - 1)
			{
				this._currentQualityIndex = 0;
			}
			else
			{
				this._currentQualityIndex++;
			}
			this.UpdateCurrentTrackDescriptor(this._currentQualityIndex);
		}

		private void UpdateCurrentTrackDescriptor(int index)
		{
			if (this._qualityOptions.Count > index)
			{
				this._currentQualityOption = this._qualityOptions[this._currentQualityIndex];
				Action<CameraTrackDescriptor> onQualityOptionSelected = this.OnQualityOptionSelected;
				if (onQualityOptionSelected != null)
				{
					onQualityOptionSelected(this._currentQualityOption.RecordingCameraTrackDescriptor);
				}
				Action<QualityOption> onQualityOptionChanged = this.OnQualityOptionChanged;
				if (onQualityOptionChanged != null)
				{
					onQualityOptionChanged(this._currentQualityOption);
				}
				this._onQualityOptionChanged.Invoke(this._currentQualityOption.Name);
			}
		}

		public void SetQualityButtonIsDisabledState(bool state)
		{
			this._onSetQualityButtonIsDisabledState.Invoke(state);
		}

		[InjectLck]
		private ILckService _lckService;

		private QualityOption _currentQualityOption;

		private int _currentQualityIndex;

		private List<QualityOption> _qualityOptions = new List<QualityOption>();

		[Obsolete("Only provides recording parameters for the selected quality option, and does not affect  streaming - Use OnQualityOptionChanged instead")]
		public Action<CameraTrackDescriptor> OnQualityOptionSelected;

		public Action<QualityOption> OnQualityOptionChanged;

		[SerializeField]
		private UnityEvent<string> _onQualityOptionChanged;

		[SerializeField]
		private UnityEvent<bool> _onSetQualityButtonIsDisabledState;
	}
}
