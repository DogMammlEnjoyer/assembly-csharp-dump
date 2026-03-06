using System;
using System.Collections.Generic;

namespace Meta.Voice.NLayer.Decoder
{
	internal sealed class LayerIIIDecoder : LayerDecoderBase
	{
		internal static bool GetCRC(MpegFrame frame, ref uint crc)
		{
			int num = frame.GetSideDataSize();
			while (--num >= 0)
			{
				MpegFrame.UpdateCRC(frame.ReadBits(8), 8, ref crc);
			}
			return true;
		}

		internal LayerIIIDecoder()
		{
			this._tableSelect = new int[][][]
			{
				new int[][]
				{
					new int[3],
					new int[3]
				},
				new int[][]
				{
					new int[3],
					new int[3]
				}
			};
			this._subblockGain = new float[][][]
			{
				new float[][]
				{
					new float[3],
					new float[3]
				},
				new float[][]
				{
					new float[3],
					new float[3]
				}
			};
		}

		internal override int DecodeFrame(IMpegFrame frame, float[] ch0, float[] ch1)
		{
			this.ReadSideInfo(frame);
			if (!this._bitRes.AddBits(frame, this._mainDataBegin))
			{
				return 0;
			}
			this.PrepTables(frame);
			int num = 0;
			int num2 = this._channels - 1;
			if (this._channels == 1 || base.StereoMode == StereoMode.LeftOnly || base.StereoMode == StereoMode.DownmixToMono)
			{
				this._chanBufs[0] = ch0;
				num2 = 0;
			}
			else if (base.StereoMode == StereoMode.RightOnly)
			{
				this._chanBufs[1] = ch0;
				num = 1;
			}
			else
			{
				this._chanBufs[0] = ch0;
				this._chanBufs[1] = ch1;
			}
			int num3;
			if (frame.Version == MpegVersion.Version1)
			{
				num3 = 2;
			}
			else
			{
				num3 = 1;
			}
			int num4 = 0;
			for (int i = 0; i < num3; i++)
			{
				for (int j = 0; j < this._channels; j++)
				{
					int sfBits;
					if (frame.Version == MpegVersion.Version1)
					{
						sfBits = this.ReadScalefactors(i, j);
					}
					else
					{
						sfBits = this.ReadLsfScalefactors(i, j, frame.ChannelModeExtension);
					}
					this.ReadSamples(sfBits, i, j);
				}
				this.Stereo(frame.ChannelMode, frame.ChannelModeExtension, i, frame.Version != MpegVersion.Version1);
				for (int k = num; k <= num2; k++)
				{
					float[] array = this._samples[k];
					int num5 = this._blockType[i][k];
					bool flag = this._blockSplitFlag[i][k];
					bool flag2 = this._mixedBlockFlag[i][k];
					if (flag && num5 == 2)
					{
						if (flag2)
						{
							this.Reorder(array, true);
							this.AntiAlias(array, true);
						}
						else
						{
							this.Reorder(array, false);
						}
					}
					else
					{
						this.AntiAlias(array, false);
					}
					this._hybrid.Apply(array, k, num5, flag && flag2);
					this.FrequencyInversion(array);
					this.InversePolyphase(array, k, num4, this._chanBufs[k]);
				}
				num4 += 576;
			}
			return num4;
		}

		internal override void ResetForSeek()
		{
			base.ResetForSeek();
			this._hybrid.Reset();
			this._bitRes.Reset();
		}

		private void ReadSideInfo(IMpegFrame frame)
		{
			if (frame.Version == MpegVersion.Version1)
			{
				this._mainDataBegin = frame.ReadBits(9);
				if (frame.ChannelMode == MpegChannelMode.Mono)
				{
					this._privBits = frame.ReadBits(5);
					this._channels = 1;
				}
				else
				{
					this._privBits = frame.ReadBits(3);
					this._channels = 2;
				}
				for (int i = 0; i < this._channels; i++)
				{
					this._scfsi[i][0] = frame.ReadBits(1);
					this._scfsi[i][1] = frame.ReadBits(1);
					this._scfsi[i][2] = frame.ReadBits(1);
					this._scfsi[i][3] = frame.ReadBits(1);
				}
				for (int j = 0; j < 2; j++)
				{
					for (int k = 0; k < this._channels; k++)
					{
						this._part23Length[j][k] = frame.ReadBits(12);
						this._bigValues[j][k] = frame.ReadBits(9);
						this._globalGain[j][k] = LayerIIIDecoder.GAIN_TAB[frame.ReadBits(8)];
						this._scalefacCompress[j][k] = frame.ReadBits(4);
						this._blockSplitFlag[j][k] = (frame.ReadBits(1) == 1);
						if (this._blockSplitFlag[j][k])
						{
							this._blockType[j][k] = frame.ReadBits(2);
							this._mixedBlockFlag[j][k] = (frame.ReadBits(1) == 1);
							this._tableSelect[j][k][0] = frame.ReadBits(5);
							this._tableSelect[j][k][1] = frame.ReadBits(5);
							this._tableSelect[j][k][2] = 0;
							if (this._blockType[j][k] == 2 && !this._mixedBlockFlag[j][k])
							{
								this._regionAddress1[j][k] = 8;
							}
							else
							{
								this._regionAddress1[j][k] = 7;
							}
							this._regionAddress2[j][k] = 20 - this._regionAddress1[j][k];
							this._subblockGain[j][k][0] = (float)frame.ReadBits(3) * -2f;
							this._subblockGain[j][k][1] = (float)frame.ReadBits(3) * -2f;
							this._subblockGain[j][k][2] = (float)frame.ReadBits(3) * -2f;
						}
						else
						{
							this._tableSelect[j][k][0] = frame.ReadBits(5);
							this._tableSelect[j][k][1] = frame.ReadBits(5);
							this._tableSelect[j][k][2] = frame.ReadBits(5);
							this._regionAddress1[j][k] = frame.ReadBits(4);
							this._regionAddress2[j][k] = frame.ReadBits(3);
							this._blockType[j][k] = 0;
							this._subblockGain[j][k][0] = 0f;
							this._subblockGain[j][k][1] = 0f;
							this._subblockGain[j][k][2] = 0f;
						}
						this._preflag[j][k] = frame.ReadBits(1);
						this._scalefacScale[j][k] = 0.5f * (1f + (float)frame.ReadBits(1));
						this._count1TableSelect[j][k] = frame.ReadBits(1);
					}
				}
				return;
			}
			this._mainDataBegin = frame.ReadBits(8);
			if (frame.ChannelMode == MpegChannelMode.Mono)
			{
				this._privBits = frame.ReadBits(1);
				this._channels = 1;
			}
			else
			{
				this._privBits = frame.ReadBits(2);
				this._channels = 2;
			}
			int num = 0;
			for (int l = 0; l < this._channels; l++)
			{
				this._part23Length[num][l] = frame.ReadBits(12);
				this._bigValues[num][l] = frame.ReadBits(9);
				this._globalGain[num][l] = LayerIIIDecoder.GAIN_TAB[frame.ReadBits(8)];
				this._scalefacCompress[num][l] = frame.ReadBits(9);
				this._blockSplitFlag[num][l] = (frame.ReadBits(1) == 1);
				if (this._blockSplitFlag[num][l])
				{
					this._blockType[num][l] = frame.ReadBits(2);
					this._mixedBlockFlag[num][l] = (frame.ReadBits(1) == 1);
					this._tableSelect[num][l][0] = frame.ReadBits(5);
					this._tableSelect[num][l][1] = frame.ReadBits(5);
					this._tableSelect[num][l][2] = 0;
					if (this._blockType[num][l] == 2 && !this._mixedBlockFlag[num][l])
					{
						this._regionAddress1[num][l] = 8;
					}
					else
					{
						this._regionAddress1[num][l] = 7;
					}
					this._regionAddress2[num][l] = 20 - this._regionAddress1[num][l];
					this._subblockGain[num][l][0] = (float)frame.ReadBits(3) * -2f;
					this._subblockGain[num][l][1] = (float)frame.ReadBits(3) * -2f;
					this._subblockGain[num][l][2] = (float)frame.ReadBits(3) * -2f;
				}
				else
				{
					this._tableSelect[num][l][0] = frame.ReadBits(5);
					this._tableSelect[num][l][1] = frame.ReadBits(5);
					this._tableSelect[num][l][2] = frame.ReadBits(5);
					this._regionAddress1[num][l] = frame.ReadBits(4);
					this._regionAddress2[num][l] = frame.ReadBits(3);
					this._blockType[num][l] = 0;
					this._subblockGain[num][l][0] = 0f;
					this._subblockGain[num][l][1] = 0f;
					this._subblockGain[num][l][2] = 0f;
				}
				this._scalefacScale[num][l] = 0.5f * (1f + (float)frame.ReadBits(1));
				this._count1TableSelect[num][l] = frame.ReadBits(1);
			}
		}

		private void PrepTables(IMpegFrame frame)
		{
			if (this._cbLookupSR != frame.SampleRate)
			{
				int sampleRate = frame.SampleRate;
				if (sampleRate <= 16000)
				{
					if (sampleRate <= 11025)
					{
						if (sampleRate != 8000)
						{
							if (sampleRate == 11025)
							{
								this._sfBandIndexL = LayerIIIDecoder._sfBandIndexLTable[6];
								this._sfBandIndexS = LayerIIIDecoder._sfBandIndexSTable[6];
							}
						}
						else
						{
							this._sfBandIndexL = LayerIIIDecoder._sfBandIndexLTable[8];
							this._sfBandIndexS = LayerIIIDecoder._sfBandIndexSTable[8];
						}
					}
					else if (sampleRate != 12000)
					{
						if (sampleRate == 16000)
						{
							this._sfBandIndexL = LayerIIIDecoder._sfBandIndexLTable[5];
							this._sfBandIndexS = LayerIIIDecoder._sfBandIndexSTable[5];
						}
					}
					else
					{
						this._sfBandIndexL = LayerIIIDecoder._sfBandIndexLTable[7];
						this._sfBandIndexS = LayerIIIDecoder._sfBandIndexSTable[7];
					}
				}
				else if (sampleRate <= 24000)
				{
					if (sampleRate != 22050)
					{
						if (sampleRate == 24000)
						{
							this._sfBandIndexL = LayerIIIDecoder._sfBandIndexLTable[4];
							this._sfBandIndexS = LayerIIIDecoder._sfBandIndexSTable[4];
						}
					}
					else
					{
						this._sfBandIndexL = LayerIIIDecoder._sfBandIndexLTable[3];
						this._sfBandIndexS = LayerIIIDecoder._sfBandIndexSTable[3];
					}
				}
				else if (sampleRate != 32000)
				{
					if (sampleRate != 44100)
					{
						if (sampleRate == 48000)
						{
							this._sfBandIndexL = LayerIIIDecoder._sfBandIndexLTable[1];
							this._sfBandIndexS = LayerIIIDecoder._sfBandIndexSTable[1];
						}
					}
					else
					{
						this._sfBandIndexL = LayerIIIDecoder._sfBandIndexLTable[0];
						this._sfBandIndexS = LayerIIIDecoder._sfBandIndexSTable[0];
					}
				}
				else
				{
					this._sfBandIndexL = LayerIIIDecoder._sfBandIndexLTable[2];
					this._sfBandIndexS = LayerIIIDecoder._sfBandIndexSTable[2];
				}
				int num = 0;
				int i = 0;
				int num2 = this._sfBandIndexL[1];
				int num3 = this._sfBandIndexS[1] * 3;
				for (int j = 0; j < 576; j++)
				{
					if (j == num2)
					{
						num++;
						num2 = this._sfBandIndexL[num + 1];
					}
					if (j == num3)
					{
						i++;
						num3 = this._sfBandIndexS[i + 1] * 3;
					}
					this._cbLookupL[j] = (byte)num;
					this._cbLookupS[j] = (byte)i;
				}
				int num4 = 0;
				for (i = 0; i < 12; i++)
				{
					int num5 = this._sfBandIndexS[i + 1] - this._sfBandIndexS[i];
					for (int k = 0; k < 3; k++)
					{
						int l = 0;
						while (l < num5)
						{
							this._cbwLookupS[num4] = (byte)k;
							l++;
							num4++;
						}
					}
				}
				this._cbLookupSR = frame.SampleRate;
			}
		}

		private int ReadScalefactors(int gr, int ch)
		{
			int num = LayerIIIDecoder._slen[0][this._scalefacCompress[gr][ch]];
			int num2 = LayerIIIDecoder._slen[1][this._scalefacCompress[gr][ch]];
			int i = 0;
			int num3;
			if (this._blockSplitFlag[gr][ch] && this._blockType[gr][ch] == 2)
			{
				if (num > 0)
				{
					num3 = num * 18;
					if (this._mixedBlockFlag[gr][ch])
					{
						while (i < 8)
						{
							this._scalefac[ch][3][i] = this._bitRes.GetBits(num);
							i++;
						}
						i = 3;
						num3 -= num;
					}
					while (i < 6)
					{
						this._scalefac[ch][0][i] = this._bitRes.GetBits(num);
						this._scalefac[ch][1][i] = this._bitRes.GetBits(num);
						this._scalefac[ch][2][i] = this._bitRes.GetBits(num);
						i++;
					}
				}
				else
				{
					Array.Clear(this._scalefac[ch][3], 0, 8);
					Array.Clear(this._scalefac[ch][0], 0, 6);
					Array.Clear(this._scalefac[ch][1], 0, 6);
					Array.Clear(this._scalefac[ch][2], 0, 6);
					num3 = 0;
				}
				if (num2 > 0)
				{
					num3 += num2 * 18;
					for (i = 6; i < 12; i++)
					{
						this._scalefac[ch][0][i] = this._bitRes.GetBits(num2);
						this._scalefac[ch][1][i] = this._bitRes.GetBits(num2);
						this._scalefac[ch][2][i] = this._bitRes.GetBits(num2);
					}
				}
				else
				{
					Array.Clear(this._scalefac[ch][0], 6, 6);
					Array.Clear(this._scalefac[ch][1], 6, 6);
					Array.Clear(this._scalefac[ch][2], 6, 6);
				}
			}
			else
			{
				num3 = 0;
				if (gr == 0 || this._scfsi[ch][0] == 0)
				{
					if (num > 0)
					{
						num3 += num * 6;
						this._scalefac[ch][3][0] = this._bitRes.GetBits(num);
						this._scalefac[ch][3][1] = this._bitRes.GetBits(num);
						this._scalefac[ch][3][2] = this._bitRes.GetBits(num);
						this._scalefac[ch][3][3] = this._bitRes.GetBits(num);
						this._scalefac[ch][3][4] = this._bitRes.GetBits(num);
						this._scalefac[ch][3][5] = this._bitRes.GetBits(num);
					}
					else
					{
						Array.Clear(this._scalefac[ch][3], 0, 6);
					}
				}
				if (gr == 0 || this._scfsi[ch][1] == 0)
				{
					if (num > 0)
					{
						num3 += num * 5;
						this._scalefac[ch][3][6] = this._bitRes.GetBits(num);
						this._scalefac[ch][3][7] = this._bitRes.GetBits(num);
						this._scalefac[ch][3][8] = this._bitRes.GetBits(num);
						this._scalefac[ch][3][9] = this._bitRes.GetBits(num);
						this._scalefac[ch][3][10] = this._bitRes.GetBits(num);
					}
					else
					{
						Array.Clear(this._scalefac[ch][3], 6, 5);
					}
				}
				if (gr == 0 || this._scfsi[ch][2] == 0)
				{
					if (num2 > 0)
					{
						num3 += num2 * 5;
						this._scalefac[ch][3][11] = this._bitRes.GetBits(num2);
						this._scalefac[ch][3][12] = this._bitRes.GetBits(num2);
						this._scalefac[ch][3][13] = this._bitRes.GetBits(num2);
						this._scalefac[ch][3][14] = this._bitRes.GetBits(num2);
						this._scalefac[ch][3][15] = this._bitRes.GetBits(num2);
					}
					else
					{
						Array.Clear(this._scalefac[ch][3], 11, 5);
					}
				}
				if (gr == 0 || this._scfsi[ch][3] == 0)
				{
					if (num2 > 0)
					{
						num3 += num2 * 5;
						this._scalefac[ch][3][16] = this._bitRes.GetBits(num2);
						this._scalefac[ch][3][17] = this._bitRes.GetBits(num2);
						this._scalefac[ch][3][18] = this._bitRes.GetBits(num2);
						this._scalefac[ch][3][19] = this._bitRes.GetBits(num2);
						this._scalefac[ch][3][20] = this._bitRes.GetBits(num2);
					}
					else
					{
						Array.Clear(this._scalefac[ch][3], 16, 5);
					}
				}
			}
			return num3;
		}

		private int ReadLsfScalefactors(int gr, int ch, int chanModeExt)
		{
			int num = this._scalefacCompress[gr][ch];
			int num2;
			if (this._blockType[gr][ch] == 2)
			{
				if (this._mixedBlockFlag[gr][ch])
				{
					num2 = 2;
				}
				else
				{
					num2 = 1;
				}
			}
			else
			{
				num2 = 0;
			}
			int num4;
			if ((chanModeExt & 1) == 1 && ch == 1)
			{
				int num3 = num >> 1;
				if (num3 < 180)
				{
					this._readLsfScalefactorsSlen[0] = num3 / 36;
					this._readLsfScalefactorsSlen[1] = num3 % 36 / 6;
					this._readLsfScalefactorsSlen[2] = num3 % 6;
					this._readLsfScalefactorsSlen[3] = 0;
					this._preflag[gr][ch] = 0;
					num4 = 3;
				}
				else if (num3 < 244)
				{
					this._readLsfScalefactorsSlen[0] = (num3 - 180) % 64 >> 4;
					this._readLsfScalefactorsSlen[1] = (num3 - 180) % 16 >> 2;
					this._readLsfScalefactorsSlen[2] = (num3 - 180) % 4;
					this._readLsfScalefactorsSlen[3] = 0;
					this._preflag[gr][ch] = 0;
					num4 = 4;
				}
				else if (num3 < 255)
				{
					this._readLsfScalefactorsSlen[0] = (num3 - 244) / 3;
					this._readLsfScalefactorsSlen[1] = (num3 - 244) % 3;
					this._readLsfScalefactorsSlen[2] = 0;
					this._readLsfScalefactorsSlen[3] = 0;
					this._preflag[gr][ch] = 0;
					num4 = 5;
				}
				else
				{
					this._readLsfScalefactorsSlen[0] = 0;
					this._readLsfScalefactorsSlen[1] = 0;
					this._readLsfScalefactorsSlen[2] = 0;
					this._readLsfScalefactorsSlen[3] = 0;
					num4 = 0;
				}
			}
			else if (num < 400)
			{
				this._readLsfScalefactorsSlen[0] = (num >> 4) / 5;
				this._readLsfScalefactorsSlen[1] = (num >> 4) % 5;
				this._readLsfScalefactorsSlen[2] = (num & 15) >> 2;
				this._readLsfScalefactorsSlen[3] = (num & 3);
				this._preflag[gr][ch] = 0;
				num4 = 0;
			}
			else if (num < 500)
			{
				this._readLsfScalefactorsSlen[0] = (num - 400 >> 2) / 5;
				this._readLsfScalefactorsSlen[1] = (num - 400 >> 2) % 5;
				this._readLsfScalefactorsSlen[2] = (num - 400 & 3);
				this._readLsfScalefactorsSlen[3] = 0;
				this._preflag[gr][ch] = 0;
				num4 = 1;
			}
			else if (num < 512)
			{
				this._readLsfScalefactorsSlen[0] = (num - 500) / 3;
				this._readLsfScalefactorsSlen[1] = (num - 500) % 3;
				this._readLsfScalefactorsSlen[2] = 0;
				this._readLsfScalefactorsSlen[3] = 0;
				this._preflag[gr][ch] = 1;
				num4 = 2;
			}
			else
			{
				this._readLsfScalefactorsSlen[0] = 0;
				this._readLsfScalefactorsSlen[1] = 0;
				this._readLsfScalefactorsSlen[2] = 0;
				this._readLsfScalefactorsSlen[3] = 0;
				num4 = 0;
			}
			int num5 = 0;
			int[] array = LayerIIIDecoder._sfbBlockCntTab[num4][num2];
			for (int i = 0; i < 4; i++)
			{
				int j = 0;
				while (j < array[i])
				{
					this._readLsfScalefactorsBuffer[num5] = ((this._readLsfScalefactorsSlen[i] != 0) ? this._bitRes.GetBits(this._readLsfScalefactorsSlen[i]) : 0);
					j++;
					num5++;
				}
			}
			num5 = 0;
			int k = 0;
			if (this._blockSplitFlag[gr][ch] && this._blockType[gr][ch] == 2)
			{
				if (this._mixedBlockFlag[gr][ch])
				{
					while (k < 8)
					{
						this._scalefac[ch][3][k] = this._readLsfScalefactorsBuffer[num5++];
						k++;
					}
					k = 3;
				}
				while (k < 12)
				{
					for (int l = 0; l < 3; l++)
					{
						this._scalefac[ch][l][k] = this._readLsfScalefactorsBuffer[num5++];
					}
					k++;
				}
				this._scalefac[ch][0][12] = 0;
				this._scalefac[ch][1][12] = 0;
				this._scalefac[ch][2][12] = 0;
			}
			else
			{
				while (k < 21)
				{
					this._scalefac[ch][3][k] = this._readLsfScalefactorsBuffer[num5++];
					k++;
				}
				this._scalefac[ch][3][22] = 0;
			}
			return this._readLsfScalefactorsSlen[0] * array[0] + this._readLsfScalefactorsSlen[1] * array[1] + this._readLsfScalefactorsSlen[2] * array[2] + this._readLsfScalefactorsSlen[3] * array[3];
		}

		private void ReadSamples(int sfBits, int gr, int ch)
		{
			int num;
			int num2;
			if (this._blockSplitFlag[gr][ch] && this._blockType[gr][ch] == 2)
			{
				num = 36;
				num2 = 576;
			}
			else
			{
				num = this._sfBandIndexL[this._regionAddress1[gr][ch] + 1];
				num2 = this._sfBandIndexL[Math.Min(this._regionAddress1[gr][ch] + this._regionAddress2[gr][ch] + 2, 22)];
			}
			long num3 = this._bitRes.BitsRead - (long)sfBits + (long)this._part23Length[gr][ch];
			int i = 0;
			int table = this._tableSelect[gr][ch][0];
			int num4 = this._bigValues[gr][ch] * 2;
			while (i < num4 && i < num)
			{
				float val;
				float val2;
				Huffman.Decode(this._bitRes, table, out val, out val2);
				this._samples[ch][i] = this.Dequantize(i, val, gr, ch);
				i++;
				this._samples[ch][i] = this.Dequantize(i, val2, gr, ch);
				i++;
			}
			table = this._tableSelect[gr][ch][1];
			while (i < num4 && i < num2)
			{
				float val;
				float val2;
				Huffman.Decode(this._bitRes, table, out val, out val2);
				this._samples[ch][i] = this.Dequantize(i, val, gr, ch);
				i++;
				this._samples[ch][i] = this.Dequantize(i, val2, gr, ch);
				i++;
			}
			table = this._tableSelect[gr][ch][2];
			while (i < num4)
			{
				float val;
				float val2;
				Huffman.Decode(this._bitRes, table, out val, out val2);
				this._samples[ch][i] = this.Dequantize(i, val, gr, ch);
				i++;
				this._samples[ch][i] = this.Dequantize(i, val2, gr, ch);
				i++;
			}
			table = this._count1TableSelect[gr][ch] + 32;
			while (num3 > this._bitRes.BitsRead && i < 573)
			{
				float val;
				float val2;
				float val3;
				float val4;
				Huffman.Decode(this._bitRes, table, out val, out val2, out val3, out val4);
				this._samples[ch][i] = this.Dequantize(i, val3, gr, ch);
				i++;
				this._samples[ch][i] = this.Dequantize(i, val4, gr, ch);
				i++;
				this._samples[ch][i] = this.Dequantize(i, val, gr, ch);
				i++;
				this._samples[ch][i] = this.Dequantize(i, val2, gr, ch);
				i++;
			}
			if (this._bitRes.BitsRead > num3)
			{
				this._bitRes.RewindBits((int)(this._bitRes.BitsRead - num3));
				i -= 4;
				if (i < 0)
				{
					i = 0;
				}
			}
			if (this._bitRes.BitsRead < num3)
			{
				this._bitRes.SkipBits((int)(num3 - this._bitRes.BitsRead));
			}
			if (i < 576)
			{
				Array.Clear(this._samples[ch], i, 579 - i);
			}
		}

		private float Dequantize(int idx, float val, int gr, int ch)
		{
			if (val == 0f)
			{
				return 0f;
			}
			int num;
			if (this._blockSplitFlag[gr][ch] && this._blockType[gr][ch] == 2 && (!this._mixedBlockFlag[gr][ch] || idx >= this._sfBandIndexL[8]))
			{
				num = (int)this._cbLookupS[idx];
				int num2 = (int)this._cbwLookupS[idx];
				return val * this._globalGain[gr][ch] * LayerIIIDecoder.POW2_TAB[(int)(-2f * (this._subblockGain[gr][ch][num2] - this._scalefacScale[gr][ch] * (float)this._scalefac[ch][num2][num]))];
			}
			num = (int)this._cbLookupL[idx];
			return val * this._globalGain[gr][ch] * LayerIIIDecoder.POW2_TAB[(int)(2f * this._scalefacScale[gr][ch] * (float)(this._scalefac[ch][3][num] + this._preflag[gr][ch] * LayerIIIDecoder.PRETAB[num]))];
		}

		private void Stereo(MpegChannelMode channelMode, int chanModeExt, int gr, bool lsf)
		{
			if (channelMode != MpegChannelMode.JointStereo || chanModeExt == 0)
			{
				if (this._channels != 1)
				{
					this.ApplyFullStereo(0, 576);
				}
				return;
			}
			bool flag = (chanModeExt & 2) == 2;
			if ((chanModeExt & 1) == 1)
			{
				int num = -1;
				for (int i = 543; i >= 0; i--)
				{
					if (this._samples[1][i] != 0f)
					{
						num = i;
						break;
					}
				}
				int num2 = -1;
				int num3 = -1;
				if (this._blockSplitFlag[gr][0] && this._blockType[gr][0] == 2)
				{
					if (this._mixedBlockFlag[gr][0])
					{
						if (num < this._sfBandIndexL[8])
						{
							num2 = 8;
						}
						num3 = 3;
					}
					else
					{
						num3 = 0;
					}
				}
				else
				{
					num2 = 21;
				}
				int j = 0;
				if (num > -1)
				{
					j = (int)(this._cbLookupL[num] + 1);
				}
				if (j > 0 && num3 == -1)
				{
					if (flag)
					{
						this.ApplyMidSide(0, this._sfBandIndexL[j]);
					}
					else
					{
						this.ApplyFullStereo(0, this._sfBandIndexL[j]);
					}
				}
				while (j < num2)
				{
					int i2 = this._sfBandIndexL[j];
					int sb = this._sfBandIndexL[j + 1] - this._sfBandIndexL[j];
					int num4 = this._scalefac[1][3][j];
					if (num4 == 7)
					{
						if (flag)
						{
							this.ApplyMidSide(i2, sb);
						}
						else
						{
							this.ApplyFullStereo(i2, sb);
						}
					}
					else if (lsf)
					{
						this.ApplyLsfIStereo(i2, sb, num4, this._scalefacCompress[gr][0]);
					}
					else
					{
						this.ApplyIStereo(i2, sb, num4);
					}
					j++;
				}
				if (num3 > -1)
				{
					int[] array = new int[]
					{
						-1,
						-1,
						-1
					};
					int k;
					if (num > -1)
					{
						j = (int)this._cbLookupS[num];
						k = (int)this._cbwLookupS[num];
						array[k] = j;
					}
					else
					{
						j = 12;
						k = 3;
					}
					k = (k - 1) % 3;
					while (j >= num3 && k >= 0)
					{
						if (array[k] != -1)
						{
							if (array[0] != -1 && array[1] != -1 && array[2] != -1)
							{
								break;
							}
						}
						else
						{
							int num5 = this._sfBandIndexS[j + 1] - this._sfBandIndexS[j];
							int num6 = this._sfBandIndexS[j] * 3 + num5 * (k + 1);
							while (--num5 >= -1)
							{
								if (this._samples[1][--num6] != 0f)
								{
									array[k] = j;
									break;
								}
							}
							if (k == 0)
							{
								j--;
							}
						}
						k = (k - 1) % 3;
					}
					for (j = num3; j < 12; j++)
					{
						int num7 = this._sfBandIndexS[j + 1] - this._sfBandIndexS[j];
						int num8 = this._sfBandIndexS[j] * 3;
						for (k = 0; k < 3; k++)
						{
							if (j > array[k])
							{
								int num9 = this._scalefac[1][k][j];
								if (num9 == 7)
								{
									if (flag)
									{
										this.ApplyMidSide(num8, num7);
									}
									else
									{
										this.ApplyFullStereo(num8, num7);
									}
								}
								else if (lsf)
								{
									this.ApplyLsfIStereo(num8, num7, num9, this._scalefacCompress[gr][0]);
								}
								else
								{
									this.ApplyIStereo(num8, num7, num9);
								}
							}
							else if (flag)
							{
								this.ApplyMidSide(num8, num7);
							}
							else
							{
								this.ApplyFullStereo(num8, num7);
							}
							num8 += num7;
						}
					}
					int num10 = this._sfBandIndexS[13] - this._sfBandIndexS[12];
					for (k = 0; k < 3; k++)
					{
						int num11 = this._scalefac[1][k][11];
						if (num11 == 7)
						{
							if (flag)
							{
								this.ApplyMidSide(this._sfBandIndexS[11] * 3 + num10 * k, num10);
							}
							else
							{
								this.ApplyFullStereo(this._sfBandIndexS[11] * 3 + num10 * k, num10);
							}
						}
						else if (lsf)
						{
							this.ApplyLsfIStereo(this._sfBandIndexS[11] * 3 + num10 * k, num10, num11, this._scalefacCompress[gr][0]);
						}
						else
						{
							this.ApplyIStereo(this._sfBandIndexS[11] * 3 + num10 * k, num10, num11);
						}
					}
					return;
				}
				int num12 = this._scalefac[1][3][20];
				if (num12 == 7)
				{
					if (flag)
					{
						this.ApplyMidSide(this._sfBandIndexL[21], 576 - this._sfBandIndexL[21]);
						return;
					}
					this.ApplyFullStereo(this._sfBandIndexL[21], 576 - this._sfBandIndexL[21]);
					return;
				}
				else
				{
					if (lsf)
					{
						this.ApplyLsfIStereo(this._sfBandIndexL[21], 576 - this._sfBandIndexL[21], num12, this._scalefacCompress[gr][0]);
						return;
					}
					this.ApplyIStereo(this._sfBandIndexL[21], 576 - this._sfBandIndexL[21], num12);
					return;
				}
			}
			else
			{
				if (flag)
				{
					this.ApplyMidSide(0, 576);
					return;
				}
				this.ApplyFullStereo(0, 576);
				return;
			}
		}

		private void ApplyIStereo(int i, int sb, int isPos)
		{
			if (base.StereoMode == StereoMode.DownmixToMono)
			{
				while (sb > 0)
				{
					this._samples[0][i] /= 2f;
					sb--;
					i++;
				}
				return;
			}
			float num = LayerIIIDecoder._isRatio[0][isPos];
			float num2 = LayerIIIDecoder._isRatio[1][isPos];
			while (sb > 0)
			{
				this._samples[1][i] = this._samples[0][i] * num2;
				this._samples[0][i] *= num;
				sb--;
				i++;
			}
		}

		private void ApplyLsfIStereo(int i, int sb, int isPos, int scalefacCompress)
		{
			float num = LayerIIIDecoder._lsfRatio[scalefacCompress % 1][isPos][0];
			float num2 = LayerIIIDecoder._lsfRatio[scalefacCompress % 1][isPos][1];
			if (base.StereoMode == StereoMode.DownmixToMono)
			{
				float num3 = 1f / (num + num2);
				while (sb > 0)
				{
					this._samples[0][i] *= num3;
					sb--;
					i++;
				}
				return;
			}
			while (sb > 0)
			{
				this._samples[1][i] = this._samples[0][i] * num2;
				this._samples[0][i] *= num;
				sb--;
				i++;
			}
		}

		private void ApplyMidSide(int i, int sb)
		{
			if (base.StereoMode == StereoMode.DownmixToMono)
			{
				while (sb > 0)
				{
					this._samples[0][i] *= 0.70710677f;
					sb--;
					i++;
				}
				return;
			}
			while (sb > 0)
			{
				float num = this._samples[0][i];
				float num2 = this._samples[1][i];
				this._samples[0][i] = (num + num2) * 0.70710677f;
				this._samples[1][i] = (num - num2) * 0.70710677f;
				sb--;
				i++;
			}
		}

		private void ApplyFullStereo(int i, int sb)
		{
			if (base.StereoMode == StereoMode.DownmixToMono)
			{
				while (sb > 0)
				{
					this._samples[0][i] = (this._samples[0][i] + this._samples[1][i]) / 2f;
					sb--;
					i++;
				}
			}
		}

		private void Reorder(float[] buf, bool mixedBlock)
		{
			int i = 0;
			if (mixedBlock)
			{
				Array.Copy(buf, 0, this._reorderBuf, 0, 36);
				i = 3;
			}
			while (i < 13)
			{
				int num = this._sfBandIndexS[i];
				int num2 = this._sfBandIndexS[i + 1] - num;
				for (int j = 0; j < 3; j++)
				{
					for (int k = 0; k < num2; k++)
					{
						int num3 = num * 3 + j * num2 + k;
						int num4 = num * 3 + j + k * 3;
						this._reorderBuf[num4] = buf[num3];
					}
				}
				i++;
			}
			Array.Copy(this._reorderBuf, buf, 576);
		}

		private void AntiAlias(float[] buf, bool mixedBlock)
		{
			int num;
			if (mixedBlock)
			{
				num = 1;
			}
			else
			{
				num = 31;
			}
			int i = 0;
			int num2 = 0;
			while (i < num)
			{
				int j = 0;
				int num3 = num2 + 18 - 1;
				int num4 = num2 + 18;
				while (j < 8)
				{
					float num5 = buf[num3];
					float num6 = buf[num4];
					buf[num3] = num5 * LayerIIIDecoder._scs[j] - num6 * LayerIIIDecoder._sca[j];
					buf[num4] = num6 * LayerIIIDecoder._scs[j] + num5 * LayerIIIDecoder._sca[j];
					j++;
					num3--;
					num4++;
				}
				i++;
				num2 += 18;
			}
		}

		private void FrequencyInversion(float[] buf)
		{
			for (int i = 1; i < 18; i += 2)
			{
				for (int j = 1; j < 32; j += 2)
				{
					buf[j * 18 + i] = -buf[j * 18 + i];
				}
			}
		}

		private void InversePolyphase(float[] buf, int ch, int ofs, float[] outBuf)
		{
			int i = 0;
			while (i < 18)
			{
				for (int j = 0; j < 32; j++)
				{
					this._polyPhase[j] = buf[j * 18 + i];
				}
				base.InversePolyPhase(ch, this._polyPhase);
				Array.Copy(this._polyPhase, 0, outBuf, ofs, 32);
				i++;
				ofs += 32;
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static LayerIIIDecoder()
		{
			int[][][] array = new int[6][][];
			array[0] = new int[][]
			{
				new int[]
				{
					6,
					5,
					5,
					5
				},
				new int[]
				{
					9,
					9,
					9,
					9
				},
				new int[]
				{
					6,
					9,
					9,
					9
				}
			};
			array[1] = new int[][]
			{
				new int[]
				{
					6,
					5,
					7,
					3
				},
				new int[]
				{
					9,
					9,
					12,
					6
				},
				new int[]
				{
					6,
					9,
					12,
					6
				}
			};
			int num = 2;
			int[][] array2 = new int[3][];
			int num2 = 0;
			int[] array3 = new int[4];
			array3[0] = 11;
			array3[1] = 10;
			array2[num2] = array3;
			int num3 = 1;
			int[] array4 = new int[4];
			array4[0] = 18;
			array4[1] = 18;
			array2[num3] = array4;
			int num4 = 2;
			int[] array5 = new int[4];
			array5[0] = 15;
			array5[1] = 18;
			array2[num4] = array5;
			array[num] = array2;
			array[3] = new int[][]
			{
				new int[]
				{
					7,
					7,
					7,
					0
				},
				new int[]
				{
					12,
					12,
					12,
					0
				},
				new int[]
				{
					6,
					15,
					12,
					0
				}
			};
			array[4] = new int[][]
			{
				new int[]
				{
					6,
					6,
					6,
					3
				},
				new int[]
				{
					12,
					9,
					9,
					6
				},
				new int[]
				{
					6,
					12,
					9,
					6
				}
			};
			array[5] = new int[][]
			{
				new int[]
				{
					8,
					8,
					5,
					0
				},
				new int[]
				{
					15,
					12,
					9,
					0
				},
				new int[]
				{
					6,
					18,
					9,
					0
				}
			};
			LayerIIIDecoder._sfbBlockCntTab = array;
			LayerIIIDecoder.PRETAB = new int[]
			{
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				1,
				1,
				1,
				2,
				2,
				3,
				3,
				3,
				2,
				0
			};
			LayerIIIDecoder.POW2_TAB = new float[]
			{
				1f,
				0.70710677f,
				0.5f,
				0.35355338f,
				0.25f,
				0.17677669f,
				0.125f,
				0.088388346f,
				0.0625f,
				0.044194173f,
				0.03125f,
				0.022097087f,
				0.015625f,
				0.011048543f,
				0.0078125f,
				0.0055242716f,
				0.00390625f,
				0.0027621358f,
				0.001953125f,
				0.0013810679f,
				0.0009765625f,
				0.00069053395f,
				0.00048828125f,
				0.00034526698f,
				0.00024414062f,
				0.00017263349f,
				0.00012207031f,
				8.6316744E-05f,
				6.1035156E-05f,
				4.3158372E-05f,
				3.0517578E-05f,
				2.1579186E-05f,
				1.5258789E-05f,
				1.0789593E-05f,
				7.6293945E-06f,
				5.3947965E-06f,
				3.8146973E-06f,
				2.6973983E-06f,
				1.9073486E-06f,
				1.3486991E-06f,
				9.536743E-07f,
				6.7434956E-07f,
				4.7683716E-07f,
				3.3717478E-07f,
				2.3841858E-07f,
				1.6858739E-07f,
				1.1920929E-07f,
				8.4293696E-08f,
				5.9604645E-08f,
				4.2146848E-08f,
				2.9802322E-08f,
				2.1073424E-08f,
				1.4901161E-08f,
				1.0536712E-08f,
				7.450581E-09f,
				5.268356E-09f,
				3.7252903E-09f,
				2.634178E-09f,
				1.8626451E-09f,
				1.317089E-09f,
				9.313226E-10f,
				6.585445E-10f,
				4.656613E-10f,
				3.2927225E-10f
			};
			LayerIIIDecoder._isRatio = new float[][]
			{
				new float[]
				{
					0f,
					0.21132487f,
					0.36602542f,
					0.5f,
					0.6339746f,
					0.7886751f,
					1f
				},
				new float[]
				{
					1f,
					0.7886751f,
					0.6339746f,
					0.5f,
					0.36602542f,
					0.21132487f,
					0f
				}
			};
			LayerIIIDecoder._lsfRatio = new float[][][]
			{
				new float[][]
				{
					new float[]
					{
						1f,
						0.8408964f,
						1f,
						0.70710677f,
						1f,
						0.59460354f,
						1f,
						0.5f,
						1f,
						0.4204482f,
						1f,
						0.35355338f,
						1f,
						0.29730177f,
						1f,
						0.25f,
						1f,
						0.2102241f,
						1f,
						0.17677669f,
						1f,
						0.14865088f,
						1f,
						0.125f,
						1f,
						0.10511205f,
						1f,
						0.088388346f,
						1f,
						0.07432544f,
						1f,
						0.0625f
					},
					new float[]
					{
						1f,
						1f,
						0.8408964f,
						1f,
						0.70710677f,
						1f,
						0.59460354f,
						1f,
						0.5f,
						1f,
						0.4204482f,
						1f,
						0.35355338f,
						1f,
						0.29730177f,
						1f,
						0.25f,
						1f,
						0.2102241f,
						1f,
						0.17677669f,
						1f,
						0.14865088f,
						1f,
						0.125f,
						1f,
						0.10511205f,
						1f,
						0.088388346f,
						1f,
						0.07432544f,
						1f
					}
				},
				new float[][]
				{
					new float[]
					{
						1f,
						0.70710677f,
						1f,
						0.5f,
						1f,
						0.35355338f,
						1f,
						0.25f,
						1f,
						0.17677669f,
						1f,
						0.125f,
						1f,
						0.088388346f,
						1f,
						0.0625f,
						1f,
						0.044194173f,
						1f,
						0.03125f,
						1f,
						0.022097087f,
						1f,
						0.015625f,
						1f,
						0.011048543f,
						1f,
						0.0078125f,
						1f,
						0.0055242716f,
						1f,
						0.00390625f
					},
					new float[]
					{
						1f,
						1f,
						0.70710677f,
						1f,
						0.5f,
						1f,
						0.35355338f,
						1f,
						0.25f,
						1f,
						0.17677669f,
						1f,
						0.125f,
						1f,
						0.088388346f,
						1f,
						0.0625f,
						1f,
						0.044194173f,
						1f,
						0.03125f,
						1f,
						0.022097087f,
						1f,
						0.015625f,
						1f,
						0.011048543f,
						1f,
						0.0078125f,
						1f,
						0.0055242716f,
						1f
					}
				}
			};
			LayerIIIDecoder._scs = new float[]
			{
				0.8574929f,
				0.881742f,
				0.94962865f,
				0.9833146f,
				0.9955178f,
				0.9991606f,
				0.9998992f,
				0.99999315f
			};
			LayerIIIDecoder._sca = new float[]
			{
				-0.51449573f,
				-0.47173196f,
				-0.31337744f,
				-0.1819132f,
				-0.09457419f,
				-0.040965583f,
				-0.014198569f,
				-0.0036999746f
			};
		}

		private const int SSLIMIT = 18;

		private readonly float[][] _chanBufs = new float[2][];

		private readonly int[] _readLsfScalefactorsSlen = new int[4];

		private readonly int[] _readLsfScalefactorsBuffer = new int[54];

		private readonly LayerIIIDecoder.HybridMDCT _hybrid = new LayerIIIDecoder.HybridMDCT();

		private BitReservoir _bitRes = new BitReservoir();

		private int _channels;

		private int _privBits;

		private int _mainDataBegin;

		private int[][] _scfsi = new int[][]
		{
			new int[4],
			new int[4]
		};

		private int[][] _part23Length = new int[][]
		{
			new int[2],
			new int[2]
		};

		private int[][] _bigValues = new int[][]
		{
			new int[2],
			new int[2]
		};

		private float[][] _globalGain = new float[][]
		{
			new float[2],
			new float[2]
		};

		private int[][] _scalefacCompress = new int[][]
		{
			new int[2],
			new int[2]
		};

		private bool[][] _blockSplitFlag = new bool[][]
		{
			new bool[2],
			new bool[2]
		};

		private bool[][] _mixedBlockFlag = new bool[][]
		{
			new bool[2],
			new bool[2]
		};

		private int[][] _blockType = new int[][]
		{
			new int[2],
			new int[2]
		};

		private int[][][] _tableSelect;

		private float[][][] _subblockGain;

		private int[][] _regionAddress1 = new int[][]
		{
			new int[2],
			new int[2]
		};

		private int[][] _regionAddress2 = new int[][]
		{
			new int[2],
			new int[2]
		};

		private int[][] _preflag = new int[][]
		{
			new int[2],
			new int[2]
		};

		private float[][] _scalefacScale = new float[][]
		{
			new float[2],
			new float[2]
		};

		private int[][] _count1TableSelect = new int[][]
		{
			new int[2],
			new int[2]
		};

		private static float[] GAIN_TAB = new float[]
		{
			1.5700924E-16f,
			1.8671652E-16f,
			2.220446E-16f,
			2.6405702E-16f,
			3.1401849E-16f,
			3.7343303E-16f,
			4.440892E-16f,
			5.2811403E-16f,
			6.2803697E-16f,
			7.4686606E-16f,
			8.881784E-16f,
			1.0562281E-15f,
			1.2560739E-15f,
			1.4937321E-15f,
			1.7763568E-15f,
			2.1124561E-15f,
			2.5121479E-15f,
			2.9874642E-15f,
			3.5527137E-15f,
			4.2249122E-15f,
			5.0242958E-15f,
			5.9749285E-15f,
			7.1054274E-15f,
			8.4498245E-15f,
			1.00485916E-14f,
			1.1949857E-14f,
			1.4210855E-14f,
			1.6899649E-14f,
			2.0097183E-14f,
			2.3899714E-14f,
			2.842171E-14f,
			3.3799298E-14f,
			4.0194366E-14f,
			4.7799428E-14f,
			5.684342E-14f,
			6.7598596E-14f,
			8.038873E-14f,
			9.5598856E-14f,
			1.1368684E-13f,
			1.3519719E-13f,
			1.6077747E-13f,
			1.9119771E-13f,
			2.2737368E-13f,
			2.7039438E-13f,
			3.2155493E-13f,
			3.8239542E-13f,
			4.5474735E-13f,
			5.4078877E-13f,
			6.4310986E-13f,
			7.6479085E-13f,
			9.094947E-13f,
			1.0815775E-12f,
			1.2862197E-12f,
			1.5295817E-12f,
			1.8189894E-12f,
			2.163155E-12f,
			2.5724394E-12f,
			3.0591634E-12f,
			3.637979E-12f,
			4.32631E-12f,
			5.144879E-12f,
			6.1183268E-12f,
			7.275958E-12f,
			8.65262E-12f,
			1.0289758E-11f,
			1.22366535E-11f,
			1.4551915E-11f,
			1.730524E-11f,
			2.0579516E-11f,
			2.4473307E-11f,
			2.910383E-11f,
			3.461048E-11f,
			4.115903E-11f,
			4.8946614E-11f,
			5.820766E-11f,
			6.922096E-11f,
			8.231806E-11f,
			9.789323E-11f,
			1.1641532E-10f,
			1.3844192E-10f,
			1.6463612E-10f,
			1.9578646E-10f,
			2.3283064E-10f,
			2.7688385E-10f,
			3.2927225E-10f,
			3.915729E-10f,
			4.656613E-10f,
			5.537677E-10f,
			6.585445E-10f,
			7.831458E-10f,
			9.313226E-10f,
			1.1075354E-09f,
			1.317089E-09f,
			1.5662917E-09f,
			1.8626451E-09f,
			2.2150708E-09f,
			2.634178E-09f,
			3.1325833E-09f,
			3.7252903E-09f,
			4.4301416E-09f,
			5.268356E-09f,
			6.2651666E-09f,
			7.450581E-09f,
			8.860283E-09f,
			1.0536712E-08f,
			1.2530333E-08f,
			1.4901161E-08f,
			1.7720566E-08f,
			2.1073424E-08f,
			2.5060666E-08f,
			2.9802322E-08f,
			3.5441133E-08f,
			4.2146848E-08f,
			5.0121333E-08f,
			5.9604645E-08f,
			7.0882265E-08f,
			8.4293696E-08f,
			1.00242666E-07f,
			1.1920929E-07f,
			1.4176453E-07f,
			1.6858739E-07f,
			2.0048533E-07f,
			2.3841858E-07f,
			2.8352906E-07f,
			3.3717478E-07f,
			4.0097066E-07f,
			4.7683716E-07f,
			5.670581E-07f,
			6.7434956E-07f,
			8.019413E-07f,
			9.536743E-07f,
			1.1341162E-06f,
			1.3486991E-06f,
			1.6038827E-06f,
			1.9073486E-06f,
			2.2682325E-06f,
			2.6973983E-06f,
			3.2077653E-06f,
			3.8146973E-06f,
			4.536465E-06f,
			5.3947965E-06f,
			6.4155306E-06f,
			7.6293945E-06f,
			9.07293E-06f,
			1.0789593E-05f,
			1.2831061E-05f,
			1.5258789E-05f,
			1.814586E-05f,
			2.1579186E-05f,
			2.5662122E-05f,
			3.0517578E-05f,
			3.629172E-05f,
			4.3158372E-05f,
			5.1324245E-05f,
			6.1035156E-05f,
			7.258344E-05f,
			8.6316744E-05f,
			0.00010264849f,
			0.00012207031f,
			0.00014516688f,
			0.00017263349f,
			0.00020529698f,
			0.00024414062f,
			0.00029033376f,
			0.00034526698f,
			0.00041059396f,
			0.00048828125f,
			0.0005806675f,
			0.00069053395f,
			0.0008211879f,
			0.0009765625f,
			0.001161335f,
			0.0013810679f,
			0.0016423758f,
			0.001953125f,
			0.00232267f,
			0.0027621358f,
			0.0032847517f,
			0.00390625f,
			0.00464534f,
			0.0055242716f,
			0.0065695033f,
			0.0078125f,
			0.00929068f,
			0.011048543f,
			0.013139007f,
			0.015625f,
			0.01858136f,
			0.022097087f,
			0.026278013f,
			0.03125f,
			0.03716272f,
			0.044194173f,
			0.052556027f,
			0.0625f,
			0.07432544f,
			0.088388346f,
			0.10511205f,
			0.125f,
			0.14865088f,
			0.17677669f,
			0.2102241f,
			0.25f,
			0.29730177f,
			0.35355338f,
			0.4204482f,
			0.5f,
			0.59460354f,
			0.70710677f,
			0.8408964f,
			1f,
			1.1892071f,
			1.4142135f,
			1.6817929f,
			2f,
			2.3784142f,
			2.828427f,
			3.3635857f,
			4f,
			4.7568283f,
			5.656854f,
			6.7271714f,
			8f,
			9.513657f,
			11.313708f,
			13.454343f,
			16f,
			19.027313f,
			22.627417f,
			26.908686f,
			32f,
			38.054626f,
			45.254833f,
			53.81737f,
			64f,
			76.10925f,
			90.50967f,
			107.63474f,
			128f,
			152.2185f,
			181.01933f,
			215.26949f,
			256f,
			304.437f,
			362.03867f,
			430.53897f,
			512f,
			608.874f,
			724.07733f,
			861.07794f,
			1024f,
			1217.748f,
			1448.1547f,
			1722.1559f,
			2048f,
			2435.496f
		};

		private int[] _sfBandIndexL;

		private int[] _sfBandIndexS;

		private byte[] _cbLookupL = new byte[576];

		private byte[] _cbLookupS = new byte[576];

		private byte[] _cbwLookupS = new byte[576];

		private int _cbLookupSR;

		private static readonly int[][] _sfBandIndexLTable = new int[][]
		{
			new int[]
			{
				0,
				4,
				8,
				12,
				16,
				20,
				24,
				30,
				36,
				44,
				52,
				62,
				74,
				90,
				110,
				134,
				162,
				196,
				238,
				288,
				342,
				418,
				576
			},
			new int[]
			{
				0,
				4,
				8,
				12,
				16,
				20,
				24,
				30,
				36,
				42,
				50,
				60,
				72,
				88,
				106,
				128,
				156,
				190,
				230,
				276,
				330,
				384,
				576
			},
			new int[]
			{
				0,
				4,
				8,
				12,
				16,
				20,
				24,
				30,
				36,
				44,
				54,
				66,
				82,
				102,
				126,
				156,
				194,
				240,
				296,
				364,
				448,
				550,
				576
			},
			new int[]
			{
				0,
				6,
				12,
				18,
				24,
				30,
				36,
				44,
				54,
				66,
				80,
				96,
				116,
				140,
				168,
				200,
				238,
				284,
				336,
				396,
				464,
				522,
				576
			},
			new int[]
			{
				0,
				6,
				12,
				18,
				24,
				30,
				36,
				44,
				54,
				66,
				80,
				96,
				114,
				136,
				162,
				194,
				232,
				278,
				330,
				394,
				464,
				540,
				576
			},
			new int[]
			{
				0,
				6,
				12,
				18,
				24,
				30,
				36,
				44,
				54,
				66,
				80,
				96,
				116,
				140,
				168,
				200,
				238,
				284,
				336,
				396,
				464,
				522,
				576
			},
			new int[]
			{
				0,
				6,
				12,
				18,
				24,
				30,
				36,
				44,
				54,
				66,
				80,
				96,
				116,
				140,
				168,
				200,
				238,
				284,
				336,
				396,
				464,
				522,
				576
			},
			new int[]
			{
				0,
				6,
				12,
				18,
				24,
				30,
				36,
				44,
				54,
				66,
				80,
				96,
				116,
				140,
				168,
				200,
				238,
				284,
				336,
				396,
				464,
				522,
				576
			},
			new int[]
			{
				0,
				12,
				24,
				36,
				48,
				60,
				72,
				88,
				108,
				132,
				160,
				192,
				232,
				280,
				336,
				400,
				476,
				566,
				568,
				570,
				572,
				574,
				576
			}
		};

		private static readonly int[][] _sfBandIndexSTable = new int[][]
		{
			new int[]
			{
				0,
				4,
				8,
				12,
				16,
				22,
				30,
				40,
				52,
				66,
				84,
				106,
				136,
				192
			},
			new int[]
			{
				0,
				4,
				8,
				12,
				16,
				22,
				28,
				38,
				50,
				64,
				80,
				100,
				126,
				192
			},
			new int[]
			{
				0,
				4,
				8,
				12,
				16,
				22,
				30,
				42,
				58,
				78,
				104,
				138,
				180,
				192
			},
			new int[]
			{
				0,
				4,
				8,
				12,
				18,
				24,
				32,
				42,
				56,
				74,
				100,
				132,
				174,
				192
			},
			new int[]
			{
				0,
				4,
				8,
				12,
				18,
				26,
				36,
				48,
				62,
				80,
				104,
				136,
				180,
				192
			},
			new int[]
			{
				0,
				4,
				8,
				12,
				18,
				26,
				36,
				48,
				62,
				80,
				104,
				134,
				174,
				192
			},
			new int[]
			{
				0,
				4,
				8,
				12,
				18,
				26,
				36,
				48,
				62,
				80,
				104,
				134,
				174,
				192
			},
			new int[]
			{
				0,
				4,
				8,
				12,
				18,
				26,
				36,
				48,
				62,
				80,
				104,
				134,
				174,
				192
			},
			new int[]
			{
				0,
				8,
				16,
				24,
				36,
				52,
				72,
				96,
				124,
				160,
				162,
				164,
				166,
				192
			}
		};

		private int[][][] _scalefac = new int[][][]
		{
			new int[][]
			{
				new int[13],
				new int[13],
				new int[13],
				new int[23]
			},
			new int[][]
			{
				new int[13],
				new int[13],
				new int[13],
				new int[23]
			}
		};

		private static readonly int[][] _slen = new int[][]
		{
			new int[]
			{
				0,
				0,
				0,
				0,
				3,
				1,
				1,
				1,
				2,
				2,
				2,
				3,
				3,
				3,
				4,
				4
			},
			new int[]
			{
				0,
				1,
				2,
				3,
				0,
				1,
				2,
				3,
				1,
				2,
				3,
				1,
				2,
				3,
				2,
				3
			}
		};

		private static readonly int[][][] _sfbBlockCntTab;

		private float[][] _samples = new float[][]
		{
			new float[579],
			new float[579]
		};

		private static readonly int[] PRETAB;

		private static readonly float[] POW2_TAB;

		private static readonly float[][] _isRatio;

		private static readonly float[][][] _lsfRatio;

		private float[] _reorderBuf = new float[576];

		private static readonly float[] _scs;

		private static readonly float[] _sca;

		private float[] _polyPhase = new float[32];

		private class HybridMDCT
		{
			static HybridMDCT()
			{
				LayerIIIDecoder.HybridMDCT._swin = new float[][]
				{
					new float[36],
					new float[36],
					new float[36],
					new float[36]
				};
				for (int i = 0; i < 36; i++)
				{
					LayerIIIDecoder.HybridMDCT._swin[0][i] = (float)Math.Sin(0.0872664675116539 * ((double)i + 0.5));
				}
				for (int i = 0; i < 18; i++)
				{
					LayerIIIDecoder.HybridMDCT._swin[1][i] = (float)Math.Sin(0.0872664675116539 * ((double)i + 0.5));
				}
				for (int i = 18; i < 24; i++)
				{
					LayerIIIDecoder.HybridMDCT._swin[1][i] = 1f;
				}
				for (int i = 24; i < 30; i++)
				{
					LayerIIIDecoder.HybridMDCT._swin[1][i] = (float)Math.Sin(0.2617993950843811 * ((double)i + 0.5 - 18.0));
				}
				for (int i = 30; i < 36; i++)
				{
					LayerIIIDecoder.HybridMDCT._swin[1][i] = 0f;
				}
				for (int i = 0; i < 6; i++)
				{
					LayerIIIDecoder.HybridMDCT._swin[3][i] = 0f;
				}
				for (int i = 6; i < 12; i++)
				{
					LayerIIIDecoder.HybridMDCT._swin[3][i] = (float)Math.Sin(0.2617993950843811 * ((double)i + 0.5 - 6.0));
				}
				for (int i = 12; i < 18; i++)
				{
					LayerIIIDecoder.HybridMDCT._swin[3][i] = 1f;
				}
				for (int i = 18; i < 36; i++)
				{
					LayerIIIDecoder.HybridMDCT._swin[3][i] = (float)Math.Sin(0.0872664675116539 * ((double)i + 0.5));
				}
				for (int i = 0; i < 12; i++)
				{
					LayerIIIDecoder.HybridMDCT._swin[2][i] = (float)Math.Sin(0.2617993950843811 * ((double)i + 0.5));
				}
				for (int i = 12; i < 36; i++)
				{
					LayerIIIDecoder.HybridMDCT._swin[2][i] = 0f;
				}
			}

			internal HybridMDCT()
			{
				this._prevBlock = new List<float[]>();
				this._nextBlock = new List<float[]>();
			}

			internal void Reset()
			{
				this._prevBlock.Clear();
				this._nextBlock.Clear();
			}

			private void GetPrevBlock(int channel, out float[] prevBlock, out float[] nextBlock)
			{
				while (this._prevBlock.Count <= channel)
				{
					if (this._prevBlock.Count == 0)
					{
						this._prevBlock.Add(this._prevBlockFirst);
					}
					else
					{
						this._prevBlock.Add(new float[576]);
					}
				}
				while (this._nextBlock.Count <= channel)
				{
					if (this._nextBlock.Count == 0)
					{
						this._nextBlock.Add(this._nextBlockFirst);
					}
					else
					{
						this._nextBlock.Add(new float[576]);
					}
				}
				prevBlock = this._prevBlock[channel];
				nextBlock = this._nextBlock[channel];
				this._nextBlock[channel] = prevBlock;
				this._prevBlock[channel] = nextBlock;
			}

			internal void Apply(float[] fsIn, int channel, int blockType, bool doMixed)
			{
				float[] array;
				float[] nextblck;
				this.GetPrevBlock(channel, out array, out nextblck);
				int sbStart = 0;
				if (doMixed)
				{
					this.LongImpl(fsIn, 0, 2, nextblck, 0);
					sbStart = 2;
				}
				if (blockType == 2)
				{
					this.ShortImpl(fsIn, sbStart, nextblck);
				}
				else
				{
					this.LongImpl(fsIn, sbStart, 32, nextblck, blockType);
				}
				for (int i = 0; i < 576; i++)
				{
					fsIn[i] += array[i];
				}
			}

			private void LongImpl(float[] fsIn, int sbStart, int sbLimit, float[] nextblck, int blockType)
			{
				int i = sbStart;
				int num = sbStart * 18;
				while (i < sbLimit)
				{
					Array.Copy(fsIn, num, this._imdctTemp, 0, 18);
					this.LongIMDCT(this._imdctTemp, this._imdctResult);
					float[] array = LayerIIIDecoder.HybridMDCT._swin[blockType];
					int j;
					for (j = 0; j < 18; j++)
					{
						fsIn[num++] = this._imdctResult[j] * array[j];
					}
					num -= 18;
					while (j < 36)
					{
						nextblck[num++] = this._imdctResult[j] * array[j];
						j++;
					}
					i++;
				}
			}

			private void LongIMDCT(float[] invec, float[] outvec)
			{
				float[] imdct_H = this._imdct_H;
				float[] imdct_h = this._imdct_h;
				float[] imdct_even = this._imdct_even;
				float[] imdct_odd = this._imdct_odd;
				float[] imdct_even_idct = this._imdct_even_idct;
				float[] imdct_odd_idct = this._imdct_odd_idct;
				int i;
				for (i = 0; i < 17; i++)
				{
					imdct_H[i] = invec[i] + invec[i + 1];
				}
				imdct_even[0] = invec[0];
				imdct_odd[0] = imdct_H[0];
				int num = 0;
				i = 1;
				while (i < 9)
				{
					imdct_even[i] = imdct_H[num + 1];
					imdct_odd[i] = imdct_H[num] + imdct_H[num + 2];
					i++;
					num += 2;
				}
				this.imdct_9pt(imdct_even, imdct_even_idct);
				this.imdct_9pt(imdct_odd, imdct_odd_idct);
				for (i = 0; i < 9; i++)
				{
					imdct_odd_idct[i] *= LayerIIIDecoder.HybridMDCT.ICOS36_A(i);
					imdct_h[i] = (imdct_even_idct[i] + imdct_odd_idct[i]) * LayerIIIDecoder.HybridMDCT.ICOS72_A(i);
				}
				while (i < 18)
				{
					imdct_h[i] = (imdct_even_idct[17 - i] - imdct_odd_idct[17 - i]) * LayerIIIDecoder.HybridMDCT.ICOS72_A(i);
					i++;
				}
				outvec[0] = imdct_h[9];
				outvec[1] = imdct_h[10];
				outvec[2] = imdct_h[11];
				outvec[3] = imdct_h[12];
				outvec[4] = imdct_h[13];
				outvec[5] = imdct_h[14];
				outvec[6] = imdct_h[15];
				outvec[7] = imdct_h[16];
				outvec[8] = imdct_h[17];
				outvec[9] = -imdct_h[17];
				outvec[10] = -imdct_h[16];
				outvec[11] = -imdct_h[15];
				outvec[12] = -imdct_h[14];
				outvec[13] = -imdct_h[13];
				outvec[14] = -imdct_h[12];
				outvec[15] = -imdct_h[11];
				outvec[16] = -imdct_h[10];
				outvec[17] = -imdct_h[9];
				outvec[35] = (outvec[18] = -imdct_h[8]);
				outvec[34] = (outvec[19] = -imdct_h[7]);
				outvec[33] = (outvec[20] = -imdct_h[6]);
				outvec[32] = (outvec[21] = -imdct_h[5]);
				outvec[31] = (outvec[22] = -imdct_h[4]);
				outvec[30] = (outvec[23] = -imdct_h[3]);
				outvec[29] = (outvec[24] = -imdct_h[2]);
				outvec[28] = (outvec[25] = -imdct_h[1]);
				outvec[27] = (outvec[26] = -imdct_h[0]);
			}

			private static float ICOS72_A(int i)
			{
				return LayerIIIDecoder.HybridMDCT.icos72_table[2 * i];
			}

			private static float ICOS36_A(int i)
			{
				return LayerIIIDecoder.HybridMDCT.icos72_table[4 * i + 1];
			}

			private void imdct_9pt(float[] invec, float[] outvec)
			{
				float[] imdct_9pt_even_idct = this._imdct_9pt_even_idct;
				float[] imdct_9pt_odd_idct = this._imdct_9pt_odd_idct;
				float num = invec[6] / 2f + invec[0];
				float num2 = invec[0] - invec[6];
				float num3 = invec[2] - invec[4] - invec[8];
				imdct_9pt_even_idct[0] = num + invec[2] * 0.9396926f + invec[4] * 0.76604444f + invec[8] * 0.17364818f;
				imdct_9pt_even_idct[1] = num3 / 2f + num2;
				imdct_9pt_even_idct[2] = num - invec[2] * 0.17364818f - invec[4] * 0.9396926f + invec[8] * 0.76604444f;
				imdct_9pt_even_idct[3] = num - invec[2] * 0.76604444f + invec[4] * 0.17364818f - invec[8] * 0.9396926f;
				imdct_9pt_even_idct[4] = num2 - num3;
				float num4 = invec[1] + invec[3];
				float num5 = invec[3] + invec[5];
				num = (invec[5] + invec[7]) * 0.5f + invec[1];
				imdct_9pt_odd_idct[0] = num + num4 * 0.9396926f + num5 * 0.76604444f;
				imdct_9pt_odd_idct[1] = (invec[1] - invec[5]) * 1.5f - invec[7];
				imdct_9pt_odd_idct[2] = num - num4 * 0.17364818f - num5 * 0.9396926f;
				imdct_9pt_odd_idct[3] = num - num4 * 0.76604444f + num5 * 0.17364818f;
				imdct_9pt_odd_idct[0] += invec[7] * 0.17364818f;
				imdct_9pt_odd_idct[1] -= invec[7] * 0.5f;
				imdct_9pt_odd_idct[2] += invec[7] * 0.76604444f;
				imdct_9pt_odd_idct[3] -= invec[7] * 0.9396926f;
				imdct_9pt_odd_idct[0] *= 0.5077133f;
				imdct_9pt_odd_idct[1] *= 0.57735026f;
				imdct_9pt_odd_idct[2] *= 0.7778619f;
				imdct_9pt_odd_idct[3] *= 1.4619021f;
				for (int i = 0; i < 4; i++)
				{
					outvec[i] = imdct_9pt_even_idct[i] + imdct_9pt_odd_idct[i];
				}
				outvec[4] = imdct_9pt_even_idct[4];
				for (int i = 5; i < 9; i++)
				{
					outvec[i] = imdct_9pt_even_idct[8 - i] - imdct_9pt_odd_idct[8 - i];
				}
			}

			private void ShortImpl(float[] fsIn, int sbStart, float[] nextblck)
			{
				float[] array = LayerIIIDecoder.HybridMDCT._swin[2];
				int i = sbStart;
				int num = sbStart * 18;
				while (i < 32)
				{
					int j = 0;
					int num2 = 0;
					while (j < 3)
					{
						int num3 = num + j;
						for (int k = 0; k < 6; k++)
						{
							this._imdctTemp[num2 + k] = fsIn[num3];
							num3 += 3;
						}
						num2 += 6;
						j++;
					}
					Array.Clear(fsIn, num, 6);
					this.ShortIMDCT(this._imdctTemp, 0, this._imdctResult);
					Array.Copy(this._imdctResult, 0, fsIn, num + 6, 12);
					this.ShortIMDCT(this._imdctTemp, 6, this._imdctResult);
					for (int l = 0; l < 6; l++)
					{
						fsIn[num + l + 12] += this._imdctResult[l];
					}
					Array.Copy(this._imdctResult, 6, nextblck, num, 6);
					this.ShortIMDCT(this._imdctTemp, 12, this._imdctResult);
					for (int m = 0; m < 6; m++)
					{
						nextblck[num + m] += this._imdctResult[m];
					}
					Array.Copy(this._imdctResult, 6, nextblck, num + 6, 6);
					Array.Clear(nextblck, num + 12, 6);
					i++;
					num += 18;
				}
			}

			private void ShortIMDCT(float[] invec, int inIdx, float[] outvec)
			{
				float[] shortIMDCT_H = this._ShortIMDCT_H;
				float[] shortIMDCT_h = this._ShortIMDCT_h;
				float[] shortIMDCT_even_idct = this._ShortIMDCT_even_idct;
				float[] shortIMDCT_odd_idct = this._ShortIMDCT_odd_idct;
				int num = inIdx;
				for (int i = 1; i < 6; i++)
				{
					shortIMDCT_H[i] = invec[num];
					shortIMDCT_H[i] += invec[++num];
				}
				float num2 = shortIMDCT_H[4] / 2f + invec[inIdx];
				float num3 = shortIMDCT_H[2] * 0.8660254f;
				shortIMDCT_even_idct[0] = num2 + num3;
				shortIMDCT_even_idct[1] = invec[inIdx] - shortIMDCT_H[4];
				shortIMDCT_even_idct[2] = num2 - num3;
				float num4 = shortIMDCT_H[3] + shortIMDCT_H[5];
				num2 = num4 / 2f + shortIMDCT_H[1];
				num3 = (shortIMDCT_H[1] + shortIMDCT_H[3]) * 0.8660254f;
				shortIMDCT_odd_idct[0] = num2 + num3;
				shortIMDCT_odd_idct[1] = shortIMDCT_H[1] - num4;
				shortIMDCT_odd_idct[2] = num2 - num3;
				shortIMDCT_odd_idct[0] *= 0.5176381f;
				shortIMDCT_odd_idct[1] *= 0.70710677f;
				shortIMDCT_odd_idct[2] *= 1.9318516f;
				shortIMDCT_h[0] = (shortIMDCT_even_idct[0] + shortIMDCT_odd_idct[0]) * 0.5043145f;
				shortIMDCT_h[1] = (shortIMDCT_even_idct[1] + shortIMDCT_odd_idct[1]) * 0.5411961f;
				shortIMDCT_h[2] = (shortIMDCT_even_idct[2] + shortIMDCT_odd_idct[2]) * 0.6302362f;
				shortIMDCT_h[3] = (shortIMDCT_even_idct[2] - shortIMDCT_odd_idct[2]) * 0.82133985f;
				shortIMDCT_h[4] = (shortIMDCT_even_idct[1] - shortIMDCT_odd_idct[1]) * 1.306563f;
				shortIMDCT_h[5] = (shortIMDCT_even_idct[0] - shortIMDCT_odd_idct[0]) * 3.830649f;
				outvec[0] = shortIMDCT_h[3] * LayerIIIDecoder.HybridMDCT._swin[2][0];
				outvec[1] = shortIMDCT_h[4] * LayerIIIDecoder.HybridMDCT._swin[2][1];
				outvec[2] = shortIMDCT_h[5] * LayerIIIDecoder.HybridMDCT._swin[2][2];
				outvec[3] = -shortIMDCT_h[5] * LayerIIIDecoder.HybridMDCT._swin[2][3];
				outvec[4] = -shortIMDCT_h[4] * LayerIIIDecoder.HybridMDCT._swin[2][4];
				outvec[5] = -shortIMDCT_h[3] * LayerIIIDecoder.HybridMDCT._swin[2][5];
				outvec[6] = -shortIMDCT_h[2] * LayerIIIDecoder.HybridMDCT._swin[2][6];
				outvec[7] = -shortIMDCT_h[1] * LayerIIIDecoder.HybridMDCT._swin[2][7];
				outvec[8] = -shortIMDCT_h[0] * LayerIIIDecoder.HybridMDCT._swin[2][8];
				outvec[9] = -shortIMDCT_h[0] * LayerIIIDecoder.HybridMDCT._swin[2][9];
				outvec[10] = -shortIMDCT_h[1] * LayerIIIDecoder.HybridMDCT._swin[2][10];
				outvec[11] = -shortIMDCT_h[2] * LayerIIIDecoder.HybridMDCT._swin[2][11];
			}

			private const float PI = 3.1415927f;

			private static float[][] _swin;

			private static float[] icos72_table = new float[]
			{
				0.50047636f,
				0.5019099f,
				0.5043145f,
				0.5077133f,
				0.51213974f,
				0.5176381f,
				0.5242646f,
				0.5320889f,
				0.5411961f,
				0.55168897f,
				0.56369096f,
				0.57735026f,
				0.59284455f,
				0.61038727f,
				0.6302362f,
				0.65270364f,
				0.67817086f,
				0.70710677f,
				0.7400936f,
				0.7778619f,
				0.8213398f,
				0.8717234f,
				0.9305795f,
				1f,
				1.0828403f,
				1.1831008f,
				1.306563f,
				1.4619021f,
				1.6627548f,
				1.9318516f,
				2.3101132f,
				2.8793852f,
				3.830649f,
				5.7368565f,
				11.462792f
			};

			private List<float[]> _prevBlock;

			private List<float[]> _nextBlock;

			private float[] _prevBlockFirst = new float[576];

			private float[] _nextBlockFirst = new float[576];

			private float[] _imdctTemp = new float[18];

			private float[] _imdctResult = new float[36];

			private readonly float[] _imdct_H = new float[17];

			private readonly float[] _imdct_h = new float[18];

			private readonly float[] _imdct_even = new float[9];

			private readonly float[] _imdct_odd = new float[9];

			private readonly float[] _imdct_even_idct = new float[9];

			private readonly float[] _imdct_odd_idct = new float[9];

			private readonly float[] _imdct_9pt_even_idct = new float[5];

			private readonly float[] _imdct_9pt_odd_idct = new float[4];

			private const float sqrt32 = 0.8660254f;

			private readonly float[] _ShortIMDCT_H = new float[6];

			private readonly float[] _ShortIMDCT_h = new float[6];

			private readonly float[] _ShortIMDCT_even_idct = new float[3];

			private readonly float[] _ShortIMDCT_odd_idct = new float[3];
		}
	}
}
