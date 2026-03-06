using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace System.IO
{
	internal class DefaultWatcher : IFileWatcher
	{
		private DefaultWatcher()
		{
		}

		public static bool GetInstance(out IFileWatcher watcher)
		{
			if (DefaultWatcher.instance != null)
			{
				watcher = DefaultWatcher.instance;
				return true;
			}
			DefaultWatcher.instance = new DefaultWatcher();
			watcher = DefaultWatcher.instance;
			return true;
		}

		public void StartDispatching(object handle)
		{
			FileSystemWatcher fileSystemWatcher = handle as FileSystemWatcher;
			lock (this)
			{
				if (DefaultWatcher.watches == null)
				{
					DefaultWatcher.watches = new Hashtable();
				}
				if (DefaultWatcher.thread == null)
				{
					DefaultWatcher.thread = new Thread(new ThreadStart(this.Monitor));
					DefaultWatcher.thread.IsBackground = true;
					DefaultWatcher.thread.Start();
				}
			}
			Hashtable obj = DefaultWatcher.watches;
			lock (obj)
			{
				DefaultWatcherData defaultWatcherData = (DefaultWatcherData)DefaultWatcher.watches[fileSystemWatcher];
				if (defaultWatcherData == null)
				{
					defaultWatcherData = new DefaultWatcherData();
					defaultWatcherData.Files = new Dictionary<string, FileData>();
					DefaultWatcher.watches[fileSystemWatcher] = defaultWatcherData;
				}
				defaultWatcherData.FSW = fileSystemWatcher;
				defaultWatcherData.Directory = fileSystemWatcher.FullPath;
				defaultWatcherData.NoWildcards = !fileSystemWatcher.Pattern.HasWildcard;
				if (defaultWatcherData.NoWildcards)
				{
					defaultWatcherData.FileMask = Path.Combine(defaultWatcherData.Directory, fileSystemWatcher.MangledFilter);
				}
				else
				{
					defaultWatcherData.FileMask = fileSystemWatcher.MangledFilter;
				}
				defaultWatcherData.IncludeSubdirs = fileSystemWatcher.IncludeSubdirectories;
				defaultWatcherData.Enabled = true;
				defaultWatcherData.DisabledTime = DateTime.MaxValue;
				this.UpdateDataAndDispatch(defaultWatcherData, false);
			}
		}

		public void StopDispatching(object handle)
		{
			FileSystemWatcher key = handle as FileSystemWatcher;
			lock (this)
			{
				if (DefaultWatcher.watches == null)
				{
					return;
				}
			}
			Hashtable obj = DefaultWatcher.watches;
			lock (obj)
			{
				DefaultWatcherData defaultWatcherData = (DefaultWatcherData)DefaultWatcher.watches[key];
				if (defaultWatcherData != null)
				{
					object filesLock = defaultWatcherData.FilesLock;
					lock (filesLock)
					{
						defaultWatcherData.Enabled = false;
						defaultWatcherData.DisabledTime = DateTime.UtcNow;
					}
				}
			}
		}

		public void Dispose(object handle)
		{
		}

		private void Monitor()
		{
			int num = 0;
			for (;;)
			{
				Thread.Sleep(750);
				Hashtable obj = DefaultWatcher.watches;
				Hashtable hashtable;
				lock (obj)
				{
					if (DefaultWatcher.watches.Count == 0)
					{
						if (++num == 20)
						{
							break;
						}
						continue;
					}
					else
					{
						hashtable = (Hashtable)DefaultWatcher.watches.Clone();
					}
				}
				if (hashtable.Count != 0)
				{
					num = 0;
					using (IEnumerator enumerator = hashtable.Values.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							object obj2 = enumerator.Current;
							DefaultWatcherData defaultWatcherData = (DefaultWatcherData)obj2;
							if (this.UpdateDataAndDispatch(defaultWatcherData, true))
							{
								obj = DefaultWatcher.watches;
								lock (obj)
								{
									DefaultWatcher.watches.Remove(defaultWatcherData.FSW);
								}
							}
						}
						continue;
					}
					break;
				}
			}
			lock (this)
			{
				DefaultWatcher.thread = null;
			}
		}

		private bool UpdateDataAndDispatch(DefaultWatcherData data, bool dispatch)
		{
			if (!data.Enabled)
			{
				return data.DisabledTime != DateTime.MaxValue && (DateTime.UtcNow - data.DisabledTime).TotalSeconds > 5.0;
			}
			this.DoFiles(data, data.Directory, dispatch);
			return false;
		}

		private static void DispatchEvents(FileSystemWatcher fsw, FileAction action, string filename)
		{
			RenamedEventArgs renamedEventArgs = null;
			lock (fsw)
			{
				fsw.DispatchEvents(action, filename, ref renamedEventArgs);
				if (fsw.Waiting)
				{
					fsw.Waiting = false;
					System.Threading.Monitor.PulseAll(fsw);
				}
			}
		}

		private void DoFiles(DefaultWatcherData data, string directory, bool dispatch)
		{
			bool flag = Directory.Exists(directory);
			if (flag && data.IncludeSubdirs)
			{
				foreach (string directory2 in Directory.GetDirectories(directory))
				{
					this.DoFiles(data, directory2, dispatch);
				}
			}
			string[] files;
			if (!flag)
			{
				files = DefaultWatcher.NoStringsArray;
			}
			else if (!data.NoWildcards)
			{
				files = Directory.GetFileSystemEntries(directory, data.FileMask);
			}
			else if (File.Exists(data.FileMask) || Directory.Exists(data.FileMask))
			{
				files = new string[]
				{
					data.FileMask
				};
			}
			else
			{
				files = DefaultWatcher.NoStringsArray;
			}
			object filesLock = data.FilesLock;
			lock (filesLock)
			{
				if (data.Enabled)
				{
					this.IterateAndModifyFilesData(data, directory, dispatch, files);
				}
			}
		}

		private void IterateAndModifyFilesData(DefaultWatcherData data, string directory, bool dispatch, string[] files)
		{
			foreach (KeyValuePair<string, FileData> keyValuePair in data.Files)
			{
				FileData value = keyValuePair.Value;
				if (value.Directory == directory)
				{
					value.NotExists = true;
				}
			}
			foreach (string text in files)
			{
				FileData fileData;
				if (!data.Files.TryGetValue(text, out fileData))
				{
					try
					{
						data.Files.Add(text, DefaultWatcher.CreateFileData(directory, text));
					}
					catch
					{
						data.Files.Remove(text);
						goto IL_CA;
					}
					if (dispatch)
					{
						DefaultWatcher.DispatchEvents(data.FSW, FileAction.Added, Path.GetRelativePath(data.Directory, text));
					}
				}
				else if (fileData.Directory == directory)
				{
					fileData.NotExists = false;
				}
				IL_CA:;
			}
			if (!dispatch)
			{
				return;
			}
			List<string> list = null;
			foreach (KeyValuePair<string, FileData> keyValuePair2 in data.Files)
			{
				string key = keyValuePair2.Key;
				if (keyValuePair2.Value.NotExists)
				{
					if (list == null)
					{
						list = new List<string>();
					}
					list.Add(key);
					DefaultWatcher.DispatchEvents(data.FSW, FileAction.Removed, Path.GetRelativePath(data.Directory, key));
				}
			}
			if (list != null)
			{
				foreach (string key2 in list)
				{
					data.Files.Remove(key2);
				}
				list = null;
			}
			foreach (KeyValuePair<string, FileData> keyValuePair3 in data.Files)
			{
				string key3 = keyValuePair3.Key;
				FileData value2 = keyValuePair3.Value;
				DateTime creationTime;
				DateTime lastWriteTime;
				try
				{
					creationTime = File.GetCreationTime(key3);
					lastWriteTime = File.GetLastWriteTime(key3);
				}
				catch
				{
					if (list == null)
					{
						list = new List<string>();
					}
					list.Add(key3);
					DefaultWatcher.DispatchEvents(data.FSW, FileAction.Removed, Path.GetRelativePath(data.Directory, key3));
					continue;
				}
				if (creationTime != value2.CreationTime || lastWriteTime != value2.LastWriteTime)
				{
					value2.CreationTime = creationTime;
					value2.LastWriteTime = lastWriteTime;
					DefaultWatcher.DispatchEvents(data.FSW, FileAction.Modified, Path.GetRelativePath(data.Directory, key3));
				}
			}
			if (list != null)
			{
				foreach (string key4 in list)
				{
					data.Files.Remove(key4);
				}
			}
		}

		private static FileData CreateFileData(string directory, string filename)
		{
			FileData fileData = new FileData();
			string path = Path.Combine(directory, filename);
			fileData.Directory = directory;
			fileData.Attributes = File.GetAttributes(path);
			fileData.CreationTime = File.GetCreationTime(path);
			fileData.LastWriteTime = File.GetLastWriteTime(path);
			return fileData;
		}

		private static DefaultWatcher instance;

		private static Thread thread;

		private static Hashtable watches;

		private static string[] NoStringsArray = new string[0];
	}
}
