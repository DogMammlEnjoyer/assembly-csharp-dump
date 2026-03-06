using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Liv.Lck.Rendering;
using UnityEngine;
using UnityEngine.Events;

namespace Liv.Lck.GorillaTag
{
	public class LckFrameController : MonoBehaviour
	{
		private void Start()
		{
			LckFrameController.<Start>d__8 <Start>d__;
			<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Start>d__.<>4__this = this;
			<Start>d__.<>1__state = -1;
			<Start>d__.<>t__builder.Start<LckFrameController.<Start>d__8>(ref <Start>d__);
		}

		private void OnDisable()
		{
			if (this._overlayButton != null)
			{
				this._overlayButton.onTapStarted.RemoveListener(new UnityAction(this.ToggleOverlay));
			}
			if (this._qckController != null)
			{
				this._qckController.OnHorizontalModeChanged -= this.OnHorizontalModeChanged;
			}
		}

		private void OnHorizontalModeChanged(bool isHorizontal)
		{
			this._compositionProfile.SetOrientation(isHorizontal);
		}

		private void SetOverlayEnabled(bool value)
		{
			this._compositionProfile.SetLayerActive(this._overlayLayerName, value);
			this._qckController.SetOverlayEnabled(value);
			this._overlayButton.IsActive = value;
		}

		private void ToggleOverlay()
		{
			this.SetOverlayEnabled(!this._overlayLayer.IsActive);
		}

		private Task LoadAndApplyTextures()
		{
			LckFrameController.<LoadAndApplyTextures>d__13 <LoadAndApplyTextures>d__;
			<LoadAndApplyTextures>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<LoadAndApplyTextures>d__.<>4__this = this;
			<LoadAndApplyTextures>d__.<>1__state = -1;
			<LoadAndApplyTextures>d__.<>t__builder.Start<LckFrameController.<LoadAndApplyTextures>d__13>(ref <LoadAndApplyTextures>d__);
			return <LoadAndApplyTextures>d__.<>t__builder.Task;
		}

		private Task<Texture> LoadTexture(string url)
		{
			LckFrameController.<LoadTexture>d__14 <LoadTexture>d__;
			<LoadTexture>d__.<>t__builder = AsyncTaskMethodBuilder<Texture>.Create();
			<LoadTexture>d__.<>1__state = -1;
			<LoadTexture>d__.<>t__builder.Start<LckFrameController.<LoadTexture>d__14>(ref <LoadTexture>d__);
			return <LoadTexture>d__.<>t__builder.Task;
		}

		[SerializeField]
		private LckCompositionProfile _compositionProfile;

		[SerializeField]
		private GTLckController _qckController;

		[SerializeField]
		private GtScreenButton _overlayButton;

		[Header("Configuration")]
		[Tooltip("The name of the Overlay Frame Layer as defined in the Composition Profile.")]
		[SerializeField]
		private string _overlayLayerName = "Overlay Frame";

		[SerializeField]
		private List<ScheduledDuration> _defaultOnSchedules;

		[Header("Runtime Texture URLs (Optional)")]
		[SerializeField]
		private string _horizontalOverlayUrl;

		[SerializeField]
		private string _verticalOverlayUrl;

		private LckOverlayFrameLayer _overlayLayer;
	}
}
