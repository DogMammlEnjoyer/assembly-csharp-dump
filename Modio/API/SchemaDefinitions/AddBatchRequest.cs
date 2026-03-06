using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
	internal readonly struct AddBatchRequest : IApiRequest
	{
		[JsonConstructor]
		public AddBatchRequest(string batch0RelativeUrl, string batch0Method, string batch1RelativeUrl, string batch1Method, string batch2RelativeUrl, string batch2Method)
		{
			this.Batch0RelativeUrl = batch0RelativeUrl;
			this.Batch0Method = batch0Method;
			this.Batch1RelativeUrl = batch1RelativeUrl;
			this.Batch1Method = batch1Method;
			this.Batch2RelativeUrl = batch2RelativeUrl;
			this.Batch2Method = batch2Method;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			AddBatchRequest._bodyParameters.Clear();
			AddBatchRequest._bodyParameters.Add("batch[0][relative_url]", this.Batch0RelativeUrl);
			AddBatchRequest._bodyParameters.Add("batch[0][method]", this.Batch0Method);
			AddBatchRequest._bodyParameters.Add("batch[1][relative_url]", this.Batch1RelativeUrl);
			AddBatchRequest._bodyParameters.Add("batch[1][method]", this.Batch1Method);
			AddBatchRequest._bodyParameters.Add("batch[2][relative_url]", this.Batch2RelativeUrl);
			AddBatchRequest._bodyParameters.Add("batch[2][method]", this.Batch2Method);
			return AddBatchRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string Batch0RelativeUrl;

		internal readonly string Batch0Method;

		internal readonly string Batch1RelativeUrl;

		internal readonly string Batch1Method;

		internal readonly string Batch2RelativeUrl;

		internal readonly string Batch2Method;
	}
}
