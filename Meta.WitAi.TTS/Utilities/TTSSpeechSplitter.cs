using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Meta.WitAi.TTS.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Meta.WitAi.TTS.Utilities
{
	public class TTSSpeechSplitter : MonoBehaviour, ISpeakerTextPreprocessor
	{
		public void OnPreprocessTTS(TTSSpeaker speaker, List<string> phrases)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int i = 0;
			while (i < phrases.Count)
			{
				string text = this._cleaner.Replace(phrases[i], " ");
				if (text.Length <= this.MaxTextLength)
				{
					phrases[i] = text;
					i++;
				}
				else
				{
					phrases.RemoveAt(i);
					foreach (string text2 in this._sentenceSplitter.Split(text))
					{
						if (text2.Length != 0)
						{
							if (stringBuilder.Length > 0 && stringBuilder.Length + text2.Length > this.MaxTextLength)
							{
								phrases.Insert(i, stringBuilder.ToString().Trim());
								stringBuilder.Clear();
								i++;
							}
							if (text2.Length <= this.MaxTextLength)
							{
								stringBuilder.Append(text2);
							}
							else
							{
								foreach (string text3 in this._wordSplitter.Split(text2))
								{
									if (text3.Length != 0)
									{
										if (stringBuilder.Length > 0 && stringBuilder.Length + text3.Length > this.MaxTextLength)
										{
											phrases.Insert(i, stringBuilder.ToString().Trim());
											stringBuilder.Clear();
											i++;
										}
										if (stringBuilder.Length == 0)
										{
											text3 = text3.TrimStart();
										}
										if (text3.Length <= this.MaxTextLength)
										{
											stringBuilder.Append(text3);
										}
										else
										{
											stringBuilder.Append(text3.Substring(0, this.MaxTextLength));
											VLog.W(string.Format("Word is longer than MaxTextLength & will be truncated\nWord: {0}\nTruncated: {1}\nFrom Length: {2}\nTo Length: {3}", new object[]
											{
												text3,
												stringBuilder,
												text3.Length,
												this.MaxTextLength
											}), null);
											phrases.Insert(i, stringBuilder.ToString());
											stringBuilder.Clear();
											i++;
										}
									}
								}
							}
						}
					}
					if (stringBuilder.Length > 0)
					{
						phrases.Insert(i, stringBuilder.ToString().Trim());
						stringBuilder.Clear();
						i++;
					}
				}
			}
		}

		[Tooltip("If text-to-speech phrase is greater than this length, it will be split.")]
		[Range(10f, 250f)]
		[FormerlySerializedAs("maxTextLength")]
		public int MaxTextLength = 250;

		private Regex _cleaner = new Regex("\\s\\s+|</?s>|</?p>", RegexOptions.Multiline | RegexOptions.Compiled);

		private Regex _sentenceSplitter = new Regex("(?<=[.?,;!]\\s+|<p>|<s>)", RegexOptions.Compiled);

		private Regex _wordSplitter = new Regex("(?=\\s+)", RegexOptions.Compiled);
	}
}
