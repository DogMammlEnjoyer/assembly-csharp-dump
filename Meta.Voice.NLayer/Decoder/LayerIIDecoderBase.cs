using System;

namespace Meta.Voice.NLayer.Decoder
{
	internal abstract class LayerIIDecoderBase : LayerDecoderBase
	{
		protected static bool GetCRC(MpegFrame frame, int[] rateTable, int[][] allocLookupTable, bool readScfsiBits, ref uint crc)
		{
			int i = 0;
			int num = rateTable.Length;
			int num2 = num;
			if (frame.ChannelMode == MpegChannelMode.JointStereo)
			{
				num2 = frame.ChannelModeExtension * 4 + 4;
			}
			int num3 = (frame.ChannelMode == MpegChannelMode.Mono) ? 1 : 2;
			int j;
			for (j = 0; j < num2; j++)
			{
				int num4 = allocLookupTable[rateTable[j]][0];
				for (int k = 0; k < num3; k++)
				{
					int num5 = frame.ReadBits(num4);
					if (num5 > 0)
					{
						i += 2;
					}
					MpegFrame.UpdateCRC(num5, num4, ref crc);
				}
			}
			while (j < num)
			{
				int num6 = allocLookupTable[rateTable[j]][0];
				int num7 = frame.ReadBits(num6);
				if (num7 > 0)
				{
					i += num3 * 2;
				}
				MpegFrame.UpdateCRC(num7, num6, ref crc);
				j++;
			}
			if (readScfsiBits)
			{
				while (i >= 2)
				{
					MpegFrame.UpdateCRC(frame.ReadBits(2), 2, ref crc);
					i -= 2;
				}
			}
			return true;
		}

		protected LayerIIDecoderBase(int[][] allocLookupTable, int granuleCount)
		{
			this._allocLookupTable = allocLookupTable;
			this._granuleCount = granuleCount;
			this._allocation = new int[][]
			{
				new int[32],
				new int[32]
			};
			this._scfsi = new int[][]
			{
				new int[32],
				new int[32]
			};
			this._samples = new int[][]
			{
				new int[384 * this._granuleCount],
				new int[384 * this._granuleCount]
			};
			this._scalefac = new int[][][]
			{
				new int[3][],
				new int[3][]
			};
			for (int i = 0; i < 3; i++)
			{
				this._scalefac[0][i] = new int[32];
				this._scalefac[1][i] = new int[32];
			}
			this._polyPhaseBuf = new float[32];
		}

		internal override int DecodeFrame(IMpegFrame frame, float[] ch0, float[] ch1)
		{
			this.InitFrame(frame);
			int[] rateTable = this.GetRateTable(frame);
			this.ReadAllocation(frame, rateTable);
			for (int i = 0; i < this._scfsi[0].Length; i++)
			{
				this._scfsi[0][i] = ((this._allocation[0][i] != 0) ? 2 : -1);
				this._scfsi[1][i] = ((this._allocation[1][i] != 0) ? 2 : -1);
			}
			this.ReadScaleFactorSelection(frame, this._scfsi, this._channels);
			this.ReadScaleFactors(frame);
			this.ReadSamples(frame);
			return this.DecodeSamples(ch0, ch1);
		}

		private void InitFrame(IMpegFrame frame)
		{
			MpegChannelMode channelMode = frame.ChannelMode;
			if (channelMode == MpegChannelMode.JointStereo)
			{
				this._channels = 2;
				this._jsbound = frame.ChannelModeExtension * 4 + 4;
				return;
			}
			if (channelMode == MpegChannelMode.Mono)
			{
				this._channels = 1;
				this._jsbound = 32;
				return;
			}
			this._channels = 2;
			this._jsbound = 32;
		}

		protected abstract int[] GetRateTable(IMpegFrame frame);

		private void ReadAllocation(IMpegFrame frame, int[] rateTable)
		{
			int num = rateTable.Length;
			if (this._jsbound > num)
			{
				this._jsbound = num;
			}
			Array.Clear(this._allocation[0], 0, 32);
			Array.Clear(this._allocation[1], 0, 32);
			int i;
			for (i = 0; i < this._jsbound; i++)
			{
				int[] array = this._allocLookupTable[rateTable[i]];
				int bitCount = array[0];
				for (int j = 0; j < this._channels; j++)
				{
					this._allocation[j][i] = array[frame.ReadBits(bitCount) + 1];
				}
			}
			while (i < num)
			{
				int[] array2 = this._allocLookupTable[rateTable[i]];
				this._allocation[0][i] = (this._allocation[1][i] = array2[frame.ReadBits(array2[0]) + 1]);
				i++;
			}
		}

		protected abstract void ReadScaleFactorSelection(IMpegFrame frame, int[][] scfsi, int channels);

		private void ReadScaleFactors(IMpegFrame frame)
		{
			for (int i = 0; i < 32; i++)
			{
				for (int j = 0; j < this._channels; j++)
				{
					switch (this._scfsi[j][i])
					{
					case 0:
						this._scalefac[j][0][i] = frame.ReadBits(6);
						this._scalefac[j][1][i] = frame.ReadBits(6);
						this._scalefac[j][2][i] = frame.ReadBits(6);
						break;
					case 1:
						this._scalefac[j][0][i] = (this._scalefac[j][1][i] = frame.ReadBits(6));
						this._scalefac[j][2][i] = frame.ReadBits(6);
						break;
					case 2:
						this._scalefac[j][0][i] = (this._scalefac[j][1][i] = (this._scalefac[j][2][i] = frame.ReadBits(6)));
						break;
					case 3:
						this._scalefac[j][0][i] = frame.ReadBits(6);
						this._scalefac[j][1][i] = (this._scalefac[j][2][i] = frame.ReadBits(6));
						break;
					default:
						this._scalefac[j][0][i] = 63;
						this._scalefac[j][1][i] = 63;
						this._scalefac[j][2][i] = 63;
						break;
					}
				}
			}
		}

		private void ReadSamples(IMpegFrame frame)
		{
			int i = 0;
			int num = 0;
			while (i < 12)
			{
				int j = 0;
				while (j < 32)
				{
					for (int k = 0; k < this._channels; k++)
					{
						if (k == 0 || j < this._jsbound)
						{
							int num2 = this._allocation[k][j];
							if (num2 != 0)
							{
								if (num2 < 0)
								{
									int num3 = frame.ReadBits(-num2);
									int num4 = (1 << -num2 / 2 + -num2 % 2 - 1) + 1;
									this._samples[k][num] = num3 % num4;
									num3 /= num4;
									this._samples[k][num + 32] = num3 % num4;
									this._samples[k][num + 64] = num3 / num4;
								}
								else
								{
									for (int l = 0; l < this._granuleCount; l++)
									{
										this._samples[k][num + 32 * l] = frame.ReadBits(num2);
									}
								}
							}
							else
							{
								for (int m = 0; m < this._granuleCount; m++)
								{
									this._samples[k][num + 32 * m] = 0;
								}
							}
						}
						else
						{
							for (int n = 0; n < this._granuleCount; n++)
							{
								this._samples[1][num + 32 * n] = this._samples[0][num + 32 * n];
							}
						}
					}
					j++;
					num++;
				}
				i++;
				num += 32 * (this._granuleCount - 1);
			}
		}

		private int DecodeSamples(float[] ch0, float[] ch1)
		{
			float[][] array = new float[2][];
			int num = 0;
			int num2 = this._channels - 1;
			if (this._channels == 1 || base.StereoMode == StereoMode.LeftOnly)
			{
				array[0] = ch0;
				num2 = 0;
			}
			else if (base.StereoMode == StereoMode.RightOnly)
			{
				array[1] = ch0;
				num = 1;
			}
			else
			{
				array[0] = ch0;
				array[1] = ch1;
			}
			int num3 = 0;
			for (int i = num; i <= num2; i++)
			{
				num3 = 0;
				for (int j = 0; j < this._granuleCount; j++)
				{
					for (int k = 0; k < 12; k++)
					{
						int l = 0;
						while (l < 32)
						{
							int num4 = this._allocation[i][l];
							if (num4 != 0)
							{
								float[] array2;
								float[] array3;
								if (num4 < 0)
								{
									num4 = -num4 / 2 + -num4 % 2 - 1;
									array2 = LayerIIDecoderBase._groupedC;
									array3 = LayerIIDecoderBase._groupedD;
								}
								else
								{
									array2 = LayerIIDecoderBase._C;
									array3 = LayerIIDecoderBase._D;
								}
								this._polyPhaseBuf[l] = array2[num4] * ((float)(this._samples[i][num3] << 16 - num4) / 32768f + array3[num4]) * LayerIIDecoderBase._denormalMultiplier[this._scalefac[i][j][l]];
							}
							else
							{
								this._polyPhaseBuf[l] = 0f;
							}
							l++;
							num3++;
						}
						base.InversePolyPhase(i, this._polyPhaseBuf);
						Array.Copy(this._polyPhaseBuf, 0, array[i], num3 - 32, 32);
					}
				}
			}
			if (this._channels == 2 && base.StereoMode == StereoMode.DownmixToMono)
			{
				for (int m = 0; m < num3; m++)
				{
					ch0[m] = (ch0[m] + ch1[m]) / 2f;
				}
			}
			return num3;
		}

		protected const int SSLIMIT = 12;

		private static readonly float[] _groupedC = new float[]
		{
			0f,
			0f,
			1.3333334f,
			1.6f,
			1.7777778f
		};

		private static readonly float[] _groupedD = new float[]
		{
			0f,
			0f,
			-0.5f,
			-0.5f,
			-0.5f
		};

		private static readonly float[] _C = new float[]
		{
			0f,
			0f,
			1.3333334f,
			1.1428572f,
			1.0666667f,
			1.032258f,
			1.0158731f,
			1.007874f,
			1.0039216f,
			1.0019569f,
			1.0009775f,
			1.0004885f,
			1.0002443f,
			1.0001221f,
			1.000061f,
			1.0000305f,
			1.0000153f
		};

		private static readonly float[] _D = new float[]
		{
			0f,
			0f,
			-0.5f,
			-0.75f,
			-0.875f,
			-0.9375f,
			-0.96875f,
			-0.984375f,
			-0.9921875f,
			-0.99609375f,
			-0.9980469f,
			-0.99902344f,
			-0.9995117f,
			-0.99975586f,
			-0.9998779f,
			-0.99993896f,
			-0.9999695f
		};

		private static readonly float[] _denormalMultiplier = new float[]
		{
			2f,
			1.587401f,
			1.2599211f,
			1f,
			0.7937005f,
			0.62996054f,
			0.5f,
			0.39685026f,
			0.31498027f,
			0.25f,
			0.19842513f,
			0.15749013f,
			0.125f,
			0.099212565f,
			0.07874507f,
			0.0625f,
			0.049606282f,
			0.039372534f,
			0.03125f,
			0.024803141f,
			0.019686267f,
			0.015625f,
			0.012401571f,
			0.009843133f,
			0.0078125f,
			0.0062007853f,
			0.0049215667f,
			0.00390625f,
			0.0031003926f,
			0.0024607833f,
			0.001953125f,
			0.0015501963f,
			0.0012303917f,
			0.0009765625f,
			0.00077509816f,
			0.00061519584f,
			0.00048828125f,
			0.00038754908f,
			0.00030759792f,
			0.00024414062f,
			0.00019377454f,
			0.00015379896f,
			0.00012207031f,
			9.688727E-05f,
			7.689948E-05f,
			6.1035156E-05f,
			4.8443635E-05f,
			3.844974E-05f,
			3.0517578E-05f,
			2.4221818E-05f,
			1.922487E-05f,
			1.5258789E-05f,
			1.2110909E-05f,
			9.612435E-06f,
			7.6293945E-06f,
			6.0554544E-06f,
			4.8062175E-06f,
			3.8146973E-06f,
			3.0277272E-06f,
			2.4031087E-06f,
			1.9073486E-06f,
			1.5138636E-06f,
			1.2015544E-06f,
			9.536743E-07f
		};

		private int _channels;

		private int _jsbound;

		private int _granuleCount;

		private int[][] _allocLookupTable;

		private int[][] _scfsi;

		private int[][] _samples;

		private int[][][] _scalefac;

		private float[] _polyPhaseBuf;

		private int[][] _allocation;
	}
}
