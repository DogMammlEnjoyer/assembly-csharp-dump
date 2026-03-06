using System;
using UnityEngine;

namespace Liv.Lck.Smoothing
{
	public class KalmanFilterQuaternion
	{
		public KalmanFilterQuaternion(Quaternion initialEstimate, float initialCovariance = 1f)
		{
			this._filteredValue = initialEstimate;
			this._estimationErrorCovariance = initialCovariance;
		}

		public Quaternion Update(Quaternion measurement, float deltaTime, float smoothing)
		{
			float num = Mathf.Lerp(10f, 0f, smoothing);
			float num2 = Mathf.Lerp(0f, 10f, smoothing);
			float num3 = Mathf.Max(deltaTime, 0.0001f);
			float num4 = num * num3;
			this._estimationErrorCovariance += num4;
			this._kalmanGain = this._estimationErrorCovariance / (this._estimationErrorCovariance + num2);
			this._filteredValue = Quaternion.Slerp(this._filteredValue, measurement, this._kalmanGain);
			this._estimationErrorCovariance = (1f - this._kalmanGain) * this._estimationErrorCovariance;
			return this._filteredValue;
		}

		private float _estimationErrorCovariance;

		private float _kalmanGain;

		private Quaternion _filteredValue;
	}
}
