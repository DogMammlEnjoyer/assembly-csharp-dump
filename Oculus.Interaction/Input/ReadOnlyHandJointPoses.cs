using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public class ReadOnlyHandJointPoses : IReadOnlyList<Pose>, IEnumerable<Pose>, IEnumerable, IReadOnlyCollection<Pose>
	{
		public ReadOnlyHandJointPoses(Pose[] poses)
		{
			this._poses = poses;
		}

		public IEnumerator<Pose> GetEnumerator()
		{
			foreach (Pose pose in this._poses)
			{
				yield return pose;
			}
			Pose[] array = null;
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public static ReadOnlyHandJointPoses Empty { get; } = new ReadOnlyHandJointPoses(Array.Empty<Pose>());

		public int Count
		{
			get
			{
				return this._poses.Length;
			}
		}

		public Pose this[int index]
		{
			get
			{
				return this._poses[index];
			}
		}

		public Pose this[HandJointId index]
		{
			get
			{
				return ref this._poses[(int)index];
			}
		}

		private Pose[] _poses;
	}
}
