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

	protected override void OnLostObservable()
	{
		if (VODTarget.AlertDisabled != null)
		{
			VODTarget.AlertDisabled(this);
		}
	}

	protected override void OnBecameObservable()
	{
		if (VODTarget.AlertEnabled != null)
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
			base.gameObject.SetActive(false);
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
		base.gameObject.SetActive(false);
	}

	protected override void ObservableSliceUpdate()
	{
	}

	[SerializeField]
	private Renderer targetRenderer;

	[SerializeField]
	private Material standbyOverride;

	[SerializeField]
	private VODTarget.VODTargetAudioSettings audioSettings;

	[SerializeField]
	private TMP_Text upNext;

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
