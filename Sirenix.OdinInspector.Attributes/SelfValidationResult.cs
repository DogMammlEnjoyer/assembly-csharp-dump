using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sirenix.OdinInspector
{
	public class SelfValidationResult
	{
		public int Count
		{
			get
			{
				return this.itemsCount;
			}
		}

		public SelfValidationResult.ResultItem this[int index]
		{
			get
			{
				return ref this.items[index];
			}
		}

		public ref SelfValidationResult.ResultItem AddError(string error)
		{
			return this.Add(new SelfValidationResult.ResultItem
			{
				Message = error,
				ResultType = SelfValidationResult.ResultType.Error
			});
		}

		public ref SelfValidationResult.ResultItem AddWarning(string warning)
		{
			return this.Add(new SelfValidationResult.ResultItem
			{
				Message = warning,
				ResultType = SelfValidationResult.ResultType.Warning
			});
		}

		public ref SelfValidationResult.ResultItem Add(ValidatorSeverity severity, string message)
		{
			if (severity == ValidatorSeverity.Error)
			{
				return this.Add(new SelfValidationResult.ResultItem
				{
					Message = message,
					ResultType = SelfValidationResult.ResultType.Error
				});
			}
			if (severity == ValidatorSeverity.Warning)
			{
				return this.Add(new SelfValidationResult.ResultItem
				{
					Message = message,
					ResultType = SelfValidationResult.ResultType.Warning
				});
			}
			SelfValidationResult.NoResultItem = default(SelfValidationResult.ResultItem);
			return ref SelfValidationResult.NoResultItem;
		}

		public ref SelfValidationResult.ResultItem Add(SelfValidationResult.ResultItem item)
		{
			SelfValidationResult.ResultItem[] array = this.items;
			if (array == null)
			{
				array = new SelfValidationResult.ResultItem[2];
				this.items = array;
			}
			while (array.Length <= this.itemsCount + 1)
			{
				SelfValidationResult.ResultItem[] array2 = new SelfValidationResult.ResultItem[array.Length * 2];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = array[i];
				}
				array = array2;
				this.items = array2;
			}
			array[this.itemsCount] = item;
			SelfValidationResult.ResultItem[] array3 = array;
			int num = this.itemsCount;
			this.itemsCount = num + 1;
			return ref array3[num];
		}

		private static SelfValidationResult.ResultItem NoResultItem;

		private SelfValidationResult.ResultItem[] items;

		private int itemsCount;

		public struct ContextMenuItem
		{
			public string Path;

			public bool On;

			public bool AddSeparatorBefore;

			public Action OnClick;
		}

		public enum ResultType
		{
			Error,
			Warning,
			Valid
		}

		public struct ResultItem
		{
			public string Message;

			public SelfValidationResult.ResultType ResultType;

			public SelfFix? Fix;

			public SelfValidationResult.ResultItemMetaData[] MetaData;

			public Func<IEnumerable<SelfValidationResult.ContextMenuItem>> OnContextClick;

			public Action OnSceneGUI;

			public UnityEngine.Object SelectionObject;

			public bool RichText;
		}

		public struct ResultItemMetaData
		{
			public ResultItemMetaData(string name, object value, params Attribute[] attributes)
			{
				this.Name = name;
				this.Value = value;
				this.Attributes = attributes;
			}

			public string Name;

			public object Value;

			public Attribute[] Attributes;
		}
	}
}
