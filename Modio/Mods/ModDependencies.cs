using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.API.SchemaDefinitions;
using Modio.Extensions;

namespace Modio.Mods
{
	public class ModDependencies
	{
		public int Count
		{
			get
			{
				if (!this.HasDependencies)
				{
					return 0;
				}
				List<Mod> flattenedMods = this._flattenedMods;
				object obj;
				if (flattenedMods == null)
				{
					List<Mod>[] depthMap = this._depthMap;
					if (depthMap == null)
					{
						obj = 0;
					}
					else
					{
						obj = depthMap.Sum((List<Mod> list) => list.Count);
					}
				}
				else
				{
					obj = flattenedMods.Count;
				}
				object obj2 = obj;
				if (obj2 == null && this._isFetchingDependencies == null)
				{
					this.FetchDependencies().ForgetTaskSafely();
				}
				return obj2;
			}
		}

		public bool HasDependencies { get; }

		public bool IsMapped
		{
			get
			{
				return this._depthMap != null;
			}
		}

		internal ModDependencies(Mod dependent, bool hasDependencies)
		{
			this.HasDependencies = hasDependencies;
			this._dependent = dependent;
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"results"
		})]
		public Task<ValueTuple<Error, IReadOnlyList<Mod>>> GetAllDependencies()
		{
			ModDependencies.<GetAllDependencies>d__13 <GetAllDependencies>d__;
			<GetAllDependencies>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, IReadOnlyList<Mod>>>.Create();
			<GetAllDependencies>d__.<>4__this = this;
			<GetAllDependencies>d__.<>1__state = -1;
			<GetAllDependencies>d__.<>t__builder.Start<ModDependencies.<GetAllDependencies>d__13>(ref <GetAllDependencies>d__);
			return <GetAllDependencies>d__.<>t__builder.Task;
		}

		private Task<Error> FetchDependencies()
		{
			ModDependencies.<FetchDependencies>d__14 <FetchDependencies>d__;
			<FetchDependencies>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<FetchDependencies>d__.<>4__this = this;
			<FetchDependencies>d__.<>1__state = -1;
			<FetchDependencies>d__.<>t__builder.Start<ModDependencies.<FetchDependencies>d__14>(ref <FetchDependencies>d__);
			return <FetchDependencies>d__.<>t__builder.Task;
		}

		private static ModObject ConstructModObject(ModDependenciesObject dependency)
		{
			return new ModObject(dependency.Id, dependency.GameId, dependency.Status, dependency.Visible, dependency.SubmittedBy, dependency.DateAdded, dependency.DateUpdated, dependency.DateLive, dependency.MaturityOption, dependency.CommunityOptions, dependency.MonetizationOptions, 0L, dependency.Stock, dependency.Price, dependency.Tax, dependency.Logo, dependency.HomepageUrl, dependency.Name, dependency.NameId, dependency.Summary, dependency.Description, dependency.DescriptionPlaintext, dependency.MetadataBlob, dependency.ProfileUrl, dependency.Media, dependency.Modfile, dependency.Dependencies, dependency.Platforms, dependency.MetadataKvp, dependency.Tags, dependency.Stats);
		}

		private const int MAX_DEPTH = 5;

		private TaskCompletionSource<Error> _isFetchingDependencies;

		private List<Mod>[] _depthMap;

		private readonly Mod _dependent;

		private List<Mod> _flattenedMods;
	}
}
