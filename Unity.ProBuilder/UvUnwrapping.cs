using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.ProBuilder
{
	internal static class UvUnwrapping
	{
		internal static void SetAutoUV(ProBuilderMesh mesh, Face[] faces, bool auto)
		{
			if (auto)
			{
				UvUnwrapping.SetAutoAndAlignUnwrapParamsToUVs(mesh, from x in faces
				where x.manualUV
				select x);
				return;
			}
			foreach (Face face in faces)
			{
				face.textureGroup = -1;
				face.manualUV = true;
			}
		}

		internal static void SetAutoAndAlignUnwrapParamsToUVs(ProBuilderMesh mesh, IEnumerable<Face> facesToConvert)
		{
			Vector2[] dst = mesh.textures.ToArray<Vector2>();
			Face[] array = (facesToConvert as Face[]) ?? facesToConvert.ToArray<Face>();
			foreach (Face face in array)
			{
				face.uv = AutoUnwrapSettings.defaultAutoUnwrapSettings;
				face.elementGroup = -1;
				face.textureGroup = -1;
				face.manualUV = false;
			}
			mesh.RefreshUV(array);
			Vector2[] texturesInternal = mesh.texturesInternal;
			foreach (Face face2 in array)
			{
				UvUnwrapping.UVTransform uvtransform = UvUnwrapping.CalculateDelta(texturesInternal, face2.indexesInternal, dst, face2.indexesInternal);
				AutoUnwrapSettings uv = face2.uv;
				uv.offset = -uvtransform.translation;
				uv.rotation = uvtransform.rotation;
				uv.scale = uvtransform.scale;
				face2.uv = uv;
			}
			mesh.RefreshUV(array);
		}

		internal static AutoUnwrapSettings GetAutoUnwrapSettings(ProBuilderMesh mesh, Face face)
		{
			if (!face.manualUV)
			{
				return new AutoUnwrapSettings(face.uv);
			}
			UvUnwrapping.UVTransform uvtransform = UvUnwrapping.GetUVTransform(mesh, face);
			AutoUnwrapSettings defaultAutoUnwrapSettings = AutoUnwrapSettings.defaultAutoUnwrapSettings;
			defaultAutoUnwrapSettings.offset = uvtransform.translation;
			defaultAutoUnwrapSettings.rotation = 360f - uvtransform.rotation;
			defaultAutoUnwrapSettings.scale /= uvtransform.scale;
			return defaultAutoUnwrapSettings;
		}

		internal static UvUnwrapping.UVTransform GetUVTransform(ProBuilderMesh mesh, Face face)
		{
			Projection.PlanarProject(mesh.positionsInternal, face.indexesInternal, Math.Normal(mesh, face), UvUnwrapping.s_UVTransformProjectionBuffer);
			return UvUnwrapping.CalculateDelta(mesh.texturesInternal, face.indexesInternal, UvUnwrapping.s_UVTransformProjectionBuffer, null);
		}

		private static int GetIndex(IList<int> collection, int index)
		{
			if (collection != null)
			{
				return collection[index];
			}
			return index;
		}

		internal static UvUnwrapping.UVTransform CalculateDelta(IList<Vector2> src, IList<int> srcIndices, IList<Vector2> dst, IList<int> dstIndices)
		{
			Vector2 vector = src[UvUnwrapping.GetIndex(srcIndices, 1)] - src[UvUnwrapping.GetIndex(srcIndices, 0)];
			Vector2 vector2 = dst[UvUnwrapping.GetIndex(dstIndices, 1)] - dst[UvUnwrapping.GetIndex(dstIndices, 0)];
			float num = Vector2.Angle(vector2, vector);
			if (Vector2.Dot(Vector2.Perpendicular(vector2), vector) < 0f)
			{
				num = 360f - num;
			}
			Vector2 vector3 = (dstIndices == null) ? Bounds2D.Center(dst) : Bounds2D.Center(dst, dstIndices);
			Vector2 rotatedSize = UvUnwrapping.GetRotatedSize(dst, dstIndices, vector3, -num);
			Bounds2D bounds2D = (srcIndices == null) ? new Bounds2D(src) : new Bounds2D(src, srcIndices);
			Vector2 b = rotatedSize.DivideBy(bounds2D.size);
			Vector2 vector4 = new Vector2(float.MaxValue, float.MaxValue);
			Vector2 vector5 = new Vector2(float.MinValue, float.MinValue);
			int num2 = (srcIndices != null) ? srcIndices.Count : src.Count;
			for (int i = 0; i < num2; i++)
			{
				int index = UvUnwrapping.GetIndex(srcIndices, i);
				Vector2 vector6 = src[index].RotateAroundPoint(bounds2D.center, num);
				vector4.x = Mathf.Min(vector4.x, vector6.x);
				vector4.y = Mathf.Min(vector4.y, vector6.y);
				vector5.x = Mathf.Max(vector5.x, vector6.x);
				vector5.y = Mathf.Max(vector5.y, vector6.y);
			}
			Vector2 b2 = (vector4 + vector5) * 0.5f * b;
			return new UvUnwrapping.UVTransform
			{
				translation = vector3 - b2,
				rotation = num,
				scale = rotatedSize.DivideBy(bounds2D.size)
			};
		}

		private static Vector2 GetRotatedSize(IList<Vector2> points, IList<int> indices, Vector2 center, float rotation)
		{
			int num = (indices == null) ? points.Count : indices.Count;
			Vector2 vector = points[UvUnwrapping.GetIndex(indices, 0)].RotateAroundPoint(center, rotation);
			float num2 = vector.x;
			float num3 = vector.y;
			float num4 = num2;
			float num5 = num3;
			for (int i = 1; i < num; i++)
			{
				Vector2 vector2 = points[UvUnwrapping.GetIndex(indices, i)].RotateAroundPoint(center, rotation);
				float x = vector2.x;
				float y = vector2.y;
				if (x < num2)
				{
					num2 = x;
				}
				if (x > num4)
				{
					num4 = x;
				}
				if (y < num3)
				{
					num3 = y;
				}
				if (y > num5)
				{
					num5 = y;
				}
			}
			return new Vector2(num4 - num2, num5 - num3);
		}

		internal static void Unwrap(ProBuilderMesh mesh, Face face, Vector3 projection = default(Vector3))
		{
			Projection.PlanarProject(mesh, face, (projection != Vector3.zero) ? projection : Vector3.zero);
			UvUnwrapping.ApplyUVSettings(mesh.texturesInternal, face.distinctIndexesInternal, face.uv);
		}

		internal static void CopyUVs(ProBuilderMesh mesh, Face source, Face dest)
		{
			Vector2[] texturesInternal = mesh.texturesInternal;
			int[] distinctIndexesInternal = source.distinctIndexesInternal;
			int[] distinctIndexesInternal2 = dest.distinctIndexesInternal;
			for (int i = 0; i < distinctIndexesInternal.Length; i++)
			{
				texturesInternal[distinctIndexesInternal2[i]].x = texturesInternal[distinctIndexesInternal[i]].x;
				texturesInternal[distinctIndexesInternal2[i]].y = texturesInternal[distinctIndexesInternal[i]].y;
			}
		}

		internal static void ProjectTextureGroup(ProBuilderMesh mesh, int group, AutoUnwrapSettings unwrapSettings)
		{
			Projection.PlanarProject(mesh, group, unwrapSettings);
			UvUnwrapping.s_IndexBuffer.Clear();
			foreach (Face face in mesh.facesInternal)
			{
				if (face.textureGroup == group)
				{
					UvUnwrapping.s_IndexBuffer.AddRange(face.distinctIndexesInternal);
				}
			}
			UvUnwrapping.ApplyUVSettings(mesh.texturesInternal, UvUnwrapping.s_IndexBuffer, unwrapSettings);
		}

		private static void ApplyUVSettings(Vector2[] uvs, IList<int> indexes, AutoUnwrapSettings uvSettings)
		{
			int count = indexes.Count;
			Bounds2D bounds2D = new Bounds2D(uvs, indexes);
			switch (uvSettings.fill)
			{
			case AutoUnwrapSettings.Fill.Fit:
			{
				float num = Mathf.Max(bounds2D.size.x, bounds2D.size.y);
				UvUnwrapping.ScaleUVs(uvs, indexes, new Vector2(num, num), bounds2D);
				bounds2D.center /= num;
				break;
			}
			case AutoUnwrapSettings.Fill.Stretch:
				UvUnwrapping.ScaleUVs(uvs, indexes, bounds2D.size, bounds2D);
				bounds2D.center /= bounds2D.size;
				break;
			}
			if (uvSettings.scale.x != 1f || uvSettings.scale.y != 1f || uvSettings.rotation != 0f)
			{
				Vector2 vector = bounds2D.center * uvSettings.scale;
				Vector2 b = bounds2D.center - vector;
				Vector2 origin = vector;
				for (int i = 0; i < count; i++)
				{
					uvs[indexes[i]] -= b;
					uvs[indexes[i]] = uvs[indexes[i]].ScaleAroundPoint(origin, uvSettings.scale);
					uvs[indexes[i]] = uvs[indexes[i]].RotateAroundPoint(origin, uvSettings.rotation);
				}
			}
			if (!uvSettings.useWorldSpace && uvSettings.anchor != AutoUnwrapSettings.Anchor.None)
			{
				UvUnwrapping.ApplyUVAnchor(uvs, indexes, uvSettings.anchor);
			}
			if (uvSettings.flipU || uvSettings.flipV || uvSettings.swapUV)
			{
				for (int j = 0; j < count; j++)
				{
					float num2 = uvs[indexes[j]].x;
					float num3 = uvs[indexes[j]].y;
					if (uvSettings.flipU)
					{
						num2 = -num2;
					}
					if (uvSettings.flipV)
					{
						num3 = -num3;
					}
					if (!uvSettings.swapUV)
					{
						uvs[indexes[j]].x = num2;
						uvs[indexes[j]].y = num3;
					}
					else
					{
						uvs[indexes[j]].x = num3;
						uvs[indexes[j]].y = num2;
					}
				}
			}
			for (int k = 0; k < indexes.Count; k++)
			{
				int num4 = indexes[k];
				uvs[num4].x = uvs[num4].x - uvSettings.offset.x;
				int num5 = indexes[k];
				uvs[num5].y = uvs[num5].y - uvSettings.offset.y;
			}
		}

		private static void ScaleUVs(Vector2[] uvs, IList<int> indexes, Vector2 scale, Bounds2D bounds)
		{
			Vector2 vector = bounds.center;
			Vector2 vector2 = vector / scale;
			Vector2 b = vector - vector2;
			vector = vector2;
			for (int i = 0; i < indexes.Count; i++)
			{
				Vector2 vector3 = uvs[indexes[i]] - b;
				vector3.x = (vector3.x - vector.x) / scale.x + vector.x;
				vector3.y = (vector3.y - vector.y) / scale.y + vector.y;
				uvs[indexes[i]] = vector3;
			}
		}

		private static void ApplyUVAnchor(Vector2[] uvs, IList<int> indexes, AutoUnwrapSettings.Anchor anchor)
		{
			UvUnwrapping.s_TempVector2.x = 0f;
			UvUnwrapping.s_TempVector2.y = 0f;
			Vector2 vector = Math.SmallestVector2(uvs, indexes);
			Vector2 vector2 = Math.LargestVector2(uvs, indexes);
			if (anchor == AutoUnwrapSettings.Anchor.UpperLeft || anchor == AutoUnwrapSettings.Anchor.MiddleLeft || anchor == AutoUnwrapSettings.Anchor.LowerLeft)
			{
				UvUnwrapping.s_TempVector2.x = vector.x;
			}
			else if (anchor == AutoUnwrapSettings.Anchor.UpperRight || anchor == AutoUnwrapSettings.Anchor.MiddleRight || anchor == AutoUnwrapSettings.Anchor.LowerRight)
			{
				UvUnwrapping.s_TempVector2.x = vector2.x - 1f;
			}
			else
			{
				UvUnwrapping.s_TempVector2.x = vector.x + (vector2.x - vector.x) * 0.5f - 0.5f;
			}
			if (anchor == AutoUnwrapSettings.Anchor.UpperLeft || anchor == AutoUnwrapSettings.Anchor.UpperCenter || anchor == AutoUnwrapSettings.Anchor.UpperRight)
			{
				UvUnwrapping.s_TempVector2.y = vector2.y - 1f;
			}
			else if (anchor == AutoUnwrapSettings.Anchor.MiddleLeft || anchor == AutoUnwrapSettings.Anchor.MiddleCenter || anchor == AutoUnwrapSettings.Anchor.MiddleRight)
			{
				UvUnwrapping.s_TempVector2.y = vector.y + (vector2.y - vector.y) * 0.5f - 0.5f;
			}
			else
			{
				UvUnwrapping.s_TempVector2.y = vector.y;
			}
			int count = indexes.Count;
			for (int i = 0; i < count; i++)
			{
				int num = indexes[i];
				uvs[num].x = uvs[num].x - UvUnwrapping.s_TempVector2.x;
				int num2 = indexes[i];
				uvs[num2].y = uvs[num2].y - UvUnwrapping.s_TempVector2.y;
			}
		}

		internal static void UpgradeAutoUVScaleOffset(ProBuilderMesh mesh)
		{
			Vector2[] src = mesh.textures.ToArray<Vector2>();
			mesh.RefreshUV(mesh.facesInternal);
			Vector2[] texturesInternal = mesh.texturesInternal;
			foreach (Face face in mesh.facesInternal)
			{
				if (!face.manualUV)
				{
					UvUnwrapping.UVTransform uvtransform = UvUnwrapping.CalculateDelta(src, face.indexesInternal, texturesInternal, face.indexesInternal);
					AutoUnwrapSettings uv = face.uv;
					uv.offset += uvtransform.translation;
					face.uv = uv;
				}
			}
		}

		private static List<Vector2> s_UVTransformProjectionBuffer = new List<Vector2>(8);

		private static Vector2 s_TempVector2 = Vector2.zero;

		private static readonly List<int> s_IndexBuffer = new List<int>(64);

		internal struct UVTransform
		{
			public override string ToString()
			{
				return string.Concat(new string[]
				{
					this.translation.ToString("F2"),
					", ",
					this.rotation.ToString(),
					", ",
					this.scale.ToString("F2")
				});
			}

			public Vector2 translation;

			public float rotation;

			public Vector2 scale;
		}
	}
}
