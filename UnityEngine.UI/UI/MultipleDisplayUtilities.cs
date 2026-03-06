using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	internal static class MultipleDisplayUtilities
	{
		public static bool GetRelativeMousePositionForDrag(PointerEventData eventData, ref Vector2 position)
		{
			int displayIndex = eventData.pointerPressRaycast.displayIndex;
			Vector3 vector = MultipleDisplayUtilities.RelativeMouseAtScaled(eventData.position, eventData.displayIndex);
			if ((int)vector.z != displayIndex)
			{
				return false;
			}
			position = ((displayIndex != 0) ? vector : eventData.position);
			return true;
		}

		internal static Vector3 GetRelativeMousePositionForRaycast(PointerEventData eventData)
		{
			Vector3 vector = MultipleDisplayUtilities.RelativeMouseAtScaled(eventData.position, eventData.displayIndex);
			if (vector == Vector3.zero)
			{
				vector = eventData.position;
			}
			if (eventData.displayIndex > 0)
			{
				vector.z = (float)eventData.displayIndex;
			}
			return vector;
		}

		public static Vector3 RelativeMouseAtScaled(Vector2 position, int displayIndex)
		{
			Display display = Display.main;
			if (displayIndex >= Display.displays.Length)
			{
				displayIndex = 0;
			}
			display = Display.displays[displayIndex];
			if (!Screen.fullScreen)
			{
				return new Vector3(position.x, position.y, (float)displayIndex);
			}
			if (display.renderingWidth != display.systemWidth || display.renderingHeight != display.systemHeight)
			{
				float num = (float)display.systemWidth / (float)display.systemHeight;
				Vector2 vector = new Vector2((float)display.renderingWidth, (float)display.renderingHeight);
				Vector2 zero = Vector2.zero;
				if (Screen.fullScreen)
				{
					float num2 = (float)Screen.width / (float)Screen.height;
					if ((float)display.systemHeight * num2 < (float)display.systemWidth)
					{
						vector.x = (float)display.renderingHeight * num;
						zero.x = (vector.x - (float)display.renderingWidth) * 0.5f;
					}
					else
					{
						vector.y = (float)display.renderingWidth / num;
						zero.y = (vector.y - (float)display.renderingHeight) * 0.5f;
					}
				}
				Vector2 vector2 = vector - zero;
				if (position.y < -zero.y || position.y > vector2.y || position.x < -zero.x || position.x > vector2.x)
				{
					Vector2 vector3 = position;
					if (!Screen.fullScreen)
					{
						vector3.x -= (float)(display.renderingWidth - display.systemWidth) * 0.5f;
						vector3.y -= (float)(display.renderingHeight - display.systemHeight) * 0.5f;
					}
					else
					{
						vector3 += zero;
						vector3.x *= (float)display.systemWidth / vector.x;
						vector3.y *= (float)display.systemHeight / vector.y;
					}
					Vector3 vector4 = new Vector3(vector3.x, vector3.y, (float)displayIndex);
					if (vector4.z != 0f)
					{
						return vector4;
					}
				}
				return new Vector3(position.x, position.y, 0f);
			}
			return new Vector3(position.x, position.y, (float)displayIndex);
		}
	}
}
