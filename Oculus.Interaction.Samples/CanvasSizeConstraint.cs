using System;
using UnityEngine;

public class CanvasSizeConstraint : MonoBehaviour
{
	private void Start()
	{
		this._rectTransform = base.GetComponent<RectTransform>();
		this._initialRectSize = this._rectTransform.sizeDelta;
		this._initialSize = new Vector2(Vector3.Distance(this.horizontalAnchorA.position, this.horizontalAnchorB.position) - this.horizontalSizeOffset, Vector3.Distance(this.verticalAnchorA.position, this.verticalAnchorB.position) - this.verticalSizeOffset);
		this._initialLocalScale = this._rectTransform.localScale;
	}

	private void Update()
	{
		Vector2 vector = new Vector2(Vector3.Distance(this.horizontalAnchorA.position, this.horizontalAnchorB.position) - this.horizontalSizeOffset, Vector3.Distance(this.verticalAnchorA.position, this.verticalAnchorB.position) - this.verticalSizeOffset);
		Vector2 vector2 = new Vector2(vector.x / this._initialSize.x, vector.y / this._initialSize.y);
		this._rectTransform.localScale = new Vector3(this._initialLocalScale.x / vector2.x, this._initialLocalScale.y / vector2.y, this._initialLocalScale.z);
		this._rectTransform.sizeDelta = new Vector2(this._initialRectSize.x * vector2.x, this._initialRectSize.y * vector2.y);
	}

	public Transform horizontalAnchorA;

	public Transform horizontalAnchorB;

	public Transform verticalAnchorA;

	public Transform verticalAnchorB;

	public float horizontalSizeOffset;

	public float verticalSizeOffset;

	private Vector2 _initialSize;

	private Vector2 _initialRectSize;

	private RectTransform _rectTransform;

	private Vector3 _initialLocalScale;
}
