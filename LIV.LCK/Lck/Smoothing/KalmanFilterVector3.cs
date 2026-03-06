using System;
using UnityEngine;

namespace Liv.Lck.Smoothing
{
	public class KalmanFilterVector3
	{
		public KalmanFilterVector3(Vector3 initialEstimate = default(Vector3), float initialCovariance = 1f)
		{
			this._filterX = new KalmanFilter(initialEstimate.x, initialCovariance);
			this._filterY = new KalmanFilter(initialEstimate.y, initialCovariance);
			this._filterZ = new KalmanFilter(initialEstimate.z, initialCovariance);
		}

		public Vector3 Update(Vector3 measurement, float deltaTime, float smoothing)
		{
			return new Vector3(this._filterX.Update(measurement.x, deltaTime, smoothing), this._filterY.Update(measurement.y, deltaTime, smoothing), this._filterZ.Update(measurement.z, deltaTime, smoothing));
		}

		private KalmanFilter _filterX;

		private KalmanFilter _filterY;

		private KalmanFilter _filterZ;
	}
}
