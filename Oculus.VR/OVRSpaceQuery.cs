using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

internal static class OVRSpaceQuery
{
	[return: TupleElementNames(new string[]
	{
		"result",
		"why"
	})]
	public static ValueTuple<OVRPlugin.Result, string> ForAnchors([CanBeNull] IEnumerable<Guid> anchorIds, out OVRPlugin.SpaceQueryInfo2 query)
	{
		query = OVRSpaceQuery.s_TemplateQuery;
		query.FilterType = OVRPlugin.SpaceQueryFilterType.Ids;
		query.MaxQuerySpaces = 1024;
		return OVRSpaceQuery.AppendAnchors(ref query, anchorIds);
	}

	internal static OVRPlugin.SpaceQueryInfo2 ForAnchorsUnchecked(OVREnumerable<Guid> anchorIds)
	{
		OVRPlugin.SpaceQueryInfo2 spaceQueryInfo = OVRSpaceQuery.s_TemplateQuery;
		spaceQueryInfo.FilterType = OVRPlugin.SpaceQueryFilterType.Ids;
		spaceQueryInfo.MaxQuerySpaces = 1024;
		foreach (Guid guid in anchorIds)
		{
			Guid[] ids = spaceQueryInfo.IdInfo.Ids;
			int numIds = spaceQueryInfo.IdInfo.NumIds;
			spaceQueryInfo.IdInfo.NumIds = numIds + 1;
			ids[numIds] = guid;
		}
		OVRSpaceQuery.PostProcessQuery(ref spaceQueryInfo, OVRPlugin.Result.Success, string.Empty);
		return spaceQueryInfo;
	}

	internal static OVRPlugin.SpaceQueryInfo2 ForAnchorsThrow([NotNull] IEnumerable<Guid> anchorIds, string argName = null)
	{
		OVRPlugin.SpaceQueryInfo2 result;
		ValueTuple<OVRPlugin.Result, string> valueTuple = OVRSpaceQuery.ForAnchors(anchorIds, out result);
		OVRPlugin.Result item = valueTuple.Item1;
		string text = valueTuple.Item2;
		if (item.IsSuccess())
		{
			return result;
		}
		text = string.Format("{0} ({1} {2})", text, (int)item, item);
		if (item == OVRPlugin.Result.Failure_HandleInvalid || item == OVRPlugin.Result.Failure_InvalidParameter)
		{
			throw new ArgumentException(text, argName);
		}
		throw new InvalidOperationException(text);
	}

	[return: TupleElementNames(new string[]
	{
		"result",
		"why"
	})]
	public static ValueTuple<OVRPlugin.Result, string> ForComponent(OVRPlugin.SpaceComponentType type, out OVRPlugin.SpaceQueryInfo2 query)
	{
		query = OVRSpaceQuery.s_TemplateQuery;
		query.FilterType = OVRPlugin.SpaceQueryFilterType.Components;
		query.Location = OVRPlugin.SpaceStorageLocation.Local;
		query.MaxQuerySpaces = 1024;
		query.ComponentsInfo.Components[0] = type;
		query.ComponentsInfo.NumComponents = 1;
		return OVRSpaceQuery.PostProcessQuery(ref query, OVRPlugin.Result.Success, string.Empty);
	}

	internal static OVRPlugin.SpaceQueryInfo2 ForComponentUnchecked(OVRPlugin.SpaceComponentType type)
	{
		OVRPlugin.SpaceQueryInfo2 spaceQueryInfo = OVRSpaceQuery.s_TemplateQuery;
		spaceQueryInfo.FilterType = OVRPlugin.SpaceQueryFilterType.Components;
		spaceQueryInfo.Location = OVRPlugin.SpaceStorageLocation.Local;
		spaceQueryInfo.MaxQuerySpaces = 1024;
		spaceQueryInfo.ComponentsInfo.Components[0] = type;
		spaceQueryInfo.ComponentsInfo.NumComponents = 1;
		OVRSpaceQuery.PostProcessQuery(ref spaceQueryInfo, OVRPlugin.Result.Success, string.Empty);
		return spaceQueryInfo;
	}

	internal static OVRPlugin.SpaceQueryInfo2 ForComponentThrow(OVRPlugin.SpaceComponentType type, string argName = null)
	{
		OVRPlugin.SpaceQueryInfo2 result;
		ValueTuple<OVRPlugin.Result, string> valueTuple = OVRSpaceQuery.ForComponent(type, out result);
		OVRPlugin.Result item = valueTuple.Item1;
		string text = valueTuple.Item2;
		if (item.IsSuccess())
		{
			return result;
		}
		text = string.Format("{0} ({1} {2})", text, (int)item, item);
		if (item == OVRPlugin.Result.Failure_InvalidParameter)
		{
			throw new ArgumentException(text, argName);
		}
		throw new InvalidOperationException(text);
	}

	[return: TupleElementNames(new string[]
	{
		"result",
		"why"
	})]
	public static ValueTuple<OVRPlugin.Result, string> ForGroup(Guid groupUuid, out OVRPlugin.SpaceQueryInfo2 query, IEnumerable<Guid> anchorIds = null)
	{
		query = OVRSpaceQuery.s_TemplateQuery;
		query.FilterType = OVRPlugin.SpaceQueryFilterType.Group;
		query.MaxQuerySpaces = 1024;
		query.GroupUuidInfo = groupUuid;
		OVRPlugin.Result result = OVRPlugin.Result.Success;
		string empty = string.Empty;
		if (groupUuid == Guid.Empty)
		{
			result = OVRPlugin.Result.Failure_InvalidParameter;
		}
		else if (anchorIds != null)
		{
			return OVRSpaceQuery.AppendAnchors(ref query, anchorIds);
		}
		return OVRSpaceQuery.PostProcessQuery(ref query, result, empty);
	}

	internal static OVRPlugin.SpaceQueryInfo2 ForGroupUnchecked(Guid groupUuid, OVREnumerable<Guid> anchorIds = default(OVREnumerable<Guid>))
	{
		OVRPlugin.SpaceQueryInfo2 spaceQueryInfo = OVRSpaceQuery.s_TemplateQuery;
		spaceQueryInfo.FilterType = OVRPlugin.SpaceQueryFilterType.Group;
		spaceQueryInfo.MaxQuerySpaces = 1024;
		spaceQueryInfo.GroupUuidInfo = groupUuid;
		foreach (Guid guid in anchorIds)
		{
			Guid[] ids = spaceQueryInfo.IdInfo.Ids;
			int numIds = spaceQueryInfo.IdInfo.NumIds;
			spaceQueryInfo.IdInfo.NumIds = numIds + 1;
			ids[numIds] = guid;
		}
		OVRSpaceQuery.PostProcessQuery(ref spaceQueryInfo, OVRPlugin.Result.Success, string.Empty);
		return spaceQueryInfo;
	}

	internal static OVRPlugin.SpaceQueryInfo2 ForGroupThrow(Guid groupUuid, string argName = null, IEnumerable<Guid> anchorIds = null)
	{
		OVRPlugin.SpaceQueryInfo2 result;
		ValueTuple<OVRPlugin.Result, string> valueTuple = OVRSpaceQuery.ForGroup(groupUuid, out result, anchorIds);
		OVRPlugin.Result item = valueTuple.Item1;
		string text = valueTuple.Item2;
		if (item.IsSuccess())
		{
			return result;
		}
		text = string.Format("{0} ({1} {2})", text, (int)item, item);
		if (item == OVRPlugin.Result.Failure_HandleInvalid || item == OVRPlugin.Result.Failure_InvalidParameter)
		{
			throw new ArgumentException(text, argName);
		}
		throw new InvalidOperationException(text);
	}

	public static OVRPlugin.SpaceQueryInfo ToV1(this OVRPlugin.SpaceQueryInfo2 query2)
	{
		return new OVRSpaceQuery.QueryInfoUnion
		{
			V2 = query2
		}.V1;
	}

	public static OVRPlugin.SpaceQueryInfo2 ToV2(this OVRPlugin.SpaceQueryInfo query1)
	{
		return new OVRSpaceQuery.QueryInfoUnion
		{
			V1 = query1
		}.V2;
	}

	[return: TupleElementNames(new string[]
	{
		"result",
		"why"
	})]
	private static ValueTuple<OVRPlugin.Result, string> AppendAnchors(ref OVRPlugin.SpaceQueryInfo2 query, IEnumerable<Guid> anchorIds)
	{
		OVRPlugin.Result result = OVRPlugin.Result.Success;
		string empty = string.Empty;
		if (query.FilterType != OVRPlugin.SpaceQueryFilterType.Ids && query.FilterType != OVRPlugin.SpaceQueryFilterType.Group)
		{
			result = OVRPlugin.Result.Failure_InvalidOperation;
			return OVRSpaceQuery.PostProcessQuery(ref query, result, empty);
		}
		foreach (Guid guid in anchorIds.ToNonAlloc<Guid>())
		{
			if (query.IdInfo.NumIds >= query.MaxQuerySpaces)
			{
				result = OVRPlugin.Result.Failure_InvalidParameter;
				return OVRSpaceQuery.PostProcessQuery(ref query, result, empty);
			}
			Guid[] ids = query.IdInfo.Ids;
			int numIds = query.IdInfo.NumIds;
			query.IdInfo.NumIds = numIds + 1;
			ids[numIds] = guid;
		}
		return OVRSpaceQuery.PostProcessQuery(ref query, result, empty);
	}

	[return: TupleElementNames(new string[]
	{
		"result",
		"why"
	})]
	private static ValueTuple<OVRPlugin.Result, string> PostProcessQuery(ref OVRPlugin.SpaceQueryInfo2 query, OVRPlugin.Result result, in string why)
	{
		if (result.IsSuccess())
		{
			if (query.MaxQuerySpaces > query.IdInfo.NumIds && query.IdInfo.NumIds > 0)
			{
				query.MaxQuerySpaces = query.IdInfo.NumIds;
			}
		}
		else
		{
			query.MaxQuerySpaces = 0;
			query.IdInfo.NumIds = 0;
		}
		return new ValueTuple<OVRPlugin.Result, string>(result, why);
	}

	public const int MaxResultsForAnchors = 1024;

	public const int MaxResultsForGroup = 1024;

	public const OVRPlugin.SpaceStorageLocation DefaultStorageLocation = OVRPlugin.SpaceStorageLocation.Cloud;

	public const double DefaultTimeout = 0.0;

	private static readonly Guid[] s_Ids = new Guid[1024];

	private static readonly OVRPlugin.SpaceComponentType[] s_ComponentTypes = new OVRPlugin.SpaceComponentType[16];

	private static readonly OVRPlugin.SpaceQueryInfo2 s_TemplateQuery = new OVRPlugin.SpaceQueryInfo2
	{
		QueryType = OVRPlugin.SpaceQueryType.Action,
		ActionType = OVRPlugin.SpaceQueryActionType.Load,
		Location = OVRPlugin.SpaceStorageLocation.Cloud,
		Timeout = 0.0,
		IdInfo = new OVRPlugin.SpaceFilterInfoIds
		{
			Ids = OVRSpaceQuery.s_Ids
		},
		ComponentsInfo = new OVRPlugin.SpaceFilterInfoComponents
		{
			Components = OVRSpaceQuery.s_ComponentTypes
		}
	};

	[StructLayout(LayoutKind.Explicit)]
	private struct QueryInfoUnion
	{
		[FieldOffset(0)]
		public OVRPlugin.SpaceQueryInfo V1;

		[FieldOffset(0)]
		public OVRPlugin.SpaceQueryInfo2 V2;
	}

	[Obsolete("This helper is for obsolete usages of xrQuerySpacesFB. See OVRAnchor.FetchAnchorsAsync.")]
	public struct Options
	{
		public int MaxResults { readonly get; set; }

		public double Timeout { readonly get; set; }

		public OVRSpace.StorageLocation Location { readonly get; set; }

		public OVRPlugin.SpaceQueryType QueryType { readonly get; set; }

		public OVRPlugin.SpaceQueryActionType ActionType { readonly get; set; }

		public OVRPlugin.SpaceComponentType ComponentFilter
		{
			get
			{
				return this._componentType;
			}
			set
			{
				OVRSpaceQuery.Options.ValidateSingleFilter(this._uuidFilter, value, this._groupFilter);
				this._componentType = value;
			}
		}

		public IEnumerable<Guid> UuidFilter
		{
			get
			{
				return this._uuidFilter;
			}
			set
			{
				OVRSpaceQuery.Options.ValidateSingleFilter(value, this._componentType, this._groupFilter);
				IReadOnlyCollection<Guid> readOnlyCollection = value as IReadOnlyCollection<Guid>;
				if (readOnlyCollection != null && readOnlyCollection.Count > 1024)
				{
					throw new ArgumentException(string.Format("There must not be more than {0} UUIDs specified by the {1} (new value contains {2} UUIDs).", 1024, "UuidFilter", readOnlyCollection.Count), "value");
				}
				this._uuidFilter = value;
			}
		}

		public Guid? GroupFilter
		{
			get
			{
				return this._groupFilter;
			}
			set
			{
				OVRSpaceQuery.Options.ValidateSingleFilter(this._uuidFilter, this._componentType, value);
				this._groupFilter = value;
			}
		}

		public OVRPlugin.SpaceQueryInfo ToQueryInfo()
		{
			OVRPlugin.SpaceQueryInfo2 spaceQueryInfo;
			OVRPlugin.Result item;
			string item2;
			if (this._uuidFilter != null)
			{
				ValueTuple<OVRPlugin.Result, string> valueTuple = OVRSpaceQuery.ForAnchors(this._uuidFilter, out spaceQueryInfo);
				item = valueTuple.Item1;
				item2 = valueTuple.Item2;
			}
			else
			{
				ValueTuple<OVRPlugin.Result, string> valueTuple2 = OVRSpaceQuery.ForComponent(this._componentType, out spaceQueryInfo);
				item = valueTuple2.Item1;
				item2 = valueTuple2.Item2;
			}
			if (item.IsSuccess())
			{
				return spaceQueryInfo.ToV1();
			}
			if (item == OVRPlugin.Result.Failure_InvalidParameter)
			{
				throw new InvalidOperationException(string.Format("{0} must not contain more than {1} UUIDs.", "UuidFilter", 1024));
			}
			throw new InvalidOperationException(item2);
		}

		public OVRPlugin.SpaceQueryInfo2 ToQueryInfo2()
		{
			OVRPlugin.SpaceQueryInfo2 result;
			OVRPlugin.Result item;
			string item2;
			if (this._groupFilter != null)
			{
				ValueTuple<OVRPlugin.Result, string> valueTuple = OVRSpaceQuery.ForGroup(this._groupFilter.Value, out result, this._uuidFilter);
				item = valueTuple.Item1;
				item2 = valueTuple.Item2;
			}
			else if (this._uuidFilter != null)
			{
				ValueTuple<OVRPlugin.Result, string> valueTuple2 = OVRSpaceQuery.ForAnchors(this._uuidFilter, out result);
				item = valueTuple2.Item1;
				item2 = valueTuple2.Item2;
			}
			else
			{
				ValueTuple<OVRPlugin.Result, string> valueTuple3 = OVRSpaceQuery.ForComponent(this._componentType, out result);
				item = valueTuple3.Item1;
				item2 = valueTuple3.Item2;
			}
			if (item.IsSuccess())
			{
				return result;
			}
			if (item == OVRPlugin.Result.Failure_InvalidParameter)
			{
				throw new InvalidOperationException(string.Format("{0} must not contain more than {1} UUIDs.", "UuidFilter", 1024));
			}
			throw new InvalidOperationException(item2);
		}

		public bool TryQuerySpaces(out ulong requestId)
		{
			return OVRPlugin.QuerySpaces(this.ToQueryInfo(), out requestId);
		}

		private static void ValidateSingleFilter(IEnumerable<Guid> uuidFilter, OVRPlugin.SpaceComponentType componentFilter, Guid? groupFilter)
		{
			int num = 0;
			if (uuidFilter != null)
			{
				num++;
			}
			if (groupFilter != null)
			{
				num++;
			}
			if (componentFilter != OVRPlugin.SpaceComponentType.Locatable)
			{
				num++;
			}
			if (num > 1)
			{
				throw new InvalidOperationException("You may only query by one of UUID, Group, or component type.");
			}
		}

		public const int MaxUuidCount = 1024;

		private OVRPlugin.SpaceComponentType _componentType;

		private IEnumerable<Guid> _uuidFilter;

		private Guid? _groupFilter;
	}
}
