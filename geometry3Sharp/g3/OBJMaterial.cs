using System;

namespace g3
{
	public class OBJMaterial : GenericMaterial
	{
		public OBJMaterial()
		{
			base.Type = GenericMaterial.KnownMaterialTypes.OBJ_MTL_Format;
			this.id = -1;
			this.name = "///INVALID_NAME";
			this.Ka = (this.Kd = (this.Ks = (this.Ke = (this.Tf = GenericMaterial.Invalid))));
			this.illum = -1;
			this.d = (this.Ns = (this.sharpness = (this.Ni = GenericMaterial.Invalidf)));
		}

		public override Vector3f DiffuseColor
		{
			get
			{
				if (!(this.Kd == GenericMaterial.Invalid))
				{
					return this.Kd;
				}
				return new Vector3f(1f, 1f, 1f);
			}
			set
			{
				this.Kd = value;
			}
		}

		public override float Alpha
		{
			get
			{
				if (this.d != GenericMaterial.Invalidf)
				{
					return this.d;
				}
				return 1f;
			}
			set
			{
				this.d = value;
			}
		}

		public Vector3f Ka;

		public Vector3f Kd;

		public Vector3f Ks;

		public Vector3f Ke;

		public Vector3f Tf;

		public int illum;

		public float d;

		public float Ns;

		public float sharpness;

		public float Ni;

		public string map_Ka;

		public string map_Kd;

		public string map_Ks;

		public string map_Ke;

		public string map_d;

		public string map_Ns;

		public string bump;

		public string disp;

		public string decal;

		public string refl;
	}
}
