using System;
using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine;

namespace Unity.XR.CoreUtils.Datums
{
	public abstract class Datum<T> : ScriptableObject
	{
		public string Comments
		{
			get
			{
				return this.m_Comments;
			}
			set
			{
				this.m_Comments = value;
			}
		}

		public bool ReadOnly
		{
			get
			{
				return this.m_ReadOnly;
			}
			set
			{
				this.m_ReadOnly = value;
			}
		}

		public IReadOnlyBindableVariable<T> BindableVariableReference
		{
			get
			{
				return this.m_BindableVariableReference;
			}
		}

		public T Value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				if (this.m_ReadOnly)
				{
					Debug.LogWarning(string.Format("{0} ValueDatum is set to read-only, variable can't be changed!", this), this);
					return;
				}
				this.m_Value = value;
				this.m_BindableVariableReference.Value = value;
			}
		}

		protected void OnEnable()
		{
			this.m_BindableVariableReference.Value = this.Value;
		}

		[Multiline]
		[SerializeField]
		private string m_Comments;

		[SerializeField]
		private bool m_ReadOnly = true;

		[SerializeField]
		private T m_Value;

		private readonly BindableVariableAlloc<T> m_BindableVariableReference = new BindableVariableAlloc<T>(default(T), true, null, false);
	}
}
