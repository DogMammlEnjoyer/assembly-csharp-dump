using System;
using UnityEngine.InputSystem;

namespace UnityEngine.VFX.Utility
{
	[RequireComponent(typeof(Collider))]
	internal class VFXMouseEventBinder : VFXEventBinderBase
	{
		protected override void SetEventAttribute(object[] parameters)
		{
			if (this.RaycastMousePosition)
			{
				Ray ray = Camera.main.ScreenPointToRay(VFXMouseEventBinder.GetMousePosition());
				RaycastHit raycastHit;
				if (base.GetComponent<Collider>().Raycast(ray, out raycastHit, 3.4028235E+38f))
				{
					this.eventAttribute.SetVector3(this.position, raycastHit.point);
				}
			}
		}

		private void Awake()
		{
			InputActionMap map = new InputActionMap("VFX Mouse Event Binder");
			this.mouseDown = map.AddAction("Mouse Down", InputActionType.Value, "<Mouse>/leftButton", "press(behavior=0)", null, null, null);
			this.mouseDown.performed += delegate(InputAction.CallbackContext ctx)
			{
				this.RayCastAndTriggerEvent(new Action(this.DoOnMouseDown));
			};
			this.mouseUp = map.AddAction("Mouse Up", InputActionType.Value, "<Mouse>/leftButton", "press(behavior=1)", null, null, null);
			this.mouseUp.performed += delegate(InputAction.CallbackContext ctx)
			{
				this.RayCastAndTriggerEvent(new Action(this.DoOnMouseUp));
			};
			this.mouseDragStart = map.AddAction("Mouse Drag Start", InputActionType.Value, "<Mouse>/leftButton", "press(behavior=0)", null, null, null);
			this.mouseDragStop = map.AddAction("Mouse Drag Stop", InputActionType.Value, "<Mouse>/leftButton", "press(behavior=1)", null, null, null);
		}

		private void RaycastMainCamera()
		{
			Ray ray = Camera.main.ScreenPointToRay(VFXMouseEventBinder.GetMousePosition());
			RaycastHit raycastHit;
			bool flag = base.GetComponent<Collider>().Raycast(ray, out raycastHit, float.MaxValue);
			if (this.mouseOver != flag)
			{
				this.mouseOver = flag;
				if (flag)
				{
					this.DoOnMouseOver();
					return;
				}
				this.DoOnMouseExit();
			}
		}

		private void RayCastDrag()
		{
			this.RayCastAndTriggerEvent(new Action(this.DoOnMouseDrag));
		}

		private void RayCastAndTriggerEvent(Action trigger)
		{
			Ray ray = Camera.main.ScreenPointToRay(VFXMouseEventBinder.GetMousePosition());
			RaycastHit raycastHit;
			if (base.GetComponent<Collider>().Raycast(ray, out raycastHit, 3.4028235E+38f))
			{
				trigger();
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.mouseDown.Enable();
			this.mouseUp.Enable();
			this.mouseDragStart.Enable();
		}

		private void OnDisable()
		{
			this.mouseDown.Disable();
			this.mouseUp.Disable();
			this.mouseDragStart.Disable();
		}

		private static Vector2 GetMousePosition()
		{
			return Pointer.current.position.ReadValue();
		}

		private void DoOnMouseDown()
		{
			if (this.activation == VFXMouseEventBinder.Activation.OnMouseDown)
			{
				base.SendEventToVisualEffect(Array.Empty<object>());
			}
		}

		private void DoOnMouseUp()
		{
			if (this.activation == VFXMouseEventBinder.Activation.OnMouseUp)
			{
				base.SendEventToVisualEffect(Array.Empty<object>());
			}
		}

		private void DoOnMouseDrag()
		{
			if (this.activation == VFXMouseEventBinder.Activation.OnMouseDrag)
			{
				base.SendEventToVisualEffect(Array.Empty<object>());
			}
		}

		private void DoOnMouseOver()
		{
			if (this.activation == VFXMouseEventBinder.Activation.OnMouseOver)
			{
				base.SendEventToVisualEffect(Array.Empty<object>());
			}
		}

		private void DoOnMouseEnter()
		{
			if (this.activation == VFXMouseEventBinder.Activation.OnMouseEnter)
			{
				base.SendEventToVisualEffect(Array.Empty<object>());
			}
		}

		private void DoOnMouseExit()
		{
			if (this.activation == VFXMouseEventBinder.Activation.OnMouseExit)
			{
				base.SendEventToVisualEffect(Array.Empty<object>());
			}
		}

		private void OnMouseDown()
		{
			this.DoOnMouseDown();
		}

		private void OnMouseUp()
		{
			this.DoOnMouseUp();
		}

		private void OnMouseDrag()
		{
			this.DoOnMouseDrag();
		}

		private void OnMouseOver()
		{
			this.DoOnMouseOver();
		}

		private void OnMouseEnter()
		{
			this.DoOnMouseEnter();
		}

		private void OnMouseExit()
		{
			this.DoOnMouseExit();
		}

		public VFXMouseEventBinder.Activation activation = VFXMouseEventBinder.Activation.OnMouseDown;

		private ExposedProperty position = "position";

		[Tooltip("Computes intersection in world space and sets it to the position EventAttribute")]
		public bool RaycastMousePosition;

		private InputAction mouseDown;

		private InputAction mouseUp;

		private InputAction mouseDragStart;

		private InputAction mouseDragStop;

		private InputAction mouseEnter;

		private bool mouseOver;

		private bool drag;

		public enum Activation
		{
			OnMouseUp,
			OnMouseDown,
			OnMouseEnter,
			OnMouseExit,
			OnMouseOver,
			OnMouseDrag
		}
	}
}
