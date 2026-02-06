using System;
using System.Collections.Generic;
using System.Text;
using GorillaGameModes;
using GorillaTagScripts;
using TMPro;
using UnityEngine;

public class GorillaScoreBoard : MonoBehaviour
{
	public bool IsDirty
	{
		get
		{
			return this._isDirty || string.IsNullOrEmpty(this.initialGameMode);
		}
		set
		{
			this._isDirty = value;
		}
	}

	public void SetSleepState(bool awake)
	{
		this.boardText.enabled = awake;
		this.buttonText.enabled = awake;
		if (this.linesParent != null)
		{
			this.linesParent.SetActive(awake);
		}
	}

	private string GetBeginningString()
	{
		string text = string.Format(" ({0})", 10);
		if (NetworkSystem.Instance.SessionIsSubscription)
		{
			text = string.Format(" ({0})", 20);
		}
		return string.Concat(new string[]
		{
			"ROOM ID: ",
			NetworkSystem.Instance.SessionIsPrivate ? "-PRIVATE- GAME: " : (NetworkSystem.Instance.RoomName + "   GAME: "),
			this.RoomType(),
			text,
			"\n  PLAYER     COLOR  MUTE   REPORT"
		});
	}

	private string RoomType()
	{
		this.initialGameMode = RoomSystem.RoomGameMode;
		this.gmNames = GameMode.gameModeNames;
		this.gmName = "ERROR";
		int count = this.gmNames.Count;
		int num = this.initialGameMode.LastIndexOf('|');
		if (num >= 0)
		{
			this.tempGmName = this.initialGameMode.Substring(num + 1);
			for (int i = 0; i < count; i++)
			{
				if (this.tempGmName == this.gmNames[i])
				{
					this.gmName = this.tempGmName;
					break;
				}
			}
		}
		else
		{
			for (int j = 0; j < count; j++)
			{
				this.tempGmName = this.gmNames[j];
				if (this.initialGameMode.Contains(this.tempGmName))
				{
					this.gmName = this.tempGmName;
					break;
				}
			}
		}
		return this.gmName;
	}

	public void RedrawPlayerLines()
	{
		this.stringBuilder.Clear();
		this.stringBuilder.Append(this.GetBeginningString());
		this.buttonStringBuilder.Clear();
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
		int num = 0;
		for (int i = 0; i < this.lines.Count; i++)
		{
			if (this.lines[i].gameObject.activeInHierarchy)
			{
				num++;
			}
		}
		if (num > 10)
		{
			this.linesParent.transform.localScale = new Vector3(1f, 0.5f, 1f);
			this.linesParent.transform.localPosition = new Vector3(0f, this.bigRoomYOffset, 0f);
			this.textsParent.transform.localScale = new Vector3(1f, 0.5f, 1f);
		}
		else
		{
			this.linesParent.transform.localScale = Vector3.one;
			this.linesParent.transform.localPosition = Vector3.zero;
			this.textsParent.transform.localScale = Vector3.one;
		}
		for (int j = 0; j < this.lines.Count; j++)
		{
			try
			{
				if (this.lines[j].gameObject.activeInHierarchy)
				{
					this.linesRTs[j].localPosition = new Vector3(0f, (float)(this.startingYValue - this.lineHeight * j), 0f);
					if (this.lines[j].linePlayer != null && this.lines[j].linePlayer.InRoom)
					{
						this.stringBuilder.Append("\n ");
						SubscriptionManager.SubscriptionDetails subscriptionDetails = SubscriptionManager.GetSubscriptionDetails(this.lines[j].linePlayer);
						if (subscriptionDetails.active && subscriptionDetails.tier > 0)
						{
							this.stringBuilder.Append("<color=#ffc600>");
						}
						else
						{
							this.stringBuilder.Append("<color=#ffffff>");
						}
						this.stringBuilder.Append(flag ? this.lines[j].playerNameVisible : this.lines[j].linePlayer.DefaultName);
						this.stringBuilder.Append("</color>");
						if (this.lines[j].linePlayer != NetworkSystem.Instance.LocalPlayer)
						{
							if (this.lines[j].reportButton.isActiveAndEnabled)
							{
								this.buttonStringBuilder.Append("MUTE                                REPORT\n");
							}
							else
							{
								this.buttonStringBuilder.Append("MUTE                HATE SPEECH    TOXICITY     CHEATING       CANCEL\n");
							}
						}
						else
						{
							this.buttonStringBuilder.Append("\n");
						}
					}
				}
			}
			catch
			{
			}
		}
		this.boardText.text = this.stringBuilder.ToString();
		this.buttonText.text = this.buttonStringBuilder.ToString();
		this._isDirty = false;
	}

	public string NormalizeName(bool doIt, string text)
	{
		if (doIt)
		{
			text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => Utils.IsASCIILetterOrDigit(c)));
			if (text.Length > 12)
			{
				text = text.Substring(0, 10);
			}
			text = text.ToUpper();
		}
		return text;
	}

	private void Start()
	{
		this.linesRTs.Clear();
		for (int i = 0; i < this.lines.Count; i++)
		{
			this.linesRTs.Add(this.lines[i].GetComponent<RectTransform>());
		}
		GorillaScoreboardTotalUpdater.RegisterScoreboard(this);
	}

	private void OnEnable()
	{
		GorillaScoreboardTotalUpdater.RegisterScoreboard(this);
		this._isDirty = true;
	}

	private void OnDisable()
	{
		GorillaScoreboardTotalUpdater.UnregisterScoreboard(this);
	}

	public GameObject scoreBoardLinePrefab;

	public int startingYValue;

	public int lineHeight;

	public bool includeMMR;

	public bool isActive;

	public GameObject linesParent;

	public float bigRoomYOffset = 32.5f;

	[SerializeField]
	public List<GorillaPlayerScoreboardLine> lines;

	private List<RectTransform> linesRTs = new List<RectTransform>();

	public GameObject textsParent;

	public TextMeshPro boardText;

	public TextMeshPro buttonText;

	public bool needsUpdate;

	public TextMeshPro notInRoomText;

	public string initialGameMode;

	private string tempGmName;

	private string gmName;

	private const string error = "ERROR";

	private List<string> gmNames;

	private bool _isDirty = true;

	private StringBuilder stringBuilder = new StringBuilder(220);

	private StringBuilder buttonStringBuilder = new StringBuilder(720);
}
