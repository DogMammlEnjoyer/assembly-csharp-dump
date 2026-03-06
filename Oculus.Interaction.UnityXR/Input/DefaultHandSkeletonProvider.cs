using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public class DefaultHandSkeletonProvider : MonoBehaviour, IHandSkeletonProvider
	{
		public HandSkeleton this[Handedness handedness]
		{
			get
			{
				return this._skeletons[(int)handedness];
			}
		}

		private readonly HandSkeleton[] _skeletons = new HandSkeleton[]
		{
			HandSkeleton.DefaultLeftSkeleton,
			HandSkeleton.DefaultRightSkeleton
		};
	}
}
