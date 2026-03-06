using System;
using System.Collections.Generic;
using UnityEngine;

namespace Drawing.Examples
{
	public class CurveEditor : MonoBehaviour
	{
		private void Awake()
		{
			this.cam = Camera.main;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				this.curves.Add(new CurveEditor.CurvePoint
				{
					position = Input.mousePosition,
					controlPoint0 = Vector2.zero,
					controlPoint1 = Vector2.zero
				});
			}
			if (this.curves.Count > 0 && Input.GetKey(KeyCode.Mouse0) && (Input.mousePosition - this.curves[this.curves.Count - 1].position).magnitude > 4f)
			{
				CurveEditor.CurvePoint curvePoint = this.curves[this.curves.Count - 1];
				curvePoint.controlPoint1 = Input.mousePosition - curvePoint.position;
				curvePoint.controlPoint0 = -curvePoint.controlPoint1;
			}
			this.Render();
		}

		private void Render()
		{
			using (CommandBuilder builder = DrawingManager.GetBuilder(true))
			{
				using (builder.InScreenSpace(this.cam))
				{
					for (int i = 0; i < this.curves.Count; i++)
					{
						builder.xy.Circle(this.curves[i].position, 2f, Color.blue);
					}
					for (int j = 0; j < this.curves.Count - 1; j++)
					{
						Vector2 position = this.curves[j].position;
						Vector2 v = position + this.curves[j].controlPoint1;
						Vector2 position2 = this.curves[j + 1].position;
						Vector2 v2 = position2 + this.curves[j + 1].controlPoint0;
						builder.Bezier(position, v, v2, position2, this.curveColor);
					}
				}
			}
		}

		private List<CurveEditor.CurvePoint> curves = new List<CurveEditor.CurvePoint>();

		private Camera cam;

		public Color curveColor;

		private class CurvePoint
		{
			public Vector2 position;

			public Vector2 controlPoint0;

			public Vector2 controlPoint1;
		}
	}
}
