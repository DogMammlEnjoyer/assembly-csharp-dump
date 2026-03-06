using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class TransformsPolyline : MonoBehaviour, IPolyline
	{
		public int PointsCount
		{
			get
			{
				return this._transforms.Length;
			}
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		public Vector3 PointAtIndex(int index)
		{
			return this._transforms[index].position;
		}

		public void InjectAllTransformsPolyline(Transform[] transforms)
		{
			this.InjectTransforms(transforms);
		}

		public void InjectTransforms(Transform[] transforms)
		{
			this._transforms = transforms;
		}

		[SerializeField]
		private Transform[] _transforms;

		protected bool _started;
	}
}
