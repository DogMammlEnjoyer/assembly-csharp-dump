using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Drawing.Text
{
	internal struct SDFLookupData
	{
		public SDFLookupData(SDFFont font)
		{
			int num = 0;
			SDFCharacter value = font.characters[0];
			for (int i = 0; i < font.characters.Length; i++)
			{
				if (font.characters[i].codePoint == '?')
				{
					value = font.characters[i];
				}
				if (font.characters[i].codePoint >= '\u0080')
				{
					num++;
				}
			}
			this.characters = new NativeArray<SDFCharacter>(128 + num, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			for (int j = 0; j < this.characters.Length; j++)
			{
				this.characters[j] = value;
			}
			this.lookup = new Dictionary<char, int>();
			this.material = font.material;
			num = 0;
			for (int k = 0; k < font.characters.Length; k++)
			{
				SDFCharacter sdfcharacter = font.characters[k];
				int num2 = (int)sdfcharacter.codePoint;
				if (sdfcharacter.codePoint >= '\u0080')
				{
					num2 = 128 + num;
					num++;
				}
				this.characters[num2] = sdfcharacter;
				this.lookup[sdfcharacter.codePoint] = num2;
			}
		}

		public int GetIndex(char c)
		{
			int result;
			if (this.lookup.TryGetValue(c, out result))
			{
				return result;
			}
			if (c == '\n')
			{
				return 65535;
			}
			return this.lookup['?'];
		}

		public void Dispose()
		{
			if (this.characters.IsCreated)
			{
				this.characters.Dispose();
			}
		}

		public NativeArray<SDFCharacter> characters;

		private Dictionary<char, int> lookup;

		public Material material;

		public const ushort Newline = 65535;
	}
}
