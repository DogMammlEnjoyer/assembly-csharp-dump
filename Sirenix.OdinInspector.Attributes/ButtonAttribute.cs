using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
	[Conditional("UNITY_EDITOR")]
	public class ButtonAttribute : ShowInInspectorAttribute
	{
		public int ButtonHeight
		{
			get
			{
				return this.buttonHeight;
			}
			set
			{
				this.buttonHeight = value;
				this.HasDefinedButtonHeight = true;
			}
		}

		public IconAlignment IconAlignment
		{
			get
			{
				return this.buttonIconAlignment;
			}
			set
			{
				this.buttonIconAlignment = value;
				this.HasDefinedButtonIconAlignment = true;
			}
		}

		public float ButtonAlignment
		{
			get
			{
				return this.buttonAlignment;
			}
			set
			{
				this.buttonAlignment = value;
				this.HasDefinedButtonAlignment = true;
			}
		}

		public bool Stretch
		{
			get
			{
				return this.stretch;
			}
			set
			{
				this.stretch = value;
				this.HasDefinedStretch = true;
			}
		}

		public bool DrawResult
		{
			get
			{
				return this.drawResult;
			}
			set
			{
				this.drawResult = value;
				this.drawResultIsSet = true;
			}
		}

		public bool DrawResultIsSet
		{
			get
			{
				return this.drawResultIsSet;
			}
		}

		public bool HasDefinedButtonHeight { get; private set; }

		public bool HasDefinedIcon
		{
			get
			{
				return this.Icon > SdfIconType.None;
			}
		}

		public bool HasDefinedButtonIconAlignment { get; private set; }

		public bool HasDefinedButtonAlignment { get; private set; }

		public bool HasDefinedStretch { get; private set; }

		public ButtonAttribute()
		{
			this.Name = null;
		}

		public ButtonAttribute(ButtonSizes size)
		{
			this.Name = null;
			this.ButtonHeight = (int)size;
		}

		public ButtonAttribute(int buttonSize)
		{
			this.ButtonHeight = buttonSize;
			this.Name = null;
		}

		public ButtonAttribute(string name)
		{
			this.Name = name;
		}

		public ButtonAttribute(string name, ButtonSizes buttonSize)
		{
			this.Name = name;
			this.ButtonHeight = (int)buttonSize;
		}

		public ButtonAttribute(string name, int buttonSize)
		{
			this.Name = name;
			this.ButtonHeight = buttonSize;
		}

		public ButtonAttribute(ButtonStyle parameterBtnStyle)
		{
			this.Name = null;
			this.Style = parameterBtnStyle;
		}

		public ButtonAttribute(int buttonSize, ButtonStyle parameterBtnStyle)
		{
			this.ButtonHeight = buttonSize;
			this.Name = null;
			this.Style = parameterBtnStyle;
		}

		public ButtonAttribute(ButtonSizes size, ButtonStyle parameterBtnStyle)
		{
			this.ButtonHeight = (int)size;
			this.Name = null;
			this.Style = parameterBtnStyle;
		}

		public ButtonAttribute(string name, ButtonStyle parameterBtnStyle)
		{
			this.Name = name;
			this.Style = parameterBtnStyle;
		}

		public ButtonAttribute(string name, ButtonSizes buttonSize, ButtonStyle parameterBtnStyle)
		{
			this.Name = name;
			this.ButtonHeight = (int)buttonSize;
			this.Style = parameterBtnStyle;
		}

		public ButtonAttribute(string name, int buttonSize, ButtonStyle parameterBtnStyle)
		{
			this.Name = name;
			this.ButtonHeight = buttonSize;
			this.Style = parameterBtnStyle;
		}

		public ButtonAttribute(SdfIconType icon, IconAlignment iconAlignment)
		{
			this.Icon = icon;
			this.IconAlignment = iconAlignment;
			this.Name = null;
		}

		public ButtonAttribute(SdfIconType icon)
		{
			this.Icon = icon;
			this.Name = null;
		}

		public ButtonAttribute(SdfIconType icon, string name)
		{
			this.Name = name;
			this.Icon = icon;
		}

		public string Name;

		public ButtonStyle Style;

		public bool Expanded;

		public bool DisplayParameters = true;

		public bool DirtyOnClick = true;

		public SdfIconType Icon;

		private int buttonHeight;

		private bool drawResult;

		private bool drawResultIsSet;

		private bool stretch;

		private IconAlignment buttonIconAlignment;

		private float buttonAlignment;
	}
}
