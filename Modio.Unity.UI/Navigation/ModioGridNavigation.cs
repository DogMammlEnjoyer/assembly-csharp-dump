using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Modio.Unity.UI.Navigation
{
	public class ModioGridNavigation : Selectable, ILayoutController
	{
		protected override void OnEnable()
		{
			base.OnEnable();
			this._lastSelectedGameObject = null;
			LayoutRebuilder.MarkLayoutForRebuild((RectTransform)base.transform);
			this._needsDelayedNavigationCorrection = true;
		}

		public void SetLayoutHorizontal()
		{
		}

		public void SetLayoutVertical()
		{
			this._needsDelayedNavigationCorrection = true;
		}

		private void LateUpdate()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			if (this._needsDelayedNavigationCorrection)
			{
				this.RecalculateNavigation();
				this._needsDelayedNavigationCorrection = false;
			}
			if (this._selectChildImmediately && EventSystem.current.currentSelectedGameObject != base.gameObject)
			{
				this._selectChildImmediately = false;
			}
			if (this._selectChildImmediately)
			{
				this._selectChildImmediately = false;
				Vector3 position = new Vector3(-10000f, 10000f, 0f);
				if (this._lastSelectedGameObject != null)
				{
					position = this._lastSelectedGameObject.transform.position;
				}
				float num = float.MaxValue;
				Selectable selectable = null;
				foreach (object obj in base.transform)
				{
					Transform transform = (Transform)obj;
					if (transform.gameObject.activeSelf)
					{
						if (this._getSelectablesInChildrensChildren)
						{
							transform.GetComponentsInChildren<Selectable>(ModioGridNavigation.ReusedSelectables);
						}
						else
						{
							ModioGridNavigation.ReusedSelectables.Clear();
							Selectable component = transform.GetComponent<Selectable>();
							if (component != null)
							{
								ModioGridNavigation.ReusedSelectables.Add(component);
							}
						}
						foreach (Selectable selectable2 in ModioGridNavigation.ReusedSelectables)
						{
							if (!(selectable2 == this) && selectable2.interactable && selectable2.navigation.mode != Navigation.Mode.None)
							{
								RectTransform rectTransform = selectable2.transform as RectTransform;
								float num2 = float.MaxValue;
								if (rectTransform != null)
								{
									rectTransform.GetWorldCorners(ModioGridNavigation.TransCorners);
									Vector3[] transCorners = ModioGridNavigation.TransCorners;
									for (int i = 0; i < transCorners.Length; i++)
									{
										float sqrMagnitude = (transCorners[i] - position).sqrMagnitude;
										num2 = Mathf.Min(num2, sqrMagnitude);
									}
								}
								else
								{
									num2 = (selectable2.transform.position - position).sqrMagnitude;
								}
								if (num2 <= num)
								{
									num = num2;
									selectable = selectable2;
								}
							}
						}
					}
				}
				if (selectable != null)
				{
					EventSystem.current.SetSelectedGameObject(selectable.gameObject);
				}
				else if (this._fallbackSelectionToIfNoValidChildren != null)
				{
					EventSystem.current.SetSelectedGameObject(this._fallbackSelectionToIfNoValidChildren);
				}
				else
				{
					EventSystem.current.SetSelectedGameObject(this._lastSelectedGameObject);
				}
			}
			GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
			if (currentSelectedGameObject != null && currentSelectedGameObject.activeInHierarchy)
			{
				this._lastSelectedGameObject = currentSelectedGameObject;
			}
		}

		public override void OnSelect(BaseEventData eventData)
		{
			this._selectChildImmediately = true;
		}

		private void RecalculateNavigation()
		{
			Selectable selectable = null;
			Selectable selectable2 = null;
			bool flag = true;
			int num = 0;
			ModioGridNavigation.PrevRow.Clear();
			foreach (object obj in base.transform)
			{
				Transform transform = (Transform)obj;
				if (transform.gameObject.activeSelf)
				{
					if (this._getSelectablesInChildrensChildren)
					{
						transform.GetComponentsInChildren<Selectable>(ModioGridNavigation.ReusedSelectables);
					}
					else
					{
						ModioGridNavigation.ReusedSelectables.Clear();
						Selectable component = transform.GetComponent<Selectable>();
						if (component != null)
						{
							ModioGridNavigation.ReusedSelectables.Add(component);
						}
					}
					foreach (Selectable selectable3 in ModioGridNavigation.ReusedSelectables)
					{
						if (!(selectable3 == this) && selectable3.interactable && selectable3.navigation.mode != Navigation.Mode.None)
						{
							Navigation navigation = selectable3.navigation;
							navigation.mode = Navigation.Mode.Explicit;
							bool flag2 = false;
							if (selectable != null)
							{
								RectTransform rectTransform = selectable.transform as RectTransform;
								if (rectTransform == null)
								{
									goto IL_117;
								}
								RectTransform rectTransform2 = selectable3.transform as RectTransform;
								if (rectTransform2 == null)
								{
									goto IL_117;
								}
								flag2 = ModioGridNavigation.IsToTheRight(rectTransform, rectTransform2);
								IL_142:
								flag = (flag && flag2);
								goto IL_147;
								IL_117:
								flag2 = (selectable.transform.position.x + 1f < selectable3.transform.position.x);
								goto IL_142;
							}
							IL_147:
							if (flag2)
							{
								num++;
								navigation.selectOnLeft = selectable;
								if (selectable != null)
								{
									Navigation navigation2 = selectable.navigation;
									navigation2.selectOnRight = selectable3;
									selectable.navigation = navigation2;
								}
							}
							else
							{
								while (ModioGridNavigation.PrevRow.Count > num && ModioGridNavigation.PrevRow.Count > 0)
								{
									Selectable selectable4 = ModioGridNavigation.PrevRow.Dequeue();
									Navigation navigation3 = selectable4.navigation;
									navigation3.selectOnDown = selectable;
									selectable4.navigation = navigation3;
								}
								num = 1;
								navigation.selectOnLeft = this.GetNeighbourInDir(MoveDirection.Left);
								if (selectable != null)
								{
									Navigation navigation4 = selectable.navigation;
									navigation4.selectOnRight = this.GetNeighbourInDir(MoveDirection.Right);
									selectable.navigation = navigation4;
								}
								selectable2 = selectable;
							}
							if (flag)
							{
								navigation.selectOnUp = this.GetNeighbourInDir(MoveDirection.Up);
							}
							else
							{
								Selectable selectable5 = selectable2;
								if (ModioGridNavigation.PrevRow.Count >= num)
								{
									selectable5 = ModioGridNavigation.PrevRow.Dequeue();
									Navigation navigation5 = selectable5.navigation;
									navigation5.selectOnDown = selectable3;
									selectable5.navigation = navigation5;
								}
								navigation.selectOnUp = selectable5;
							}
							selectable3.navigation = navigation;
							selectable = selectable3;
							ModioGridNavigation.PrevRow.Enqueue(selectable);
						}
					}
				}
			}
			int num2 = ModioGridNavigation.PrevRow.Count - num;
			foreach (Selectable selectable6 in ModioGridNavigation.PrevRow)
			{
				Navigation navigation6 = selectable6.navigation;
				if (num2-- > 0)
				{
					navigation6.selectOnDown = selectable;
				}
				else
				{
					navigation6.selectOnDown = this.GetNeighbourInDir(MoveDirection.Down);
				}
				selectable6.navigation = navigation6;
			}
			if (selectable != null)
			{
				Navigation navigation7 = selectable.navigation;
				navigation7.selectOnRight = this.GetNeighbourInDir(MoveDirection.Right);
				selectable.navigation = navigation7;
			}
		}

		public void NeedsNavigationCorrection()
		{
			this._needsDelayedNavigationCorrection = true;
		}

		private static bool IsToTheRight(RectTransform prevRectTransform, RectTransform rectTransform)
		{
			prevRectTransform.GetWorldCorners(ModioGridNavigation.PrevCorners);
			rectTransform.GetWorldCorners(ModioGridNavigation.TransCorners);
			Vector3 vector = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			Vector3[] array = ModioGridNavigation.PrevCorners;
			for (int i = 0; i < array.Length; i++)
			{
				vector = Vector3.Max(array[i], vector);
			}
			array = ModioGridNavigation.TransCorners;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].x < vector.x)
				{
					return false;
				}
			}
			return true;
		}

		private Selectable GetNeighbourInDir(MoveDirection moveDirection)
		{
			Selectable selectable = this;
			int num = 0;
			for (;;)
			{
				Selectable selectable2;
				switch (moveDirection)
				{
				case MoveDirection.Left:
					selectable2 = selectable.navigation.selectOnLeft;
					goto IL_72;
				case MoveDirection.Up:
					selectable2 = selectable.navigation.selectOnUp;
					goto IL_72;
				case MoveDirection.Right:
					selectable2 = selectable.navigation.selectOnRight;
					goto IL_72;
				case MoveDirection.Down:
					selectable2 = selectable.navigation.selectOnDown;
					goto IL_72;
				}
				break;
				IL_72:
				selectable = selectable2;
				if (!(selectable != null) || selectable.isActiveAndEnabled || num++ >= 10)
				{
					goto IL_91;
				}
			}
			throw new ArgumentOutOfRangeException("moveDirection", moveDirection, null);
			IL_91:
			if (selectable == this)
			{
				return null;
			}
			return selectable;
		}

		private static readonly Queue<Selectable> PrevRow = new Queue<Selectable>();

		[SerializeField]
		private bool _getSelectablesInChildrensChildren;

		[SerializeField]
		private GameObject _fallbackSelectionToIfNoValidChildren;

		private bool _selectChildImmediately;

		private bool _needsDelayedNavigationCorrection;

		private GameObject _lastSelectedGameObject;

		private static readonly List<Selectable> ReusedSelectables = new List<Selectable>();

		private static readonly Vector3[] PrevCorners = new Vector3[4];

		private static readonly Vector3[] TransCorners = new Vector3[4];
	}
}
