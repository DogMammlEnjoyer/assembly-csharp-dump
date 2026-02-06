using System;
using System.Collections.Generic;
using Fusion.Statistics;
using UnityEngine;

namespace Fusion
{
	internal class ClientTimeProvider : ITimeProvider
	{
		internal ClientTimeProvider() : this(ClientTimeProviderSettings.Default())
		{
		}

		internal ClientTimeProvider(ClientTimeProviderSettings settings)
		{
			this._settings = settings;
			this._resetInputTimeCallbacks = new List<TimeProviderCallback>();
			this._resetSimulationTimeCallbacks = new List<TimeProviderCallback>();
			this._resetInterpTimeCallbacks = new List<TimeProviderCallback>();
			this.Initialize();
		}

		internal void OnReset(Clock clock, TimeProviderCallback callback)
		{
			switch (clock)
			{
			case Clock.Input:
				this._resetInputTimeCallbacks.Add(callback);
				break;
			case Clock.Local:
				this._resetSimulationTimeCallbacks.Add(callback);
				break;
			case Clock.Remote:
				this._resetInterpTimeCallbacks.Add(callback);
				break;
			}
		}

		private void Configure(TimeSyncConfiguration tsc)
		{
			this._settings.SampleWindowSeconds = tsc.SampleWindowSecondsNormalized;
			this._settings.TimeScaleOffsetMax = tsc.MaxSimSpeedAdjustNormalized;
			this._settings.OutgoingQuantile = 1.0 - tsc.MaxLateInputsNormalized;
			this._settings.IncomingQuantile = 1.0 - tsc.MaxLateSnapshotsNormalized;
			this._settings.OutgoingRedundancy = (double)tsc.RedundantInputsNormalized;
			this._settings.IncomingRedundancy = (double)tsc.RedundantSnapshotsNormalized;
			this.Initialize();
		}

		private void Configure(SimulationRuntimeConfig src)
		{
			this._settings.OutgoingSendRate = src.TickRate.ClientSend;
			this._settings.IncomingSendRate = src.TickRate.ServerSend;
			this._settings.OutgoingSendDelta = src.TickRate.ClientSendDelta;
			this._settings.IncomingSendDelta = src.TickRate.ServerSendDelta;
			this._settings.ClientTickRate = src.TickRate.Client;
			this._settings.ClientSimDeltaTime = src.TickRate.ClientTickDelta;
			this._settings.ServerTickRate = src.TickRate.Server;
			this._settings.ServerSimDeltaTime = src.TickRate.ServerTickDelta;
			this._settings.PredictionMax = double.MaxValue;
			this._settings.InputDelayMin = 0.0;
			this._settings.InputDelayMax = 0.0;
			this.Initialize();
		}

		private void Initialize()
		{
			double timeScaleOffsetMax = this._settings.TimeScaleOffsetMax;
			this._clockFeedback = new VariableFeedback(0.3, 0.0, 0.0, -timeScaleOffsetMax, timeScaleOffsetMax);
			this._delayFeedback = new VariableFeedback(0.3, 0.0, 0.0, -timeScaleOffsetMax, timeScaleOffsetMax);
			this._interpFeedback = new VariableFeedback(0.3, 0.0, 0.0, -timeScaleOffsetMax, timeScaleOffsetMax);
			this._clockSyncTimer = default(Timer);
			this._snapshotTimer = default(Timer);
			this._resetInputTimer = default(Timer);
			this._resetSimulationTimer = default(Timer);
			this._resetInterpTimer = default(Timer);
			this._sampleTimer = default(Timer);
			this._roundTripTime = new TimeSeries((int)((double)this._settings.IncomingSendRate * this._settings.SampleWindowSeconds));
			this._inputOffset = new TimeSeries((int)((double)this._settings.IncomingSendRate * this._settings.SampleWindowSeconds));
			this._snapshotTimeDelta = new TimeSeries((int)((double)this._settings.IncomingSendRate * this._settings.SampleWindowSeconds));
			this._interpDelay = new TimeSeries((int)((double)this._settings.IncomingSendRate * this._settings.SampleWindowSeconds));
			this._inputOffsetAdjust = new RingBuffer<TimeAdjustment>(this._settings.ClientTickRate);
			this._frameTimeDeltaHist = new Histogram();
			this._frameTimeDeltaHistDecay = new ExponentialDecay(0.5, 2.0);
			this._snapshotTimeDeltaHist = new Histogram();
			this._snapshotTimeDeltaHistDecay = new ExponentialDecay(0.5, 2.0);
		}

		private void Reset(double roundTripTime, Tick snapshot, double time, double timeScale)
		{
			bool trace = this._trace;
			if (trace)
			{
				this._timeTrace.OnPacket(snapshot, this._snapshotTimer.ElapsedInSeconds, roundTripTime);
			}
			this._clockFeedback.Reset();
			this._delayFeedback.Reset();
			this._interpFeedback.Reset();
			this._roundTripTime.Clear();
			this._inputOffset.Clear();
			this._interpDelay.Clear();
			this._interpDelay.Fill(this._settings.IncomingSendDelta);
			this._snapshotTimeDelta.Clear();
			this._snapshotTimeDelta.Fill(this._settings.IncomingSendDelta);
			this._snapshotTimeDeltaHist.Clear();
			this._snapshotTimeDeltaHist.Record(this._settings.IncomingSendDelta, (double)this._settings.IncomingSendRate);
			this._frameTimeDeltaHist.Clear();
			this._frameTimeDeltaHist.Record(0.025, 50.0);
			this._frameTimeDeltaChecked = false;
			this._frameTimeDeltaOutliersSeen = false;
			this._clockSyncTimer.Reset();
			this._snapshotTimer.Reset();
			this._resetInputTimer.Reset();
			this._resetSimulationTimer.Reset();
			this._resetInterpTimer.Reset();
			this._sampleTimer.Restart();
			this._inputTimeResetCount = 0;
			this._simulationTimeResetCount = 0;
			this._interpTimeResetCount = 0;
			this._serverFeedback = new Simulation.TimeFeedback(0.0, 0.025, 0.0, 0.025);
			this.UpdateSnapshot(snapshot);
			this.UpdateServerStats(roundTripTime, time, timeScale);
			this.UpdateIncomingTargets(true);
			this.UpdateOutgoingTargets(true);
			this.Snap();
			this._isRunning = true;
		}

		private void Snap()
		{
			this.ResetInputTime();
			this.ResetSimulationTime();
			this.ResetInterpolationTime(false);
		}

		private void ResetInputTime()
		{
			this._inputOffsetAdjust.Clear();
			this._lastInputOffsetAdjustTick = this._snapshot;
			this._totalInputOffsetAdjust = 0.0;
			this._clockFeedback.Reset();
			this._clockTimeScaleOffset = 0.0;
			double inputTime = this._inputTime;
			double inputTime2 = this.GetServerTime() + this._targetInputOffset;
			this._inputTime = inputTime2;
			this._latestInputOffset = this._targetInputOffset;
			this._inputTimeResetCount++;
			this._inputTimeReset = true;
			this._resetInputTimer.Restart();
			foreach (TimeProviderCallback timeProviderCallback in this._resetInputTimeCallbacks)
			{
				timeProviderCallback();
			}
		}

		private void ResetSimulationTime()
		{
			this._delayFeedback.Reset();
			this._delayTimeScaleOffset = 0.0;
			double simulationTime = this._simulationTime;
			double num = this._inputTime - this._targetInputDelay;
			TraceLogStream logTraceTime = InternalLogStreams.LogTraceTime;
			if (logTraceTime != null)
			{
				logTraceTime.Log(string.Format("[P{0}] (re)setting client local time from {1:f3} to {2:f3}", this._playerIndex, simulationTime, num));
			}
			this._simulationTime = num;
			this._latestInputDelay = this._targetInputDelay;
			this._simulationTimeResetCount++;
			this._simulationTimeReset = true;
			this._resetSimulationTimer.Restart();
			foreach (TimeProviderCallback timeProviderCallback in this._resetSimulationTimeCallbacks)
			{
				timeProviderCallback();
			}
		}

		private void ResetInterpolationTime(bool resetMayBeCausedByFalseOutlier = false)
		{
			double snapshotTime = this.GetSnapshotTime();
			double interpTime = this._interpTime;
			double num = snapshotTime - this._targetInterpDelay;
			bool flag = resetMayBeCausedByFalseOutlier && num < interpTime && interpTime < snapshotTime;
			if (flag)
			{
				TraceLogStream logTraceTime = InternalLogStreams.LogTraceTime;
				if (logTraceTime != null)
				{
					logTraceTime.Log(string.Format("[P{0}] canceled client remote time reset because time would have jumped backwards from {1:f3} to {2:f3}, even though {3:f3} was still behind the latest snapshot ({4:f3})", new object[]
					{
						this._playerIndex,
						interpTime,
						num,
						interpTime,
						snapshotTime
					}));
				}
				this._snapshotTimeDelta.Clear();
				this._snapshotTimeDelta.Fill(this._settings.IncomingSendDelta);
				this._snapshotTimeDeltaHist.Clear();
				this._snapshotTimeDeltaHist.Record(this._settings.IncomingSendDelta, (double)this._settings.IncomingSendRate);
				this.UpdateIncomingTargets(true);
			}
			else
			{
				TraceLogStream logTraceTime2 = InternalLogStreams.LogTraceTime;
				if (logTraceTime2 != null)
				{
					logTraceTime2.Log(string.Format("[P{0}] (re)setting client remote time from {1:f3} to {2:f3}", this._playerIndex, interpTime, num));
				}
				this._interpFeedback.Reset();
				this._interpTimeScaleOffset = 0.0;
				this._interpTime = num;
				this._latestInterpDelay = this._targetInterpDelay;
				this._interpTimeResetCount++;
				this._interpTimeReset = true;
				this._resetInterpTimer.Restart();
				foreach (TimeProviderCallback timeProviderCallback in this._resetInterpTimeCallbacks)
				{
					timeProviderCallback();
				}
			}
		}

		private double GetInputOffsetLegacy()
		{
			double inputTime = this._inputTime;
			double serverTime = this.GetServerTime();
			return inputTime - serverTime;
		}

		private double GetInputOffset()
		{
			return (double)this._serverFeedback.OffsetAvg + this._totalInputOffsetAdjust;
		}

		private void AddInputOffsetAdjustment(double amount)
		{
			Tick tick = (int)(this._inputTime * (double)this._settings.ClientTickRate);
			bool flag = this._inputOffsetAdjust.IsEmpty || tick != this._lastInputOffsetAdjustTick;
			if (flag)
			{
				this._inputOffsetAdjust.PushBack(new TimeAdjustment(tick, 0.0));
			}
			try
			{
				this._inputOffsetAdjust.BackMut().Total += amount;
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(error);
				}
				this._totalInputOffsetAdjust = 0.0;
				this._inputOffsetAdjust = new RingBuffer<TimeAdjustment>(this._settings.ClientTickRate);
				this._inputOffsetAdjust.PushBack(new TimeAdjustment(tick, amount));
			}
			this._totalInputOffsetAdjust += amount;
			this._lastInputOffsetAdjustTick = tick;
		}

		private void RemoveInputOffsetAdjustmentsOlderThan(Tick snapshot)
		{
			this._totalInputOffsetAdjust = 0.0;
			foreach (TimeAdjustment timeAdjustment in this._inputOffsetAdjust)
			{
				bool flag = timeAdjustment.Tick >= snapshot;
				if (flag)
				{
					this._totalInputOffsetAdjust += timeAdjustment.Total;
				}
			}
		}

		private double GetServerTime()
		{
			double num = this._roundTripTime.Smoothed(0.25);
			double num2 = this._clockSyncTimer.ElapsedInSeconds + num;
			num2 *= this._lastSeenServerTimeScale;
			return this._lastSeenServerTime + num2;
		}

		private double GetSnapshotTime()
		{
			double num = this._snapshotTimer.ElapsedInSeconds;
			num *= this._lastSeenServerTimeScale;
			return (double)this._snapshot * this._settings.ClientSimDeltaTime + num;
		}

		private void UpdateServerStats(double roundTripTime, double time, double timeScale)
		{
			Assert.Check(roundTripTime >= 0.0);
			Assert.Check(time >= 0.0);
			Assert.Check(timeScale >= 0.0);
			double num = time - this._lastSeenServerTime;
			this._lastSeenServerTime = time;
			this._lastSeenServerTimeScale = timeScale;
			bool flag = num <= 0.0;
			if (!flag)
			{
				this._clockSyncTimer.Restart();
				this._roundTripTime.Add(roundTripTime);
				double inputOffset = this.GetInputOffset();
				this._inputOffset.Add(inputOffset);
				this.UpdateOutgoingTargets(false);
			}
		}

		private void FrameTimeDeltaCheck(double dt)
		{
			bool frameTimeDeltaChecked = this._frameTimeDeltaChecked;
			if (!frameTimeDeltaChecked)
			{
				bool flag = this.FrameTimeDeltaSeemsLikeAnExtremeOutlier(dt);
				if (flag)
				{
					this._frameTimeDeltaOutliersSeen = true;
				}
				this._frameTimeDeltaHist.Rescale(this._frameTimeDeltaHistDecay.Calculate(dt));
				this._frameTimeDeltaHist.Record(dt);
				this._frameTimeDeltaChecked = true;
			}
		}

		private void FrameTimeDeltaCheckReset()
		{
			this._frameTimeDeltaChecked = false;
		}

		private bool FrameTimeDeltaSeemsLikeAnExtremeOutlier(double dt)
		{
			bool flag = this._frameTimeDeltaHist.Count <= 1.0;
			return !flag && ((dt > 0.1 && dt > this._frameTimeDeltaHist.Quantile(0.95)) || dt > 0.25);
		}

		private void UpdateSnapshot(Tick snapshot)
		{
			Assert.Check(snapshot >= 0);
			bool flag = this._snapshot == snapshot;
			if (!flag)
			{
				this._latestSnapshotIsOutlier = this._frameTimeDeltaOutliersSeen;
				bool flag2 = !this._latestSnapshotIsOutlier && this._snapshotExceededFrames > 0;
				if (flag2)
				{
					TraceLogStream logTraceTime = InternalLogStreams.LogTraceTime;
					if (logTraceTime != null)
					{
						logTraceTime.Log(string.Format("[P{0}] before receiving snapshot {1}, remote time went past snapshot {2} for {3} frame(s) by up to {4:f3} seconds", new object[]
						{
							this._playerIndex,
							snapshot,
							this._snapshot,
							this._snapshotExceededFrames,
							this._snapshotExceededTime
						}));
					}
				}
				this._snapshotExceededFrames = 0;
				this._snapshotExceededTime = 0.0;
				this.RemoveInputOffsetAdjustmentsOlderThan(snapshot);
				this._snapshot = snapshot;
				double elapsedInSeconds = this._snapshotTimer.ElapsedInSeconds;
				this._snapshotTimer.Restart();
				double value = elapsedInSeconds;
				bool frameTimeDeltaOutliersSeen = this._frameTimeDeltaOutliersSeen;
				if (frameTimeDeltaOutliersSeen)
				{
					this._frameTimeDeltaOutliersSeen = false;
					value = Math.Min(elapsedInSeconds, this._snapshotTimeDeltaHist.Quantile(0.95));
					TraceLogStream logTraceTime2 = InternalLogStreams.LogTraceTime;
					if (logTraceTime2 != null)
					{
						logTraceTime2.Log(string.Format("[P{0}] rejecting sample of {1:f3} seconds elapsed since receiving last snapshot because it was influenced by unusual frame lag", this._playerIndex, elapsedInSeconds));
					}
				}
				this._snapshotTimeDelta.Add(value);
				this._snapshotTimeDeltaHist.Rescale(this._snapshotTimeDeltaHistDecay.Calculate(elapsedInSeconds));
				this._snapshotTimeDeltaHist.Record(value);
				this.UpdateIncomingTargets(false);
			}
		}

		private void SaveInterpDelaySample(double interpDelay)
		{
			bool flag = this._sampleTimer.ElapsedInSeconds < this._settings.IncomingSendDelta;
			if (!flag)
			{
				bool flag2 = this._frameTimeDeltaOutliersSeen || this._latestSnapshotIsOutlier;
				if (!flag2)
				{
					this._interpDelay.Add(interpDelay);
					this._sampleTimer.Restart();
				}
			}
		}

		private double RoundToNearestMultiple(double x, double round, bool minimumOne = false)
		{
			double num = Math.Round(x / round);
			num = ((minimumOne && num < 1.0) ? 1.0 : num);
			return num * round;
		}

		private void UpdateOutgoingTargets(bool snap = false)
		{
			double num = this._roundTripTime.Smoothed(0.25);
			double predictionMax = this._settings.PredictionMax;
			double num2 = Math.Max(0.0, num - predictionMax - this._settings.InputDelayMin);
			double num3 = this._settings.InputDelayMin + num2;
			num3 *= this._lastSeenServerTimeScale;
			if (snap)
			{
				this._targetInputDelay = num3;
			}
			else
			{
				this._targetInputDelay = Maths.Lerp(this._targetInputDelay, num3, 0.2);
			}
			double num4 = TimeSeries.InverseCdfNormal(this._settings.OutgoingQuantile);
			double num5 = this.RoundToNearestMultiple((double)this._serverFeedback.RecvDeltaAvg, this._settings.OutgoingSendDelta, true);
			double num6 = Math.Max((double)this._serverFeedback.RecvDeltaDev, this._roundTripTime.MedianAbsDev);
			double num7 = num5 * this._settings.OutgoingRedundancy + num4 * num6;
			num7 *= this._lastSeenServerTimeScale;
			if (snap)
			{
				this._targetInputOffset = num7;
			}
			else
			{
				this._targetInputOffset = Maths.Lerp(this._targetInputOffset, num7, 0.2);
			}
		}

		private void UpdateIncomingTargets(bool snap = false)
		{
			double num = this._snapshotTimeDeltaHist.Quantile(this._settings.IncomingQuantile);
			double num2 = (1.0 + this._settings.IncomingRedundancy) * num;
			num2 *= this._lastSeenServerTimeScale;
			if (snap)
			{
				this._targetInterpDelay = num2;
			}
			else
			{
				this._targetInterpDelay = Maths.Lerp(this._targetInterpDelay, num2, 0.2);
			}
		}

		private void Update(double unscaledDeltaTime)
		{
			bool flag = !this._isRunning;
			if (!flag)
			{
				bool flag2 = this._trace && this._timeTrace != null;
				if (flag2)
				{
					this._timeTrace.OnFrame(unscaledDeltaTime);
				}
				bool flag3 = unscaledDeltaTime > 1.0;
				if (flag3)
				{
					TraceLogStream logTraceTime = InternalLogStreams.LogTraceTime;
					if (logTraceTime != null)
					{
						logTraceTime.Log(string.Format("[P{0}] time will not advance this frame because its dt={1:f3} is too big", this._playerIndex, unscaledDeltaTime));
					}
					this.FrameTimeDeltaCheckReset();
				}
				else
				{
					this.FrameTimeDeltaCheck(unscaledDeltaTime);
					double lastSeenServerTimeScale = this._lastSeenServerTimeScale;
					double num = lastSeenServerTimeScale + this._clockTimeScaleOffset;
					double num2 = lastSeenServerTimeScale + this._clockTimeScaleOffset + this._delayTimeScaleOffset;
					double num3 = lastSeenServerTimeScale + this._interpTimeScaleOffset;
					this._inputTime += Math.Max(0.0, num * unscaledDeltaTime);
					this._simulationTime += Math.Max(0.0, num2 * unscaledDeltaTime);
					this._interpTime += Math.Max(0.0, num3 * unscaledDeltaTime);
					double inputOffset = this.GetInputOffset();
					double b = this._inputTime - this._simulationTime;
					double num4 = this.GetSnapshotTime() - this._interpTime;
					this.SaveInterpDelaySample(num4);
					this._latestInputOffset = Maths.Lerp(this._latestInputOffset, inputOffset, 0.5);
					this._latestInputDelay = Maths.Lerp(this._latestInputDelay, b, 0.5);
					this._latestInterpDelay = Maths.Lerp(this._latestInterpDelay, num4, this._latestSnapshotIsOutlier ? 0.125 : 0.5);
					double num5 = this._targetInputOffset - this._latestInputOffset;
					double num6 = -(this._targetInputDelay - this._latestInputDelay);
					double num7 = -(this._targetInterpDelay - this._latestInterpDelay);
					double num8 = 0.0;
					double num9 = 0.0;
					double num10 = 0.0;
					this._inputTimeReset = false;
					this._simulationTimeReset = false;
					this._interpTimeReset = false;
					bool isRunning = this._resetInputTimer.IsRunning;
					if (isRunning)
					{
						bool flag4 = this._resetInputTimer.ElapsedInSeconds > 1.0;
						if (flag4)
						{
							this._resetInputTimer.Reset();
							this._resetSimulationTimer.Reset();
						}
					}
					bool flag5 = !this._resetInputTimer.IsRunning && (num5 < -0.5 || num5 > 1.0);
					if (flag5)
					{
						this.ResetInputTime();
						this.ResetSimulationTime();
					}
					else
					{
						bool flag6 = num5 > 0.05;
						if (flag6)
						{
							num8 = num5 * 0.125;
						}
					}
					bool isRunning2 = this._resetSimulationTimer.IsRunning;
					if (isRunning2)
					{
						bool flag7 = this._resetSimulationTimer.ElapsedInSeconds > 1.0;
						if (flag7)
						{
							this._resetSimulationTimer.Reset();
						}
					}
					bool flag8 = !this._resetSimulationTimer.IsRunning && (num6 < -0.5 || num6 > 1.0);
					if (flag8)
					{
						this.ResetSimulationTime();
					}
					else
					{
						bool flag9 = num6 > 0.05;
						if (flag9)
						{
							num9 = num6 * 0.125;
						}
					}
					bool isRunning3 = this._resetInterpTimer.IsRunning;
					if (isRunning3)
					{
						bool flag10 = this._resetInterpTimer.ElapsedInSeconds > 1.0;
						if (flag10)
						{
							this._resetInterpTimer.Reset();
						}
					}
					bool flag11 = !this._resetInterpTimer.IsRunning && (num7 < -0.5 || num7 > 1.0);
					if (flag11)
					{
						this.ResetInterpolationTime(true);
					}
					else
					{
						bool flag12 = num7 > 0.05;
						if (flag12)
						{
							num10 = num7 * 0.125;
						}
					}
					this._inputTime += Math.Max(0.0, num8);
					this._simulationTime += Math.Max(0.0, num8 + num9);
					this._interpTime += Math.Max(0.0, num10);
					this._latestInputOffset += num8;
					this._latestInputDelay -= num9;
					this._latestInterpDelay -= num10;
					double amount = this._clockTimeScaleOffset * unscaledDeltaTime + num8;
					this.AddInputOffsetAdjustment(amount);
					this._clockFeedback.Update(this._latestInputOffset, this._targetInputOffset, unscaledDeltaTime);
					this._delayFeedback.Update(this._latestInputDelay, this._targetInputDelay, unscaledDeltaTime);
					this._interpFeedback.Update(this._latestInterpDelay, this._targetInterpDelay, unscaledDeltaTime);
					this._clockTimeScaleOffset = this._clockFeedback.Output();
					this._delayTimeScaleOffset = -this._delayFeedback.Output();
					this._interpTimeScaleOffset = -this._interpFeedback.Output();
					double num11 = (double)this._snapshot * this._settings.ClientSimDeltaTime;
					bool flag13 = this._interpTime > num11;
					if (flag13)
					{
						this._snapshotExceededFrames++;
						this._snapshotExceededTime = Math.Max(this._snapshotExceededTime, this._interpTime - num11);
					}
					this.FrameTimeDeltaCheckReset();
				}
			}
		}

		bool ITimeProvider.IsRunning()
		{
			return this._isRunning;
		}

		void ITimeProvider.Configure(SimulationRuntimeConfig src)
		{
			this.Configure(src);
		}

		void ITimeProvider.Configure(TimeSyncConfiguration tsc)
		{
			this.Configure(tsc);
		}

		void ITimeProvider.Reset(double roundTripTime, Tick snapshot)
		{
			double time = (double)snapshot * this._settings.ClientSimDeltaTime;
			double timeScale = 1.0;
			this.Reset(roundTripTime, snapshot, time, timeScale);
		}

		void ITimeProvider.Snap()
		{
			this.Snap();
		}

		void ITimeProvider.Update(double unscaledDeltaTime)
		{
			this.Update(unscaledDeltaTime);
		}

		void ITimeProvider.OnSnapshotReceived(double roundTripTime, Tick snapshot)
		{
			bool flag = this._trace && this._timeTrace != null;
			if (flag)
			{
				this._timeTrace.OnPacket(snapshot, this._snapshotTimer.ElapsedInSeconds, roundTripTime);
			}
			double time = (double)snapshot * this._settings.ClientSimDeltaTime;
			double timeScale = 1.0;
			this.FrameTimeDeltaCheck((double)Time.unscaledDeltaTime);
			this.UpdateSnapshot(snapshot);
			this.UpdateServerStats(roundTripTime, time, timeScale);
		}

		void ITimeProvider.OnFeedbackReceived(Simulation.TimeFeedback feedback)
		{
			bool flag = this._trace && this._timeTrace != null;
			if (flag)
			{
				this._timeTrace.OnFeedback(feedback);
			}
			this._serverFeedback.OffsetAvg = Maths.Lerp(this._serverFeedback.OffsetAvg, feedback.OffsetAvg, 0.5f);
			this._serverFeedback.OffsetDev = Maths.Lerp(this._serverFeedback.OffsetDev, feedback.OffsetDev, 0.5f);
			this._serverFeedback.RecvDeltaAvg = Maths.Lerp(this._serverFeedback.RecvDeltaAvg, feedback.RecvDeltaAvg, 0.5f);
			this._serverFeedback.RecvDeltaDev = Maths.Lerp(this._serverFeedback.RecvDeltaDev, feedback.RecvDeltaDev, 0.5f);
		}

		void ITimeProvider.ResetFeedback()
		{
			this._serverFeedback = new Simulation.TimeFeedback(0.0, 0.025, 0.0, 0.025);
		}

		Instant ITimeProvider.Now()
		{
			return new Instant
			{
				Input = this._inputTime,
				Local = this._simulationTime,
				Remote = this._interpTime
			};
		}

		void ITimeProvider.Log(FusionStatisticsManager stats)
		{
			EngineProfiler.InputRecvDelta(this._serverFeedback.RecvDeltaAvg);
			EngineProfiler.InputRecvDeltaDeviation(this._serverFeedback.RecvDeltaDev);
			EngineProfiler.StateRecvDelta((float)this._snapshotTimeDelta.Avg);
			EngineProfiler.StateRecvDeltaDeviation((float)this._snapshotTimeDelta.Dev);
			EngineProfiler.SimulationOffset(this._serverFeedback.OffsetAvg);
			EngineProfiler.SimulationOffsetDeviation(this._serverFeedback.OffsetDev);
			double lastSeenServerTimeScale = this._lastSeenServerTimeScale;
			double num = lastSeenServerTimeScale + this._clockTimeScaleOffset + this._delayTimeScaleOffset;
			EngineProfiler.SimulationSpeed((float)num);
			EngineProfiler.InterpolationOffset((float)this._interpDelay.Avg);
			EngineProfiler.InterpolationOffsetDeviation((float)this._interpDelay.Dev);
			double num2 = lastSeenServerTimeScale + this._interpTimeScaleOffset;
			EngineProfiler.InterpolationSpeed((float)num2);
			bool flag = this._inputTimeReset | this._simulationTimeReset | this._interpTimeReset;
			stats.PendingSnapshot.AddToInputReceiveDeltaStat(this._serverFeedback.RecvDeltaAvg, true);
			stats.PendingSnapshot.AddToTimeResetsStat(flag ? 1 : 0, false);
			stats.PendingSnapshot.AddToStateReceiveDeltaStat((float)this._snapshotTimeDelta.Avg, true);
			stats.PendingSnapshot.AddToSimulationTimeOffsetStat(this._serverFeedback.OffsetAvg, true);
			stats.PendingSnapshot.AddToSimulationSpeedStat((float)num, true);
			stats.PendingSnapshot.AddToInterpolationOffsetStat((float)this._interpDelay.Avg, true);
			stats.PendingSnapshot.AddToInterpolationSpeedStat((float)num2, true);
		}

		void ITimeProvider.SetPlayerIndex(int index)
		{
			this._playerIndex = index;
		}

		void ITimeProvider.StartTrace()
		{
			TickRate.Resolved tickRate = new TickRate.Resolved(this._settings.ClientTickRate, this._settings.OutgoingSendRate, this._settings.ServerTickRate, this._settings.IncomingSendRate);
			this._timeTrace = new ClientTimeTrace(this._playerIndex, tickRate);
			this._trace = true;
		}

		void ITimeProvider.StopTrace()
		{
			this._trace = false;
		}

		private ClientTimeProviderSettings _settings;

		private IFeedbackController _clockFeedback;

		private double _latestInputOffset;

		private double _targetInputOffset;

		private double _clockTimeScaleOffset;

		private IFeedbackController _delayFeedback;

		private double _latestInputDelay;

		private double _targetInputDelay;

		private double _delayTimeScaleOffset;

		private IFeedbackController _interpFeedback;

		private double _latestInterpDelay;

		private double _targetInterpDelay;

		private double _interpTimeScaleOffset;

		private double _inputTime;

		private double _simulationTime;

		private double _interpTime;

		private TimeSeries _roundTripTime;

		private TimeSeries _inputOffset;

		private TimeSeries _interpDelay;

		private Histogram _frameTimeDeltaHist;

		private ExponentialDecay _frameTimeDeltaHistDecay;

		private bool _frameTimeDeltaChecked;

		private bool _frameTimeDeltaOutliersSeen;

		private bool _latestSnapshotIsOutlier;

		private Histogram _snapshotTimeDeltaHist;

		private ExponentialDecay _snapshotTimeDeltaHistDecay;

		private Timer _snapshotTimer;

		private TimeSeries _snapshotTimeDelta;

		private Tick _snapshot;

		private Timer _clockSyncTimer;

		private double _lastSeenServerTime;

		private double _lastSeenServerTimeScale;

		private Timer _sampleTimer;

		private Timer _resetInputTimer;

		private Timer _resetSimulationTimer;

		private Timer _resetInterpTimer;

		private int _inputTimeResetCount;

		private int _simulationTimeResetCount;

		private int _interpTimeResetCount;

		private bool _inputTimeReset;

		private bool _simulationTimeReset;

		private bool _interpTimeReset;

		private readonly List<TimeProviderCallback> _resetInputTimeCallbacks;

		private readonly List<TimeProviderCallback> _resetSimulationTimeCallbacks;

		private readonly List<TimeProviderCallback> _resetInterpTimeCallbacks;

		private bool _isRunning;

		private int _playerIndex;

		private int _snapshotExceededFrames;

		private double _snapshotExceededTime;

		private Simulation.TimeFeedback _serverFeedback;

		private RingBuffer<TimeAdjustment> _inputOffsetAdjust;

		private Tick _lastInputOffsetAdjustTick;

		private double _totalInputOffsetAdjust;

		private bool _trace;

		private ClientTimeTrace _timeTrace;

		private const double RttSmoothingFactor = 0.25;

		private const float FeedbackSmoothingFactor = 0.5f;

		private const double InputOffsetSmoothingFactor = 0.5;

		private const double InputDelaySmoothingFactor = 0.5;

		private const double InterpDelaySmoothingFactor = 0.5;

		private const double InterpDelayTempSmoothingFactor = 0.125;

		private const double TargetInputOffsetSmoothingFactor = 0.2;

		private const double TargetInputDelaySmoothingFactor = 0.2;

		private const double TargetInterpDelaySmoothingFactor = 0.2;

		private const double SnapshotTimeDeltaHistogramDecayFraction = 0.5;

		private const double SnapshotTimeDeltaHistogramDecayTime = 2.0;

		private const double SnapshotTimeDeltaHighQuantile = 0.95;

		private const double FrameTimeDeltaHistogramDecayFraction = 0.5;

		private const double FrameTimeDeltaHistogramDecayTime = 2.0;

		private const double FrameTimeDeltaHighQuantile = 0.95;

		private const double FrameTimeDeltaOutlierTest1Quantile = 0.95;

		private const double FrameTimeDeltaOutlierTest1Threshold = 0.1;

		private const double FrameTimeDeltaOutlierTest2Threshold = 0.25;

		private const double InitialServerFeedbackJitter = 0.025;

		private const double PositiveBumpThreshold = 0.05;

		private const double BumpFraction = 0.125;

		private const double NegativeResetThreshold = -0.5;

		private const double PositiveResetThreshold = 1.0;

		private const double ResetCooldownSeconds = 1.0;

		private const double Kp = 0.3;

		private const double Ki = 0.0;

		private const double Kd = 0.0;
	}
}
