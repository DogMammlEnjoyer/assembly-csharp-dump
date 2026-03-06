using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct CreateMultipartUploadSessionRequest : IApiRequest
	{
		[JsonConstructor]
		public CreateMultipartUploadSessionRequest(string filename, string nonce)
		{
			this.Filename = filename;
			this.Nonce = nonce;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			CreateMultipartUploadSessionRequest._bodyParameters.Clear();
			CreateMultipartUploadSessionRequest._bodyParameters.Add("filename", this.Filename);
			CreateMultipartUploadSessionRequest._bodyParameters.Add("nonce", this.Nonce);
			return CreateMultipartUploadSessionRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string Filename;

		internal readonly string Nonce;
	}
}
