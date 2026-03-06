using System;
using System.IO;
using Unity;

namespace System.Media
{
	/// <summary>Represents a system sound type.</summary>
	public class SystemSound
	{
		internal SystemSound(string tag)
		{
			this.resource = typeof(SystemSound).Assembly.GetManifestResourceStream(tag + ".wav");
		}

		/// <summary>Plays the system sound type.</summary>
		public void Play()
		{
			new SoundPlayer(this.resource).Play();
		}

		internal SystemSound()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private Stream resource;
	}
}
