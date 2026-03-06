using System;

namespace UnityEngine.UIElements
{
	internal static class DragAndDropUtility
	{
		internal static IDragAndDrop GetDragAndDrop(IPanel panel)
		{
			bool flag = panel.contextType == ContextType.Player;
			IDragAndDrop result;
			if (flag)
			{
				IDragAndDrop dragAndDrop;
				if ((dragAndDrop = DragAndDropUtility.s_DragAndDropPlayMode) == null)
				{
					dragAndDrop = (DragAndDropUtility.s_DragAndDropPlayMode = new DefaultDragAndDropClient());
				}
				result = dragAndDrop;
			}
			else
			{
				IDragAndDrop dragAndDrop2;
				if ((dragAndDrop2 = DragAndDropUtility.s_DragAndDropEditor) == null)
				{
					IDragAndDrop dragAndDrop4;
					if (DragAndDropUtility.s_MakeDragAndDropClientFunc == null)
					{
						IDragAndDrop dragAndDrop3 = new DefaultDragAndDropClient();
						dragAndDrop4 = dragAndDrop3;
					}
					else
					{
						dragAndDrop4 = DragAndDropUtility.s_MakeDragAndDropClientFunc();
					}
					dragAndDrop2 = (DragAndDropUtility.s_DragAndDropEditor = dragAndDrop4);
				}
				result = dragAndDrop2;
			}
			return result;
		}

		internal static void RegisterMakeClientFunc(Func<IDragAndDrop> makeClient)
		{
			DragAndDropUtility.s_MakeDragAndDropClientFunc = makeClient;
			DragAndDropUtility.s_DragAndDropEditor = null;
		}

		private static Func<IDragAndDrop> s_MakeDragAndDropClientFunc;

		private static IDragAndDrop s_DragAndDropEditor;

		private static IDragAndDrop s_DragAndDropPlayMode;
	}
}
