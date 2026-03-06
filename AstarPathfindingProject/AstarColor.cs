using System;
using UnityEngine;

namespace Pathfinding
{
	[Serializable]
	public class AstarColor
	{
		public static int ColorHash()
		{
			int num = AstarColor.SolidColor.GetHashCode() ^ AstarColor.UnwalkableNode.GetHashCode() ^ AstarColor.BoundsHandles.GetHashCode() ^ AstarColor.ConnectionLowLerp.GetHashCode() ^ AstarColor.ConnectionHighLerp.GetHashCode() ^ AstarColor.MeshEdgeColor.GetHashCode();
			for (int i = 0; i < AstarColor.AreaColors.Length; i++)
			{
				num = (7 * num ^ AstarColor.AreaColors[i].GetHashCode());
			}
			return num;
		}

		public static Color GetAreaColor(uint area)
		{
			if ((ulong)area >= (ulong)((long)AstarColor.AreaColors.Length))
			{
				return AstarMath.IntToColor((int)area, 1f);
			}
			return AstarColor.AreaColors[(int)area];
		}

		public static Color GetTagColor(uint tag)
		{
			if ((ulong)tag >= (ulong)((long)AstarColor.AreaColors.Length))
			{
				return AstarMath.IntToColor((int)tag, 1f);
			}
			return AstarColor.AreaColors[(int)tag];
		}

		public void PushToStatic(AstarPath astar)
		{
			this._AreaColors = (this._AreaColors ?? new Color[1]);
			AstarColor.SolidColor = this._SolidColor;
			AstarColor.UnwalkableNode = this._UnwalkableNode;
			AstarColor.BoundsHandles = this._BoundsHandles;
			AstarColor.ConnectionLowLerp = this._ConnectionLowLerp;
			AstarColor.ConnectionHighLerp = this._ConnectionHighLerp;
			AstarColor.MeshEdgeColor = this._MeshEdgeColor;
			AstarColor.AreaColors = this._AreaColors;
		}

		public AstarColor()
		{
			this._SolidColor = new Color(0.11764706f, 0.4f, 0.7882353f, 0.9f);
			this._UnwalkableNode = new Color(1f, 0f, 0f, 0.5f);
			this._BoundsHandles = new Color(0.29f, 0.454f, 0.741f, 0.9f);
			this._ConnectionLowLerp = new Color(0f, 1f, 0f, 0.5f);
			this._ConnectionHighLerp = new Color(1f, 0f, 0f, 0.5f);
			this._MeshEdgeColor = new Color(0f, 0f, 0f, 0.5f);
		}

		public Color _SolidColor;

		public Color _UnwalkableNode;

		public Color _BoundsHandles;

		public Color _ConnectionLowLerp;

		public Color _ConnectionHighLerp;

		public Color _MeshEdgeColor;

		public Color[] _AreaColors;

		public static Color SolidColor = new Color(0.11764706f, 0.4f, 0.7882353f, 0.9f);

		public static Color UnwalkableNode = new Color(1f, 0f, 0f, 0.5f);

		public static Color BoundsHandles = new Color(0.29f, 0.454f, 0.741f, 0.9f);

		public static Color ConnectionLowLerp = new Color(0f, 1f, 0f, 0.5f);

		public static Color ConnectionHighLerp = new Color(1f, 0f, 0f, 0.5f);

		public static Color MeshEdgeColor = new Color(0f, 0f, 0f, 0.5f);

		private static Color[] AreaColors = new Color[1];
	}
}
