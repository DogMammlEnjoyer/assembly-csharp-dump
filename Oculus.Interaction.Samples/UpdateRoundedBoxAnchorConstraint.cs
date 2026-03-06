using System;
using UnityEngine;
using UnityEngine.Animations;

public class UpdateRoundedBoxAnchorConstraint : MonoBehaviour
{
	private static void UpdateOffset(PositionConstraint constraint, Vector2 direction, Vector2 offset, float interactableLength)
	{
		constraint.translationOffset = direction * offset + direction * interactableLength * 0.5f;
	}

	public static void UpdateAnchors(PositionConstraint topLeft, PositionConstraint topRight, PositionConstraint bottomLeft, PositionConstraint bottomRight, Vector2 offset, float interactableLength)
	{
		UpdateRoundedBoxAnchorConstraint.UpdateOffset(topLeft, new Vector2(1f, -1f), offset, interactableLength);
		UpdateRoundedBoxAnchorConstraint.UpdateOffset(topRight, new Vector2(-1f, -1f), offset, interactableLength);
		UpdateRoundedBoxAnchorConstraint.UpdateOffset(bottomLeft, new Vector2(1f, 1f), offset, interactableLength);
		UpdateRoundedBoxAnchorConstraint.UpdateOffset(bottomRight, new Vector2(-1f, 1f), offset, interactableLength);
	}

	[ContextMenu("Update Anchors")]
	public void UpdateAnchorsMenu()
	{
		UpdateRoundedBoxAnchorConstraint.UpdateAnchors(this._topLeft, this._topRight, this._bottomLeft, this._bottomRight, this._offset, this._interactableLength);
	}

	[SerializeField]
	private PositionConstraint _topLeft;

	[SerializeField]
	private PositionConstraint _topRight;

	[SerializeField]
	private PositionConstraint _bottomLeft;

	[SerializeField]
	private PositionConstraint _bottomRight;

	[SerializeField]
	private float _interactableLength;

	[SerializeField]
	private Vector2 _offset;
}
