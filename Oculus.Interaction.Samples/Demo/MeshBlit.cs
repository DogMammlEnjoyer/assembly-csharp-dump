using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Oculus.Interaction.Demo
{
	[RequireComponent(typeof(MeshFilter))]
	public class MeshBlit : MonoBehaviour
	{
		public float BlitsPerSecond
		{
			get
			{
				return this._blitsPerSecond;
			}
			set
			{
				this.SetBlitsPerSecond(value);
			}
		}

		private Mesh Mesh
		{
			get
			{
				if (!this._mesh)
				{
					return this._mesh = base.GetComponent<MeshFilter>().sharedMesh;
				}
				return this._mesh;
			}
		}

		private void OnEnable()
		{
			this.SetBlitsPerSecond(this._blitsPerSecond);
			base.StartCoroutine(this.<OnEnable>g__BlitRoutine|11_0());
		}

		public void Blit()
		{
			if (this.renderTexture == null)
			{
				throw new NullReferenceException("MeshBlit.Blit must have a RenderTexture assigned");
			}
			if (this.material == null)
			{
				throw new NullReferenceException("MeshBlit.Blit must have a Material assigned");
			}
			if (this.Mesh == null)
			{
				throw new NullReferenceException("MeshBlit.Blit's MeshFilter has no mesh");
			}
			RenderTexture temporary = RenderTexture.GetTemporary(this.renderTexture.descriptor);
			Graphics.Blit(this.renderTexture, temporary);
			this.material.SetTexture(MeshBlit.MAIN_TEX, temporary);
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = this.renderTexture;
			this.material.SetPass(0);
			Graphics.DrawMeshNow(this.Mesh, base.transform.localToWorldMatrix);
			RenderTexture.active = active;
			this.material.SetTexture(MeshBlit.MAIN_TEX, null);
			RenderTexture.ReleaseTemporary(temporary);
		}

		private void SetBlitsPerSecond(float value)
		{
			this._blitsPerSecond = value;
			this._waitForSeconds = ((value > 0f) ? new WaitForSeconds(1f / this._blitsPerSecond) : null);
		}

		[CompilerGenerated]
		private IEnumerator <OnEnable>g__BlitRoutine|11_0()
		{
			for (;;)
			{
				yield return this._waitForSeconds;
				this.Blit();
			}
			yield break;
		}

		private static int MAIN_TEX = Shader.PropertyToID("_MainTex");

		public Material material;

		public RenderTexture renderTexture;

		[SerializeField]
		private float _blitsPerSecond = -1f;

		private Mesh _mesh;

		private WaitForSeconds _waitForSeconds;
	}
}
