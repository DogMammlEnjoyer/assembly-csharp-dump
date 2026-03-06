using System;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class HandFingerMaskGenerator
	{
		private static float HandednessMultiplier(Handedness hand)
		{
			if (hand == Handedness.Right)
			{
				return 1f;
			}
			return -1f;
		}

		private static List<Vector2> GenerateModelUV(Handedness handedness, Mesh sharedHandMesh, out Vector2 minPosition, out Vector2 maxPosition)
		{
			List<Vector3> list = new List<Vector3>();
			sharedHandMesh.GetVertices(list);
			minPosition = new Vector2(list[0].x, list[0].z);
			maxPosition = new Vector2(list[0].x, list[0].z);
			for (int i = 0; i < list.Count; i++)
			{
				Vector3 vector = list[i] * HandFingerMaskGenerator.HandednessMultiplier(handedness);
				Vector2 rhs = new Vector2(vector.x, vector.z);
				minPosition = Vector2.Min(minPosition, rhs);
				maxPosition = Vector2.Max(maxPosition, rhs);
				list[i] = vector;
			}
			List<Vector2> list2 = new List<Vector2>();
			Vector2 vector2 = maxPosition - minPosition;
			float d = Mathf.Max(vector2.x, vector2.y);
			foreach (Vector3 vector3 in list)
			{
				Vector2 item = (new Vector2(vector3.x, vector3.z) - minPosition) / d;
				list2.Add(item);
			}
			return list2;
		}

		private static Vector2 GetPositionOnRegion(HandVisual handVisual, HandJointId jointId, Vector2 minRegion, float sideLength)
		{
			IHand hand = handVisual.Hand;
			Pose pose = handVisual.Joints[(int)jointId].GetPose(Space.World);
			Vector3 vector = handVisual.Root.InverseTransformPoint(pose.position);
			return (new Vector2(vector.x, vector.z) * HandFingerMaskGenerator.HandednessMultiplier(hand.Handedness) - minRegion) / sideLength;
		}

		private static Vector4[] GenerateFingerLines(HandVisual handVisual, Vector2 minPosition, float maxLength, float[] lineScale)
		{
			Vector4 vector = HandFingerMaskGenerator.GenerateLineData(handVisual, HandJointId.HandThumbTip, HandJointId.HandThumb1, minPosition, maxLength, lineScale[0]);
			Vector4 vector2 = HandFingerMaskGenerator.GenerateLineData(handVisual, HandJointId.HandIndexTip, HandJointId.HandIndex1, minPosition, maxLength, lineScale[1]);
			Vector4 vector3 = HandFingerMaskGenerator.GenerateLineData(handVisual, HandJointId.HandMiddleTip, HandJointId.HandMiddle1, minPosition, maxLength, lineScale[2]);
			Vector4 vector4 = HandFingerMaskGenerator.GenerateLineData(handVisual, HandJointId.HandRingTip, HandJointId.HandRing1, minPosition, maxLength, lineScale[3]);
			Vector4 vector5 = HandFingerMaskGenerator.GenerateLineData(handVisual, HandJointId.HandPinkyTip, HandJointId.HandPinky1, minPosition, maxLength, lineScale[4]);
			return new Vector4[]
			{
				vector,
				vector2,
				vector3,
				vector4,
				vector5
			};
		}

		private static Vector4 GenerateLineData(HandVisual handVisual, HandJointId jointIdStart, HandJointId jointIdEnd, Vector2 minRegion, float sideLength, float lineScale)
		{
			Vector2 positionOnRegion = HandFingerMaskGenerator.GetPositionOnRegion(handVisual, jointIdStart, minRegion, sideLength);
			Vector2 vector = HandFingerMaskGenerator.GetPositionOnRegion(handVisual, jointIdEnd, minRegion, sideLength);
			vector = Vector2.LerpUnclamped(positionOnRegion, vector, lineScale);
			return new Vector4(positionOnRegion.x, positionOnRegion.y, vector.x, vector.y);
		}

		private static void SetGlowModelUV(SkinnedMeshRenderer handRenderer, Handedness handedness, out Vector2 minPosition, out Vector2 maxPosition)
		{
			Mesh sharedMesh = handRenderer.sharedMesh;
			List<Vector2> uvs = HandFingerMaskGenerator.GenerateModelUV(handedness, sharedMesh, out minPosition, out maxPosition);
			sharedMesh.SetUVs(1, uvs);
			sharedMesh.UploadMeshData(false);
		}

		private static void SetFingerMaskUniforms(HandVisual handVisual, MaterialPropertyBlock materialPropertyBlock, Vector2 minPosition, Vector2 maxPosition)
		{
			Vector2 vector = maxPosition - minPosition;
			float maxLength = Mathf.Max(vector.x, vector.y);
			float[] lineScale = new float[]
			{
				0.9f,
				0.91f,
				0.9f,
				0.87f,
				0.87f
			};
			Vector4[] array = HandFingerMaskGenerator.GenerateFingerLines(handVisual, minPosition, maxLength, lineScale);
			array[0].z = Mathf.Lerp(array[0].z, array[0].x, 0.3f);
			array[0].x = array[0].z;
			float[] lineScale2 = new float[]
			{
				1.2f,
				1.25f,
				1.25f,
				1.25f,
				1.25f
			};
			Vector4[] array2 = HandFingerMaskGenerator.GenerateFingerLines(handVisual, minPosition, maxLength, lineScale2);
			float num = Mathf.Abs(array2[0].x - array2[0].z) * 0.1f;
			Vector4[] array3 = array2;
			int num2 = 0;
			array3[num2].z = array3[num2].z + num;
			for (int i = 0; i < 5; i++)
			{
				materialPropertyBlock.SetVector(HandFingerMaskGenerator._fingerLinesID[i], array[i]);
				materialPropertyBlock.SetVector(HandFingerMaskGenerator._palmFingerLinesID[i], array2[i]);
			}
		}

		public static void GenerateFingerMask(SkinnedMeshRenderer handRenderer, HandVisual handVisual, MaterialPropertyBlock materialPropertyBlock)
		{
			Vector2 minPosition;
			Vector2 maxPosition;
			HandFingerMaskGenerator.SetGlowModelUV(handRenderer, handVisual.Hand.Handedness, out minPosition, out maxPosition);
			HandFingerMaskGenerator.SetFingerMaskUniforms(handVisual, materialPropertyBlock, minPosition, maxPosition);
		}

		private static readonly int[] _fingerLinesID = new int[]
		{
			Shader.PropertyToID("_ThumbLine"),
			Shader.PropertyToID("_IndexLine"),
			Shader.PropertyToID("_MiddleLine"),
			Shader.PropertyToID("_RingLine"),
			Shader.PropertyToID("_PinkyLine")
		};

		private static readonly int[] _palmFingerLinesID = new int[]
		{
			Shader.PropertyToID("_PalmThumbLine"),
			Shader.PropertyToID("_PalmIndexLine"),
			Shader.PropertyToID("_PalmMiddleLine"),
			Shader.PropertyToID("_PalmRingLine"),
			Shader.PropertyToID("_PalmPinkyLine")
		};
	}
}
