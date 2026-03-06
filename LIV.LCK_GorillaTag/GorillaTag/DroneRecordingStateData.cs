using System;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	public class DroneRecordingStateData
	{
		public event DroneRecordingStateData.OnDroneRecordingState OnDroneRecordingStateChanged;

		public RecordingState State
		{
			get
			{
				return this._recordingState;
			}
			set
			{
				this._recordingState = value;
				DroneRecordingStateData.OnDroneRecordingState onDroneRecordingStateChanged = this.OnDroneRecordingStateChanged;
				if (onDroneRecordingStateChanged == null)
				{
					return;
				}
				onDroneRecordingStateChanged(this._recordingState);
			}
		}

		public TimeSpan Span
		{
			get
			{
				return this._span;
			}
			set
			{
				this._span = value;
			}
		}

		public string FormattedDuration
		{
			get
			{
				int num = Mathf.FloorToInt((float)this._span.Hours);
				int num2 = Mathf.FloorToInt((float)this._span.Minutes);
				int num3 = Mathf.FloorToInt((float)this._span.Seconds);
				if (num != 0)
				{
					return string.Format("{0:00}:{1:00}:{2:00}", num, num2, num3);
				}
				return string.Format("{0:00}:{1:00}", num2, num3);
			}
		}

		private TimeSpan _span;

		private RecordingState _recordingState;

		public delegate void OnDroneRecordingState(RecordingState state);
	}
}
