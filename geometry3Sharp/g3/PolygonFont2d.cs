using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace g3
{
	public class PolygonFont2d
	{
		public PolygonFont2d()
		{
			this.Characters = new Dictionary<string, PolygonFont2d.CharacterInfo>();
			this.MaxBounds = AxisAlignedBox2d.Empty;
		}

		public void AddCharacter(string s, GeneralPolygon2d[] polygons)
		{
			PolygonFont2d.CharacterInfo characterInfo = new PolygonFont2d.CharacterInfo();
			characterInfo.Polygons = polygons;
			characterInfo.Bounds = polygons[0].Bounds;
			for (int i = 1; i < polygons.Length; i++)
			{
				characterInfo.Bounds.Contain(polygons[i].Bounds);
			}
			this.Characters.Add(s, characterInfo);
			this.MaxBounds.Contain(characterInfo.Bounds);
		}

		public List<GeneralPolygon2d> GetCharacter(char c)
		{
			string key = c.ToString();
			if (!this.Characters.ContainsKey(key))
			{
				throw new Exception("PolygonFont2d.GetCharacterBounds: character " + c.ToString() + " not available!");
			}
			return new List<GeneralPolygon2d>(this.Characters[key].Polygons);
		}

		public List<GeneralPolygon2d> GetCharacter(string s)
		{
			if (!this.Characters.ContainsKey(s))
			{
				throw new Exception("PolygonFont2d.GetCharacterBounds: character " + s + " not available!");
			}
			return new List<GeneralPolygon2d>(this.Characters[s].Polygons);
		}

		public AxisAlignedBox2d GetCharacterBounds(char c)
		{
			string key = c.ToString();
			if (!this.Characters.ContainsKey(key))
			{
				throw new Exception("PolygonFont2d.GetCharacterBounds: character " + c.ToString() + " not available!");
			}
			return this.Characters[key].Bounds;
		}

		public bool HasCharacter(char c)
		{
			string key = c.ToString();
			return this.Characters.ContainsKey(key);
		}

		public static void Store(PolygonFont2d font, BinaryWriter writer)
		{
			writer.Write(3);
			int count = font.Characters.Count;
			writer.Write(count);
			foreach (KeyValuePair<string, PolygonFont2d.CharacterInfo> keyValuePair in font.Characters)
			{
				byte[] bytes = Encoding.Unicode.GetBytes(keyValuePair.Key);
				writer.Write(bytes.Length);
				writer.Write(bytes);
				PolygonFont2d.CharacterInfo value = keyValuePair.Value;
				writer.Write(value.Polygons.Length);
				for (int i = 0; i < value.Polygons.Length; i++)
				{
					gSerialization.Store(value.Polygons[i], writer);
				}
			}
		}

		public static PolygonFont2d ReadFont(string filename)
		{
			PolygonFont2d result;
			using (FileStream fileStream = File.Open(filename, FileMode.Open))
			{
				BinaryReader reader = new BinaryReader(fileStream);
				PolygonFont2d polygonFont2d = new PolygonFont2d();
				PolygonFont2d.Restore(polygonFont2d, reader);
				result = polygonFont2d;
			}
			return result;
		}

		public static PolygonFont2d ReadFont(Stream s)
		{
			BinaryReader reader = new BinaryReader(s);
			PolygonFont2d polygonFont2d = new PolygonFont2d();
			PolygonFont2d.Restore(polygonFont2d, reader);
			return polygonFont2d;
		}

		public static void Restore(PolygonFont2d font, BinaryReader reader)
		{
			if (reader.ReadInt32() != 3)
			{
				throw new Exception("PolygonFont2d.Restore: invalid version!");
			}
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				int count = reader.ReadInt32();
				byte[] bytes = reader.ReadBytes(count);
				string @string = Encoding.Unicode.GetString(bytes);
				int num2 = reader.ReadInt32();
				GeneralPolygon2d[] array = new GeneralPolygon2d[num2];
				for (int j = 0; j < num2; j++)
				{
					array[j] = new GeneralPolygon2d();
					gSerialization.Restore(array[j], reader);
				}
				font.AddCharacter(@string, array);
			}
		}

		public Dictionary<string, PolygonFont2d.CharacterInfo> Characters;

		public AxisAlignedBox2d MaxBounds;

		private const int SerializerVersion = 3;

		public class CharacterInfo
		{
			public GeneralPolygon2d[] Polygons;

			public AxisAlignedBox2d Bounds;
		}
	}
}
