using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components
{
	public class ModioUIMaximumHeight : MonoBehaviour, ILayoutElement
	{
		public float minWidth
		{
			get
			{
				return -1f;
			}
		}

		public float preferredWidth
		{
			get
			{
				return -1f;
			}
		}

		public float flexibleWidth
		{
			get
			{
				return -1f;
			}
		}

		public float minHeight
		{
			get
			{
				return -1f;
			}
		}

		public float preferredHeight
		{
			get
			{
				if (!this._isRestrictingHeight)
				{
					return -1f;
				}
				return this._restrictHeightTo;
			}
		}

		public float flexibleHeight
		{
			get
			{
				return -1f;
			}
		}

		public int layoutPriority
		{
			get
			{
				return 10;
			}
		}

		private void Awake()
		{
			if (this._expandAnyway != null)
			{
				this._expandAnyway.onValueChanged.AddListener(new UnityAction<bool>(this.OnExpandAnywayChanged));
			}
			this._graphic = base.GetComponent<Graphic>();
			if (this._graphic != null)
			{
				this._graphic.RegisterDirtyLayoutCallback(new UnityAction(this.GraphicLayoutDirty));
			}
		}

		private void OnDestroy()
		{
			if (this._graphic != null)
			{
				this._graphic.UnregisterDirtyLayoutCallback(new UnityAction(this.GraphicLayoutDirty));
			}
		}

		private void OnEnable()
		{
			if (this._expandAnyway != null)
			{
				this._expandAnyway.isOn = false;
			}
		}

		private void OnExpandAnywayChanged(bool isExpanded)
		{
			this.SetDirty();
		}

		public void CalculateLayoutInputHorizontal()
		{
		}

		public void CalculateLayoutInputVertical()
		{
			this.RecalculateRestrictingHeight(true);
		}

		private void GraphicLayoutDirty()
		{
			this.RecalculateRestrictingHeight(false);
		}

		private void RecalculateRestrictingHeight(bool delayButtonActivation)
		{
			this._isRestrictingHeight = false;
			this._isRestrictingHeight = (LayoutUtility.GetPreferredHeight((RectTransform)base.transform) > this._restrictHeightTo);
			if (this._expandAnyway != null || this._showWhenRestrictingHeight != null)
			{
				if (delayButtonActivation)
				{
					base.StartCoroutine(this.SetButtonsActiveDelayed(this._isRestrictingHeight));
				}
				else
				{
					this.SetButtonsActive(this._isRestrictingHeight);
				}
				if (this._expandAnyway != null && this._expandAnyway.isOn)
				{
					this._isRestrictingHeight = false;
				}
			}
		}

		private IEnumerator SetButtonsActiveDelayed(bool shouldBeVisible)
		{
			yield return new WaitForEndOfFrame();
			this.SetButtonsActive(shouldBeVisible);
			yield break;
		}

		private void SetButtonsActive(bool shouldBeVisible)
		{
			if (this._expandAnyway != null)
			{
				this._expandAnyway.gameObject.SetActive(shouldBeVisible);
			}
			if (this._showWhenRestrictingHeight != null)
			{
				this._showWhenRestrictingHeight.SetActive(shouldBeVisible);
			}
		}

		private void SetDirty()
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			LayoutRebuilder.MarkLayoutForRebuild(base.transform as RectTransform);
		}

		[SerializeField]
		private float _restrictHeightTo = 100f;

		[SerializeField]
		private Toggle _expandAnyway;

		[SerializeField]
		private GameObject _showWhenRestrictingHeight;

		private bool _isRestrictingHeight;

		private Graphic _graphic;
	}
}
