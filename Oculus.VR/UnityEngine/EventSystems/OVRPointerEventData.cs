using System;
using System.Text;

namespace UnityEngine.EventSystems
{
	[HelpURL("https://developer.oculus.com/documentation/unity/unity-isdk-pointer-events/")]
	public class OVRPointerEventData : PointerEventData
	{
		public OVRPointerEventData(EventSystem eventSystem) : base(eventSystem)
		{
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<b>Position</b>: " + base.position.ToString());
			stringBuilder.AppendLine("<b>delta</b>: " + base.delta.ToString());
			stringBuilder.AppendLine("<b>eligibleForClick</b>: " + base.eligibleForClick.ToString());
			string str = "<b>pointerEnter</b>: ";
			GameObject pointerEnter = base.pointerEnter;
			stringBuilder.AppendLine(str + ((pointerEnter != null) ? pointerEnter.ToString() : null));
			string str2 = "<b>pointerPress</b>: ";
			GameObject pointerPress = base.pointerPress;
			stringBuilder.AppendLine(str2 + ((pointerPress != null) ? pointerPress.ToString() : null));
			string str3 = "<b>lastPointerPress</b>: ";
			GameObject lastPress = base.lastPress;
			stringBuilder.AppendLine(str3 + ((lastPress != null) ? lastPress.ToString() : null));
			string str4 = "<b>pointerDrag</b>: ";
			GameObject pointerDrag = base.pointerDrag;
			stringBuilder.AppendLine(str4 + ((pointerDrag != null) ? pointerDrag.ToString() : null));
			string str5 = "<b>worldSpaceRay</b>: ";
			Ray ray = this.worldSpaceRay;
			stringBuilder.AppendLine(str5 + ray.ToString());
			string str6 = "<b>swipeStart</b>: ";
			Vector2 vector = this.swipeStart;
			stringBuilder.AppendLine(str6 + vector.ToString());
			stringBuilder.AppendLine("<b>Use Drag Threshold</b>: " + base.useDragThreshold.ToString());
			return stringBuilder.ToString();
		}

		public Ray worldSpaceRay;

		public Vector2 swipeStart;
	}
}
