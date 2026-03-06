using System;
using TMPro;
using UnityEngine;

namespace Oculus.Interaction
{
	public class VersionTextVisual : MonoBehaviour
	{
		protected virtual void Reset()
		{
			this._text = base.GetComponent<TMP_Text>();
		}

		protected virtual void Start()
		{
			this._text.text = string.Format(this._format, Application.version);
		}

		public void InjectAllVersionTextVisual(TMP_Text text, string format)
		{
			this.InjectText(text);
			this.InjectFormat(format);
		}

		public void InjectText(TMP_Text text)
		{
			this._text = text;
		}

		public void InjectFormat(string format)
		{
			this._format = format;
		}

		[SerializeField]
		private TMP_Text _text;

		[SerializeField]
		private string _format = "Version: {0}";
	}
}
