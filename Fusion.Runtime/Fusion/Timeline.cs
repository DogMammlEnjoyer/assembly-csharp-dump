using System;

namespace Fusion
{
	internal class Timeline
	{
		public Timeline(int capacity)
		{
			this.Points = new RingBuffer<TimelinePoint>(capacity);
			this.Params = default(InterpolationParams);
		}

		public bool IsEmpty
		{
			get
			{
				return this.Points.IsEmpty;
			}
		}

		public void Clear()
		{
			this.Points.Clear();
			this.Params = default(InterpolationParams);
		}

		public unsafe void AddPoint(TimelinePoint point, double tickDeltaDouble, bool allowInactiveHandling = true)
		{
			bool flag = allowInactiveHandling && !this.Points.IsEmpty;
			if (flag)
			{
				TimelinePoint timelinePoint = *this.Points.Back();
				double num = point.Time - timelinePoint.Time;
				bool flag2 = num >= 0.25;
				if (flag2)
				{
					TimelinePoint item = timelinePoint;
					item.Time = (double)(point.Tick - 1) * tickDeltaDouble;
					this.Points.PushBack(item);
				}
			}
			this.Points.PushBack(point);
		}

		public InterpolationParams GetInterpolationParams(double time)
		{
			InterpolationParams interpolationParams = default(InterpolationParams);
			interpolationParams.Time = time;
			bool isEmpty = this.Points.IsEmpty;
			InterpolationParams result;
			if (isEmpty)
			{
				result = interpolationParams;
			}
			else
			{
				try
				{
					double time2 = this.Points.Front().Time;
					double time3 = this.Points.Back().Time;
					bool flag = time <= time2;
					if (flag)
					{
						interpolationParams.From = this.Points.Front().Snapshot;
						interpolationParams.To = this.Points.Front().Snapshot;
						interpolationParams.Alpha = 0f;
						interpolationParams.Status = Status.Behind;
					}
					else
					{
						bool flag2 = time >= time3;
						if (flag2)
						{
							interpolationParams.From = this.Points.Back().Snapshot;
							interpolationParams.To = this.Points.Back().Snapshot;
							interpolationParams.Alpha = 0f;
							interpolationParams.Status = Status.Ahead;
						}
						else
						{
							for (int i = 0; i < this.Points.Count - 1; i++)
							{
								double time4 = this.Points[i].Time;
								double time5 = this.Points[i + 1].Time;
								bool flag3 = time >= time4 && time < time5;
								if (flag3)
								{
									interpolationParams.From = this.Points[i].Snapshot;
									interpolationParams.To = this.Points[i + 1].Snapshot;
									interpolationParams.Alpha = (float)Maths.Clamp01((time - time4) / (time5 - time4));
									interpolationParams.Status = Status.Good;
									break;
								}
							}
						}
					}
				}
				catch (Exception error)
				{
					LogStream logException = InternalLogStreams.LogException;
					if (logException != null)
					{
						logException.Log(error);
					}
				}
				result = interpolationParams;
			}
			return result;
		}

		public void UpdateInterpolationParams(double time)
		{
			this.Params = this.GetInterpolationParams(time);
		}

		public RingBuffer<TimelinePoint> Points;

		public InterpolationParams Params;
	}
}
