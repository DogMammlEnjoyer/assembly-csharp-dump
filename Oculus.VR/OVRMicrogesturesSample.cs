using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OVRMicrogesturesSample : MonoBehaviour
{
	private void Start()
	{
		this.leftGestureSource.GestureRecognizedEvent.AddListener(delegate(OVRHand.MicrogestureType gesture)
		{
			this.OnGestureRecognized(OVRPlugin.Hand.HandLeft, gesture);
		});
		this.rightGestureSource.GestureRecognizedEvent.AddListener(delegate(OVRHand.MicrogestureType gesture)
		{
			this.OnGestureRecognized(OVRPlugin.Hand.HandRight, gesture);
		});
	}

	private void HighlightGesture(OVRPlugin.Hand hand, OVRHand.MicrogestureType gesture)
	{
		switch (gesture)
		{
		case OVRHand.MicrogestureType.SwipeLeft:
			this.HighlightIcon((hand == OVRPlugin.Hand.HandLeft) ? this.leftArrowL : this.leftArrowR);
			return;
		case OVRHand.MicrogestureType.SwipeRight:
			this.HighlightIcon((hand == OVRPlugin.Hand.HandLeft) ? this.rightArrowL : this.rightArrowR);
			return;
		case OVRHand.MicrogestureType.SwipeForward:
			this.HighlightIcon((hand == OVRPlugin.Hand.HandLeft) ? this.upArrowL : this.upArrowR);
			return;
		case OVRHand.MicrogestureType.SwipeBackward:
			this.HighlightIcon((hand == OVRPlugin.Hand.HandLeft) ? this.downArrowL : this.downArrowR);
			return;
		case OVRHand.MicrogestureType.ThumbTap:
			this.HighlightIcon((hand == OVRPlugin.Hand.HandLeft) ? this.selectIconL : this.selectIconR);
			return;
		default:
			return;
		}
	}

	private void HighlightIcon(Image icon)
	{
		Coroutine routine;
		if (this.highlightCoroutines.TryGetValue(icon.gameObject, out routine))
		{
			base.StopCoroutine(routine);
			this.highlightCoroutines.Remove(icon.gameObject);
		}
		this.highlightCoroutines.Add(icon.gameObject, base.StartCoroutine(this.HighlightIconCoroutine(icon)));
	}

	private IEnumerator HighlightIconCoroutine(Image navIcon)
	{
		Color initialCol = this.initialColor;
		navIcon.color = this.highlightColor;
		float timer = 0f;
		while (timer < this.highlightDuration)
		{
			navIcon.color = Color.Lerp(this.highlightColor, initialCol, timer / this.highlightDuration);
			timer += Time.deltaTime;
			yield return null;
		}
		yield break;
	}

	private void HighlightIcon(Image navIcon, bool state)
	{
		navIcon.color = (state ? this.highlightColor : this.initialColor);
	}

	private void OnGestureRecognized(OVRPlugin.Hand hand, OVRHand.MicrogestureType gesture)
	{
		this.HighlightGesture(hand, gesture);
		this.ShowRecognizedGestureLabel((hand == OVRPlugin.Hand.HandLeft) ? this.leftGestureLabel : this.rightGestureLabel, gesture.ToString());
	}

	private void ShowRecognizedGestureLabel(Text gestureLabel, string label)
	{
		Coroutine routine;
		if (this.highlightCoroutines.TryGetValue(gestureLabel.gameObject, out routine))
		{
			base.StopCoroutine(routine);
			this.highlightCoroutines.Remove(gestureLabel.gameObject);
		}
		this.highlightCoroutines.Add(gestureLabel.gameObject, base.StartCoroutine(this.ShowGestureLabel(gestureLabel, label)));
	}

	private IEnumerator ShowGestureLabel(Text gestureLabel, string label)
	{
		gestureLabel.text = label;
		yield return new WaitForSeconds(this.gestureShowDuration);
		gestureLabel.text = string.Empty;
		yield break;
	}

	[SerializeField]
	private OVRMicrogestureEventSource leftGestureSource;

	[SerializeField]
	private OVRMicrogestureEventSource rightGestureSource;

	[Header("Gesture Labels")]
	[SerializeField]
	private Text leftGestureLabel;

	[SerializeField]
	private Text rightGestureLabel;

	[SerializeField]
	private float gestureShowDuration = 1.5f;

	[Header("Navigation Icons Left")]
	[SerializeField]
	private Image leftArrowL;

	[SerializeField]
	private Image rightArrowL;

	[SerializeField]
	private Image upArrowL;

	[SerializeField]
	private Image downArrowL;

	[SerializeField]
	private Image selectIconL;

	[Header("Navigation Icons Right")]
	[SerializeField]
	private Image leftArrowR;

	[SerializeField]
	private Image rightArrowR;

	[SerializeField]
	private Image upArrowR;

	[SerializeField]
	private Image downArrowR;

	[SerializeField]
	private Image selectIconR;

	[Header("Colors")]
	[SerializeField]
	private Color initialColor = Color.white;

	[SerializeField]
	private Color highlightColor = Color.blue;

	[SerializeField]
	private float highlightDuration = 1f;

	private Dictionary<GameObject, Coroutine> highlightCoroutines = new Dictionary<GameObject, Coroutine>();
}
