using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	[Obsolete]
	public class HeadingTracker
	{
		public HeadingTracker(int filterSize)
		{
			this.mHistory = new HeadingTracker.Item[filterSize];
			float num = (float)filterSize / 5f;
			HeadingTracker.mDecayExponent = -Mathf.Log(2f) / num;
			this.ClearHistory();
		}

		public int FilterSize
		{
			get
			{
				return this.mHistory.Length;
			}
		}

		private void ClearHistory()
		{
			this.mTop = (this.mBottom = (this.mCount = 0));
			this.mWeightSum = 0f;
			this.mHeadingSum = Vector3.zero;
		}

		private static float Decay(float time)
		{
			return Mathf.Exp(time * HeadingTracker.mDecayExponent);
		}

		public void Add(Vector3 velocity)
		{
			if (this.FilterSize == 0)
			{
				this.mLastGoodHeading = velocity;
				return;
			}
			float magnitude = velocity.magnitude;
			if (magnitude > 0.0001f)
			{
				HeadingTracker.Item item = default(HeadingTracker.Item);
				item.velocity = velocity;
				item.weight = magnitude;
				item.time = CinemachineCore.CurrentTime;
				if (this.mCount == this.FilterSize)
				{
					this.PopBottom();
				}
				this.mCount++;
				this.mHistory[this.mTop] = item;
				int num = this.mTop + 1;
				this.mTop = num;
				if (num == this.FilterSize)
				{
					this.mTop = 0;
				}
				this.mWeightSum *= HeadingTracker.Decay(item.time - this.mWeightTime);
				this.mWeightTime = item.time;
				this.mWeightSum += magnitude;
				this.mHeadingSum += item.velocity;
			}
		}

		private void PopBottom()
		{
			if (this.mCount > 0)
			{
				float currentTime = CinemachineCore.CurrentTime;
				HeadingTracker.Item item = this.mHistory[this.mBottom];
				int num = this.mBottom + 1;
				this.mBottom = num;
				if (num == this.FilterSize)
				{
					this.mBottom = 0;
				}
				this.mCount--;
				float num2 = HeadingTracker.Decay(currentTime - item.time);
				this.mWeightSum -= item.weight * num2;
				this.mHeadingSum -= item.velocity * num2;
				if (this.mWeightSum <= 0.0001f || this.mCount == 0)
				{
					this.ClearHistory();
				}
			}
		}

		public void DecayHistory()
		{
			float currentTime = CinemachineCore.CurrentTime;
			float num = HeadingTracker.Decay(currentTime - this.mWeightTime);
			this.mWeightSum *= num;
			this.mWeightTime = currentTime;
			if (this.mWeightSum < 0.0001f)
			{
				this.ClearHistory();
				return;
			}
			this.mHeadingSum *= num;
		}

		public Vector3 GetReliableHeading()
		{
			if (this.mWeightSum > 0.0001f && (this.mCount == this.mHistory.Length || this.mLastGoodHeading.AlmostZero()))
			{
				Vector3 v = this.mHeadingSum / this.mWeightSum;
				if (!v.AlmostZero())
				{
					this.mLastGoodHeading = v.normalized;
				}
			}
			return this.mLastGoodHeading;
		}

		private HeadingTracker.Item[] mHistory;

		private int mTop;

		private int mBottom;

		private int mCount;

		private Vector3 mHeadingSum;

		private float mWeightSum;

		private float mWeightTime;

		private Vector3 mLastGoodHeading = Vector3.zero;

		private static float mDecayExponent;

		private struct Item
		{
			public Vector3 velocity;

			public float weight;

			public float time;
		}
	}
}
