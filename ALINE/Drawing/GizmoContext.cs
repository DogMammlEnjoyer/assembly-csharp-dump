using System;
using System.Collections.Generic;
using UnityEngine;

namespace Drawing
{
	public static class GizmoContext
	{
		public static int selectionSize
		{
			get
			{
				GizmoContext.Refresh();
				return GizmoContext.selectionSizeInternal;
			}
			private set
			{
				GizmoContext.selectionSizeInternal = value;
			}
		}

		internal static void SetDirty()
		{
			GizmoContext.dirty = true;
		}

		private static void Refresh()
		{
		}

		public static bool InSelection(Component c)
		{
			return GizmoContext.InSelection(c.transform);
		}

		public static bool InSelection(Transform tr)
		{
			GizmoContext.Refresh();
			Transform item = tr;
			while (tr != null)
			{
				if (GizmoContext.selectedTransforms.Contains(tr))
				{
					GizmoContext.selectedTransforms.Add(item);
					return true;
				}
				tr = tr.parent;
			}
			return false;
		}

		public static bool InActiveSelection(Component c)
		{
			return GizmoContext.InActiveSelection(c.transform);
		}

		public static bool InActiveSelection(Transform tr)
		{
			return false;
		}

		private static HashSet<Transform> selectedTransforms = new HashSet<Transform>();

		internal static bool drawingGizmos;

		internal static bool dirty;

		private static int selectionSizeInternal;
	}
}
