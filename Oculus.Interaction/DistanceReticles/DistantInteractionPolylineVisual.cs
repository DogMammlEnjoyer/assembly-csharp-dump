using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.DistanceReticles
{
	public class DistantInteractionPolylineVisual : DistantInteractionLineVisual
	{
		public Color Color
		{
			get
			{
				return this._color;
			}
			set
			{
				this._color = value;
			}
		}

		public float LineWidth
		{
			get
			{
				return this._lineWidth;
			}
			set
			{
				this._lineWidth = value;
			}
		}

		protected override void Start()
		{
			base.Start();
			this._polylineRenderer = new PolylineRenderer(this._lineMaterial, true);
			this._linePointsVec4 = new List<Vector4>(new Vector4[base.NumLinePoints]);
		}

		private void OnDestroy()
		{
			this._polylineRenderer.Cleanup();
		}

		protected override void RenderLine(Vector3[] linePoints)
		{
			for (int i = 0; i < linePoints.Length; i++)
			{
				Vector3 vector = linePoints[i];
				this._linePointsVec4[i] = new Vector4(vector.x, vector.y, vector.z, this._lineWidth);
			}
			this._polylineRenderer.SetLines(this._linePointsVec4, this._color);
			this._polylineRenderer.RenderLines();
		}

		protected override void HideLine()
		{
		}

		public void InjectAllDistantInteractionPolylineVisual(IDistanceInteractor interactor, Color color, Material material)
		{
			base.InjectDistanceInteractor(interactor);
			this.InjectLineColor(color);
			this.InjectLineMaterial(material);
		}

		public void InjectLineColor(Color color)
		{
			this._color = color;
		}

		public void InjectLineMaterial(Material material)
		{
			this._lineMaterial = material;
		}

		[SerializeField]
		private Color _color = Color.white;

		[SerializeField]
		private float _lineWidth = 0.02f;

		private List<Vector4> _linePointsVec4;

		[SerializeField]
		private Material _lineMaterial;

		private PolylineRenderer _polylineRenderer;
	}
}
