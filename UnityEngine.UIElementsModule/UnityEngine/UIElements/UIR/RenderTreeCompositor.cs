using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UIElements.Layout;

namespace UnityEngine.UIElements.UIR
{
	internal class RenderTreeCompositor : IDisposable
	{
		public RenderTreeCompositor(RenderTreeManager owner)
		{
			this.m_RenderTreeManager = owner;
		}

		public void Update(RenderTree rootRenderTree)
		{
			this.CleanupOperationTree();
			bool flag = rootRenderTree == null;
			if (!flag)
			{
				this.BuildDrawOperationTree(rootRenderTree);
				RenderTreeCompositor.UpdateDrawBounds_PostOrder(this.m_RootOperation);
				this.AssignTextureIds_DepthFirst(this.m_RootOperation);
			}
		}

		private void BuildDrawOperationTree(RenderTree rootRenderTree)
		{
			this.m_RootOperation = this.m_DrawOperationPool.Get();
			this.m_RootOperation.Init(rootRenderTree);
		}

		private static PostProcessingMargins GetReadMargins(PostProcessingPass effect, FilterFunction func)
		{
			bool flag = effect.computeRequiredReadMarginsCallback != null;
			PostProcessingMargins result;
			if (flag)
			{
				result = effect.computeRequiredReadMarginsCallback(func);
			}
			else
			{
				result = effect.readMargins;
			}
			return result;
		}

		private static PostProcessingMargins GetWriteMargins(PostProcessingPass effect, FilterFunction func)
		{
			bool flag = effect.computeRequiredWriteMarginsCallback != null;
			PostProcessingMargins result;
			if (flag)
			{
				result = effect.computeRequiredWriteMarginsCallback(func);
			}
			else
			{
				result = effect.writeMargins;
			}
			return result;
		}

		private static void UpdateDrawBounds_PostOrder(RenderTreeCompositor.DrawOperation op)
		{
			Rect? rect = null;
			RenderTreeCompositor.DrawOperationType type = op.type;
			RenderTreeCompositor.DrawOperationType drawOperationType = type;
			if (drawOperationType != RenderTreeCompositor.DrawOperationType.RenderTree)
			{
				if (drawOperationType != RenderTreeCompositor.DrawOperationType.Effect)
				{
					throw new NotImplementedException();
				}
				RenderTreeCompositor.DrawOperation firstChild = op.firstChild;
				bool flag = firstChild != null;
				if (flag)
				{
					Debug.Assert(firstChild.nextSibling == null);
					RenderTreeCompositor.UpdateDrawBounds_PostOrder(firstChild);
					bool flag2 = UIRUtility.RectHasArea(op.drawSourceBounds);
					if (flag2)
					{
						rect = new Rect?(UIRUtility.CastToRect(op.drawSourceBounds));
					}
				}
			}
			else
			{
				for (RenderTreeCompositor.DrawOperation drawOperation = op.firstChild; drawOperation != null; drawOperation = drawOperation.nextSibling)
				{
					RenderTreeCompositor.UpdateDrawBounds_PostOrder(drawOperation);
					bool flag3 = UIRUtility.RectHasArea(drawOperation.bounds);
					if (flag3)
					{
						Matrix4x4 matrix4x;
						UIRUtility.ComputeMatrixRelativeToRenderTree(drawOperation.visualElement.renderData, out matrix4x);
						Rect rect2 = VisualElement.CalculateConservativeRect(ref matrix4x, UIRUtility.CastToRect(drawOperation.bounds));
						rect = new Rect?((rect == null) ? rect2 : UIRUtility.Encapsulate(rect.Value, rect2));
					}
				}
				Rect boundingBox = op.renderTree.rootRenderData.owner.boundingBox;
				bool flag4 = UIRUtility.RectHasArea(boundingBox);
				if (flag4)
				{
					rect = new Rect?((rect == null) ? boundingBox : UIRUtility.Encapsulate(rect.Value, boundingBox));
				}
				else
				{
					Debug.Assert(rect == null);
				}
			}
			bool flag5 = rect != null;
			if (flag5)
			{
				Rect value = rect.Value;
				PostProcessingMargins margins = default(PostProcessingMargins);
				PostProcessingMargins margins2 = default(PostProcessingMargins);
				RenderTreeCompositor.DrawOperation parent = op.parent;
				bool flag6 = parent != null && parent.type == RenderTreeCompositor.DrawOperationType.Effect;
				RectInt bounds;
				if (flag6)
				{
					margins = RenderTreeCompositor.GetReadMargins(parent.effect, parent.filter);
					margins2 = RenderTreeCompositor.GetWriteMargins(parent.effect, parent.filter);
					Rect rect3 = UIRUtility.InflateByMargins(UIRUtility.InflateByMargins(value, margins), margins2);
					bounds = UIRUtility.CastToRectInt(rect3);
				}
				else
				{
					bounds = UIRUtility.CastToRectInt(value);
				}
				RenderTreeCompositor.DrawOperation parent2 = op.parent;
				bool flag7 = parent2 != null && parent2.type == RenderTreeCompositor.DrawOperationType.Effect;
				if (flag7)
				{
					Rect rect4 = value;
					rect4 = UIRUtility.InflateByMargins(rect4, margins2);
					op.parent.drawSourceBounds = UIRUtility.CastToRectInt(rect4);
					Vector4 zero = new Vector4(margins.left, margins.top, margins.right, margins.bottom);
					bool flag8 = bounds.width > 0 && bounds.height > 0;
					if (flag8)
					{
						float num = 1f / (float)bounds.width;
						float num2 = 1f / (float)bounds.height;
						zero.x *= num;
						zero.y *= num2;
						zero.z *= num;
						zero.w *= num2;
					}
					else
					{
						zero = Vector4.zero;
					}
					op.parent.drawSourceTexOffsets = zero;
				}
				op.bounds = bounds;
			}
			else
			{
				op.bounds = RectInt.zero;
			}
			RenderTreeCompositor.DrawOperation parent3 = op.parent;
			bool flag9 = parent3 != null && parent3.type == RenderTreeCompositor.DrawOperationType.RenderTree;
			if (flag9)
			{
				op.renderTree.quadRect = op.bounds;
			}
		}

		private void AssignTextureIds_DepthFirst(RenderTreeCompositor.DrawOperation op)
		{
			RenderTreeCompositor.DrawOperation parent = op.parent;
			bool flag = parent != null && parent.type == RenderTreeCompositor.DrawOperationType.RenderTree;
			if (flag)
			{
				Debug.Assert(!op.renderTree.quadTextureId.IsValid());
				TextureId textureId = this.m_RenderTreeManager.textureRegistry.AllocAndAcquireDynamic();
				op.dstTextureId = textureId;
				op.renderTree.quadTextureId = textureId;
				op.parent.renderTree.OnRenderDataVisualsChanged(op.visualElement.renderData, false);
			}
			else
			{
				Debug.Assert(!op.dstTextureId.IsValid());
			}
			for (RenderTreeCompositor.DrawOperation drawOperation = op.firstChild; drawOperation != null; drawOperation = drawOperation.nextSibling)
			{
				this.AssignTextureIds_DepthFirst(drawOperation);
			}
		}

		public void RenderNestedPasses()
		{
			this.ExecuteDrawOperation_PostOrder(this.m_RootOperation);
		}

		private void ExecuteDrawOperation_PostOrder(RenderTreeCompositor.DrawOperation op)
		{
			for (RenderTreeCompositor.DrawOperation drawOperation = op.firstChild; drawOperation != null; drawOperation = drawOperation.nextSibling)
			{
				this.ExecuteDrawOperation_PostOrder(drawOperation);
			}
			bool flag = op.parent == null;
			if (!flag)
			{
				RectInt bounds = op.bounds;
				bool flag2 = bounds.width <= 0;
				if (!flag2)
				{
					Debug.Assert(bounds.height > 0);
					GraphicsFormat colorFormat = (QualitySettings.activeColorSpace == ColorSpace.Linear) ? GraphicsFormat.R8G8B8A8_SRGB : GraphicsFormat.R8G8B8A8_UNorm;
					RenderTextureDescriptor desc = new RenderTextureDescriptor(bounds.width, bounds.height, colorFormat, GraphicsFormat.D24_UNorm_S8_UInt);
					op.dstTexture = RenderTexture.GetTemporary(desc);
					this.m_AllocatedRenderTextures.Add(op.dstTexture);
					bool flag3 = op.dstTextureId.IsValid();
					if (flag3)
					{
						this.m_RenderTreeManager.textureRegistry.UpdateDynamic(op.dstTextureId, op.dstTexture);
					}
					RenderTreeCompositor.DrawOperationType type = op.type;
					RenderTreeCompositor.DrawOperationType drawOperationType = type;
					if (drawOperationType != RenderTreeCompositor.DrawOperationType.RenderTree)
					{
						if (drawOperationType != RenderTreeCompositor.DrawOperationType.Effect)
						{
							throw new NotImplementedException();
						}
						try
						{
							RenderTexture active = RenderTexture.active;
							RenderTexture.active = op.dstTexture;
							GL.Clear(false, true, Color.clear);
							op.effect.material.SetPass(op.effect.passIndex);
							this.m_Block.SetTexture("_MainTex", op.firstChild.dstTexture);
							bool flag4 = op.effect.prepareMaterialPropertyBlockCallback != null;
							if (flag4)
							{
								op.effect.prepareMaterialPropertyBlockCallback(this.m_Block, op.filter);
							}
							else
							{
								this.ApplyEffectParameters(op.effect, op.filter, op.visualElement);
							}
							Utility.SetPropertyBlock(this.m_Block);
							Matrix4x4 mat = ProjectionUtils.Ortho((float)bounds.xMin, (float)bounds.xMax, (float)bounds.yMax, (float)bounds.yMin, 0f, 1f);
							GL.LoadProjectionMatrix(mat);
							GL.modelview = Matrix4x4.identity;
							RectInt drawSourceBounds = op.drawSourceBounds;
							Vector4 drawSourceTexOffsets = op.drawSourceTexOffsets;
							GL.Viewport(new Rect(0f, 0f, (float)bounds.width, (float)bounds.height));
							GL.Begin(7);
							GL.TexCoord2(drawSourceTexOffsets.x, drawSourceTexOffsets.w);
							GL.Vertex3((float)drawSourceBounds.xMin, (float)drawSourceBounds.yMax, 0.5f);
							GL.TexCoord2(drawSourceTexOffsets.x, 1f - drawSourceTexOffsets.y);
							GL.Vertex3((float)drawSourceBounds.xMin, (float)drawSourceBounds.yMin, 0.5f);
							GL.TexCoord2(1f - drawSourceTexOffsets.z, 1f - drawSourceTexOffsets.y);
							GL.Vertex3((float)drawSourceBounds.xMax, (float)drawSourceBounds.yMin, 0.5f);
							GL.TexCoord2(1f - drawSourceTexOffsets.z, drawSourceTexOffsets.w);
							GL.Vertex3((float)drawSourceBounds.xMax, (float)drawSourceBounds.yMax, 0.5f);
							GL.End();
							RenderTexture.active = active;
						}
						catch
						{
						}
					}
					else
					{
						this.m_RenderTreeManager.RenderSingleTree(op.renderTree, op.dstTexture, bounds);
					}
				}
			}
		}

		private unsafe void ApplyEffectParameters(PostProcessingPass effect, FilterFunction filter, VisualElement source)
		{
			bool flag = effect.parameterBindings == null;
			if (!flag)
			{
				FixedBuffer4<FilterParameter> parameters = filter.parameters;
				int parameterCount = filter.parameterCount;
				for (int i = 0; i < effect.parameterBindings.Length; i++)
				{
					bool flag2 = i >= parameterCount;
					if (flag2)
					{
						break;
					}
					ParameterBinding parameterBinding = effect.parameterBindings[i];
					FilterParameter filterParameter = *parameters[i];
					bool flag3 = filterParameter.type == FilterParameterType.Float;
					if (flag3)
					{
						this.m_Block.SetFloat(parameterBinding.name, filterParameter.floatValue);
					}
					else
					{
						bool flag4 = filterParameter.type == FilterParameterType.Color;
						if (flag4)
						{
							this.m_Block.SetColor(parameterBinding.name, filterParameter.colorValue);
						}
					}
				}
			}
		}

		private void CleanupOperationTree()
		{
			bool flag = this.m_RootOperation != null;
			if (flag)
			{
				this.CleanupOperation_PostOrder(this.m_RootOperation);
				this.m_RootOperation = null;
			}
			for (int i = 0; i < this.m_AllocatedRenderTextures.Count; i++)
			{
				RenderTexture.ReleaseTemporary(this.m_AllocatedRenderTextures[i]);
			}
			this.m_AllocatedRenderTextures.Clear();
		}

		private void CleanupOperation_PostOrder(RenderTreeCompositor.DrawOperation op)
		{
			for (RenderTreeCompositor.DrawOperation drawOperation = op.firstChild; drawOperation != null; drawOperation = drawOperation.nextSibling)
			{
				this.CleanupOperation_PostOrder(drawOperation);
			}
			bool flag = op.dstTextureId.IsValid();
			if (flag)
			{
				this.m_RenderTreeManager.textureRegistry.Release(op.dstTextureId);
				op.dstTextureId = TextureId.invalid;
				op.renderTree.quadTextureId = TextureId.invalid;
			}
			op.Reset();
			this.m_DrawOperationPool.Release(op);
		}

		private protected bool disposed { protected get; private set; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			bool disposed = this.disposed;
			if (!disposed)
			{
				if (disposing)
				{
					this.CleanupOperationTree();
				}
				this.disposed = true;
			}
		}

		private readonly RenderTreeManager m_RenderTreeManager;

		private RenderTreeCompositor.DrawOperation m_RootOperation;

		private List<RenderTexture> m_AllocatedRenderTextures = new List<RenderTexture>();

		private MaterialPropertyBlock m_Block = new MaterialPropertyBlock();

		private ObjectPool<RenderTreeCompositor.DrawOperation> m_DrawOperationPool = new ObjectPool<RenderTreeCompositor.DrawOperation>(() => new RenderTreeCompositor.DrawOperation(), 100);

		private enum DrawOperationType
		{
			Undefined,
			RenderTree,
			Effect
		}

		private class DrawOperation
		{
			public RenderTreeCompositor.DrawOperationType type
			{
				get
				{
					return this.m_Type;
				}
			}

			public VisualElement visualElement
			{
				get
				{
					return this.m_VisualElement;
				}
			}

			public RenderTree renderTree
			{
				get
				{
					return this.m_RenderTree;
				}
			}

			public PostProcessingPass effect
			{
				get
				{
					return this.m_Effect;
				}
			}

			public FilterFunction filter
			{
				get
				{
					return this.m_Filter;
				}
			}

			public void Init(VisualElement ve, in PostProcessingPass effect, FilterFunction filter)
			{
				this.m_Type = RenderTreeCompositor.DrawOperationType.Effect;
				this.m_VisualElement = ve;
				this.m_Effect = effect;
				this.m_Filter = filter;
				this.m_RenderTree = ve.nestedRenderData.renderTree;
				this.InitPointers();
			}

			public void Init(RenderTree renderTree)
			{
				this.m_Type = RenderTreeCompositor.DrawOperationType.RenderTree;
				this.m_VisualElement = renderTree.rootRenderData.owner;
				this.m_RenderTree = renderTree;
				this.InitPointers();
			}

			private void InitPointers()
			{
				this.parent = null;
				this.firstChild = null;
				this.lastChild = null;
				this.prevSibling = null;
				this.nextSibling = null;
			}

			public void Reset()
			{
				this.m_Type = RenderTreeCompositor.DrawOperationType.Undefined;
				this.m_VisualElement = null;
				this.m_RenderTree = null;
				this.m_Effect = default(PostProcessingPass);
				this.m_Filter = default(FilterFunction);
				this.dstTexture = null;
				this.dstTextureId = TextureId.invalid;
			}

			public void AddChild(RenderTreeCompositor.DrawOperation op)
			{
				Debug.Assert(op.prevSibling == null);
				op.parent = this;
				op.nextSibling = this.firstChild;
				bool flag = this.firstChild != null;
				if (flag)
				{
					this.firstChild.prevSibling = op;
				}
				this.firstChild = op;
			}

			private RenderTreeCompositor.DrawOperationType m_Type;

			private VisualElement m_VisualElement;

			private RenderTree m_RenderTree;

			private PostProcessingPass m_Effect;

			private FilterFunction m_Filter;

			public RectInt bounds;

			public RectInt drawSourceBounds;

			public Vector4 drawSourceTexOffsets;

			public RenderTexture dstTexture;

			public TextureId dstTextureId;

			public RenderTreeCompositor.DrawOperation parent;

			public RenderTreeCompositor.DrawOperation firstChild;

			public RenderTreeCompositor.DrawOperation lastChild;

			public RenderTreeCompositor.DrawOperation prevSibling;

			public RenderTreeCompositor.DrawOperation nextSibling;
		}
	}
}
