using System;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class MRPassThroughMaterialChanger : MonoBehaviour
	{
		protected virtual void Reset()
		{
			this._renderer = base.gameObject.GetComponent<Renderer>();
			this._material = this._renderer.material;
		}

		protected virtual void Start()
		{
			if (this._renderer == null)
			{
				this._renderer = base.gameObject.GetComponent<Renderer>();
				this._material = this._renderer.material;
			}
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		private void Update()
		{
			if (this._passThroughMaterial != null && MRPassthrough.PassThrough.IsPassThroughOn)
			{
				this._renderer.material = this._passThroughMaterial;
				return;
			}
			this._renderer.material = this._material;
		}

		public void InjectAllChanger(Material passthroughMaterial, Renderer render, Material material)
		{
			this.InjectPassthroughMaterial(passthroughMaterial);
			this.InjectRenderer(render);
			this.InjectMaterial(material);
		}

		public void InjectPassthroughMaterial(Material passthroughMaterial)
		{
			this._passThroughMaterial = passthroughMaterial;
		}

		public void InjectRenderer(Renderer render)
		{
			this._renderer = render;
		}

		public void InjectMaterial(Material material)
		{
			this._material = material;
		}

		[Header("Passthrough Material")]
		[Tooltip("Material that should be rendered during passthrough")]
		[SerializeField]
		private Material _passThroughMaterial;

		[Header("Current GameObject Material")]
		[SerializeField]
		private Material _material;

		[Tooltip("This current gameobject renderer")]
		[SerializeField]
		private Renderer _renderer;

		protected bool _started;
	}
}
