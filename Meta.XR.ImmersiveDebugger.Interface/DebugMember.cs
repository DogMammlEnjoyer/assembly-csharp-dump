using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Meta.XR.ImmersiveDebugger
{
	[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
	[Serializable]
	public class DebugMember : PreserveAttribute
	{
		public DebugMember(DebugColor color = DebugColor.Gray)
		{
			DebugMember.ParsedColors.TryGetValue(color, out this.Color);
		}

		public DebugMember(string colorString)
		{
			if (!string.IsNullOrEmpty(colorString))
			{
				Color color;
				this.Color = (ColorUtility.TryParseHtmlString(colorString, out color) ? color : Color.gray);
			}
		}

		public const string DisplayNameTooltip = "Optional name override to be used in the Inspector Panel";

		private static readonly Dictionary<DebugColor, Color> ParsedColors = new Dictionary<DebugColor, Color>
		{
			{
				DebugColor.Red,
				Color.red
			},
			{
				DebugColor.Gray,
				Color.gray
			}
		};

		public DebugGizmoType GizmoType;

		public bool ShowGizmoByDefault;

		public Color Color = Color.gray;

		public bool Tweakable = true;

		public float Min;

		public float Max = 1f;

		public string Category;

		public string Description;

		[Tooltip("Optional name override to be used in the Inspector Panel")]
		public string DisplayName;
	}
}
