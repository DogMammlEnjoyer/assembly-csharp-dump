using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sirenix.OdinInspector.Internal;

namespace Sirenix.OdinInspector
{
	public class ColumnGroupAttribute : PropertyGroupAttribute, ISubGroupProviderAttribute
	{
		public ColumnGroupAttribute(string rowId, string columnId, ColumnType columnType = ColumnType.Auto, float columnSize = 0f, float order = 0f) : base(rowId, order)
		{
			this.ColumnId = columnId;
			this.Size = new ColumnSize(columnType, columnSize);
			this.Columns = new List<ColumnGroupAttribute>
			{
				this
			};
		}

		public ColumnGroupAttribute(string rowId, string columnId, float columnSize, float order = 0f) : base(rowId, order)
		{
			this.ColumnId = columnId;
			ColumnSize size;
			if (columnSize <= 0f)
			{
				size = ColumnSize.Auto;
			}
			else if (columnSize <= 1f && columnSize >= 0f)
			{
				size = ColumnSize.Percent(columnSize);
			}
			else
			{
				size = ColumnSize.Pixel(columnSize);
			}
			this.Size = size;
			this.Columns = new List<ColumnGroupAttribute>
			{
				this
			};
		}

		public ColumnGroupAttribute(string columnId) : this("_DefaultRow", columnId, ColumnType.Auto, 0f, 0f)
		{
		}

		public ColumnGroupAttribute(string columnId, float columnSize, float order = 0f) : this("_DefaultRow", columnId, columnSize, order)
		{
		}

		public ColumnGroupAttribute(string columnId, ColumnType columnType, float columnSize, float order = 0f) : this("_DefaultRow", columnId, columnType, columnSize, order)
		{
		}

		public IList<PropertyGroupAttribute> GetSubGroupAttributes()
		{
			int num = 0;
			List<PropertyGroupAttribute> list = new List<PropertyGroupAttribute>(this.Columns.Count)
			{
				new ColumnGroupAttribute.ColumnSubGroupAttribute(this, this.GroupID + "/" + this.ColumnId, (float)num++)
			};
			foreach (ColumnGroupAttribute columnGroupAttribute in this.Columns)
			{
				if (columnGroupAttribute.ColumnId != this.ColumnId)
				{
					list.Add(new ColumnGroupAttribute.ColumnSubGroupAttribute(columnGroupAttribute, this.GroupID + "/" + columnGroupAttribute.ColumnId, (float)num++));
				}
			}
			return list;
		}

		public string RepathMemberAttribute(PropertyGroupAttribute attr)
		{
			ColumnGroupAttribute columnGroupAttribute = (ColumnGroupAttribute)attr;
			return this.GroupID + "/" + columnGroupAttribute.ColumnId;
		}

		protected override void CombineValuesWith(PropertyGroupAttribute other)
		{
			ColumnGroupAttribute columnGroupAttribute = (ColumnGroupAttribute)other;
			for (int i = 0; i < this.Columns.Count; i++)
			{
				if (this.Columns[i].ColumnId == columnGroupAttribute.ColumnId)
				{
					return;
				}
			}
			this.Columns.Add(columnGroupAttribute);
		}

		public const string DEFAULT_ROW_NAME = "_DefaultRow";

		public string ColumnId;

		public List<ColumnGroupAttribute> Columns;

		public ColumnSize Size;

		[Conditional("UNITY_EDITOR")]
		public class ColumnSubGroupAttribute : PropertyGroupAttribute
		{
			public ColumnSubGroupAttribute(ColumnGroupAttribute column, string groupId, float order) : base(groupId, order)
			{
				if (column == null)
				{
					this.Size = ColumnSize.Auto;
					return;
				}
				this.Size = column.Size;
			}

			public ColumnSize Size;
		}
	}
}
