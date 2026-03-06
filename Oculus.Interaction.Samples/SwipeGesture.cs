using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SwipeGesture : UIBehaviour, IBeginDragHandler, IEventSystemHandler, IEndDragHandler
{
	public void OnBeginDrag(PointerEventData eventData)
	{
		this.startTime = Time.time;
		Vector2 vector;
		RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)base.transform, eventData.position, eventData.pressEventCamera, out vector);
		this.startLocalPosition = vector;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		float num = Time.time - this.startTime;
		RectTransform rectTransform = (RectTransform)base.transform;
		Vector2 a;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out a);
		Vector2 vector = a - this.startLocalPosition;
		bool flag = Mathf.Abs(vector.normalized[(int)this.swipeAxis]) > 0.5f;
		bool flag2 = num < this.gestureMaxDuration;
		float num2 = (this.swipeAxis == SwipeGesture.Axis.Horizontal) ? rectTransform.rect.width : rectTransform.rect.height;
		float num3 = Mathf.Abs(vector[(int)this.swipeAxis]);
		float num4 = num2 * this.gestureMinDistanceNormalized;
		bool flag3 = num3 > num4;
		if (flag && flag3 && flag2)
		{
			int num5 = (int)Mathf.Sign(vector[(int)this.swipeAxis]);
			num5 *= (this.invertScroll ? -1 : 1);
			this.swipeExecuted.Invoke(num5);
		}
	}

	public float gestureMaxDuration = 1f;

	public float gestureMinDistanceNormalized = 0.15f;

	[Space(10f)]
	public bool invertScroll;

	public SwipeGesture.Axis swipeAxis;

	private float startTime;

	private Vector2 startLocalPosition;

	[Space(10f)]
	[SerializeField]
	private UnityEvent<int> swipeExecuted;

	public enum Axis
	{
		Horizontal,
		Vertical
	}
}
