using System;
using Meta.XR.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Feature(Feature.VirtualKeyboard)]
public class OVRVirtualKeyboardInputFieldTextHandler : OVRVirtualKeyboard.AbstractTextHandler
{
	public InputField InputField
	{
		get
		{
			return this.inputField;
		}
		set
		{
			if (value == this.inputField)
			{
				return;
			}
			if (this.inputField)
			{
				this.inputField.onValueChanged.RemoveListener(new UnityAction<string>(this.ProxyOnValueChanged));
			}
			this.inputField = value;
			if (this.inputField)
			{
				this.inputField.onValueChanged.AddListener(new UnityAction<string>(this.ProxyOnValueChanged));
			}
			Action<string> onTextChanged = this.OnTextChanged;
			if (onTextChanged == null)
			{
				return;
			}
			onTextChanged(this.Text);
		}
	}

	public override Action<string> OnTextChanged { get; set; }

	public override string Text
	{
		get
		{
			if (!this.inputField)
			{
				return string.Empty;
			}
			return this.inputField.text;
		}
	}

	public override bool SubmitOnEnter
	{
		get
		{
			return this.inputField && this.inputField.lineType != InputField.LineType.MultiLineNewline;
		}
	}

	public override bool IsFocused
	{
		get
		{
			return this.inputField && this.inputField.isFocused;
		}
	}

	public override void Submit()
	{
		if (!this.inputField)
		{
			return;
		}
		this.inputField.onEndEdit.Invoke(this.inputField.text);
	}

	public override void AppendText(string s)
	{
		if (!this.inputField)
		{
			return;
		}
		InputField inputField = this.inputField;
		inputField.text += s;
	}

	public override void ApplyBackspace()
	{
		if (!this.inputField || string.IsNullOrEmpty(this.inputField.text))
		{
			return;
		}
		this.inputField.text = this.Text.Substring(0, this.Text.Length - 1);
	}

	public override void MoveTextEnd()
	{
		if (!this.inputField)
		{
			return;
		}
		this.inputField.MoveTextEnd(false);
	}

	protected void Start()
	{
		if (this.inputField)
		{
			this.inputField.onValueChanged.AddListener(new UnityAction<string>(this.ProxyOnValueChanged));
		}
	}

	protected void ProxyOnValueChanged(string arg0)
	{
		Action<string> onTextChanged = this.OnTextChanged;
		if (onTextChanged == null)
		{
			return;
		}
		onTextChanged(arg0);
	}

	[SerializeField]
	private InputField inputField;

	private bool _isSelected;
}
