using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteAlways]
[HelpURL("https://developer.oculus.com/documentation/unity/unity-sf-stereo180video/")]
public class OVROverlayMeshGenerator : MonoBehaviour
{
	protected void OnEnable()
	{
		this.Initialize();
	}

	protected void OnDestroy()
	{
		if (this._Mesh != null)
		{
			if (Application.isPlaying)
			{
				Object.Destroy(this._Mesh);
			}
			else
			{
				Object.DestroyImmediate(this._Mesh);
			}
		}
		if (this._PreviewMaterial != null)
		{
			if (Application.isPlaying)
			{
				Object.Destroy(this._PreviewMaterial);
				return;
			}
			Object.DestroyImmediate(this._PreviewMaterial);
		}
	}

	private void Initialize()
	{
		this._MeshFilter = base.GetComponent<MeshFilter>();
		this._MeshRenderer = base.GetComponent<MeshRenderer>();
		this._MeshCollider = base.GetComponent<MeshCollider>();
		this._Transform = base.transform;
		if (Camera.main && Camera.main.transform.parent)
		{
			this._CameraRoot = Camera.main.transform.parent;
		}
		this.TryUpdateMesh();
	}

	public void SetOverlay(OVROverlay overlay)
	{
		this._Overlay = overlay;
		this.Initialize();
	}

	private void TryUpdateMesh()
	{
		if (this._Overlay == null)
		{
			return;
		}
		if (this._Mesh == null)
		{
			this._Mesh = new Mesh
			{
				name = "Overlay"
			};
			this._Mesh.hideFlags = (HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild);
		}
		if (this._Transform == null)
		{
			this._Transform = base.transform;
		}
		OVROverlay.OverlayShape currentOverlayShape = this._Overlay.currentOverlayShape;
		Vector3 vector = this._CameraRoot ? (this._Transform.position - this._CameraRoot.position) : this._Transform.position;
		Quaternion rotation = this._Transform.rotation;
		Vector3 lossyScale = this._Transform.lossyScale;
		Rect rect = this._Overlay.overrideTextureRectMatrix ? this._Overlay.destRectLeft : new Rect(0f, 0f, 1f, 1f);
		Rect rect2 = this._Overlay.overrideTextureRectMatrix ? this._Overlay.srcRectLeft : new Rect(0f, 0f, 1f, 1f);
		Texture texture = this._Overlay.textures[0];
		TextureDimension textureDimension = (texture != null) ? texture.dimension : TextureDimension.Tex2D;
		if (this._LastShape != currentOverlayShape || this._LastPosition != vector || this._LastRotation != rotation || this._LastScale != lossyScale || this._LastDestRectLeft != rect || this._LastTextureDimension != textureDimension)
		{
			this.UpdateMesh(currentOverlayShape, vector, rotation, lossyScale, rect, textureDimension == TextureDimension.Cube);
		}
		if (this._MeshRenderer)
		{
			if (this._MeshRenderer.sharedMaterial == null || textureDimension != this._LastTextureDimension)
			{
				if (this._PreviewMaterial != null)
				{
					if (Application.isPlaying)
					{
						Object.Destroy(this._PreviewMaterial);
					}
					else
					{
						Object.DestroyImmediate(this._PreviewMaterial);
					}
				}
				this._PreviewMaterial = null;
				if (textureDimension != TextureDimension.Tex2D)
				{
					if (textureDimension == TextureDimension.Cube)
					{
						this._PreviewMaterial = new Material(Shader.Find("Hidden/CubeCopy"));
					}
				}
				else
				{
					this._PreviewMaterial = new Material(Shader.Find("Unlit/Transparent"));
				}
				if (this._PreviewMaterial != null)
				{
					this._PreviewMaterial.mainTexture = texture;
				}
				this._MeshRenderer.sharedMaterial = this._PreviewMaterial;
			}
			if (this._LastSrcRectLeft != rect2)
			{
				this._MeshRenderer.sharedMaterial.mainTextureOffset = rect2.position;
				this._MeshRenderer.sharedMaterial.mainTextureScale = rect2.size;
			}
			if (this._MeshRenderer.sharedMaterial.mainTexture != texture)
			{
				this._MeshRenderer.sharedMaterial.mainTexture = texture;
			}
		}
		if (this._MeshFilter)
		{
			this._MeshFilter.sharedMesh = this._Mesh;
		}
		if (this._MeshCollider)
		{
			this._MeshCollider.sharedMesh = this._Mesh;
		}
		this._LastShape = currentOverlayShape;
		this._LastPosition = vector;
		this._LastRotation = rotation;
		this._LastScale = lossyScale;
		this._LastDestRectLeft = rect;
		this._LastSrcRectLeft = rect2;
		this._LastTextureDimension = textureDimension;
	}

	private void UpdateMesh(OVROverlay.OverlayShape shape, Vector3 position, Quaternion rotation, Vector3 scale, Rect rect, bool cubemap = false)
	{
		this._Verts.Clear();
		this._UV.Clear();
		this._CubeUV.Clear();
		this._Tris.Clear();
		OVROverlayMeshGenerator.GenerateMesh(this._Verts, this._UV, this._CubeUV, this._Tris, shape, position, rotation, scale, rect);
		this._Mesh.Clear(false);
		this._Mesh.SetVertices(this._Verts);
		if (cubemap)
		{
			this._Mesh.SetUVs(0, this._CubeUV);
		}
		else
		{
			this._Mesh.SetUVs(0, this._UV);
		}
		this._Mesh.SetTriangles(this._Tris, 0);
		this._Mesh.UploadMeshData(false);
	}

	public static void GenerateMesh(List<Vector3> verts, List<Vector2> uvs, List<Vector4> cubeUVs, List<int> tris, OVROverlay.OverlayShape shape, Vector3 position, Quaternion rotation, Vector3 scale, Rect rect)
	{
		switch (shape)
		{
		case OVROverlay.OverlayShape.Quad:
			OVROverlayMeshGenerator.BuildQuad(verts, uvs, tris, rect);
			return;
		case OVROverlay.OverlayShape.Cylinder:
			OVROverlayMeshGenerator.BuildHemicylinder(verts, uvs, tris, scale, rect, 128);
			break;
		case OVROverlay.OverlayShape.Cubemap:
		case OVROverlay.OverlayShape.OffcenterCubemap:
			OVROverlayMeshGenerator.BuildCube(verts, uvs, cubeUVs, tris, position, rotation, scale, 800f, 1, 1.01f);
			return;
		case (OVROverlay.OverlayShape)3:
			break;
		case OVROverlay.OverlayShape.Equirect:
			OVROverlayMeshGenerator.BuildSphere(verts, uvs, tris, position, rotation, scale, rect, 800f, 128, 128, 1f);
			return;
		default:
			return;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Vector3 InverseTransformVert(in Vector3 vert, in Vector3 position, in Vector3 scale, float worldScale)
	{
		return new Vector3((worldScale * vert.x - position.x) / scale.x, (worldScale * vert.y - position.y) / scale.y, (worldScale * vert.z - position.z) / scale.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Vector2 GetSphereUV(float theta, float phi, float expandScale)
	{
		float x = expandScale * (theta / 6.2831855f - 0.5f) + 0.5f;
		float y = expandScale * phi / 3.1415927f + 0.5f;
		return new Vector2(x, y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Vector3 GetSphereVert(float theta, float phi)
	{
		return new Vector3(-Mathf.Sin(theta) * Mathf.Cos(phi), Mathf.Sin(phi), -Mathf.Cos(theta) * Mathf.Cos(phi));
	}

	public static void BuildSphere(List<Vector3> verts, List<Vector2> uv, List<int> triangles, Vector3 position, Quaternion rotation, Vector3 scale, Rect rect, float worldScale = 800f, int latitudes = 128, int longitudes = 128, float expandCoefficient = 1f)
	{
		position = Quaternion.Inverse(rotation) * position;
		latitudes = Mathf.CeilToInt((float)latitudes * rect.height);
		longitudes = Mathf.CeilToInt((float)longitudes * rect.width);
		float num = 6.2831855f * rect.x;
		float num2 = 3.1415927f * (0.5f - rect.y - rect.height);
		float num3 = 6.2831855f * rect.width / (float)longitudes;
		float num4 = 3.1415927f * rect.height / (float)latitudes;
		float expandScale = 1f / expandCoefficient;
		for (int i = 0; i < latitudes + 1; i++)
		{
			for (int j = 0; j < longitudes + 1; j++)
			{
				float theta = num + (float)j * num3;
				float phi = num2 + (float)i * num4;
				Vector2 sphereUV = OVROverlayMeshGenerator.GetSphereUV(theta, phi, expandScale);
				uv.Add(new Vector2((sphereUV.x - rect.x) / rect.width, (sphereUV.y - rect.y) / rect.height));
				Vector3 sphereVert = OVROverlayMeshGenerator.GetSphereVert(theta, phi);
				verts.Add(OVROverlayMeshGenerator.InverseTransformVert(sphereVert, position, scale, worldScale));
			}
		}
		for (int k = 0; k < latitudes; k++)
		{
			for (int l = 0; l < longitudes; l++)
			{
				triangles.Add(k * (longitudes + 1) + l);
				triangles.Add((k + 1) * (longitudes + 1) + l);
				triangles.Add((k + 1) * (longitudes + 1) + l + 1);
				triangles.Add((k + 1) * (longitudes + 1) + l + 1);
				triangles.Add(k * (longitudes + 1) + l + 1);
				triangles.Add(k * (longitudes + 1) + l);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Vector2 GetCubeUV(OVROverlayMeshGenerator.CubeFace face, float sideU, float sideV, float expandScale, float expandOffset)
	{
		sideU = sideU * expandScale + expandOffset;
		sideV = sideV * expandScale + expandOffset;
		switch (face)
		{
		case OVROverlayMeshGenerator.CubeFace.Bottom:
			return new Vector2(sideU / 3f, sideV / 2f);
		case OVROverlayMeshGenerator.CubeFace.Front:
			return new Vector2((1f + sideU) / 3f, sideV / 2f);
		case OVROverlayMeshGenerator.CubeFace.Back:
			return new Vector2((2f + sideU) / 3f, sideV / 2f);
		case OVROverlayMeshGenerator.CubeFace.Right:
			return new Vector2(sideU / 3f, (1f + sideV) / 2f);
		case OVROverlayMeshGenerator.CubeFace.Left:
			return new Vector2((1f + sideU) / 3f, (1f + sideV) / 2f);
		case OVROverlayMeshGenerator.CubeFace.Top:
			return new Vector2((2f + sideU) / 3f, (1f + sideV) / 2f);
		default:
			return Vector2.zero;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Vector3 GetCubeVert(OVROverlayMeshGenerator.CubeFace face, float sideU, float sideV)
	{
		switch (face)
		{
		case OVROverlayMeshGenerator.CubeFace.Bottom:
			return new Vector3(0.5f - sideU, -0.5f, 0.5f - sideV);
		case OVROverlayMeshGenerator.CubeFace.Front:
			return new Vector3(0.5f - sideU, -0.5f + sideV, -0.5f);
		case OVROverlayMeshGenerator.CubeFace.Back:
			return new Vector3(-0.5f + sideU, -0.5f + sideV, 0.5f);
		case OVROverlayMeshGenerator.CubeFace.Right:
			return new Vector3(-0.5f, -0.5f + sideV, -0.5f + sideU);
		case OVROverlayMeshGenerator.CubeFace.Left:
			return new Vector3(0.5f, -0.5f + sideV, 0.5f - sideU);
		case OVROverlayMeshGenerator.CubeFace.Top:
			return new Vector3(0.5f - sideU, 0.5f, -0.5f + sideV);
		default:
			return Vector3.zero;
		}
	}

	public static void BuildCube(List<Vector3> verts, List<Vector2> uv, List<Vector4> cubeUV, List<int> triangles, Vector3 position, Quaternion rotation, Vector3 scale, float worldScale = 800f, int subQuads = 1, float expandCoefficient = 1.01f)
	{
		position = Quaternion.Inverse(rotation) * position;
		int num = (subQuads + 1) * (subQuads + 1);
		float expandScale = 1f / expandCoefficient;
		float expandOffset = 0.5f - 0.5f / expandCoefficient;
		for (int i = 0; i < 6; i++)
		{
			for (int j = 0; j < subQuads + 1; j++)
			{
				for (int k = 0; k < subQuads + 1; k++)
				{
					float sideU = (float)j / (float)subQuads;
					float sideV = (float)k / (float)subQuads;
					uv.Add(OVROverlayMeshGenerator.GetCubeUV((OVROverlayMeshGenerator.CubeFace)i, sideU, sideV, expandScale, expandOffset));
					Vector3 cubeVert = OVROverlayMeshGenerator.GetCubeVert((OVROverlayMeshGenerator.CubeFace)i, sideU, sideV);
					verts.Add(OVROverlayMeshGenerator.InverseTransformVert(cubeVert, position, scale, worldScale));
					cubeUV.Add(cubeVert.normalized);
				}
			}
			for (int l = 0; l < subQuads; l++)
			{
				for (int m = 0; m < subQuads; m++)
				{
					triangles.Add(num * i + (l + 1) * (subQuads + 1) + m);
					triangles.Add(num * i + l * (subQuads + 1) + m);
					triangles.Add(num * i + (l + 1) * (subQuads + 1) + m + 1);
					triangles.Add(num * i + (l + 1) * (subQuads + 1) + m + 1);
					triangles.Add(num * i + l * (subQuads + 1) + m);
					triangles.Add(num * i + l * (subQuads + 1) + m + 1);
				}
			}
		}
	}

	public static void BuildQuad(List<Vector3> verts, List<Vector2> uv, List<int> triangles, Rect rect)
	{
		verts.Add(new Vector3(rect.x - 0.5f, 1f - rect.y - rect.height - 0.5f, 0f));
		verts.Add(new Vector3(rect.x - 0.5f, 1f - rect.y - 0.5f, 0f));
		verts.Add(new Vector3(rect.x + rect.width - 0.5f, 1f - rect.y - 0.5f, 0f));
		verts.Add(new Vector3(rect.x + rect.width - 0.5f, 1f - rect.y - rect.height - 0.5f, 0f));
		uv.Add(new Vector2(0f, 0f));
		uv.Add(new Vector2(0f, 1f));
		uv.Add(new Vector2(1f, 1f));
		uv.Add(new Vector2(1f, 0f));
		triangles.Add(0);
		triangles.Add(1);
		triangles.Add(2);
		triangles.Add(2);
		triangles.Add(3);
		triangles.Add(0);
	}

	public static void BuildHemicylinder(List<Vector3> verts, List<Vector2> uv, List<int> triangles, Vector3 scale, Rect rect, int longitudes = 128)
	{
		float num = Mathf.Abs(scale.y) * rect.height;
		float z = scale.z;
		float num2 = scale.x * rect.width;
		float num3 = num2 / z;
		float num4 = scale.x * (-0.5f + rect.x) / z;
		int num5 = Mathf.CeilToInt((float)longitudes * num3 / 6.2831855f);
		float num6 = num2 / (float)num5;
		int num7 = Mathf.CeilToInt(num / num6 / 2f);
		for (int i = 0; i < num7 + 1; i++)
		{
			for (int j = 0; j < num5 + 1; j++)
			{
				uv.Add(new Vector2((float)j / (float)num5, 1f - (float)i / (float)num7));
				verts.Add(new Vector3(Mathf.Sin(num4 + (float)j * num3 / (float)num5) * z / scale.x, 0.5f - rect.y - rect.height + rect.height * (1f - (float)i / (float)num7), Mathf.Cos(num4 + (float)j * num3 / (float)num5) * z / scale.z));
			}
		}
		for (int k = 0; k < num7; k++)
		{
			for (int l = 0; l < num5; l++)
			{
				triangles.Add(k * (num5 + 1) + l);
				triangles.Add((k + 1) * (num5 + 1) + l + 1);
				triangles.Add((k + 1) * (num5 + 1) + l);
				triangles.Add((k + 1) * (num5 + 1) + l + 1);
				triangles.Add(k * (num5 + 1) + l);
				triangles.Add(k * (num5 + 1) + l + 1);
			}
		}
	}

	private readonly List<int> _Tris = new List<int>();

	private readonly List<Vector2> _UV = new List<Vector2>();

	private readonly List<Vector4> _CubeUV = new List<Vector4>();

	private readonly List<Vector3> _Verts = new List<Vector3>();

	private Transform _CameraRoot;

	private Rect _LastDestRectLeft;

	private Vector3 _LastPosition;

	private Quaternion _LastRotation;

	private Vector3 _LastScale;

	private TextureDimension _LastTextureDimension = TextureDimension.Tex2D;

	private OVROverlay.OverlayShape _LastShape;

	private Rect _LastSrcRectLeft;

	private Mesh _Mesh;

	private MeshCollider _MeshCollider;

	private MeshFilter _MeshFilter;

	private MeshRenderer _MeshRenderer;

	private OVROverlay _Overlay;

	private Transform _Transform;

	private Material _PreviewMaterial;

	private enum CubeFace
	{
		Bottom,
		Front,
		Back,
		Right,
		Left,
		Top,
		COUNT
	}
}
