using System;

namespace UnityEngine.UIElements
{
	internal class PhysicsDocumentPicker
	{
		private void Pick(Ray worldRay, float maxDistance, int layerMask, out Collider collider, out UIDocument document, out VisualElement pickedElement, out float distance)
		{
			WorldSpaceInput.PickResult pickResult = WorldSpaceInput.PickDocument3D(worldRay, maxDistance, layerMask);
			collider = pickResult.collider;
			document = pickResult.document;
			pickedElement = pickResult.pickedElement;
			distance = pickResult.distance;
		}

		public bool TryPickWithCapture(int pointerId, Ray worldRay, float maxDistance, int layerMask, out Collider collider, out UIDocument document, out VisualElement elementUnderPointer, out float distance, out bool captured)
		{
			UIDocument uidocument;
			captured = this.GetCapturingDocument(pointerId, out uidocument);
			bool flag = !captured;
			bool result;
			if (flag)
			{
				this.Pick(worldRay, maxDistance, layerMask, out collider, out document, out elementUnderPointer, out distance);
				result = !float.IsPositiveInfinity(distance);
			}
			else
			{
				bool flag2 = uidocument != null && (1 << uidocument.gameObject.layer & layerMask) != 0;
				if (flag2)
				{
					collider = null;
					document = uidocument;
					elementUnderPointer = WorldSpaceInput.Pick3D(document, worldRay, out distance);
					result = true;
				}
				else
				{
					collider = null;
					document = null;
					elementUnderPointer = null;
					distance = 0f;
					result = false;
				}
			}
			return result;
		}

		private bool GetCapturingDocument(int pointerId, out UIDocument capturingDocument)
		{
			IEventHandler capturingElement = RuntimePanel.s_EventDispatcher.pointerState.GetCapturingElement(pointerId);
			VisualElement visualElement = capturingElement as VisualElement;
			bool flag = visualElement != null;
			if (flag)
			{
				BaseVisualElementPanel elementPanel = visualElement.elementPanel;
				bool flag2 = elementPanel != null && !elementPanel.isFlat;
				if (flag2)
				{
					capturingDocument = UIDocument.FindRootUIDocument(visualElement);
					bool flag3 = capturingDocument != null;
					if (flag3)
					{
						return true;
					}
				}
			}
			RuntimePanel playerPanelWithSoftPointerCapture = PointerDeviceState.GetPlayerPanelWithSoftPointerCapture(pointerId);
			bool flag4 = playerPanelWithSoftPointerCapture != null;
			if (flag4)
			{
				bool flag5 = !playerPanelWithSoftPointerCapture.isFlat;
				if (flag5)
				{
					capturingDocument = PointerDeviceState.GetWorldSpaceDocumentWithSoftPointerCapture(pointerId);
					bool flag6 = capturingDocument != null;
					if (flag6)
					{
						return true;
					}
				}
			}
			capturingDocument = null;
			return false;
		}
	}
}
