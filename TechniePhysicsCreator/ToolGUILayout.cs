using System;
using System.Collections.Generic;
using UnityEngine;

namespace Technie.PhysicsCreator
{
	public class ToolGUILayout
	{
		public static bool Button(string buttonName, ref Vector2 buttonPos)
		{
			bool result = GUILayout.Button(buttonName, Array.Empty<GUILayoutOption>());
			if (Event.current.type == EventType.Repaint)
			{
				buttonPos = GUIUtility.GUIToScreenPoint(GUILayoutUtility.GetLastRect().center);
			}
			return result;
		}

		public static bool Button(string buttonId, Rect rect, GUIContent content, GUIStyle style)
		{
			bool result = GUI.Button(rect, content, style);
			if (Event.current.type == EventType.Repaint)
			{
				ToolGUILayout.buttonPositions[buttonId] = GUIUtility.GUIToScreenPoint(rect.center);
			}
			return result;
		}

		public static bool Button(string buttonId, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
		{
			bool result = GUILayout.Button(content, style, options);
			if (Event.current.type == EventType.Repaint)
			{
				Rect lastRect = GUILayoutUtility.GetLastRect();
				ToolGUILayout.buttonPositions[buttonId] = GUIUtility.GUIToScreenPoint(lastRect.center);
			}
			return result;
		}

		public static bool Button(string buttonId, string buttonName)
		{
			bool result = GUILayout.Button(buttonName, Array.Empty<GUILayoutOption>());
			if (Event.current.type == EventType.Repaint)
			{
				Vector2 value = GUIUtility.GUIToScreenPoint(GUILayoutUtility.GetLastRect().center);
				ToolGUILayout.buttonPositions[buttonId] = value;
			}
			return result;
		}

		public static Vector2 GetButtonPosition(string buttonId)
		{
			return ToolGUILayout.buttonPositions[buttonId];
		}

		public static Dictionary<string, Vector2> buttonPositions = new Dictionary<string, Vector2>();
	}
}
