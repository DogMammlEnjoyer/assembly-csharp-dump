using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class PointableCanvasEventArgs
	{
		public PointableCanvasEventArgs(Canvas canvas, GameObject hovered, bool dragging)
		{
			this.Canvas = canvas;
			this.Hovered = hovered;
			this.Dragging = dragging;
		}

		public readonly Canvas Canvas;

		public readonly GameObject Hovered;

		public readonly bool Dragging;
	}
}
