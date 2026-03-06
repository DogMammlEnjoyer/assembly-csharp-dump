using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.ProBuilder
{
	internal sealed class PreferenceDictionary : ScriptableObject, ISerializationCallbackReceiver, IHasDefault
	{
		public void OnBeforeSerialize()
		{
			this.m_Bool_keys = this.m_Bool.Keys.ToList<string>();
			this.m_Int_keys = this.m_Int.Keys.ToList<string>();
			this.m_Float_keys = this.m_Float.Keys.ToList<string>();
			this.m_String_keys = this.m_String.Keys.ToList<string>();
			this.m_Color_keys = this.m_Color.Keys.ToList<string>();
			this.m_Material_keys = this.m_Material.Keys.ToList<string>();
			this.m_Bool_values = this.m_Bool.Values.ToList<bool>();
			this.m_Int_values = this.m_Int.Values.ToList<int>();
			this.m_Float_values = this.m_Float.Values.ToList<float>();
			this.m_String_values = this.m_String.Values.ToList<string>();
			this.m_Color_values = this.m_Color.Values.ToList<Color>();
			this.m_Material_values = this.m_Material.Values.ToList<Material>();
		}

		public void OnAfterDeserialize()
		{
			for (int i = 0; i < this.m_Bool_keys.Count; i++)
			{
				this.m_Bool.Add(this.m_Bool_keys[i], this.m_Bool_values[i]);
			}
			for (int j = 0; j < this.m_Int_keys.Count; j++)
			{
				this.m_Int.Add(this.m_Int_keys[j], this.m_Int_values[j]);
			}
			for (int k = 0; k < this.m_Float_keys.Count; k++)
			{
				this.m_Float.Add(this.m_Float_keys[k], this.m_Float_values[k]);
			}
			for (int l = 0; l < this.m_String_keys.Count; l++)
			{
				this.m_String.Add(this.m_String_keys[l], this.m_String_values[l]);
			}
			for (int m = 0; m < this.m_Color_keys.Count; m++)
			{
				this.m_Color.Add(this.m_Color_keys[m], this.m_Color_values[m]);
			}
			for (int n = 0; n < this.m_Material_keys.Count; n++)
			{
				this.m_Material.Add(this.m_Material_keys[n], this.m_Material_values[n]);
			}
		}

		public void SetDefaultValues()
		{
			this.m_Bool.Clear();
			this.m_Int.Clear();
			this.m_Float.Clear();
			this.m_String.Clear();
			this.m_Color.Clear();
			this.m_Material.Clear();
		}

		public bool HasKey(string key)
		{
			return this.m_Bool.ContainsKey(key) || this.m_Int.ContainsKey(key) || this.m_Float.ContainsKey(key) || this.m_String.ContainsKey(key) || this.m_Color.ContainsKey(key) || this.m_Material.ContainsKey(key);
		}

		public bool HasKey<T>(string key)
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(int))
			{
				return this.m_Int.ContainsKey(key);
			}
			if (typeFromHandle == typeof(float))
			{
				return this.m_Float.ContainsKey(key);
			}
			if (typeFromHandle == typeof(bool))
			{
				return this.m_Bool.ContainsKey(key);
			}
			if (typeFromHandle == typeof(string))
			{
				return this.m_String.ContainsKey(key);
			}
			if (typeFromHandle == typeof(Color))
			{
				return this.m_Color.ContainsKey(key);
			}
			if (typeFromHandle == typeof(Material))
			{
				return this.m_Material.ContainsKey(key);
			}
			Debug.LogWarning(string.Format("HasKey<{0}>({1}) not valid preference type.", typeof(T).ToString(), key));
			return false;
		}

		public void DeleteKey(string key)
		{
			if (this.m_Bool.ContainsKey(key))
			{
				this.m_Bool.Remove(key);
			}
			if (this.m_Int.ContainsKey(key))
			{
				this.m_Int.Remove(key);
			}
			if (this.m_Float.ContainsKey(key))
			{
				this.m_Float.Remove(key);
			}
			if (this.m_String.ContainsKey(key))
			{
				this.m_String.Remove(key);
			}
			if (this.m_Color.ContainsKey(key))
			{
				this.m_Color.Remove(key);
			}
			if (this.m_Material.ContainsKey(key))
			{
				this.m_Material.Remove(key);
			}
		}

		public T Get<T>(string key, T fallback = default(T))
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(int))
			{
				if (this.m_Int.ContainsKey(key))
				{
					return (T)((object)this.GetInt(key, 0));
				}
			}
			else if (typeFromHandle == typeof(float))
			{
				if (this.m_Float.ContainsKey(key))
				{
					return (T)((object)this.GetFloat(key, 0f));
				}
			}
			else if (typeFromHandle == typeof(bool))
			{
				if (this.m_Bool.ContainsKey(key))
				{
					return (T)((object)this.GetBool(key, false));
				}
			}
			else if (typeFromHandle == typeof(string))
			{
				if (this.m_String.ContainsKey(key))
				{
					return (T)((object)this.GetString(key, null));
				}
			}
			else if (typeFromHandle == typeof(Color))
			{
				if (this.m_Color.ContainsKey(key))
				{
					return (T)((object)this.GetColor(key, default(Color)));
				}
			}
			else if (typeFromHandle == typeof(Material))
			{
				if (this.m_Material.ContainsKey(key))
				{
					return (T)((object)this.GetMaterial(key, null));
				}
			}
			else
			{
				Debug.LogWarning(string.Format("Get<{0}>({1}) not valid preference type.", typeof(T).ToString(), key));
			}
			return fallback;
		}

		public void Set<T>(string key, T value)
		{
			object obj = value;
			if (value is int)
			{
				this.SetInt(key, (int)obj);
				return;
			}
			if (value is float)
			{
				this.SetFloat(key, (float)obj);
				return;
			}
			if (value is bool)
			{
				this.SetBool(key, (bool)obj);
				return;
			}
			if (value is string)
			{
				this.SetString(key, (string)obj);
				return;
			}
			if (value is Color)
			{
				this.SetColor(key, (Color)obj);
				return;
			}
			if (value is Material)
			{
				this.SetMaterial(key, (Material)obj);
				return;
			}
			Debug.LogWarning(string.Format("Set<{0}>({1}, {2}) not valid preference type.", typeof(T).ToString(), key, value.ToString()));
		}

		public bool GetBool(string key, bool fallback = false)
		{
			bool result;
			if (this.m_Bool.TryGetValue(key, out result))
			{
				return result;
			}
			return fallback;
		}

		public int GetInt(string key, int fallback = 0)
		{
			int result;
			if (this.m_Int.TryGetValue(key, out result))
			{
				return result;
			}
			return fallback;
		}

		public float GetFloat(string key, float fallback = 0f)
		{
			float result;
			if (this.m_Float.TryGetValue(key, out result))
			{
				return result;
			}
			return fallback;
		}

		public string GetString(string key, string fallback = null)
		{
			string result;
			if (this.m_String.TryGetValue(key, out result))
			{
				return result;
			}
			return fallback;
		}

		public Color GetColor(string key, Color fallback = default(Color))
		{
			Color result;
			if (this.m_Color.TryGetValue(key, out result))
			{
				return result;
			}
			return fallback;
		}

		public Material GetMaterial(string key, Material fallback = null)
		{
			Material result;
			if (this.m_Material.TryGetValue(key, out result))
			{
				return result;
			}
			return fallback;
		}

		public void SetBool(string key, bool value)
		{
			this.m_Bool[key] = value;
		}

		public void SetInt(string key, int value)
		{
			this.m_Int[key] = value;
		}

		public void SetFloat(string key, float value)
		{
			this.m_Float[key] = value;
		}

		public void SetString(string key, string value)
		{
			this.m_String[key] = value;
		}

		public void SetColor(string key, Color value)
		{
			this.m_Color[key] = value;
		}

		public void SetMaterial(string key, Material value)
		{
			this.m_Material[key] = value;
		}

		public Dictionary<string, bool> GetBoolDictionary()
		{
			return this.m_Bool;
		}

		public Dictionary<string, int> GetIntDictionary()
		{
			return this.m_Int;
		}

		public Dictionary<string, float> GetFloatDictionary()
		{
			return this.m_Float;
		}

		public Dictionary<string, string> GetStringDictionary()
		{
			return this.m_String;
		}

		public Dictionary<string, Color> GetColorDictionary()
		{
			return this.m_Color;
		}

		public Dictionary<string, Material> GetMaterialDictionary()
		{
			return this.m_Material;
		}

		public void Clear()
		{
			this.m_Bool.Clear();
			this.m_Int.Clear();
			this.m_Float.Clear();
			this.m_String.Clear();
			this.m_Color.Clear();
		}

		private Dictionary<string, bool> m_Bool = new Dictionary<string, bool>();

		private Dictionary<string, int> m_Int = new Dictionary<string, int>();

		private Dictionary<string, float> m_Float = new Dictionary<string, float>();

		private Dictionary<string, string> m_String = new Dictionary<string, string>();

		private Dictionary<string, Color> m_Color = new Dictionary<string, Color>();

		private Dictionary<string, Material> m_Material = new Dictionary<string, Material>();

		[SerializeField]
		private List<string> m_Bool_keys;

		[SerializeField]
		private List<string> m_Int_keys;

		[SerializeField]
		private List<string> m_Float_keys;

		[SerializeField]
		private List<string> m_String_keys;

		[SerializeField]
		private List<string> m_Color_keys;

		[SerializeField]
		private List<string> m_Material_keys;

		[SerializeField]
		private List<bool> m_Bool_values;

		[SerializeField]
		private List<int> m_Int_values;

		[SerializeField]
		private List<float> m_Float_values;

		[SerializeField]
		private List<string> m_String_values;

		[SerializeField]
		private List<Color> m_Color_values;

		[SerializeField]
		private List<Material> m_Material_values;
	}
}
