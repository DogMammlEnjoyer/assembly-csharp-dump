using System;
using System.Collections.Generic;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using Meta.XR.ImmersiveDebugger.Utils;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.UserInterface
{
	[DefaultExecutionOrder(1)]
	public class Console : DebugPanel
	{
		internal bool Dirty { get; set; }

		private SeverityEntry GetSeverity(LogType logType)
		{
			SeverityEntry result;
			if (!this._severitiesPerType.TryGetValue(logType, out result))
			{
				return null;
			}
			return result;
		}

		internal bool LogCollapseMode { get; private set; }

		internal int MaximumNumberOfLogEntries { get; private set; }

		public ImageStyle LogDetailBackgroundStyle
		{
			set
			{
				this._logDetailPaneBackground.Sprite = value.sprite;
				this._logDetailPaneBackground.Color = value.color;
				this._logDetailPaneBackground.PixelDensityMultiplier = value.pixelDensityMultiplier;
			}
		}

		protected override void Setup(Controller owner)
		{
			base.Setup(owner);
			this._flex = base.Append<Flex>("main");
			this._flex.LayoutStyle = Style.Load<LayoutStyle>("ConsoleFlex");
			this._buttonsAnchor = this._flex.Append<Flex>("buttons");
			this._buttonsAnchor.LayoutStyle = Style.Load<LayoutStyle>("ConsoleButtons");
			this.LogCollapseMode = RuntimeSettings.Instance.CollapsedIdenticalLogEntries;
			this._collapseActiveIcon = Resources.Load<Texture2D>("Textures/compress_icon");
			this._collapseInactiveIcon = Resources.Load<Texture2D>("Textures/expand_icon");
			this._collapseBtn = this.RegisterControl("LogCollapse", this.LogCollapseMode ? this._collapseInactiveIcon : this._collapseActiveIcon, Style.Load<ImageStyle>("LogCollapseIcon"), new Action(this.ToggleCollapseMode));
			this._collapseBtn.State = this.LogCollapseMode;
			this.RegisterControl("Clear", Resources.Load<Texture2D>("Textures/bin_icon"), Style.Load<ImageStyle>("BinIcon"), new Action(this.Clear));
			SeverityEntry severityEntry = new SeverityEntry(this, "Error", Resources.Load<Texture2D>("Textures/error_icon"), Style.Load<ImageStyle>("ErrorIcon"), Style.Load<ImageStyle>("PillError"));
			SeverityEntry severityEntry2 = new SeverityEntry(this, "Warning", Resources.Load<Texture2D>("Textures/warning_icon"), Style.Load<ImageStyle>("WarningIcon"), Style.Load<ImageStyle>("PillWarning"));
			SeverityEntry severityEntry3 = new SeverityEntry(this, "Log", Resources.Load<Texture2D>("Textures/notice_icon"), Style.Load<ImageStyle>("NoticeIcon"), Style.Load<ImageStyle>("PillInfo"));
			this._severities.Add(severityEntry3);
			this._severities.Add(severityEntry2);
			this._severities.Add(severityEntry);
			this._severitiesPerType.Add(LogType.Assert, severityEntry);
			this._severitiesPerType.Add(LogType.Error, severityEntry);
			this._severitiesPerType.Add(LogType.Exception, severityEntry);
			this._severitiesPerType.Add(LogType.Warning, severityEntry2);
			this._severitiesPerType.Add(LogType.Log, severityEntry3);
			RuntimeSettings instance = RuntimeSettings.Instance;
			severityEntry.ShouldShow = instance.ShowErrorLog;
			severityEntry2.ShouldShow = instance.ShowWarningLog;
			severityEntry3.ShouldShow = instance.ShowInfoLog;
			this._scrollView = base.Append<ScrollView>("logs");
			this._scrollView.LayoutStyle = Style.Instantiate<LayoutStyle>("LogsScrollView");
			this._scrollView.Flex.LayoutStyle = Style.Load<LayoutStyle>("ConsoleLogs");
			this.MaximumNumberOfLogEntries = instance.MaximumNumberOfLogEntries;
			this._proxyFlex = new ProxyFlex<ConsoleLine, ProxyConsoleLine>(14, this.MaximumNumberOfLogEntries, Style.Load<LayoutStyle>("ConsoleLine"), this._scrollView);
			this._logDetailPaneBackground = base.Append<Background>("background");
			this._logDetailPaneBackground.LayoutStyle = Style.Load<LayoutStyle>("LogDetailsPaneBackground");
			this._logDetailPaneBackgroundImageStyle = Style.Load<ImageStyle>("LogDetailPaneBackground");
			this.LogDetailBackgroundStyle = this._logDetailPaneBackgroundImageStyle;
			this._scrollViewLogDetails = base.Append<ScrollView>("details");
			this._scrollViewLogDetails.LayoutStyle = Style.Load<LayoutStyle>("LogDetailsScrollView");
			this._scrollViewLogDetails.Flex.LayoutStyle = Style.Load<LayoutStyle>("ConsoleLogDetails");
			this._logDetailLabel = this._scrollViewLogDetails.Flex.Append<Label>("entry");
			this._logDetailLabel.LayoutStyle = Style.Instantiate<LayoutStyle>("ConsoleLineLogDetailsLabel");
			this._logDetailLabel.TextStyle = Style.Load<TextStyle>("ConsoleLogDetailsLabel");
			this._logDetailLabel.Text.horizontalOverflow = HorizontalWrapMode.Wrap;
			this._logDetailPaneCloseBtn = base.Append<ButtonWithIcon>("close");
			this._logDetailPaneCloseBtn.LayoutStyle = Style.Load<LayoutStyle>("LogDetailPaneCloseButton");
			this._logDetailPaneCloseBtn.Icon = Resources.Load<Texture2D>("Textures/close_icon");
			this._logDetailPaneCloseBtn.IconStyle = Style.Load<ImageStyle>("LogDetailPaneCloseButton");
			this._logDetailPaneCloseBtn.Callback = new Action(this.HideLogDetailsPanel);
			this.HideLogDetailsPanel();
			this.LogCollapseMode = RuntimeSettings.Instance.CollapsedIdenticalLogEntries;
			LogEntry.OnDisplayDetails = new Action<LogEntry>(this.OnConsoleLineClicked);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			ConsoleLogsCache.OnLogReceived = (Action<string, string, LogType>)Delegate.Remove(ConsoleLogsCache.OnLogReceived, new Action<string, string, LogType>(this.EnqueueLogEntry));
			ConsoleLogsCache.OnLogReceived = (Action<string, string, LogType>)Delegate.Combine(ConsoleLogsCache.OnLogReceived, new Action<string, string, LogType>(this.EnqueueLogEntry));
			ConsoleLogsCache.ConsumeStartupLogs(new Action<string, string, LogType>(this.EnqueueLogEntry));
		}

		protected override void OnDisable()
		{
			ConsoleLogsCache.OnLogReceived = (Action<string, string, LogType>)Delegate.Remove(ConsoleLogsCache.OnLogReceived, new Action<string, string, LogType>(this.EnqueueLogEntry));
			base.OnDisable();
		}

		protected override void OnTransparencyChanged()
		{
			base.OnTransparencyChanged();
			this._logDetailPaneBackground.Color = (base.Transparent ? this._logDetailPaneBackgroundImageStyle.colorOff : this._logDetailPaneBackgroundImageStyle.color);
		}

		internal Label RegisterCount()
		{
			Label label = this._buttonsAnchor.Append<Label>("");
			label.LayoutStyle = Style.Load<LayoutStyle>("ConsoleButtonCount");
			label.TextStyle = Style.Load<TextStyle>("ConsoleButtonCount");
			return label;
		}

		internal Toggle RegisterControl(string buttonName, Texture2D icon, ImageStyle style, Action callback)
		{
			if (buttonName == null)
			{
				throw new ArgumentNullException("buttonName");
			}
			if (icon == null)
			{
				throw new ArgumentNullException("icon");
			}
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			Toggle toggle = this._buttonsAnchor.Append<Toggle>(buttonName);
			toggle.LayoutStyle = Style.Load<LayoutStyle>("ConsoleButton");
			toggle.Icon = icon;
			toggle.IconStyle = (style ? style : Style.Default<ImageStyle>());
			toggle.Callback = callback;
			return toggle;
		}

		private void ToggleCollapseMode()
		{
			this.LogCollapseMode = !this.LogCollapseMode;
			this._collapseBtn.Icon = (this.LogCollapseMode ? this._collapseInactiveIcon : this._collapseActiveIcon);
			if (this.LogCollapseMode)
			{
				this.MergeEntries();
				return;
			}
			this.FlattenEntries();
		}

		private void EnqueueLogEntry(string logString, string stackTrace, LogType type)
		{
			SeverityEntry severity = this.GetSeverity(type);
			if (severity == null)
			{
				return;
			}
			int key = Console.ComputeLogHash(logString, stackTrace);
			LogEntry logEntry;
			int count;
			if (this._entryMap.TryGetValue(key, out logEntry) && this.LogCollapseMode)
			{
				this._entries.Remove(logEntry);
				this._proxyFlex.RemoveProxy(logEntry.Line);
				LogEntry logEntry2 = logEntry;
				count = logEntry2.Count;
				logEntry2.Count = count + 1;
			}
			else
			{
				if (this._entries.Count >= this.MaximumNumberOfLogEntries)
				{
					this.RemoveLogEntry(this._entries[0]);
				}
				logEntry = OVRObjectPool.Get<LogEntry>();
				logEntry.Setup(logString, stackTrace, severity);
				this._entryMap[key] = logEntry;
			}
			this._entries.Add(logEntry);
			LogEntry logEntry3 = OVRObjectPool.Get<LogEntry>();
			logEntry3.Setup(logString, stackTrace, severity);
			this._allEntries.Add(logEntry3);
			SeverityEntry severityEntry = severity;
			count = severityEntry.Count;
			severityEntry.Count = count + 1;
			this.AppendToProxyFlex(logEntry);
		}

		private void RemoveLogEntry(LogEntry logEntry)
		{
			logEntry.Severity.Count -= logEntry.Count;
			this._entries.Remove(logEntry);
			this._allEntries.RemoveAll(delegate(LogEntry entry)
			{
				bool flag = entry == logEntry;
				if (flag)
				{
					OVRObjectPool.Return<LogEntry>(entry);
				}
				return flag;
			});
			OVRObjectPool.Return<LogEntry>(logEntry);
		}

		private void Update()
		{
			if (this.Dirty)
			{
				this.RefreshAllEntries();
				this.Dirty = false;
			}
			this._proxyFlex.Update();
			if (this._lerpCompleted)
			{
				return;
			}
			this._currentPosition = Utils.LerpPosition(this._currentPosition, this._targetPosition, this._lerpSpeed);
			this._lerpCompleted = (this._currentPosition == this._targetPosition);
			base.SphericalCoordinates = this._currentPosition;
		}

		private void Clear()
		{
			this._entries.Clear();
			foreach (LogEntry obj in this._allEntries)
			{
				OVRObjectPool.Return<LogEntry>(obj);
			}
			this._allEntries.Clear();
			this._entryMap.Clear();
			this._proxyFlex.Clear();
			foreach (SeverityEntry severityEntry in this._severities)
			{
				severityEntry.Reset();
			}
			this.HideLogDetailsPanel();
			this.Dirty = true;
		}

		private void RefreshAllEntries()
		{
			foreach (LogEntry logEntry in this._entries)
			{
				if (!logEntry.Severity.ShouldShow)
				{
					if (logEntry.Shown)
					{
						this._proxyFlex.RemoveProxy(logEntry.Line);
						logEntry.Line = null;
					}
				}
				else if (!logEntry.Shown)
				{
					ProxyConsoleLine proxyConsoleLine = this._proxyFlex.AppendProxy();
					proxyConsoleLine.Entry = logEntry;
					logEntry.Line = proxyConsoleLine;
				}
			}
		}

		private void MergeEntries()
		{
			this._entries.Clear();
			this._proxyFlex.Clear();
			this.ResetLogCount();
			foreach (LogEntry logEntry in this._allEntries)
			{
				int key = Console.ComputeLogHash(logEntry.Label, logEntry.Callstack);
				LogEntry logEntry2;
				if (this._entryMap.TryGetValue(key, out logEntry2))
				{
					this._entries.Remove(logEntry2);
					this._proxyFlex.RemoveProxy(logEntry2.Line);
					LogEntry logEntry3 = logEntry2;
					int count = logEntry3.Count;
					logEntry3.Count = count + 1;
				}
				this._entries.Add(logEntry2);
				this.AppendToProxyFlex(logEntry2);
			}
			this.Dirty = true;
		}

		private void ResetLogCount()
		{
			foreach (LogEntry logEntry in this._allEntries)
			{
				logEntry.Count = 0;
			}
		}

		private void FlattenEntries()
		{
			this._entries.Clear();
			this._proxyFlex.Clear();
			foreach (LogEntry logEntry in this._allEntries)
			{
				this._entries.Add(logEntry);
				this.AppendToProxyFlex(logEntry);
			}
			this.Dirty = true;
		}

		private void AppendToProxyFlex(LogEntry entry)
		{
			if (entry.Severity.ShouldShow)
			{
				ProxyConsoleLine proxyConsoleLine = this._proxyFlex.AppendProxy();
				proxyConsoleLine.Entry = entry;
				entry.Line = proxyConsoleLine;
			}
		}

		private void OnConsoleLineClicked(LogEntry entry)
		{
			this.ShowLogDetailsPanel();
			this._logDetailLabel.Content = entry.Label + "\n" + entry.Callstack;
			this._logDetailLabel.SetHeight(this._logDetailLabel.Text.preferredHeight + 20f);
			this._logDetailLabel.RefreshLayout();
			this._scrollViewLogDetails.Progress = 1f;
		}

		private void ShowLogDetailsPanel()
		{
			if (this._scrollViewLogDetails.Visibility)
			{
				return;
			}
			this._scrollViewLogDetails.Show();
			this._logDetailPaneCloseBtn.Show();
			this._logDetailPaneBackground.Show();
			this._scrollView.LayoutStyle.bottomRightMargin.y = 140f;
			this._scrollView.RefreshLayout();
		}

		private void HideLogDetailsPanel()
		{
			if (!this._scrollViewLogDetails.Visibility)
			{
				return;
			}
			this._scrollViewLogDetails.Hide();
			this._logDetailPaneCloseBtn.Hide();
			this._logDetailPaneBackground.Hide();
			this._scrollView.LayoutStyle.bottomRightMargin.y = 40f;
			this._scrollView.RefreshLayout();
		}

		private static int ComputeLogHash(string content, string stackTrace)
		{
			HashCode hashCode = default(HashCode);
			hashCode.Add<int>(content.GetHashCode());
			hashCode.Add<int>(stackTrace.GetHashCode());
			return hashCode.ToHashCode();
		}

		internal void SetPanelPosition(RuntimeSettings.DistanceOption distanceOption, bool skipAnimation = false)
		{
			ValueContainer<Vector3> valueContainer = ValueContainer<Vector3>.Load("ConsolePanelPositions");
			Vector3 targetPosition;
			if (distanceOption != RuntimeSettings.DistanceOption.Close)
			{
				if (distanceOption != RuntimeSettings.DistanceOption.Far)
				{
					targetPosition = valueContainer["Default"];
				}
				else
				{
					targetPosition = valueContainer["Far"];
				}
			}
			else
			{
				targetPosition = valueContainer["Close"];
			}
			this._targetPosition = targetPosition;
			if (skipAnimation)
			{
				base.SphericalCoordinates = this._targetPosition;
				this._currentPosition = this._targetPosition;
				return;
			}
			this._lerpCompleted = false;
		}

		private const int NumberOfLines = 14;

		private const int FullLogPanelBottomMargin = 40;

		private const int ContractedLogPanelBottomMargin = 140;

		private ScrollView _scrollView;

		private ScrollView _scrollViewLogDetails;

		private ProxyFlex<ConsoleLine, ProxyConsoleLine> _proxyFlex;

		private Flex _flex;

		private Flex _buttonsAnchor;

		private List<SeverityEntry> _severities = new List<SeverityEntry>();

		private Dictionary<LogType, SeverityEntry> _severitiesPerType = new Dictionary<LogType, SeverityEntry>();

		private readonly List<LogEntry> _entries = new List<LogEntry>();

		private readonly List<LogEntry> _allEntries = new List<LogEntry>();

		private readonly Dictionary<int, LogEntry> _entryMap = new Dictionary<int, LogEntry>();

		private Label _logDetailLabel;

		private Toggle _collapseBtn;

		private Texture2D _collapseActiveIcon;

		private Texture2D _collapseInactiveIcon;

		private ButtonWithIcon _logDetailPaneCloseBtn;

		private Vector3 _currentPosition;

		private Vector3 _targetPosition;

		private readonly float _lerpSpeed = 10f;

		private bool _lerpCompleted = true;

		private Background _logDetailPaneBackground;

		private ImageStyle _logDetailPaneBackgroundImageStyle;
	}
}
