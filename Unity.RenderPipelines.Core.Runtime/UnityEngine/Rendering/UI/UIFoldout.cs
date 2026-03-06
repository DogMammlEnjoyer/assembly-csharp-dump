using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityEngine.Rendering.UI
{
	[ExecuteAlways]
	public class UIFoldout : Toggle
	{
		protected override void Start()
		{
			base.Start();
			this.onValueChanged.AddListener(new UnityAction<bool>(this.SetState));
			this.SetState(base.isOn);
		}

		private void OnValidate()
		{
			this.SetState(base.isOn, false);
		}

		public void SetState(bool state)
		{
			this.SetState(state, true);
		}

		public void SetState(bool state, bool rebuildLayout)
		{
			if (this.arrowOpened == null || this.arrowClosed == null || this.content == null)
			{
				return;
			}
			if (this.arrowOpened.activeSelf != state)
			{
				this.arrowOpened.SetActive(state);
			}
			if (this.arrowClosed.activeSelf == state)
			{
				this.arrowClosed.SetActive(!state);
			}
			if (this.content.activeSelf != state)
			{
				this.content.SetActive(state);
			}
			if (rebuildLayout)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform.parent as RectTransform);
			}
		}

		public GameObject content;

		public GameObject arrowOpened;

		public GameObject arrowClosed;
	}
}
