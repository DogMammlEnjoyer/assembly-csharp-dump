using System;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Photon.Realtime
{
	internal static class CustomTypesUnity
	{
		internal static void Register()
		{
			PhotonPeer.RegisterType(typeof(Vector2), 87, new SerializeStreamMethod(CustomTypesUnity.SerializeVector2), new DeserializeStreamMethod(CustomTypesUnity.DeserializeVector2));
			PhotonPeer.RegisterType(typeof(Vector3), 86, new SerializeStreamMethod(CustomTypesUnity.SerializeVector3), new DeserializeStreamMethod(CustomTypesUnity.DeserializeVector3));
			PhotonPeer.RegisterType(typeof(Quaternion), 81, new SerializeStreamMethod(CustomTypesUnity.SerializeQuaternion), new DeserializeStreamMethod(CustomTypesUnity.DeserializeQuaternion));
		}

		private static short SerializeVector3(StreamBuffer outStream, object customobject)
		{
			Vector3 vector = (Vector3)customobject;
			int num = 0;
			byte[] obj = CustomTypesUnity.memVector3;
			lock (obj)
			{
				byte[] array = CustomTypesUnity.memVector3;
				Protocol.Serialize(vector.x, array, ref num);
				Protocol.Serialize(vector.y, array, ref num);
				Protocol.Serialize(vector.z, array, ref num);
				outStream.Write(array, 0, 12);
			}
			return 12;
		}

		private static object DeserializeVector3(StreamBuffer inStream, short length)
		{
			Vector3 vector = default(Vector3);
			if (length != 12)
			{
				return vector;
			}
			byte[] obj = CustomTypesUnity.memVector3;
			lock (obj)
			{
				inStream.Read(CustomTypesUnity.memVector3, 0, 12);
				int num = 0;
				Protocol.Deserialize(out vector.x, CustomTypesUnity.memVector3, ref num);
				Protocol.Deserialize(out vector.y, CustomTypesUnity.memVector3, ref num);
				Protocol.Deserialize(out vector.z, CustomTypesUnity.memVector3, ref num);
			}
			return vector;
		}

		private static short SerializeVector2(StreamBuffer outStream, object customobject)
		{
			Vector2 vector = (Vector2)customobject;
			byte[] obj = CustomTypesUnity.memVector2;
			lock (obj)
			{
				byte[] array = CustomTypesUnity.memVector2;
				int num = 0;
				Protocol.Serialize(vector.x, array, ref num);
				Protocol.Serialize(vector.y, array, ref num);
				outStream.Write(array, 0, 8);
			}
			return 8;
		}

		private static object DeserializeVector2(StreamBuffer inStream, short length)
		{
			Vector2 vector = default(Vector2);
			if (length != 8)
			{
				return vector;
			}
			byte[] obj = CustomTypesUnity.memVector2;
			lock (obj)
			{
				inStream.Read(CustomTypesUnity.memVector2, 0, 8);
				int num = 0;
				Protocol.Deserialize(out vector.x, CustomTypesUnity.memVector2, ref num);
				Protocol.Deserialize(out vector.y, CustomTypesUnity.memVector2, ref num);
			}
			return vector;
		}

		private static short SerializeQuaternion(StreamBuffer outStream, object customobject)
		{
			Quaternion quaternion = (Quaternion)customobject;
			byte[] obj = CustomTypesUnity.memQuarternion;
			lock (obj)
			{
				byte[] array = CustomTypesUnity.memQuarternion;
				int num = 0;
				Protocol.Serialize(quaternion.w, array, ref num);
				Protocol.Serialize(quaternion.x, array, ref num);
				Protocol.Serialize(quaternion.y, array, ref num);
				Protocol.Serialize(quaternion.z, array, ref num);
				outStream.Write(array, 0, 16);
			}
			return 16;
		}

		private static object DeserializeQuaternion(StreamBuffer inStream, short length)
		{
			Quaternion identity = Quaternion.identity;
			if (length != 16)
			{
				return identity;
			}
			byte[] obj = CustomTypesUnity.memQuarternion;
			lock (obj)
			{
				inStream.Read(CustomTypesUnity.memQuarternion, 0, 16);
				int num = 0;
				Protocol.Deserialize(out identity.w, CustomTypesUnity.memQuarternion, ref num);
				Protocol.Deserialize(out identity.x, CustomTypesUnity.memQuarternion, ref num);
				Protocol.Deserialize(out identity.y, CustomTypesUnity.memQuarternion, ref num);
				Protocol.Deserialize(out identity.z, CustomTypesUnity.memQuarternion, ref num);
			}
			return identity;
		}

		private const int SizeV2 = 8;

		private const int SizeV3 = 12;

		private const int SizeQuat = 16;

		public static readonly byte[] memVector3 = new byte[12];

		public static readonly byte[] memVector2 = new byte[8];

		public static readonly byte[] memQuarternion = new byte[16];
	}
}
