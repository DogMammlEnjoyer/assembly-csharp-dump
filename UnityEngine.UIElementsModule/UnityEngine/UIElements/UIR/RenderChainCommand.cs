using System;
using Unity.Profiling;

namespace UnityEngine.UIElements.UIR
{
	internal class RenderChainCommand : LinkedPoolItem<RenderChainCommand>
	{
		internal void Reset()
		{
			this.owner = null;
			this.prev = (this.next = null);
			this.isTail = false;
			this.type = CommandType.Draw;
			this.state = default(State);
			this.mesh = null;
			this.indexOffset = (this.indexCount = 0);
			this.callback = null;
		}

		internal void ExecuteNonDrawMesh(DrawParams drawParams, float pixelsPerPoint, ref Exception immediateException)
		{
			switch (this.type)
			{
			case CommandType.ImmediateCull:
			{
				bool flag = !RenderChainCommand.RectPointsToPixelsAndFlipYAxis(this.owner.owner.worldBound, pixelsPerPoint).Overlaps(Utility.GetActiveViewport());
				if (flag)
				{
					return;
				}
				break;
			}
			case CommandType.Immediate:
				break;
			case CommandType.PushView:
			{
				Matrix4x4 matrix4x;
				UIRUtility.ComputeMatrixRelativeToRenderTree(this.owner, out matrix4x);
				drawParams.view.Push(matrix4x);
				GL.modelview = matrix4x;
				RenderData parent = this.owner.parent;
				bool flag2 = parent != null;
				Rect scissor;
				if (flag2)
				{
					scissor = parent.clippingRect;
				}
				else
				{
					scissor = DrawParams.k_FullNormalizedRect;
				}
				RenderChainCommand.PushScissor(drawParams, scissor, pixelsPerPoint);
				return;
			}
			case CommandType.PopView:
				drawParams.view.Pop();
				GL.modelview = drawParams.view.Peek();
				RenderChainCommand.PopScissor(drawParams, pixelsPerPoint);
				return;
			case CommandType.PushScissor:
				RenderChainCommand.PushScissor(drawParams, this.owner.clippingRect, pixelsPerPoint);
				return;
			case CommandType.PopScissor:
				RenderChainCommand.PopScissor(drawParams, pixelsPerPoint);
				return;
			case CommandType.PushDefaultMaterial:
			case CommandType.PopDefaultMaterial:
				return;
			default:
				return;
			}
			bool flag3 = immediateException != null;
			if (!flag3)
			{
				bool flag4 = this.owner.compositeOpacity < 0.001f;
				if (!flag4)
				{
					Matrix4x4 unityProjectionMatrix = Utility.GetUnityProjectionMatrix();
					Camera current = Camera.current;
					RenderTexture active = RenderTexture.active;
					Matrix4x4 modelview;
					UIRUtility.ComputeMatrixRelativeToRenderTree(this.owner, out modelview);
					GL.modelview = modelview;
					RenderChainCommand.PushScissor(drawParams, this.owner.clippingRect, pixelsPerPoint);
					try
					{
						this.callback();
					}
					catch (Exception ex)
					{
						immediateException = ex;
					}
					RenderChainCommand.PopScissor(drawParams, pixelsPerPoint);
					Camera.SetupCurrent(current);
					RenderTexture.active = active;
					GL.modelview = drawParams.view.Peek();
					GL.LoadProjectionMatrix(unityProjectionMatrix);
				}
			}
		}

		internal static void PushScissor(DrawParams drawParams, Rect scissor, float pixelsPerPoint)
		{
			Rect rect = RenderChainCommand.CombineScissorRects(scissor, drawParams.scissor.Peek());
			drawParams.scissor.Push(rect);
			Utility.SetScissorRect(RenderChainCommand.RectPointsToPixelsAndFlipYAxis(rect, pixelsPerPoint));
		}

		internal static void PopScissor(DrawParams drawParams, float pixelsPerPoint)
		{
			drawParams.scissor.Pop();
			Rect rect = drawParams.scissor.Peek();
			bool flag = rect.x == DrawParams.k_UnlimitedRect.x;
			if (flag)
			{
				Utility.DisableScissor();
			}
			else
			{
				Utility.SetScissorRect(RenderChainCommand.RectPointsToPixelsAndFlipYAxis(rect, pixelsPerPoint));
			}
		}

		private void Blit(Texture source, RenderTexture destination, float depth)
		{
			GL.PushMatrix();
			GL.LoadOrtho();
			RenderTexture.active = destination;
			this.state.material.SetTexture(RenderChainCommand.k_ID_MainTex, source);
			this.state.material.SetPass(0);
			GL.Begin(7);
			GL.TexCoord2(0f, 0f);
			GL.Vertex3(0f, 0f, depth);
			GL.TexCoord2(0f, 1f);
			GL.Vertex3(0f, 1f, depth);
			GL.TexCoord2(1f, 1f);
			GL.Vertex3(1f, 1f, depth);
			GL.TexCoord2(1f, 0f);
			GL.Vertex3(1f, 0f, depth);
			GL.End();
			GL.PopMatrix();
		}

		private static Vector4 RectToClipSpace(Rect rc)
		{
			Matrix4x4 deviceProjectionMatrix = Utility.GetDeviceProjectionMatrix();
			Vector3 vector = deviceProjectionMatrix.MultiplyPoint(new Vector3(rc.xMin, rc.yMin, 0f));
			Vector3 vector2 = deviceProjectionMatrix.MultiplyPoint(new Vector3(rc.xMax, rc.yMax, 0f));
			return new Vector4(Mathf.Min(vector.x, vector2.x), Mathf.Min(vector.y, vector2.y), Mathf.Max(vector.x, vector2.x), Mathf.Max(vector.y, vector2.y));
		}

		private static Rect CombineScissorRects(Rect r0, Rect r1)
		{
			Rect result = new Rect(0f, 0f, 0f, 0f);
			result.x = Math.Max(r0.x, r1.x);
			result.y = Math.Max(r0.y, r1.y);
			result.xMax = Math.Max(result.x, Math.Min(r0.xMax, r1.xMax));
			result.yMax = Math.Max(result.y, Math.Min(r0.yMax, r1.yMax));
			return result;
		}

		private static RectInt RectPointsToPixelsAndFlipYAxis(Rect rect, float pixelsPerPoint)
		{
			float num = (float)Utility.GetActiveViewport().height;
			return new RectInt(0, 0, 0, 0)
			{
				x = Mathf.RoundToInt(rect.x * pixelsPerPoint),
				y = Mathf.RoundToInt(num - rect.yMax * pixelsPerPoint),
				width = Mathf.RoundToInt(rect.width * pixelsPerPoint),
				height = Mathf.RoundToInt(rect.height * pixelsPerPoint)
			};
		}

		internal RenderData owner;

		internal RenderChainCommand prev;

		internal RenderChainCommand next;

		internal bool isTail;

		internal CommandType type;

		internal State state;

		internal MeshHandle mesh;

		internal int indexOffset;

		internal int indexCount;

		internal Action callback;

		private static readonly int k_ID_MainTex = Shader.PropertyToID("_MainTex");

		private static ProfilerMarker s_ImmediateOverheadMarker = new ProfilerMarker("UIR.ImmediateOverhead");
	}
}
