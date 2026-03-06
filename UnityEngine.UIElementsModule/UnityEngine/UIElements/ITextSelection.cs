using System;

namespace UnityEngine.UIElements
{
	public interface ITextSelection
	{
		bool isSelectable { get; set; }

		[Obsolete("cursorColor is deprecated. Please use the corresponding USS property (--unity-cursor-color) instead.")]
		Color cursorColor { get; set; }

		[Obsolete("selectionColor is deprecated. Please use the corresponding USS property (--unity-selection-color) instead.")]
		Color selectionColor { get; set; }

		int cursorIndex { get; set; }

		bool doubleClickSelectsWord { get; set; }

		int selectIndex { get; set; }

		bool tripleClickSelectsLine { get; set; }

		bool HasSelection();

		void SelectAll();

		void SelectNone();

		void SelectRange(int cursorIndex, int selectionIndex);

		bool selectAllOnFocus { get; set; }

		bool selectAllOnMouseUp { get; set; }

		Vector2 cursorPosition { get; }

		float lineHeightAtCursorPosition { get; }

		float cursorWidth { get; set; }

		void MoveTextEnd();
	}
}
