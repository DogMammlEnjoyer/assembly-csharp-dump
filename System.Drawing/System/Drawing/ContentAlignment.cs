using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace System.Drawing
{
	/// <summary>Specifies alignment of content on the drawing surface.</summary>
	[Editor("System.Drawing.Design.ContentAlignmentEditor, System.Drawing.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	public enum ContentAlignment
	{
		/// <summary>Content is vertically aligned at the top, and horizontally aligned on the left.</summary>
		TopLeft = 1,
		/// <summary>Content is vertically aligned at the top, and horizontally aligned at the center.</summary>
		TopCenter,
		/// <summary>Content is vertically aligned at the top, and horizontally aligned on the right.</summary>
		TopRight = 4,
		/// <summary>Content is vertically aligned in the middle, and horizontally aligned on the left.</summary>
		MiddleLeft = 16,
		/// <summary>Content is vertically aligned in the middle, and horizontally aligned at the center.</summary>
		MiddleCenter = 32,
		/// <summary>Content is vertically aligned in the middle, and horizontally aligned on the right.</summary>
		MiddleRight = 64,
		/// <summary>Content is vertically aligned at the bottom, and horizontally aligned on the left.</summary>
		BottomLeft = 256,
		/// <summary>Content is vertically aligned at the bottom, and horizontally aligned at the center.</summary>
		BottomCenter = 512,
		/// <summary>Content is vertically aligned at the bottom, and horizontally aligned on the right.</summary>
		BottomRight = 1024
	}
}
