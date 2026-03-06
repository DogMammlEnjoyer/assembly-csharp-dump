using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Meta.XR.EnvironmentDepth;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace Meta.XR
{
	[AddComponentMenu("")]
	[DefaultExecutionOrder(-48)]
	internal class EnvironmentDepthRaycaster : MonoBehaviour
	{
		private void Awake()
		{
			this._shader = Resources.Load<ComputeShader>("CopyDepthTexture");
			this._computeBuffer = new ComputeBuffer(32768, 4);
			this._depthTexturePixels = new NativeArray<float>(32768, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this._gpuRequestBuffer = new NativeArray<float>(32768, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			List<XRDisplaySubsystem> list = new List<XRDisplaySubsystem>(1);
			SubsystemManager.GetSubsystems<XRDisplaySubsystem>(list);
			this._xrDisplay = list.Single<XRDisplaySubsystem>();
		}

		private void OnDisable()
		{
			this.InvalidateDepthTexture();
		}

		internal void OnDepthTextureUpdate(RenderTexture updatedDepthTexture)
		{
			this._updatedDepthTexture = updatedDepthTexture;
			this.CreateTextureCopyRequestIfNeeded();
		}

		private void InvalidateDepthTexture()
		{
			this._isDepthTextureAvailable = false;
		}

		private void OnDestroy()
		{
			this.depthManager.onDepthTextureUpdate -= this.OnDepthTextureUpdate;
			if (this._currentGpuReadbackRequest != null && !this._currentGpuReadbackRequest.Value.done)
			{
				this._currentGpuReadbackRequest.Value.WaitForCompletion();
			}
			this._computeBuffer.Dispose();
			this._depthTexturePixels.Dispose();
			this._gpuRequestBuffer.Dispose();
		}

		private void CreateTextureCopyRequestIfNeeded()
		{
			if (this._currentGpuReadbackRequest != null)
			{
				return;
			}
			if (!this.depthManager.enabled || !this.depthManager.IsDepthAvailable)
			{
				this.InvalidateDepthTexture();
				return;
			}
			if (!this._warmUpRaycast)
			{
				this.InvalidateDepthTexture();
				return;
			}
			RenderTexture updatedDepthTexture = this._updatedDepthTexture;
			if (updatedDepthTexture == null)
			{
				return;
			}
			this._updatedDepthTexture = null;
			for (int i = 0; i < 2; i++)
			{
				this._depthFrameDesc[i] = this.depthManager.frameDescriptors[i];
			}
			this._worldToTrackingSpaceMatrix = this.depthManager.GetTrackingSpaceWorldToLocalMatrix();
			this._shader.SetTexture(0, EnvironmentDepthRaycaster.EnvironmentDepthTextureId, updatedDepthTexture);
			this._shader.SetFloat(EnvironmentDepthRaycaster.EnvironmentDepthTextureSizeId, (float)updatedDepthTexture.width);
			this._EnvironmentDepthZBufferParams = Shader.GetGlobalVector(EnvironmentDepthRaycaster.EnvironmentDepthZBufferParamsId);
			this._shader.SetVector(EnvironmentDepthRaycaster.EnvironmentDepthZBufferParamsId, this._EnvironmentDepthZBufferParams);
			this._shader.SetBuffer(0, EnvironmentDepthRaycaster.CopiedDepthTextureId, this._computeBuffer);
			this._shader.Dispatch(0, 1, 1, 1);
			this._currentGpuReadbackRequest = new AsyncGPUReadbackRequest?(AsyncGPUReadback.RequestIntoNativeArray<float>(ref this._gpuRequestBuffer, this._computeBuffer, null));
		}

		private void UpdateTextureCopyRequest()
		{
			if (this._currentGpuReadbackRequest == null || !this._currentGpuReadbackRequest.Value.done)
			{
				return;
			}
			if (this._currentGpuReadbackRequest.Value.hasError)
			{
				Debug.LogError("AsyncGPUReadback.RequestIntoNativeArray() hasError");
			}
			else
			{
				NativeArray<float> gpuRequestBuffer = this._gpuRequestBuffer;
				NativeArray<float> depthTexturePixels = this._depthTexturePixels;
				this._depthTexturePixels = gpuRequestBuffer;
				this._gpuRequestBuffer = depthTexturePixels;
				for (int i = 0; i < 2; i++)
				{
					Matrix4x4 lhs;
					Matrix4x4 matrix4x;
					EnvironmentDepthUtils.CalculateDepthCameraMatrices(this._depthFrameDesc[i], out lhs, out matrix4x);
					matrix4x *= this._worldToTrackingSpaceMatrix;
					this._matrixV[i] = matrix4x;
					this._matrixVP[i] = lhs * matrix4x;
					GeometryUtility.CalculateFrustumPlanes(this._matrixVP[i], this._camFrustumPlanes[i]);
					this._matrixVP_inv[i] = this._matrixVP[i].inverse;
				}
				this._isDepthTextureAvailable = true;
			}
			this._currentGpuReadbackRequest = null;
		}

		private void Update()
		{
			if (this.depthManager == null)
			{
				Object.Destroy(this);
				return;
			}
			this.UpdateTextureCopyRequest();
			this.CreateTextureCopyRequestIfNeeded();
		}

		private Vector2Int WorldPosToNonNormalizedTextureCoords(Vector3 worldPos)
		{
			Vector4 vector = this._matrixVP[this._currentEyeIndex] * new Vector4(worldPos.x, worldPos.y, worldPos.z, 1f);
			Vector2 vector2 = (new Vector2(vector.x, vector.y) / vector.w + Vector2.one) * 0.5f;
			return new Vector2Int(Mathf.Clamp((int)(vector2.x * 128f), 0, 127), Mathf.Clamp((int)(vector2.y * 128f), 0, 127));
		}

		private float SampleDepthTexture(Vector2Int texCoord)
		{
			return this._depthTexturePixels[texCoord.x + texCoord.y * 128 + 16384 * this._currentEyeIndex];
		}

		private Vector3 WorldPosAtDepthTexCoord(Vector2Int texCoord)
		{
			float num = this.SampleDepthTexture(texCoord);
			float z = (num == 0f) ? 0f : (this._EnvironmentDepthZBufferParams.x / num - this._EnvironmentDepthZBufferParams.y);
			Vector4 vector = new Vector4((float)texCoord.x * 0.0078125f * 2f - 1f, (float)texCoord.y * 0.0078125f * 2f - 1f, z, 1f);
			Vector4 vector2 = this._matrixVP_inv[this._currentEyeIndex] * vector;
			return vector2 / vector2.w;
		}

		private float WorldPosToLinearDepth(Vector3 worldPos)
		{
			return -(this._matrixV[this._currentEyeIndex] * new Vector4(worldPos.x, worldPos.y, worldPos.z, 1f)).z;
		}

		private Vector3 ReconstructNormal(Vector2Int texCoord)
		{
			EnvironmentDepthRaycaster.<>c__DisplayClass36_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.texCoord = texCoord;
			CS$<>8__locals1.centerDepth = this.SampleDepthTexture(CS$<>8__locals1.texCoord);
			CS$<>8__locals1.centerWorldPos = this.WorldPosAtDepthTexCoord(CS$<>8__locals1.texCoord);
			Vector3 lhs = this.<ReconstructNormal>g__ClosestDerivativeToAdjacentExtrapolations|36_0(new Vector2Int(1, 0), ref CS$<>8__locals1);
			Vector3 rhs = this.<ReconstructNormal>g__ClosestDerivativeToAdjacentExtrapolations|36_0(new Vector2Int(0, 1), ref CS$<>8__locals1);
			return -Vector3.Normalize(Vector3.Cross(lhs, rhs));
		}

		internal DepthRaycastResult Raycast(Ray ray, out Vector3 position, out Vector3 normal, out float normalConfidence, float maxDistance, Eye eye, bool allowOccludedRayOrigin)
		{
			normal = default(Vector3);
			normalConfidence = 0f;
			ValueTuple<DepthRaycastResult, Vector3, int> valueTuple = this.Raycast(ray, maxDistance, eye, allowOccludedRayOrigin);
			position = valueTuple.Item2;
			if (valueTuple.Item1 != DepthRaycastResult.Success)
			{
				return valueTuple.Item1;
			}
			this._currentEyeIndex = valueTuple.Item3;
			if (this.ReconstructNormalAtWorldPos(position, out normal, out normalConfidence))
			{
				return DepthRaycastResult.Success;
			}
			this._currentEyeIndex = ((this._currentEyeIndex == 0) ? 1 : 0);
			if (!this.ReconstructNormalAtWorldPos(position, out normal, out normalConfidence))
			{
				return DepthRaycastResult.RayOutsideOfDepthCameraFrustum;
			}
			return DepthRaycastResult.Success;
		}

		private unsafe bool ReconstructNormalAtWorldPos(Vector3 position, out Vector3 normal, out float normalConfidence)
		{
			normal = default(Vector3);
			normalConfidence = 0f;
			Vector2Int a = this.WorldPosToNonNormalizedTextureCoords(position);
			if (a.x < 4 || a.x >= 124 || a.y < 4 || a.y >= 124)
			{
				return false;
			}
			IntPtr intPtr = stackalloc byte[checked(unchecked((UIntPtr)5) * (UIntPtr)sizeof(Vector2Int))];
			*intPtr = new Vector2Int(-2, 0);
			*(intPtr + (IntPtr)sizeof(Vector2Int)) = new Vector2Int(2, 0);
			*(intPtr + (IntPtr)2 * (IntPtr)sizeof(Vector2Int)) = new Vector2Int(0, 0);
			*(intPtr + (IntPtr)3 * (IntPtr)sizeof(Vector2Int)) = new Vector2Int(0, -2);
			*(intPtr + (IntPtr)4 * (IntPtr)sizeof(Vector2Int)) = new Vector2Int(0, 2);
			Span<Vector2Int> span = new Span<Vector2Int>(intPtr, 5);
			int length = span.Length;
			int num = length;
			Span<Vector3> span2 = new Span<Vector3>(stackalloc byte[checked(unchecked((UIntPtr)num) * (UIntPtr)sizeof(Vector3))], num);
			Vector3 vector = Vector3.zero;
			for (int i = 0; i < length; i++)
			{
				Vector3 vector2 = this.ReconstructNormal(a + *span[i]);
				*span2[i] = vector2;
				vector += vector2;
			}
			vector = Vector3.Normalize(vector);
			normal = vector;
			float num2 = 0f;
			for (int j = 0; j < length; j++)
			{
				if (Vector3.Dot(*span2[j], vector) > 0.95f)
				{
					num2 += 1f;
				}
			}
			normalConfidence = num2 / (float)length;
			return true;
		}

		[return: TupleElementNames(new string[]
		{
			"status",
			"position",
			"eyeIndex"
		})]
		internal ValueTuple<DepthRaycastResult, Vector3, int> Raycast(Ray ray, float maxDistance, Eye eye, bool allowOccludedRayOrigin)
		{
			EnvironmentDepthRaycaster.<>c__DisplayClass39_0 CS$<>8__locals1;
			CS$<>8__locals1.ray = ray;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.maxDistance = maxDistance;
			CS$<>8__locals1.allowOccludedRayOrigin = allowOccludedRayOrigin;
			if (!this._isDepthTextureAvailable)
			{
				return new ValueTuple<DepthRaycastResult, Vector3, int>(DepthRaycastResult.NotReady, default(Vector3), 0);
			}
			if (eye != Eye.Both)
			{
				return this.<Raycast>g__GetRaycastResultForEye|39_0((eye == Eye.Left) ? 0 : 1, ref CS$<>8__locals1);
			}
			ValueTuple<DepthRaycastResult, Vector3, int> valueTuple = this.<Raycast>g__GetRaycastResultForEye|39_0(0, ref CS$<>8__locals1);
			if (valueTuple.Item1 == DepthRaycastResult.Success)
			{
				return valueTuple;
			}
			ValueTuple<DepthRaycastResult, Vector3, int> valueTuple2 = this.<Raycast>g__GetRaycastResultForEye|39_0(1, ref CS$<>8__locals1);
			if (valueTuple2.Item1 == DepthRaycastResult.Success)
			{
				return valueTuple2;
			}
			if (valueTuple.Item1 != DepthRaycastResult.HitPointOccluded || valueTuple2.Item1 != DepthRaycastResult.HitPointOccluded)
			{
				return valueTuple;
			}
			if (Vector3.Distance(CS$<>8__locals1.ray.origin, valueTuple.Item2) <= Vector3.Distance(CS$<>8__locals1.ray.origin, valueTuple2.Item2))
			{
				return valueTuple2;
			}
			return valueTuple;
		}

		private static Vector3 ClosestPointOnFirstRay(Vector3 ray1Pos, Vector3 ray1Dir, Vector3 ray2Pos, Vector3 ray2Dir)
		{
			Vector3 lhs = ray2Pos - ray1Pos;
			Vector3 vector = Vector3.Cross(ray1Dir, ray2Dir);
			float d = Vector3.Dot(Vector3.Cross(lhs, ray2Dir), vector) / Vector3.Dot(vector, vector);
			return ray1Pos + ray1Dir * d;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsInBounds(Vector2Int texCoord)
		{
			return texCoord.x >= 0 && texCoord.x < 128 && texCoord.y >= 0 && texCoord.x < 128;
		}

		private DepthRaycastResult RaycastInternal(Ray ray, out Vector3 position, float maxDistance, int eyeIndex, bool allowOccludedRayOrigin)
		{
			position = default(Vector3);
			if (!Mathf.Approximately(ray.direction.sqrMagnitude, 1f))
			{
				Debug.LogError("ray.direction should be normalized.");
				return DepthRaycastResult.NoHit;
			}
			if (maxDistance < 0.01f)
			{
				return DepthRaycastResult.NoHit;
			}
			this._currentEyeIndex = eyeIndex;
			Vector3 origin = ray.origin;
			Vector3 direction = ray.direction;
			Vector3 worldPos = origin + maxDistance * direction;
			Vector2Int texCoord = this.WorldPosToNonNormalizedTextureCoords(origin);
			if (!allowOccludedRayOrigin)
			{
				float num = this.WorldPosToLinearDepth(origin);
				float num2 = this.SampleDepthTexture(texCoord);
				if (num > num2)
				{
					return DepthRaycastResult.RayOccluded;
				}
			}
			Vector2Int vector2Int = this.WorldPosToNonNormalizedTextureCoords(worldPos);
			int num3 = vector2Int.x - texCoord.x;
			int num4 = vector2Int.y - texCoord.y;
			int num5 = Mathf.Max(Mathf.Abs(num3), Mathf.Abs(num4));
			if (num5 == 0)
			{
				float num6 = this.SampleDepthTexture(texCoord);
				if (num6 < maxDistance && this.WorldPosToLinearDepth(origin) < num6 && this.WorldPosToLinearDepth(worldPos) > num6)
				{
					position = ray.origin + direction * num6;
					return DepthRaycastResult.Success;
				}
				return DepthRaycastResult.NoHit;
			}
			else
			{
				float num7 = 1f / this.WorldPosToLinearDepth(origin);
				float num8 = 1f / this.WorldPosToLinearDepth(worldPos);
				float num9 = (float)num3 / (float)num5;
				float num10 = (float)num4 / (float)num5;
				float num11 = (num8 - num7) / (float)num5;
				float num12 = (float)texCoord.x;
				float num13 = (float)texCoord.y;
				float num14 = num7;
				bool flag = false;
				for (int i = 0; i <= num5; i++)
				{
					Vector2Int texCoord2 = new Vector2Int((int)num12, (int)num13);
					if (!EnvironmentDepthRaycaster.IsInBounds(texCoord2))
					{
						return DepthRaycastResult.RayOutsideOfDepthCameraFrustum;
					}
					float num15 = this.SampleDepthTexture(texCoord2);
					if (num15 != 0f)
					{
						float num16 = 1f / num14;
						if (!flag)
						{
							flag = (num15 > num16);
						}
						else if (num15 <= num16)
						{
							Vector2Int texCoord3 = new Vector2Int((int)(num12 - num9), (int)(num13 - num10));
							float num17 = this.SampleDepthTexture(texCoord3);
							Vector3 vector = this.WorldPosAtDepthTexCoord(texCoord3);
							Vector3 a = this.WorldPosAtDepthTexCoord(texCoord2);
							position = EnvironmentDepthRaycaster.ClosestPointOnFirstRay(origin, direction, vector, a - vector);
							if (num17 - num15 <= 0.3f)
							{
								return DepthRaycastResult.Success;
							}
							return DepthRaycastResult.HitPointOccluded;
						}
					}
					num12 += num9;
					num13 += num10;
					num14 += num11;
				}
				if (!flag)
				{
					return DepthRaycastResult.RayOccluded;
				}
				return DepthRaycastResult.NoHit;
			}
		}

		private static bool ClampRayOriginToCamFrustumPlanes(ref Ray ray, Plane[] planes, ref float maxDistance)
		{
			if (GeometryUtility.TestPlanesAABB(planes, new Bounds(ray.origin, Vector3.zero)))
			{
				return true;
			}
			int i = 0;
			while (i < 5)
			{
				Plane plane = planes[i];
				float num;
				if (plane.Raycast(ray, out num) && GeometryUtility.TestPlanesAABB(planes, new Bounds(ray.GetPoint(num + 0.01f), Vector3.zero)))
				{
					maxDistance -= num;
					if (maxDistance <= 0f)
					{
						return false;
					}
					ray.origin = ray.GetPoint(num);
					return true;
				}
				else
				{
					i++;
				}
			}
			return false;
		}

		[CompilerGenerated]
		private Vector3 <ReconstructNormal>g__ClosestDerivativeToAdjacentExtrapolations|36_0(Vector2Int axis, ref EnvironmentDepthRaycaster.<>c__DisplayClass36_0 A_2)
		{
			Vector4 vector = new Vector4(this.SampleDepthTexture(A_2.texCoord - axis), this.SampleDepthTexture(A_2.texCoord + axis), this.SampleDepthTexture(A_2.texCoord - axis * 2), this.SampleDepthTexture(A_2.texCoord + axis * 2));
			Vector2 vector2 = new Vector2(Mathf.Abs(vector.x * vector.z / (2f * vector.z - vector.x) - A_2.centerDepth), Mathf.Abs(vector.y * vector.w / (2f * vector.w - vector.y) - A_2.centerDepth));
			if (vector2.x <= vector2.y)
			{
				return A_2.centerWorldPos - this.WorldPosAtDepthTexCoord(A_2.texCoord - axis);
			}
			return this.WorldPosAtDepthTexCoord(A_2.texCoord + axis) - A_2.centerWorldPos;
		}

		[CompilerGenerated]
		[return: TupleElementNames(new string[]
		{
			"status",
			"position",
			"eyeIndex"
		})]
		private ValueTuple<DepthRaycastResult, Vector3, int> <Raycast>g__GetRaycastResultForEye|39_0(int index, ref EnvironmentDepthRaycaster.<>c__DisplayClass39_0 A_2)
		{
			if (!EnvironmentDepthRaycaster.ClampRayOriginToCamFrustumPlanes(ref A_2.ray, this._camFrustumPlanes[index], ref A_2.maxDistance))
			{
				return new ValueTuple<DepthRaycastResult, Vector3, int>(DepthRaycastResult.RayOutsideOfDepthCameraFrustum, default(Vector3), 0);
			}
			Plane plane = this._camFrustumPlanes[index][4];
			float num;
			if (Vector3.Dot(A_2.ray.direction, plane.normal) < 0f && plane.Raycast(A_2.ray, out num) && A_2.maxDistance > num)
			{
				A_2.maxDistance = num;
			}
			Vector3 item;
			return new ValueTuple<DepthRaycastResult, Vector3, int>(this.RaycastInternal(A_2.ray, out item, A_2.maxDistance, index, A_2.allowOccludedRayOrigin), item, index);
		}

		private static readonly int EnvironmentDepthTextureId = Shader.PropertyToID("_EnvironmentDepthTexture");

		private static readonly int EnvironmentDepthTextureSizeId = Shader.PropertyToID("_EnvironmentDepthTextureSize");

		private static readonly int CopiedDepthTextureId = Shader.PropertyToID("_CopiedDepthTexture");

		private static readonly int EnvironmentDepthZBufferParamsId = Shader.PropertyToID("_EnvironmentDepthZBufferParams");

		private const int TextureSize = 128;

		private const int NumEyes = 2;

		private ComputeShader _shader;

		internal EnvironmentDepthManager depthManager;

		private ComputeBuffer _computeBuffer;

		private NativeArray<float> _depthTexturePixels;

		private NativeArray<float> _gpuRequestBuffer;

		private bool _isDepthTextureAvailable;

		private AsyncGPUReadbackRequest? _currentGpuReadbackRequest;

		private RenderTexture _updatedDepthTexture;

		private readonly Matrix4x4[] _matrixVP = new Matrix4x4[2];

		private readonly Matrix4x4[] _matrixV = new Matrix4x4[2];

		private readonly Matrix4x4[] _matrixVP_inv = new Matrix4x4[2];

		private readonly Plane[][] _camFrustumPlanes = new Plane[][]
		{
			new Plane[6],
			new Plane[6]
		};

		private Vector4 _EnvironmentDepthZBufferParams;

		private readonly DepthFrameDesc[] _depthFrameDesc = new DepthFrameDesc[2];

		private Matrix4x4 _worldToTrackingSpaceMatrix = Matrix4x4.identity;

		internal bool _warmUpRaycast;

		private int _currentEyeIndex;

		private XRDisplaySubsystem _xrDisplay;
	}
}
