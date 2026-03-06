using System;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;

namespace UnityEngine.InputSystem.OnScreen
{
	[AddComponentMenu("Input/On-Screen Button")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.inputsystem@1.14/manual/OnScreen.html#on-screen-buttons")]
	public class OnScreenButton : OnScreenControl, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		public void OnPointerUp(PointerEventData eventData)
		{
			base.SendValueToControl<float>(0f);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			base.SendValueToControl<float>(1f);
		}

		protected override string controlPathInternal
		{
			get
			{
				return this.m_ControlPath;
			}
			set
			{
				this.m_ControlPath = value;
			}
		}

		[InputControl(layout = "Button")]
		[SerializeField]
		private string m_ControlPath;
	}
}
