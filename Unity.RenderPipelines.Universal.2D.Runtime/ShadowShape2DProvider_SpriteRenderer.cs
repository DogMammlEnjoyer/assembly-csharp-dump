using System;
using Unity.Collections;
using UnityEngine.Events;
using UnityEngine.U2D;

namespace UnityEngine.Rendering.Universal
{
	[Serializable]
	internal class ShadowShape2DProvider_SpriteRenderer : ShadowShape2DProvider
	{
		private void SetFullRectShapeData(SpriteRenderer spriteRenderer, ShadowShape2D shadowShape2D)
		{
			if (spriteRenderer.drawMode != SpriteDrawMode.Simple)
			{
				Sprite sprite = spriteRenderer.sprite;
				Vector2 size = spriteRenderer.size;
				Vector3 a = new Vector2(size.x * sprite.pivot.x / sprite.rect.width, size.y * sprite.pivot.y / sprite.rect.height);
				Rect rect = new Rect(-a, new Vector2(size.x, size.y));
				NativeArray<Vector3> vertices = new NativeArray<Vector3>(4, Allocator.Temp, NativeArrayOptions.ClearMemory);
				NativeArray<int> indices = new NativeArray<int>(8, Allocator.Temp, NativeArrayOptions.ClearMemory);
				vertices[0] = new Vector3(rect.min.x, rect.min.y);
				vertices[1] = new Vector3(rect.min.x, rect.max.y);
				vertices[2] = new Vector3(rect.max.x, rect.max.y);
				vertices[3] = new Vector3(rect.max.x, rect.min.y);
				indices[0] = 0;
				indices[1] = 1;
				indices[2] = 1;
				indices[3] = 2;
				indices[4] = 2;
				indices[5] = 3;
				indices[6] = 3;
				indices[7] = 0;
				shadowShape2D.SetShape(vertices, indices, ShadowShape2D.OutlineTopology.Lines, ShadowShape2D.WindingOrder.Clockwise, true, false);
				vertices.Dispose();
				indices.Dispose();
			}
		}

		private void SetPersistantShapeData(Sprite sprite, ShadowShape2D shadowShape2D, NativeSlice<Vector3> vertexSlice)
		{
			if (shadowShape2D != null)
			{
				NativeArray<ushort> indices = sprite.GetIndices();
				NativeArray<int> indices2 = new NativeArray<int>(indices.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
				NativeArray<Vector3> vertices = new NativeArray<Vector3>(vertexSlice.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
				for (int i = 0; i < indices2.Length; i++)
				{
					indices2[i] = (int)indices[i];
				}
				for (int j = 0; j < vertices.Length; j++)
				{
					vertices[j] = vertexSlice[j];
				}
				shadowShape2D.SetShape(vertices, indices2, ShadowShape2D.OutlineTopology.Triangles, ShadowShape2D.WindingOrder.Clockwise, true, false);
				vertices.Dispose();
				indices2.Dispose();
			}
		}

		private void TryToSetPersistantShapeData(SpriteRenderer spriteRenderer, ShadowShape2D persistantShadowShape, bool force)
		{
			if (spriteRenderer != null && spriteRenderer.sprite != null)
			{
				if (spriteRenderer.drawMode != SpriteDrawMode.Simple && (spriteRenderer.size.x != this.m_CurrentDrawModeSize.x || spriteRenderer.size.y != this.m_CurrentDrawModeSize.y || spriteRenderer.drawMode != this.m_CurrentDrawMode || force))
				{
					this.m_CurrentDrawModeSize = spriteRenderer.size;
					this.SetFullRectShapeData(spriteRenderer, persistantShadowShape);
				}
				else if (spriteRenderer.drawMode != this.m_CurrentDrawMode || force)
				{
					Sprite sprite = spriteRenderer.sprite;
					NativeSlice<Vector3> vertexAttribute = sprite.GetVertexAttribute(VertexAttribute.Position);
					this.SetPersistantShapeData(sprite, this.m_PersistantShapeData, vertexAttribute);
				}
				this.m_CurrentDrawMode = spriteRenderer.drawMode;
			}
		}

		private void UpdatePersistantShapeData(SpriteRenderer spriteRenderer)
		{
			this.TryToSetPersistantShapeData(spriteRenderer, this.m_PersistantShapeData, true);
		}

		public override int Priority()
		{
			return 1;
		}

		public override bool IsShapeSource(Component sourceComponent)
		{
			return sourceComponent is SpriteRenderer;
		}

		public override void OnPersistantDataCreated(Component sourceComponent, ShadowShape2D persistantShadowShape)
		{
			SpriteRenderer spriteRenderer = (SpriteRenderer)sourceComponent;
			this.m_PersistantShapeData = (persistantShadowShape as ShadowMesh2D);
			if (spriteRenderer.sprite != null)
			{
				float trimEdgeFromBounds = ShadowShapeProvider2DUtility.GetTrimEdgeFromBounds(spriteRenderer.bounds, 0.05f);
				persistantShadowShape.SetDefaultTrim(trimEdgeFromBounds);
			}
			this.TryToSetPersistantShapeData(spriteRenderer, persistantShadowShape, true);
		}

		public override void OnBeforeRender(Component sourceComponent, Bounds worldCullingBounds, ShadowShape2D persistantShadowShape)
		{
			SpriteRenderer spriteRenderer = (SpriteRenderer)sourceComponent;
			persistantShadowShape.SetFlip(spriteRenderer.flipX, spriteRenderer.flipY);
			this.TryToSetPersistantShapeData(spriteRenderer, persistantShadowShape, false);
		}

		public override void Enabled(Component sourceComponent, ShadowShape2D persistantShadowShape)
		{
			SpriteRenderer spriteRenderer = (SpriteRenderer)sourceComponent;
			this.m_PersistantShapeData = persistantShadowShape;
			spriteRenderer.RegisterSpriteChangeCallback(new UnityAction<SpriteRenderer>(this.UpdatePersistantShapeData));
		}

		public override void Disabled(Component sourceComponent, ShadowShape2D persistantShadowShape)
		{
			((SpriteRenderer)sourceComponent).UnregisterSpriteChangeCallback(new UnityAction<SpriteRenderer>(this.UpdatePersistantShapeData));
		}

		private const float k_InitialTrim = 0.05f;

		private ShadowShape2D m_PersistantShapeData;

		private SpriteDrawMode m_CurrentDrawMode;

		private Vector2 m_CurrentDrawModeSize;
	}
}
