using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sirenix.OdinInspector
{
	public static class SelfValidationResultItemExtensions
	{
		public static ref SelfValidationResult.ResultItem WithFix(this SelfValidationResult.ResultItem item, string title, Action fix, bool offerInInspector = true)
		{
			item.Fix = new SelfFix?(SelfFix.Create(title, fix, offerInInspector));
			return ref item;
		}

		public static ref SelfValidationResult.ResultItem WithFix<T>(this SelfValidationResult.ResultItem item, string title, Action<T> fix, bool offerInInspector = true) where T : new()
		{
			item.Fix = new SelfFix?(SelfFix.Create<T>(title, fix, offerInInspector));
			return ref item;
		}

		public static ref SelfValidationResult.ResultItem WithFix(this SelfValidationResult.ResultItem item, Action fix, bool offerInInspector = true)
		{
			item.Fix = new SelfFix?(SelfFix.Create(fix, offerInInspector));
			return ref item;
		}

		public static ref SelfValidationResult.ResultItem WithFix<T>(this SelfValidationResult.ResultItem item, Action<T> fix, bool offerInInspector = true) where T : new()
		{
			item.Fix = new SelfFix?(SelfFix.Create<T>(fix, offerInInspector));
			return ref item;
		}

		public static ref SelfValidationResult.ResultItem WithFix(this SelfValidationResult.ResultItem item, SelfFix fix)
		{
			item.Fix = new SelfFix?(fix);
			return ref item;
		}

		public static ref SelfValidationResult.ResultItem WithContextClick(this SelfValidationResult.ResultItem item, Func<IEnumerable<SelfValidationResult.ContextMenuItem>> onContextClick)
		{
			item.OnContextClick = (Func<IEnumerable<SelfValidationResult.ContextMenuItem>>)Delegate.Combine(item.OnContextClick, onContextClick);
			return ref item;
		}

		public static ref SelfValidationResult.ResultItem WithContextClick(this SelfValidationResult.ResultItem item, string path, Action onClick)
		{
			item.OnContextClick = (Func<IEnumerable<SelfValidationResult.ContextMenuItem>>)Delegate.Combine(item.OnContextClick, new Func<IEnumerable<SelfValidationResult.ContextMenuItem>>(() => new SelfValidationResult.ContextMenuItem[]
			{
				new SelfValidationResult.ContextMenuItem
				{
					Path = path,
					OnClick = onClick
				}
			}));
			return ref item;
		}

		public static ref SelfValidationResult.ResultItem WithContextClick(this SelfValidationResult.ResultItem item, string path, bool on, Action onClick)
		{
			item.OnContextClick = (Func<IEnumerable<SelfValidationResult.ContextMenuItem>>)Delegate.Combine(item.OnContextClick, new Func<IEnumerable<SelfValidationResult.ContextMenuItem>>(() => new SelfValidationResult.ContextMenuItem[]
			{
				new SelfValidationResult.ContextMenuItem
				{
					Path = path,
					On = on,
					OnClick = onClick
				}
			}));
			return ref item;
		}

		public static ref SelfValidationResult.ResultItem WithContextClick(this SelfValidationResult.ResultItem item, SelfValidationResult.ContextMenuItem onContextClick)
		{
			item.OnContextClick = (Func<IEnumerable<SelfValidationResult.ContextMenuItem>>)Delegate.Combine(item.OnContextClick, new Func<IEnumerable<SelfValidationResult.ContextMenuItem>>(() => new SelfValidationResult.ContextMenuItem[]
			{
				onContextClick
			}));
			return ref item;
		}

		public static ref SelfValidationResult.ResultItem WithSceneGUI(this SelfValidationResult.ResultItem item, Action onSceneGUI)
		{
			item.OnSceneGUI = onSceneGUI;
			return ref item;
		}

		public static ref SelfValidationResult.ResultItem SetSelectionObject(this SelfValidationResult.ResultItem item, UnityEngine.Object uObj)
		{
			item.SelectionObject = uObj;
			return ref item;
		}

		public static ref SelfValidationResult.ResultItem EnableRichText(this SelfValidationResult.ResultItem item)
		{
			item.RichText = true;
			return ref item;
		}

		public static ref SelfValidationResult.ResultItem WithMetaData(this SelfValidationResult.ResultItem resultItem, string name, object value, params Attribute[] attributes)
		{
			resultItem.MetaData = (resultItem.MetaData ?? new SelfValidationResult.ResultItemMetaData[0]);
			Array.Resize<SelfValidationResult.ResultItemMetaData>(ref resultItem.MetaData, resultItem.MetaData.Length + 1);
			resultItem.MetaData[resultItem.MetaData.Length - 1] = new SelfValidationResult.ResultItemMetaData(name, value, attributes);
			return ref resultItem;
		}

		public static ref SelfValidationResult.ResultItem WithMetaData(this SelfValidationResult.ResultItem resultItem, object value, params Attribute[] attributes)
		{
			resultItem.MetaData = (resultItem.MetaData ?? new SelfValidationResult.ResultItemMetaData[0]);
			Array.Resize<SelfValidationResult.ResultItemMetaData>(ref resultItem.MetaData, resultItem.MetaData.Length + 1);
			resultItem.MetaData[resultItem.MetaData.Length - 1] = new SelfValidationResult.ResultItemMetaData(null, value, attributes);
			return ref resultItem;
		}

		public static ref SelfValidationResult.ResultItem WithButton(this SelfValidationResult.ResultItem resultItem, string name, Action onClick)
		{
			resultItem.MetaData = (resultItem.MetaData ?? new SelfValidationResult.ResultItemMetaData[0]);
			Array.Resize<SelfValidationResult.ResultItemMetaData>(ref resultItem.MetaData, resultItem.MetaData.Length + 1);
			resultItem.MetaData[resultItem.MetaData.Length - 1] = new SelfValidationResult.ResultItemMetaData(name, onClick, Array.Empty<Attribute>());
			return ref resultItem;
		}
	}
}
