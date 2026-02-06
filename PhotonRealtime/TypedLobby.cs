using System;

namespace Photon.Realtime
{
	public class TypedLobby
	{
		public bool IsDefault
		{
			get
			{
				return string.IsNullOrEmpty(this.Name);
			}
		}

		internal TypedLobby()
		{
		}

		public TypedLobby(string name, LobbyType type)
		{
			this.Name = name;
			this.Type = type;
		}

		public override string ToString()
		{
			return string.Format("lobby '{0}'[{1}]", this.Name, this.Type);
		}

		public string Name;

		public LobbyType Type;

		public static readonly TypedLobby Default = new TypedLobby();
	}
}
