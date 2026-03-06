using System;

namespace System.IO
{
	/// <summary>Provides data for the <see cref="E:System.IO.FileSystemWatcher.Renamed" /> event.</summary>
	public class RenamedEventArgs : FileSystemEventArgs
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.IO.RenamedEventArgs" /> class.</summary>
		/// <param name="changeType">One of the <see cref="T:System.IO.WatcherChangeTypes" /> values.</param>
		/// <param name="directory">The name of the affected file or directory.</param>
		/// <param name="name">The name of the affected file or directory.</param>
		/// <param name="oldName">The old name of the affected file or directory.</param>
		public RenamedEventArgs(WatcherChangeTypes changeType, string directory, string name, string oldName) : base(changeType, directory, name)
		{
			this._oldName = oldName;
			this._oldFullPath = FileSystemEventArgs.Combine(directory, oldName);
		}

		/// <summary>Gets the previous fully qualified path of the affected file or directory.</summary>
		/// <returns>The previous fully qualified path of the affected file or directory.</returns>
		public string OldFullPath
		{
			get
			{
				return this._oldFullPath;
			}
		}

		/// <summary>Gets the old name of the affected file or directory.</summary>
		/// <returns>The previous name of the affected file or directory.</returns>
		public string OldName
		{
			get
			{
				return this._oldName;
			}
		}

		private readonly string _oldName;

		private readonly string _oldFullPath;
	}
}
