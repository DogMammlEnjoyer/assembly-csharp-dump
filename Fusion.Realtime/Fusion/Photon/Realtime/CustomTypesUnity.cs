using System;
using System.Runtime.CompilerServices;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Fusion.Photon.Realtime
{
	internal static class CustomTypesUnity
	{
		internal static void Register()
		{
			Type typeFromHandle = typeof(Vector2);
			byte code = 87;
			SerializeStreamMethod serializeMethod;
			if ((serializeMethod = CustomTypesUnity.<>O.<0>__SerializeVector2) == null)
			{
				serializeMethod = (CustomTypesUnity.<>O.<0>__SerializeVector2 = new SerializeStreamMethod(CustomTypesUnity.SerializeVector2));
			}
			DeserializeStreamMethod constructor;
			if ((constructor = CustomTypesUnity.<>O.<1>__DeserializeVector2) == null)
			{
				constructor = (CustomTypesUnity.<>O.<1>__DeserializeVector2 = new DeserializeStreamMethod(CustomTypesUnity.DeserializeVector2));
			}
			PhotonPeer.RegisterType(typeFromHandle, code, serializeMethod, constructor);
			Type typeFromHandle2 = typeof(Vector3);
			byte code2 = 86;
			SerializeStreamMethod serializeMethod2;
			if ((serializeMethod2 = CustomTypesUnity.<>O.<2>__SerializeVector3) == null)
			{
				serializeMethod2 = (CustomTypesUnity.<>O.<2>__SerializeVector3 = new SerializeStreamMethod(CustomTypesUnity.SerializeVector3));
			}
			DeserializeStreamMethod constructor2;
			if ((constructor2 = CustomTypesUnity.<>O.<3>__DeserializeVector3) == null)
			{
				constructor2 = (CustomTypesUnity.<>O.<3>__DeserializeVector3 = new DeserializeStreamMethod(CustomTypesUnity.DeserializeVector3));
			}
			PhotonPeer.RegisterType(typeFromHandle2, code2, serializeMethod2, constructor2);
			Type typeFromHandle3 = typeof(Quaternion);
			byte code3 = 81;
			SerializeStreamMethod serializeMethod3;
			if ((serializeMethod3 = CustomTypesUnity.<>O.<4>__SerializeQuaternion) == null)
			{
				serializeMethod3 = (CustomTypesUnity.<>O.<4>__SerializeQuaternion = new SerializeStreamMethod(CustomTypesUnity.SerializeQuaternion));
			}
			DeserializeStreamMethod constructor3;
			if ((constructor3 = CustomTypesUnity.<>O.<5>__DeserializeQuaternion) == null)
			{
				constructor3 = (CustomTypesUnity.<>O.<5>__DeserializeQuaternion = new DeserializeStreamMethod(CustomTypesUnity.DeserializeQuaternion));
			}
			PhotonPeer.RegisterType(typeFromHandle3, code3, serializeMethod3, constructor3);
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
			bool flag = length != 12;
			object result;
			if (flag)
			{
				result = vector;
			}
			else
			{
				byte[] obj = CustomTypesUnity.memVector3;
				lock (obj)
				{
					inStream.Read(CustomTypesUnity.memVector3, 0, 12);
					int num = 0;
					Protocol.Deserialize(out vector.x, CustomTypesUnity.memVector3, ref num);
					Protocol.Deserialize(out vector.y, CustomTypesUnity.memVector3, ref num);
					Protocol.Deserialize(out vector.z, CustomTypesUnity.memVector3, ref num);
				}
				result = vector;
			}
			return result;
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
			bool flag = length != 8;
			object result;
			if (flag)
			{
				result = vector;
			}
			else
			{
				byte[] obj = CustomTypesUnity.memVector2;
				lock (obj)
				{
					inStream.Read(CustomTypesUnity.memVector2, 0, 8);
					int num = 0;
					Protocol.Deserialize(out vector.x, CustomTypesUnity.memVector2, ref num);
					Protocol.Deserialize(out vector.y, CustomTypesUnity.memVector2, ref num);
				}
				result = vector;
			}
			return result;
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
			bool flag = length != 16;
			object result;
			if (flag)
			{
				result = identity;
			}
			else
			{
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
				result = identity;
			}
			return result;
		}

		private const int SizeV2 = 8;

		private const int SizeV3 = 12;

		private const int SizeQuat = 16;

		public static readonly byte[] memVector3 = new byte[12];

		public static readonly byte[] memVector2 = new byte[8];

		public static readonly byte[] memQuarternion = new byte[16];

		[CompilerGenerated]
		private static class <>O
		{
			public static SerializeStreamMethod <0>__SerializeVector2;

			public static DeserializeStreamMethod <1>__DeserializeVector2;

			public static SerializeStreamMethod <2>__SerializeVector3;

			public static DeserializeStreamMethod <3>__DeserializeVector3;

			public static SerializeStreamMethod <4>__SerializeQuaternion;

			public static DeserializeStreamMethod <5>__DeserializeQuaternion;
		}
	}
}
