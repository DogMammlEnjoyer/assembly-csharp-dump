using System;
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
	[Feature(Feature.Scene)]
	public class SpaceMapGPUMask : Mask
	{
		public override float SampleMask(Candidate candidate)
		{
			if (this.spaceMap == null)
			{
				this.spaceMap = Object.FindAnyObjectByType<SpaceMapGPU>();
				if (this.spaceMap == null)
				{
					Debug.LogWarning("SpaceMapGPU cannot be found, does it exist in the Scene?");
					return 0f;
				}
			}
			return this.spaceMap.GetColorAtPosition(candidate.hit.point).r;
		}

		public override bool Check(Candidate c)
		{
			return true;
		}

		private SpaceMapGPU spaceMap;
	}
}
