using System;
using System.IO;
using UnityEngine;

namespace Fusion
{
	internal class ClientTimeTrace
	{
		internal string Folder
		{
			get
			{
				return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Photon/Fusion/Dev/Logs";
			}
		}

		internal string File
		{
			get
			{
				string arg = DateTimeOffset.FromUnixTimeMilliseconds(this.Timestamp).UtcDateTime.ToString("yyyy-MM-ddTHH-mm-ssZ");
				return string.Format("fusion_player{0}_timing_{1}.csv", this.Player, arg);
			}
		}

		public ClientTimeTrace(int player, TickRate.Resolved tickRate)
		{
			this.Timestamp = DateTime.Now.ToUnixTimeMilliseconds();
			this.Player = player;
			this.WriteHeaders(tickRate);
		}

		public void OnFeedback(Simulation.TimeFeedback packetFeedback)
		{
			this.PacketFeedback = packetFeedback;
		}

		public void OnPacket(int packetNumber, double packetDeltaTime, double roundTripTime)
		{
			this.RoundTripTime = roundTripTime;
			this.PacketDeltaTime = packetDeltaTime;
			this.Packets++;
			this.PacketNumber = packetNumber;
			this.PacketReceived = true;
		}

		public void OnFrame(double frameDeltaTime)
		{
			this.FrameDeltaTime = frameDeltaTime;
			this.Frames++;
			this.WriteLine();
			this.PacketReceived = false;
		}

		private void WriteHeaders(TickRate.Resolved tickRate)
		{
			Directory.CreateDirectory(this.Folder);
			StreamWriter streamWriter = System.IO.File.AppendText(this.Folder + "/" + this.File);
			string text = string.Format("{0}", Application.platform);
			streamWriter.WriteLine("client_platform, client_tick_hz, client_send_hz, server_tick_hz, server_send_hz");
			streamWriter.WriteLine(string.Format("{0},{1},{2},{3},{4}", new object[]
			{
				text,
				tickRate.Client,
				tickRate.ClientSend,
				tickRate.Server,
				tickRate.ServerSend
			}));
			streamWriter.WriteLine("frame, frame_dt, received_packet, packet, packet_sequence, packet_dt, rtt, fb_packet_dt_avg, fb_packet_dt_dev, fb_buffer_avg, fb_buffer_dev");
			streamWriter.Close();
		}

		private void WriteLine()
		{
			Directory.CreateDirectory(this.Folder);
			StreamWriter streamWriter = System.IO.File.AppendText(this.Folder + "/" + this.File);
			bool packetReceived = this.PacketReceived;
			if (packetReceived)
			{
				streamWriter.WriteLine(string.Format("{0},{1:f4},{2},{3},{4},{5:f4},{6:f4},{7:f4},{8:f4},{9:f4},{10:f4}", new object[]
				{
					this.Frames - 1,
					this.FrameDeltaTime,
					Convert.ToInt32(this.PacketReceived),
					this.Packets - 1,
					this.PacketNumber,
					this.PacketDeltaTime,
					this.RoundTripTime,
					this.PacketFeedback.RecvDeltaAvg,
					this.PacketFeedback.RecvDeltaDev,
					this.PacketFeedback.OffsetAvg,
					this.PacketFeedback.OffsetDev
				}));
			}
			else
			{
				streamWriter.WriteLine(string.Format("{0},{1:f4},{2}", this.Frames - 1, this.FrameDeltaTime, Convert.ToInt32(this.PacketReceived)));
			}
			streamWriter.Close();
		}

		private readonly long Timestamp;

		private readonly int Player;

		private int Frames;

		private double FrameDeltaTime;

		private bool PacketReceived;

		private int PacketNumber;

		private int Packets;

		private double PacketDeltaTime;

		private double RoundTripTime;

		private Simulation.TimeFeedback PacketFeedback;
	}
}
