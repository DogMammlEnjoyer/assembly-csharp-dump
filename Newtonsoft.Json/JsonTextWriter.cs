using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json
{
	[NullableContext(1)]
	[Nullable(0)]
	public class JsonTextWriter : JsonWriter
	{
		public override Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.FlushAsync(cancellationToken);
			}
			return this.DoFlushAsync(cancellationToken);
		}

		internal Task DoFlushAsync(CancellationToken cancellationToken)
		{
			return cancellationToken.CancelIfRequestedAsync() ?? this._writer.FlushAsync();
		}

		protected override Task WriteValueDelimiterAsync(CancellationToken cancellationToken)
		{
			if (!this._safeAsync)
			{
				return base.WriteValueDelimiterAsync(cancellationToken);
			}
			return this.DoWriteValueDelimiterAsync(cancellationToken);
		}

		internal Task DoWriteValueDelimiterAsync(CancellationToken cancellationToken)
		{
			return this._writer.WriteAsync(',', cancellationToken);
		}

		protected override Task WriteEndAsync(JsonToken token, CancellationToken cancellationToken)
		{
			if (!this._safeAsync)
			{
				return base.WriteEndAsync(token, cancellationToken);
			}
			return this.DoWriteEndAsync(token, cancellationToken);
		}

		internal Task DoWriteEndAsync(JsonToken token, CancellationToken cancellationToken)
		{
			switch (token)
			{
			case JsonToken.EndObject:
				return this._writer.WriteAsync('}', cancellationToken);
			case JsonToken.EndArray:
				return this._writer.WriteAsync(']', cancellationToken);
			case JsonToken.EndConstructor:
				return this._writer.WriteAsync(')', cancellationToken);
			default:
				throw JsonWriterException.Create(this, "Invalid JsonToken: " + token.ToString(), null);
			}
		}

		public override Task CloseAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.CloseAsync(cancellationToken);
			}
			return this.DoCloseAsync(cancellationToken);
		}

		internal Task DoCloseAsync(CancellationToken cancellationToken)
		{
			JsonTextWriter.<DoCloseAsync>d__8 <DoCloseAsync>d__;
			<DoCloseAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DoCloseAsync>d__.<>4__this = this;
			<DoCloseAsync>d__.cancellationToken = cancellationToken;
			<DoCloseAsync>d__.<>1__state = -1;
			<DoCloseAsync>d__.<>t__builder.Start<JsonTextWriter.<DoCloseAsync>d__8>(ref <DoCloseAsync>d__);
			return <DoCloseAsync>d__.<>t__builder.Task;
		}

		private Task CloseBufferAndWriterAsync()
		{
			JsonTextWriter.<CloseBufferAndWriterAsync>d__9 <CloseBufferAndWriterAsync>d__;
			<CloseBufferAndWriterAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<CloseBufferAndWriterAsync>d__.<>4__this = this;
			<CloseBufferAndWriterAsync>d__.<>1__state = -1;
			<CloseBufferAndWriterAsync>d__.<>t__builder.Start<JsonTextWriter.<CloseBufferAndWriterAsync>d__9>(ref <CloseBufferAndWriterAsync>d__);
			return <CloseBufferAndWriterAsync>d__.<>t__builder.Task;
		}

		public override Task WriteEndAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteEndAsync(cancellationToken);
			}
			return base.WriteEndInternalAsync(cancellationToken);
		}

		protected override Task WriteIndentAsync(CancellationToken cancellationToken)
		{
			if (!this._safeAsync)
			{
				return base.WriteIndentAsync(cancellationToken);
			}
			return this.DoWriteIndentAsync(cancellationToken);
		}

		internal Task DoWriteIndentAsync(CancellationToken cancellationToken)
		{
			int num = base.Top * this._indentation;
			int num2 = this.SetIndentChars();
			if (num <= 12)
			{
				return this._writer.WriteAsync(this._indentChars, 0, num2 + num, cancellationToken);
			}
			return this.WriteIndentAsync(num, num2, cancellationToken);
		}

		private Task WriteIndentAsync(int currentIndentCount, int newLineLen, CancellationToken cancellationToken)
		{
			JsonTextWriter.<WriteIndentAsync>d__13 <WriteIndentAsync>d__;
			<WriteIndentAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteIndentAsync>d__.<>4__this = this;
			<WriteIndentAsync>d__.currentIndentCount = currentIndentCount;
			<WriteIndentAsync>d__.newLineLen = newLineLen;
			<WriteIndentAsync>d__.cancellationToken = cancellationToken;
			<WriteIndentAsync>d__.<>1__state = -1;
			<WriteIndentAsync>d__.<>t__builder.Start<JsonTextWriter.<WriteIndentAsync>d__13>(ref <WriteIndentAsync>d__);
			return <WriteIndentAsync>d__.<>t__builder.Task;
		}

		private Task WriteValueInternalAsync(JsonToken token, string value, CancellationToken cancellationToken)
		{
			Task task = base.InternalWriteValueAsync(token, cancellationToken);
			if (task.IsCompletedSuccessfully())
			{
				return this._writer.WriteAsync(value, cancellationToken);
			}
			return this.WriteValueInternalAsync(task, value, cancellationToken);
		}

		private Task WriteValueInternalAsync(Task task, string value, CancellationToken cancellationToken)
		{
			JsonTextWriter.<WriteValueInternalAsync>d__15 <WriteValueInternalAsync>d__;
			<WriteValueInternalAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteValueInternalAsync>d__.<>4__this = this;
			<WriteValueInternalAsync>d__.task = task;
			<WriteValueInternalAsync>d__.value = value;
			<WriteValueInternalAsync>d__.cancellationToken = cancellationToken;
			<WriteValueInternalAsync>d__.<>1__state = -1;
			<WriteValueInternalAsync>d__.<>t__builder.Start<JsonTextWriter.<WriteValueInternalAsync>d__15>(ref <WriteValueInternalAsync>d__);
			return <WriteValueInternalAsync>d__.<>t__builder.Task;
		}

		protected override Task WriteIndentSpaceAsync(CancellationToken cancellationToken)
		{
			if (!this._safeAsync)
			{
				return base.WriteIndentSpaceAsync(cancellationToken);
			}
			return this.DoWriteIndentSpaceAsync(cancellationToken);
		}

		internal Task DoWriteIndentSpaceAsync(CancellationToken cancellationToken)
		{
			return this._writer.WriteAsync(' ', cancellationToken);
		}

		public override Task WriteRawAsync([Nullable(2)] string json, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteRawAsync(json, cancellationToken);
			}
			return this.DoWriteRawAsync(json, cancellationToken);
		}

		internal Task DoWriteRawAsync([Nullable(2)] string json, CancellationToken cancellationToken)
		{
			return this._writer.WriteAsync(json, cancellationToken);
		}

		public override Task WriteNullAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteNullAsync(cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		internal Task DoWriteNullAsync(CancellationToken cancellationToken)
		{
			return this.WriteValueInternalAsync(JsonToken.Null, JsonConvert.Null, cancellationToken);
		}

		private Task WriteDigitsAsync(ulong uvalue, bool negative, CancellationToken cancellationToken)
		{
			if (uvalue <= 9UL & !negative)
			{
				return this._writer.WriteAsync((char)(48UL + uvalue), cancellationToken);
			}
			int count = this.WriteNumberToBuffer(uvalue, negative);
			return this._writer.WriteAsync(this._writeBuffer, 0, count, cancellationToken);
		}

		private Task WriteIntegerValueAsync(ulong uvalue, bool negative, CancellationToken cancellationToken)
		{
			Task task = base.InternalWriteValueAsync(JsonToken.Integer, cancellationToken);
			if (task.IsCompletedSuccessfully())
			{
				return this.WriteDigitsAsync(uvalue, negative, cancellationToken);
			}
			return this.WriteIntegerValueAsync(task, uvalue, negative, cancellationToken);
		}

		private Task WriteIntegerValueAsync(Task task, ulong uvalue, bool negative, CancellationToken cancellationToken)
		{
			JsonTextWriter.<WriteIntegerValueAsync>d__24 <WriteIntegerValueAsync>d__;
			<WriteIntegerValueAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteIntegerValueAsync>d__.<>4__this = this;
			<WriteIntegerValueAsync>d__.task = task;
			<WriteIntegerValueAsync>d__.uvalue = uvalue;
			<WriteIntegerValueAsync>d__.negative = negative;
			<WriteIntegerValueAsync>d__.cancellationToken = cancellationToken;
			<WriteIntegerValueAsync>d__.<>1__state = -1;
			<WriteIntegerValueAsync>d__.<>t__builder.Start<JsonTextWriter.<WriteIntegerValueAsync>d__24>(ref <WriteIntegerValueAsync>d__);
			return <WriteIntegerValueAsync>d__.<>t__builder.Task;
		}

		internal Task WriteIntegerValueAsync(long value, CancellationToken cancellationToken)
		{
			bool flag = value < 0L;
			if (flag)
			{
				value = -value;
			}
			return this.WriteIntegerValueAsync((ulong)value, flag, cancellationToken);
		}

		internal Task WriteIntegerValueAsync(ulong uvalue, CancellationToken cancellationToken)
		{
			return this.WriteIntegerValueAsync(uvalue, false, cancellationToken);
		}

		private Task WriteEscapedStringAsync(string value, bool quote, CancellationToken cancellationToken)
		{
			return JavaScriptUtils.WriteEscapedJavaScriptStringAsync(this._writer, value, this._quoteChar, quote, this._charEscapeFlags, base.StringEscapeHandling, this, this._writeBuffer, cancellationToken);
		}

		public override Task WritePropertyNameAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WritePropertyNameAsync(name, cancellationToken);
			}
			return this.DoWritePropertyNameAsync(name, cancellationToken);
		}

		internal Task DoWritePropertyNameAsync(string name, CancellationToken cancellationToken)
		{
			Task task = base.InternalWritePropertyNameAsync(name, cancellationToken);
			if (!task.IsCompletedSuccessfully())
			{
				return this.DoWritePropertyNameAsync(task, name, cancellationToken);
			}
			task = this.WriteEscapedStringAsync(name, this._quoteName, cancellationToken);
			if (task.IsCompletedSuccessfully())
			{
				return this._writer.WriteAsync(':', cancellationToken);
			}
			return JavaScriptUtils.WriteCharAsync(task, this._writer, ':', cancellationToken);
		}

		private Task DoWritePropertyNameAsync(Task task, string name, CancellationToken cancellationToken)
		{
			JsonTextWriter.<DoWritePropertyNameAsync>d__30 <DoWritePropertyNameAsync>d__;
			<DoWritePropertyNameAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DoWritePropertyNameAsync>d__.<>4__this = this;
			<DoWritePropertyNameAsync>d__.task = task;
			<DoWritePropertyNameAsync>d__.name = name;
			<DoWritePropertyNameAsync>d__.cancellationToken = cancellationToken;
			<DoWritePropertyNameAsync>d__.<>1__state = -1;
			<DoWritePropertyNameAsync>d__.<>t__builder.Start<JsonTextWriter.<DoWritePropertyNameAsync>d__30>(ref <DoWritePropertyNameAsync>d__);
			return <DoWritePropertyNameAsync>d__.<>t__builder.Task;
		}

		public override Task WritePropertyNameAsync(string name, bool escape, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WritePropertyNameAsync(name, escape, cancellationToken);
			}
			return this.DoWritePropertyNameAsync(name, escape, cancellationToken);
		}

		internal Task DoWritePropertyNameAsync(string name, bool escape, CancellationToken cancellationToken)
		{
			JsonTextWriter.<DoWritePropertyNameAsync>d__32 <DoWritePropertyNameAsync>d__;
			<DoWritePropertyNameAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DoWritePropertyNameAsync>d__.<>4__this = this;
			<DoWritePropertyNameAsync>d__.name = name;
			<DoWritePropertyNameAsync>d__.escape = escape;
			<DoWritePropertyNameAsync>d__.cancellationToken = cancellationToken;
			<DoWritePropertyNameAsync>d__.<>1__state = -1;
			<DoWritePropertyNameAsync>d__.<>t__builder.Start<JsonTextWriter.<DoWritePropertyNameAsync>d__32>(ref <DoWritePropertyNameAsync>d__);
			return <DoWritePropertyNameAsync>d__.<>t__builder.Task;
		}

		public override Task WriteStartArrayAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteStartArrayAsync(cancellationToken);
			}
			return this.DoWriteStartArrayAsync(cancellationToken);
		}

		internal Task DoWriteStartArrayAsync(CancellationToken cancellationToken)
		{
			Task task = base.InternalWriteStartAsync(JsonToken.StartArray, JsonContainerType.Array, cancellationToken);
			if (task.IsCompletedSuccessfully())
			{
				return this._writer.WriteAsync('[', cancellationToken);
			}
			return this.DoWriteStartArrayAsync(task, cancellationToken);
		}

		internal Task DoWriteStartArrayAsync(Task task, CancellationToken cancellationToken)
		{
			JsonTextWriter.<DoWriteStartArrayAsync>d__35 <DoWriteStartArrayAsync>d__;
			<DoWriteStartArrayAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DoWriteStartArrayAsync>d__.<>4__this = this;
			<DoWriteStartArrayAsync>d__.task = task;
			<DoWriteStartArrayAsync>d__.cancellationToken = cancellationToken;
			<DoWriteStartArrayAsync>d__.<>1__state = -1;
			<DoWriteStartArrayAsync>d__.<>t__builder.Start<JsonTextWriter.<DoWriteStartArrayAsync>d__35>(ref <DoWriteStartArrayAsync>d__);
			return <DoWriteStartArrayAsync>d__.<>t__builder.Task;
		}

		public override Task WriteStartObjectAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteStartObjectAsync(cancellationToken);
			}
			return this.DoWriteStartObjectAsync(cancellationToken);
		}

		internal Task DoWriteStartObjectAsync(CancellationToken cancellationToken)
		{
			Task task = base.InternalWriteStartAsync(JsonToken.StartObject, JsonContainerType.Object, cancellationToken);
			if (task.IsCompletedSuccessfully())
			{
				return this._writer.WriteAsync('{', cancellationToken);
			}
			return this.DoWriteStartObjectAsync(task, cancellationToken);
		}

		internal Task DoWriteStartObjectAsync(Task task, CancellationToken cancellationToken)
		{
			JsonTextWriter.<DoWriteStartObjectAsync>d__38 <DoWriteStartObjectAsync>d__;
			<DoWriteStartObjectAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DoWriteStartObjectAsync>d__.<>4__this = this;
			<DoWriteStartObjectAsync>d__.task = task;
			<DoWriteStartObjectAsync>d__.cancellationToken = cancellationToken;
			<DoWriteStartObjectAsync>d__.<>1__state = -1;
			<DoWriteStartObjectAsync>d__.<>t__builder.Start<JsonTextWriter.<DoWriteStartObjectAsync>d__38>(ref <DoWriteStartObjectAsync>d__);
			return <DoWriteStartObjectAsync>d__.<>t__builder.Task;
		}

		public override Task WriteStartConstructorAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteStartConstructorAsync(name, cancellationToken);
			}
			return this.DoWriteStartConstructorAsync(name, cancellationToken);
		}

		internal Task DoWriteStartConstructorAsync(string name, CancellationToken cancellationToken)
		{
			JsonTextWriter.<DoWriteStartConstructorAsync>d__40 <DoWriteStartConstructorAsync>d__;
			<DoWriteStartConstructorAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DoWriteStartConstructorAsync>d__.<>4__this = this;
			<DoWriteStartConstructorAsync>d__.name = name;
			<DoWriteStartConstructorAsync>d__.cancellationToken = cancellationToken;
			<DoWriteStartConstructorAsync>d__.<>1__state = -1;
			<DoWriteStartConstructorAsync>d__.<>t__builder.Start<JsonTextWriter.<DoWriteStartConstructorAsync>d__40>(ref <DoWriteStartConstructorAsync>d__);
			return <DoWriteStartConstructorAsync>d__.<>t__builder.Task;
		}

		public override Task WriteUndefinedAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteUndefinedAsync(cancellationToken);
			}
			return this.DoWriteUndefinedAsync(cancellationToken);
		}

		internal Task DoWriteUndefinedAsync(CancellationToken cancellationToken)
		{
			Task task = base.InternalWriteValueAsync(JsonToken.Undefined, cancellationToken);
			if (task.IsCompletedSuccessfully())
			{
				return this._writer.WriteAsync(JsonConvert.Undefined, cancellationToken);
			}
			return this.DoWriteUndefinedAsync(task, cancellationToken);
		}

		private Task DoWriteUndefinedAsync(Task task, CancellationToken cancellationToken)
		{
			JsonTextWriter.<DoWriteUndefinedAsync>d__43 <DoWriteUndefinedAsync>d__;
			<DoWriteUndefinedAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DoWriteUndefinedAsync>d__.<>4__this = this;
			<DoWriteUndefinedAsync>d__.task = task;
			<DoWriteUndefinedAsync>d__.cancellationToken = cancellationToken;
			<DoWriteUndefinedAsync>d__.<>1__state = -1;
			<DoWriteUndefinedAsync>d__.<>t__builder.Start<JsonTextWriter.<DoWriteUndefinedAsync>d__43>(ref <DoWriteUndefinedAsync>d__);
			return <DoWriteUndefinedAsync>d__.<>t__builder.Task;
		}

		public override Task WriteWhitespaceAsync(string ws, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteWhitespaceAsync(ws, cancellationToken);
			}
			return this.DoWriteWhitespaceAsync(ws, cancellationToken);
		}

		internal Task DoWriteWhitespaceAsync(string ws, CancellationToken cancellationToken)
		{
			base.InternalWriteWhitespace(ws);
			return this._writer.WriteAsync(ws, cancellationToken);
		}

		public override Task WriteValueAsync(bool value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		internal Task DoWriteValueAsync(bool value, CancellationToken cancellationToken)
		{
			return this.WriteValueInternalAsync(JsonToken.Boolean, JsonConvert.ToString(value), cancellationToken);
		}

		public override Task WriteValueAsync(bool? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		internal Task DoWriteValueAsync(bool? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.DoWriteValueAsync(value.GetValueOrDefault(), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		public override Task WriteValueAsync(byte value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.WriteIntegerValueAsync((long)((ulong)value), cancellationToken);
		}

		public override Task WriteValueAsync(byte? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		internal Task DoWriteValueAsync(byte? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.WriteIntegerValueAsync((long)((ulong)value.GetValueOrDefault()), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		public override Task WriteValueAsync([Nullable(2)] byte[] value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			if (value != null)
			{
				return this.WriteValueNonNullAsync(value, cancellationToken);
			}
			return this.WriteNullAsync(cancellationToken);
		}

		internal Task WriteValueNonNullAsync(byte[] value, CancellationToken cancellationToken)
		{
			JsonTextWriter.<WriteValueNonNullAsync>d__54 <WriteValueNonNullAsync>d__;
			<WriteValueNonNullAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteValueNonNullAsync>d__.<>4__this = this;
			<WriteValueNonNullAsync>d__.value = value;
			<WriteValueNonNullAsync>d__.cancellationToken = cancellationToken;
			<WriteValueNonNullAsync>d__.<>1__state = -1;
			<WriteValueNonNullAsync>d__.<>t__builder.Start<JsonTextWriter.<WriteValueNonNullAsync>d__54>(ref <WriteValueNonNullAsync>d__);
			return <WriteValueNonNullAsync>d__.<>t__builder.Task;
		}

		public override Task WriteValueAsync(char value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		internal Task DoWriteValueAsync(char value, CancellationToken cancellationToken)
		{
			return this.WriteValueInternalAsync(JsonToken.String, JsonConvert.ToString(value), cancellationToken);
		}

		public override Task WriteValueAsync(char? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		internal Task DoWriteValueAsync(char? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.DoWriteValueAsync(value.GetValueOrDefault(), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		public override Task WriteValueAsync(DateTime value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		internal Task DoWriteValueAsync(DateTime value, CancellationToken cancellationToken)
		{
			JsonTextWriter.<DoWriteValueAsync>d__60 <DoWriteValueAsync>d__;
			<DoWriteValueAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DoWriteValueAsync>d__.<>4__this = this;
			<DoWriteValueAsync>d__.value = value;
			<DoWriteValueAsync>d__.cancellationToken = cancellationToken;
			<DoWriteValueAsync>d__.<>1__state = -1;
			<DoWriteValueAsync>d__.<>t__builder.Start<JsonTextWriter.<DoWriteValueAsync>d__60>(ref <DoWriteValueAsync>d__);
			return <DoWriteValueAsync>d__.<>t__builder.Task;
		}

		public override Task WriteValueAsync(DateTime? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		internal Task DoWriteValueAsync(DateTime? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.DoWriteValueAsync(value.GetValueOrDefault(), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		public override Task WriteValueAsync(DateTimeOffset value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		internal Task DoWriteValueAsync(DateTimeOffset value, CancellationToken cancellationToken)
		{
			JsonTextWriter.<DoWriteValueAsync>d__64 <DoWriteValueAsync>d__;
			<DoWriteValueAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DoWriteValueAsync>d__.<>4__this = this;
			<DoWriteValueAsync>d__.value = value;
			<DoWriteValueAsync>d__.cancellationToken = cancellationToken;
			<DoWriteValueAsync>d__.<>1__state = -1;
			<DoWriteValueAsync>d__.<>t__builder.Start<JsonTextWriter.<DoWriteValueAsync>d__64>(ref <DoWriteValueAsync>d__);
			return <DoWriteValueAsync>d__.<>t__builder.Task;
		}

		public override Task WriteValueAsync(DateTimeOffset? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		internal Task DoWriteValueAsync(DateTimeOffset? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.DoWriteValueAsync(value.GetValueOrDefault(), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		public override Task WriteValueAsync(decimal value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		internal Task DoWriteValueAsync(decimal value, CancellationToken cancellationToken)
		{
			return this.WriteValueInternalAsync(JsonToken.Float, JsonConvert.ToString(value), cancellationToken);
		}

		public override Task WriteValueAsync(decimal? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		internal Task DoWriteValueAsync(decimal? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.DoWriteValueAsync(value.GetValueOrDefault(), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		public override Task WriteValueAsync(double value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.WriteValueAsync(value, false, cancellationToken);
		}

		internal Task WriteValueAsync(double value, bool nullable, CancellationToken cancellationToken)
		{
			return this.WriteValueInternalAsync(JsonToken.Float, JsonConvert.ToString(value, base.FloatFormatHandling, this.QuoteChar, nullable), cancellationToken);
		}

		public override Task WriteValueAsync(double? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			if (value == null)
			{
				return this.WriteNullAsync(cancellationToken);
			}
			return this.WriteValueAsync(value.GetValueOrDefault(), true, cancellationToken);
		}

		public override Task WriteValueAsync(float value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.WriteValueAsync(value, false, cancellationToken);
		}

		internal Task WriteValueAsync(float value, bool nullable, CancellationToken cancellationToken)
		{
			return this.WriteValueInternalAsync(JsonToken.Float, JsonConvert.ToString(value, base.FloatFormatHandling, this.QuoteChar, nullable), cancellationToken);
		}

		public override Task WriteValueAsync(float? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			if (value == null)
			{
				return this.WriteNullAsync(cancellationToken);
			}
			return this.WriteValueAsync(value.GetValueOrDefault(), true, cancellationToken);
		}

		public override Task WriteValueAsync(Guid value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		internal Task DoWriteValueAsync(Guid value, CancellationToken cancellationToken)
		{
			JsonTextWriter.<DoWriteValueAsync>d__78 <DoWriteValueAsync>d__;
			<DoWriteValueAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DoWriteValueAsync>d__.<>4__this = this;
			<DoWriteValueAsync>d__.value = value;
			<DoWriteValueAsync>d__.cancellationToken = cancellationToken;
			<DoWriteValueAsync>d__.<>1__state = -1;
			<DoWriteValueAsync>d__.<>t__builder.Start<JsonTextWriter.<DoWriteValueAsync>d__78>(ref <DoWriteValueAsync>d__);
			return <DoWriteValueAsync>d__.<>t__builder.Task;
		}

		public override Task WriteValueAsync(Guid? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		internal Task DoWriteValueAsync(Guid? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.DoWriteValueAsync(value.GetValueOrDefault(), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		public override Task WriteValueAsync(int value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.WriteIntegerValueAsync((long)value, cancellationToken);
		}

		public override Task WriteValueAsync(int? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		internal Task DoWriteValueAsync(int? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.WriteIntegerValueAsync((long)value.GetValueOrDefault(), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		public override Task WriteValueAsync(long value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.WriteIntegerValueAsync(value, cancellationToken);
		}

		public override Task WriteValueAsync(long? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		internal Task DoWriteValueAsync(long? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.WriteIntegerValueAsync(value.GetValueOrDefault(), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		internal Task WriteValueAsync(BigInteger value, CancellationToken cancellationToken)
		{
			return this.WriteValueInternalAsync(JsonToken.Integer, value.ToString(CultureInfo.InvariantCulture), cancellationToken);
		}

		public override Task WriteValueAsync([Nullable(2)] object value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			if (value == null)
			{
				return this.WriteNullAsync(cancellationToken);
			}
			if (value is BigInteger)
			{
				BigInteger value2 = (BigInteger)value;
				return this.WriteValueAsync(value2, cancellationToken);
			}
			return JsonWriter.WriteValueAsync(this, ConvertUtils.GetTypeCode(value.GetType()), value, cancellationToken);
		}

		[CLSCompliant(false)]
		public override Task WriteValueAsync(sbyte value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.WriteIntegerValueAsync((long)value, cancellationToken);
		}

		[CLSCompliant(false)]
		public override Task WriteValueAsync(sbyte? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		internal Task DoWriteValueAsync(sbyte? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.WriteIntegerValueAsync((long)value.GetValueOrDefault(), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		public override Task WriteValueAsync(short value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.WriteIntegerValueAsync((long)value, cancellationToken);
		}

		public override Task WriteValueAsync(short? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		internal Task DoWriteValueAsync(short? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.WriteIntegerValueAsync((long)value.GetValueOrDefault(), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		public override Task WriteValueAsync([Nullable(2)] string value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		internal Task DoWriteValueAsync([Nullable(2)] string value, CancellationToken cancellationToken)
		{
			Task task = base.InternalWriteValueAsync(JsonToken.String, cancellationToken);
			if (!task.IsCompletedSuccessfully())
			{
				return this.DoWriteValueAsync(task, value, cancellationToken);
			}
			if (value != null)
			{
				return this.WriteEscapedStringAsync(value, true, cancellationToken);
			}
			return this._writer.WriteAsync(JsonConvert.Null, cancellationToken);
		}

		private Task DoWriteValueAsync(Task task, [Nullable(2)] string value, CancellationToken cancellationToken)
		{
			JsonTextWriter.<DoWriteValueAsync>d__97 <DoWriteValueAsync>d__;
			<DoWriteValueAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DoWriteValueAsync>d__.<>4__this = this;
			<DoWriteValueAsync>d__.task = task;
			<DoWriteValueAsync>d__.value = value;
			<DoWriteValueAsync>d__.cancellationToken = cancellationToken;
			<DoWriteValueAsync>d__.<>1__state = -1;
			<DoWriteValueAsync>d__.<>t__builder.Start<JsonTextWriter.<DoWriteValueAsync>d__97>(ref <DoWriteValueAsync>d__);
			return <DoWriteValueAsync>d__.<>t__builder.Task;
		}

		public override Task WriteValueAsync(TimeSpan value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		internal Task DoWriteValueAsync(TimeSpan value, CancellationToken cancellationToken)
		{
			JsonTextWriter.<DoWriteValueAsync>d__99 <DoWriteValueAsync>d__;
			<DoWriteValueAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DoWriteValueAsync>d__.<>4__this = this;
			<DoWriteValueAsync>d__.value = value;
			<DoWriteValueAsync>d__.cancellationToken = cancellationToken;
			<DoWriteValueAsync>d__.<>1__state = -1;
			<DoWriteValueAsync>d__.<>t__builder.Start<JsonTextWriter.<DoWriteValueAsync>d__99>(ref <DoWriteValueAsync>d__);
			return <DoWriteValueAsync>d__.<>t__builder.Task;
		}

		public override Task WriteValueAsync(TimeSpan? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		internal Task DoWriteValueAsync(TimeSpan? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.DoWriteValueAsync(value.GetValueOrDefault(), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		[CLSCompliant(false)]
		public override Task WriteValueAsync(uint value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.WriteIntegerValueAsync((long)((ulong)value), cancellationToken);
		}

		[CLSCompliant(false)]
		public override Task WriteValueAsync(uint? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		internal Task DoWriteValueAsync(uint? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.WriteIntegerValueAsync((long)((ulong)value.GetValueOrDefault()), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		[CLSCompliant(false)]
		public override Task WriteValueAsync(ulong value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.WriteIntegerValueAsync(value, cancellationToken);
		}

		[CLSCompliant(false)]
		public override Task WriteValueAsync(ulong? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		internal Task DoWriteValueAsync(ulong? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.WriteIntegerValueAsync(value.GetValueOrDefault(), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		public override Task WriteValueAsync([Nullable(2)] Uri value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			if (!(value == null))
			{
				return this.WriteValueNotNullAsync(value, cancellationToken);
			}
			return this.WriteNullAsync(cancellationToken);
		}

		internal Task WriteValueNotNullAsync(Uri value, CancellationToken cancellationToken)
		{
			Task task = base.InternalWriteValueAsync(JsonToken.String, cancellationToken);
			if (task.IsCompletedSuccessfully())
			{
				return this.WriteEscapedStringAsync(value.OriginalString, true, cancellationToken);
			}
			return this.WriteValueNotNullAsync(task, value, cancellationToken);
		}

		internal Task WriteValueNotNullAsync(Task task, Uri value, CancellationToken cancellationToken)
		{
			JsonTextWriter.<WriteValueNotNullAsync>d__110 <WriteValueNotNullAsync>d__;
			<WriteValueNotNullAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteValueNotNullAsync>d__.<>4__this = this;
			<WriteValueNotNullAsync>d__.task = task;
			<WriteValueNotNullAsync>d__.value = value;
			<WriteValueNotNullAsync>d__.cancellationToken = cancellationToken;
			<WriteValueNotNullAsync>d__.<>1__state = -1;
			<WriteValueNotNullAsync>d__.<>t__builder.Start<JsonTextWriter.<WriteValueNotNullAsync>d__110>(ref <WriteValueNotNullAsync>d__);
			return <WriteValueNotNullAsync>d__.<>t__builder.Task;
		}

		[CLSCompliant(false)]
		public override Task WriteValueAsync(ushort value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.WriteIntegerValueAsync((long)((ulong)value), cancellationToken);
		}

		[CLSCompliant(false)]
		public override Task WriteValueAsync(ushort? value, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteValueAsync(value, cancellationToken);
			}
			return this.DoWriteValueAsync(value, cancellationToken);
		}

		internal Task DoWriteValueAsync(ushort? value, CancellationToken cancellationToken)
		{
			if (value != null)
			{
				return this.WriteIntegerValueAsync((long)((ulong)value.GetValueOrDefault()), cancellationToken);
			}
			return this.DoWriteNullAsync(cancellationToken);
		}

		public override Task WriteCommentAsync([Nullable(2)] string text, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteCommentAsync(text, cancellationToken);
			}
			return this.DoWriteCommentAsync(text, cancellationToken);
		}

		internal Task DoWriteCommentAsync([Nullable(2)] string text, CancellationToken cancellationToken)
		{
			JsonTextWriter.<DoWriteCommentAsync>d__115 <DoWriteCommentAsync>d__;
			<DoWriteCommentAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DoWriteCommentAsync>d__.<>4__this = this;
			<DoWriteCommentAsync>d__.text = text;
			<DoWriteCommentAsync>d__.cancellationToken = cancellationToken;
			<DoWriteCommentAsync>d__.<>1__state = -1;
			<DoWriteCommentAsync>d__.<>t__builder.Start<JsonTextWriter.<DoWriteCommentAsync>d__115>(ref <DoWriteCommentAsync>d__);
			return <DoWriteCommentAsync>d__.<>t__builder.Task;
		}

		public override Task WriteEndArrayAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteEndArrayAsync(cancellationToken);
			}
			return base.InternalWriteEndAsync(JsonContainerType.Array, cancellationToken);
		}

		public override Task WriteEndConstructorAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteEndConstructorAsync(cancellationToken);
			}
			return base.InternalWriteEndAsync(JsonContainerType.Constructor, cancellationToken);
		}

		public override Task WriteEndObjectAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteEndObjectAsync(cancellationToken);
			}
			return base.InternalWriteEndAsync(JsonContainerType.Object, cancellationToken);
		}

		public override Task WriteRawValueAsync([Nullable(2)] string json, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.WriteRawValueAsync(json, cancellationToken);
			}
			return this.DoWriteRawValueAsync(json, cancellationToken);
		}

		internal Task DoWriteRawValueAsync([Nullable(2)] string json, CancellationToken cancellationToken)
		{
			base.UpdateScopeWithFinishedValue();
			Task task = base.AutoCompleteAsync(JsonToken.Undefined, cancellationToken);
			if (task.IsCompletedSuccessfully())
			{
				return this.WriteRawAsync(json, cancellationToken);
			}
			return this.DoWriteRawValueAsync(task, json, cancellationToken);
		}

		private Task DoWriteRawValueAsync(Task task, [Nullable(2)] string json, CancellationToken cancellationToken)
		{
			JsonTextWriter.<DoWriteRawValueAsync>d__121 <DoWriteRawValueAsync>d__;
			<DoWriteRawValueAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DoWriteRawValueAsync>d__.<>4__this = this;
			<DoWriteRawValueAsync>d__.task = task;
			<DoWriteRawValueAsync>d__.json = json;
			<DoWriteRawValueAsync>d__.cancellationToken = cancellationToken;
			<DoWriteRawValueAsync>d__.<>1__state = -1;
			<DoWriteRawValueAsync>d__.<>t__builder.Start<JsonTextWriter.<DoWriteRawValueAsync>d__121>(ref <DoWriteRawValueAsync>d__);
			return <DoWriteRawValueAsync>d__.<>t__builder.Task;
		}

		internal char[] EnsureWriteBuffer(int length, int copyTo)
		{
			if (length < 35)
			{
				length = 35;
			}
			char[] writeBuffer = this._writeBuffer;
			if (writeBuffer == null)
			{
				return this._writeBuffer = BufferUtils.RentBuffer(this._arrayPool, length);
			}
			if (writeBuffer.Length >= length)
			{
				return writeBuffer;
			}
			char[] array = BufferUtils.RentBuffer(this._arrayPool, length);
			if (copyTo != 0)
			{
				Array.Copy(writeBuffer, array, copyTo);
			}
			BufferUtils.ReturnBuffer(this._arrayPool, writeBuffer);
			this._writeBuffer = array;
			return array;
		}

		private Base64Encoder Base64Encoder
		{
			get
			{
				if (this._base64Encoder == null)
				{
					this._base64Encoder = new Base64Encoder(this._writer);
				}
				return this._base64Encoder;
			}
		}

		[Nullable(2)]
		public IArrayPool<char> ArrayPool
		{
			[NullableContext(2)]
			get
			{
				return this._arrayPool;
			}
			[NullableContext(2)]
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this._arrayPool = value;
			}
		}

		public int Indentation
		{
			get
			{
				return this._indentation;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("Indentation value must be greater than 0.");
				}
				this._indentation = value;
			}
		}

		public char QuoteChar
		{
			get
			{
				return this._quoteChar;
			}
			set
			{
				if (value != '"' && value != '\'')
				{
					throw new ArgumentException("Invalid JavaScript string quote character. Valid quote characters are ' and \".");
				}
				this._quoteChar = value;
				this.UpdateCharEscapeFlags();
			}
		}

		public char IndentChar
		{
			get
			{
				return this._indentChar;
			}
			set
			{
				if (value != this._indentChar)
				{
					this._indentChar = value;
					this._indentChars = null;
				}
			}
		}

		public bool QuoteName
		{
			get
			{
				return this._quoteName;
			}
			set
			{
				this._quoteName = value;
			}
		}

		public JsonTextWriter(TextWriter textWriter)
		{
			if (textWriter == null)
			{
				throw new ArgumentNullException("textWriter");
			}
			this._writer = textWriter;
			this._quoteChar = '"';
			this._quoteName = true;
			this._indentChar = ' ';
			this._indentation = 2;
			this.UpdateCharEscapeFlags();
			this._safeAsync = (base.GetType() == typeof(JsonTextWriter));
		}

		public override void Flush()
		{
			this._writer.Flush();
		}

		public override void Close()
		{
			base.Close();
			this.CloseBufferAndWriter();
		}

		private void CloseBufferAndWriter()
		{
			if (this._writeBuffer != null)
			{
				BufferUtils.ReturnBuffer(this._arrayPool, this._writeBuffer);
				this._writeBuffer = null;
			}
			if (base.CloseOutput)
			{
				TextWriter writer = this._writer;
				if (writer == null)
				{
					return;
				}
				writer.Close();
			}
		}

		public override void WriteStartObject()
		{
			base.InternalWriteStart(JsonToken.StartObject, JsonContainerType.Object);
			this._writer.Write('{');
		}

		public override void WriteStartArray()
		{
			base.InternalWriteStart(JsonToken.StartArray, JsonContainerType.Array);
			this._writer.Write('[');
		}

		public override void WriteStartConstructor(string name)
		{
			base.InternalWriteStart(JsonToken.StartConstructor, JsonContainerType.Constructor);
			this._writer.Write("new ");
			this._writer.Write(name);
			this._writer.Write('(');
		}

		protected override void WriteEnd(JsonToken token)
		{
			switch (token)
			{
			case JsonToken.EndObject:
				this._writer.Write('}');
				return;
			case JsonToken.EndArray:
				this._writer.Write(']');
				return;
			case JsonToken.EndConstructor:
				this._writer.Write(')');
				return;
			default:
				throw JsonWriterException.Create(this, "Invalid JsonToken: " + token.ToString(), null);
			}
		}

		public override void WritePropertyName(string name)
		{
			base.InternalWritePropertyName(name);
			this.WriteEscapedString(name, this._quoteName);
			this._writer.Write(':');
		}

		public override void WritePropertyName(string name, bool escape)
		{
			base.InternalWritePropertyName(name);
			if (escape)
			{
				this.WriteEscapedString(name, this._quoteName);
			}
			else
			{
				if (this._quoteName)
				{
					this._writer.Write(this._quoteChar);
				}
				this._writer.Write(name);
				if (this._quoteName)
				{
					this._writer.Write(this._quoteChar);
				}
			}
			this._writer.Write(':');
		}

		internal override void OnStringEscapeHandlingChanged()
		{
			this.UpdateCharEscapeFlags();
		}

		private void UpdateCharEscapeFlags()
		{
			this._charEscapeFlags = JavaScriptUtils.GetCharEscapeFlags(base.StringEscapeHandling, this._quoteChar);
		}

		protected override void WriteIndent()
		{
			int num = base.Top * this._indentation;
			int num2 = this.SetIndentChars();
			this._writer.Write(this._indentChars, 0, num2 + Math.Min(num, 12));
			while ((num -= 12) > 0)
			{
				this._writer.Write(this._indentChars, num2, Math.Min(num, 12));
			}
		}

		private int SetIndentChars()
		{
			string newLine = this._writer.NewLine;
			int length = newLine.Length;
			bool flag = this._indentChars != null && this._indentChars.Length == 12 + length;
			if (flag)
			{
				for (int num = 0; num != length; num++)
				{
					if (newLine[num] != this._indentChars[num])
					{
						flag = false;
						break;
					}
				}
			}
			if (!flag)
			{
				this._indentChars = (newLine + new string(this._indentChar, 12)).ToCharArray();
			}
			return length;
		}

		protected override void WriteValueDelimiter()
		{
			this._writer.Write(',');
		}

		protected override void WriteIndentSpace()
		{
			this._writer.Write(' ');
		}

		private void WriteValueInternal(string value, JsonToken token)
		{
			this._writer.Write(value);
		}

		[NullableContext(2)]
		public override void WriteValue(object value)
		{
			if (value is BigInteger)
			{
				BigInteger bigInteger = (BigInteger)value;
				base.InternalWriteValue(JsonToken.Integer);
				this.WriteValueInternal(bigInteger.ToString(CultureInfo.InvariantCulture), JsonToken.String);
				return;
			}
			base.WriteValue(value);
		}

		public override void WriteNull()
		{
			base.InternalWriteValue(JsonToken.Null);
			this.WriteValueInternal(JsonConvert.Null, JsonToken.Null);
		}

		public override void WriteUndefined()
		{
			base.InternalWriteValue(JsonToken.Undefined);
			this.WriteValueInternal(JsonConvert.Undefined, JsonToken.Undefined);
		}

		[NullableContext(2)]
		public override void WriteRaw(string json)
		{
			base.InternalWriteRaw();
			this._writer.Write(json);
		}

		[NullableContext(2)]
		public override void WriteValue(string value)
		{
			base.InternalWriteValue(JsonToken.String);
			if (value == null)
			{
				this.WriteValueInternal(JsonConvert.Null, JsonToken.Null);
				return;
			}
			this.WriteEscapedString(value, true);
		}

		private void WriteEscapedString(string value, bool quote)
		{
			this.EnsureWriteBuffer();
			JavaScriptUtils.WriteEscapedJavaScriptString(this._writer, value, this._quoteChar, quote, this._charEscapeFlags, base.StringEscapeHandling, this._arrayPool, ref this._writeBuffer);
		}

		public override void WriteValue(int value)
		{
			base.InternalWriteValue(JsonToken.Integer);
			this.WriteIntegerValue(value);
		}

		[CLSCompliant(false)]
		public override void WriteValue(uint value)
		{
			base.InternalWriteValue(JsonToken.Integer);
			this.WriteIntegerValue((long)((ulong)value));
		}

		public override void WriteValue(long value)
		{
			base.InternalWriteValue(JsonToken.Integer);
			this.WriteIntegerValue(value);
		}

		[CLSCompliant(false)]
		public override void WriteValue(ulong value)
		{
			base.InternalWriteValue(JsonToken.Integer);
			this.WriteIntegerValue(value, false);
		}

		public override void WriteValue(float value)
		{
			base.InternalWriteValue(JsonToken.Float);
			this.WriteValueInternal(JsonConvert.ToString(value, base.FloatFormatHandling, this.QuoteChar, false), JsonToken.Float);
		}

		public override void WriteValue(float? value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			base.InternalWriteValue(JsonToken.Float);
			this.WriteValueInternal(JsonConvert.ToString(value.GetValueOrDefault(), base.FloatFormatHandling, this.QuoteChar, true), JsonToken.Float);
		}

		public override void WriteValue(double value)
		{
			base.InternalWriteValue(JsonToken.Float);
			this.WriteValueInternal(JsonConvert.ToString(value, base.FloatFormatHandling, this.QuoteChar, false), JsonToken.Float);
		}

		public override void WriteValue(double? value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			base.InternalWriteValue(JsonToken.Float);
			this.WriteValueInternal(JsonConvert.ToString(value.GetValueOrDefault(), base.FloatFormatHandling, this.QuoteChar, true), JsonToken.Float);
		}

		public override void WriteValue(bool value)
		{
			base.InternalWriteValue(JsonToken.Boolean);
			this.WriteValueInternal(JsonConvert.ToString(value), JsonToken.Boolean);
		}

		public override void WriteValue(short value)
		{
			base.InternalWriteValue(JsonToken.Integer);
			this.WriteIntegerValue((int)value);
		}

		[CLSCompliant(false)]
		public override void WriteValue(ushort value)
		{
			base.InternalWriteValue(JsonToken.Integer);
			this.WriteIntegerValue((int)value);
		}

		public override void WriteValue(char value)
		{
			base.InternalWriteValue(JsonToken.String);
			this.WriteValueInternal(JsonConvert.ToString(value), JsonToken.String);
		}

		public override void WriteValue(byte value)
		{
			base.InternalWriteValue(JsonToken.Integer);
			this.WriteIntegerValue((int)value);
		}

		[CLSCompliant(false)]
		public override void WriteValue(sbyte value)
		{
			base.InternalWriteValue(JsonToken.Integer);
			this.WriteIntegerValue((int)value);
		}

		public override void WriteValue(decimal value)
		{
			base.InternalWriteValue(JsonToken.Float);
			this.WriteValueInternal(JsonConvert.ToString(value), JsonToken.Float);
		}

		public override void WriteValue(DateTime value)
		{
			base.InternalWriteValue(JsonToken.Date);
			value = DateTimeUtils.EnsureDateTime(value, base.DateTimeZoneHandling);
			if (StringUtils.IsNullOrEmpty(base.DateFormatString))
			{
				int count = this.WriteValueToBuffer(value);
				this._writer.Write(this._writeBuffer, 0, count);
				return;
			}
			this._writer.Write(this._quoteChar);
			this._writer.Write(value.ToString(base.DateFormatString, base.Culture));
			this._writer.Write(this._quoteChar);
		}

		private int WriteValueToBuffer(DateTime value)
		{
			this.EnsureWriteBuffer();
			int num = 0;
			this._writeBuffer[num++] = this._quoteChar;
			num = DateTimeUtils.WriteDateTimeString(this._writeBuffer, num, value, null, value.Kind, base.DateFormatHandling);
			this._writeBuffer[num++] = this._quoteChar;
			return num;
		}

		[NullableContext(2)]
		public override void WriteValue(byte[] value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			base.InternalWriteValue(JsonToken.Bytes);
			this._writer.Write(this._quoteChar);
			this.Base64Encoder.Encode(value, 0, value.Length);
			this.Base64Encoder.Flush();
			this._writer.Write(this._quoteChar);
		}

		public override void WriteValue(DateTimeOffset value)
		{
			base.InternalWriteValue(JsonToken.Date);
			if (StringUtils.IsNullOrEmpty(base.DateFormatString))
			{
				int count = this.WriteValueToBuffer(value);
				this._writer.Write(this._writeBuffer, 0, count);
				return;
			}
			this._writer.Write(this._quoteChar);
			this._writer.Write(value.ToString(base.DateFormatString, base.Culture));
			this._writer.Write(this._quoteChar);
		}

		private int WriteValueToBuffer(DateTimeOffset value)
		{
			this.EnsureWriteBuffer();
			int num = 0;
			this._writeBuffer[num++] = this._quoteChar;
			num = DateTimeUtils.WriteDateTimeString(this._writeBuffer, num, (base.DateFormatHandling == DateFormatHandling.IsoDateFormat) ? value.DateTime : value.UtcDateTime, new TimeSpan?(value.Offset), DateTimeKind.Local, base.DateFormatHandling);
			this._writeBuffer[num++] = this._quoteChar;
			return num;
		}

		public override void WriteValue(Guid value)
		{
			base.InternalWriteValue(JsonToken.String);
			string value2 = value.ToString("D", CultureInfo.InvariantCulture);
			this._writer.Write(this._quoteChar);
			this._writer.Write(value2);
			this._writer.Write(this._quoteChar);
		}

		public override void WriteValue(TimeSpan value)
		{
			base.InternalWriteValue(JsonToken.String);
			string value2 = value.ToString(null, CultureInfo.InvariantCulture);
			this._writer.Write(this._quoteChar);
			this._writer.Write(value2);
			this._writer.Write(this._quoteChar);
		}

		[NullableContext(2)]
		public override void WriteValue(Uri value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			base.InternalWriteValue(JsonToken.String);
			this.WriteEscapedString(value.OriginalString, true);
		}

		[NullableContext(2)]
		public override void WriteComment(string text)
		{
			base.InternalWriteComment();
			this._writer.Write("/*");
			this._writer.Write(text);
			this._writer.Write("*/");
		}

		public override void WriteWhitespace(string ws)
		{
			base.InternalWriteWhitespace(ws);
			this._writer.Write(ws);
		}

		private void EnsureWriteBuffer()
		{
			if (this._writeBuffer == null)
			{
				this._writeBuffer = BufferUtils.RentBuffer(this._arrayPool, 35);
			}
		}

		private void WriteIntegerValue(long value)
		{
			if (value >= 0L && value <= 9L)
			{
				this._writer.Write((char)(48L + value));
				return;
			}
			bool flag = value < 0L;
			this.WriteIntegerValue((ulong)(flag ? (-(ulong)value) : value), flag);
		}

		private void WriteIntegerValue(ulong value, bool negative)
		{
			if (!negative & value <= 9UL)
			{
				this._writer.Write((char)(48UL + value));
				return;
			}
			int count = this.WriteNumberToBuffer(value, negative);
			this._writer.Write(this._writeBuffer, 0, count);
		}

		private int WriteNumberToBuffer(ulong value, bool negative)
		{
			if (value <= (ulong)-1)
			{
				return this.WriteNumberToBuffer((uint)value, negative);
			}
			this.EnsureWriteBuffer();
			int num = MathUtils.IntLength(value);
			if (negative)
			{
				num++;
				this._writeBuffer[0] = '-';
			}
			int num2 = num;
			do
			{
				ulong num3 = value / 10UL;
				ulong num4 = value - num3 * 10UL;
				this._writeBuffer[--num2] = (char)(48UL + num4);
				value = num3;
			}
			while (value != 0UL);
			return num;
		}

		private void WriteIntegerValue(int value)
		{
			if (value >= 0 && value <= 9)
			{
				this._writer.Write((char)(48 + value));
				return;
			}
			bool flag = value < 0;
			this.WriteIntegerValue((uint)(flag ? (-(uint)value) : value), flag);
		}

		private void WriteIntegerValue(uint value, bool negative)
		{
			if (!negative & value <= 9U)
			{
				this._writer.Write((char)(48U + value));
				return;
			}
			int count = this.WriteNumberToBuffer(value, negative);
			this._writer.Write(this._writeBuffer, 0, count);
		}

		private int WriteNumberToBuffer(uint value, bool negative)
		{
			this.EnsureWriteBuffer();
			int num = MathUtils.IntLength((ulong)value);
			if (negative)
			{
				num++;
				this._writeBuffer[0] = '-';
			}
			int num2 = num;
			do
			{
				uint num3 = value / 10U;
				uint num4 = value - num3 * 10U;
				this._writeBuffer[--num2] = (char)(48U + num4);
				value = num3;
			}
			while (value != 0U);
			return num;
		}

		private readonly bool _safeAsync;

		private const int IndentCharBufferSize = 12;

		private readonly TextWriter _writer;

		[Nullable(2)]
		private Base64Encoder _base64Encoder;

		private char _indentChar;

		private int _indentation;

		private char _quoteChar;

		private bool _quoteName;

		[Nullable(2)]
		private bool[] _charEscapeFlags;

		[Nullable(2)]
		private char[] _writeBuffer;

		[Nullable(2)]
		private IArrayPool<char> _arrayPool;

		[Nullable(2)]
		private char[] _indentChars;
	}
}
