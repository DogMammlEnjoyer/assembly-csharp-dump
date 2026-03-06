using System;

namespace System.Drawing.Printing
{
	/// <summary>Specifies the standard paper sizes.</summary>
	public enum PaperKind
	{
		/// <summary>The paper size is defined by the user.</summary>
		Custom,
		/// <summary>Letter paper (8.5 in. by 11 in.).</summary>
		Letter,
		/// <summary>Legal paper (8.5 in. by 14 in.).</summary>
		Legal = 5,
		/// <summary>A4 paper (210 mm by 297 mm).</summary>
		A4 = 9,
		/// <summary>C paper (17 in. by 22 in.).</summary>
		CSheet = 24,
		/// <summary>D paper (22 in. by 34 in.).</summary>
		DSheet,
		/// <summary>E paper (34 in. by 44 in.).</summary>
		ESheet,
		/// <summary>Letter small paper (8.5 in. by 11 in.).</summary>
		LetterSmall = 2,
		/// <summary>Tabloid paper (11 in. by 17 in.).</summary>
		Tabloid,
		/// <summary>Ledger paper (17 in. by 11 in.).</summary>
		Ledger,
		/// <summary>Statement paper (5.5 in. by 8.5 in.).</summary>
		Statement = 6,
		/// <summary>Executive paper (7.25 in. by 10.5 in.).</summary>
		Executive,
		/// <summary>A3 paper (297 mm by 420 mm).</summary>
		A3,
		/// <summary>A4 small paper (210 mm by 297 mm).</summary>
		A4Small = 10,
		/// <summary>A5 paper (148 mm by 210 mm).</summary>
		A5,
		/// <summary>B4 paper (250 mm by 353 mm).</summary>
		B4,
		/// <summary>B5 paper (176 mm by 250 mm).</summary>
		B5,
		/// <summary>Folio paper (8.5 in. by 13 in.).</summary>
		Folio,
		/// <summary>Quarto paper (215 mm by 275 mm).</summary>
		Quarto,
		/// <summary>Standard paper (10 in. by 14 in.).</summary>
		Standard10x14,
		/// <summary>Standard paper (11 in. by 17 in.).</summary>
		Standard11x17,
		/// <summary>Note paper (8.5 in. by 11 in.).</summary>
		Note,
		/// <summary>#9 envelope (3.875 in. by 8.875 in.).</summary>
		Number9Envelope,
		/// <summary>#10 envelope (4.125 in. by 9.5 in.).</summary>
		Number10Envelope,
		/// <summary>#11 envelope (4.5 in. by 10.375 in.).</summary>
		Number11Envelope,
		/// <summary>#12 envelope (4.75 in. by 11 in.).</summary>
		Number12Envelope,
		/// <summary>#14 envelope (5 in. by 11.5 in.).</summary>
		Number14Envelope,
		/// <summary>DL envelope (110 mm by 220 mm).</summary>
		DLEnvelope = 27,
		/// <summary>C5 envelope (162 mm by 229 mm).</summary>
		C5Envelope,
		/// <summary>C3 envelope (324 mm by 458 mm).</summary>
		C3Envelope,
		/// <summary>C4 envelope (229 mm by 324 mm).</summary>
		C4Envelope,
		/// <summary>C6 envelope (114 mm by 162 mm).</summary>
		C6Envelope,
		/// <summary>C65 envelope (114 mm by 229 mm).</summary>
		C65Envelope,
		/// <summary>B4 envelope (250 mm by 353 mm).</summary>
		B4Envelope,
		/// <summary>B5 envelope (176 mm by 250 mm).</summary>
		B5Envelope,
		/// <summary>B6 envelope (176 mm by 125 mm).</summary>
		B6Envelope,
		/// <summary>Italy envelope (110 mm by 230 mm).</summary>
		ItalyEnvelope,
		/// <summary>Monarch envelope (3.875 in. by 7.5 in.).</summary>
		MonarchEnvelope,
		/// <summary>6 3/4 envelope (3.625 in. by 6.5 in.).</summary>
		PersonalEnvelope,
		/// <summary>US standard fanfold (14.875 in. by 11 in.).</summary>
		USStandardFanfold,
		/// <summary>German standard fanfold (8.5 in. by 12 in.).</summary>
		GermanStandardFanfold,
		/// <summary>German legal fanfold (8.5 in. by 13 in.).</summary>
		GermanLegalFanfold,
		/// <summary>ISO B4 (250 mm by 353 mm).</summary>
		IsoB4,
		/// <summary>Japanese postcard (100 mm by 148 mm).</summary>
		JapanesePostcard,
		/// <summary>Standard paper (9 in. by 11 in.).</summary>
		Standard9x11,
		/// <summary>Standard paper (10 in. by 11 in.).</summary>
		Standard10x11,
		/// <summary>Standard paper (15 in. by 11 in.).</summary>
		Standard15x11,
		/// <summary>Invitation envelope (220 mm by 220 mm).</summary>
		InviteEnvelope,
		/// <summary>Letter extra paper (9.275 in. by 12 in.). This value is specific to the PostScript driver and is used only by Linotronic printers in order to conserve paper.</summary>
		LetterExtra = 50,
		/// <summary>Legal extra paper (9.275 in. by 15 in.). This value is specific to the PostScript driver and is used only by Linotronic printers in order to conserve paper.</summary>
		LegalExtra,
		/// <summary>Tabloid extra paper (11.69 in. by 18 in.). This value is specific to the PostScript driver and is used only by Linotronic printers in order to conserve paper.</summary>
		TabloidExtra,
		/// <summary>A4 extra paper (236 mm by 322 mm). This value is specific to the PostScript driver and is used only by Linotronic printers to help save paper.</summary>
		A4Extra,
		/// <summary>Letter transverse paper (8.275 in. by 11 in.).</summary>
		LetterTransverse,
		/// <summary>A4 transverse paper (210 mm by 297 mm).</summary>
		A4Transverse,
		/// <summary>Letter extra transverse paper (9.275 in. by 12 in.).</summary>
		LetterExtraTransverse,
		/// <summary>SuperA/SuperA/A4 paper (227 mm by 356 mm).</summary>
		APlus,
		/// <summary>SuperB/SuperB/A3 paper (305 mm by 487 mm).</summary>
		BPlus,
		/// <summary>Letter plus paper (8.5 in. by 12.69 in.).</summary>
		LetterPlus,
		/// <summary>A4 plus paper (210 mm by 330 mm).</summary>
		A4Plus,
		/// <summary>A5 transverse paper (148 mm by 210 mm).</summary>
		A5Transverse,
		/// <summary>JIS B5 transverse paper (182 mm by 257 mm).</summary>
		B5Transverse,
		/// <summary>A3 extra paper (322 mm by 445 mm).</summary>
		A3Extra,
		/// <summary>A5 extra paper (174 mm by 235 mm).</summary>
		A5Extra,
		/// <summary>ISO B5 extra paper (201 mm by 276 mm).</summary>
		B5Extra,
		/// <summary>A2 paper (420 mm by 594 mm).</summary>
		A2,
		/// <summary>A3 transverse paper (297 mm by 420 mm).</summary>
		A3Transverse,
		/// <summary>A3 extra transverse paper (322 mm by 445 mm).</summary>
		A3ExtraTransverse,
		/// <summary>Japanese double postcard (200 mm by 148 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		JapaneseDoublePostcard,
		/// <summary>A6 paper (105 mm by 148 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		A6,
		/// <summary>Japanese Kaku #2 envelope. Requires Windows 98, Windows NT 4.0, or later.</summary>
		JapaneseEnvelopeKakuNumber2,
		/// <summary>Japanese Kaku #3 envelope. Requires Windows 98, Windows NT 4.0, or later.</summary>
		JapaneseEnvelopeKakuNumber3,
		/// <summary>Japanese Chou #3 envelope. Requires Windows 98, Windows NT 4.0, or later.</summary>
		JapaneseEnvelopeChouNumber3,
		/// <summary>Japanese Chou #4 envelope. Requires Windows 98, Windows NT 4.0, or later.</summary>
		JapaneseEnvelopeChouNumber4,
		/// <summary>Letter rotated paper (11 in. by 8.5 in.).</summary>
		LetterRotated,
		/// <summary>A3 rotated paper (420 mm by 297 mm).</summary>
		A3Rotated,
		/// <summary>A4 rotated paper (297 mm by 210 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		A4Rotated,
		/// <summary>A5 rotated paper (210 mm by 148 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		A5Rotated,
		/// <summary>JIS B4 rotated paper (364 mm by 257 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		B4JisRotated,
		/// <summary>JIS B5 rotated paper (257 mm by 182 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		B5JisRotated,
		/// <summary>Japanese rotated postcard (148 mm by 100 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		JapanesePostcardRotated,
		/// <summary>Japanese rotated double postcard (148 mm by 200 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		JapaneseDoublePostcardRotated,
		/// <summary>A6 rotated paper (148 mm by 105 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		A6Rotated,
		/// <summary>Japanese rotated Kaku #2 envelope. Requires Windows 98, Windows NT 4.0, or later.</summary>
		JapaneseEnvelopeKakuNumber2Rotated,
		/// <summary>Japanese rotated Kaku #3 envelope. Requires Windows 98, Windows NT 4.0, or later.</summary>
		JapaneseEnvelopeKakuNumber3Rotated,
		/// <summary>Japanese rotated Chou #3 envelope. Requires Windows 98, Windows NT 4.0, or later.</summary>
		JapaneseEnvelopeChouNumber3Rotated,
		/// <summary>Japanese rotated Chou #4 envelope. Requires Windows 98, Windows NT 4.0, or later.</summary>
		JapaneseEnvelopeChouNumber4Rotated,
		/// <summary>JIS B6 paper (128 mm by 182 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		B6Jis,
		/// <summary>JIS B6 rotated paper (182 mm by 128 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		B6JisRotated,
		/// <summary>Standard paper (12 in. by 11 in.). Requires Windows 98, Windows NT 4.0, or later.</summary>
		Standard12x11,
		/// <summary>Japanese You #4 envelope. Requires Windows 98, Windows NT 4.0, or later.</summary>
		JapaneseEnvelopeYouNumber4,
		/// <summary>Japanese You #4 rotated envelope. Requires Windows 98, Windows NT 4.0, or later.</summary>
		JapaneseEnvelopeYouNumber4Rotated,
		/// <summary>16K paper (146 mm by 215 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		Prc16K,
		/// <summary>32K paper (97 mm by 151 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		Prc32K,
		/// <summary>32K big paper (97 mm by 151 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		Prc32KBig,
		/// <summary>#1 envelope (102 mm by 165 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		PrcEnvelopeNumber1,
		/// <summary>#2 envelope (102 mm by 176 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		PrcEnvelopeNumber2,
		/// <summary>#3 envelope (125 mm by 176 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		PrcEnvelopeNumber3,
		/// <summary>#4 envelope (110 mm by 208 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		PrcEnvelopeNumber4,
		/// <summary>#5 envelope (110 mm by 220 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		PrcEnvelopeNumber5,
		/// <summary>#6 envelope (120 mm by 230 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		PrcEnvelopeNumber6,
		/// <summary>#7 envelope (160 mm by 230 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		PrcEnvelopeNumber7,
		/// <summary>#8 envelope (120 mm by 309 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		PrcEnvelopeNumber8,
		/// <summary>#9 envelope (229 mm by 324 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		PrcEnvelopeNumber9,
		/// <summary>#10 envelope (324 mm by 458 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		PrcEnvelopeNumber10,
		/// <summary>16K rotated paper (146 mm by 215 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		Prc16KRotated,
		/// <summary>32K rotated paper (97 mm by 151 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		Prc32KRotated,
		/// <summary>32K big rotated paper (97 mm by 151 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		Prc32KBigRotated,
		/// <summary>#1 rotated envelope (165 mm by 102 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		PrcEnvelopeNumber1Rotated,
		/// <summary>#2 rotated envelope (176 mm by 102 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		PrcEnvelopeNumber2Rotated,
		/// <summary>#3 rotated envelope (176 mm by 125 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		PrcEnvelopeNumber3Rotated,
		/// <summary>#4 rotated envelope (208 mm by 110 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		PrcEnvelopeNumber4Rotated,
		/// <summary>Envelope #5 rotated envelope (220 mm by 110 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		PrcEnvelopeNumber5Rotated,
		/// <summary>#6 rotated envelope (230 mm by 120 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		PrcEnvelopeNumber6Rotated,
		/// <summary>#7 rotated envelope (230 mm by 160 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		PrcEnvelopeNumber7Rotated,
		/// <summary>#8 rotated envelope (309 mm by 120 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		PrcEnvelopeNumber8Rotated,
		/// <summary>#9 rotated envelope (324 mm by 229 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		PrcEnvelopeNumber9Rotated,
		/// <summary>#10 rotated envelope (458 mm by 324 mm). Requires Windows 98, Windows NT 4.0, or later.</summary>
		PrcEnvelopeNumber10Rotated
	}
}
