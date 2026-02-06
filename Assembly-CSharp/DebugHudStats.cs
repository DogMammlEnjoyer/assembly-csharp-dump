using System;
using System.Collections.Generic;
using System.Text;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using TMPro;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

public class DebugHudStats : MonoBehaviour
{
	public static DebugHudStats Instance
	{
		get
		{
			return DebugHudStats._instance;
		}
	}

	private void Awake()
	{
		if (DebugHudStats._instance != null && DebugHudStats._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			DebugHudStats._instance = this;
		}
		base.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		if (DebugHudStats._instance == this)
		{
			DebugHudStats._instance = null;
			if (this.drawCallsRecorder.Valid)
			{
				this.drawCallsRecorder.Dispose();
			}
			if (this.trisRecorder.Valid)
			{
				this.trisRecorder.Dispose();
			}
		}
	}

	private void LateUpdate()
	{
		base.transform.LookAt(Camera.main.transform.position, Vector3.up);
		if (this.currentState == DebugHudStats.State.timeAdjust)
		{
			bool flag = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand);
			bool flag2 = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
			bool flag3 = ControllerInputPoller.TriggerFloat(XRNode.RightHand) > 0.5f;
			bool flag4 = ControllerInputPoller.GripFloat(XRNode.RightHand) > 0.5f;
			if (this.button1Down && !flag)
			{
				GorillaComputer.instance.AddSeverTime(flag4 ? -60 : 60);
			}
			if (this.button2Down && !flag2)
			{
				GorillaComputer.instance.AddSeverTime(flag4 ? -1 : 5);
			}
			if (this.button3Down && !flag3)
			{
				GorillaComputer.instance.AddSeverTime(flag4 ? -1440 : 1440);
			}
			this.button1Down = flag;
			this.button2Down = flag2;
			this.button3Down = flag3;
		}
		bool flag5 = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
		if (this.buttonDown && !flag5)
		{
			Application.logMessageReceived -= this.LogMessageReceived;
			PlayerGameEvents.OnPlayerMoved -= this.OnPlayerMoved;
			PlayerGameEvents.OnPlayerSwam -= this.OnPlayerSwam;
			switch (this.currentState)
			{
			case DebugHudStats.State.Inactive:
				this.currentState = DebugHudStats.State.Active;
				break;
			case DebugHudStats.State.Active:
				this.currentState = DebugHudStats.State.ShowLog;
				break;
			case DebugHudStats.State.ShowLog:
				this.currentState = DebugHudStats.State.ShowError;
				break;
			case DebugHudStats.State.ShowError:
				this.currentState = DebugHudStats.State.ShowStats;
				break;
			case DebugHudStats.State.ShowStats:
				this.currentState = DebugHudStats.State.ShowRBs;
				break;
			case DebugHudStats.State.ShowRBs:
				this.currentState = DebugHudStats.State.timeAdjust;
				break;
			case DebugHudStats.State.timeAdjust:
				this.currentState = DebugHudStats.State.Inactive;
				break;
			}
			Application.logMessageReceived -= this.LogMessageReceived;
			PlayerGameEvents.OnPlayerMoved -= this.OnPlayerMoved;
			PlayerGameEvents.OnPlayerSwam -= this.OnPlayerSwam;
			DebugHudStats.State state = this.currentState;
			if (state - DebugHudStats.State.ShowLog > 1)
			{
				if (state == DebugHudStats.State.ShowStats)
				{
					this.distanceMoved = (this.distanceSwam = 0f);
					PlayerGameEvents.OnPlayerMoved += this.OnPlayerMoved;
					PlayerGameEvents.OnPlayerSwam += this.OnPlayerSwam;
				}
			}
			else
			{
				Application.logMessageReceived += this.LogMessageReceived;
			}
			this.text.gameObject.SetActive(this.currentState > DebugHudStats.State.Inactive);
			if (RigidbodyHighlighter.Instance != null)
			{
				RigidbodyHighlighter.Instance.Active = (this.currentState == DebugHudStats.State.ShowRBs);
			}
		}
		this.buttonDown = flag5;
		if (this.firstAwake == 0f)
		{
			this.firstAwake = Time.time;
		}
		if (this.updateTimer < this.delayUpdateRate)
		{
			this.updateTimer += Time.deltaTime;
			return;
		}
		int num = Mathf.RoundToInt(1f / Time.smoothDeltaTime);
		if (num < DebugHudStats.FPS_THRESHOLD)
		{
			this.lowFps++;
		}
		else
		{
			this.lowFps = 0;
		}
		this.fpsWarning.gameObject.SetActive(this.lowFps > 5 && this.currentState == DebugHudStats.State.Inactive);
		if (this.currentState != DebugHudStats.State.Inactive)
		{
			this.builder.Clear();
			this.builder.Append("<color=\"" + this.colorFromState(this.currentState) + "\">");
			this.builder.Append("gt: ");
			this.builder.Append(GorillaComputer.instance.version);
			this.builder.Append(":");
			this.builder.Append(GorillaComputer.instance.buildCode);
			this.builder.Append("</color>");
			num = Mathf.Min(num, 90);
			this.builder.Append((num < DebugHudStats.FPS_THRESHOLD) ? " - <color=\"red\">" : " - <color=\"white\">");
			this.builder.Append(num);
			this.builder.Append(string.Format(" fps / {0} fps</color> ", DebugHudStats.FPS_THRESHOLD + 1));
			this.builder.AppendLine(string.Format("sfps: {0} (Health: {1})", GorillaTagger.Instance.SmoothedFramerate, GorillaTagger.Instance.FramerateHealth));
			float eyeTextureResolutionScale = XRSettings.eyeTextureResolutionScale;
			float renderViewportScale = XRSettings.renderViewportScale;
			float renderScale = (GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset).renderScale;
			this.builder.AppendLine(string.Format("draw calls: {0} tris: {1} ", this.drawCallsRecorder.LastValue, this.trisRecorder.LastValue) + string.Format("rs: {0}/{1}/{2} ", eyeTextureResolutionScale, renderViewportScale, renderScale));
			if (GorillaComputer.instance != null)
			{
				this.builder.AppendLine(GorillaComputer.instance.GetServerTime().ToString());
			}
			else
			{
				this.builder.AppendLine("Server Time Unavailable");
			}
			this.zones = GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone.ToString().ToUpperInvariant();
			if (NetworkSystem.Instance.IsMasterClient)
			{
				this.builder.Append("H");
			}
			if (NetworkSystem.Instance.InRoom)
			{
				if (NetworkSystem.Instance.SessionIsPrivate)
				{
					this.builder.Append("Pri ");
				}
				else
				{
					this.builder.Append("Pub ");
				}
			}
			else
			{
				this.builder.Append("DC ");
			}
			this.builder.Append("z: <color=\"green\">");
			this.builder.Append(this.zones);
			this.builder.AppendLine("</color>");
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaGameManager instance = GorillaGameManager.instance;
				if (instance != null)
				{
					GorillaTagCompetitiveManager gorillaTagCompetitiveManager = instance as GorillaTagCompetitiveManager;
					if (gorillaTagCompetitiveManager != null)
					{
						this.builder.Append("Ranked Mode ELO: ");
						this.builder.Append(gorillaTagCompetitiveManager.GetScoring().Progression.GetEloScore().ToString());
						this.builder.Append("  Tier: ");
						this.builder.AppendLine(gorillaTagCompetitiveManager.GetScoring().Progression.GetRankedProgressionTierName());
						RankedMultiplayerScore.PlayerScoreInRound inGameScoreForSelf = gorillaTagCompetitiveManager.GetScoring().GetInGameScoreForSelf();
						this.builder.Append("Tags: ");
						this.builder.Append(inGameScoreForSelf.NumTags.ToString());
						this.builder.Append("  Defense: ");
						this.builder.Append(Mathf.RoundToInt(inGameScoreForSelf.PointsOnDefense).ToString());
						this.builder.Append("  Score: ");
						this.builder.AppendLine(Mathf.RoundToInt(gorillaTagCompetitiveManager.GetScoring().ComputeGameScore(inGameScoreForSelf.NumTags, inGameScoreForSelf.PointsOnDefense)).ToString());
						if (gorillaTagCompetitiveManager.ShowDebugPing)
						{
							this.builder.AppendLine("Server MatchID Ping!");
						}
					}
				}
			}
			if (this.currentState == DebugHudStats.State.ShowStats)
			{
				this.builder.AppendLine();
				Vector3 vector = GTPlayer.Instance.AveragedVelocity;
				Vector3 headCenterPosition = GTPlayer.Instance.HeadCenterPosition;
				float magnitude = vector.magnitude;
				this.groundVelocity = vector;
				this.groundVelocity.y = 0f;
				this.builder.AppendLine(string.Format("v: {0:F1} m/s\t\todo: {1:F2}m\tswam: {2:F2}m", magnitude, this.distanceMoved, this.distanceSwam));
				this.builder.AppendLine(string.Format("ground: {0:F1} m/s\thead: {1:F2}", this.groundVelocity.magnitude, headCenterPosition));
			}
			else if (this.currentState == DebugHudStats.State.ShowLog)
			{
				this.builder.AppendLine();
				for (int i = this.logMessage.Count - 1; i >= 0; i--)
				{
					this.builder.AppendLine(this.logMessage[i]);
				}
			}
			else if (this.currentState == DebugHudStats.State.ShowError)
			{
				this.builder.AppendLine();
				for (int j = this.logError.Count - 1; j >= 0; j--)
				{
					this.builder.AppendLine(this.logError[j]);
				}
			}
			else if (this.currentState == DebugHudStats.State.timeAdjust)
			{
				this.builder.AppendLine();
				this.builder.AppendLine("Press A to advance one hour [+ R Grip to go back one hour]");
				this.builder.AppendLine("Press B to advance five minutes [+ R Grip to go back one minute]");
				this.builder.AppendLine("Press R Trigger to advance one day [+ R Grip to go back one day]");
			}
			this.text.text = this.builder.ToString();
		}
		this.updateTimer = 0f;
	}

	private string colorFromState(DebugHudStats.State s)
	{
		switch (s)
		{
		case DebugHudStats.State.ShowLog:
			return "yellow";
		case DebugHudStats.State.ShowError:
			return "orange";
		case DebugHudStats.State.ShowStats:
			return "green";
		case DebugHudStats.State.ShowRBs:
			return "red";
		default:
			return "white";
		}
	}

	private void OnPlayerSwam(float distance, float speed)
	{
		if (distance > 0.005f)
		{
			this.distanceSwam += distance;
		}
	}

	private void OnPlayerMoved(float distance, float speed)
	{
		if (distance > 0.005f)
		{
			this.distanceMoved += distance;
		}
	}

	private void OnDisable()
	{
		Application.logMessageReceived -= this.LogMessageReceived;
	}

	private void LogMessageReceived(string condition, string stackTrace, LogType type)
	{
		string text = string.Format("{0:F2}> {1}{2}</color>", Time.realtimeSinceStartup, this.getColorStringFromLogType(type), condition);
		if (this.pLog != condition)
		{
			this.logMessage.Add(text);
		}
		else
		{
			this.logMessage[this.logMessage.Count - 1] = text;
		}
		this.pLog = condition;
		if (this.logMessage.Count > 10)
		{
			this.logMessage.RemoveAt(0);
		}
		if (type == LogType.Error || type == LogType.Assert || type == LogType.Exception)
		{
			this.logError.Add(text + "\n" + stackTrace);
			if (this.logError.Count > 10)
			{
				this.logError.RemoveAt(0);
			}
		}
	}

	private string getColorStringFromLogType(LogType type)
	{
		switch (type)
		{
		case LogType.Error:
		case LogType.Assert:
		case LogType.Exception:
			return "<color=\"red\">";
		case LogType.Warning:
			return "<color=\"yellow\">";
		}
		return "<color=\"white\">";
	}

	private void OnZoneChanged(ZoneData[] zoneData)
	{
		this.zones = string.Empty;
		for (int i = 0; i < zoneData.Length; i++)
		{
			if (zoneData[i].active)
			{
				this.zones = this.zones + zoneData[i].zone.ToString().ToUpper() + "; ";
			}
		}
	}

	public static int FPS_THRESHOLD = 89;

	private static DebugHudStats _instance;

	[SerializeField]
	public TMP_Text text;

	[SerializeField]
	private TMP_Text fpsWarning;

	[SerializeField]
	private float delayUpdateRate = 0.25f;

	private float updateTimer;

	public float sessionAnytrackingLost;

	public float last30SecondsTrackingLost;

	private float firstAwake;

	private bool leftHandTracked;

	private bool rightHandTracked;

	private StringBuilder builder;

	private Vector3 averagedVelocity;

	private Vector3 groundVelocity;

	private Vector3 centerHeadPos;

	private float distanceMoved;

	private float distanceSwam;

	private List<string> logMessage = new List<string>();

	private List<string> logError = new List<string>();

	private bool buttonDown;

	private bool showLog;

	private int lowFps;

	private string zones;

	private GroupJoinZoneAB lastGroupJoinZone;

	private DebugHudStats.State currentState = DebugHudStats.State.Active;

	private ProfilerRecorder drawCallsRecorder;

	private ProfilerRecorder trisRecorder;

	private string pLog;

	private bool button1Down;

	private bool button2Down;

	private bool button3Down;

	private enum State
	{
		Inactive,
		Active,
		ShowLog,
		ShowError,
		ShowStats,
		ShowRBs,
		timeAdjust
	}
}
