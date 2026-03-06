using System;
using TMPro;
using UnityEngine;

namespace Oculus.Interaction
{
	[Obsolete("Use VersionTextVisual instead")]
	public class VersionTextGUI : MonoBehaviour
	{
		protected virtual void Reset()
		{
			this._text = base.GetComponent<TextMeshProUGUI>();
		}

		protected virtual void Start()
		{
			this._text.text = "Version: " + Application.version;
		}

		public void InjectAllVersionTextGuiVisual(TextMeshProUGUI text)
		{
			this.InjectTextUI(text);
		}

		public void InjectTextUI(TextMeshProUGUI text)
		{
			this._text = text;
		}

		[SerializeField]
		private TextMeshProUGUI _text;

		protected bool _started;
	}
}
