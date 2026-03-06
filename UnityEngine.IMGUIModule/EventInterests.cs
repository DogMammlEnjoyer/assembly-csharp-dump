using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEngine.UIElementsModule"
	})]
	internal struct EventInterests
	{
		public bool wantsMouseMove { readonly get; set; }

		public bool wantsMouseEnterLeaveWindow { readonly get; set; }

		public bool wantsLessLayoutEvents { readonly get; set; }

		public bool WantsEvent(EventType type)
		{
			bool result;
			if (type != EventType.MouseMove)
			{
				result = (type - EventType.MouseEnterWindow > 1 || this.wantsMouseEnterLeaveWindow);
			}
			else
			{
				result = this.wantsMouseMove;
			}
			return result;
		}

		public bool WantsLayoutPass(EventType type)
		{
			bool flag = !this.wantsLessLayoutEvents;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				switch (type)
				{
				case EventType.MouseDown:
				case EventType.MouseUp:
					return this.wantsMouseMove;
				case EventType.MouseMove:
				case EventType.MouseDrag:
				case EventType.ScrollWheel:
					goto IL_6C;
				case EventType.KeyDown:
				case EventType.KeyUp:
					return GUIUtility.textFieldInput;
				case EventType.Repaint:
					break;
				default:
					if (type != EventType.ExecuteCommand)
					{
						if (type - EventType.MouseEnterWindow > 1)
						{
							goto IL_6C;
						}
						return this.wantsMouseEnterLeaveWindow;
					}
					break;
				}
				return true;
				IL_6C:
				result = false;
			}
			return result;
		}
	}
}
