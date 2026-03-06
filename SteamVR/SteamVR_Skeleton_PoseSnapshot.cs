using System;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Skeleton_PoseSnapshot
	{
		public SteamVR_Skeleton_PoseSnapshot(int boneCount, SteamVR_Input_Sources source)
		{
			this.inputSource = source;
			this.bonePositions = new Vector3[boneCount];
			this.boneRotations = new Quaternion[boneCount];
			this.position = Vector3.zero;
			this.rotation = Quaternion.identity;
		}

		public void CopyFrom(SteamVR_Skeleton_PoseSnapshot source)
		{
			this.inputSource = source.inputSource;
			this.position = source.position;
			this.rotation = source.rotation;
			for (int i = 0; i < this.bonePositions.Length; i++)
			{
				this.bonePositions[i] = source.bonePositions[i];
				this.boneRotations[i] = source.boneRotations[i];
			}
		}

		public SteamVR_Input_Sources inputSource;

		public Vector3 position;

		public Quaternion rotation;

		public Vector3[] bonePositions;

		public Quaternion[] boneRotations;
	}
}
