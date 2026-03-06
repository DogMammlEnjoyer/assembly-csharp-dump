using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UnityEngine.Rendering.Universal
{
	[Serializable]
	internal class ShadowShape2DProvider_Collider2D : ShadowShape2DProvider
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool CompareApproximately(ref Bounds a, ref Bounds b)
		{
			return (a.min - b.min).sqrMagnitude <= Mathf.Epsilon && (a.max - b.max).sqrMagnitude <= Mathf.Epsilon;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void TransformBounds2D(Matrix4x4 transform, ref Bounds bounds)
		{
			Vector3 center = transform.MultiplyPoint(bounds.center);
			Vector3 extents = bounds.extents;
			Vector3 vector = transform.MultiplyVector(new Vector3(extents.x, 0f, 0f));
			Vector3 vector2 = transform.MultiplyVector(new Vector3(0f, extents.y, 0f));
			extents.x = MathF.Abs(vector.x) + MathF.Abs(vector2.x);
			extents.y = MathF.Abs(vector.y) + MathF.Abs(vector2.y);
			bounds = new Bounds
			{
				center = center,
				extents = extents
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ClearShapes(ShadowShape2D persistantShapeObject)
		{
			persistantShapeObject.SetShape(default(NativeArray<Vector3>), default(NativeArray<int>), ShadowShape2D.OutlineTopology.Lines, ShadowShape2D.WindingOrder.CounterClockwise, true, false);
		}

		private void CalculateShadows(Collider2D collider, ShadowShape2D persistantShapeObject, Bounds worldCullingBounds)
		{
			if (this.m_ShadowShapeGroup == null)
			{
				this.m_ShadowShapeGroup = new PhysicsShapeGroup2D(collider.shapeCount, 8);
			}
			if (this.m_ShadowShapeBounds == null)
			{
				this.m_ShadowShapeBounds = new List<Bounds>(collider.shapeCount);
			}
			if (this.m_ShadowShapeMinMaxBounds == null)
			{
				this.m_ShadowShapeMinMaxBounds = new List<ShadowShape2DProvider_Collider2D.MinMaxBounds>();
			}
			Rigidbody2D attachedRigidbody = collider.attachedRigidbody;
			Matrix4x4 matrix4x = attachedRigidbody ? attachedRigidbody.localToWorldMatrix : Matrix4x4.identity;
			uint shapeHash = collider.GetShapeHash();
			if (shapeHash != this.m_ShadowStateHash)
			{
				this.m_ShadowStateHash = shapeHash;
				this.m_ShadowShapeGroup.Clear();
				if (collider.shapeCount == 0)
				{
					ShadowShape2DProvider_Collider2D.ClearShapes(persistantShapeObject);
					return;
				}
				if (collider.GetShapes(this.m_ShadowShapeGroup) == 0)
				{
					return;
				}
				this.m_LastWorldCullingBounds = worldCullingBounds;
				Bounds shapeBounds = collider.GetShapeBounds(this.m_ShadowShapeBounds, true, false);
				this.m_ShadowCombinedShapeMinMaxBounds = new ShadowShape2DProvider_Collider2D.MinMaxBounds(ref shapeBounds);
				this.m_ShadowShapeMinMaxBounds.Clear();
				this.m_ShadowShapeMinMaxBounds.Capacity = this.m_ShadowShapeBounds.Capacity;
				for (int i = 0; i < this.m_ShadowShapeBounds.Count; i++)
				{
					Bounds bounds = this.m_ShadowShapeBounds[i];
					this.m_ShadowShapeMinMaxBounds.Add(new ShadowShape2DProvider_Collider2D.MinMaxBounds(ref bounds));
				}
				this.m_ShadowDirty = true;
			}
			else
			{
				if (matrix4x.Equals(this.m_LastColliderSpace) && ShadowShape2DProvider_Collider2D.CompareApproximately(ref this.m_LastWorldCullingBounds, ref worldCullingBounds))
				{
					return;
				}
				this.m_LastWorldCullingBounds = worldCullingBounds;
				this.m_ShadowDirty = true;
			}
			this.m_LastColliderSpace = matrix4x;
			if (!this.m_ShadowDirty || this.m_ShadowShapeGroup.shapeCount == 0)
			{
				return;
			}
			this.m_ShadowDirty = false;
			ShadowShape2DProvider_Collider2D.TransformBounds2D(Matrix4x4.Inverse(matrix4x), ref worldCullingBounds);
			ShadowShape2DProvider_Collider2D.MinMaxBounds minMaxBounds = new ShadowShape2DProvider_Collider2D.MinMaxBounds(ref worldCullingBounds);
			if (!this.m_ShadowCombinedShapeMinMaxBounds.Intersects(ref minMaxBounds))
			{
				ShadowShape2DProvider_Collider2D.ClearShapes(persistantShapeObject);
				return;
			}
			int shapeCount = this.m_ShadowShapeGroup.shapeCount;
			List<PhysicsShape2D> groupShapes = this.m_ShadowShapeGroup.groupShapes;
			List<Vector2> groupVertices = this.m_ShadowShapeGroup.groupVertices;
			NativeArray<int> nativeArray = new NativeArray<int>(shapeCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (int j = 0; j < shapeCount; j++)
			{
				if (this.m_ShadowShapeMinMaxBounds[j].Intersects(ref minMaxBounds))
				{
					PhysicsShape2D physicsShape2D = groupShapes[j];
					int vertexCount = physicsShape2D.vertexCount;
					PhysicsShapeType2D shapeType = physicsShape2D.shapeType;
					num += vertexCount;
					switch (shapeType)
					{
					case PhysicsShapeType2D.Circle:
					case PhysicsShapeType2D.Capsule:
						num2 += 2;
						break;
					case PhysicsShapeType2D.Polygon:
						num2 += 2 * vertexCount;
						break;
					case PhysicsShapeType2D.Edges:
					{
						Vector2 b = groupVertices[physicsShape2D.vertexStartIndex];
						bool flag = (groupVertices[physicsShape2D.vertexStartIndex + physicsShape2D.vertexCount - 1] - b).sqrMagnitude > Mathf.Epsilon;
						num2 += 2 * (flag ? (vertexCount - 1) : vertexCount);
						break;
					}
					}
					nativeArray[num3++] = j;
				}
			}
			if (num3 > 0)
			{
				NativeArray<float> radii = new NativeArray<float>(num, Allocator.Temp, NativeArrayOptions.ClearMemory);
				NativeArray<Vector3> vertices = new NativeArray<Vector3>(num, Allocator.Temp, NativeArrayOptions.ClearMemory);
				NativeArray<int> indices = new NativeArray<int>(num2, Allocator.Temp, NativeArrayOptions.ClearMemory);
				int num4 = 0;
				int num5 = 0;
				for (int k = 0; k < num3; k++)
				{
					PhysicsShape2D physicsShape2D2 = groupShapes[nativeArray[k]];
					PhysicsShapeType2D shapeType2 = physicsShape2D2.shapeType;
					float radius = physicsShape2D2.radius;
					int vertexStartIndex = physicsShape2D2.vertexStartIndex;
					int vertexCount2 = physicsShape2D2.vertexCount;
					switch (shapeType2)
					{
					case PhysicsShapeType2D.Circle:
						radii[num4] = radius;
						indices[num5++] = num4;
						indices[num5++] = num4;
						vertices[num4++] = groupVertices[vertexStartIndex];
						break;
					case PhysicsShapeType2D.Capsule:
						radii[num4] = radius;
						indices[num5++] = num4;
						vertices[num4++] = groupVertices[vertexStartIndex++];
						radii[num4] = radius;
						indices[num5++] = num4;
						vertices[num4++] = groupVertices[vertexStartIndex++];
						break;
					case PhysicsShapeType2D.Polygon:
					{
						int value = num4;
						int value2 = num4;
						for (int l = 0; l < vertexCount2 - 1; l++)
						{
							radii[num4] = radius;
							vertices[num4++] = groupVertices[vertexStartIndex++];
							indices[num5++] = value2++;
							indices[num5++] = value2;
						}
						radii[num4] = radius;
						vertices[num4++] = groupVertices[vertexStartIndex++];
						indices[num5++] = value2;
						indices[num5++] = value;
						break;
					}
					case PhysicsShapeType2D.Edges:
					{
						int value3 = num4;
						int value4 = num4;
						for (int m = 0; m < vertexCount2 - 1; m++)
						{
							radii[num4] = radius;
							vertices[num4++] = groupVertices[vertexStartIndex++];
							indices[num5++] = value4++;
							indices[num5++] = value4;
						}
						radii[num4] = radius;
						vertices[num4++] = groupVertices[vertexStartIndex++];
						Vector2 b2 = groupVertices[physicsShape2D2.vertexStartIndex];
						if ((groupVertices[physicsShape2D2.vertexStartIndex + physicsShape2D2.vertexCount - 1] - b2).sqrMagnitude <= Mathf.Epsilon)
						{
							indices[num5++] = value4;
							indices[num5++] = value3;
						}
						break;
					}
					}
				}
				Matrix4x4 transform = collider.transform.worldToLocalMatrix * matrix4x;
				Renderer renderer;
				bool createInteriorGeometry = !collider.TryGetComponent<Renderer>(out renderer);
				persistantShapeObject.SetShape(vertices, indices, radii, transform, ShadowShape2D.WindingOrder.CounterClockwise, true, createInteriorGeometry);
				indices.Dispose();
				vertices.Dispose();
				radii.Dispose();
			}
			else
			{
				ShadowShape2DProvider_Collider2D.ClearShapes(persistantShapeObject);
			}
			nativeArray.Dispose();
		}

		public override bool IsShapeSource(Component sourceComponent)
		{
			return sourceComponent is Collider2D;
		}

		public override void OnPersistantDataCreated(Component sourceComponent, ShadowShape2D persistantShadowShapeData)
		{
			this.m_ShadowStateHash = 0U;
			this.m_ShadowCombinedShapeMinMaxBounds = default(ShadowShape2DProvider_Collider2D.MinMaxBounds);
			this.m_LastColliderSpace = Matrix4x4.identity;
		}

		public override void OnBeforeRender(Component sourceComponent, Bounds worldCullingBounds, ShadowShape2D persistantShadowShape)
		{
			Collider2D collider = (Collider2D)sourceComponent;
			this.CalculateShadows(collider, persistantShadowShape, worldCullingBounds);
		}

		private const float k_InitialTrim = 0.05f;

		private List<Bounds> m_ShadowShapeBounds;

		private List<ShadowShape2DProvider_Collider2D.MinMaxBounds> m_ShadowShapeMinMaxBounds;

		private ShadowShape2DProvider_Collider2D.MinMaxBounds m_ShadowCombinedShapeMinMaxBounds;

		private Bounds m_LastWorldCullingBounds;

		private Matrix4x4 m_LastColliderSpace;

		private bool m_ShadowDirty = true;

		private uint m_ShadowStateHash;

		private PhysicsShapeGroup2D m_ShadowShapeGroup;

		private struct MinMaxBounds
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool Intersects(ref ShadowShape2DProvider_Collider2D.MinMaxBounds bounds)
			{
				return this.min.x <= bounds.max.x && this.max.x >= bounds.min.x && this.min.y <= bounds.max.y && this.max.y >= bounds.min.y && this.min.z <= bounds.max.z && this.max.z >= bounds.min.z;
			}

			public MinMaxBounds(ref Bounds bounds)
			{
				this.min = bounds.min;
				this.max = bounds.max;
			}

			public Vector3 min;

			public Vector3 max;
		}
	}
}
