using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All)]
	[Conditional("UNITY_EDITOR")]
	public class InlineEditorAttribute : Attribute
	{
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

		public bool IncrementInlineEditorDrawerDepth = true;

		public InlineEditorObjectFieldModes ObjectFieldMode;

		public bool DisableGUIForVCSLockedAssets = true;

		public PreviewAlignment PreviewAlignment = PreviewAlignment.Right;
	}
}
