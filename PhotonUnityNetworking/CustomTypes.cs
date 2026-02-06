using System;
using ExitGames.Client.Photon;
using Photon.Realtime;

namespace Photon.Pun
{
	internal static class CustomTypes
	{
		internal static void Register()
		{
			PhotonPeer.RegisterType(typeof(Player), 80, new SerializeStreamMethod(CustomTypes.SerializePhotonPlayer), new DeserializeStreamMethod(CustomTypes.DeserializePhotonPlayer));
		}

		private static short SerializePhotonPlayer(StreamBuffer outStream, object customobject)
		{
			int actorNumber = ((Player)customobject).ActorNumber;
			byte[] obj = CustomTypes.memPlayer;
			short result;
			lock (obj)
			{
				byte[] array = CustomTypes.memPlayer;
				int num = 0;
				Protocol.Serialize(actorNumber, array, ref num);
				outStream.Write(array, 0, 4);
				result = 4;
			}
			return result;
		}

		private static object DeserializePhotonPlayer(StreamBuffer inStream, short length)
		{
			if (length != 4)
			{
				return null;
			}
			byte[] obj = CustomTypes.memPlayer;
			int id;
			lock (obj)
			{
				inStream.Read(CustomTypes.memPlayer, 0, (int)length);
				int num = 0;
				Protocol.Deserialize(out id, CustomTypes.memPlayer, ref num);
			}
			if (PhotonNetwork.CurrentRoom != null)
			{
				return PhotonNetwork.CurrentRoom.GetPlayer(id, false);
			}
			return null;
		}

		public static readonly byte[] memPlayer = new byte[4];
	}
}
