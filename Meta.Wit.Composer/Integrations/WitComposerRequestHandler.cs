using System;
using System.Collections.Generic;
using System.Text;
using Meta.Voice;
using Meta.WitAi.Composer.Data;
using Meta.WitAi.Composer.Interfaces;
using Meta.WitAi.Json;
using Meta.WitAi.Requests;

namespace Meta.WitAi.Composer.Integrations
{
	public class WitComposerRequestHandler : IComposerRequestHandler
	{
		public WitComposerRequestHandler(IWitRequestConfiguration configuration)
		{
			this._configuration = configuration;
		}

		public void OnComposerRequestSetup(ComposerSessionData sessionData, VoiceServiceRequest request)
		{
			if (request == null || sessionData.composer == null || sessionData.composer.VoiceService == null)
			{
				return;
			}
			request.Options.QueryParams["session_id"] = sessionData.sessionID;
			if (sessionData.composer.VoiceService.UsePlatformIntegrations)
			{
				request.Options.QueryParams["useComposer"] = "True";
			}
			bool flag = false;
			string text = null;
			if (request.InputType == NLPRequestInputType.Text)
			{
				text = request.Options.Text;
				int num = this.IsEventJson(text) ? 1 : 0;
				flag = string.IsNullOrEmpty(text);
				if (num == 0 || flag)
				{
					if (request is WitSocketRequest)
					{
						request.Options.QueryParams["message"] = text;
						request.Options.QueryParams["type"] = WitComposerMessageType.Message.ToString().ToLower();
					}
					else
					{
						Dictionary<string, string> dictionary = new Dictionary<string, string>();
						dictionary["message"] = text;
						dictionary["type"] = WitComposerMessageType.Message.ToString().ToLower();
						text = JsonConvert.SerializeObject<Dictionary<string, string>>(dictionary, null, false);
					}
				}
				request.Options.Text = text;
				if (request.Options.QueryParams.ContainsKey("q"))
				{
					request.Options.QueryParams.Remove("q");
				}
			}
			ComposerContextMap contextMap = sessionData.contextMap;
			if (contextMap != null)
			{
				contextMap.SetData<string>(sessionData.composer.contextMapEventKey, flag.ToString().ToLower());
			}
			bool flag2 = contextMap != null && contextMap.Data["debug"].AsBool;
			request.Options.QueryParams["context_map"] = ((contextMap != null) ? contextMap.GetJson(false) : null);
			request.Options.QueryParams["debug"] = (flag2 ? "true" : "false");
			bool flag3 = contextMap != null && contextMap.Data[WitComposerConstants.PRELOAD].AsBool;
			request.Options.QueryParams[WitComposerConstants.PRELOAD] = (flag3 ? "true" : "false");
			WitRequest witRequest = request as WitRequest;
			if (witRequest != null)
			{
				witRequest.Path = this.GetEndpointPath(request.InputType);
				if (request.InputType == NLPRequestInputType.Text)
				{
					witRequest.postContentType = "application/json";
					witRequest.postData = Encoding.UTF8.GetBytes(text);
					return;
				}
			}
			else
			{
				WitUnityRequest witUnityRequest = request as WitUnityRequest;
				if (witUnityRequest != null)
				{
					witUnityRequest.Endpoint = this.GetEndpointPath(request.InputType);
					witUnityRequest.ShouldPost = true;
					return;
				}
				WitSocketRequest witSocketRequest = request as WitSocketRequest;
				if (witSocketRequest != null)
				{
					witSocketRequest.Endpoint = this.GetEndpointPath(request.InputType);
				}
			}
		}

		public bool IsEventJson(string json)
		{
			if (string.IsNullOrEmpty(json))
			{
				return true;
			}
			WitResponseNode witResponseNode = JsonConvert.DeserializeToken(json);
			if (witResponseNode != null)
			{
				bool flag = false;
				WitResponseClass asObject = witResponseNode.AsObject;
				if (asObject == null || !asObject.HasChild("type"))
				{
					flag = true;
				}
				if (!flag)
				{
					return true;
				}
			}
			return false;
		}

		private string GetEndpointPath(NLPRequestInputType inputType)
		{
			if (inputType == NLPRequestInputType.Audio)
			{
				if (this._configuration != null)
				{
					return this._configuration.GetEndpointInfo().Converse;
				}
				return "converse";
			}
			else
			{
				if (inputType != NLPRequestInputType.Text)
				{
					VLog.E(string.Format("Unsupported input type: {0}", inputType), null);
					return null;
				}
				if (this._configuration != null)
				{
					return this._configuration.GetEndpointInfo().Event;
				}
				return "event";
			}
		}

		private readonly IWitRequestConfiguration _configuration;
	}
}
