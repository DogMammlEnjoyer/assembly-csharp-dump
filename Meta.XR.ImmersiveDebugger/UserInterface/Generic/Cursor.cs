using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	public class Cursor : OVRCursor
	{
		internal GameObject GameObject { get; private set; }

		private Transform Transform { get; set; }

		private void Awake()
		{
			this.GameObject = base.gameObject;
			this.GameObject.layer = RuntimeSettings.Instance.PanelLayer;
			this._canvas = this.GameObject.AddComponent<Canvas>();
			this._canvas.overrideSorting = true;
			this._canvas.sortingOrder = 31000;
			CanvasGroup canvasGroup = this.GameObject.AddComponent<CanvasGroup>();
			canvasGroup.blocksRaycasts = false;
			canvasGroup.interactable = false;
			RawImage rawImage = this.GameObject.AddComponent<RawImage>();
			rawImage.texture = Resources.Load<Texture2D>("Textures/pointer");
			rawImage.rectTransform.sizeDelta = new Vector2(20f, 20f);
			rawImage.raycastTarget = false;
			this.Transform = base.transform;
		}

		public override void SetCursorStartDest(Vector3 start, Vector3 dest, Vector3 normal)
		{
			this._endPoint = dest;
			this._normal = normal;
			this._hit = true;
		}

		public override void SetCursorRay(Transform t)
		{
			this._forward = t.forward;
			this._normal = this._forward;
			this._hit = false;
		}

		public void SetClickState(PointerEventData.FramePressState state)
		{
			if (state == PointerEventData.FramePressState.NotChanged)
			{
				if (this._pressState == PointerEventData.FramePressState.PressedAndReleased)
				{
					this._pressState = PointerEventData.FramePressState.Released;
				}
				return;
			}
			this._pressState = state;
		}

		private void LateUpdate()
		{
			if (this._hit)
			{
				this.Transform.position = this._endPoint;
				this.Transform.rotation = Quaternion.LookRotation(this._normal, Vector3.up);
				PointerEventData.FramePressState pressState = this._pressState;
				bool flag = pressState == PointerEventData.FramePressState.Pressed || pressState == PointerEventData.FramePressState.PressedAndReleased;
				this.Transform.localScale = Vector3.one * (flag ? 0.8f : 1f);
				return;
			}
			this.GameObject.SetActive(false);
		}

		internal void Attach(Panel panel)
		{
			if (panel == null)
			{
				return;
			}
			this.GameObject.SetActive(true);
			this.Transform.SetParent(panel.Transform, false);
			this._canvas.overrideSorting = true;
			this._canvas.sortingOrder = 31000;
		}

		private const float _pressedScale = 0.8f;

		private const float _releasedScale = 1f;

		private Vector3 _forward;

		private Vector3 _endPoint;

		private Vector3 _normal;

		private bool _hit;

		private PointerEventData.FramePressState _pressState = PointerEventData.FramePressState.Released;

		private Canvas _canvas;
	}
}
