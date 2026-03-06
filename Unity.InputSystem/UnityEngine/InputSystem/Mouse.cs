using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(stateType = typeof(MouseState), isGenericTypeOfDevice = true)]
	public class Mouse : Pointer, IInputStateCallbackReceiver
	{
		public DeltaControl scroll { get; protected set; }

		public ButtonControl leftButton { get; protected set; }

		public ButtonControl middleButton { get; protected set; }

		public ButtonControl rightButton { get; protected set; }

		public ButtonControl backButton { get; protected set; }

		public ButtonControl forwardButton { get; protected set; }

		public IntegerControl clickCount { get; protected set; }

		public new static Mouse current { get; private set; }

		public override void MakeCurrent()
		{
			base.MakeCurrent();
			Mouse.current = this;
		}

		protected override void OnAdded()
		{
			base.OnAdded();
			if (base.native && Mouse.s_PlatformMouseDevice == null)
			{
				Mouse.s_PlatformMouseDevice = this;
			}
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();
			if (Mouse.current == this)
			{
				Mouse.current = null;
			}
		}

		public void WarpCursorPosition(Vector2 position)
		{
			WarpMousePositionCommand warpMousePositionCommand = WarpMousePositionCommand.Create(position);
			base.ExecuteCommand<WarpMousePositionCommand>(ref warpMousePositionCommand);
		}

		protected override void FinishSetup()
		{
			this.scroll = base.GetChildControl<DeltaControl>("scroll");
			this.leftButton = base.GetChildControl<ButtonControl>("leftButton");
			this.middleButton = base.GetChildControl<ButtonControl>("middleButton");
			this.rightButton = base.GetChildControl<ButtonControl>("rightButton");
			this.forwardButton = base.GetChildControl<ButtonControl>("forwardButton");
			this.backButton = base.GetChildControl<ButtonControl>("backButton");
			base.displayIndex = base.GetChildControl<IntegerControl>("displayIndex");
			this.clickCount = base.GetChildControl<IntegerControl>("clickCount");
			base.FinishSetup();
		}

		protected new void OnNextUpdate()
		{
			base.OnNextUpdate();
			InputState.Change<Vector2>(this.scroll, Vector2.zero, InputUpdateType.None, default(InputEventPtr));
		}

		protected new void OnStateEvent(InputEventPtr eventPtr)
		{
			this.scroll.AccumulateValueInEvent(base.currentStatePtr, eventPtr);
			base.OnStateEvent(eventPtr);
		}

		void IInputStateCallbackReceiver.OnNextUpdate()
		{
			this.OnNextUpdate();
		}

		void IInputStateCallbackReceiver.OnStateEvent(InputEventPtr eventPtr)
		{
			this.OnStateEvent(eventPtr);
		}

		internal static Mouse s_PlatformMouseDevice;
	}
}
