using System;
using System.Collections.Generic;
using System.Linq;

namespace Modio.API
{
	public class ModioAPIRequest : IDisposable
	{
		private ModioAPIRequest()
		{
		}

		private string Uri { get; set; }

		public ModioAPIRequestOptions Options { get; } = new ModioAPIRequestOptions();

		public ModioAPIRequestMethod Method { get; private set; }

		public ModioAPIRequestContentType ContentType { get; private set; }

		public string ContentTypeHint { get; private set; } = "";

		internal static ModioAPIRequest New(string uri, ModioAPIRequestMethod method = ModioAPIRequestMethod.Get, ModioAPIRequestContentType contentType = ModioAPIRequestContentType.None, string contentTypeHint = "")
		{
			List<ModioAPIRequest> pool = ModioAPIRequest.Pool;
			ModioAPIRequest modioAPIRequest;
			lock (pool)
			{
				if (ModioAPIRequest.Pool.Count == 0)
				{
					modioAPIRequest = new ModioAPIRequest();
				}
				else
				{
					int index = ModioAPIRequest.Pool.Count - 1;
					modioAPIRequest = ModioAPIRequest.Pool[index];
					ModioAPIRequest.Pool.RemoveAt(index);
				}
			}
			modioAPIRequest.Uri = uri;
			modioAPIRequest.Method = method;
			modioAPIRequest.ContentType = contentType;
			modioAPIRequest.ContentTypeHint = contentTypeHint;
			return modioAPIRequest;
		}

		public string GetUri(List<string> defaultParameters)
		{
			string[] array = new string[defaultParameters.Count + this.Options.QueryParameters.Count];
			if (array.Length == 0)
			{
				return this.Uri;
			}
			(from key in this.Options.QueryParameters
			select key.Key + "=" + key.Value).ToArray<string>().CopyTo(array, 0);
			defaultParameters.CopyTo(array, this.Options.QueryParameters.Count);
			if (array.Length != 0)
			{
				return this.Uri + ((this.Uri.LastIndexOf('?') == -1) ? "?" : "&") + string.Join("&", array);
			}
			return this.Uri;
		}

		public void Dispose()
		{
			this.Options.Dispose();
			this.Method = ModioAPIRequestMethod.Get;
			this.ContentType = ModioAPIRequestContentType.None;
			List<ModioAPIRequest> pool = ModioAPIRequest.Pool;
			lock (pool)
			{
				ModioAPIRequest.Pool.Add(this);
			}
		}

		private static readonly List<ModioAPIRequest> Pool = new List<ModioAPIRequest>();
	}
}
