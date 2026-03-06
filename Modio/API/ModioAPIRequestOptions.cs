using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Modio.API
{
	public class ModioAPIRequestOptions : IDisposable
	{
		internal Dictionary<string, string> QueryParameters { get; } = new Dictionary<string, string>();

		internal Dictionary<string, string> HeaderParameters { get; } = new Dictionary<string, string>();

		internal bool RequiresAuthentication { get; private set; }

		internal Dictionary<string, string> FormParameters { get; } = new Dictionary<string, string>();

		internal Dictionary<string, ModioAPIFileParameter> FileParameters { get; } = new Dictionary<string, ModioAPIFileParameter>();

		public byte[] BodyDataBytes { get; private set; }

		public void Dispose()
		{
			this.RequiresAuthentication = false;
			this.HeaderParameters.Clear();
			this.QueryParameters.Clear();
			this.FormParameters.Clear();
			this.FileParameters.Clear();
		}

		internal void AddQueryParameter(string key, object value)
		{
			if (value != null)
			{
				this.QueryParameters.Add(key, ModioAPIRequestOptions.ParameterToString(value));
			}
		}

		internal void AddHeaderParameter(string key, object value)
		{
			if (value != null)
			{
				this.HeaderParameters.Add(key, ModioAPIRequestOptions.ParameterToString(value));
			}
		}

		internal void AddFilterParameters(SearchFilter filter)
		{
			if (filter == null)
			{
				return;
			}
			this.AddQueryParameter("_offset", filter.PageIndex * filter.PageSize);
			this.AddQueryParameter("_limit", filter.PageSize);
			foreach (KeyValuePair<string, object> keyValuePair in filter.Parameters)
			{
				this.AddQueryParameter(keyValuePair.Key, keyValuePair.Value);
			}
		}

		public void RequireAuthentication()
		{
			this.RequiresAuthentication = true;
		}

		private static string ParameterToString(object value)
		{
			if (value == null)
			{
				return string.Empty;
			}
			IEnumerable<string> enumerable = value as IEnumerable<string>;
			if (enumerable != null)
			{
				return string.Join(",", enumerable);
			}
			ICollection collection = value as ICollection;
			if (collection != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				int num = 0;
				foreach (object value2 in collection)
				{
					if (num++ > 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(value2);
				}
				return stringBuilder.ToString();
			}
			return string.Format("{0}", value);
		}

		public void AddBody(byte[] data)
		{
			this.BodyDataBytes = data;
		}

		public void AddBody(IApiRequest request)
		{
			foreach (KeyValuePair<string, object> keyValuePair in request.GetBodyParameters())
			{
				object value = keyValuePair.Value;
				if (value is ModioAPIFileParameter)
				{
					ModioAPIFileParameter value2 = (ModioAPIFileParameter)value;
					this.FileParameters.Add(keyValuePair.Key, value2);
				}
				else if (keyValuePair.Value != null)
				{
					this.FormParameters.Add(keyValuePair.Key, ModioAPIRequestOptions.ParameterToString(keyValuePair.Value));
				}
			}
		}

		public void AddBody(IApiRequest request, string hint)
		{
			if (hint == "application/json")
			{
				string s = JsonConvert.SerializeObject(from param in request.GetBodyParameters()
				where param.Value != null
				select param);
				this.AddBody(Encoding.UTF8.GetBytes(s));
				return;
			}
			this.AddBody(request);
		}
	}
}
