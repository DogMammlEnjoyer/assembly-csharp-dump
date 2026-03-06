using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Modio.API.SchemaDefinitions;

namespace Modio.Metrics
{
	public class MetricsSession
	{
		public MetricsSession(string id, long[] mods)
		{
			this.SessionId = id;
			this._ids = mods;
			this.SessionOrderId = 2L;
		}

		private string GetSessionHash(bool includeIds, string sessionTs, string nonce, string secret)
		{
			string text = null;
			if (includeIds)
			{
				text = string.Join<long>(",", this._ids);
			}
			text = text + sessionTs + this.SessionId + nonce;
			byte[] bytes = Encoding.UTF8.GetBytes(secret);
			byte[] bytes2 = Encoding.UTF8.GetBytes(text);
			string result;
			using (HMACSHA256 hmacsha = new HMACSHA256(bytes))
			{
				result = BitConverter.ToString(hmacsha.ComputeHash(bytes2)).Replace("-", "").ToLower();
			}
			return result;
		}

		internal MetricsSessionRequest ToRequest(bool includeIds, string secret)
		{
			string text = Guid.NewGuid().ToString();
			long sessionTs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
			string sessionHash = this.GetSessionHash(includeIds, sessionTs.ToString(), text, secret);
			return new MetricsSessionRequest(this.SessionId, sessionTs, sessionHash, text, this.SessionOrderId, includeIds ? this._ids : null);
		}

		private readonly long[] _ids;

		internal readonly string SessionId;

		internal long SessionOrderId;

		internal bool Active;

		public CancellationTokenSource HeartbeatCancellationToken;

		public TaskCompletionSource<bool> HeartbeatCompletionSource;
	}
}
