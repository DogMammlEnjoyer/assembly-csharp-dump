using System;
using System.Diagnostics;
using UnityEngine;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class PreviewFieldAttribute : Attribute
	{
		public ObjectFieldAlignment Alignment
		{
			get
			{
				return this.alignment;
			}
			set
			{
				this.alignment = value;
				this.alignmentHasValue = true;
			}
		}

		public bool AlignmentHasValue
		{
			get
			{
				return this.alignmentHasValue;
			}
		}

		public string PreviewGetter
		{
			get
			{
				return this.previewGetter;
			}
			set
			{
				this.previewGetter = value;
				this.PreviewGetterHasValue = true;
			}
		}

		public bool PreviewGetterHasValue { get; private set; }

		public PreviewFieldAttribute()
		{
			this.Height = 0f;
		}

		public PreviewFieldAttribute(float height)
		{
			this.Height = height;
		}

		public PreviewFieldAttribute(string previewGetter, FilterMode filterMode = FilterMode.Bilinear)
		{
			this.PreviewGetter = previewGetter;
			this.FilterMode = filterMode;
		}

		public PreviewFieldAttribute(string previewGetter, float height, FilterMode filterMode = FilterMode.Bilinear)
		{
			this.PreviewGetter = previewGetter;
			this.Height = height;
			this.FilterMode = filterMode;
		}

		public PreviewFieldAttribute(float height, ObjectFieldAlignment alignment)
		{
			this.Height = height;
			this.Alignment = alignment;
		}

		public PreviewFieldAttribute(string previewGetter, ObjectFieldAlignment alignment, FilterMode filterMode = FilterMode.Bilinear)
		{
			this.PreviewGetter = previewGetter;
			this.Alignment = alignment;
			this.FilterMode = filterMode;
		}

		public PreviewFieldAttribute(string previewGetter, float height, ObjectFieldAlignment alignment, FilterMode filterMode = FilterMode.Bilinear)
		{
			this.PreviewGetter = previewGetter;
			this.Height = height;
			this.Alignment = alignment;
			this.FilterMode = filterMode;
		}

		public PreviewFieldAttribute(ObjectFieldAlignment alignment)
		{
			this.Alignment = alignment;
		}

		private ObjectFieldAlignment alignment;

		private bool alignmentHasValue;

		private string previewGetter;

		public float Height;

		public FilterMode FilterMode = FilterMode.Bilinear;
	}
}
