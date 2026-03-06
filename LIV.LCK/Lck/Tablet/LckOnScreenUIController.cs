using System;
using System.Collections.Generic;
using Liv.Lck.DependencyInjection;
using Liv.Lck.UI;
using UnityEngine;

namespace Liv.Lck.Tablet
{
	public class LckOnScreenUIController : MonoBehaviour
	{
		private void OnEnable()
		{
			this._lckService.OnRecordingStarted += this.OnRecordingStarted;
		}

		private void OnDisable()
		{
			this._lckService.OnRecordingStarted -= this.OnRecordingStarted;
			this.SetAllOnscreenButtonsState(true);
		}

		private void OnRecordingStarted(LckResult result)
		{
			if (!result.Success)
			{
				return;
			}
			this.SetAllOnscreenButtonsState(true);
		}

		public void OnNotificationStarted()
		{
			this.SetAllOnscreenButtonsState(false);
		}

		public void OnNotificationEnded()
		{
			this.SetAllOnscreenButtonsState(true);
			this.SetAllOnscreenButtonsToDefaultVisual(this._allOnscreenUI);
		}

		private void SetAllOnscreenButtonsState(bool state)
		{
			this.SetObjectsState(this._allOnscreenUI, state);
		}

		private void SetObjectsState(List<GameObject> objectList, bool state)
		{
			foreach (GameObject gameObject in objectList)
			{
				gameObject.SetActive(state);
			}
		}

		private void SetAllOnscreenButtonsToDefaultVisual(List<GameObject> objectList)
		{
			using (List<GameObject>.Enumerator enumerator = objectList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					LckScreenButton lckScreenButton;
					if (enumerator.Current.TryGetComponent<LckScreenButton>(out lckScreenButton))
					{
						lckScreenButton.SetDefaultButtonColors();
					}
				}
			}
		}

		[InjectLck]
		private ILckService _lckService;

		[SerializeField]
		private List<GameObject> _allOnscreenUI = new List<GameObject>();
	}
}
