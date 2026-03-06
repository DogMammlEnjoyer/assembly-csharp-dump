using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace Liv.Lck
{
	public class LckHideObjectFromCamera : MonoBehaviour
	{
		private void OnEnable()
		{
			RenderPipelineManager.beginCameraRendering += this.BeginCameraRendering;
			RenderPipelineManager.endCameraRendering += this.EndCameraRendering;
			this._hiddenLayer = LayerMask.NameToLayer(this._hiddenLayerName);
			this._originalLayer = base.gameObject.layer;
			this.HideCanvases(base.transform);
		}

		private void EndCameraRendering(ScriptableRenderContext arg1, Camera cameraBeingRendered)
		{
			if (cameraBeingRendered != this._targetCamera)
			{
				return;
			}
			if (!this._dirty)
			{
				return;
			}
			this.FrameCleanup();
		}

		private void HideCanvases(Transform parent)
		{
			foreach (object obj in parent)
			{
				Transform transform = (Transform)obj;
				if (transform.GetComponent<Canvas>())
				{
					transform.gameObject.layer = this._hiddenLayer;
				}
				if (transform.GetComponent<TextMeshPro>())
				{
					transform.gameObject.layer = this._hiddenLayer;
				}
				this.HideCanvases(transform);
			}
		}

		private void BeginCameraRendering(ScriptableRenderContext scriptableRenderContext, Camera cameraBeingRendered)
		{
			if (cameraBeingRendered == this._targetCamera)
			{
				this.SetLayerRecursively(base.gameObject, this._hiddenLayer);
				this._dirty = true;
			}
		}

		private void FrameCleanup()
		{
			if (!this._dirty)
			{
				return;
			}
			this.SetLayerRecursively(base.gameObject, this._originalLayer);
			this._dirty = false;
		}

		private void SetLayerRecursively(GameObject obj, int newLayer)
		{
			if (obj == null)
			{
				return;
			}
			if (!obj.GetComponent<Canvas>())
			{
				obj.layer = newLayer;
			}
			foreach (object obj2 in obj.transform)
			{
				Transform transform = (Transform)obj2;
				if (transform != null)
				{
					this.SetLayerRecursively(transform.gameObject, newLayer);
				}
			}
		}

		private void OnDisable()
		{
			RenderPipelineManager.beginCameraRendering -= this.BeginCameraRendering;
			RenderPipelineManager.endCameraRendering += this.EndCameraRendering;
		}

		[SerializeField]
		private Camera _targetCamera;

		[SerializeField]
		private string _hiddenLayerName = "HideInRecording";

		private int _hiddenLayer;

		private int _originalLayer;

		private bool _dirty;
	}
}
