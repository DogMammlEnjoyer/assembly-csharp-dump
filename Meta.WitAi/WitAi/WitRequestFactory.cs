using System;
using System.Collections.Generic;
using Meta.Voice;
using Meta.WitAi.Configuration;
using Meta.WitAi.Data.Configuration;
using Meta.WitAi.Data.Entities;
using Meta.WitAi.Data.Info;
using Meta.WitAi.Interfaces;
using Meta.WitAi.Json;
using Meta.WitAi.Requests;

namespace Meta.WitAi
{
	public static class WitRequestFactory
	{
		private static VoiceServiceRequestOptions.QueryParam QueryParam(string key, string value)
		{
			return new VoiceServiceRequestOptions.QueryParam
			{
				key = key,
				value = value
			};
		}

		private static void HandleWitRequestOptions(WitRequestOptions requestOptions, IDynamicEntitiesProvider[] additionalEntityProviders)
		{
			WitResponseClass witResponseClass = new WitResponseClass();
			bool flag = false;
			if (additionalEntityProviders != null)
			{
				for (int i = 0; i < additionalEntityProviders.Length; i++)
				{
					foreach (WitDynamicEntity providerEntity in additionalEntityProviders[i].GetDynamicEntities())
					{
						flag = true;
						WitRequestFactory.MergeEntities(witResponseClass, providerEntity);
					}
				}
			}
			if (DynamicEntityKeywordRegistry.HasDynamicEntityRegistry)
			{
				foreach (WitDynamicEntity providerEntity2 in DynamicEntityKeywordRegistry.Instance.GetDynamicEntities())
				{
					flag = true;
					WitRequestFactory.MergeEntities(witResponseClass, providerEntity2);
				}
			}
			if (requestOptions != null && requestOptions.dynamicEntities != null)
			{
				foreach (WitDynamicEntity providerEntity3 in requestOptions.dynamicEntities.GetDynamicEntities())
				{
					flag = true;
					WitRequestFactory.MergeEntities(witResponseClass, providerEntity3);
				}
			}
			if (flag)
			{
				requestOptions.QueryParams["entities"] = witResponseClass.ToString();
			}
		}

		private static void MergeEntities(WitResponseClass entities, WitDynamicEntity providerEntity)
		{
			if (!entities.HasChild(providerEntity.entity))
			{
				entities[providerEntity.entity] = new WitResponseArray();
			}
			WitResponseNode witResponseNode = entities[providerEntity.entity];
			Dictionary<string, WitResponseClass> dictionary = new Dictionary<string, WitResponseClass>();
			new HashSet<string>();
			WitResponseArray asArray = witResponseNode.AsArray;
			for (int i = 0; i < asArray.Count; i++)
			{
				WitResponseClass asObject = asArray[i].AsObject;
				string value = asObject["keyword"].Value;
				if (!dictionary.ContainsKey(value))
				{
					dictionary[value] = asObject;
				}
			}
			foreach (WitEntityKeywordInfo witEntityKeywordInfo in providerEntity.keywords)
			{
				WitResponseClass asObject2;
				if (dictionary.TryGetValue(witEntityKeywordInfo.keyword, out asObject2))
				{
					using (List<string>.Enumerator enumerator2 = witEntityKeywordInfo.synonyms.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							string s = enumerator2.Current;
							asObject2["synonyms"].Add(s);
						}
						continue;
					}
				}
				asObject2 = JsonConvert.SerializeToken<WitEntityKeywordInfo>(witEntityKeywordInfo, null, false).AsObject;
				dictionary[witEntityKeywordInfo.keyword] = asObject2;
				witResponseNode.Add(asObject2);
			}
		}

		public static WitRequestOptions GetSetupOptions(WitConfiguration configuration, WitRequestOptions newOptions, IDynamicEntitiesProvider[] additionalDynamicEntities)
		{
			WitRequestOptions witRequestOptions = newOptions ?? new WitRequestOptions(Array.Empty<VoiceServiceRequestOptions.QueryParam>());
			if (-1 != witRequestOptions.nBestIntents)
			{
				witRequestOptions.QueryParams["n"] = witRequestOptions.nBestIntents.ToString();
			}
			string versionTag = configuration.GetVersionTag();
			if (!string.IsNullOrEmpty(versionTag))
			{
				witRequestOptions.QueryParams["tag"] = versionTag;
			}
			WitRequestFactory.HandleWitRequestOptions(witRequestOptions, additionalDynamicEntities);
			return witRequestOptions;
		}

		public static VoiceServiceRequest CreateMessageRequest(this WitConfiguration config, WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents, IDynamicEntitiesProvider[] additionalEntityProviders = null)
		{
			WitRequestOptions setupOptions = WitRequestFactory.GetSetupOptions(config, requestOptions, additionalEntityProviders);
			return new WitUnityRequest(config, NLPRequestInputType.Text, setupOptions, requestEvents);
		}

		public static WitRequest CreateSpeechRequest(this WitConfiguration config, WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents, IDynamicEntitiesProvider[] additionalEntityProviders = null)
		{
			WitRequestOptions setupOptions = WitRequestFactory.GetSetupOptions(config, requestOptions, additionalEntityProviders);
			string speech = config.GetEndpointInfo().Speech;
			return new WitRequest(config, speech, setupOptions, requestEvents);
		}

		public static WitRequest CreateDictationRequest(this WitConfiguration config, WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents = null)
		{
			WitRequestOptions setupOptions = WitRequestFactory.GetSetupOptions(config, requestOptions, null);
			string dictation = config.GetEndpointInfo().Dictation;
			return new WitRequest(config, dictation, setupOptions, requestEvents);
		}
	}
}
