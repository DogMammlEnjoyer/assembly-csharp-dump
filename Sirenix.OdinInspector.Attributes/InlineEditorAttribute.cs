using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All)]
	[Conditional("UNITY_EDITOR")]
	public class InlineEditorAttribute : Attribute
	{
		[ShowInInspector]
		[OdinDesignerBinding(new string[]
		{
			"expanded",
			"ExpandedHasValue"
		})]
		public bool Expanded
		{
			get
			{
				return this.expanded;
			}
			set
			{
				this.expanded = value;
				this.ExpandedHasValue = true;
			}
		}

		public bool ExpandedHasValue { get; private set; }

		public InlineEditorAttribute(InlineEditorModes inlineEditorMode = InlineEditorModes.GUIOnly, InlineEditorObjectFieldModes objectFieldMode = InlineEditorObjectFieldModes.Boxed)
		{
			this.ObjectFieldMode = objectFieldMode;
			switch (inlineEditorMode)
			{
			case InlineEditorModes.GUIOnly:
				this.DrawGUI = true;
				return;
			case InlineEditorModes.GUIAndHeader:
				this.DrawGUI = true;
				this.DrawHeader = true;
				return;
			case InlineEditorModes.GUIAndPreview:
				this.DrawGUI = true;
				this.DrawPreview = true;
				return;
			case InlineEditorModes.SmallPreview:
				this.expanded = true;
				this.DrawPreview = true;
				return;
			case InlineEditorModes.LargePreview:
				this.expanded = true;
				this.DrawPreview = true;
				this.PreviewHeight = 170f;
				return;
			case InlineEditorModes.FullEditor:
				this.DrawGUI = true;
				this.DrawHeader = true;
				this.DrawPreview = true;
				return;
			default:
				throw new NotImplementedException();
			}
		}

		public InlineEditorAttribute(InlineEditorObjectFieldModes objectFieldMode) : this(InlineEditorModes.GUIOnly, objectFieldMode)
		{
		}

		private bool expanded;

		public bool DrawHeader;

		public bool DrawGUI;

		public bool DrawPreview;

		public float MaxHeight;

		public float PreviewWidth = 100f;

		public float PreviewHeight = 35f;

		[LabelWidth(220f)]
		public bool IncrementInlineEditorDrawerDepth = true;

		[LabelWidth(220f)]
		public bool DisableGUIForVCSLockedAssets = true;

		public InlineEditorObjectFieldModes ObjectFieldMode;

		public PreviewAlignment PreviewAlignment = PreviewAlignment.Right;
	}
}
