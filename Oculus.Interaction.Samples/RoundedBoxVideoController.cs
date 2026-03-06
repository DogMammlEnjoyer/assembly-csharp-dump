using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundedBoxVideoController : MonoBehaviour
{
	private void OnEnable()
	{
		this.UpdateBackgroundMaterialProperties();
	}

	private void Start()
	{
		this.animations = new List<RoundedBoxVideoController.BoxAnimation>();
		Vector2 size = ((RectTransform)base.transform).rect.size;
		float num = size.x / (float)this.boxes.Count;
		this.timeSlider.onValueChanged.AddListener(delegate(float <p0>)
		{
			this.OnSliderValueChange();
		});
		float num2 = ((float)this.boxes.Count - 1f) * 0.35f + 1f;
		this.animationCycleDuration = this.animationDuration / (float)this.cycleCount;
		float num3 = this.animationCycleDuration / num2;
		float num4 = this.boxes[0].rect.height * 0.6f;
		float num5 = size.y * 0.5f + num4;
		float num6 = 2f * num5 / (num3 * 0.5f);
		float acceleration = num6 / (num3 * 0.5f);
		for (int i = 0; i < this.boxes.Count; i++)
		{
			RectTransform rectTransform = this.boxes[i];
			float num7 = (float)i + 0.5f;
			rectTransform.anchoredPosition = new Vector2(num7 * num, 0f);
			RoundedBoxVideoController.BoxAnimation item = new RoundedBoxVideoController.BoxAnimation
			{
				duration = num3,
				startHeight = num4,
				animationMaxHeight = num5,
				rectTransform = rectTransform,
				startVelocity = num6,
				acceleration = acceleration,
				startTime = num3 * 0.35f * (float)i,
				image = rectTransform.GetComponent<Image>()
			};
			this.animations.Add(item);
		}
		this.SetPlay();
		this.UpdateBackgroundMaterialProperties();
	}

	public void UpdateBackgroundMaterialProperties()
	{
		Vector2 normalized = this.direction.normalized;
		this.backgroundImage.materialForRendering.SetVector(this.columnDirectionID, normalized);
		this.backgroundImage.materialForRendering.SetVector(this.rowDirectionID, new Vector2(-normalized.y, normalized.x));
		this.backgroundImage.materialForRendering.SetColor(this.colorAID, this.colorA.linear);
		this.backgroundImage.materialForRendering.SetColor(this.colorBID, this.colorB.linear);
		this.backgroundImage.materialForRendering.SetFloat(this.animationTimeID, this.animationTime);
	}

	public void OnSliderValueChange()
	{
		this.animationTime = this.timeSlider.value * this.animationDuration;
	}

	public void TogglePlayPause()
	{
		if (this.isPlaying)
		{
			this.SetPaused();
			return;
		}
		if (Mathf.Abs(this.animationDuration - this.animationTime) < 0.1f)
		{
			this.animationTime = 0f;
		}
		this.SetPlay();
	}

	private void SetPaused()
	{
		this.isPlaying = false;
		this.playPauseImg.sprite = this.playIcon;
	}

	private void SetPlay()
	{
		this.isPlaying = true;
		this.playPauseImg.sprite = this.pauseIcon;
	}

	private string FormatTime(float seconds)
	{
		int num = Mathf.FloorToInt(seconds / 60f);
		int num2 = (int)seconds % 60;
		string str = num.ToString();
		string str2 = num2.ToString("D2");
		return str + ":" + str2;
	}

	private void LateUpdate()
	{
		if (this.isPlaying)
		{
			this.animationTime += Time.deltaTime;
			this.timeSlider.SetValueWithoutNotify(this.animationTime / this.animationDuration);
			if (this.animationTime > this.animationDuration)
			{
				this.animationTime = this.animationDuration;
				this.SetPaused();
			}
		}
		else
		{
			this.animationTime = this.timeSlider.value * this.animationDuration;
		}
		for (int i = 0; i < this.animations.Count; i++)
		{
			float num = Mathf.Floor(this.animationTime / this.animationCycleDuration) % (float)this.boxColors.Count;
			this.animations[i].SetColor(this.boxColors[(int)num]);
			this.animations[i].Update(this.animationTime % this.animationCycleDuration);
		}
		float seconds = Mathf.Round(this.animationDuration - this.animationTime);
		this.leftLabel.SetText(this.FormatTime(this.animationTime));
		this.rightLabel.SetText(this.FormatTime(seconds));
		this.backgroundImage.materialForRendering.SetFloat(this.animationTimeID, this.animationTime);
	}

	public Slider timeSlider;

	public float animationDuration;

	public float animationTime;

	public int cycleCount;

	public Sprite playIcon;

	public Sprite pauseIcon;

	public Image playPauseImg;

	public bool isPlaying;

	public List<Color> boxColors;

	public List<RectTransform> boxes;

	private float animationCycleDuration;

	[Header("Time Labels")]
	public TextMeshProUGUI leftLabel;

	public TextMeshProUGUI rightLabel;

	[Header("Background Material Settings")]
	public Image backgroundImage;

	public Vector2 direction;

	public Color colorA;

	public Color colorB;

	private readonly int columnDirectionID = Shader.PropertyToID("columnDirection");

	private readonly int rowDirectionID = Shader.PropertyToID("rowDirection");

	private readonly int animationTimeID = Shader.PropertyToID("animationTime");

	private readonly int colorAID = Shader.PropertyToID("colorA");

	private readonly int colorBID = Shader.PropertyToID("colorB");

	private List<RoundedBoxVideoController.BoxAnimation> animations;

	private struct BoxAnimation
	{
		public void Update(float animationTime)
		{
			float num = animationTime - this.startTime;
			num = Mathf.Clamp(num, 0f, this.duration);
			float num2 = num * num;
			float num3 = this.startVelocity * num - 0.5f * this.acceleration * num2;
			float num4 = num3 / this.animationMaxHeight;
			float y = this.startHeight - num3;
			Vector2 anchoredPosition = this.rectTransform.anchoredPosition;
			anchoredPosition.y = y;
			this.rectTransform.anchoredPosition = anchoredPosition;
			this.rectTransform.rotation = Quaternion.Euler(0f, 0f, num4 * 360f);
		}

		public void SetColor(Color color)
		{
			this.image.color = color;
		}

		public RectTransform rectTransform;

		public Image image;

		public float duration;

		public float startHeight;

		public float animationMaxHeight;

		public float startVelocity;

		public float startTime;

		public float acceleration;
	}
}
