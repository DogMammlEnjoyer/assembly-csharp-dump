using System;
using UnityEngine;

namespace Oculus.Interaction.DistanceReticles
{
	public class ReticleDataIcon : MonoBehaviour, IReticleData
	{
		public Texture CustomIcon
		{
			get
			{
				return this._customIcon;
			}
			set
			{
				this._customIcon = value;
			}
		}

		public float Snappiness
		{
			get
			{
				return this._snappiness;
			}
			set
			{
				this._snappiness = value;
			}
		}

		public Vector3 GetTargetSize()
		{
			if (this._renderer != null)
			{
				return this._renderer.bounds.size;
			}
			return base.transform.localScale;
		}

		public Vector3 ProcessHitPoint(Vector3 hitPoint)
		{
			return Vector3.Lerp(hitPoint, base.transform.position, this._snappiness);
		}

		public void InjectOptionalRenderer(MeshRenderer renderer)
		{
			this._renderer = renderer;
		}

		[Tooltip("The Mesh Renderer of the GameObject that the icon can appear on.")]
		[SerializeField]
		[Optional]
		private MeshRenderer _renderer;

		[Tooltip("The icon's appearance.")]
		[SerializeField]
		[Optional]
		private Texture _customIcon;

		[SerializeField]
		[Range(0f, 1f)]
		private float _snappiness;
	}
}
