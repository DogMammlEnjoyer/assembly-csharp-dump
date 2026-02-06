using System;
using System.Collections.Generic;
using System.Linq;

namespace Photon.Voice
{
	internal class PhotonTransportProtocol
	{
		public PhotonTransportProtocol(VoiceClient voiceClient, ILogger logger)
		{
			this.voiceClient = voiceClient;
			this.logger = logger;
		}

		internal object[] buildVoicesInfo(IEnumerable<LocalVoice> voicesToSend, bool logInfo)
		{
			object[] array = new object[voicesToSend.Count<LocalVoice>()];
			object[] result = new object[]
			{
				0,
				PhotonTransportProtocol.EventSubcode.VoiceInfo,
				array
			};
			int num = 0;
			foreach (LocalVoice localVoice in voicesToSend)
			{
				array[num] = new Dictionary<byte, object>
				{
					{
						1,
						localVoice.ID
					},
					{
						12,
						localVoice.Info.Codec
					},
					{
						2,
						localVoice.Info.SamplingRate
					},
					{
						3,
						localVoice.Info.Channels
					},
					{
						4,
						localVoice.Info.FrameDurationUs
					},
					{
						5,
						localVoice.Info.Bitrate
					},
					{
						6,
						localVoice.Info.Width
					},
					{
						7,
						localVoice.Info.Height
					},
					{
						8,
						localVoice.Info.FPS
					},
					{
						9,
						localVoice.Info.KeyFrameInt
					},
					{
						10,
						localVoice.Info.UserData
					},
					{
						11,
						localVoice.EvNumber
					}
				};
				num++;
				if (logInfo)
				{
					this.logger.LogInfo(string.Concat(new string[]
					{
						localVoice.LogPrefix,
						" Sending info: ",
						localVoice.Info.ToString(),
						" ev=",
						localVoice.EvNumber.ToString()
					}), Array.Empty<object>());
				}
			}
			return result;
		}

		internal object[] buildVoiceRemoveMessage(LocalVoice v)
		{
			byte[] array = new byte[]
			{
				v.ID
			};
			object[] result = new object[]
			{
				0,
				PhotonTransportProtocol.EventSubcode.VoiceRemove,
				array
			};
			this.logger.LogInfo(v.LogPrefix + " remove sent", Array.Empty<object>());
			return result;
		}

		internal object[] buildFrameMessage(byte voiceId, byte evNumber, ArraySegment<byte> data, FrameFlags flags)
		{
			return new object[]
			{
				voiceId,
				evNumber,
				data,
				(byte)flags
			};
		}

		internal void onVoiceEvent(object content0, int channelId, int playerId, bool isLocalPlayer)
		{
			object[] array = (object[])content0;
			if ((byte)array[0] != 0)
			{
				byte voiceId = (byte)array[0];
				byte evNumber = (byte)array[1];
				byte[] array2 = (byte[])array[2];
				FrameFlags flags = (FrameFlags)0;
				if (array.Length > 3)
				{
					flags = (FrameFlags)array[3];
				}
				FrameBuffer frameBuffer = new FrameBuffer(array2, flags);
				this.voiceClient.onFrame(channelId, playerId, voiceId, evNumber, ref frameBuffer, isLocalPlayer);
				frameBuffer.Release();
				return;
			}
			byte b = (byte)array[1];
			if (b == 1)
			{
				this.onVoiceInfo(channelId, playerId, array[2]);
				return;
			}
			if (b != 2)
			{
				ILogger logger = this.logger;
				string str = "[PV] Unknown sevent subcode ";
				object obj = array[1];
				logger.LogError(str + ((obj != null) ? obj.ToString() : null), Array.Empty<object>());
				return;
			}
			this.onVoiceRemove(channelId, playerId, array[2]);
		}

		private void onVoiceInfo(int channelId, int playerId, object payload)
		{
			foreach (Dictionary<byte, object> dictionary in (object[])payload)
			{
				byte voiceId = (byte)dictionary[1];
				byte eventNumber = (byte)dictionary[11];
				VoiceInfo info = this.createVoiceInfoFromEventPayload(dictionary);
				this.voiceClient.onVoiceInfo(channelId, playerId, voiceId, eventNumber, info);
			}
		}

		private void onVoiceRemove(int channelId, int playerId, object payload)
		{
			byte[] voiceIds = (byte[])payload;
			this.voiceClient.onVoiceRemove(channelId, playerId, voiceIds);
		}

		private VoiceInfo createVoiceInfoFromEventPayload(Dictionary<byte, object> h)
		{
			VoiceInfo result = default(VoiceInfo);
			result.Codec = (Codec)h[12];
			result.SamplingRate = (int)h[2];
			result.Channels = (int)h[3];
			result.FrameDurationUs = (int)h[4];
			result.Bitrate = (int)h[5];
			if (h.ContainsKey(6))
			{
				result.Width = (int)h[6];
			}
			if (h.ContainsKey(7))
			{
				result.Height = (int)h[7];
			}
			if (h.ContainsKey(8))
			{
				result.FPS = (int)h[8];
			}
			if (h.ContainsKey(9))
			{
				result.KeyFrameInt = (int)h[9];
			}
			result.UserData = h[10];
			return result;
		}

		private VoiceClient voiceClient;

		private ILogger logger;

		private enum EventSubcode : byte
		{
			VoiceInfo = 1,
			VoiceRemove,
			Frame
		}

		private enum EventParam : byte
		{
			VoiceId = 1,
			SamplingRate,
			Channels,
			FrameDurationUs,
			Bitrate,
			Width,
			Height,
			FPS,
			KeyFrameInt,
			UserData,
			EventNumber,
			Codec
		}
	}
}
