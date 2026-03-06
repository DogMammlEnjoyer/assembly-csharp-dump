using System;
using System.Collections.Generic;
using Drawing.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Drawing
{
	internal static class GeometryBuilder
	{
		internal unsafe static JobHandle Build(DrawingData gizmos, DrawingData.ProcessedBuilderData.MeshBuffers* buffers, ref GeometryBuilder.CameraInfo cameraInfo, JobHandle dependency)
		{
			return new GeometryBuilderJob
			{
				buffers = buffers,
				currentMatrix = Matrix4x4.identity,
				currentLineWidthData = new CommandBuilder.LineWidthData
				{
					pixels = 1f,
					automaticJoins = false
				},
				lineWidthMultiplier = DrawingManager.lineWidthMultiplier,
				currentColor = Color.white,
				cameraPosition = cameraInfo.cameraPosition,
				cameraRotation = cameraInfo.cameraRotation,
				cameraDepthToPixelSize = cameraInfo.cameraDepthToPixelSize,
				cameraIsOrthographic = cameraInfo.cameraIsOrthographic,
				characterInfo = (SDFCharacter*)gizmos.fontData.characters.GetUnsafeReadOnlyPtr<SDFCharacter>(),
				characterInfoLength = gizmos.fontData.characters.Length,
				maxPixelError = 0.5f / math.max(0.1f, gizmos.settingsRef.curveResolution)
			}.Schedule(dependency);
		}

		private static float2 CameraDepthToPixelSize(Camera camera)
		{
			if (camera.orthographic)
			{
				return new float2(0f, 2f * camera.orthographicSize / (float)camera.pixelHeight);
			}
			return new float2(Mathf.Tan(camera.fieldOfView * 0.017453292f * 0.5f) / (0.5f * (float)camera.pixelHeight), 0f);
		}

		private unsafe static NativeArray<T> ConvertExistingDataToNativeArray<T>(UnsafeAppendBuffer data) where T : struct
		{
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)data.Ptr, data.Length / UnsafeUtility.SizeOf<T>(), Allocator.Invalid);
		}

		internal unsafe static void BuildMesh(DrawingData gizmos, List<DrawingData.MeshWithType> meshes, DrawingData.ProcessedBuilderData.MeshBuffers* inputBuffers)
		{
			if (inputBuffers->triangles.Length > 0)
			{
				Mesh mesh = GeometryBuilder.AssignMeshData<GeometryBuilderJob.Vertex>(gizmos, inputBuffers->bounds, inputBuffers->vertices, inputBuffers->triangles, MeshLayouts.MeshLayout);
				meshes.Add(new DrawingData.MeshWithType
				{
					mesh = mesh,
					type = DrawingData.MeshType.Lines
				});
			}
			if (inputBuffers->solidTriangles.Length > 0)
			{
				Mesh mesh2 = GeometryBuilder.AssignMeshData<GeometryBuilderJob.Vertex>(gizmos, inputBuffers->bounds, inputBuffers->solidVertices, inputBuffers->solidTriangles, MeshLayouts.MeshLayout);
				meshes.Add(new DrawingData.MeshWithType
				{
					mesh = mesh2,
					type = DrawingData.MeshType.Solid
				});
			}
			if (inputBuffers->textTriangles.Length > 0)
			{
				Mesh mesh3 = GeometryBuilder.AssignMeshData<GeometryBuilderJob.TextVertex>(gizmos, inputBuffers->bounds, inputBuffers->textVertices, inputBuffers->textTriangles, MeshLayouts.MeshLayoutText);
				meshes.Add(new DrawingData.MeshWithType
				{
					mesh = mesh3,
					type = DrawingData.MeshType.Text
				});
			}
		}

		private static Mesh AssignMeshData<VertexType>(DrawingData gizmos, Bounds bounds, UnsafeAppendBuffer vertices, UnsafeAppendBuffer triangles, VertexAttributeDescriptor[] layout) where VertexType : struct
		{
			NativeArray<VertexType> data = GeometryBuilder.ConvertExistingDataToNativeArray<VertexType>(vertices);
			NativeArray<int> data2 = GeometryBuilder.ConvertExistingDataToNativeArray<int>(triangles);
			Mesh mesh = gizmos.GetMesh(data.Length);
			mesh.SetVertexBufferParams(math.ceilpow2(data.Length), layout);
			mesh.SetIndexBufferParams(math.ceilpow2(data2.Length), IndexFormat.UInt32);
			mesh.SetVertexBufferData<VertexType>(data, 0, 0, data.Length, 0, MeshUpdateFlags.Default);
			mesh.SetIndexBufferData<int>(data2, 0, 0, data2.Length, MeshUpdateFlags.DontValidateIndices);
			mesh.subMeshCount = 1;
			SubMeshDescriptor desc = new SubMeshDescriptor(0, data2.Length, MeshTopology.Triangles)
			{
				vertexCount = data.Length,
				bounds = bounds
			};
			mesh.SetSubMesh(0, desc, MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontRecalculateBounds);
			mesh.bounds = bounds;
			return mesh;
		}

		public struct CameraInfo
		{
			public CameraInfo(Camera camera)
			{
				Transform transform = (camera != null) ? camera.transform : null;
				this.cameraPosition = ((transform != null) ? transform.position : float3.zero);
				this.cameraRotation = ((transform != null) ? transform.rotation : quaternion.identity);
				this.cameraDepthToPixelSize = ((camera != null) ? GeometryBuilder.CameraDepthToPixelSize(camera) : 0);
				this.cameraIsOrthographic = (camera != null && camera.orthographic);
			}

			public float3 cameraPosition;

			public quaternion cameraRotation;

			public float2 cameraDepthToPixelSize;

			public bool cameraIsOrthographic;
		}
	}
}
