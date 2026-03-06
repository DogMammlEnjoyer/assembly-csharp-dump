using System;

namespace Modio.FileIO
{
	public class DefaultRootPathProvider : IModioRootPathProvider
	{
		public virtual string Path
		{
			get
			{
				return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) ?? "";
			}
		}

		public string UserPath
		{
			get
			{
				return this.Path;
			}
		}
	}
}
