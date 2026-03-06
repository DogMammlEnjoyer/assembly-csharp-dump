using System;

namespace g3
{
	public abstract class GenericMaterial
	{
		public abstract Vector3f DiffuseColor { get; set; }

		public abstract float Alpha { get; set; }

		public GenericMaterial.KnownMaterialTypes Type { get; set; }

		public static readonly float Invalidf = float.MinValue;

		public static readonly Vector3f Invalid = new Vector3f(-1f, -1f, -1f);

		public string name;

		public int id;

		public enum KnownMaterialTypes
		{
			OBJ_MTL_Format
		}
	}
}
