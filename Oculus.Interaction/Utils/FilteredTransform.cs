using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.Utils
{
	public class FilteredTransform : MonoBehaviour
	{
		protected virtual void Start()
		{
			this._positionFilter = OneEuroFilter.CreateVector3();
			this._rotationFilter = OneEuroFilter.CreateQuaternion();
		}

		protected virtual void Update()
		{
			if (this._filterPosition)
			{
				Vector3 position = this._sourceTransform.position;
				this._positionFilter.SetProperties(this._positionFilterProperties);
				base.transform.position = this._positionFilter.Step(this._sourceTransform.position, Time.deltaTime);
			}
			else
			{
				base.transform.position = this._sourceTransform.position;
			}
			if (this._filterRotation)
			{
				this._rotationFilter.SetProperties(this._rotationFilterProperties);
				base.transform.rotation = this._rotationFilter.Step(this._sourceTransform.rotation, Time.deltaTime);
				return;
			}
			base.transform.rotation = this._sourceTransform.rotation;
		}

		public void InjectAllFilteredTransform(Transform sourceTransform)
		{
			this.InjectSourceTransform(sourceTransform);
		}

		public void InjectSourceTransform(Transform sourceTransform)
		{
			this._sourceTransform = sourceTransform;
		}

		[SerializeField]
		private Transform _sourceTransform;

		[SerializeField]
		private bool _filterPosition;

		[SerializeField]
		private OneEuroFilterPropertyBlock _positionFilterProperties = new OneEuroFilterPropertyBlock(2f, 3f);

		[SerializeField]
		private bool _filterRotation;

		[SerializeField]
		private OneEuroFilterPropertyBlock _rotationFilterProperties = new OneEuroFilterPropertyBlock(2f, 3f);

		private IOneEuroFilter<Vector3> _positionFilter;

		private IOneEuroFilter<Quaternion> _rotationFilter;
	}
}
