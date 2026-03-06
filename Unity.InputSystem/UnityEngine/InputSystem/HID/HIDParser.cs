using System;
using System.Collections.Generic;

namespace UnityEngine.InputSystem.HID
{
	internal static class HIDParser
	{
		public unsafe static bool ParseReportDescriptor(byte[] buffer, ref HID.HIDDeviceDescriptor deviceDescriptor)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			byte* bufferPtr;
			if (buffer == null || buffer.Length == 0)
			{
				bufferPtr = null;
			}
			else
			{
				bufferPtr = &buffer[0];
			}
			return HIDParser.ParseReportDescriptor(bufferPtr, buffer.Length, ref deviceDescriptor);
		}

		public unsafe static bool ParseReportDescriptor(byte* bufferPtr, int bufferLength, ref HID.HIDDeviceDescriptor deviceDescriptor)
		{
			HIDParser.HIDItemStateLocal hiditemStateLocal = default(HIDParser.HIDItemStateLocal);
			HIDParser.HIDItemStateGlobal hiditemStateGlobal = default(HIDParser.HIDItemStateGlobal);
			List<HIDParser.HIDReportData> list = new List<HIDParser.HIDReportData>();
			List<HID.HIDElementDescriptor> list2 = new List<HID.HIDElementDescriptor>();
			List<HID.HIDCollectionDescriptor> list3 = new List<HID.HIDCollectionDescriptor>();
			int num = -1;
			byte* ptr = bufferPtr + bufferLength;
			byte* ptr2 = bufferPtr;
			while (ptr2 < ptr)
			{
				byte b = *ptr2;
				if (b == 254)
				{
					throw new NotImplementedException("long item support");
				}
				byte b2 = b & 3;
				byte b3 = b & 252;
				ptr2++;
				if (b3 <= 84)
				{
					if (b3 <= 24)
					{
						if (b3 <= 8)
						{
							if (b3 != 4)
							{
								if (b3 == 8)
								{
									hiditemStateLocal.SetUsage(HIDParser.ReadData((int)b2, ptr2, ptr));
								}
							}
							else
							{
								hiditemStateGlobal.usagePage = new int?(HIDParser.ReadData((int)b2, ptr2, ptr));
							}
						}
						else if (b3 != 20)
						{
							if (b3 == 24)
							{
								hiditemStateLocal.usageMinimum = new int?(HIDParser.ReadData((int)b2, ptr2, ptr));
							}
						}
						else
						{
							hiditemStateGlobal.logicalMinimum = new int?(HIDParser.ReadData((int)b2, ptr2, ptr));
						}
					}
					else if (b3 <= 40)
					{
						if (b3 != 36)
						{
							if (b3 == 40)
							{
								hiditemStateLocal.usageMaximum = new int?(HIDParser.ReadData((int)b2, ptr2, ptr));
							}
						}
						else
						{
							hiditemStateGlobal.logicalMaximum = new int?(HIDParser.ReadData((int)b2, ptr2, ptr));
						}
					}
					else if (b3 != 52)
					{
						if (b3 != 68)
						{
							if (b3 == 84)
							{
								hiditemStateGlobal.unitExponent = new int?(HIDParser.ReadData((int)b2, ptr2, ptr));
							}
						}
						else
						{
							hiditemStateGlobal.physicalMaximum = new int?(HIDParser.ReadData((int)b2, ptr2, ptr));
						}
					}
					else
					{
						hiditemStateGlobal.physicalMinimum = new int?(HIDParser.ReadData((int)b2, ptr2, ptr));
					}
				}
				else
				{
					if (b3 <= 132)
					{
						if (b3 <= 116)
						{
							if (b3 == 100)
							{
								hiditemStateGlobal.unit = new int?(HIDParser.ReadData((int)b2, ptr2, ptr));
								goto IL_521;
							}
							if (b3 != 116)
							{
								goto IL_521;
							}
							hiditemStateGlobal.reportSize = new int?(HIDParser.ReadData((int)b2, ptr2, ptr));
							goto IL_521;
						}
						else if (b3 != 128)
						{
							if (b3 != 132)
							{
								goto IL_521;
							}
							hiditemStateGlobal.reportId = new int?(HIDParser.ReadData((int)b2, ptr2, ptr));
							goto IL_521;
						}
					}
					else if (b3 <= 148)
					{
						if (b3 != 144)
						{
							if (b3 != 148)
							{
								goto IL_521;
							}
							hiditemStateGlobal.reportCount = new int?(HIDParser.ReadData((int)b2, ptr2, ptr));
							goto IL_521;
						}
					}
					else
					{
						if (b3 == 160)
						{
							int parent = num;
							num = list3.Count;
							list3.Add(new HID.HIDCollectionDescriptor
							{
								type = (HID.HIDCollectionType)HIDParser.ReadData((int)b2, ptr2, ptr),
								parent = parent,
								usagePage = hiditemStateGlobal.GetUsagePage(0, ref hiditemStateLocal),
								usage = hiditemStateLocal.GetUsage(0),
								firstChild = list2.Count
							});
							HIDParser.HIDItemStateLocal.Reset(ref hiditemStateLocal);
							goto IL_521;
						}
						if (b3 != 176)
						{
							if (b3 != 192)
							{
								goto IL_521;
							}
							if (num == -1)
							{
								return false;
							}
							HID.HIDCollectionDescriptor hidcollectionDescriptor = list3[num];
							hidcollectionDescriptor.childCount = list2.Count - hidcollectionDescriptor.firstChild;
							list3[num] = hidcollectionDescriptor;
							num = hidcollectionDescriptor.parent;
							HIDParser.HIDItemStateLocal.Reset(ref hiditemStateLocal);
							goto IL_521;
						}
					}
					HID.HIDReportType reportType = (b3 == 128) ? HID.HIDReportType.Input : ((b3 == 144) ? HID.HIDReportType.Output : HID.HIDReportType.Feature);
					int index = HIDParser.HIDReportData.FindOrAddReport(hiditemStateGlobal.reportId, reportType, list);
					HIDParser.HIDReportData hidreportData = list[index];
					if (hidreportData.currentBitOffset == 0 && hiditemStateGlobal.reportId != null)
					{
						hidreportData.currentBitOffset = 8;
					}
					int valueOrDefault = hiditemStateGlobal.reportCount.GetValueOrDefault(1);
					int flags = HIDParser.ReadData((int)b2, ptr2, ptr);
					for (int i = 0; i < valueOrDefault; i++)
					{
						HID.HIDElementDescriptor hidelementDescriptor = new HID.HIDElementDescriptor
						{
							usage = (hiditemStateLocal.GetUsage(i) & 65535),
							usagePage = hiditemStateGlobal.GetUsagePage(i, ref hiditemStateLocal),
							reportType = reportType,
							reportSizeInBits = hiditemStateGlobal.reportSize.GetValueOrDefault(8),
							reportOffsetInBits = hidreportData.currentBitOffset,
							reportId = hiditemStateGlobal.reportId.GetValueOrDefault(1),
							flags = (HID.HIDElementFlags)flags,
							logicalMin = hiditemStateGlobal.logicalMinimum.GetValueOrDefault(0),
							logicalMax = hiditemStateGlobal.logicalMaximum.GetValueOrDefault(0),
							physicalMin = hiditemStateGlobal.GetPhysicalMin(),
							physicalMax = hiditemStateGlobal.GetPhysicalMax(),
							unitExponent = hiditemStateGlobal.unitExponent.GetValueOrDefault(0),
							unit = hiditemStateGlobal.unit.GetValueOrDefault(0)
						};
						hidreportData.currentBitOffset += hidelementDescriptor.reportSizeInBits;
						list2.Add(hidelementDescriptor);
					}
					list[index] = hidreportData;
					HIDParser.HIDItemStateLocal.Reset(ref hiditemStateLocal);
				}
				IL_521:
				if (b2 == 3)
				{
					ptr2 += 4;
				}
				else
				{
					ptr2 += b2;
				}
			}
			deviceDescriptor.elements = list2.ToArray();
			deviceDescriptor.collections = list3.ToArray();
			foreach (HID.HIDCollectionDescriptor hidcollectionDescriptor2 in list3)
			{
				if (hidcollectionDescriptor2.parent == -1 && hidcollectionDescriptor2.type == HID.HIDCollectionType.Application)
				{
					deviceDescriptor.usage = hidcollectionDescriptor2.usage;
					deviceDescriptor.usagePage = hidcollectionDescriptor2.usagePage;
					break;
				}
			}
			return true;
		}

		private unsafe static int ReadData(int itemSize, byte* currentPtr, byte* endPtr)
		{
			if (itemSize == 0)
			{
				return 0;
			}
			if (itemSize == 1)
			{
				if (currentPtr >= endPtr)
				{
					return 0;
				}
				return (int)(*currentPtr);
			}
			else if (itemSize == 2)
			{
				if (currentPtr + 2 >= endPtr)
				{
					return 0;
				}
				byte b = *currentPtr;
				return (int)currentPtr[1] << 8 | (int)b;
			}
			else
			{
				if (itemSize != 3)
				{
					return 0;
				}
				if (currentPtr + 4 >= endPtr)
				{
					return 0;
				}
				byte b2 = *currentPtr;
				byte b3 = currentPtr[1];
				byte b4 = currentPtr[2];
				return (int)currentPtr[3] << 24 | (int)b4 << 24 | (int)b3 << 8 | (int)b2;
			}
		}

		private struct HIDReportData
		{
			public static int FindOrAddReport(int? reportId, HID.HIDReportType reportType, List<HIDParser.HIDReportData> reports)
			{
				int num = 1;
				if (reportId != null)
				{
					num = reportId.Value;
				}
				for (int i = 0; i < reports.Count; i++)
				{
					if (reports[i].reportId == num && reports[i].reportType == reportType)
					{
						return i;
					}
				}
				reports.Add(new HIDParser.HIDReportData
				{
					reportId = num,
					reportType = reportType
				});
				return reports.Count - 1;
			}

			public int reportId;

			public HID.HIDReportType reportType;

			public int currentBitOffset;
		}

		private enum HIDItemTypeAndTag
		{
			Input = 128,
			Output = 144,
			Feature = 176,
			Collection = 160,
			EndCollection = 192,
			UsagePage = 4,
			LogicalMinimum = 20,
			LogicalMaximum = 36,
			PhysicalMinimum = 52,
			PhysicalMaximum = 68,
			UnitExponent = 84,
			Unit = 100,
			ReportSize = 116,
			ReportID = 132,
			ReportCount = 148,
			Push = 164,
			Pop = 180,
			Usage = 8,
			UsageMinimum = 24,
			UsageMaximum = 40,
			DesignatorIndex = 56,
			DesignatorMinimum = 72,
			DesignatorMaximum = 88,
			StringIndex = 120,
			StringMinimum = 136,
			StringMaximum = 152,
			Delimiter = 168
		}

		private struct HIDItemStateLocal
		{
			public static void Reset(ref HIDParser.HIDItemStateLocal state)
			{
				List<int> list = state.usageList;
				state = default(HIDParser.HIDItemStateLocal);
				if (list != null)
				{
					list.Clear();
					state.usageList = list;
				}
			}

			public void SetUsage(int value)
			{
				if (this.usage != null)
				{
					if (this.usageList == null)
					{
						this.usageList = new List<int>();
					}
					this.usageList.Add(this.usage.Value);
				}
				this.usage = new int?(value);
			}

			public int GetUsage(int index)
			{
				if (this.usageMinimum != null && this.usageMaximum != null)
				{
					int value = this.usageMinimum.Value;
					int value2 = this.usageMaximum.Value;
					int num = value2 - value;
					if (num < 0)
					{
						return 0;
					}
					if (index >= num)
					{
						return value2;
					}
					return value + index;
				}
				else if (this.usageList != null && this.usageList.Count > 0)
				{
					int count = this.usageList.Count;
					if (index >= count)
					{
						return this.usage.Value;
					}
					return this.usageList[index];
				}
				else
				{
					if (this.usage != null)
					{
						return this.usage.Value;
					}
					return 0;
				}
			}

			public int? usage;

			public int? usageMinimum;

			public int? usageMaximum;

			public int? designatorIndex;

			public int? designatorMinimum;

			public int? designatorMaximum;

			public int? stringIndex;

			public int? stringMinimum;

			public int? stringMaximum;

			public List<int> usageList;
		}

		private struct HIDItemStateGlobal
		{
			public HID.UsagePage GetUsagePage(int index, ref HIDParser.HIDItemStateLocal localItemState)
			{
				if (this.usagePage == null)
				{
					return (HID.UsagePage)(localItemState.GetUsage(index) >> 16);
				}
				return (HID.UsagePage)this.usagePage.Value;
			}

			public int GetPhysicalMin()
			{
				if (this.physicalMinimum == null || this.physicalMaximum == null || (this.physicalMinimum.Value == 0 && this.physicalMaximum.Value == 0))
				{
					return this.logicalMinimum.GetValueOrDefault(0);
				}
				return this.physicalMinimum.Value;
			}

			public int GetPhysicalMax()
			{
				if (this.physicalMinimum == null || this.physicalMaximum == null || (this.physicalMinimum.Value == 0 && this.physicalMaximum.Value == 0))
				{
					return this.logicalMaximum.GetValueOrDefault(0);
				}
				return this.physicalMaximum.Value;
			}

			public int? usagePage;

			public int? logicalMinimum;

			public int? logicalMaximum;

			public int? physicalMinimum;

			public int? physicalMaximum;

			public int? unitExponent;

			public int? unit;

			public int? reportSize;

			public int? reportCount;

			public int? reportId;
		}
	}
}
