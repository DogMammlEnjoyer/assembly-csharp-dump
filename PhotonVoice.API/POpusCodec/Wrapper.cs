using System;
using System.Runtime.InteropServices;
using Photon.Voice;
using POpusCodec.Enums;

namespace POpusCodec
{
	internal class Wrapper
	{
		[DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private static extern int opus_encoder_get_size(Channels channels);

		[DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private static extern OpusStatusCode opus_encoder_init(IntPtr st, SamplingRate Fs, Channels channels, OpusApplicationType application);

		[DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public static extern IntPtr opus_get_version_string();

		[DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private static extern int opus_encode(IntPtr st, short[] pcm, int frame_size, byte[] data, int max_data_bytes);

		[DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private static extern int opus_encode_float(IntPtr st, float[] pcm, int frame_size, byte[] data, int max_data_bytes);

		[DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "opus_encoder_ctl")]
		private static extern int opus_encoder_ctl_set(IntPtr st, OpusCtlSetRequest request, int value);

		[DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "opus_encoder_ctl")]
		private static extern int opus_encoder_ctl_get(IntPtr st, OpusCtlGetRequest request, ref int value);

		[DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "opus_decoder_ctl")]
		private static extern int opus_decoder_ctl_set(IntPtr st, OpusCtlSetRequest request, int value);

		[DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "opus_decoder_ctl")]
		private static extern int opus_decoder_ctl_get(IntPtr st, OpusCtlGetRequest request, ref int value);

		[DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private static extern int opus_decoder_get_size(Channels channels);

		[DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private static extern OpusStatusCode opus_decoder_init(IntPtr st, SamplingRate Fs, Channels channels);

		[DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private static extern int opus_decode(IntPtr st, IntPtr data, int len, short[] pcm, int frame_size, int decode_fec);

		[DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private static extern int opus_decode_float(IntPtr st, IntPtr data, int len, float[] pcm, int frame_size, int decode_fec);

		[DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public static extern int opus_packet_get_bandwidth(IntPtr data);

		[DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public static extern int opus_packet_get_nb_channels(byte[] data);

		[DllImport("opus_egpv", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private static extern IntPtr opus_strerror(OpusStatusCode error);

		public static IntPtr opus_encoder_create(SamplingRate Fs, Channels channels, OpusApplicationType application)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(Wrapper.opus_encoder_get_size(channels));
			OpusStatusCode statusCode = Wrapper.opus_encoder_init(intPtr, Fs, channels, application);
			try
			{
				Wrapper.HandleStatusCode(statusCode, new object[]
				{
					"opus_encoder_create/opus_encoder_init",
					Fs,
					channels,
					application
				});
			}
			catch (Exception ex)
			{
				if (intPtr != IntPtr.Zero)
				{
					Wrapper.opus_encoder_destroy(intPtr);
					intPtr = IntPtr.Zero;
				}
				throw ex;
			}
			return intPtr;
		}

		public static int opus_encode(IntPtr st, short[] pcm, int frame_size, byte[] data)
		{
			if (st == IntPtr.Zero)
			{
				throw new ObjectDisposedException("OpusEncoder");
			}
			int num = Wrapper.opus_encode(st, pcm, frame_size, data, data.Length);
			if (num <= 0)
			{
				Wrapper.HandleStatusCode((OpusStatusCode)num, new object[]
				{
					"opus_encode/short",
					frame_size,
					data.Length
				});
			}
			return num;
		}

		public static int opus_encode(IntPtr st, float[] pcm, int frame_size, byte[] data)
		{
			if (st == IntPtr.Zero)
			{
				throw new ObjectDisposedException("OpusEncoder");
			}
			int num = Wrapper.opus_encode_float(st, pcm, frame_size, data, data.Length);
			if (num <= 0)
			{
				Wrapper.HandleStatusCode((OpusStatusCode)num, new object[]
				{
					"opus_encode/float",
					frame_size,
					data.Length
				});
			}
			return num;
		}

		public static void opus_encoder_destroy(IntPtr st)
		{
			Marshal.FreeHGlobal(st);
		}

		public static int get_opus_encoder_ctl(IntPtr st, OpusCtlGetRequest request)
		{
			if (st == IntPtr.Zero)
			{
				throw new ObjectDisposedException("OpusEncoder");
			}
			int result = 0;
			Wrapper.HandleStatusCode((OpusStatusCode)Wrapper.opus_encoder_ctl_get(st, request, ref result), new object[]
			{
				"opus_encoder_ctl_get",
				request
			});
			return result;
		}

		public static void set_opus_encoder_ctl(IntPtr st, OpusCtlSetRequest request, int value)
		{
			if (st == IntPtr.Zero)
			{
				throw new ObjectDisposedException("OpusEncoder");
			}
			Wrapper.HandleStatusCode((OpusStatusCode)Wrapper.opus_encoder_ctl_set(st, request, value), new object[]
			{
				"opus_encoder_ctl_set",
				request,
				value
			});
		}

		public static int get_opus_decoder_ctl(IntPtr st, OpusCtlGetRequest request)
		{
			if (st == IntPtr.Zero)
			{
				throw new ObjectDisposedException("OpusDcoder");
			}
			int num = 0;
			Wrapper.HandleStatusCode((OpusStatusCode)Wrapper.opus_decoder_ctl_get(st, request, ref num), new object[]
			{
				"get_opus_decoder_ctl",
				request,
				num
			});
			return num;
		}

		public static void set_opus_decoder_ctl(IntPtr st, OpusCtlSetRequest request, int value)
		{
			if (st == IntPtr.Zero)
			{
				throw new ObjectDisposedException("OpusDecoder");
			}
			Wrapper.HandleStatusCode((OpusStatusCode)Wrapper.opus_decoder_ctl_set(st, request, value), new object[]
			{
				"set_opus_decoder_ctl",
				request,
				value
			});
		}

		public static IntPtr opus_decoder_create(SamplingRate Fs, Channels channels)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(Wrapper.opus_decoder_get_size(channels));
			OpusStatusCode statusCode = Wrapper.opus_decoder_init(intPtr, Fs, channels);
			try
			{
				Wrapper.HandleStatusCode(statusCode, new object[]
				{
					"opus_decoder_create",
					Fs,
					channels
				});
			}
			catch (Exception ex)
			{
				if (intPtr != IntPtr.Zero)
				{
					Wrapper.opus_decoder_destroy(intPtr);
					intPtr = IntPtr.Zero;
				}
				throw ex;
			}
			return intPtr;
		}

		public static void opus_decoder_destroy(IntPtr st)
		{
			Marshal.FreeHGlobal(st);
		}

		public static int opus_decode(IntPtr st, FrameBuffer data, short[] pcm, int decode_fec, int channels)
		{
			if (st == IntPtr.Zero)
			{
				throw new ObjectDisposedException("OpusDecoder");
			}
			int num = Wrapper.opus_decode(st, data.Ptr, data.Length, pcm, pcm.Length / channels, decode_fec);
			if (num == -4)
			{
				return 0;
			}
			if (num <= 0)
			{
				Wrapper.HandleStatusCode((OpusStatusCode)num, new object[]
				{
					"opus_decode/short",
					data.Length,
					pcm.Length,
					decode_fec,
					channels
				});
			}
			return num;
		}

		public static int opus_decode(IntPtr st, FrameBuffer data, float[] pcm, int decode_fec, int channels)
		{
			if (st == IntPtr.Zero)
			{
				throw new ObjectDisposedException("OpusDecoder");
			}
			int num = Wrapper.opus_decode_float(st, data.Ptr, data.Length, pcm, pcm.Length / channels, decode_fec);
			if (num == -4)
			{
				return 0;
			}
			if (num <= 0)
			{
				Wrapper.HandleStatusCode((OpusStatusCode)num, new object[]
				{
					"opus_decode/float",
					data.Length,
					pcm.Length,
					decode_fec,
					channels
				});
			}
			return num;
		}

		private static void HandleStatusCode(OpusStatusCode statusCode, params object[] info)
		{
			if (statusCode != OpusStatusCode.OK)
			{
				string str = "";
				foreach (object obj in info)
				{
					str = str + obj.ToString() + ":";
				}
				throw new OpusException(statusCode, str + Marshal.PtrToStringAnsi(Wrapper.opus_strerror(statusCode)));
			}
		}

		private const string lib_name = "opus_egpv";

		private const string ctl_entry_point_set = "";

		private const string ctl_entry_point_get = "";
	}
}
