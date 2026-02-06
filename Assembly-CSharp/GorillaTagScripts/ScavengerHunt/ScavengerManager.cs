using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag.Scripts.Utilities;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.ScavengerHunt
{
	[NullableContext(1)]
	[Nullable(0)]
	public class ScavengerManager : MonoBehaviour
	{
		[Nullable(2)]
		public static ScavengerManager Instance { [NullableContext(2)] get; [NullableContext(2)] private set; }

		private void Awake()
		{
			if (ScavengerManager.Instance == null)
			{
				ScavengerManager.Instance = this;
				return;
			}
			throw new Exception("Too ScavengerManagers exist at once, this should never happen.");
		}

		private void Start()
		{
			base.StartCoroutine(this.ImportMothershipUserData());
		}

		private IEnumerator ImportMothershipUserData()
		{
			int i = 0;
			while (i < 10)
			{
				if (!MothershipClientContext.IsClientLoggedIn())
				{
					yield return new WaitForSeconds(1f);
					int num = i + 1;
					i = num;
				}
				else
				{
					if (PlayFabAuthenticator.instance != null && PlayFabAuthenticator.instance.loginFailed)
					{
						Debug.LogError("ScavengerManager critical error, could not log into Mothership.");
						yield break;
					}
					break;
				}
			}
			MothershipClientApiUnity.GetUserDataValue("ScavengerHunt", new Action<MothershipUserData>(this.OnGetUserDataSuccess), new Action<MothershipError, int>(this.OnGetUserDataFailure), "");
			yield break;
		}

		private void OnGetUserDataSuccess(MothershipUserData data)
		{
			Debug.Log("Successfully read scavenger hunt data from Mothership.");
			byte[] bytes = Convert.FromBase64String(data.value);
			string @string = Encoding.UTF8.GetString(bytes);
			this.FromJson(@string);
		}

		private void OnGetUserDataFailure(MothershipError error, int responseCode)
		{
			Debug.LogError(string.Format("Failed to read scavenger hunt user data (error {0} / {1}): {2}", error.Name, responseCode, error.Message));
		}

		private void OnDestroy()
		{
			ScavengerManager.Instance = null;
		}

		[return: Nullable(2)]
		public ScavengerManager.Hunt GetHunt(string huntName)
		{
			foreach (ScavengerManager.Hunt hunt in this.Hunts)
			{
				if (hunt.Name == huntName)
				{
					return hunt;
				}
			}
			return null;
		}

		public void RegisterTarget(ScavengerTarget target)
		{
			ScavengerManager.Hunt hunt = this.GetHunt(target.HuntName);
			if (hunt == null)
			{
				throw new Exception("No hunt found with name " + target.HuntName + ".");
			}
			if (!hunt.Targets.Contains(target))
			{
				hunt.Targets.Add(target);
			}
		}

		public bool IsCollected(ScavengerTarget target)
		{
			ScavengerManager.Hunt hunt = this.GetHunt(target.HuntName);
			return hunt != null && hunt.IsCollected(target);
		}

		public void Collect(ScavengerTarget target)
		{
			ScavengerManager.Hunt hunt = this.GetHunt(target.HuntName);
			if (hunt == null)
			{
				throw new Exception(string.Concat(new string[]
				{
					"Cannot collect scavenger hunt ",
					target.TargetName,
					", hunt ",
					target.HuntName,
					" does not exist."
				}));
			}
			if (!hunt.Collect(target, false))
			{
				Debug.Log("Did not collect scavenger hunt " + target.TargetName + ". This is normally because the user already collected it.");
				return;
			}
			Debug.Log("Collected " + target.HuntName + "." + target.TargetName);
			string value = this.ToJson().Write();
			MothershipClientApiUnity.SetUserDataValue("ScavengerHunt", value, new Action<SetUserDataResponse>(this.OnSetUserDataSuccess), new Action<MothershipError, int>(this.OnSetUserDataFailure), "");
		}

		private void OnSetUserDataSuccess(SetUserDataResponse response)
		{
			Debug.Log("Successfully wrote scavenger hunt data for user " + response.user_id + " on Mothership key " + response.key_name);
		}

		private void OnSetUserDataFailure(MothershipError error, int statusCode)
		{
			Debug.LogError(string.Format("Failed to write scavenger hunt data to Mothership (error {0} / {1}): {2}", error.Name, statusCode, error.Message));
		}

		public ScavengerManager.ScavengerJson ToJson()
		{
			return ScavengerManager.ScavengerJson.FromManager(this);
		}

		public void FromJson(string json)
		{
			this.FromJson(ScavengerManager.ScavengerJson.FromJson(json));
		}

		public void FromJson(ScavengerManager.ScavengerJson json)
		{
			ScavengerManager.Hunt[] hunts = this.Hunts;
			for (int i = 0; i < hunts.Length; i++)
			{
				hunts[i].ClearCollectedTargets();
			}
			foreach (KeyValuePair<string, string[]> keyValuePair in json.CollectedTargets)
			{
				ScavengerManager.Hunt hunt2 = this.GetHunt(keyValuePair.Key);
				if (hunt2 == null)
				{
					throw new Exception("Cannot import scavenger data, no hunt by name " + keyValuePair.Key + ".");
				}
				foreach (string text in keyValuePair.Value)
				{
					ScavengerTarget target = hunt2.GetTarget(text);
					if (target == null)
					{
						throw new Exception("Cannot import scavenger data, no hunt/target by name " + keyValuePair.Key + "." + text);
					}
					hunt2.Collect(target, true);
				}
			}
			int num = this.Hunts.Sum((ScavengerManager.Hunt hunt) => hunt.Targets.Count);
			Debug.Log(string.Format("Imported {0} targets from {1} scavenger hunts.", num, this.Hunts.Length));
		}

		public const string MothershipKey = "ScavengerHunt";

		public ScavengerManager.Hunt[] Hunts = new ScavengerManager.Hunt[0];

		[Nullable(0)]
		[Serializable]
		public class Hunt
		{
			public bool IsCompleted
			{
				get
				{
					return this.Targets.Count == this.CollectedTargetNames.Count;
				}
			}

			public List<ScavengerTarget> Targets
			{
				get
				{
					List<ScavengerTarget> result;
					if ((result = this._targets) == null)
					{
						result = (this._targets = new List<ScavengerTarget>());
					}
					return result;
				}
			}

			public IReadOnlyCollection<string> CollectedTargetNames
			{
				get
				{
					HashSet<string> result;
					if ((result = this._collectedTargetNamesNullable) == null)
					{
						result = (this._collectedTargetNamesNullable = new HashSet<string>());
					}
					return result;
				}
			}

			private HashSet<string> _collectedTargetNames
			{
				get
				{
					HashSet<string> result;
					if ((result = this._collectedTargetNamesNullable) == null)
					{
						result = (this._collectedTargetNamesNullable = new HashSet<string>());
					}
					return result;
				}
			}

			public Hunt(string name)
			{
				this.Name = name;
			}

			public bool Collect(ScavengerTarget target, bool initialLoad = false)
			{
				if (!this.Targets.Contains(target))
				{
					return false;
				}
				if (this._collectedTargetNames.Add(target.TargetName))
				{
					if (!initialLoad || this.SendTargetCollectedEventsOnLoad)
					{
						this.SendTargetCollectedEvents(target);
					}
					if (this.IsCompleted && (!initialLoad || this.SendHuntCompletedEventsOnLoad))
					{
						this.SendHuntCompletedEvents();
					}
					return true;
				}
				return false;
			}

			private void SendTargetCollectedEvents(ScavengerTarget target)
			{
				this.TargetCollected.InvokeAll();
				this.TargetCollectedArg.InvokeAll(target);
				target.TargetCollected.InvokeAll();
				target.TargetCollectedArg.InvokeAll(target);
			}

			private void SendHuntCompletedEvents()
			{
				this.HuntCompleted.InvokeAll();
				this.HuntCompletedArg.InvokeAll(this);
			}

			public bool IsCollected(ScavengerTarget target)
			{
				return this._collectedTargetNames.Contains(target.TargetName);
			}

			public void ClearCollectedTargets()
			{
				this._collectedTargetNames.Clear();
			}

			[return: Nullable(2)]
			public ScavengerTarget GetTarget(string name)
			{
				foreach (ScavengerTarget scavengerTarget in this.Targets)
				{
					if (scavengerTarget.TargetName == name)
					{
						return scavengerTarget;
					}
				}
				return null;
			}

			public string Name;

			public bool SendTargetCollectedEventsOnLoad;

			public bool SendHuntCompletedEventsOnLoad;

			public UnityEvent[] TargetCollected = new UnityEvent[0];

			public UnityEvent<ScavengerTarget>[] TargetCollectedArg = new UnityEvent<ScavengerTarget>[0];

			public UnityEvent[] HuntCompleted = new UnityEvent[0];

			public UnityEvent<ScavengerManager.Hunt>[] HuntCompletedArg = new UnityEvent<ScavengerManager.Hunt>[0];

			[Nullable(new byte[]
			{
				2,
				1
			})]
			private List<ScavengerTarget> _targets;

			[Nullable(new byte[]
			{
				2,
				1
			})]
			private HashSet<string> _collectedTargetNamesNullable;
		}

		[Nullable(0)]
		public class ScavengerJson
		{
			public static ScavengerManager.ScavengerJson FromManager(ScavengerManager manager)
			{
				ScavengerManager.ScavengerJson scavengerJson = new ScavengerManager.ScavengerJson();
				foreach (ScavengerManager.Hunt hunt in manager.Hunts)
				{
					string[] value = hunt.CollectedTargetNames.ToArray<string>();
					scavengerJson.CollectedTargets[hunt.Name] = value;
				}
				return scavengerJson;
			}

			public static ScavengerManager.ScavengerJson FromJson(string json)
			{
				ScavengerManager.ScavengerJson scavengerJson = new ScavengerManager.ScavengerJson();
				ScavengerManager.ScavengerJson result;
				using (TextReader textReader = new StringReader(json))
				{
					using (JsonReader jsonReader = new JsonTextReader(textReader))
					{
						Debug.Log("Scavenger hunt parsing raw json " + json);
						while (jsonReader.Read())
						{
							if (jsonReader.TokenType == JsonToken.PropertyName && (string)jsonReader.Value == "CollectedTargets")
							{
								ScavengerManager.ScavengerJson.ReadCollectedTargets(scavengerJson, jsonReader);
							}
						}
						result = scavengerJson;
					}
				}
				return result;
			}

			private static void ReadCollectedTargets(ScavengerManager.ScavengerJson json, JsonReader reader)
			{
				int num = 0;
				bool flag = false;
				string text = null;
				List<string> list = new List<string>();
				while (reader.Read())
				{
					JsonToken tokenType = reader.TokenType;
					if (tokenType <= JsonToken.String)
					{
						switch (tokenType)
						{
						case JsonToken.StartObject:
							num++;
							break;
						case JsonToken.StartArray:
							if (flag)
							{
								throw new Exception("Json read error");
							}
							flag = true;
							break;
						case JsonToken.StartConstructor:
							break;
						case JsonToken.PropertyName:
						{
							if (text != null)
							{
								throw new Exception("Json read error");
							}
							string text2 = reader.Value as string;
							if (text2 == null)
							{
								throw new Exception("Json read error");
							}
							text = text2;
							break;
						}
						default:
							if (tokenType == JsonToken.String)
							{
								if (!flag)
								{
									throw new Exception("Json read error");
								}
								string text3 = reader.Value as string;
								if (text3 == null)
								{
									throw new Exception("Json read error");
								}
								list.Add(text3);
							}
							break;
						}
					}
					else if (tokenType != JsonToken.EndObject)
					{
						if (tokenType == JsonToken.EndArray)
						{
							if (!flag)
							{
								throw new Exception("Json read error");
							}
							if (string.IsNullOrEmpty(text))
							{
								throw new Exception("Json read error");
							}
							json.CollectedTargets[text] = list.ToArray();
							text = null;
							list.Clear();
							flag = false;
						}
					}
					else
					{
						num--;
					}
					if (num <= 0)
					{
						return;
					}
				}
				throw new Exception("Json read error");
			}

			public string Write()
			{
				JsonSerializer jsonSerializer = new JsonSerializer();
				string result;
				using (TextWriter textWriter = new StringWriterWithEncoding(Encoding.UTF8))
				{
					using (JsonWriter jsonWriter = new JsonTextWriter(textWriter))
					{
						jsonSerializer.Serialize(jsonWriter, this);
						result = textWriter.ToString();
					}
				}
				return result;
			}

			public readonly Dictionary<string, string[]> CollectedTargets = new Dictionary<string, string[]>();
		}
	}
}
