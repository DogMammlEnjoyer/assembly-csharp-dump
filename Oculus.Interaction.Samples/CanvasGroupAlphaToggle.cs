using System;
using UnityEngine;

public class CanvasGroupAlphaToggle : MonoBehaviour
{
	public void ToggleVisible()
	{
		this.visible = !this.visible;
	}

	private void Start()
	{
	}

	private void Update()
	{
		this.canvasGroup.alpha = Mathf.Lerp(this.canvasGroup.alpha, this.visible ? 1f : 0f, this.animationSpeed * Time.deltaTime);
	}

	public CanvasGroup canvasGroup;

	public float animationSpeed;

	private bool visible;
}
