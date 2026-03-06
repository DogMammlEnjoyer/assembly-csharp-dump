using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace UnityEngine.Rendering
{
	[Serializable]
	public class VolumeComponent : ScriptableObject
	{
		public string displayName { get; protected set; } = "";

		public ReadOnlyCollection<VolumeParameter> parameters
		{
			get
			{
				ReadOnlyCollection<VolumeParameter> result;
				if ((result = this.m_ParameterReadOnlyCollection) == null)
				{
					result = (this.m_ParameterReadOnlyCollection = new ReadOnlyCollection<VolumeParameter>(this.parameterList));
				}
				return result;
			}
		}

		internal static void FindParameters(object o, List<VolumeParameter> parameters, Func<FieldInfo, bool> filter = null)
		{
			if (o == null)
			{
				return;
			}
			foreach (FieldInfo fieldInfo in from t in o.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			orderby t.MetadataToken
			select t)
			{
				Type fieldType = fieldInfo.FieldType;
				if (fieldType.IsSubclassOf(typeof(VolumeParameter)))
				{
					if (filter == null || filter(fieldInfo))
					{
						VolumeParameter item = (VolumeParameter)fieldInfo.GetValue(o);
						parameters.Add(item);
					}
				}
				else if (!fieldType.IsArray && fieldType.IsClass)
				{
					VolumeComponent.FindParameters(fieldInfo.GetValue(o), parameters, filter);
				}
			}
		}

		protected virtual void OnEnable()
		{
			List<VolumeParameter> list;
			ListPool<VolumeParameter>.Get(out list);
			VolumeComponent.FindParameters(this, list, null);
			this.parameterList = list.ToArray();
			ListPool<VolumeParameter>.Release(list);
			foreach (VolumeParameter volumeParameter in this.parameterList)
			{
				if (volumeParameter != null)
				{
					volumeParameter.OnEnable();
				}
				else
				{
					Debug.LogWarning("Volume Component " + base.GetType().Name + " contains a null parameter; please make sure all parameters are initialized to a default value. Until this is fixed the null parameters will not be considered by the system.");
				}
			}
		}

		protected virtual void OnDisable()
		{
			foreach (VolumeParameter volumeParameter in this.parameterList)
			{
				if (volumeParameter != null)
				{
					volumeParameter.OnDisable();
				}
			}
		}

		public virtual void Override(VolumeComponent state, float interpFactor)
		{
			int num = this.parameterList.Length;
			for (int i = 0; i < num; i++)
			{
				VolumeParameter volumeParameter = state.parameterList[i];
				VolumeParameter volumeParameter2 = this.parameterList[i];
				if (volumeParameter2.overrideState)
				{
					volumeParameter.overrideState = volumeParameter2.overrideState;
					volumeParameter.Interp(volumeParameter, volumeParameter2, interpFactor);
				}
			}
		}

		public void SetAllOverridesTo(bool state)
		{
			this.SetOverridesTo(this.parameterList, state);
		}

		internal void SetOverridesTo(IEnumerable<VolumeParameter> enumerable, bool state)
		{
			foreach (VolumeParameter volumeParameter in enumerable)
			{
				volumeParameter.overrideState = state;
				Type type = volumeParameter.GetType();
				if (VolumeParameter.IsObjectParameter(type))
				{
					ReadOnlyCollection<VolumeParameter> readOnlyCollection = (ReadOnlyCollection<VolumeParameter>)type.GetProperty("parameters", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(volumeParameter, null);
					if (readOnlyCollection != null)
					{
						this.SetOverridesTo(readOnlyCollection, state);
					}
				}
			}
		}

		public override int GetHashCode()
		{
			int num = 17;
			for (int i = 0; i < this.parameterList.Length; i++)
			{
				num = num * 23 + this.parameterList[i].GetHashCode();
			}
			return num;
		}

		public bool AnyPropertiesIsOverridden()
		{
			for (int i = 0; i < this.parameterList.Length; i++)
			{
				if (this.parameterList[i].overrideState)
				{
					return true;
				}
			}
			return false;
		}

		protected virtual void OnDestroy()
		{
			this.Release();
		}

		public void Release()
		{
			if (this.parameterList == null)
			{
				return;
			}
			for (int i = 0; i < this.parameterList.Length; i++)
			{
				if (this.parameterList[i] != null)
				{
					this.parameterList[i].Release();
				}
			}
		}

		public bool active = true;

		internal VolumeParameter[] parameterList;

		private ReadOnlyCollection<VolumeParameter> m_ParameterReadOnlyCollection;

		public sealed class Indent : PropertyAttribute
		{
			public Indent(int relativeAmount = 1)
			{
				this.relativeAmount = relativeAmount;
			}

			public readonly int relativeAmount;
		}
	}
}
