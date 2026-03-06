using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Bindings;
using UnityEngine.TextCore.Text;

namespace UnityEngine
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEngine.UIElementsModule"
	})]
	internal class TextEditingUtilities
	{
		private bool hasSelection
		{
			get
			{
				return this.m_TextSelectingUtility.hasSelection;
			}
		}

		private string SelectedText
		{
			get
			{
				return this.m_TextSelectingUtility.selectedText;
			}
		}

		private int m_iAltCursorPos
		{
			get
			{
				return this.m_TextSelectingUtility.iAltCursorPos;
			}
		}

		internal bool revealCursor
		{
			get
			{
				return this.m_TextSelectingUtility.revealCursor;
			}
			set
			{
				this.m_TextSelectingUtility.revealCursor = value;
			}
		}

		internal int stringCursorIndex
		{
			get
			{
				return this.textHandle.GetCorrespondingStringIndex(this.cursorIndex);
			}
			set
			{
				this.cursorIndex = this.textHandle.GetCorrespondingCodePointIndex(value);
			}
		}

		private int cursorIndex
		{
			get
			{
				return this.m_TextSelectingUtility.cursorIndex;
			}
			set
			{
				this.m_TextSelectingUtility.cursorIndex = value;
			}
		}

		private int cursorIndexNoValidation
		{
			get
			{
				return this.m_TextSelectingUtility.cursorIndexNoValidation;
			}
			set
			{
				this.m_TextSelectingUtility.cursorIndexNoValidation = value;
			}
		}

		private int selectIndexNoValidation
		{
			get
			{
				return this.m_TextSelectingUtility.selectIndexNoValidation;
			}
			set
			{
				this.m_TextSelectingUtility.selectIndexNoValidation = value;
			}
		}

		private int stringCursorIndexNoValidation
		{
			get
			{
				return this.textHandle.GetCorrespondingStringIndex(this.m_TextSelectingUtility.cursorIndexNoValidation);
			}
		}

		internal int stringSelectIndex
		{
			get
			{
				return this.textHandle.GetCorrespondingStringIndex(this.selectIndex);
			}
			set
			{
				this.selectIndex = this.textHandle.GetCorrespondingCodePointIndex(value);
			}
		}

		private int selectIndex
		{
			get
			{
				return this.m_TextSelectingUtility.selectIndex;
			}
			set
			{
				this.m_TextSelectingUtility.selectIndex = value;
			}
		}

		public string text
		{
			get
			{
				return this.m_Text;
			}
			set
			{
				bool flag = value == this.m_Text;
				if (!flag)
				{
					this.m_Text = (value ?? string.Empty);
					Action onTextChanged = this.OnTextChanged;
					if (onTextChanged != null)
					{
						onTextChanged();
					}
				}
			}
		}

		internal void SetTextWithoutNotify(string value)
		{
			this.m_Text = value;
		}

		public TextEditingUtilities(TextSelectingUtilities selectingUtilities, TextHandle textHandle, string text)
		{
			this.m_TextSelectingUtility = selectingUtilities;
			this.textHandle = textHandle;
			this.m_Text = text;
		}

		public bool UpdateImeState()
		{
			bool flag = GUIUtility.compositionString.Length > 0;
			if (flag)
			{
				bool flag2 = !this.isCompositionActive;
				if (flag2)
				{
					this.m_UpdateImeWindowPosition = true;
					this.ReplaceSelection(string.Empty);
				}
				this.isCompositionActive = true;
			}
			else
			{
				this.isCompositionActive = false;
			}
			return this.isCompositionActive;
		}

		public bool ShouldUpdateImeWindowPosition()
		{
			return this.m_UpdateImeWindowPosition;
		}

		public void SetImeWindowPosition(Vector2 worldPosition)
		{
			Vector2 cursorPositionFromStringIndexUsingCharacterHeight = this.textHandle.GetCursorPositionFromStringIndexUsingCharacterHeight(this.cursorIndex, true);
			GUIUtility.compositionCursorPos = worldPosition + cursorPositionFromStringIndexUsingCharacterHeight;
		}

		public string GeneratePreviewString(bool richText)
		{
			this.RestoreCursorState();
			string compositionString = GUIUtility.compositionString;
			bool flag = this.isCompositionActive;
			string result;
			if (flag)
			{
				result = (richText ? this.text.Insert(this.stringCursorIndex, "<u>" + compositionString + "</u>") : this.text.Insert(this.stringCursorIndex, compositionString));
			}
			else
			{
				result = this.text;
			}
			return result;
		}

		public void EnableCursorPreviewState()
		{
			bool flag = this.m_CursorIndexSavedState != -1;
			if (!flag)
			{
				this.m_CursorIndexSavedState = this.m_TextSelectingUtility.cursorIndexNoValidation;
				this.cursorIndexNoValidation = (this.selectIndexNoValidation = this.m_CursorIndexSavedState + GUIUtility.compositionString.Length);
			}
		}

		public void RestoreCursorState()
		{
			bool flag = this.m_CursorIndexSavedState == -1;
			if (!flag)
			{
				this.cursorIndex = (this.selectIndex = this.m_CursorIndexSavedState);
				this.m_CursorIndexSavedState = -1;
			}
		}

		[VisibleToOtherModules]
		internal bool HandleKeyEvent(Event e)
		{
			this.RestoreCursorState();
			this.InitKeyActions();
			EventModifiers modifiers = e.modifiers;
			e.modifiers &= ~EventModifiers.CapsLock;
			bool flag = TextEditingUtilities.s_KeyEditOps.ContainsKey(e);
			bool result;
			if (flag)
			{
				TextEditOp operation = TextEditingUtilities.s_KeyEditOps[e];
				this.PerformOperation(operation);
				e.modifiers = modifiers;
				result = true;
			}
			else
			{
				e.modifiers = modifiers;
				result = false;
			}
			return result;
		}

		private void PerformOperation(TextEditOp operation)
		{
			this.revealCursor = true;
			switch (operation)
			{
			case TextEditOp.MoveLeft:
				this.m_TextSelectingUtility.MoveLeft();
				return;
			case TextEditOp.MoveRight:
				this.m_TextSelectingUtility.MoveRight();
				return;
			case TextEditOp.MoveUp:
				this.m_TextSelectingUtility.MoveUp();
				return;
			case TextEditOp.MoveDown:
				this.m_TextSelectingUtility.MoveDown();
				return;
			case TextEditOp.MoveLineStart:
				this.m_TextSelectingUtility.MoveLineStart();
				return;
			case TextEditOp.MoveLineEnd:
				this.m_TextSelectingUtility.MoveLineEnd();
				return;
			case TextEditOp.MoveTextStart:
				this.m_TextSelectingUtility.MoveTextStart();
				return;
			case TextEditOp.MoveTextEnd:
				this.m_TextSelectingUtility.MoveTextEnd();
				return;
			case TextEditOp.MoveGraphicalLineStart:
				this.m_TextSelectingUtility.MoveGraphicalLineStart();
				return;
			case TextEditOp.MoveGraphicalLineEnd:
				this.m_TextSelectingUtility.MoveGraphicalLineEnd();
				return;
			case TextEditOp.MoveWordLeft:
				this.m_TextSelectingUtility.MoveWordLeft();
				return;
			case TextEditOp.MoveWordRight:
				this.m_TextSelectingUtility.MoveWordRight();
				return;
			case TextEditOp.MoveParagraphForward:
				this.m_TextSelectingUtility.MoveParagraphForward();
				return;
			case TextEditOp.MoveParagraphBackward:
				this.m_TextSelectingUtility.MoveParagraphBackward();
				return;
			case TextEditOp.MoveToStartOfNextWord:
				this.m_TextSelectingUtility.MoveToStartOfNextWord();
				return;
			case TextEditOp.MoveToEndOfPreviousWord:
				this.m_TextSelectingUtility.MoveToEndOfPreviousWord();
				return;
			case TextEditOp.Delete:
				this.Delete();
				return;
			case TextEditOp.Backspace:
				this.Backspace();
				return;
			case TextEditOp.DeleteWordBack:
				this.DeleteWordBack();
				return;
			case TextEditOp.DeleteWordForward:
				this.DeleteWordForward();
				return;
			case TextEditOp.DeleteLineBack:
				this.DeleteLineBack();
				return;
			case TextEditOp.Cut:
				this.Cut();
				return;
			case TextEditOp.Paste:
				this.Paste();
				return;
			}
			Debug.Log("Unimplemented: " + operation.ToString());
		}

		private static void MapKey(string key, TextEditOp action)
		{
			TextEditingUtilities.s_KeyEditOps[Event.KeyboardEvent(key)] = action;
		}

		private void InitKeyActions()
		{
			bool flag = TextEditingUtilities.s_KeyEditOps != null;
			if (!flag)
			{
				TextEditingUtilities.s_KeyEditOps = new Dictionary<Event, TextEditOp>();
				TextEditingUtilities.MapKey("left", TextEditOp.MoveLeft);
				TextEditingUtilities.MapKey("right", TextEditOp.MoveRight);
				TextEditingUtilities.MapKey("up", TextEditOp.MoveUp);
				TextEditingUtilities.MapKey("down", TextEditOp.MoveDown);
				TextEditingUtilities.MapKey("delete", TextEditOp.Delete);
				TextEditingUtilities.MapKey("backspace", TextEditOp.Backspace);
				TextEditingUtilities.MapKey("#backspace", TextEditOp.Backspace);
				bool flag2 = SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX;
				if (flag2)
				{
					TextEditingUtilities.MapKey("^left", TextEditOp.MoveGraphicalLineStart);
					TextEditingUtilities.MapKey("^right", TextEditOp.MoveGraphicalLineEnd);
					TextEditingUtilities.MapKey("&left", TextEditOp.MoveWordLeft);
					TextEditingUtilities.MapKey("&right", TextEditOp.MoveWordRight);
					TextEditingUtilities.MapKey("&up", TextEditOp.MoveParagraphBackward);
					TextEditingUtilities.MapKey("&down", TextEditOp.MoveParagraphForward);
					TextEditingUtilities.MapKey("%left", TextEditOp.MoveGraphicalLineStart);
					TextEditingUtilities.MapKey("%right", TextEditOp.MoveGraphicalLineEnd);
					TextEditingUtilities.MapKey("%up", TextEditOp.MoveTextStart);
					TextEditingUtilities.MapKey("%down", TextEditOp.MoveTextEnd);
					TextEditingUtilities.MapKey("%x", TextEditOp.Cut);
					TextEditingUtilities.MapKey("%v", TextEditOp.Paste);
					TextEditingUtilities.MapKey("^d", TextEditOp.Delete);
					TextEditingUtilities.MapKey("^h", TextEditOp.Backspace);
					TextEditingUtilities.MapKey("^b", TextEditOp.MoveLeft);
					TextEditingUtilities.MapKey("^f", TextEditOp.MoveRight);
					TextEditingUtilities.MapKey("^a", TextEditOp.MoveLineStart);
					TextEditingUtilities.MapKey("^e", TextEditOp.MoveLineEnd);
					TextEditingUtilities.MapKey("&delete", TextEditOp.DeleteWordForward);
					TextEditingUtilities.MapKey("&backspace", TextEditOp.DeleteWordBack);
					TextEditingUtilities.MapKey("%backspace", TextEditOp.DeleteLineBack);
				}
				else
				{
					TextEditingUtilities.MapKey("home", TextEditOp.MoveGraphicalLineStart);
					TextEditingUtilities.MapKey("end", TextEditOp.MoveGraphicalLineEnd);
					TextEditingUtilities.MapKey("%left", TextEditOp.MoveWordLeft);
					TextEditingUtilities.MapKey("%right", TextEditOp.MoveWordRight);
					TextEditingUtilities.MapKey("%up", TextEditOp.MoveParagraphBackward);
					TextEditingUtilities.MapKey("%down", TextEditOp.MoveParagraphForward);
					TextEditingUtilities.MapKey("^left", TextEditOp.MoveToEndOfPreviousWord);
					TextEditingUtilities.MapKey("^right", TextEditOp.MoveToStartOfNextWord);
					TextEditingUtilities.MapKey("^up", TextEditOp.MoveParagraphBackward);
					TextEditingUtilities.MapKey("^down", TextEditOp.MoveParagraphForward);
					TextEditingUtilities.MapKey("^delete", TextEditOp.DeleteWordForward);
					TextEditingUtilities.MapKey("^backspace", TextEditOp.DeleteWordBack);
					TextEditingUtilities.MapKey("%backspace", TextEditOp.DeleteLineBack);
					TextEditingUtilities.MapKey("^x", TextEditOp.Cut);
					TextEditingUtilities.MapKey("^v", TextEditOp.Paste);
					TextEditingUtilities.MapKey("#delete", TextEditOp.Cut);
					TextEditingUtilities.MapKey("#insert", TextEditOp.Paste);
				}
			}
		}

		public bool DeleteLineBack()
		{
			this.RestoreCursorState();
			bool hasSelection = this.hasSelection;
			bool result;
			if (hasSelection)
			{
				this.DeleteSelection();
				result = true;
			}
			else
			{
				bool useAdvancedText = this.textHandle.useAdvancedText;
				if (useAdvancedText)
				{
					int firstCharacterIndexOnLine = this.textHandle.GetFirstCharacterIndexOnLine(this.cursorIndex);
					bool flag = firstCharacterIndexOnLine != this.cursorIndex;
					if (flag)
					{
						this.text = this.text.Remove(firstCharacterIndexOnLine, this.stringCursorIndex - firstCharacterIndexOnLine);
						this.cursorIndex = (this.selectIndex = firstCharacterIndexOnLine);
						result = true;
					}
					else
					{
						result = false;
					}
				}
				else
				{
					LineInfo lineInfoFromCharacterIndex = this.textHandle.GetLineInfoFromCharacterIndex(this.cursorIndex);
					int firstCharacterIndex = lineInfoFromCharacterIndex.firstCharacterIndex;
					int correspondingStringIndex = this.textHandle.GetCorrespondingStringIndex(firstCharacterIndex);
					bool flag2 = firstCharacterIndex != this.cursorIndex;
					if (flag2)
					{
						this.text = this.text.Remove(correspondingStringIndex, this.stringCursorIndex - correspondingStringIndex);
						this.cursorIndex = (this.selectIndex = firstCharacterIndex);
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		public bool DeleteWordBack()
		{
			this.RestoreCursorState();
			bool hasSelection = this.hasSelection;
			bool result;
			if (hasSelection)
			{
				this.DeleteSelection();
				result = true;
			}
			else
			{
				int num = this.m_TextSelectingUtility.FindEndOfPreviousWord(this.cursorIndex);
				bool flag = this.cursorIndex != num;
				if (flag)
				{
					int correspondingStringIndex = this.textHandle.GetCorrespondingStringIndex(num);
					this.text = this.text.Remove(correspondingStringIndex, this.stringCursorIndex - correspondingStringIndex);
					this.selectIndex = (this.cursorIndex = num);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public bool DeleteWordForward()
		{
			this.RestoreCursorState();
			bool hasSelection = this.hasSelection;
			bool result;
			if (hasSelection)
			{
				this.DeleteSelection();
				result = true;
			}
			else
			{
				int index = this.m_TextSelectingUtility.FindStartOfNextWord(this.cursorIndex);
				bool flag = this.cursorIndex < this.text.Length;
				if (flag)
				{
					int correspondingStringIndex = this.textHandle.GetCorrespondingStringIndex(index);
					this.text = this.text.Remove(this.stringCursorIndex, correspondingStringIndex - this.stringCursorIndex);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public bool Delete()
		{
			this.RestoreCursorState();
			bool hasSelection = this.hasSelection;
			bool result;
			if (hasSelection)
			{
				this.DeleteSelection();
				result = true;
			}
			else
			{
				bool flag = this.stringCursorIndex < this.text.Length;
				if (flag)
				{
					bool useAdvancedText = this.textHandle.useAdvancedText;
					int count;
					if (useAdvancedText)
					{
						count = Mathf.Abs(this.textHandle.NextCodePointIndex(this.cursorIndex) - this.cursorIndex);
					}
					else
					{
						count = this.textHandle.textInfo.textElementInfo[this.cursorIndex].stringLength;
					}
					this.text = this.text.Remove(this.stringCursorIndex, count);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public bool Backspace()
		{
			this.RestoreCursorState();
			bool hasSelection = this.hasSelection;
			bool result;
			if (hasSelection)
			{
				this.DeleteSelection();
				result = true;
			}
			else
			{
				bool flag = this.cursorIndex > 0;
				if (flag)
				{
					int num = this.m_TextSelectingUtility.PreviousCodePointIndex(this.cursorIndex);
					bool useAdvancedText = this.textHandle.useAdvancedText;
					int num2;
					if (useAdvancedText)
					{
						num2 = Mathf.Abs(this.cursorIndex - num);
					}
					else
					{
						num2 = this.textHandle.textInfo.textElementInfo[this.cursorIndex - 1].stringLength;
					}
					this.text = this.text.Remove(this.stringCursorIndex - num2, num2);
					this.cursorIndex = (this.textHandle.useAdvancedText ? Math.Max(0, this.cursorIndex - num2) : num);
					this.selectIndex = (this.textHandle.useAdvancedText ? Math.Max(0, this.selectIndex - num2) : num);
					this.m_TextSelectingUtility.ClearCursorPos();
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public bool DeleteSelection()
		{
			bool flag = this.cursorIndex == this.selectIndex;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this.cursorIndex < this.selectIndex;
				if (flag2)
				{
					this.text = this.text.Substring(0, this.stringCursorIndex) + this.text.Substring(this.stringSelectIndex, this.text.Length - this.stringSelectIndex);
					this.selectIndex = this.cursorIndex;
				}
				else
				{
					this.text = this.text.Substring(0, this.stringSelectIndex) + this.text.Substring(this.stringCursorIndex, this.text.Length - this.stringCursorIndex);
					this.cursorIndex = this.selectIndex;
				}
				this.m_TextSelectingUtility.ClearCursorPos();
				result = true;
			}
			return result;
		}

		public void ReplaceSelection(string replace)
		{
			this.RestoreCursorState();
			this.DeleteSelection();
			this.text = this.text.Insert(this.stringCursorIndex, replace);
			int num = this.textHandle.useAdvancedText ? replace.Length : new StringInfo(replace).LengthInTextElements;
			int num2 = this.cursorIndexNoValidation + num;
			this.cursorIndexNoValidation = num2;
			this.selectIndexNoValidation = num2;
			this.m_TextSelectingUtility.ClearCursorPos();
		}

		public bool Insert(char c)
		{
			bool flag = char.IsHighSurrogate(c);
			bool result;
			if (flag)
			{
				this.m_HighSurrogate = c;
				result = false;
			}
			else
			{
				bool flag2 = char.IsLowSurrogate(c);
				if (flag2)
				{
					char c2 = c;
					string text = new string(new char[]
					{
						this.m_HighSurrogate,
						c2
					});
					this.ReplaceSelection(text.ToString());
					result = true;
				}
				else
				{
					this.ReplaceSelection(c.ToString());
					result = true;
				}
			}
			return result;
		}

		public void MoveSelectionToAltCursor()
		{
			this.RestoreCursorState();
			bool flag = this.m_iAltCursorPos == -1;
			if (!flag)
			{
				int iAltCursorPos = this.m_iAltCursorPos;
				string selectedText = this.SelectedText;
				this.text = this.text.Insert(iAltCursorPos, selectedText);
				bool flag2 = iAltCursorPos < this.cursorIndex;
				if (flag2)
				{
					this.cursorIndex += selectedText.Length;
					this.selectIndex += selectedText.Length;
				}
				this.DeleteSelection();
				this.selectIndex = (this.cursorIndex = iAltCursorPos);
				this.m_TextSelectingUtility.ClearCursorPos();
			}
		}

		public bool CanPaste()
		{
			return GUIUtility.systemCopyBuffer.Length != 0;
		}

		public bool Cut()
		{
			this.m_TextSelectingUtility.Copy();
			return this.DeleteSelection();
		}

		public bool Paste()
		{
			this.RestoreCursorState();
			string text = GUIUtility.systemCopyBuffer;
			bool flag = text != "";
			bool result;
			if (flag)
			{
				bool flag2 = !this.multiline;
				if (flag2)
				{
					text = TextEditingUtilities.ReplaceNewlinesWithSpaces(text);
				}
				this.ReplaceSelection(text);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private static string ReplaceNewlinesWithSpaces(string value)
		{
			value = value.Replace("\r\n", " ");
			value = value.Replace('\n', ' ');
			value = value.Replace('\r', ' ');
			return value;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEngine.UIElementsModule"
		})]
		internal void OnBlur()
		{
			this.revealCursor = false;
			this.isCompositionActive = false;
			this.RestoreCursorState();
			this.m_TextSelectingUtility.SelectNone();
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEngine.UIElementsModule"
		})]
		internal bool TouchScreenKeyboardShouldBeUsed()
		{
			RuntimePlatform platform = Application.platform;
			RuntimePlatform runtimePlatform = platform;
			RuntimePlatform runtimePlatform2 = runtimePlatform;
			bool result;
			if (runtimePlatform2 != RuntimePlatform.Android && runtimePlatform2 - RuntimePlatform.WebGLPlayer > 3)
			{
				result = TouchScreenKeyboard.isSupported;
			}
			else
			{
				result = !TouchScreenKeyboard.isInPlaceEditingAllowed;
			}
			return result;
		}

		private TextSelectingUtilities m_TextSelectingUtility;

		internal TextHandle textHandle;

		private int m_CursorIndexSavedState = -1;

		[VisibleToOtherModules(new string[]
		{
			"UnityEngine.UIElementsModule"
		})]
		internal bool isCompositionActive;

		private bool m_UpdateImeWindowPosition;

		internal Action OnTextChanged;

		public bool multiline = false;

		private string m_Text;

		private static Dictionary<Event, TextEditOp> s_KeyEditOps;

		private char m_HighSurrogate;
	}
}
