using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Fusion
{
	public class SessionInfo
	{
		public bool IsValid
		{
			get
			{
				return this._isValid;
			}
		}

		public string Name { get; internal set; }

		public string Region { get; internal set; }

		public bool IsVisible
		{
			get
			{
				return this._isVisible;
			}
			set
			{
				bool flag = this._runner == null;
				if (!flag)
				{
					bool isSinglePlayer = this._runner.IsSinglePlayer;
					if (isSinglePlayer)
					{
						this._isVisible = value;
					}
					else
					{
						NetworkRunner runner = this._runner;
						bool? flag2;
						if (runner == null)
						{
							flag2 = null;
						}
						else
						{
							CloudServices cloudServices = runner._cloudServices;
							flag2 = ((cloudServices != null) ? new bool?(cloudServices.UpdateRoomIsVisible(value)) : null);
						}
						bool? flag3 = flag2;
						bool valueOrDefault = flag3.GetValueOrDefault();
						if (valueOrDefault)
						{
							this._isValid = false;
						}
					}
				}
			}
		}

		public bool IsOpen
		{
			get
			{
				return this._isOpen;
			}
			set
			{
				bool flag = this._runner == null;
				if (!flag)
				{
					bool isSinglePlayer = this._runner.IsSinglePlayer;
					if (isSinglePlayer)
					{
						this._isOpen = value;
					}
					else
					{
						NetworkRunner runner = this._runner;
						bool? flag2;
						if (runner == null)
						{
							flag2 = null;
						}
						else
						{
							CloudServices cloudServices = runner._cloudServices;
							flag2 = ((cloudServices != null) ? new bool?(cloudServices.UpdateRoomIsOpen(value)) : null);
						}
						bool? flag3 = flag2;
						bool valueOrDefault = flag3.GetValueOrDefault();
						if (valueOrDefault)
						{
							this._isValid = false;
						}
					}
				}
			}
		}

		public ReadOnlyDictionary<string, SessionProperty> Properties { get; internal set; }

		public int PlayerCount { get; internal set; }

		public int MaxPlayers { get; internal set; }

		public static implicit operator bool(SessionInfo sessionInfo)
		{
			return sessionInfo != null && sessionInfo.IsValid;
		}

		internal SessionInfo(NetworkRunner runner = null)
		{
			this._runner = runner;
			bool flag = runner != null && runner.IsSinglePlayer;
			if (flag)
			{
				this._isValid = true;
			}
		}

		public bool UpdateCustomProperties(Dictionary<string, SessionProperty> customProperties)
		{
			bool flag = this._runner == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool isSinglePlayer = this._runner.IsSinglePlayer;
				if (isSinglePlayer)
				{
					Dictionary<string, SessionProperty> dictionary = new Dictionary<string, SessionProperty>();
					ReadOnlyDictionary<string, SessionProperty> properties = this.Properties;
					bool flag2 = properties != null && properties.Count > 0;
					if (flag2)
					{
						dictionary = new Dictionary<string, SessionProperty>(this.Properties);
					}
					foreach (KeyValuePair<string, SessionProperty> keyValuePair in customProperties)
					{
						dictionary[keyValuePair.Key] = keyValuePair.Value;
					}
					this.Properties = new ReadOnlyDictionary<string, SessionProperty>(dictionary);
					result = true;
				}
				else
				{
					NetworkRunner runner = this._runner;
					bool? flag3;
					if (runner == null)
					{
						flag3 = null;
					}
					else
					{
						CloudServices cloudServices = runner._cloudServices;
						flag3 = ((cloudServices != null) ? new bool?(cloudServices.UpdateRoomProperties(customProperties)) : null);
					}
					bool? flag4 = flag3;
					bool valueOrDefault = flag4.GetValueOrDefault();
					if (valueOrDefault)
					{
						this._isValid = false;
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("[SessionInfo: ");
			stringBuilder.Append(string.Format("{0}={1}, ", "IsValid", this.IsValid));
			stringBuilder.Append("Name=" + this.Name + ", ");
			stringBuilder.Append(string.Format("{0}={1}, ", "IsOpen", this.IsOpen));
			stringBuilder.Append(string.Format("{0}={1}, ", "IsVisible", this.IsVisible));
			stringBuilder.Append("Region=" + this.Region + ", ");
			stringBuilder.Append(string.Format("{0}={1}, ", "PlayerCount", this.PlayerCount));
			stringBuilder.Append(string.Format("{0}={1}, ", "MaxPlayers", this.MaxPlayers));
			stringBuilder.Append("Properties=");
			bool flag = this.Properties != null;
			if (flag)
			{
				foreach (KeyValuePair<string, SessionProperty> keyValuePair in this.Properties)
				{
					StringBuilder stringBuilder2 = stringBuilder;
					string format = "{0}={1},";
					object key = keyValuePair.Key;
					SessionProperty value = keyValuePair.Value;
					stringBuilder2.Append(string.Format(format, key, (value != null) ? value.PropertyValue : null));
				}
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		internal bool _isValid;

		internal bool _isOpen;

		internal bool _isVisible;

		private readonly NetworkRunner _runner;
	}
}
