using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.Extensions;
using Modio.Unity.UI.Components.Localization;
using Modio.Unity.UI.Input;
using Modio.Unity.UI.Navigation;
using Modio.Unity.UI.Search;
using Modio.Users;
using UnityEngine;
using UnityEngine.Events;

namespace Modio.Unity.UI.Panels
{
	public class ModBrowserPanel : ModioPanelBase
	{
		protected override void Start()
		{
			base.Start();
			if (!ModioUILocalizationManager.LocalizationExists)
			{
				Debug.LogWarning("Your scene doesn't appear to have a ModioUILocalizationManager or custom localization handler. Consider adding the 'ModioUI_Localisation' prefab to your scene");
			}
		}

		public override void OnGainedFocus(ModioPanelBase.GainedFocusCause selectionBehaviour)
		{
			ModioUIInput.AddHandler(ModioUIInput.ModioAction.Search, new Action(this.OpenSearch));
			ModioUIInput.AddHandler(ModioUIInput.ModioAction.Filter, new Action(this.OpenFilter));
			ModioUIInput.AddHandler(ModioUIInput.ModioAction.Sort, new Action(this.OpenSort));
			ModioUISearch.Default.OnSearchUpdatedUnityEvent.AddListener(new UnityAction(this.HookUpCancelOrClearFilter));
			base.OnGainedFocus(selectionBehaviour);
			this.HookUpCancelOrClearFilter();
			if (!ModioClient.IsInitialized || User.Current == null || !User.Current.IsAuthenticated)
			{
				if (ModBrowserPanel._isWaitingBeforeAuthFlow)
				{
					return;
				}
				if (selectionBehaviour == ModioPanelBase.GainedFocusCause.RegainingFocusFromStackedPanel)
				{
					ModioLog message = ModioLog.Message;
					if (message != null)
					{
						message.Log("Closing ModBrowserPanel after regaining focus from cancelled login attempt");
					}
					base.ClosePanel();
					return;
				}
				this.OpenAuthFlowAfterWaitingIfNeeded().ForgetTaskSafely();
			}
			if (selectionBehaviour == ModioPanelBase.GainedFocusCause.OpeningFromClosed)
			{
				this._openingPanelFromClosed.Invoke();
			}
		}

		private Task OpenAuthFlowAfterWaitingIfNeeded()
		{
			ModBrowserPanel.<OpenAuthFlowAfterWaitingIfNeeded>d__5 <OpenAuthFlowAfterWaitingIfNeeded>d__;
			<OpenAuthFlowAfterWaitingIfNeeded>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<OpenAuthFlowAfterWaitingIfNeeded>d__.<>1__state = -1;
			<OpenAuthFlowAfterWaitingIfNeeded>d__.<>t__builder.Start<ModBrowserPanel.<OpenAuthFlowAfterWaitingIfNeeded>d__5>(ref <OpenAuthFlowAfterWaitingIfNeeded>d__);
			return <OpenAuthFlowAfterWaitingIfNeeded>d__.<>t__builder.Task;
		}

		public override void OnLostFocus()
		{
			ModioUIInput.RemoveHandler(ModioUIInput.ModioAction.Search, new Action(this.OpenSearch));
			ModioUIInput.RemoveHandler(ModioUIInput.ModioAction.Filter, new Action(this.OpenFilter));
			ModioUIInput.RemoveHandler(ModioUIInput.ModioAction.Sort, new Action(this.OpenSort));
			ModioUIInput.RemoveHandler(ModioUIInput.ModioAction.SearchClear, new Action(this.ClearSearch));
			ModioUIInput.RemoveHandler(ModioUIInput.ModioAction.Cancel, new Action(this.CancelPressed));
			if (ModioUISearch.Default != null)
			{
				ModioUISearch.Default.OnSearchUpdatedUnityEvent.RemoveListener(new UnityAction(this.HookUpCancelOrClearFilter));
			}
			base.OnLostFocus();
		}

		private void HookUpCancelOrClearFilter()
		{
			if (!base.HasFocus)
			{
				return;
			}
			ModioUIInput.RemoveHandler(ModioUIInput.ModioAction.Cancel, new Action(this.CancelPressed));
			ModioUIInput.RemoveHandler(ModioUIInput.ModioAction.SearchClear, new Action(this.ClearSearch));
			if (ModioUISearch.Default.HasCustomSearch())
			{
				ModioUIInput.AddHandler(ModioUIInput.ModioAction.SearchClear, new Action(this.ClearSearch));
				return;
			}
			ModioUIInput.AddHandler(ModioUIInput.ModioAction.Cancel, new Action(this.CancelPressed));
		}

		private void OpenSearch()
		{
			if (this._searchField != null)
			{
				this._searchField.SelectInputField();
			}
		}

		private void ClearSearch()
		{
			ModioUISearch.Default.ClearSearch();
		}

		private void OpenFilter()
		{
			ModFilterPanel panelOfType = ModioPanelManager.GetPanelOfType<ModFilterPanel>();
			if (panelOfType == null)
			{
				return;
			}
			panelOfType.OpenPanel();
		}

		private void OpenSort()
		{
			ModSortPanel panelOfType = ModioPanelManager.GetPanelOfType<ModSortPanel>();
			if (panelOfType == null)
			{
				return;
			}
			panelOfType.OpenPanel();
		}

		[SerializeField]
		private ModioInputFieldSelectionWrapper _searchField;

		[SerializeField]
		private UnityEvent _openingPanelFromClosed;

		private static bool _isWaitingBeforeAuthFlow;
	}
}
