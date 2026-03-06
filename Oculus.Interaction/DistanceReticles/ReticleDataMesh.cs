using System;
using UnityEngine;

namespace Oculus.Interaction.DistanceReticles
{
	public class ReticleDataMesh : MonoBehaviour, IReticleData
	{
		public MeshFilter Filter
		{
			get
			{
				return this._filter;
			}
			set
			{
				this._filter = value;
			}
		}

		public Transform Target
		{
			get
			{
				return this._filter.transform;
			}
		}

		public Vector3 ProcessHitPoint(Vector3 hitPoint)
		{
			return this._filter.transform.position;
		}

		[Tooltip("The mesh of the GameObject to outline.")]
		[SerializeField]
		private MeshFilter _filter;
	}
}
