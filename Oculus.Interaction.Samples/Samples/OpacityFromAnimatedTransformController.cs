using System;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class OpacityFromAnimatedTransformController : MonoBehaviour
	{
		private void Start()
		{
			this._isSkinnedMeshRenderer = (this._renderer is SkinnedMeshRenderer);
			if (!this._isSkinnedMeshRenderer)
			{
				this._materialProperties = new MaterialPropertyBlock();
			}
		}

		private void Update()
		{
			float value = Mathf.Abs(this._opacityTransform.localPosition.x);
			if (this._isSkinnedMeshRenderer)
			{
				this._renderer.material.SetFloat("_Opacity", value);
				return;
			}
			this._materialProperties.SetFloat("_Opacity", value);
			this._renderer.SetPropertyBlock(this._materialProperties);
		}

		[SerializeField]
		[Tooltip("The renderer to which the opacity should be applied")]
		private Renderer _renderer;

		[SerializeField]
		[Tooltip("The animation-controlled transform whose X magnitude will be applied to the renderer as `_Opacity`")]
		private Transform _opacityTransform;

		private MaterialPropertyBlock _materialProperties;

		private bool _isSkinnedMeshRenderer;
	}
}
