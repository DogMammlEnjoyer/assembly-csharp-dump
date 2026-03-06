using System;

namespace UnityEngine.UIElements
{
	internal class KeyboardTextEditorEventHandler : TextEditorEventHandler
	{
		public KeyboardTextEditorEventHandler(TextElement textElement, TextEditingUtilities editingUtilities) : base(textElement, editingUtilities)
		{
			editingUtilities.multiline = textElement.edition.multiline;
		}

		public override void HandleEventBubbleUp(EventBase evt)
		{
			base.HandleEventBubbleUp(evt);
			KeyDownEvent keyDownEvent = evt as KeyDownEvent;
			if (keyDownEvent == null)
			{
				ValidateCommandEvent validateCommandEvent = evt as ValidateCommandEvent;
				if (validateCommandEvent == null)
				{
					ExecuteCommandEvent executeCommandEvent = evt as ExecuteCommandEvent;
					if (executeCommandEvent == null)
					{
						FocusEvent focusEvent = evt as FocusEvent;
						if (focusEvent == null)
						{
							BlurEvent blurEvent = evt as BlurEvent;
							if (blurEvent == null)
							{
								NavigationMoveEvent navigationMoveEvent = evt as NavigationMoveEvent;
								if (navigationMoveEvent == null)
								{
									NavigationSubmitEvent navigationSubmitEvent = evt as NavigationSubmitEvent;
									if (navigationSubmitEvent == null)
									{
										NavigationCancelEvent navigationCancelEvent = evt as NavigationCancelEvent;
										if (navigationCancelEvent == null)
										{
											IMEEvent imeevent = evt as IMEEvent;
											if (imeevent != null)
											{
												this.OnIMEInput(imeevent);
											}
										}
										else
										{
											this.OnNavigationEvent<NavigationCancelEvent>(navigationCancelEvent);
										}
									}
									else
									{
										this.OnNavigationEvent<NavigationSubmitEvent>(navigationSubmitEvent);
									}
								}
								else
								{
									this.OnNavigationEvent<NavigationMoveEvent>(navigationMoveEvent);
								}
							}
							else
							{
								this.OnBlur(blurEvent);
							}
						}
						else
						{
							this.OnFocus(focusEvent);
						}
					}
					else
					{
						this.OnExecuteCommandEvent(executeCommandEvent);
					}
				}
				else
				{
					this.OnValidateCommandEvent(validateCommandEvent);
				}
			}
			else
			{
				this.OnKeyDown(keyDownEvent);
			}
		}

		private void OnFocus(FocusEvent _)
		{
			GUIUtility.imeCompositionMode = IMECompositionMode.On;
			this.textElement.edition.SaveValueAndText();
		}

		private void OnBlur(BlurEvent _)
		{
			GUIUtility.imeCompositionMode = IMECompositionMode.Auto;
		}

		private void OnIMEInput(IMEEvent _)
		{
			bool isCompositionActive = this.editingUtilities.isCompositionActive;
			bool flag = this.editingUtilities.UpdateImeState() || isCompositionActive != this.editingUtilities.isCompositionActive;
			if (flag)
			{
				this.UpdateLabel(true);
			}
		}

		private void OnKeyDown(KeyDownEvent evt)
		{
			bool flag = !this.textElement.hasFocus;
			if (!flag)
			{
				this.m_Changed = false;
				evt.GetEquivalentImguiEvent(this.m_ImguiEvent);
				bool generatePreview = false;
				bool flag2 = this.editingUtilities.HandleKeyEvent(this.m_ImguiEvent);
				if (flag2)
				{
					bool flag3 = this.textElement.text != this.editingUtilities.text;
					if (flag3)
					{
						this.m_Changed = true;
					}
					evt.StopPropagation();
				}
				else
				{
					char c = evt.character;
					bool flag4 = evt.actionKey && (!evt.altKey || c == '\0');
					if (flag4)
					{
						return;
					}
					bool flag5 = (evt.keyCode >= KeyCode.F1 && evt.keyCode <= KeyCode.F15) || (evt.keyCode >= KeyCode.F16 && evt.keyCode <= KeyCode.F24);
					if (flag5)
					{
						return;
					}
					bool flag6 = evt.altKey && c == '\0';
					if (flag6)
					{
						return;
					}
					bool flag7 = c == '\t' && evt.keyCode == KeyCode.None && evt.modifiers == EventModifiers.None;
					if (flag7)
					{
						return;
					}
					bool flag8 = evt.keyCode == KeyCode.Tab || (evt.keyCode == KeyCode.Tab && evt.character == '\t' && evt.modifiers == EventModifiers.Shift);
					if (flag8)
					{
						bool flag9 = !this.textElement.edition.multiline || evt.shiftKey;
						if (flag9)
						{
							bool flag10 = evt.ShouldSendNavigationMoveEvent();
							if (flag10)
							{
								this.textElement.focusController.FocusNextInDirection(this.textElement, evt.shiftKey ? VisualElementFocusChangeDirection.left : VisualElementFocusChangeDirection.right);
								evt.StopPropagation();
							}
							return;
						}
						bool flag11 = !evt.ShouldSendNavigationMoveEvent();
						if (flag11)
						{
							return;
						}
					}
					bool flag12 = !this.textElement.edition.multiline && (evt.keyCode == KeyCode.KeypadEnter || evt.keyCode == KeyCode.Return);
					if (flag12)
					{
						this.m_ShouldInvokeUpdateValue = true;
					}
					evt.StopPropagation();
					bool flag13 = this.textElement.edition.multiline ? (c == '\n' && evt.shiftKey) : ((c == '\n' || c == '\r' || c == '\n') && !evt.altKey);
					if (flag13)
					{
						this.ApplyTextIfNeeded();
						Action moveFocusToCompositeRoot = this.textElement.edition.MoveFocusToCompositeRoot;
						if (moveFocusToCompositeRoot != null)
						{
							moveFocusToCompositeRoot();
						}
						return;
					}
					bool flag14 = evt.keyCode == KeyCode.Escape;
					if (flag14)
					{
						this.textElement.edition.RestoreValueAndText();
						Action updateValueFromText = this.textElement.edition.UpdateValueFromText;
						if (updateValueFromText != null)
						{
							updateValueFromText();
						}
						Action moveFocusToCompositeRoot2 = this.textElement.edition.MoveFocusToCompositeRoot;
						if (moveFocusToCompositeRoot2 != null)
						{
							moveFocusToCompositeRoot2();
						}
					}
					bool flag15 = evt.keyCode == KeyCode.Tab;
					if (flag15)
					{
						c = '\t';
					}
					bool flag16 = !this.textElement.edition.AcceptCharacter(c);
					if (flag16)
					{
						this.ApplyTextIfNeeded();
						return;
					}
					bool flag17 = c >= ' ' || evt.keyCode == KeyCode.Tab || (this.textElement.edition.multiline && !evt.altKey && (c == '\n' || c == '\r' || c == '\n'));
					if (flag17)
					{
						this.m_Changed = this.editingUtilities.Insert(c);
					}
					else
					{
						bool isCompositionActive = this.editingUtilities.isCompositionActive;
						generatePreview = true;
						bool flag18 = this.editingUtilities.UpdateImeState() || isCompositionActive != this.editingUtilities.isCompositionActive;
						if (flag18)
						{
							this.m_Changed = true;
						}
					}
				}
				bool flag19 = this.m_Changed || this.m_ShouldInvokeUpdateValue;
				if (flag19)
				{
					this.UpdateLabel(generatePreview);
				}
				Action<bool> updateScrollOffset = this.textElement.edition.UpdateScrollOffset;
				if (updateScrollOffset != null)
				{
					updateScrollOffset(evt.keyCode == KeyCode.Backspace);
				}
			}
		}

		private void ApplyTextIfNeeded()
		{
			bool shouldInvokeUpdateValue = this.m_ShouldInvokeUpdateValue;
			if (shouldInvokeUpdateValue)
			{
				Action updateValueFromText = this.textElement.edition.UpdateValueFromText;
				if (updateValueFromText != null)
				{
					updateValueFromText();
				}
				this.m_ShouldInvokeUpdateValue = false;
			}
		}

		private void UpdateLabel(bool generatePreview)
		{
			string text = this.editingUtilities.text;
			bool flag = this.editingUtilities.UpdateImeState();
			bool flag2 = flag && this.editingUtilities.ShouldUpdateImeWindowPosition();
			if (flag2)
			{
				this.editingUtilities.SetImeWindowPosition(new Vector2(this.textElement.worldBound.x, this.textElement.worldBound.y));
			}
			string value = generatePreview ? this.editingUtilities.GeneratePreviewString(this.textElement.enableRichText) : this.editingUtilities.text;
			this.textElement.edition.UpdateText(value);
			bool flag3 = !this.textElement.edition.isDelayed || this.m_ShouldInvokeUpdateValue;
			if (flag3)
			{
				Action updateValueFromText = this.textElement.edition.UpdateValueFromText;
				if (updateValueFromText != null)
				{
					updateValueFromText();
				}
				this.m_ShouldInvokeUpdateValue = false;
			}
			bool flag4 = flag;
			if (flag4)
			{
				this.editingUtilities.text = text;
				this.editingUtilities.EnableCursorPreviewState();
			}
			this.textElement.uitkTextHandle.ComputeSettingsAndUpdate();
		}

		private void OnValidateCommandEvent(ValidateCommandEvent evt)
		{
			bool flag = !this.textElement.hasFocus;
			if (!flag)
			{
				string commandName = evt.commandName;
				string a = commandName;
				if (!(a == "Copy") && !(a == "SelectAll"))
				{
					if (!(a == "Cut"))
					{
						if (!(a == "Paste"))
						{
							if (!(a == "Delete"))
							{
								if (!(a == "UndoRedoPerformed"))
								{
								}
							}
						}
						else
						{
							bool flag2 = !this.editingUtilities.CanPaste();
							if (flag2)
							{
								return;
							}
						}
					}
					else
					{
						bool flag3 = !this.textElement.selection.HasSelection();
						if (flag3)
						{
							return;
						}
					}
					evt.StopPropagation();
				}
			}
		}

		private void OnExecuteCommandEvent(ExecuteCommandEvent evt)
		{
			bool flag = !this.textElement.hasFocus;
			if (!flag)
			{
				this.m_Changed = false;
				bool flag2 = false;
				string text = this.editingUtilities.text;
				string commandName = evt.commandName;
				string a = commandName;
				if (!(a == "OnLostFocus"))
				{
					if (!(a == "Cut"))
					{
						if (!(a == "Paste"))
						{
							if (a == "Delete")
							{
								this.editingUtilities.Cut();
								flag2 = true;
								evt.StopPropagation();
							}
						}
						else
						{
							this.editingUtilities.Paste();
							flag2 = true;
							evt.StopPropagation();
						}
					}
					else
					{
						this.editingUtilities.Cut();
						flag2 = true;
						evt.StopPropagation();
					}
					bool flag3 = flag2;
					if (flag3)
					{
						bool flag4 = text != this.editingUtilities.text;
						if (flag4)
						{
							this.m_Changed = true;
						}
						evt.StopPropagation();
					}
					bool changed = this.m_Changed;
					if (changed)
					{
						this.UpdateLabel(true);
					}
					Action<bool> updateScrollOffset = this.textElement.edition.UpdateScrollOffset;
					if (updateScrollOffset != null)
					{
						updateScrollOffset(false);
					}
				}
				else
				{
					evt.StopPropagation();
				}
			}
		}

		private void OnNavigationEvent<TEvent>(NavigationEventBase<TEvent> evt) where TEvent : NavigationEventBase<TEvent>, new()
		{
			bool flag = evt.deviceType == NavigationDeviceType.Keyboard || evt.deviceType == NavigationDeviceType.Unknown;
			if (flag)
			{
				evt.StopPropagation();
				this.textElement.focusController.IgnoreEvent(evt);
			}
		}

		private readonly Event m_ImguiEvent = new Event();

		internal bool m_Changed;

		internal bool m_ShouldInvokeUpdateValue;

		private const int k_LineFeed = 10;

		private const int k_Space = 32;
	}
}
