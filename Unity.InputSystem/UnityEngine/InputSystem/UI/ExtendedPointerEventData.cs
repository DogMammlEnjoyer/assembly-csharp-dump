using System;
using System.Text;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UIElements;

namespace UnityEngine.InputSystem.UI
{
	public class ExtendedPointerEventData : PointerEventData
	{
		public ExtendedPointerEventData(EventSystem eventSystem) : base(eventSystem)
		{
		}

		public InputControl control { get; set; }

		public InputDevice device { get; set; }

		public int touchId { get; set; }

		public UIPointerType pointerType { get; set; }

		public int uiToolkitPointerId { get; set; }

		public Vector3 trackedDevicePosition { get; set; }

		public Quaternion trackedDeviceOrientation { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.ToString());
			stringBuilder.AppendLine("button: " + base.button.ToString());
			stringBuilder.AppendLine("clickTime: " + base.clickTime.ToString());
			stringBuilder.AppendLine("clickCount: " + base.clickCount.ToString());
			string str = "device: ";
			InputDevice device = this.device;
			stringBuilder.AppendLine(str + ((device != null) ? device.ToString() : null));
			stringBuilder.AppendLine("pointerType: " + this.pointerType.ToString());
			stringBuilder.AppendLine("touchId: " + this.touchId.ToString());
			stringBuilder.AppendLine("pressPosition: " + base.pressPosition.ToString());
			stringBuilder.AppendLine("trackedDevicePosition: " + this.trackedDevicePosition.ToString());
			stringBuilder.AppendLine("trackedDeviceOrientation: " + this.trackedDeviceOrientation.ToString());
			stringBuilder.AppendLine("pressure" + base.pressure.ToString());
			stringBuilder.AppendLine("radius: " + base.radius.ToString());
			stringBuilder.AppendLine("azimuthAngle: " + base.azimuthAngle.ToString());
			stringBuilder.AppendLine("altitudeAngle: " + base.altitudeAngle.ToString());
			stringBuilder.AppendLine("twist: " + base.twist.ToString());
			stringBuilder.AppendLine("displayIndex: " + base.displayIndex.ToString());
			return stringBuilder.ToString();
		}

		internal static int MakePointerIdForTouch(int deviceId, int touchId)
		{
			return (deviceId << 24) + touchId;
		}

		internal static int TouchIdFromPointerId(int pointerId)
		{
			return pointerId & 255;
		}

		internal unsafe void ReadDeviceState()
		{
			Pen pen = this.control.parent as Pen;
			if (pen != null)
			{
				this.uiToolkitPointerId = ExtendedPointerEventData.GetPenPointerId(pen);
				base.pressure = pen.pressure.magnitude;
				base.azimuthAngle = (pen.tilt.value.x + 1f) * 3.1415927f / 2f;
				base.altitudeAngle = (pen.tilt.value.y + 1f) * 3.1415927f / 2f;
				base.twist = *pen.twist.value * 3.1415927f * 2f;
				base.displayIndex = pen.displayIndex.ReadValue();
				return;
			}
			TouchControl touchControl = this.control.parent as TouchControl;
			if (touchControl != null)
			{
				this.uiToolkitPointerId = ExtendedPointerEventData.GetTouchPointerId(touchControl);
				base.pressure = touchControl.pressure.magnitude;
				base.radius = *touchControl.radius.value;
				base.displayIndex = touchControl.displayIndex.ReadValue();
				return;
			}
			Touchscreen touchscreen = this.control.parent as Touchscreen;
			if (touchscreen != null)
			{
				this.uiToolkitPointerId = ExtendedPointerEventData.GetTouchPointerId(touchscreen.primaryTouch);
				base.pressure = touchscreen.pressure.magnitude;
				base.radius = *touchscreen.radius.value;
				base.displayIndex = touchscreen.displayIndex.ReadValue();
				return;
			}
			this.uiToolkitPointerId = PointerId.mousePointerId;
		}

		private static int GetPenPointerId(Pen pen)
		{
			int num = 0;
			foreach (InputDevice inputDevice in InputSystem.devices)
			{
				Pen pen2 = inputDevice as Pen;
				if (pen2 != null)
				{
					if (pen == pen2)
					{
						return PointerId.penPointerIdBase + Mathf.Min(num, PointerId.penPointerCount - 1);
					}
					num++;
				}
			}
			return PointerId.penPointerIdBase;
		}

		private static int GetTouchPointerId(TouchControl touchControl)
		{
			int value = ((Touchscreen)touchControl.device).touches.IndexOfReference(touchControl);
			return PointerId.touchPointerIdBase + Mathf.Clamp(value, 0, PointerId.touchPointerCount - 1);
		}
	}
}
