using System;
using System.Collections.Generic;

namespace Oculus.Assistant.VoiceCommand.Data
{
	public class VoiceCommandResult
	{
		public string ActionId
		{
			get
			{
				return this.actionId;
			}
		}

		public byte[] CommandOutput
		{
			get
			{
				return this.commandOutput;
			}
		}

		public string InteractionId
		{
			get
			{
				return this.interactionId;
			}
		}

		public string DebugPhraseMatched
		{
			get
			{
				return this.debugPhraseMatched;
			}
		}

		public string Utterance
		{
			get
			{
				return this.utterance;
			}
		}

		public Dictionary<string, string> MatchedSlots
		{
			get
			{
				return this.slotValues;
			}
		}

		public override string ToString()
		{
			string text = string.Concat(new string[]
			{
				"{\n  actionId = ",
				this.actionId,
				",\n  commandOutput = ",
				(this.commandOutput != null) ? (this.commandOutput.Length.ToString() + " bytes") : "null",
				",\n  interactionId = ",
				this.interactionId,
				",\n  utterance = ",
				this.utterance,
				",\n  matchedSlots = ["
			});
			foreach (KeyValuePair<string, string> keyValuePair in this.slotValues)
			{
				text = string.Concat(new string[]
				{
					text,
					"\n    ",
					keyValuePair.Key,
					" = ",
					keyValuePair.Value,
					","
				});
			}
			text += "\n  ]";
			return text + "\n}";
		}

		public string this[string slotName]
		{
			get
			{
				string result;
				if (!this.slotValues.TryGetValue(slotName, out result))
				{
					return null;
				}
				return result;
			}
		}

		public bool TryGetSlot(string slotName, out string slotValue)
		{
			return this.slotValues.TryGetValue(slotName, out slotValue);
		}

		public bool HasSlot(string slotName)
		{
			return this.slotValues.ContainsKey(slotName);
		}

		private string actionId;

		private byte[] commandOutput;

		private string utterance;

		private string interactionId;

		private string debugPhraseMatched;

		private Dictionary<string, string> slotValues;

		public abstract class Builder
		{
			public abstract string ActionId { get; }

			public abstract byte[] CommandOutput { get; }

			public abstract string InteractionId { get; }

			public abstract string DebugPhraseMatched { get; }

			public abstract string Utterance { get; }

			public abstract Dictionary<string, string> SlotValues { get; }

			public virtual VoiceCommandResult Build()
			{
				return new VoiceCommandResult
				{
					actionId = this.ActionId,
					commandOutput = this.CommandOutput,
					interactionId = this.InteractionId,
					debugPhraseMatched = this.DebugPhraseMatched,
					utterance = this.Utterance,
					slotValues = this.SlotValues
				};
			}
		}
	}
}
