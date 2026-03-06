using System;
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
	[Feature(Feature.Scene)]
	public class NotInsideMask : Mask
	{
		public override float SampleMask(Candidate c)
		{
			return 0f;
		}

		public override bool Check(Candidate c)
		{
			Bounds? prefabBounds = Utilities.GetPrefabBounds(c.decorationPrefab);
			foreach (MRUKRoom mrukroom in MRUK.Instance.Rooms)
			{
				MRUKAnchor mrukanchor;
				bool flag = mrukroom.IsPositionInSceneVolume(c.hit.point, out mrukanchor, true, prefabBounds.Value.extents.x);
				if (mrukanchor != null && mrukanchor.HasAnyLabel(this.Labels))
				{
					return !flag;
				}
			}
			return true;
		}

		[SerializeField]
		public MRUKAnchor.SceneLabels Labels;
	}
}
