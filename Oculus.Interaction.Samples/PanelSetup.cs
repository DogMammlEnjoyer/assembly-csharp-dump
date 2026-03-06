using System;
using Oculus.Interaction.Surfaces;
using UnityEngine;
using UnityEngine.Animations;

public class PanelSetup : MonoBehaviour
{
	[ContextMenu("Update Panel")]
	public void UpdatePanelProperties()
	{
		Debug.Log("Update Function");
		Vector2 vector = this.panelTransform.sizeDelta * this.panelTransform.lossyScale;
		Vector3[] rectCorners = this.GetRectCorners(Vector3.zero, vector);
		float d = this.InteractableLength * 0.5f;
		this.AnchorTopLeft.localPosition = rectCorners[0] + d * this.Vec2Sign(rectCorners[0]);
		this.AnchorTopRight.localPosition = rectCorners[1] + d * this.Vec2Sign(rectCorners[1]);
		this.AnchorBottomLeft.localPosition = rectCorners[2] + d * this.Vec2Sign(rectCorners[2]);
		this.AnchorBottomRight.localPosition = rectCorners[3] + d * this.Vec2Sign(rectCorners[3]);
		Vector3 size = new Vector3(this.InteractableLength, this.InteractableLength, this.InteractableDepth);
		this.SetColliderSize(this.ScalerTopLeft, size);
		this.SetColliderSize(this.ScalerTopRight, size);
		this.SetColliderSize(this.ScalerBottomLeft, size);
		this.SetColliderSize(this.ScalerBottomRight, size);
		if (this.AddVerticalRotation)
		{
			this.SetColliderSize(this.RotatorVerticalTop, size);
			this.SetColliderSize(this.RotatorVerticalBottom, size);
			this.RotatorVerticalTop.gameObject.SetActive(true);
			this.RotatorVerticalBottom.gameObject.SetActive(true);
		}
		else
		{
			this.RotatorVerticalTop.gameObject.SetActive(false);
			this.RotatorVerticalBottom.gameObject.SetActive(false);
		}
		if (this.AddHorizontalRotation)
		{
			this.SetColliderSize(this.RotatorHorizontalLeft, size);
			this.SetColliderSize(this.RotatorHorizontalRight, size);
			this.RotatorHorizontalLeft.gameObject.SetActive(true);
			this.RotatorHorizontalRight.gameObject.SetActive(true);
		}
		else
		{
			this.RotatorHorizontalLeft.gameObject.SetActive(false);
			this.RotatorHorizontalRight.gameObject.SetActive(false);
		}
		Vector3[] rectSides = this.GetRectSides(Vector3.zero, vector);
		if (this.AddVerticalRotation)
		{
			this.CreateCollider("ColliderUpLeft", vector, rectSides[0], Vector3.up, Vector3.left, false, 0, 1, this.RotatorVerticalTop.transform, this.ScalerTopLeft.transform);
			this.CreateCollider("ColliderUpRight", vector, rectSides[0], Vector3.up, Vector3.right, false, 0, 1, this.RotatorVerticalTop.transform, this.ScalerTopRight.transform);
			this.CreateCollider("ColliderDownLeft", vector, rectSides[1], Vector3.down, Vector3.left, false, 0, 1, this.RotatorVerticalBottom.transform, this.ScalerBottomLeft.transform);
			this.CreateCollider("ColliderDownRight", vector, rectSides[1], Vector3.down, Vector3.right, false, 0, 1, this.RotatorVerticalBottom.transform, this.ScalerBottomRight.transform);
		}
		else
		{
			this.CreateCollider("ColliderUp", vector, rectSides[0], Vector3.up, Vector3.zero, true, 0, 1, this.ScalerTopLeft.transform, this.ScalerTopRight.transform);
			this.CreateCollider("ColliderDown", vector, rectSides[1], Vector3.down, Vector3.zero, true, 0, 1, this.ScalerBottomLeft.transform, this.ScalerBottomRight.transform);
		}
		if (this.AddHorizontalRotation)
		{
			this.CreateCollider("ColliderLeftUp", vector, rectSides[2], Vector3.left, Vector3.up, false, 1, 0, this.RotatorHorizontalLeft.transform, this.ScalerTopLeft.transform);
			this.CreateCollider("ColliderLeftDown", vector, rectSides[2], Vector3.left, Vector3.down, false, 1, 0, this.RotatorHorizontalLeft.transform, this.ScalerBottomLeft.transform);
			this.CreateCollider("ColliderRightUp", vector, rectSides[3], Vector3.right, Vector3.up, false, 1, 0, this.RotatorHorizontalRight.transform, this.ScalerTopRight.transform);
			this.CreateCollider("ColliderRightDown", vector, rectSides[3], Vector3.right, Vector3.down, false, 1, 0, this.RotatorHorizontalRight.transform, this.ScalerBottomRight.transform);
		}
		else
		{
			this.CreateCollider("ColliderLeft", vector, rectSides[2], Vector3.left, Vector3.zero, true, 1, 0, this.ScalerBottomLeft.transform, this.ScalerTopLeft.transform);
			this.CreateCollider("ColliderRight", vector, rectSides[3], Vector3.right, Vector3.zero, true, 1, 0, this.ScalerBottomRight.transform, this.ScalerTopRight.transform);
		}
		this.boundsClipper.Size = new Vector3(vector.x, vector.y, this.InteractableDepth);
		this.topLeftCornerAnchor.localPosition = new Vector3(-vector.x * 0.5f, vector.y * 0.5f, 0f);
	}

	private void CreateCollider(string name, Vector2 rectSize, Vector3 sidePosition, Vector3 sideDirection, Vector3 offsetDirection, bool fullSize, int wideAxis, int normalAxis, Transform anchorA, Transform anchorB)
	{
		float num = this.InteractableLength * 0.5f;
		GameObject gameObject = new GameObject(name);
		gameObject.transform.SetParent(this.PanelInteractable.transform, false);
		gameObject.AddComponent<BoxCollider>().isTrigger = true;
		gameObject.AddComponent<BoundsClipper>();
		PositionConstraint positionConstraint = gameObject.AddComponent<PositionConstraint>();
		positionConstraint.AddSource(new ConstraintSource
		{
			sourceTransform = anchorA,
			weight = 1f
		});
		positionConstraint.AddSource(new ConstraintSource
		{
			sourceTransform = anchorB,
			weight = 1f
		});
		positionConstraint.constraintActive = true;
		gameObject.transform.localPosition = sidePosition + sideDirection * num;
		float num2 = fullSize ? rectSize[wideAxis] : (Mathf.Abs(rectSize[wideAxis] - this.InteractableLength) / 2f);
		gameObject.transform.localPosition += offsetDirection * (num + num2 * 0.5f);
		Vector3 vector = new Vector3(0f, 0f, this.InteractableDepth);
		vector[wideAxis] = num2;
		vector[normalAxis] = this.InteractableLength;
		ColliderSizeConstraint colliderSizeConstraint = gameObject.AddComponent<ColliderSizeConstraint>();
		colliderSizeConstraint.pointA = anchorA;
		colliderSizeConstraint.pointB = anchorB;
		colliderSizeConstraint.size = vector;
		colliderSizeConstraint.wideSideOffset = this.InteractableLength;
		colliderSizeConstraint.expandingAxis = wideAxis;
		gameObject.transform.localScale = vector;
	}

	private void SetColliderSize(GameObject colliderGO, Vector3 size)
	{
		if (colliderGO == null)
		{
			return;
		}
		Vector3 lossyScale = colliderGO.transform.lossyScale;
		Vector3 size2 = new Vector3(size.x / lossyScale.x, size.y / lossyScale.y, size.z / lossyScale.z);
		BoxCollider component = colliderGO.GetComponent<BoxCollider>();
		if (component != null)
		{
			component.size = size2;
		}
		BoundsClipper component2 = colliderGO.GetComponent<BoundsClipper>();
		if (component2 != null)
		{
			component2.Size = size2;
		}
	}

	private Vector2 Vec2Sign(Vector2 value)
	{
		return new Vector2(Mathf.Sign(value.x), Mathf.Sign(value.y));
	}

	private Vector3[] GetRectCorners(Vector3 position, Vector2 size)
	{
		return new Vector3[]
		{
			position + new Vector3(-size.x * 0.5f, size.y * 0.5f, 0f),
			position + new Vector3(size.x * 0.5f, size.y * 0.5f, 0f),
			position + new Vector3(-size.x * 0.5f, -size.y * 0.5f, 0f),
			position + new Vector3(size.x * 0.5f, -size.y * 0.5f, 0f)
		};
	}

	private Vector3[] GetRectSides(Vector3 position, Vector2 size)
	{
		return new Vector3[]
		{
			position + new Vector3(0f, size.y * 0.5f, 0f),
			position + new Vector3(0f, -size.y * 0.5f, 0f),
			position + new Vector3(-size.x * 0.5f, 0f, 0f),
			position + new Vector3(size.x * 0.5f, 0f, 0f)
		};
	}

	public float InteractableLength;

	public float InteractableDepth;

	public bool AddVerticalRotation;

	public bool AddHorizontalRotation;

	public RectTransform panelTransform;

	public UnionClippedPlaneSurface panelClippedPlaneSurface;

	public BoundsClipper boundsClipper;

	public Transform topLeftCornerAnchor;

	[Header("Anchors")]
	public Transform AnchorTopLeft;

	public Transform AnchorTopRight;

	public Transform AnchorBottomLeft;

	public Transform AnchorBottomRight;

	[Header("SideCollider")]
	public GameObject PanelInteractable;

	[Header("Scaler")]
	public GameObject ScalerTopLeft;

	public GameObject ScalerTopRight;

	public GameObject ScalerBottomLeft;

	public GameObject ScalerBottomRight;

	[Header("Rotator")]
	public GameObject RotatorVerticalTop;

	public GameObject RotatorVerticalBottom;

	public GameObject RotatorHorizontalLeft;

	public GameObject RotatorHorizontalRight;
}
