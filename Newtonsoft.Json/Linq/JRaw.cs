using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Newtonsoft.Json.Linq
{
	[NullableContext(1)]
	[Nullable(0)]
	public class JRaw : JValue
	{
		public static Task<JRaw> CreateAsync(JsonReader reader, CancellationToken cancellationToken = default(CancellationToken))
		{
			JRaw.<CreateAsync>d__0 <CreateAsync>d__;
			<CreateAsync>d__.<>t__builder = AsyncTaskMethodBuilder<JRaw>.Create();
			<CreateAsync>d__.reader = reader;
			<CreateAsync>d__.cancellationToken = cancellationToken;
			<CreateAsync>d__.<>1__state = -1;
			<CreateAsync>d__.<>t__builder.Start<JRaw.<CreateAsync>d__0>(ref <CreateAsync>d__);
			return <CreateAsync>d__.<>t__builder.Task;
		}

		public JRaw(JRaw other) : base(other, null)
		{
		}

		internal JRaw(JRaw other, [Nullable(2)] JsonCloneSettings settings) : base(other, settings)
		{
		}

		[NullableContext(2)]
		public JRaw(object rawJson) : base(rawJson, JTokenType.Raw)
		{
		}

		public static JRaw Create(JsonReader reader)
		{
			JRaw result;
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
				{
					jsonTextWriter.WriteToken(reader);
					result = new JRaw(stringWriter.ToString());
				}
			}
			return result;
		}

		internal override JToken CloneToken([Nullable(2)] JsonCloneSettings settings)
		{
			return new JRaw(this, settings);
		}
	}
}
