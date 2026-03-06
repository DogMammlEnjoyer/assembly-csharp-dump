using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.UnityCanvas
{
	[DisallowMultipleComponent]
	public abstract class CanvasMesh : MonoBehaviour
	{
		protected abstract Vector3 MeshInverseTransform(Vector3 localPosition);

		protected abstract void GenerateMesh(out List<Vector3> verts, out List<int> tris, out List<Vector2> uvs);

		public Vector3 ImposterToCanvasTransformPoint(Vector3 worldPosition)
		{
			Vector3 localPosition = this._meshFilter.transform.InverseTransformPoint(worldPosition);
			Vector3 position = this.MeshInverseTransform(localPosition) / this._canvasRenderTexture.transform.localScale.x;
			return this._canvasRenderTexture.transform.TransformPoint(position);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.UpdateImposter();
				CanvasRenderTexture canvasRenderTexture = this._canvasRenderTexture;
				canvasRenderTexture.OnUpdateRenderTexture = (Action<Texture>)Delegate.Combine(canvasRenderTexture.OnUpdateRenderTexture, new Action<Texture>(this.HandleUpdateRenderTexture));
				if (this._canvasRenderTexture.Texture != null)
				{
					this.HandleUpdateRenderTexture(this._canvasRenderTexture.Texture);
				}
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				CanvasRenderTexture canvasRenderTexture = this._canvasRenderTexture;
				canvasRenderTexture.OnUpdateRenderTexture = (Action<Texture>)Delegate.Remove(canvasRenderTexture.OnUpdateRenderTexture, new Action<Texture>(this.HandleUpdateRenderTexture));
			}
		}

		protected virtual void HandleUpdateRenderTexture(Texture texture)
		{
			this.UpdateImposter();
		}

		protected virtual void UpdateImposter()
		{
			try
			{
				List<Vector3> vertices;
				List<int> triangles;
				List<Vector2> uvs;
				this.GenerateMesh(out vertices, out triangles, out uvs);
				Mesh mesh = new Mesh();
				mesh.SetVertices(vertices);
				mesh.SetUVs(0, uvs);
				mesh.SetTriangles(triangles, 0);
				mesh.RecalculateBounds();
				mesh.RecalculateNormals();
				this._meshFilter.mesh = mesh;
				if (this._meshCollider != null)
				{
					this._meshCollider.sharedMesh = this._meshFilter.sharedMesh;
				}
			}
			finally
			{
			}
		}

		public void InjectAllCanvasMesh(CanvasRenderTexture canvasRenderTexture, MeshFilter meshFilter)
		{
			this.InjectCanvasRenderTexture(canvasRenderTexture);
			this.InjectMeshFilter(meshFilter);
		}

		public void InjectCanvasRenderTexture(CanvasRenderTexture canvasRenderTexture)
		{
			this._canvasRenderTexture = canvasRenderTexture;
		}

		public void InjectMeshFilter(MeshFilter meshFilter)
		{
			this._meshFilter = meshFilter;
		}

		public void InjectOptionalMeshCollider(MeshCollider meshCollider)
		{
			this._meshCollider = meshCollider;
		}

		[Tooltip("Mesh construction will be driven by this texture.")]
		[SerializeField]
		protected CanvasRenderTexture _canvasRenderTexture;

		[Tooltip("The mesh filter that will be driven.")]
		[SerializeField]
		protected MeshFilter _meshFilter;

		[Tooltip("Optional mesh collider that will be driven.")]
		[SerializeField]
		[Optional]
		protected MeshCollider _meshCollider;

		protected bool _started;
	}
}
