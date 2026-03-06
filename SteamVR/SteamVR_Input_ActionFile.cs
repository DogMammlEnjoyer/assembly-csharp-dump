using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Valve.Newtonsoft.Json;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Input_ActionFile
	{
		public void InitializeHelperLists()
		{
			using (List<SteamVR_Input_ActionFile_ActionSet>.Enumerator enumerator = this.action_sets.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SteamVR_Input_ActionFile_ActionSet actionset = enumerator.Current;
					actionset.actionsInList = new List<SteamVR_Input_ActionFile_Action>(from action in this.actions
					where action.path.StartsWith(actionset.name) && SteamVR_Input_ActionFile_ActionTypes.listIn.Contains(action.type)
					select action);
					actionset.actionsOutList = new List<SteamVR_Input_ActionFile_Action>(from action in this.actions
					where action.path.StartsWith(actionset.name) && SteamVR_Input_ActionFile_ActionTypes.listOut.Contains(action.type)
					select action);
					actionset.actionsList = new List<SteamVR_Input_ActionFile_Action>(from action in this.actions
					where action.path.StartsWith(actionset.name)
					select action);
				}
			}
			foreach (Dictionary<string, string> dictionary in this.localization)
			{
				this.localizationHelperList.Add(new SteamVR_Input_ActionFile_LocalizationItem(dictionary));
			}
		}

		public void SaveHelperLists()
		{
			foreach (SteamVR_Input_ActionFile_ActionSet steamVR_Input_ActionFile_ActionSet in this.action_sets)
			{
				steamVR_Input_ActionFile_ActionSet.actionsList.Clear();
				steamVR_Input_ActionFile_ActionSet.actionsList.AddRange(steamVR_Input_ActionFile_ActionSet.actionsInList);
				steamVR_Input_ActionFile_ActionSet.actionsList.AddRange(steamVR_Input_ActionFile_ActionSet.actionsOutList);
			}
			this.actions.Clear();
			foreach (SteamVR_Input_ActionFile_ActionSet steamVR_Input_ActionFile_ActionSet2 in this.action_sets)
			{
				this.actions.AddRange(steamVR_Input_ActionFile_ActionSet2.actionsInList);
				this.actions.AddRange(steamVR_Input_ActionFile_ActionSet2.actionsOutList);
			}
			this.localization.Clear();
			foreach (SteamVR_Input_ActionFile_LocalizationItem steamVR_Input_ActionFile_LocalizationItem in this.localizationHelperList)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("language_tag", steamVR_Input_ActionFile_LocalizationItem.language);
				foreach (KeyValuePair<string, string> keyValuePair in steamVR_Input_ActionFile_LocalizationItem.items)
				{
					dictionary.Add(keyValuePair.Key, keyValuePair.Value);
				}
				this.localization.Add(dictionary);
			}
		}

		public static string GetShortName(string name)
		{
			string text = name;
			int num = text.LastIndexOf('/');
			if (num != -1)
			{
				if (num == text.Length - 1)
				{
					text = text.Remove(num);
					num = text.LastIndexOf('/');
					if (num == -1)
					{
						return SteamVR_Input_ActionFile.GetCodeFriendlyName(text);
					}
				}
				return SteamVR_Input_ActionFile.GetCodeFriendlyName(text.Substring(num + 1));
			}
			return SteamVR_Input_ActionFile.GetCodeFriendlyName(text);
		}

		public static string GetCodeFriendlyName(string name)
		{
			name = name.Replace('/', '_').Replace(' ', '_');
			if (!char.IsLetter(name[0]))
			{
				name = "_" + name;
			}
			for (int i = 0; i < name.Length; i++)
			{
				if (!char.IsLetterOrDigit(name[i]) && name[i] != '_')
				{
					name = name.Remove(i, 1);
					name = name.Insert(i, "_");
				}
			}
			return name;
		}

		public string[] GetFilesToCopy(bool throwErrors = false)
		{
			List<string> list = new List<string>();
			string fullName = new FileInfo(this.filePath).Directory.FullName;
			list.Add(this.filePath);
			foreach (SteamVR_Input_ActionFile_DefaultBinding steamVR_Input_ActionFile_DefaultBinding in this.default_bindings)
			{
				string text = Path.Combine(fullName, steamVR_Input_ActionFile_DefaultBinding.binding_url);
				if (File.Exists(text))
				{
					list.Add(text);
				}
				else if (throwErrors)
				{
					Debug.LogError("<b>[SteamVR]</b> Could not bind binding file specified by the actions.json manifest: " + text);
				}
			}
			return list.ToArray();
		}

		public void CopyFilesToPath(string toPath, bool overwrite)
		{
			foreach (string text in SteamVR_Input.actionFile.GetFilesToCopy(false))
			{
				FileInfo fileInfo = new FileInfo(text);
				string text2 = Path.Combine(toPath, fileInfo.Name);
				bool flag = false;
				if (File.Exists(text2))
				{
					flag = true;
				}
				if (flag)
				{
					if (overwrite)
					{
						new FileInfo(text2)
						{
							IsReadOnly = false
						}.Delete();
						File.Copy(text, text2);
						SteamVR_Input_ActionFile.RemoveAppKey(text2);
						Debug.Log("<b>[SteamVR]</b> Copied (overwrote) SteamVR Input file at path: " + text2);
					}
					else
					{
						Debug.Log("<b>[SteamVR]</b> Skipped writing existing file at path: " + text2);
					}
				}
				else
				{
					File.Copy(text, text2);
					SteamVR_Input_ActionFile.RemoveAppKey(text2);
					Debug.Log("<b>[SteamVR]</b> Copied SteamVR Input file to folder: " + text2);
				}
			}
		}

		private static void RemoveAppKey(string newFilePath)
		{
			if (File.Exists(newFilePath))
			{
				string text = File.ReadAllText(newFilePath);
				string value = "\"app_key\"";
				int num = text.IndexOf(value);
				if (num == -1)
				{
					return;
				}
				int num2 = text.IndexOf("\",", num);
				if (num2 == -1)
				{
					return;
				}
				num2 += "\",".Length;
				int count = num2 - num;
				string contents = text.Remove(num, count);
				new FileInfo(newFilePath).IsReadOnly = false;
				File.WriteAllText(newFilePath, contents);
			}
		}

		public static SteamVR_Input_ActionFile Open(string path)
		{
			if (File.Exists(path))
			{
				SteamVR_Input_ActionFile steamVR_Input_ActionFile = JsonConvert.DeserializeObject<SteamVR_Input_ActionFile>(File.ReadAllText(path));
				steamVR_Input_ActionFile.filePath = path;
				steamVR_Input_ActionFile.InitializeHelperLists();
				return steamVR_Input_ActionFile;
			}
			return null;
		}

		public void Save(string path)
		{
			FileInfo fileInfo = new FileInfo(path);
			if (fileInfo.Exists)
			{
				fileInfo.IsReadOnly = false;
			}
			string contents = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			});
			File.WriteAllText(path, contents);
		}

		public List<SteamVR_Input_ActionFile_Action> actions = new List<SteamVR_Input_ActionFile_Action>();

		public List<SteamVR_Input_ActionFile_ActionSet> action_sets = new List<SteamVR_Input_ActionFile_ActionSet>();

		public List<SteamVR_Input_ActionFile_DefaultBinding> default_bindings = new List<SteamVR_Input_ActionFile_DefaultBinding>();

		public List<Dictionary<string, string>> localization = new List<Dictionary<string, string>>();

		[JsonIgnore]
		public string filePath;

		[JsonIgnore]
		public List<SteamVR_Input_ActionFile_LocalizationItem> localizationHelperList = new List<SteamVR_Input_ActionFile_LocalizationItem>();

		private const string findString_appKeyStart = "\"app_key\"";

		private const string findString_appKeyEnd = "\",";
	}
}
