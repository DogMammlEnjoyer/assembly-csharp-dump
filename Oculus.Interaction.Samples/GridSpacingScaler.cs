using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(GridLayoutGroup))]
public class GridSpacingScaler : MonoBehaviour
{
	private void Start()
	{
		this._rectTransform = (base.transform as RectTransform);
		if (this._rectTransform == null)
		{
			Debug.LogError("GameObject Transform is not a Rect Transform");
		}
		this._gridLayoutGroup = base.gameObject.GetComponent<GridLayoutGroup>();
		if (this._gridLayoutGroup == null)
		{
			Debug.LogError("GameObject does not include a GridLayoutGroup");
		}
	}

	private void LateUpdate()
	{
		float num = Mathf.Floor(this._rectTransform.rect.size[(int)this.scaleAxis]);
		float num2 = this.minSpacing[(int)this.scaleAxis];
		float num3 = this._gridLayoutGroup.cellSize[(int)this.scaleAxis];
		int num4 = (this.scaleAxis == GridSpacingScaler.Axis.Horizontal) ? this._gridLayoutGroup.padding.horizontal : this._gridLayoutGroup.padding.vertical;
		if (num3 + num2 <= 0f)
		{
			return;
		}
		int num5 = Mathf.Max(1, Mathf.FloorToInt((num - (float)num4 + num2 + 0.001f) / (num3 + num2)));
		float num6 = num - (float)num5 * num3;
		this._gridLayoutGroup.constraint = ((this.scaleAxis == GridSpacingScaler.Axis.Horizontal) ? GridLayoutGroup.Constraint.FixedColumnCount : GridLayoutGroup.Constraint.FixedRowCount);
		this._gridLayoutGroup.constraintCount = num5;
		if (num5 > 1)
		{
			float f = num6 / (float)(num5 - 1);
			Vector2 spacing = this._gridLayoutGroup.spacing;
			spacing[(int)this.scaleAxis] = Mathf.Floor(f);
			this._gridLayoutGroup.spacing = spacing;
			return;
		}
	}

	public GridSpacingScaler.Axis scaleAxis;

	public Vector2 minSpacing;

	private GridLayoutGroup _gridLayoutGroup;

	private RectTransform _rectTransform;

	public enum Axis
	{
		Horizontal,
		Vertical
	}
}
