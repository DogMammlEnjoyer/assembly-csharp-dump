using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Liv.Lck.GorillaTag
{
	public class OverlayController : MonoBehaviour
	{
		private void OnEnable()
		{
			this._overlayButton.onTapStarted.AddListener(new UnityAction(this.ToggleOverlay));
		}

		private void OnDisable()
		{
			this._overlayButton.onTapStarted.RemoveListener(new UnityAction(this.ToggleOverlay));
		}

		private void Start()
		{
			if (this._horizontalOverlayTexture == null || this._verticalOverlayTexture == null)
			{
				Debug.Log("Disabling overlay because of failure to load textures");
				return;
			}
			foreach (ScheduledDuration scheduledDuration in this._defaultOnScheduls)
			{
				if (scheduledDuration.IsActive())
				{
					this._isOverlayEnabled = true;
					break;
				}
			}
			this.SetOverlayEnabled(this._isOverlayEnabled);
		}

		private void OnHorizontalModeChanged(bool value)
		{
		}

		private void SetOverlayEnabled(bool value)
		{
		}

		private void ToggleOverlay()
		{
			this._isOverlayEnabled = !this._isOverlayEnabled;
			this.SetOverlayEnabled(this._isOverlayEnabled);
		}

		private Task<Texture> LoadTexture(string url)
		{
			OverlayController.<LoadTexture>d__12 <LoadTexture>d__;
			<LoadTexture>d__.<>t__builder = AsyncTaskMethodBuilder<Texture>.Create();
			<LoadTexture>d__.url = url;
			<LoadTexture>d__.<>1__state = -1;
			<LoadTexture>d__.<>t__builder.Start<OverlayController.<LoadTexture>d__12>(ref <LoadTexture>d__);
			return <LoadTexture>d__.<>t__builder.Task;
		}

		[Header("Dependencies")]
		[SerializeField]
		private GTLckController _qckController;

		[SerializeField]
		private GtScreenButton _overlayButton;

		[SerializeField]
		private List<ScheduledDuration> _defaultOnScheduls;

		[Header("Overlay Settings")]
		[SerializeField]
		private Texture _horizontalOverlayTexture;

		[SerializeField]
		private Texture _verticalOverlayTexture;

		private bool _isOverlayEnabled;
	}
}
