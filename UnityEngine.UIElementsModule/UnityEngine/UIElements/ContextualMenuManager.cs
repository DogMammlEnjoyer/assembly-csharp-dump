using System;

namespace UnityEngine.UIElements
{
	public abstract class ContextualMenuManager
	{
		internal bool displayMenuHandledOSX { get; set; }

		public abstract void DisplayMenuIfEventMatches(EventBase evt, IEventHandler eventHandler);

		internal abstract bool CheckIfEventMatches(EventBase evt);

		public void DisplayMenu(EventBase triggerEvent, IEventHandler target)
		{
			DropdownMenu menu = new DropdownMenu();
			this.DisplayMenu(triggerEvent, target, menu);
		}

		internal void DisplayMenu(EventBase triggerEvent, IEventHandler target, DropdownMenu menu)
		{
			int pointerId;
			using (ContextualMenuPopulateEvent pooled = ContextualMenuPopulateEvent.GetPooled(triggerEvent, menu, target, this))
			{
				IPointerEvent pointerEvent = triggerEvent as IPointerEvent;
				pointerId = ((pointerEvent != null) ? pointerEvent.pointerId : PointerId.mousePointerId);
				int button = pooled.button;
				if (target != null)
				{
					target.SendEvent(pooled);
				}
			}
			bool isOSXContextualMenuPlatform = UIElementsUtility.isOSXContextualMenuPlatform;
			if (isOSXContextualMenuPlatform)
			{
				this.displayMenuHandledOSX = true;
				ContextualMenuManager.ResetPointerDown(pointerId);
			}
		}

		protected internal abstract void DoDisplayMenu(DropdownMenu menu, EventBase triggerEvent);

		internal static void ResetPointerDown(int pointerId)
		{
			PointerDeviceState.ReleaseAllButtons(pointerId);
		}
	}
}
