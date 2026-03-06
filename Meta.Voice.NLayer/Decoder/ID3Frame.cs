using System;

namespace Meta.Voice.NLayer.Decoder
{
	internal class ID3Frame : FrameBase
	{
		internal static ID3Frame TrySync(uint syncMark)
		{
			if ((syncMark & 4294967040U) == 1229206272U)
			{
				return new ID3Frame
				{
					_version = 2
				};
			}
			if ((syncMark & 4294967040U) != 1413564160U)
			{
				return null;
			}
			if ((syncMark & 255U) == 43U)
			{
				return new ID3Frame
				{
					_version = 1
				};
			}
			return new ID3Frame
			{
				_version = 0
			};
		}

		private ID3Frame()
		{
		}

		protected override int Validate()
		{
			switch (this._version)
			{
			case 0:
				return 128;
			case 1:
				return 355;
			case 2:
			{
				byte[] array = new byte[7];
				if (base.Read(3, array) == 7)
				{
					byte b;
					switch (array[0])
					{
					case 2:
						b = 63;
						break;
					case 3:
						b = 31;
						break;
					case 4:
						b = 15;
						break;
					default:
						return -1;
					}
					int num = (int)array[3] << 21 | (int)array[4] << 14 | (int)array[5] << 7 | (int)array[6];
					if (((array[2] & b) | (array[3] & 128) | (array[4] & 128) | (array[5] & 128) | (array[6] & 128)) == 0 && array[1] != 255)
					{
						return num + 10;
					}
				}
				break;
			}
			}
			return -1;
		}

		internal override void Parse()
		{
			switch (this._version)
			{
			case 0:
				this.ParseV1(3);
				return;
			case 1:
				this.ParseV1Enh();
				return;
			case 2:
				this.ParseV2();
				return;
			default:
				return;
			}
		}

		private void ParseV1(int offset)
		{
		}

		private void ParseV1Enh()
		{
			this.ParseV1(230);
		}

		private void ParseV2()
		{
		}

		internal int Version
		{
			get
			{
				if (this._version == 0)
				{
					return 1;
				}
				return this._version;
			}
		}

		internal void Merge(ID3Frame newFrame)
		{
		}

		private int _version;
	}
}
