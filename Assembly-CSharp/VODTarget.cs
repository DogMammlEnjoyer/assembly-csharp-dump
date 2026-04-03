using System;
using TMPro;
using UnityEngine;

public class VODTarget : ObservableBehavior, IBuildValidation
{
	public VODTarget.VODTargetAudioSettings AudioSettings
	{
		get
		{
			return this.audioSettings;
		}
	}

	public Renderer Renderer
	{
		get
		{
			return this.targetRenderer;
		}
	}

	public TMP_Text UpNextText
	{
		get
		{
			return this.upNext;
		}
	}

	public Material StandbyOverride
	{
		get
		{
			return this.standbyOverride;
		}
	}

	public VODPlayer.VODStream.VODStreamChannel[] Channel
	{
		get
		{
			if (this.channel.Length != 0)
			{
				return this.channel;
			}
			return new VODPlayer.VODStream.VODStreamChannel[1];
		}
	}

	public bool VerifyChannel(VODPlayer.VODStream.VODStreamChannel ch)
	{
		if (this.channel.Length == 0 && ch == VODPlayer.VODStream.VODStreamChannel.DEFAULT)
		{
			return true;
		}
		for (int i = 0; i < this.channel.Length; i++)
		{
			if (this.channel[i] == ch)
			{
				return true;
			}
		}
		return false;
	}

	protected override void OnLostObservable()
	{
		this.observable = false;
		if (!this.staticScreen.activeInHierarchy && VODTarget.AlertDisabled != null)
		{
			VODTarget.AlertDisabled(this);
		}
	}

	protected override void OnBecameObservable()
	{
		this.observable = true;
		if (!this.staticScreen.activeInHierarchy && VODTarget.AlertEnabled != null)
		{
			VODTarget.AlertEnabled(this);
		}
	}

	bool IBuildValidation.BuildValidationCheck()
	{
		if (this.targetRenderer == null)
		{
			Debug.LogError("VODTarget " + base.name + " must set a Target Renderer");
			return false;
		}
		return true;
	}

	protected override void UnityOnEnable()
	{
		VODPlayer.OnCrash = (Action)Delegate.Combine(VODPlayer.OnCrash, new Action(this.VODPlayer_OnCrash));
		if (VODPlayer.state == VODPlayer.State.CRASHED)
		{
			this.staticScreen.SetActive(true);
		}
	}

	protected override void UnityOnDisable()
	{
		VODPlayer.OnCrash = (Action)Delegate.Remove(VODPlayer.OnCrash, new Action(this.VODPlayer_OnCrash));
	}

	private void OnDestroy()
	{
		VODPlayer.OnCrash = (Action)Delegate.Remove(VODPlayer.OnCrash, new Action(this.VODPlayer_OnCrash));
	}

	private void VODPlayer_OnCrash()
	{
		this.staticScreen.SetActive(true);
	}

	protected override void ObservableSliceUpdate()
	{
	}

	public void ShowStatic(bool on)
	{
		this.staticScreen.SetActive(on);
		if (on)
		{
			if (this.observable && VODTarget.AlertDisabled != null)
			{
				VODTarget.AlertDisabled(this);
				return;
			}
		}
		else if (this.observable && VODTarget.AlertEnabled != null)
		{
			VODTarget.AlertEnabled(this);
		}
	}

	[SerializeField]
	private Renderer targetRenderer;

	[SerializeField]
	private Material standbyOverride;

	[SerializeField]
	private VODTarget.VODTargetAudioSettings audioSettings;

	[SerializeField]
	private TMP_Text upNext;

	[SerializeField]
	private VODPlayer.VODStream.VODStreamChannel[] channel;

	[SerializeField]
	private GameObject staticScreen;

	private bool observable;

	public static Action<VODTarget> AlertEnabled;

	public static Action<VODTarget> AlertDisabled;

	[Serializable]
	public class VODTargetAudioSettings
	{
		[Range(0f, 1f)]
		public float volume;

		[Range(0f, 5f)]
		public float dopplerLevel = 1f;

		[Range(0f, 360f)]
		public float spread;

		public AudioRolloffMode rolloffMode;

		public float minDistance = 0.5f;

		public float maxDistance = 5f;
	}
}
