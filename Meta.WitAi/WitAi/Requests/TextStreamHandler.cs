using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine.Scripting;

namespace Meta.WitAi.Requests
{
	internal class TextStreamHandler : DownloadHandlerScript, IVRequestDownloadDecoder
	{
		public bool IsStarted { get; private set; }

		public event VRequestResponseDelegate OnFirstResponse;

		public event VRequestResponseDelegate OnResponse;

		public float Progress { get; private set; }

		public event VRequestProgressDelegate OnProgress;

		public bool IsComplete { get; private set; }

		public TaskCompletionSource<bool> Completion { get; } = new TaskCompletionSource<bool>();

		[Preserve]
		public TextStreamHandler(TextStreamHandler.TextStreamResponseDelegate partialResponseDelegate, string partialDelimiter = "\r\n", string finalDelimiter = "\n")
		{
			this._partialResponseDelegate = partialResponseDelegate;
			this._partialDelimiter = partialDelimiter;
			this._finalDelimiter = finalDelimiter;
		}

		[Preserve]
		protected override bool ReceiveData(byte[] receiveData, int dataLength)
		{
			if (!this.IsStarted)
			{
				this.IsStarted = true;
				VRequestResponseDelegate onFirstResponse = this.OnFirstResponse;
				if (onFirstResponse != null)
				{
					onFirstResponse();
				}
			}
			VRequestResponseDelegate onResponse = this.OnResponse;
			if (onResponse != null)
			{
				onResponse();
			}
			string[] array = TextStreamHandler.SplitText(TextStreamHandler.DecodeBytes(receiveData, 0, dataLength), this._partialDelimiter);
			for (int i = 0; i < array.Length; i++)
			{
				string value = array[i];
				if (!string.IsNullOrEmpty(value))
				{
					this._partialBuilder.Append(value);
					if (i < array.Length - 1)
					{
						this.HandlePartial(this._partialBuilder.ToString());
						this._partialBuilder.Clear();
					}
				}
			}
			this.RefreshProgress();
			return true;
		}

		protected virtual void HandlePartial(string newPartial)
		{
			TextStreamHandler.TextStreamResponseDelegate partialResponseDelegate = this._partialResponseDelegate;
			if (partialResponseDelegate != null)
			{
				partialResponseDelegate(newPartial);
			}
			if (this._finalBuilder.Length > 0)
			{
				this._finalBuilder.Append(this._finalDelimiter);
			}
			this._finalBuilder.Append(newPartial);
		}

		[Preserve]
		protected override string GetText()
		{
			return this._finalBuilder.ToString() + ((this._finalBuilder.Length > 0 && this._partialBuilder.Length > 0) ? this._finalDelimiter : "") + this._partialBuilder.ToString();
		}

		[Preserve]
		protected override void ReceiveContentLengthHeader(ulong contentLength)
		{
			base.ReceiveContentLengthHeader(contentLength);
			this._finalLength = TextStreamHandler.GetDecodedLength(contentLength);
		}

		private void RefreshProgress()
		{
			if (this._finalLength <= 0)
			{
				return;
			}
			float progress = this.GetProgress();
			if (!this.Progress.Equals(progress))
			{
				this.Progress = progress;
				VRequestProgressDelegate onProgress = this.OnProgress;
				if (onProgress == null)
				{
					return;
				}
				onProgress(progress);
			}
		}

		[Preserve]
		protected override float GetProgress()
		{
			if (this.IsComplete)
			{
				return 1f;
			}
			if (this._finalLength > 0)
			{
				return (float)(this._partialBuilder.Length + this._finalBuilder.Length) / (float)this._finalLength;
			}
			return 0f;
		}

		[Preserve]
		protected override byte[] GetData()
		{
			return Encoding.UTF8.GetBytes(this._finalBuilder.ToString());
		}

		[Preserve]
		protected override void CompleteContent()
		{
			if (this._partialBuilder.Length > 0)
			{
				this.HandlePartial(this._partialBuilder.ToString());
				this._partialBuilder.Clear();
			}
			this.IsComplete = true;
			this.Completion.TrySetResult(true);
		}

		public static string DecodeBytes(byte[] receiveData, int start, int length)
		{
			return Encoding.UTF8.GetString(receiveData, start, length);
		}

		public static int GetDecodedLength(ulong totalBits)
		{
			return (int)(totalBits / 8UL);
		}

		public static string[] SplitText(string source, string delimiter)
		{
			return source.Split(delimiter, StringSplitOptions.None);
		}

		internal bool ReceiveData(byte[] receiveData)
		{
			return this.ReceiveData(receiveData, receiveData.Length);
		}

		internal void Complete()
		{
			this.CompleteContent();
		}

		private TextStreamHandler.TextStreamResponseDelegate _partialResponseDelegate;

		private string _partialDelimiter = "\r\n";

		public const string DEFAULT_PARTIAL_DELIMITER = "\r\n";

		private string _finalDelimiter = "\n";

		public const string DEFAULT_FINAL_DELIMITER = "\n";

		private StringBuilder _partialBuilder = new StringBuilder();

		private StringBuilder _finalBuilder = new StringBuilder();

		private int _finalLength;

		public delegate void TextStreamResponseDelegate(string rawText);
	}
}
