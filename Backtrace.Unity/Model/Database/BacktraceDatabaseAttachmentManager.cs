using System;
using System.Collections.Generic;
using System.IO;
using Backtrace.Unity.Common;
using Backtrace.Unity.Types;
using UnityEngine;

namespace Backtrace.Unity.Model.Database
{
	internal class BacktraceDatabaseAttachmentManager
	{
		internal int ScreenshotMaxHeight { get; set; }

		internal int ScreenshotQuality { get; set; }

		public BacktraceDatabaseAttachmentManager(BacktraceDatabaseSettings settings)
		{
			this._settings = settings;
			this.ScreenshotMaxHeight = Screen.height;
			this.ScreenshotQuality = 25;
		}

		public IEnumerable<string> GetReportAttachments(BacktraceData data)
		{
			string uuidString = data.UuidString;
			List<string> list = new List<string>();
			try
			{
				this.AddIfPathIsNotEmpty(list, this.GetScreenshotPath(uuidString));
				this.AddIfPathIsNotEmpty(list, this.GetUnityPlayerLogFile(data, uuidString));
				this.AddIfPathIsNotEmpty(list, this.GetMinidumpPath(data, uuidString));
			}
			catch (Exception ex)
			{
				Debug.LogWarning(string.Format("Cannot generate report attachments. Reason: {0}", ex.Message));
			}
			return list;
		}

		private void AddIfPathIsNotEmpty(List<string> source, string attachmentPath)
		{
			if (!string.IsNullOrEmpty(attachmentPath))
			{
				source.Add(attachmentPath);
			}
		}

		private string GetMinidumpPath(BacktraceData backtraceData, string dataPrefix)
		{
			if (this._settings.MinidumpType == MiniDumpType.None)
			{
				return string.Empty;
			}
			string text = Path.Combine(this._settings.DatabasePath, string.Format("{0}-dump.dmp", dataPrefix));
			BacktraceReport report = backtraceData.Report;
			if (report == null)
			{
				return string.Empty;
			}
			MinidumpException exceptionType = report.ExceptionTypeReport ? MinidumpException.Present : MinidumpException.None;
			if (!MinidumpHelper.Write(text, this._settings.MinidumpType, exceptionType))
			{
				return string.Empty;
			}
			return text;
		}

		private string GetScreenshotPath(string dataPrefix)
		{
			if (!this._settings.GenerateScreenshotOnException)
			{
				return string.Empty;
			}
			string text = Path.Combine(this._settings.DatabasePath, string.Format("{0}-screen.jpg", dataPrefix));
			object @lock = this._lock;
			lock (@lock)
			{
				if (BacktraceDatabase.LastFrameTime == this._lastScreenTime)
				{
					if (File.Exists(this._lastScreenPath))
					{
						File.Copy(this._lastScreenPath, text);
						return text;
					}
					return this._lastScreenPath;
				}
				else
				{
					float num = (float)Screen.width / (float)Screen.height;
					bool flag2 = this.ScreenshotMaxHeight == Screen.height;
					int num2 = flag2 ? Screen.height : Mathf.Min(Screen.height, this.ScreenshotMaxHeight);
					int num3 = flag2 ? Screen.width : Mathf.RoundToInt((float)num2 * num);
					RenderTexture temporary = RenderTexture.GetTemporary(Screen.width, Screen.height);
					ScreenCapture.CaptureScreenshotIntoRenderTexture(temporary);
					RenderTexture temporary2 = RenderTexture.GetTemporary(num3, num2);
					if (SystemInfo.graphicsUVStartsAtTop)
					{
						Graphics.Blit(temporary, temporary2, new Vector2(1f, -1f), new Vector2(0f, 1f));
					}
					else
					{
						Graphics.Blit(temporary, temporary2);
					}
					RenderTexture active = RenderTexture.active;
					RenderTexture.active = temporary2;
					Texture2D texture2D = new Texture2D(num3, num2, TextureFormat.RGB24, false);
					texture2D.ReadPixels(new Rect(0f, 0f, (float)num3, (float)num2), 0, 0);
					texture2D.Apply();
					RenderTexture.active = active;
					RenderTexture.ReleaseTemporary(temporary2);
					RenderTexture.ReleaseTemporary(temporary);
					File.WriteAllBytes(text, texture2D.EncodeToJPG(this.ScreenshotQuality));
					Object.Destroy(texture2D);
					this._lastScreenTime = BacktraceDatabase.LastFrameTime;
					this._lastScreenPath = text;
				}
			}
			return text;
		}

		private string GetUnityPlayerLogFile(BacktraceData backtraceData, string dataPrefix)
		{
			if (!this._settings.AddUnityLogToReport)
			{
				return string.Empty;
			}
			string text = Path.Combine(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)).FullName, string.Format("LocalLow/{0}/{1}/Player.log", Application.companyName, Application.productName));
			if (string.IsNullOrEmpty(text) || !File.Exists(text))
			{
				return string.Empty;
			}
			string text2 = Path.Combine(this._settings.DatabasePath, string.Format("{0}-lg.log", dataPrefix));
			File.Copy(text, text2);
			return text2;
		}

		private readonly BacktraceDatabaseSettings _settings;

		private float _lastScreenTime;

		private string _lastScreenPath;

		private readonly object _lock = new object();
	}
}
