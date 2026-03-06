using System;
using Unity.Collections;

namespace UnityEngine.UIElements.UIR
{
	internal abstract class BaseElementBuilder
	{
		public abstract bool RequiresStencilMask(VisualElement ve);

		public void Build(MeshGenerationContext mgc)
		{
			bool isSubTreeQuad = mgc.renderData.isSubTreeQuad;
			if (isSubTreeQuad)
			{
				this.BuildRenderTreeQuadElement(mgc);
			}
			else
			{
				this.BuildStandardElement(mgc);
			}
		}

		private void BuildRenderTreeQuadElement(MeshGenerationContext mgc)
		{
			VisualElement visualElement = mgc.visualElement;
			RenderTree renderTree = visualElement.nestedRenderData.renderTree;
			RectInt quadRect = renderTree.quadRect;
			bool flag = quadRect != RectInt.zero;
			if (flag)
			{
				Color white = Color.white;
				NativeSlice<Vertex> vertices;
				NativeSlice<ushort> indices;
				mgc.AllocateTempMesh(4, 6, out vertices, out indices);
				vertices[0] = new Vertex
				{
					position = new Vector3((float)quadRect.xMin, (float)quadRect.yMax, Vertex.nearZ),
					tint = white,
					uv = new Vector2(0f, 0f)
				};
				vertices[1] = new Vertex
				{
					position = new Vector3((float)quadRect.xMin, (float)quadRect.yMin, Vertex.nearZ),
					tint = white,
					uv = new Vector2(0f, 1f)
				};
				vertices[2] = new Vertex
				{
					position = new Vector3((float)quadRect.xMax, (float)quadRect.yMin, Vertex.nearZ),
					tint = white,
					uv = new Vector2(1f, 1f)
				};
				vertices[3] = new Vertex
				{
					position = new Vector3((float)quadRect.xMax, (float)quadRect.yMax, Vertex.nearZ),
					tint = white,
					uv = new Vector2(1f, 0f)
				};
				indices[0] = 0;
				indices[1] = 1;
				indices[2] = 2;
				indices[3] = 2;
				indices[4] = 3;
				indices[5] = 0;
				mgc.entryRecorder.DrawMesh(mgc.parentEntry, vertices, indices, renderTree.quadTextureId, true);
			}
			mgc.entryRecorder.DrawChildren(mgc.parentEntry);
		}

		private void BuildStandardElement(MeshGenerationContext mgc)
		{
			VisualElement visualElement = mgc.visualElement;
			RenderData renderData = mgc.renderData;
			Debug.Assert(visualElement.areAncestorsAndSelfDisplayed);
			bool isWorldSpaceRootUIDocument = visualElement.isWorldSpaceRootUIDocument;
			if (isWorldSpaceRootUIDocument)
			{
				mgc.entryRecorder.CutRenderChain(mgc.parentEntry);
			}
			bool isGroupTransform = renderData.isGroupTransform;
			bool flag = isGroupTransform;
			if (flag)
			{
				mgc.entryRecorder.PushGroupMatrix(mgc.parentEntry);
			}
			bool flag2 = false;
			bool visible = visualElement.visible;
			if (visible)
			{
				this.DrawVisualElementBackground(mgc);
				this.DrawVisualElementBorder(mgc);
				this.PushVisualElementClipping(mgc);
				flag2 = true;
				BaseElementBuilder.InvokeGenerateVisualContent(mgc);
			}
			else
			{
				bool flag3 = renderData.clipMethod == ClipMethod.Stencil;
				bool flag4 = renderData.clipMethod == ClipMethod.Scissor;
				bool flag5 = flag4 || flag3;
				if (flag5)
				{
					flag2 = true;
					this.PushVisualElementClipping(mgc);
				}
			}
			mgc.entryRecorder.DrawChildren(mgc.parentEntry);
			bool flag6 = flag2;
			if (flag6)
			{
				BaseElementBuilder.PopVisualElementClipping(mgc);
			}
			bool flag7 = isGroupTransform;
			if (flag7)
			{
				mgc.entryRecorder.PopGroupMatrix(mgc.parentEntry);
			}
		}

		protected abstract void DrawVisualElementBackground(MeshGenerationContext mgc);

		protected abstract void DrawVisualElementBorder(MeshGenerationContext mgc);

		protected abstract void DrawVisualElementStencilMask(MeshGenerationContext mgc);

		public abstract void ScheduleMeshGenerationJobs(MeshGenerationContext mgc);

		private void PushVisualElementClipping(MeshGenerationContext mgc)
		{
			RenderData renderData = mgc.renderData;
			bool flag = renderData.clipMethod == ClipMethod.Scissor;
			if (flag)
			{
				mgc.entryRecorder.PushScissors(mgc.parentEntry);
			}
			else
			{
				bool flag2 = renderData.clipMethod == ClipMethod.Stencil;
				if (flag2)
				{
					mgc.entryRecorder.BeginStencilMask(mgc.parentEntry);
					this.DrawVisualElementStencilMask(mgc);
					mgc.entryRecorder.EndStencilMask(mgc.parentEntry);
				}
			}
			mgc.entryRecorder.PushClippingRect(mgc.parentEntry);
		}

		private static void PopVisualElementClipping(MeshGenerationContext mgc)
		{
			RenderData renderData = mgc.renderData;
			mgc.entryRecorder.PopClippingRect(mgc.parentEntry);
			bool flag = renderData.clipMethod == ClipMethod.Scissor;
			if (flag)
			{
				mgc.entryRecorder.PopScissors(mgc.parentEntry);
			}
			else
			{
				bool flag2 = renderData.clipMethod == ClipMethod.Stencil;
				if (flag2)
				{
					mgc.entryRecorder.PopStencilMask(mgc.parentEntry);
				}
			}
		}

		private static void InvokeGenerateVisualContent(MeshGenerationContext mgc)
		{
			VisualElement visualElement = mgc.visualElement;
			Painter2D.isPainterActive = true;
			visualElement.InvokeGenerateVisualContent(mgc);
			Painter2D.isPainterActive = false;
		}
	}
}
