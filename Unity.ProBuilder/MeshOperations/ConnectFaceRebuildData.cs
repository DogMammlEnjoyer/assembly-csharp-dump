using System;
using System.Collections.Generic;

namespace UnityEngine.ProBuilder.MeshOperations
{
	internal sealed class ConnectFaceRebuildData
	{
		public ConnectFaceRebuildData(FaceRebuildData faceRebuildData, List<int> newVertexIndexes)
		{
			this.faceRebuildData = faceRebuildData;
			this.newVertexIndexes = newVertexIndexes;
		}

		public FaceRebuildData faceRebuildData;

		public List<int> newVertexIndexes;
	}
}
