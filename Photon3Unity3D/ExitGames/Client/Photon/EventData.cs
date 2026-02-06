using System;

namespace ExitGames.Client.Photon
{
	public class EventData
	{
		public EventData()
		{
			this.Parameters = new ParameterDictionary();
		}

		public object this[byte key]
		{
			get
			{
				object result;
				this.Parameters.TryGetValue(key, out result);
				return result;
			}
			internal set
			{
				this.Parameters.Add(key, value);
			}
		}

		public int Sender
		{
			get
			{
				bool flag = this.sender == -1;
				if (flag)
				{
					int num;
					this.sender = (this.Parameters.TryGetValue<int>(this.SenderKey, out num) ? num : -1);
				}
				return this.sender;
			}
			internal set
			{
				this.sender = value;
			}
		}

		public object CustomData
		{
			get
			{
				bool flag = this.customData == null;
				if (flag)
				{
					this.Parameters.TryGetValue(this.CustomDataKey, out this.customData);
				}
				return this.customData;
			}
			internal set
			{
				this.customData = value;
			}
		}

		internal void Reset()
		{
			this.Code = 0;
			this.Parameters.Clear();
			this.sender = -1;
			this.customData = null;
		}

		public override string ToString()
		{
			return string.Format("Event {0}.", this.Code.ToString());
		}

		public string ToStringFull()
		{
			return string.Format("Event {0}: {1}", this.Code, SupportClass.DictionaryToString(this.Parameters, true));
		}

		public byte Code;

		public readonly ParameterDictionary Parameters;

		public byte SenderKey = 254;

		private int sender = -1;

		public byte CustomDataKey = 245;

		private object customData;
	}
}
