using System;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_HistoryBuffer : SteamVR_RingBuffer<SteamVR_HistoryStep>
	{
		public SteamVR_HistoryBuffer(int size) : base(size)
		{
		}

		public void Update(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
		{
			if (this.buffer[this.currentIndex] == null)
			{
				this.buffer[this.currentIndex] = new SteamVR_HistoryStep();
			}
			this.buffer[this.currentIndex].position = position;
			this.buffer[this.currentIndex].rotation = rotation;
			this.buffer[this.currentIndex].velocity = velocity;
			this.buffer[this.currentIndex].angularVelocity = angularVelocity;
			this.buffer[this.currentIndex].timeInTicks = DateTime.Now.Ticks;
			this.StepForward();
		}

		public float GetVelocityMagnitudeTrend(int toIndex = -1, int fromIndex = -1)
		{
			if (toIndex == -1)
			{
				toIndex = this.currentIndex - 1;
			}
			if (toIndex < 0)
			{
				toIndex += this.buffer.Length;
			}
			if (fromIndex == -1)
			{
				fromIndex = toIndex - 1;
			}
			if (fromIndex < 0)
			{
				fromIndex += this.buffer.Length;
			}
			SteamVR_HistoryStep steamVR_HistoryStep = this.buffer[toIndex];
			SteamVR_HistoryStep steamVR_HistoryStep2 = this.buffer[fromIndex];
			if (this.IsValid(steamVR_HistoryStep) && this.IsValid(steamVR_HistoryStep2))
			{
				return steamVR_HistoryStep.velocity.sqrMagnitude - steamVR_HistoryStep2.velocity.sqrMagnitude;
			}
			return 0f;
		}

		public bool IsValid(SteamVR_HistoryStep step)
		{
			return step != null && step.timeInTicks != -1L;
		}

		public int GetTopVelocity(int forFrames, int addFrames = 0)
		{
			int num = this.currentIndex;
			float num2 = 0f;
			int num3 = this.currentIndex;
			while (forFrames > 0)
			{
				forFrames--;
				num3--;
				if (num3 < 0)
				{
					num3 = this.buffer.Length - 1;
				}
				SteamVR_HistoryStep step = this.buffer[num3];
				if (!this.IsValid(step))
				{
					break;
				}
				float sqrMagnitude = this.buffer[num3].velocity.sqrMagnitude;
				if (sqrMagnitude > num2)
				{
					num = num3;
					num2 = sqrMagnitude;
				}
			}
			num += addFrames;
			if (num >= this.buffer.Length)
			{
				num -= this.buffer.Length;
			}
			return num;
		}

		public void GetAverageVelocities(out Vector3 velocity, out Vector3 angularVelocity, int forFrames, int startFrame = -1)
		{
			velocity = Vector3.zero;
			angularVelocity = Vector3.zero;
			if (startFrame == -1)
			{
				startFrame = this.currentIndex - 1;
			}
			if (startFrame < 0)
			{
				startFrame = this.buffer.Length - 1;
			}
			int num = startFrame - forFrames;
			if (num < 0)
			{
				num += this.buffer.Length;
			}
			Vector3 a = Vector3.zero;
			Vector3 a2 = Vector3.zero;
			float num2 = 0f;
			int num3 = startFrame;
			while (forFrames > 0)
			{
				forFrames--;
				num3--;
				if (num3 < 0)
				{
					num3 = this.buffer.Length - 1;
				}
				SteamVR_HistoryStep steamVR_HistoryStep = this.buffer[num3];
				if (!this.IsValid(steamVR_HistoryStep))
				{
					break;
				}
				num2 += 1f;
				a += steamVR_HistoryStep.velocity;
				a2 += steamVR_HistoryStep.angularVelocity;
			}
			velocity = a / num2;
			angularVelocity = a2 / num2;
		}
	}
}
