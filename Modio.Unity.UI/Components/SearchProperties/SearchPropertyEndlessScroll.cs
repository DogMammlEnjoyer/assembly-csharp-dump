using System;
using System.Collections;
using Modio.Unity.UI.Search;
using UnityEngine;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.SearchProperties
{
	[Serializable]
	public class SearchPropertyEndlessScroll : ISearchProperty, IPropertyMonoBehaviourEvents
	{
		public void OnSearchUpdate(ModioUISearch search)
		{
			this._search = search;
		}

		public void Start()
		{
			this._hasRunStart = true;
			this.OnEnable();
		}

		public void OnDestroy()
		{
		}

		public void OnEnable()
		{
			if (!this._hasRunStart)
			{
				return;
			}
			this._monitorCoroutine = this._scrollRect.StartCoroutine(this.MonitorCo());
		}

		private IEnumerator MonitorCo()
		{
			for (;;)
			{
				if (-(((RectTransform)this._scrollRect.transform).rect.height + this._scrollRect.content.offsetMin.y) < this._distanceFromBottomToLoadContent && this._search != null && this._search.CanGetMoreMods && !this._search.IsSearching)
				{
					this._search.GetNextPageAdditivelyForLastSearch();
				}
				yield return null;
			}
			yield break;
		}

		public void OnDisable()
		{
			if (this._monitorCoroutine != null)
			{
				this._scrollRect.StopCoroutine(this._monitorCoroutine);
			}
		}

		[SerializeField]
		private ScrollRect _scrollRect;

		[SerializeField]
		private float _distanceFromBottomToLoadContent = 300f;

		private Coroutine _monitorCoroutine;

		private ModioUISearch _search;

		private bool _hasRunStart;
	}
}
