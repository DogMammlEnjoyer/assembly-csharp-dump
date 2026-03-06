using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Backtrace.Unity.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Backtrace.Unity.Model.JsonData
{
	public class Annotations
	{
		public static Dictionary<string, string> EnvironmentVariablesCache
		{
			get
			{
				if (!Annotations.VariablesLoaded)
				{
					Annotations._environmentVariablesCache = Annotations.SetEnvironmentVariables();
					Annotations.VariablesLoaded = true;
				}
				return Annotations._environmentVariablesCache;
			}
			set
			{
				Annotations._environmentVariablesCache = value;
			}
		}

		public Dictionary<string, string> EnvironmentVariables
		{
			get
			{
				return Annotations.EnvironmentVariablesCache;
			}
			set
			{
				Annotations.EnvironmentVariablesCache = value;
			}
		}

		public Exception Exception { get; set; }

		public Annotations(Exception exception, int gameObjectDepth)
		{
			this._gameObjectDepth = gameObjectDepth;
			this.Exception = exception;
		}

		private static Dictionary<string, string> SetEnvironmentVariables()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			IDictionary environmentVariables = Environment.GetEnvironmentVariables();
			if (environmentVariables == null)
			{
				return dictionary;
			}
			foreach (object obj in environmentVariables)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				string text = dictionaryEntry.Key as string;
				if (!string.IsNullOrEmpty(text))
				{
					string text2 = dictionaryEntry.Value as string;
					dictionary.Add(text, string.IsNullOrEmpty(text2) ? "NULL" : text2);
				}
			}
			return dictionary;
		}

		public BacktraceJObject ToJson()
		{
			BacktraceJObject backtraceJObject = new BacktraceJObject();
			backtraceJObject.Add("Environment Variables", new BacktraceJObject(this.EnvironmentVariables));
			if (this.Exception != null)
			{
				backtraceJObject.Add("Exception properties", new BacktraceJObject(new Dictionary<string, string>
				{
					{
						"message",
						this.Exception.Message
					},
					{
						"stackTrace",
						this.Exception.StackTrace
					},
					{
						"type",
						this.Exception.GetType().FullName
					},
					{
						"source",
						this.Exception.Source
					}
				}));
			}
			if (this._gameObjectDepth > -1)
			{
				Scene activeScene = SceneManager.GetActiveScene();
				List<BacktraceJObject> list = new List<BacktraceJObject>();
				List<GameObject> list2 = new List<GameObject>();
				activeScene.GetRootGameObjects(list2);
				foreach (GameObject gameObject in list2)
				{
					list.Add(this.ConvertGameObject(gameObject, 0));
				}
				backtraceJObject.Add("Game objects", list);
			}
			return backtraceJObject;
		}

		private BacktraceJObject ConvertGameObject(GameObject gameObject, int depth = 0)
		{
			if (gameObject == null)
			{
				return new BacktraceJObject();
			}
			BacktraceJObject jobject = this.GetJObject(gameObject, "");
			List<BacktraceJObject> list = new List<BacktraceJObject>();
			foreach (object obj in gameObject.transform)
			{
				Component component = obj as Component;
				if (!(component == null))
				{
					list.Add(this.ConvertGameObject(component, gameObject.name, depth + 1));
				}
			}
			jobject.Add("children", list);
			return jobject;
		}

		private BacktraceJObject ConvertGameObject(Component gameObject, string parentName, int depth)
		{
			if (this._gameObjectDepth > 0 && depth > this._gameObjectDepth)
			{
				return new BacktraceJObject();
			}
			BacktraceJObject jobject = this.GetJObject(gameObject, parentName);
			if (this._gameObjectDepth > 0 && depth + 1 >= this._gameObjectDepth)
			{
				return jobject;
			}
			List<BacktraceJObject> list = new List<BacktraceJObject>();
			foreach (object obj in gameObject.transform)
			{
				Component component = obj as Component;
				if (!(component == null))
				{
					list.Add(this.ConvertGameObject(component, gameObject.name, depth + 1));
				}
			}
			jobject.Add("children", list);
			return jobject;
		}

		private BacktraceJObject GetJObject(GameObject gameObject, string parentName = "")
		{
			return new BacktraceJObject(new Dictionary<string, string>
			{
				{
					"name",
					gameObject.name
				},
				{
					"isStatic",
					gameObject.isStatic.ToString(CultureInfo.InvariantCulture).ToLower()
				},
				{
					"layer",
					gameObject.layer.ToString(CultureInfo.InvariantCulture)
				},
				{
					"transform.position",
					gameObject.transform.position.ToString()
				},
				{
					"transform.rotation",
					gameObject.transform.rotation.ToString()
				},
				{
					"tag",
					gameObject.tag
				},
				{
					"activeInHierarchy",
					gameObject.activeInHierarchy.ToString(CultureInfo.InvariantCulture).ToLower()
				},
				{
					"activeSelf",
					gameObject.activeSelf.ToString(CultureInfo.InvariantCulture).ToLower()
				},
				{
					"instanceId",
					gameObject.GetInstanceID().ToString(CultureInfo.InvariantCulture)
				},
				{
					"parentName",
					string.IsNullOrEmpty(parentName) ? "root object" : parentName
				}
			});
		}

		private BacktraceJObject GetJObject(Component gameObject, string parentName = "")
		{
			return new BacktraceJObject(new Dictionary<string, string>
			{
				{
					"name",
					gameObject.name
				},
				{
					"transform.position",
					gameObject.transform.position.ToString()
				},
				{
					"transform.rotation",
					gameObject.transform.rotation.ToString()
				},
				{
					"tag",
					gameObject.tag
				},
				{
					"instanceId",
					gameObject.GetInstanceID().ToString(CultureInfo.InvariantCulture)
				},
				{
					"parentName",
					string.IsNullOrEmpty(parentName) ? "root object" : parentName
				}
			});
		}

		internal static Dictionary<string, string> _environmentVariablesCache;

		internal static bool VariablesLoaded;

		private readonly int _gameObjectDepth;
	}
}
