using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class Cylinder : MonoBehaviour
	{
		public float Radius
		{
			get
			{
				return this._radius;
			}
			set
			{
				this._radius = value;
			}
		}

		[Tooltip("The radius of the cylinder.")]
		[SerializeField]
		private float _radius = 1f;
	}
}
