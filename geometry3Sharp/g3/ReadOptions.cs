using System;

namespace g3
{
	public class ReadOptions
	{
		public ReadOptions()
		{
			this.ReadMaterials = false;
		}

		public bool ReadMaterials;

		public CommandArgumentSet CustomFlags = new CommandArgumentSet();

		public static readonly ReadOptions Defaults = new ReadOptions
		{
			ReadMaterials = false
		};
	}
}
