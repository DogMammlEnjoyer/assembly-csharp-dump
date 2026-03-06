using System;
using UnityEngine;

namespace Meta.XR.BuildingBlocks
{
	public class PassthroughProjectionSurfaceBuildingBlock : MonoBehaviour
	{
		private void Start()
		{
			OVRPassthroughLayer[] array = Object.FindObjectsByType<OVRPassthroughLayer>(FindObjectsSortMode.None);
			bool flag = false;
			foreach (OVRPassthroughLayer ovrpassthroughLayer in array)
			{
				if (ovrpassthroughLayer.GetComponent<BuildingBlock>())
				{
					flag = true;
					ovrpassthroughLayer.AddSurfaceGeometry(this.projectionObject.gameObject, true);
				}
			}
			if (flag)
			{
				this.projectionObject.GetComponent<MeshRenderer>().enabled = false;
				return;
			}
			throw new InvalidOperationException("A Building Block with the passthrough overlay layer was not found");
		}

		public MeshFilter projectionObject;
	}
}
