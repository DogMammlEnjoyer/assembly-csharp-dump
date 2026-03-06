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
	public class JsonTextReader : JsonReader, IJsonLineInfo
	{
		public override Task<bool> ReadAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.ReadAsync(cancellationToken);
			}
			return this.DoReadAsync(cancellationToken);
		}

		internal Task<bool> DoReadAsync(CancellationToken cancellationToken)
		{
			this.EnsureBuffer();
			Task<bool> task;
			for (;;)
			{
				switch (this._currentState)
				{
				case JsonReader.State.Start:
				case JsonReader.State.Property:
				case JsonReader.State.ArrayStart:
				case JsonReader.State.Array:
				case JsonReader.State.ConstructorStart:
				case JsonReader.State.Constructor:
					goto IL_49;
				case JsonReader.State.ObjectStart:
				case JsonReader.State.Object:
					goto IL_51;
				case JsonReader.State.PostValue:
					task = this.ParsePostValueAsync(false, cancellationToken);
					if (!task.IsCompletedSuccessfully())
					{
						goto IL_78;
					}
					if (task.Result)
					{
						goto Block_3;
					}
					continue;
				case JsonReader.State.Finished:
					goto IL_81;
				}
				break;
			}
			goto IL_89;
			IL_49:
			return this.ParseValueAsync(cancellationToken);
			IL_51:
			return this.ParseObjectAsync(cancellationToken);
			Block_3:
			return AsyncUtils.True;
			IL_78:
			return this.DoReadAsync(task, cancellationToken);
			IL_81:
			return this.ReadFromFinishedAsync(cancellationToken);
			IL_89:
			throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
		}

		private Task<bool> DoReadAsync(Task<bool> task, CancellationToken cancellationToken)
		{
			JsonTextReader.<DoReadAsync>d__3 <DoReadAsync>d__;
			<DoReadAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<DoReadAsync>d__.<>4__this = this;
			<DoReadAsync>d__.task = task;
			<DoReadAsync>d__.cancellationToken = cancellationToken;
			<DoReadAsync>d__.<>1__state = -1;
			<DoReadAsync>d__.<>t__builder.Start<JsonTextReader.<DoReadAsync>d__3>(ref <DoReadAsync>d__);
			return <DoReadAsync>d__.<>t__builder.Task;
		}

		private Task<bool> ParsePostValueAsync(bool ignoreComments, CancellationToken cancellationToken)
		{
			JsonTextReader.<ParsePostValueAsync>d__4 <ParsePostValueAsync>d__;
			<ParsePostValueAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<ParsePostValueAsync>d__.<>4__this = this;
			<ParsePostValueAsync>d__.ignoreComments = ignoreComments;
			<ParsePostValueAsync>d__.cancellationToken = cancellationToken;
			<ParsePostValueAsync>d__.<>1__state = -1;
			<ParsePostValueAsync>d__.<>t__builder.Start<JsonTextReader.<ParsePostValueAsync>d__4>(ref <ParsePostValueAsync>d__);
			return <ParsePostValueAsync>d__.<>t__builder.Task;
		}

		private Task<bool> ReadFromFinishedAsync(CancellationToken cancellationToken)
		{
			JsonTextReader.<ReadFromFinishedAsync>d__5 <ReadFromFinishedAsync>d__;
			<ReadFromFinishedAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<ReadFromFinishedAsync>d__.<>4__this = this;
			<ReadFromFinishedAsync>d__.cancellationToken = cancellationToken;
			<ReadFromFinishedAsync>d__.<>1__state = -1;
			<ReadFromFinishedAsync>d__.<>t__builder.Start<JsonTextReader.<ReadFromFinishedAsync>d__5>(ref <ReadFromFinishedAsync>d__);
			return <ReadFromFinishedAsync>d__.<>t__builder.Task;
		}

		private Task<int> ReadDataAsync(bool append, CancellationToken cancellationToken)
		{
			return this.ReadDataAsync(append, 0, cancellationToken);
		}

		private Task<int> ReadDataAsync(bool append, int charsRequired, CancellationToken cancellationToken)
		{
			JsonTextReader.<ReadDataAsync>d__7 <ReadDataAsync>d__;
			<ReadDataAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ReadDataAsync>d__.<>4__this = this;
			<ReadDataAsync>d__.append = append;
			<ReadDataAsync>d__.charsRequired = charsRequired;
			<ReadDataAsync>d__.cancellationToken = cancellationToken;
			<ReadDataAsync>d__.<>1__state = -1;
			<ReadDataAsync>d__.<>t__builder.Start<JsonTextReader.<ReadDataAsync>d__7>(ref <ReadDataAsync>d__);
			return <ReadDataAsync>d__.<>t__builder.Task;
		}

		private Task<bool> ParseValueAsync(CancellationToken cancellationToken)
		{
			JsonTextReader.<ParseValueAsync>d__8 <ParseValueAsync>d__;
			<ParseValueAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<ParseValueAsync>d__.<>4__this = this;
			<ParseValueAsync>d__.cancellationToken = cancellationToken;
			<ParseValueAsync>d__.<>1__state = -1;
			<ParseValueAsync>d__.<>t__builder.Start<JsonTextReader.<ParseValueAsync>d__8>(ref <ParseValueAsync>d__);
			return <ParseValueAsync>d__.<>t__builder.Task;
		}

		private Task ReadStringIntoBufferAsync(char quote, CancellationToken cancellationToken)
		{
			JsonTextReader.<ReadStringIntoBufferAsync>d__9 <ReadStringIntoBufferAsync>d__;
			<ReadStringIntoBufferAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ReadStringIntoBufferAsync>d__.<>4__this = this;
			<ReadStringIntoBufferAsync>d__.quote = quote;
			<ReadStringIntoBufferAsync>d__.cancellationToken = cancellationToken;
			<ReadStringIntoBufferAsync>d__.<>1__state = -1;
			<ReadStringIntoBufferAsync>d__.<>t__builder.Start<JsonTextReader.<ReadStringIntoBufferAsync>d__9>(ref <ReadStringIntoBufferAsync>d__);
			return <ReadStringIntoBufferAsync>d__.<>t__builder.Task;
		}

		private Task ProcessCarriageReturnAsync(bool append, CancellationToken cancellationToken)
		{
			this._charPos++;
			Task<bool> task = this.EnsureCharsAsync(1, append, cancellationToken);
			if (task.IsCompletedSuccessfully())
			{
				this.SetNewLine(task.Result);
				return AsyncUtils.CompletedTask;
			}
			return this.ProcessCarriageReturnAsync(task);
		}

		private Task ProcessCarriageReturnAsync(Task<bool> task)
		{
			JsonTextReader.<ProcessCarriageReturnAsync>d__11 <ProcessCarriageReturnAsync>d__;
			<ProcessCarriageReturnAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ProcessCarriageReturnAsync>d__.<>4__this = this;
			<ProcessCarriageReturnAsync>d__.task = task;
			<ProcessCarriageReturnAsync>d__.<>1__state = -1;
			<ProcessCarriageReturnAsync>d__.<>t__builder.Start<JsonTextReader.<ProcessCarriageReturnAsync>d__11>(ref <ProcessCarriageReturnAsync>d__);
			return <ProcessCarriageReturnAsync>d__.<>t__builder.Task;
		}

		private Task<char> ParseUnicodeAsync(CancellationToken cancellationToken)
		{
			JsonTextReader.<ParseUnicodeAsync>d__12 <ParseUnicodeAsync>d__;
			<ParseUnicodeAsync>d__.<>t__builder = AsyncTaskMethodBuilder<char>.Create();
			<ParseUnicodeAsync>d__.<>4__this = this;
			<ParseUnicodeAsync>d__.cancellationToken = cancellationToken;
			<ParseUnicodeAsync>d__.<>1__state = -1;
			<ParseUnicodeAsync>d__.<>t__builder.Start<JsonTextReader.<ParseUnicodeAsync>d__12>(ref <ParseUnicodeAsync>d__);
			return <ParseUnicodeAsync>d__.<>t__builder.Task;
		}

		private Task<bool> EnsureCharsAsync(int relativePosition, bool append, CancellationToken cancellationToken)
		{
			if (this._charPos + relativePosition < this._charsUsed)
			{
				return AsyncUtils.True;
			}
			if (this._isEndOfFile)
			{
				return AsyncUtils.False;
			}
			return this.ReadCharsAsync(relativePosition, append, cancellationToken);
		}

		private Task<bool> ReadCharsAsync(int relativePosition, bool append, CancellationToken cancellationToken)
		{
			JsonTextReader.<ReadCharsAsync>d__14 <ReadCharsAsync>d__;
			<ReadCharsAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<ReadCharsAsync>d__.<>4__this = this;
			<ReadCharsAsync>d__.relativePosition = relativePosition;
			<ReadCharsAsync>d__.append = append;
			<ReadCharsAsync>d__.cancellationToken = cancellationToken;
			<ReadCharsAsync>d__.<>1__state = -1;
			<ReadCharsAsync>d__.<>t__builder.Start<JsonTextReader.<ReadCharsAsync>d__14>(ref <ReadCharsAsync>d__);
			return <ReadCharsAsync>d__.<>t__builder.Task;
		}

		private Task<bool> ParseObjectAsync(CancellationToken cancellationToken)
		{
			JsonTextReader.<ParseObjectAsync>d__15 <ParseObjectAsync>d__;
			<ParseObjectAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<ParseObjectAsync>d__.<>4__this = this;
			<ParseObjectAsync>d__.cancellationToken = cancellationToken;
			<ParseObjectAsync>d__.<>1__state = -1;
			<ParseObjectAsync>d__.<>t__builder.Start<JsonTextReader.<ParseObjectAsync>d__15>(ref <ParseObjectAsync>d__);
			return <ParseObjectAsync>d__.<>t__builder.Task;
		}

		private Task ParseCommentAsync(bool setToken, CancellationToken cancellationToken)
		{
			JsonTextReader.<ParseCommentAsync>d__16 <ParseCommentAsync>d__;
			<ParseCommentAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseCommentAsync>d__.<>4__this = this;
			<ParseCommentAsync>d__.setToken = setToken;
			<ParseCommentAsync>d__.cancellationToken = cancellationToken;
			<ParseCommentAsync>d__.<>1__state = -1;
			<ParseCommentAsync>d__.<>t__builder.Start<JsonTextReader.<ParseCommentAsync>d__16>(ref <ParseCommentAsync>d__);
			return <ParseCommentAsync>d__.<>t__builder.Task;
		}

		private Task EatWhitespaceAsync(CancellationToken cancellationToken)
		{
			JsonTextReader.<EatWhitespaceAsync>d__17 <EatWhitespaceAsync>d__;
			<EatWhitespaceAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<EatWhitespaceAsync>d__.<>4__this = this;
			<EatWhitespaceAsync>d__.cancellationToken = cancellationToken;
			<EatWhitespaceAsync>d__.<>1__state = -1;
			<EatWhitespaceAsync>d__.<>t__builder.Start<JsonTextReader.<EatWhitespaceAsync>d__17>(ref <EatWhitespaceAsync>d__);
			return <EatWhitespaceAsync>d__.<>t__builder.Task;
		}

		private Task ParseStringAsync(char quote, ReadType readType, CancellationToken cancellationToken)
		{
			JsonTextReader.<ParseStringAsync>d__18 <ParseStringAsync>d__;
			<ParseStringAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseStringAsync>d__.<>4__this = this;
			<ParseStringAsync>d__.quote = quote;
			<ParseStringAsync>d__.readType = readType;
			<ParseStringAsync>d__.cancellationToken = cancellationToken;
			<ParseStringAsync>d__.<>1__state = -1;
			<ParseStringAsync>d__.<>t__builder.Start<JsonTextReader.<ParseStringAsync>d__18>(ref <ParseStringAsync>d__);
			return <ParseStringAsync>d__.<>t__builder.Task;
		}

		private Task<bool> MatchValueAsync(string value, CancellationToken cancellationToken)
		{
			JsonTextReader.<MatchValueAsync>d__19 <MatchValueAsync>d__;
			<MatchValueAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<MatchValueAsync>d__.<>4__this = this;
			<MatchValueAsync>d__.value = value;
			<MatchValueAsync>d__.cancellationToken = cancellationToken;
			<MatchValueAsync>d__.<>1__state = -1;
			<MatchValueAsync>d__.<>t__builder.Start<JsonTextReader.<MatchValueAsync>d__19>(ref <MatchValueAsync>d__);
			return <MatchValueAsync>d__.<>t__builder.Task;
		}

		private Task<bool> MatchValueWithTrailingSeparatorAsync(string value, CancellationToken cancellationToken)
		{
			JsonTextReader.<MatchValueWithTrailingSeparatorAsync>d__20 <MatchValueWithTrailingSeparatorAsync>d__;
			<MatchValueWithTrailingSeparatorAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<MatchValueWithTrailingSeparatorAsync>d__.<>4__this = this;
			<MatchValueWithTrailingSeparatorAsync>d__.value = value;
			<MatchValueWithTrailingSeparatorAsync>d__.cancellationToken = cancellationToken;
			<MatchValueWithTrailingSeparatorAsync>d__.<>1__state = -1;
			<MatchValueWithTrailingSeparatorAsync>d__.<>t__builder.Start<JsonTextReader.<MatchValueWithTrailingSeparatorAsync>d__20>(ref <MatchValueWithTrailingSeparatorAsync>d__);
			return <MatchValueWithTrailingSeparatorAsync>d__.<>t__builder.Task;
		}

		private Task MatchAndSetAsync(string value, JsonToken newToken, [Nullable(2)] object tokenValue, CancellationToken cancellationToken)
		{
			JsonTextReader.<MatchAndSetAsync>d__21 <MatchAndSetAsync>d__;
			<MatchAndSetAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<MatchAndSetAsync>d__.<>4__this = this;
			<MatchAndSetAsync>d__.value = value;
			<MatchAndSetAsync>d__.newToken = newToken;
			<MatchAndSetAsync>d__.tokenValue = tokenValue;
			<MatchAndSetAsync>d__.cancellationToken = cancellationToken;
			<MatchAndSetAsync>d__.<>1__state = -1;
			<MatchAndSetAsync>d__.<>t__builder.Start<JsonTextReader.<MatchAndSetAsync>d__21>(ref <MatchAndSetAsync>d__);
			return <MatchAndSetAsync>d__.<>t__builder.Task;
		}

		private Task ParseTrueAsync(CancellationToken cancellationToken)
		{
			return this.MatchAndSetAsync(JsonConvert.True, JsonToken.Boolean, true, cancellationToken);
		}

		private Task ParseFalseAsync(CancellationToken cancellationToken)
		{
			return this.MatchAndSetAsync(JsonConvert.False, JsonToken.Boolean, false, cancellationToken);
		}

		private Task ParseNullAsync(CancellationToken cancellationToken)
		{
			return this.MatchAndSetAsync(JsonConvert.Null, JsonToken.Null, null, cancellationToken);
		}

		private Task ParseConstructorAsync(CancellationToken cancellationToken)
		{
			JsonTextReader.<ParseConstructorAsync>d__25 <ParseConstructorAsync>d__;
			<ParseConstructorAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseConstructorAsync>d__.<>4__this = this;
			<ParseConstructorAsync>d__.cancellationToken = cancellationToken;
			<ParseConstructorAsync>d__.<>1__state = -1;
			<ParseConstructorAsync>d__.<>t__builder.Start<JsonTextReader.<ParseConstructorAsync>d__25>(ref <ParseConstructorAsync>d__);
			return <ParseConstructorAsync>d__.<>t__builder.Task;
		}

		private Task<object> ParseNumberNaNAsync(ReadType readType, CancellationToken cancellationToken)
		{
			JsonTextReader.<ParseNumberNaNAsync>d__26 <ParseNumberNaNAsync>d__;
			<ParseNumberNaNAsync>d__.<>t__builder = AsyncTaskMethodBuilder<object>.Create();
			<ParseNumberNaNAsync>d__.<>4__this = this;
			<ParseNumberNaNAsync>d__.readType = readType;
			<ParseNumberNaNAsync>d__.cancellationToken = cancellationToken;
			<ParseNumberNaNAsync>d__.<>1__state = -1;
			<ParseNumberNaNAsync>d__.<>t__builder.Start<JsonTextReader.<ParseNumberNaNAsync>d__26>(ref <ParseNumberNaNAsync>d__);
			return <ParseNumberNaNAsync>d__.<>t__builder.Task;
		}

		private Task<object> ParseNumberPositiveInfinityAsync(ReadType readType, CancellationToken cancellationToken)
		{
			JsonTextReader.<ParseNumberPositiveInfinityAsync>d__27 <ParseNumberPositiveInfinityAsync>d__;
			<ParseNumberPositiveInfinityAsync>d__.<>t__builder = AsyncTaskMethodBuilder<object>.Create();
			<ParseNumberPositiveInfinityAsync>d__.<>4__this = this;
			<ParseNumberPositiveInfinityAsync>d__.readType = readType;
			<ParseNumberPositiveInfinityAsync>d__.cancellationToken = cancellationToken;
			<ParseNumberPositiveInfinityAsync>d__.<>1__state = -1;
			<ParseNumberPositiveInfinityAsync>d__.<>t__builder.Start<JsonTextReader.<ParseNumberPositiveInfinityAsync>d__27>(ref <ParseNumberPositiveInfinityAsync>d__);
			return <ParseNumberPositiveInfinityAsync>d__.<>t__builder.Task;
		}

		private Task<object> ParseNumberNegativeInfinityAsync(ReadType readType, CancellationToken cancellationToken)
		{
			JsonTextReader.<ParseNumberNegativeInfinityAsync>d__28 <ParseNumberNegativeInfinityAsync>d__;
			<ParseNumberNegativeInfinityAsync>d__.<>t__builder = AsyncTaskMethodBuilder<object>.Create();
			<ParseNumberNegativeInfinityAsync>d__.<>4__this = this;
			<ParseNumberNegativeInfinityAsync>d__.readType = readType;
			<ParseNumberNegativeInfinityAsync>d__.cancellationToken = cancellationToken;
			<ParseNumberNegativeInfinityAsync>d__.<>1__state = -1;
			<ParseNumberNegativeInfinityAsync>d__.<>t__builder.Start<JsonTextReader.<ParseNumberNegativeInfinityAsync>d__28>(ref <ParseNumberNegativeInfinityAsync>d__);
			return <ParseNumberNegativeInfinityAsync>d__.<>t__builder.Task;
		}

		private Task ParseNumberAsync(ReadType readType, CancellationToken cancellationToken)
		{
			JsonTextReader.<ParseNumberAsync>d__29 <ParseNumberAsync>d__;
			<ParseNumberAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseNumberAsync>d__.<>4__this = this;
			<ParseNumberAsync>d__.readType = readType;
			<ParseNumberAsync>d__.cancellationToken = cancellationToken;
			<ParseNumberAsync>d__.<>1__state = -1;
			<ParseNumberAsync>d__.<>t__builder.Start<JsonTextReader.<ParseNumberAsync>d__29>(ref <ParseNumberAsync>d__);
			return <ParseNumberAsync>d__.<>t__builder.Task;
		}

		private Task ParseUndefinedAsync(CancellationToken cancellationToken)
		{
			return this.MatchAndSetAsync(JsonConvert.Undefined, JsonToken.Undefined, null, cancellationToken);
		}

		private Task<bool> ParsePropertyAsync(CancellationToken cancellationToken)
		{
			JsonTextReader.<ParsePropertyAsync>d__31 <ParsePropertyAsync>d__;
			<ParsePropertyAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<ParsePropertyAsync>d__.<>4__this = this;
			<ParsePropertyAsync>d__.cancellationToken = cancellationToken;
			<ParsePropertyAsync>d__.<>1__state = -1;
			<ParsePropertyAsync>d__.<>t__builder.Start<JsonTextReader.<ParsePropertyAsync>d__31>(ref <ParsePropertyAsync>d__);
			return <ParsePropertyAsync>d__.<>t__builder.Task;
		}

		private Task ReadNumberIntoBufferAsync(CancellationToken cancellationToken)
		{
			JsonTextReader.<ReadNumberIntoBufferAsync>d__32 <ReadNumberIntoBufferAsync>d__;
			<ReadNumberIntoBufferAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ReadNumberIntoBufferAsync>d__.<>4__this = this;
			<ReadNumberIntoBufferAsync>d__.cancellationToken = cancellationToken;
			<ReadNumberIntoBufferAsync>d__.<>1__state = -1;
			<ReadNumberIntoBufferAsync>d__.<>t__builder.Start<JsonTextReader.<ReadNumberIntoBufferAsync>d__32>(ref <ReadNumberIntoBufferAsync>d__);
			return <ReadNumberIntoBufferAsync>d__.<>t__builder.Task;
		}

		private Task ParseUnquotedPropertyAsync(CancellationToken cancellationToken)
		{
			JsonTextReader.<ParseUnquotedPropertyAsync>d__33 <ParseUnquotedPropertyAsync>d__;
			<ParseUnquotedPropertyAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseUnquotedPropertyAsync>d__.<>4__this = this;
			<ParseUnquotedPropertyAsync>d__.cancellationToken = cancellationToken;
			<ParseUnquotedPropertyAsync>d__.<>1__state = -1;
			<ParseUnquotedPropertyAsync>d__.<>t__builder.Start<JsonTextReader.<ParseUnquotedPropertyAsync>d__33>(ref <ParseUnquotedPropertyAsync>d__);
			return <ParseUnquotedPropertyAsync>d__.<>t__builder.Task;
		}

		private Task<bool> ReadNullCharAsync(CancellationToken cancellationToken)
		{
			JsonTextReader.<ReadNullCharAsync>d__34 <ReadNullCharAsync>d__;
			<ReadNullCharAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<ReadNullCharAsync>d__.<>4__this = this;
			<ReadNullCharAsync>d__.cancellationToken = cancellationToken;
			<ReadNullCharAsync>d__.<>1__state = -1;
			<ReadNullCharAsync>d__.<>t__builder.Start<JsonTextReader.<ReadNullCharAsync>d__34>(ref <ReadNullCharAsync>d__);
			return <ReadNullCharAsync>d__.<>t__builder.Task;
		}

		private Task HandleNullAsync(CancellationToken cancellationToken)
		{
			JsonTextReader.<HandleNullAsync>d__35 <HandleNullAsync>d__;
			<HandleNullAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<HandleNullAsync>d__.<>4__this = this;
			<HandleNullAsync>d__.cancellationToken = cancellationToken;
			<HandleNullAsync>d__.<>1__state = -1;
			<HandleNullAsync>d__.<>t__builder.Start<JsonTextReader.<HandleNullAsync>d__35>(ref <HandleNullAsync>d__);
			return <HandleNullAsync>d__.<>t__builder.Task;
		}

		private Task ReadFinishedAsync(CancellationToken cancellationToken)
		{
			JsonTextReader.<ReadFinishedAsync>d__36 <ReadFinishedAsync>d__;
			<ReadFinishedAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ReadFinishedAsync>d__.<>4__this = this;
			<ReadFinishedAsync>d__.cancellationToken = cancellationToken;
			<ReadFinishedAsync>d__.<>1__state = -1;
			<ReadFinishedAsync>d__.<>t__builder.Start<JsonTextReader.<ReadFinishedAsync>d__36>(ref <ReadFinishedAsync>d__);
			return <ReadFinishedAsync>d__.<>t__builder.Task;
		}

		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		private Task<object> ReadStringValueAsync(ReadType readType, CancellationToken cancellationToken)
		{
			JsonTextReader.<ReadStringValueAsync>d__37 <ReadStringValueAsync>d__;
			<ReadStringValueAsync>d__.<>t__builder = AsyncTaskMethodBuilder<object>.Create();
			<ReadStringValueAsync>d__.<>4__this = this;
			<ReadStringValueAsync>d__.readType = readType;
			<ReadStringValueAsync>d__.cancellationToken = cancellationToken;
			<ReadStringValueAsync>d__.<>1__state = -1;
			<ReadStringValueAsync>d__.<>t__builder.Start<JsonTextReader.<ReadStringValueAsync>d__37>(ref <ReadStringValueAsync>d__);
			return <ReadStringValueAsync>d__.<>t__builder.Task;
		}

		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		private Task<object> ReadNumberValueAsync(ReadType readType, CancellationToken cancellationToken)
		{
			JsonTextReader.<ReadNumberValueAsync>d__38 <ReadNumberValueAsync>d__;
			<ReadNumberValueAsync>d__.<>t__builder = AsyncTaskMethodBuilder<object>.Create();
			<ReadNumberValueAsync>d__.<>4__this = this;
			<ReadNumberValueAsync>d__.readType = readType;
			<ReadNumberValueAsync>d__.cancellationToken = cancellationToken;
			<ReadNumberValueAsync>d__.<>1__state = -1;
			<ReadNumberValueAsync>d__.<>t__builder.Start<JsonTextReader.<ReadNumberValueAsync>d__38>(ref <ReadNumberValueAsync>d__);
			return <ReadNumberValueAsync>d__.<>t__builder.Task;
		}

		public override Task<bool?> ReadAsBooleanAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.ReadAsBooleanAsync(cancellationToken);
			}
			return this.DoReadAsBooleanAsync(cancellationToken);
		}

		internal Task<bool?> DoReadAsBooleanAsync(CancellationToken cancellationToken)
		{
			JsonTextReader.<DoReadAsBooleanAsync>d__40 <DoReadAsBooleanAsync>d__;
			<DoReadAsBooleanAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool?>.Create();
			<DoReadAsBooleanAsync>d__.<>4__this = this;
			<DoReadAsBooleanAsync>d__.cancellationToken = cancellationToken;
			<DoReadAsBooleanAsync>d__.<>1__state = -1;
			<DoReadAsBooleanAsync>d__.<>t__builder.Start<JsonTextReader.<DoReadAsBooleanAsync>d__40>(ref <DoReadAsBooleanAsync>d__);
			return <DoReadAsBooleanAsync>d__.<>t__builder.Task;
		}

		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public override Task<byte[]> ReadAsBytesAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.ReadAsBytesAsync(cancellationToken);
			}
			return this.DoReadAsBytesAsync(cancellationToken);
		}

		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		internal Task<byte[]> DoReadAsBytesAsync(CancellationToken cancellationToken)
		{
			JsonTextReader.<DoReadAsBytesAsync>d__42 <DoReadAsBytesAsync>d__;
			<DoReadAsBytesAsync>d__.<>t__builder = AsyncTaskMethodBuilder<byte[]>.Create();
			<DoReadAsBytesAsync>d__.<>4__this = this;
			<DoReadAsBytesAsync>d__.cancellationToken = cancellationToken;
			<DoReadAsBytesAsync>d__.<>1__state = -1;
			<DoReadAsBytesAsync>d__.<>t__builder.Start<JsonTextReader.<DoReadAsBytesAsync>d__42>(ref <DoReadAsBytesAsync>d__);
			return <DoReadAsBytesAsync>d__.<>t__builder.Task;
		}

		private Task ReadIntoWrappedTypeObjectAsync(CancellationToken cancellationToken)
		{
			JsonTextReader.<ReadIntoWrappedTypeObjectAsync>d__43 <ReadIntoWrappedTypeObjectAsync>d__;
			<ReadIntoWrappedTypeObjectAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ReadIntoWrappedTypeObjectAsync>d__.<>4__this = this;
			<ReadIntoWrappedTypeObjectAsync>d__.cancellationToken = cancellationToken;
			<ReadIntoWrappedTypeObjectAsync>d__.<>1__state = -1;
			<ReadIntoWrappedTypeObjectAsync>d__.<>t__builder.Start<JsonTextReader.<ReadIntoWrappedTypeObjectAsync>d__43>(ref <ReadIntoWrappedTypeObjectAsync>d__);
			return <ReadIntoWrappedTypeObjectAsync>d__.<>t__builder.Task;
		}

		public override Task<DateTime?> ReadAsDateTimeAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.ReadAsDateTimeAsync(cancellationToken);
			}
			return this.DoReadAsDateTimeAsync(cancellationToken);
		}

		internal Task<DateTime?> DoReadAsDateTimeAsync(CancellationToken cancellationToken)
		{
			JsonTextReader.<DoReadAsDateTimeAsync>d__45 <DoReadAsDateTimeAsync>d__;
			<DoReadAsDateTimeAsync>d__.<>t__builder = AsyncTaskMethodBuilder<DateTime?>.Create();
			<DoReadAsDateTimeAsync>d__.<>4__this = this;
			<DoReadAsDateTimeAsync>d__.cancellationToken = cancellationToken;
			<DoReadAsDateTimeAsync>d__.<>1__state = -1;
			<DoReadAsDateTimeAsync>d__.<>t__builder.Start<JsonTextReader.<DoReadAsDateTimeAsync>d__45>(ref <DoReadAsDateTimeAsync>d__);
			return <DoReadAsDateTimeAsync>d__.<>t__builder.Task;
		}

		public override Task<DateTimeOffset?> ReadAsDateTimeOffsetAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.ReadAsDateTimeOffsetAsync(cancellationToken);
			}
			return this.DoReadAsDateTimeOffsetAsync(cancellationToken);
		}

		internal Task<DateTimeOffset?> DoReadAsDateTimeOffsetAsync(CancellationToken cancellationToken)
		{
			JsonTextReader.<DoReadAsDateTimeOffsetAsync>d__47 <DoReadAsDateTimeOffsetAsync>d__;
			<DoReadAsDateTimeOffsetAsync>d__.<>t__builder = AsyncTaskMethodBuilder<DateTimeOffset?>.Create();
			<DoReadAsDateTimeOffsetAsync>d__.<>4__this = this;
			<DoReadAsDateTimeOffsetAsync>d__.cancellationToken = cancellationToken;
			<DoReadAsDateTimeOffsetAsync>d__.<>1__state = -1;
			<DoReadAsDateTimeOffsetAsync>d__.<>t__builder.Start<JsonTextReader.<DoReadAsDateTimeOffsetAsync>d__47>(ref <DoReadAsDateTimeOffsetAsync>d__);
			return <DoReadAsDateTimeOffsetAsync>d__.<>t__builder.Task;
		}

		public override Task<decimal?> ReadAsDecimalAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.ReadAsDecimalAsync(cancellationToken);
			}
			return this.DoReadAsDecimalAsync(cancellationToken);
		}

		internal Task<decimal?> DoReadAsDecimalAsync(CancellationToken cancellationToken)
		{
			JsonTextReader.<DoReadAsDecimalAsync>d__49 <DoReadAsDecimalAsync>d__;
			<DoReadAsDecimalAsync>d__.<>t__builder = AsyncTaskMethodBuilder<decimal?>.Create();
			<DoReadAsDecimalAsync>d__.<>4__this = this;
			<DoReadAsDecimalAsync>d__.cancellationToken = cancellationToken;
			<DoReadAsDecimalAsync>d__.<>1__state = -1;
			<DoReadAsDecimalAsync>d__.<>t__builder.Start<JsonTextReader.<DoReadAsDecimalAsync>d__49>(ref <DoReadAsDecimalAsync>d__);
			return <DoReadAsDecimalAsync>d__.<>t__builder.Task;
		}

		public override Task<double?> ReadAsDoubleAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.ReadAsDoubleAsync(cancellationToken);
			}
			return this.DoReadAsDoubleAsync(cancellationToken);
		}

		internal Task<double?> DoReadAsDoubleAsync(CancellationToken cancellationToken)
		{
			JsonTextReader.<DoReadAsDoubleAsync>d__51 <DoReadAsDoubleAsync>d__;
			<DoReadAsDoubleAsync>d__.<>t__builder = AsyncTaskMethodBuilder<double?>.Create();
			<DoReadAsDoubleAsync>d__.<>4__this = this;
			<DoReadAsDoubleAsync>d__.cancellationToken = cancellationToken;
			<DoReadAsDoubleAsync>d__.<>1__state = -1;
			<DoReadAsDoubleAsync>d__.<>t__builder.Start<JsonTextReader.<DoReadAsDoubleAsync>d__51>(ref <DoReadAsDoubleAsync>d__);
			return <DoReadAsDoubleAsync>d__.<>t__builder.Task;
		}

		public override Task<int?> ReadAsInt32Async(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.ReadAsInt32Async(cancellationToken);
			}
			return this.DoReadAsInt32Async(cancellationToken);
		}

		internal Task<int?> DoReadAsInt32Async(CancellationToken cancellationToken)
		{
			JsonTextReader.<DoReadAsInt32Async>d__53 <DoReadAsInt32Async>d__;
			<DoReadAsInt32Async>d__.<>t__builder = AsyncTaskMethodBuilder<int?>.Create();
			<DoReadAsInt32Async>d__.<>4__this = this;
			<DoReadAsInt32Async>d__.cancellationToken = cancellationToken;
			<DoReadAsInt32Async>d__.<>1__state = -1;
			<DoReadAsInt32Async>d__.<>t__builder.Start<JsonTextReader.<DoReadAsInt32Async>d__53>(ref <DoReadAsInt32Async>d__);
			return <DoReadAsInt32Async>d__.<>t__builder.Task;
		}

		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public override Task<string> ReadAsStringAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._safeAsync)
			{
				return base.ReadAsStringAsync(cancellationToken);
			}
			return this.DoReadAsStringAsync(cancellationToken);
		}

		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		internal Task<string> DoReadAsStringAsync(CancellationToken cancellationToken)
		{
			JsonTextReader.<DoReadAsStringAsync>d__55 <DoReadAsStringAsync>d__;
			<DoReadAsStringAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<DoReadAsStringAsync>d__.<>4__this = this;
			<DoReadAsStringAsync>d__.cancellationToken = cancellationToken;
			<DoReadAsStringAsync>d__.<>1__state = -1;
			<DoReadAsStringAsync>d__.<>t__builder.Start<JsonTextReader.<DoReadAsStringAsync>d__55>(ref <DoReadAsStringAsync>d__);
			return <DoReadAsStringAsync>d__.<>t__builder.Task;
		}

		public JsonTextReader(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			this._reader = reader;
			this._lineNumber = 1;
			this._safeAsync = (base.GetType() == typeof(JsonTextReader));
		}

		[Nullable(2)]
		public JsonNameTable PropertyNameTable { [NullableContext(2)] get; [NullableContext(2)] set; }

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

		private void EnsureBufferNotEmpty()
		{
			if (this._stringBuffer.IsEmpty)
			{
				this._stringBuffer = new StringBuffer(this._arrayPool, 1024);
			}
		}

		private void SetNewLine(bool hasNextChar)
		{
			if (hasNextChar && this._chars[this._charPos] == '\n')
			{
				this._charPos++;
			}
			this.OnNewLine(this._charPos);
		}

		private void OnNewLine(int pos)
		{
			this._lineNumber++;
			this._lineStartPos = pos;
		}

		private void ParseString(char quote, ReadType readType)
		{
			this._charPos++;
			this.ShiftBufferIfNeeded();
			this.ReadStringIntoBuffer(quote);
			this.ParseReadString(quote, readType);
		}

		private void ParseReadString(char quote, ReadType readType)
		{
			base.SetPostValueState(true);
			switch (readType)
			{
			case ReadType.ReadAsInt32:
			case ReadType.ReadAsDecimal:
			case ReadType.ReadAsBoolean:
				return;
			case ReadType.ReadAsBytes:
			{
				byte[] value;
				Guid guid;
				if (this._stringReference.Length == 0)
				{
					value = CollectionUtils.ArrayEmpty<byte>();
				}
				else if (this._stringReference.Length == 36 && ConvertUtils.TryConvertGuid(this._stringReference.ToString(), out guid))
				{
					value = guid.ToByteArray();
				}
				else
				{
					value = Convert.FromBase64CharArray(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length);
				}
				base.SetToken(JsonToken.Bytes, value, false);
				return;
			}
			case ReadType.ReadAsString:
			{
				string value2 = this._stringReference.ToString();
				base.SetToken(JsonToken.String, value2, false);
				this._quoteChar = quote;
				return;
			}
			}
			if (this._dateParseHandling != DateParseHandling.None)
			{
				DateParseHandling dateParseHandling;
				if (readType == ReadType.ReadAsDateTime)
				{
					dateParseHandling = DateParseHandling.DateTime;
				}
				else if (readType == ReadType.ReadAsDateTimeOffset)
				{
					dateParseHandling = DateParseHandling.DateTimeOffset;
				}
				else
				{
					dateParseHandling = this._dateParseHandling;
				}
				DateTimeOffset dateTimeOffset;
				if (dateParseHandling == DateParseHandling.DateTime)
				{
					DateTime dateTime;
					if (DateTimeUtils.TryParseDateTime(this._stringReference, base.DateTimeZoneHandling, base.DateFormatString, base.Culture, out dateTime))
					{
						base.SetToken(JsonToken.Date, dateTime, false);
						return;
					}
				}
				else if (DateTimeUtils.TryParseDateTimeOffset(this._stringReference, base.DateFormatString, base.Culture, out dateTimeOffset))
				{
					base.SetToken(JsonToken.Date, dateTimeOffset, false);
					return;
				}
			}
			base.SetToken(JsonToken.String, this._stringReference.ToString(), false);
			this._quoteChar = quote;
		}

		private static void BlockCopyChars(char[] src, int srcOffset, char[] dst, int dstOffset, int count)
		{
			Buffer.BlockCopy(src, srcOffset * 2, dst, dstOffset * 2, count * 2);
		}

		private void ShiftBufferIfNeeded()
		{
			int num = this._chars.Length;
			if ((double)(num - this._charPos) <= (double)num * 0.1 || num >= 1073741823)
			{
				int num2 = this._charsUsed - this._charPos;
				if (num2 > 0)
				{
					JsonTextReader.BlockCopyChars(this._chars, this._charPos, this._chars, 0, num2);
				}
				this._lineStartPos -= this._charPos;
				this._charPos = 0;
				this._charsUsed = num2;
				this._chars[this._charsUsed] = '\0';
			}
		}

		private int ReadData(bool append)
		{
			return this.ReadData(append, 0);
		}

		private void PrepareBufferForReadData(bool append, int charsRequired)
		{
			if (this._charsUsed + charsRequired >= this._chars.Length - 1)
			{
				if (append)
				{
					int num = this._chars.Length * 2;
					int minSize = Math.Max((num < 0) ? int.MaxValue : num, this._charsUsed + charsRequired + 1);
					char[] array = BufferUtils.RentBuffer(this._arrayPool, minSize);
					JsonTextReader.BlockCopyChars(this._chars, 0, array, 0, this._chars.Length);
					BufferUtils.ReturnBuffer(this._arrayPool, this._chars);
					this._chars = array;
					return;
				}
				int num2 = this._charsUsed - this._charPos;
				if (num2 + charsRequired + 1 >= this._chars.Length)
				{
					char[] array2 = BufferUtils.RentBuffer(this._arrayPool, num2 + charsRequired + 1);
					if (num2 > 0)
					{
						JsonTextReader.BlockCopyChars(this._chars, this._charPos, array2, 0, num2);
					}
					BufferUtils.ReturnBuffer(this._arrayPool, this._chars);
					this._chars = array2;
				}
				else if (num2 > 0)
				{
					JsonTextReader.BlockCopyChars(this._chars, this._charPos, this._chars, 0, num2);
				}
				this._lineStartPos -= this._charPos;
				this._charPos = 0;
				this._charsUsed = num2;
			}
		}

		private int ReadData(bool append, int charsRequired)
		{
			if (this._isEndOfFile)
			{
				return 0;
			}
			this.PrepareBufferForReadData(append, charsRequired);
			int count = this._chars.Length - this._charsUsed - 1;
			int num = this._reader.Read(this._chars, this._charsUsed, count);
			this._charsUsed += num;
			if (num == 0)
			{
				this._isEndOfFile = true;
			}
			this._chars[this._charsUsed] = '\0';
			return num;
		}

		private bool EnsureChars(int relativePosition, bool append)
		{
			return this._charPos + relativePosition < this._charsUsed || this.ReadChars(relativePosition, append);
		}

		private bool ReadChars(int relativePosition, bool append)
		{
			if (this._isEndOfFile)
			{
				return false;
			}
			int num = this._charPos + relativePosition - this._charsUsed + 1;
			int num2 = 0;
			do
			{
				int num3 = this.ReadData(append, num - num2);
				if (num3 == 0)
				{
					break;
				}
				num2 += num3;
			}
			while (num2 < num);
			return num2 >= num;
		}

		public override bool Read()
		{
			this.EnsureBuffer();
			for (;;)
			{
				switch (this._currentState)
				{
				case JsonReader.State.Start:
				case JsonReader.State.Property:
				case JsonReader.State.ArrayStart:
				case JsonReader.State.Array:
				case JsonReader.State.ConstructorStart:
				case JsonReader.State.Constructor:
					goto IL_4C;
				case JsonReader.State.ObjectStart:
				case JsonReader.State.Object:
					goto IL_53;
				case JsonReader.State.PostValue:
					if (this.ParsePostValue(false))
					{
						return true;
					}
					continue;
				case JsonReader.State.Finished:
					goto IL_65;
				}
				break;
			}
			goto IL_D1;
			IL_4C:
			return this.ParseValue();
			IL_53:
			return this.ParseObject();
			IL_65:
			if (!this.EnsureChars(0, false))
			{
				base.SetToken(JsonToken.None);
				return false;
			}
			this.EatWhitespace();
			if (this._isEndOfFile)
			{
				base.SetToken(JsonToken.None);
				return false;
			}
			if (this._chars[this._charPos] == '/')
			{
				this.ParseComment(true);
				return true;
			}
			throw JsonReaderException.Create(this, "Additional text encountered after finished reading JSON content: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
			IL_D1:
			throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
		}

		public override int? ReadAsInt32()
		{
			return (int?)this.ReadNumberValue(ReadType.ReadAsInt32);
		}

		public override DateTime? ReadAsDateTime()
		{
			return (DateTime?)this.ReadStringValue(ReadType.ReadAsDateTime);
		}

		[NullableContext(2)]
		public override string ReadAsString()
		{
			return (string)this.ReadStringValue(ReadType.ReadAsString);
		}

		[NullableContext(2)]
		public override byte[] ReadAsBytes()
		{
			this.EnsureBuffer();
			bool flag = false;
			switch (this._currentState)
			{
			case JsonReader.State.Start:
			case JsonReader.State.Property:
			case JsonReader.State.ArrayStart:
			case JsonReader.State.Array:
			case JsonReader.State.ConstructorStart:
			case JsonReader.State.Constructor:
				break;
			case JsonReader.State.Complete:
			case JsonReader.State.ObjectStart:
			case JsonReader.State.Object:
			case JsonReader.State.Closed:
			case JsonReader.State.Error:
				goto IL_23E;
			case JsonReader.State.PostValue:
				if (this.ParsePostValue(true))
				{
					return null;
				}
				break;
			case JsonReader.State.Finished:
				this.ReadFinished();
				return null;
			default:
				goto IL_23E;
			}
			char c;
			for (;;)
			{
				c = this._chars[this._charPos];
				if (c <= '\'')
				{
					if (c <= '\r')
					{
						if (c != '\0')
						{
							switch (c)
							{
							case '\t':
								break;
							case '\n':
								this.ProcessLineFeed();
								continue;
							case '\v':
							case '\f':
								goto IL_215;
							case '\r':
								this.ProcessCarriageReturn(false);
								continue;
							default:
								goto IL_215;
							}
						}
						else
						{
							if (this.ReadNullChar())
							{
								break;
							}
							continue;
						}
					}
					else if (c != ' ')
					{
						if (c != '"' && c != '\'')
						{
							goto IL_215;
						}
						goto IL_FF;
					}
					this._charPos++;
					continue;
				}
				if (c <= '[')
				{
					if (c == ',')
					{
						this.ProcessValueComma();
						continue;
					}
					if (c == '/')
					{
						this.ParseComment(false);
						continue;
					}
					if (c == '[')
					{
						goto IL_175;
					}
				}
				else
				{
					if (c == ']')
					{
						goto IL_1B0;
					}
					if (c == 'n')
					{
						goto IL_191;
					}
					if (c == '{')
					{
						this._charPos++;
						base.SetToken(JsonToken.StartObject);
						base.ReadIntoWrappedTypeObject();
						flag = true;
						continue;
					}
				}
				IL_215:
				this._charPos++;
				if (!char.IsWhiteSpace(c))
				{
					goto Block_22;
				}
			}
			base.SetToken(JsonToken.None, null, false);
			return null;
			IL_FF:
			this.ParseString(c, ReadType.ReadAsBytes);
			byte[] array = (byte[])this.Value;
			if (flag)
			{
				base.ReaderReadAndAssert();
				if (this.TokenType != JsonToken.EndObject)
				{
					throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, this.TokenType));
				}
				base.SetToken(JsonToken.Bytes, array, false);
			}
			return array;
			IL_175:
			this._charPos++;
			base.SetToken(JsonToken.StartArray);
			return base.ReadArrayIntoByteArray();
			IL_191:
			this.HandleNull();
			return null;
			IL_1B0:
			this._charPos++;
			if (this._currentState == JsonReader.State.Array || this._currentState == JsonReader.State.ArrayStart || this._currentState == JsonReader.State.PostValue)
			{
				base.SetToken(JsonToken.EndArray);
				return null;
			}
			throw this.CreateUnexpectedCharacterException(c);
			Block_22:
			throw this.CreateUnexpectedCharacterException(c);
			IL_23E:
			throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
		}

		[NullableContext(2)]
		private object ReadStringValue(ReadType readType)
		{
			this.EnsureBuffer();
			switch (this._currentState)
			{
			case JsonReader.State.Start:
			case JsonReader.State.Property:
			case JsonReader.State.ArrayStart:
			case JsonReader.State.Array:
			case JsonReader.State.ConstructorStart:
			case JsonReader.State.Constructor:
				break;
			case JsonReader.State.Complete:
			case JsonReader.State.ObjectStart:
			case JsonReader.State.Object:
			case JsonReader.State.Closed:
			case JsonReader.State.Error:
				goto IL_2E1;
			case JsonReader.State.PostValue:
				if (this.ParsePostValue(true))
				{
					return null;
				}
				break;
			case JsonReader.State.Finished:
				this.ReadFinished();
				return null;
			default:
				goto IL_2E1;
			}
			char c;
			for (;;)
			{
				c = this._chars[this._charPos];
				if (c <= 'I')
				{
					if (c <= '\r')
					{
						if (c != '\0')
						{
							switch (c)
							{
							case '\t':
								break;
							case '\n':
								this.ProcessLineFeed();
								continue;
							case '\v':
							case '\f':
								goto IL_2B8;
							case '\r':
								this.ProcessCarriageReturn(false);
								continue;
							default:
								goto IL_2B8;
							}
						}
						else
						{
							if (this.ReadNullChar())
							{
								break;
							}
							continue;
						}
					}
					else
					{
						switch (c)
						{
						case ' ':
							break;
						case '!':
						case '#':
						case '$':
						case '%':
						case '&':
						case '(':
						case ')':
						case '*':
						case '+':
							goto IL_2B8;
						case '"':
						case '\'':
							goto IL_165;
						case ',':
							this.ProcessValueComma();
							continue;
						case '-':
							goto IL_175;
						case '.':
						case '0':
						case '1':
						case '2':
						case '3':
						case '4':
						case '5':
						case '6':
						case '7':
						case '8':
						case '9':
							goto IL_1A8;
						case '/':
							this.ParseComment(false);
							continue;
						default:
							if (c != 'I')
							{
								goto IL_2B8;
							}
							goto IL_224;
						}
					}
					this._charPos++;
					continue;
				}
				if (c <= ']')
				{
					if (c == 'N')
					{
						goto IL_22C;
					}
					if (c == ']')
					{
						goto IL_253;
					}
				}
				else
				{
					if (c == 'f')
					{
						goto IL_1D0;
					}
					if (c == 'n')
					{
						goto IL_234;
					}
					if (c == 't')
					{
						goto IL_1D0;
					}
				}
				IL_2B8:
				this._charPos++;
				if (!char.IsWhiteSpace(c))
				{
					goto Block_24;
				}
			}
			base.SetToken(JsonToken.None, null, false);
			return null;
			IL_165:
			this.ParseString(c, readType);
			return this.FinishReadQuotedStringValue(readType);
			IL_175:
			if (this.EnsureChars(1, true) && this._chars[this._charPos + 1] == 'I')
			{
				return this.ParseNumberNegativeInfinity(readType);
			}
			this.ParseNumber(readType);
			return this.Value;
			IL_1A8:
			if (readType != ReadType.ReadAsString)
			{
				this._charPos++;
				throw this.CreateUnexpectedCharacterException(c);
			}
			this.ParseNumber(ReadType.ReadAsString);
			return this.Value;
			IL_1D0:
			if (readType != ReadType.ReadAsString)
			{
				this._charPos++;
				throw this.CreateUnexpectedCharacterException(c);
			}
			string text = (c == 't') ? JsonConvert.True : JsonConvert.False;
			if (!this.MatchValueWithTrailingSeparator(text))
			{
				throw this.CreateUnexpectedCharacterException(this._chars[this._charPos]);
			}
			base.SetToken(JsonToken.String, text);
			return text;
			IL_224:
			return this.ParseNumberPositiveInfinity(readType);
			IL_22C:
			return this.ParseNumberNaN(readType);
			IL_234:
			this.HandleNull();
			return null;
			IL_253:
			this._charPos++;
			if (this._currentState == JsonReader.State.Array || this._currentState == JsonReader.State.ArrayStart || this._currentState == JsonReader.State.PostValue)
			{
				base.SetToken(JsonToken.EndArray);
				return null;
			}
			throw this.CreateUnexpectedCharacterException(c);
			Block_24:
			throw this.CreateUnexpectedCharacterException(c);
			IL_2E1:
			throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
		}

		[NullableContext(2)]
		private object FinishReadQuotedStringValue(ReadType readType)
		{
			switch (readType)
			{
			case ReadType.ReadAsBytes:
			case ReadType.ReadAsString:
				return this.Value;
			case ReadType.ReadAsDateTime:
			{
				object value = this.Value;
				if (value is DateTime)
				{
					DateTime dateTime = (DateTime)value;
					return dateTime;
				}
				return base.ReadDateTimeString((string)this.Value);
			}
			case ReadType.ReadAsDateTimeOffset:
			{
				object value = this.Value;
				if (value is DateTimeOffset)
				{
					DateTimeOffset dateTimeOffset = (DateTimeOffset)value;
					return dateTimeOffset;
				}
				return base.ReadDateTimeOffsetString((string)this.Value);
			}
			}
			throw new ArgumentOutOfRangeException("readType");
		}

		private JsonReaderException CreateUnexpectedCharacterException(char c)
		{
			return JsonReaderException.Create(this, "Unexpected character encountered while parsing value: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
		}

		public override bool? ReadAsBoolean()
		{
			this.EnsureBuffer();
			switch (this._currentState)
			{
			case JsonReader.State.Start:
			case JsonReader.State.Property:
			case JsonReader.State.ArrayStart:
			case JsonReader.State.Array:
			case JsonReader.State.ConstructorStart:
			case JsonReader.State.Constructor:
				break;
			case JsonReader.State.Complete:
			case JsonReader.State.ObjectStart:
			case JsonReader.State.Object:
			case JsonReader.State.Closed:
			case JsonReader.State.Error:
				goto IL_2DF;
			case JsonReader.State.PostValue:
				if (this.ParsePostValue(true))
				{
					return null;
				}
				break;
			case JsonReader.State.Finished:
				this.ReadFinished();
				return null;
			default:
				goto IL_2DF;
			}
			char c;
			for (;;)
			{
				c = this._chars[this._charPos];
				if (c <= '9')
				{
					if (c != '\0')
					{
						switch (c)
						{
						case '\t':
							break;
						case '\n':
							this.ProcessLineFeed();
							continue;
						case '\v':
						case '\f':
							goto IL_2AE;
						case '\r':
							this.ProcessCarriageReturn(false);
							continue;
						default:
							switch (c)
							{
							case ' ':
								break;
							case '!':
							case '#':
							case '$':
							case '%':
							case '&':
							case '(':
							case ')':
							case '*':
							case '+':
								goto IL_2AE;
							case '"':
							case '\'':
								goto IL_158;
							case ',':
								this.ProcessValueComma();
								continue;
							case '-':
							case '.':
							case '0':
							case '1':
							case '2':
							case '3':
							case '4':
							case '5':
							case '6':
							case '7':
							case '8':
							case '9':
								goto IL_188;
							case '/':
								this.ParseComment(false);
								continue;
							default:
								goto IL_2AE;
							}
							break;
						}
						this._charPos++;
						continue;
					}
					if (this.ReadNullChar())
					{
						break;
					}
					continue;
				}
				else if (c <= 'f')
				{
					if (c == ']')
					{
						goto IL_241;
					}
					if (c == 'f')
					{
						goto IL_1DC;
					}
				}
				else
				{
					if (c == 'n')
					{
						goto IL_178;
					}
					if (c == 't')
					{
						goto IL_1DC;
					}
				}
				IL_2AE:
				this._charPos++;
				if (!char.IsWhiteSpace(c))
				{
					goto Block_18;
				}
			}
			base.SetToken(JsonToken.None, null, false);
			return null;
			IL_158:
			this.ParseString(c, ReadType.Read);
			return base.ReadBooleanString(this._stringReference.ToString());
			IL_178:
			this.HandleNull();
			return null;
			IL_188:
			this.ParseNumber(ReadType.Read);
			object value = this.Value;
			bool flag;
			if (value is BigInteger)
			{
				BigInteger left = (BigInteger)value;
				flag = (left != 0L);
			}
			else
			{
				flag = Convert.ToBoolean(this.Value, CultureInfo.InvariantCulture);
			}
			base.SetToken(JsonToken.Boolean, flag, false);
			return new bool?(flag);
			IL_1DC:
			bool flag2 = c == 't';
			string value2 = flag2 ? JsonConvert.True : JsonConvert.False;
			if (!this.MatchValueWithTrailingSeparator(value2))
			{
				throw this.CreateUnexpectedCharacterException(this._chars[this._charPos]);
			}
			base.SetToken(JsonToken.Boolean, BoxedPrimitives.Get(flag2));
			return new bool?(flag2);
			IL_241:
			this._charPos++;
			if (this._currentState == JsonReader.State.Array || this._currentState == JsonReader.State.ArrayStart || this._currentState == JsonReader.State.PostValue)
			{
				base.SetToken(JsonToken.EndArray);
				return null;
			}
			throw this.CreateUnexpectedCharacterException(c);
			Block_18:
			throw this.CreateUnexpectedCharacterException(c);
			IL_2DF:
			throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
		}

		private void ProcessValueComma()
		{
			this._charPos++;
			if (this._currentState != JsonReader.State.PostValue)
			{
				base.SetToken(JsonToken.Undefined);
				JsonReaderException ex = this.CreateUnexpectedCharacterException(',');
				this._charPos--;
				throw ex;
			}
			base.SetStateBasedOnCurrent();
		}

		[NullableContext(2)]
		private object ReadNumberValue(ReadType readType)
		{
			this.EnsureBuffer();
			switch (this._currentState)
			{
			case JsonReader.State.Start:
			case JsonReader.State.Property:
			case JsonReader.State.ArrayStart:
			case JsonReader.State.Array:
			case JsonReader.State.ConstructorStart:
			case JsonReader.State.Constructor:
				break;
			case JsonReader.State.Complete:
			case JsonReader.State.ObjectStart:
			case JsonReader.State.Object:
			case JsonReader.State.Closed:
			case JsonReader.State.Error:
				goto IL_250;
			case JsonReader.State.PostValue:
				if (this.ParsePostValue(true))
				{
					return null;
				}
				break;
			case JsonReader.State.Finished:
				this.ReadFinished();
				return null;
			default:
				goto IL_250;
			}
			char c;
			for (;;)
			{
				c = this._chars[this._charPos];
				if (c <= '9')
				{
					if (c != '\0')
					{
						switch (c)
						{
						case '\t':
							break;
						case '\n':
							this.ProcessLineFeed();
							continue;
						case '\v':
						case '\f':
							goto IL_227;
						case '\r':
							this.ProcessCarriageReturn(false);
							continue;
						default:
							switch (c)
							{
							case ' ':
								break;
							case '!':
							case '#':
							case '$':
							case '%':
							case '&':
							case '(':
							case ')':
							case '*':
							case '+':
								goto IL_227;
							case '"':
							case '\'':
								goto IL_142;
							case ',':
								this.ProcessValueComma();
								continue;
							case '-':
								goto IL_16A;
							case '.':
							case '0':
							case '1':
							case '2':
							case '3':
							case '4':
							case '5':
							case '6':
							case '7':
							case '8':
							case '9':
								goto IL_19D;
							case '/':
								this.ParseComment(false);
								continue;
							default:
								goto IL_227;
							}
							break;
						}
						this._charPos++;
						continue;
					}
					if (this.ReadNullChar())
					{
						break;
					}
					continue;
				}
				else if (c <= 'N')
				{
					if (c == 'I')
					{
						goto IL_162;
					}
					if (c == 'N')
					{
						goto IL_15A;
					}
				}
				else
				{
					if (c == ']')
					{
						goto IL_1C2;
					}
					if (c == 'n')
					{
						goto IL_152;
					}
				}
				IL_227:
				this._charPos++;
				if (!char.IsWhiteSpace(c))
				{
					goto Block_17;
				}
			}
			base.SetToken(JsonToken.None, null, false);
			return null;
			IL_142:
			this.ParseString(c, readType);
			return this.FinishReadQuotedNumber(readType);
			IL_152:
			this.HandleNull();
			return null;
			IL_15A:
			return this.ParseNumberNaN(readType);
			IL_162:
			return this.ParseNumberPositiveInfinity(readType);
			IL_16A:
			if (this.EnsureChars(1, true) && this._chars[this._charPos + 1] == 'I')
			{
				return this.ParseNumberNegativeInfinity(readType);
			}
			this.ParseNumber(readType);
			return this.Value;
			IL_19D:
			this.ParseNumber(readType);
			return this.Value;
			IL_1C2:
			this._charPos++;
			if (this._currentState == JsonReader.State.Array || this._currentState == JsonReader.State.ArrayStart || this._currentState == JsonReader.State.PostValue)
			{
				base.SetToken(JsonToken.EndArray);
				return null;
			}
			throw this.CreateUnexpectedCharacterException(c);
			Block_17:
			throw this.CreateUnexpectedCharacterException(c);
			IL_250:
			throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
		}

		[NullableContext(2)]
		private object FinishReadQuotedNumber(ReadType readType)
		{
			if (readType == ReadType.ReadAsInt32)
			{
				return base.ReadInt32String(this._stringReference.ToString());
			}
			if (readType == ReadType.ReadAsDecimal)
			{
				return base.ReadDecimalString(this._stringReference.ToString());
			}
			if (readType != ReadType.ReadAsDouble)
			{
				throw new ArgumentOutOfRangeException("readType");
			}
			return base.ReadDoubleString(this._stringReference.ToString());
		}

		public override DateTimeOffset? ReadAsDateTimeOffset()
		{
			return (DateTimeOffset?)this.ReadStringValue(ReadType.ReadAsDateTimeOffset);
		}

		public override decimal? ReadAsDecimal()
		{
			return (decimal?)this.ReadNumberValue(ReadType.ReadAsDecimal);
		}

		public override double? ReadAsDouble()
		{
			return (double?)this.ReadNumberValue(ReadType.ReadAsDouble);
		}

		private void HandleNull()
		{
			if (!this.EnsureChars(1, true))
			{
				this._charPos = this._charsUsed;
				throw base.CreateUnexpectedEndException();
			}
			if (this._chars[this._charPos + 1] == 'u')
			{
				this.ParseNull();
				return;
			}
			this._charPos += 2;
			throw this.CreateUnexpectedCharacterException(this._chars[this._charPos - 1]);
		}

		private void ReadFinished()
		{
			if (this.EnsureChars(0, false))
			{
				this.EatWhitespace();
				if (this._isEndOfFile)
				{
					return;
				}
				if (this._chars[this._charPos] != '/')
				{
					throw JsonReaderException.Create(this, "Additional text encountered after finished reading JSON content: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
				}
				this.ParseComment(false);
			}
			base.SetToken(JsonToken.None);
		}

		private bool ReadNullChar()
		{
			if (this._charsUsed == this._charPos)
			{
				if (this.ReadData(false) == 0)
				{
					this._isEndOfFile = true;
					return true;
				}
			}
			else
			{
				this._charPos++;
			}
			return false;
		}

		private void EnsureBuffer()
		{
			if (this._chars == null)
			{
				this._chars = BufferUtils.RentBuffer(this._arrayPool, 1024);
				this._chars[0] = '\0';
			}
		}

		private void ReadStringIntoBuffer(char quote)
		{
			int num = this._charPos;
			int charPos = this._charPos;
			int lastWritePosition = this._charPos;
			this._stringBuffer.Position = 0;
			char c2;
			for (;;)
			{
				char c = this._chars[num++];
				if (c <= '\r')
				{
					if (c != '\0')
					{
						if (c != '\n')
						{
							if (c == '\r')
							{
								this._charPos = num - 1;
								this.ProcessCarriageReturn(true);
								num = this._charPos;
							}
						}
						else
						{
							this._charPos = num - 1;
							this.ProcessLineFeed();
							num = this._charPos;
						}
					}
					else if (this._charsUsed == num - 1)
					{
						num--;
						if (this.ReadData(true) == 0)
						{
							break;
						}
					}
				}
				else if (c != '"' && c != '\'')
				{
					if (c == '\\')
					{
						this._charPos = num;
						if (!this.EnsureChars(0, true))
						{
							goto Block_10;
						}
						int writeToPosition = num - 1;
						c2 = this._chars[num];
						num++;
						char c3;
						if (c2 <= '\\')
						{
							if (c2 <= '\'')
							{
								if (c2 != '"' && c2 != '\'')
								{
									goto Block_14;
								}
							}
							else if (c2 != '/')
							{
								if (c2 != '\\')
								{
									goto Block_16;
								}
								c3 = '\\';
								goto IL_28D;
							}
							c3 = c2;
						}
						else if (c2 <= 'f')
						{
							if (c2 != 'b')
							{
								if (c2 != 'f')
								{
									goto Block_19;
								}
								c3 = '\f';
							}
							else
							{
								c3 = '\b';
							}
						}
						else
						{
							if (c2 != 'n')
							{
								switch (c2)
								{
								case 'r':
									c3 = '\r';
									goto IL_28D;
								case 't':
									c3 = '\t';
									goto IL_28D;
								case 'u':
									this._charPos = num;
									c3 = this.ParseUnicode();
									if (StringUtils.IsLowSurrogate(c3))
									{
										c3 = '�';
									}
									else if (StringUtils.IsHighSurrogate(c3))
									{
										bool flag;
										do
										{
											flag = false;
											if (this.EnsureChars(2, true) && this._chars[this._charPos] == '\\' && this._chars[this._charPos + 1] == 'u')
											{
												char writeChar = c3;
												this._charPos += 2;
												c3 = this.ParseUnicode();
												if (!StringUtils.IsLowSurrogate(c3))
												{
													if (StringUtils.IsHighSurrogate(c3))
													{
														writeChar = '�';
														flag = true;
													}
													else
													{
														writeChar = '�';
													}
												}
												this.EnsureBufferNotEmpty();
												this.WriteCharToBuffer(writeChar, lastWritePosition, writeToPosition);
												lastWritePosition = this._charPos;
											}
											else
											{
												c3 = '�';
											}
										}
										while (flag);
									}
									num = this._charPos;
									goto IL_28D;
								}
								goto Block_21;
							}
							c3 = '\n';
						}
						IL_28D:
						this.EnsureBufferNotEmpty();
						this.WriteCharToBuffer(c3, lastWritePosition, writeToPosition);
						lastWritePosition = num;
					}
				}
				else if (this._chars[num - 1] == quote)
				{
					goto Block_28;
				}
			}
			this._charPos = num;
			throw JsonReaderException.Create(this, "Unterminated string. Expected delimiter: {0}.".FormatWith(CultureInfo.InvariantCulture, quote));
			Block_10:
			throw JsonReaderException.Create(this, "Unterminated string. Expected delimiter: {0}.".FormatWith(CultureInfo.InvariantCulture, quote));
			Block_14:
			Block_16:
			Block_19:
			Block_21:
			this._charPos = num;
			throw JsonReaderException.Create(this, "Bad JSON escape sequence: {0}.".FormatWith(CultureInfo.InvariantCulture, "\\" + c2.ToString()));
			Block_28:
			this.FinishReadStringIntoBuffer(num - 1, charPos, lastWritePosition);
		}

		private void FinishReadStringIntoBuffer(int charPos, int initialPosition, int lastWritePosition)
		{
			if (initialPosition == lastWritePosition)
			{
				this._stringReference = new StringReference(this._chars, initialPosition, charPos - initialPosition);
			}
			else
			{
				this.EnsureBufferNotEmpty();
				if (charPos > lastWritePosition)
				{
					this._stringBuffer.Append(this._arrayPool, this._chars, lastWritePosition, charPos - lastWritePosition);
				}
				this._stringReference = new StringReference(this._stringBuffer.InternalBuffer, 0, this._stringBuffer.Position);
			}
			this._charPos = charPos + 1;
		}

		private void WriteCharToBuffer(char writeChar, int lastWritePosition, int writeToPosition)
		{
			if (writeToPosition > lastWritePosition)
			{
				this._stringBuffer.Append(this._arrayPool, this._chars, lastWritePosition, writeToPosition - lastWritePosition);
			}
			this._stringBuffer.Append(this._arrayPool, writeChar);
		}

		private char ConvertUnicode(bool enoughChars)
		{
			if (!enoughChars)
			{
				throw JsonReaderException.Create(this, "Unexpected end while parsing Unicode escape sequence.");
			}
			int value;
			if (ConvertUtils.TryHexTextToInt(this._chars, this._charPos, this._charPos + 4, out value))
			{
				char result = Convert.ToChar(value);
				this._charPos += 4;
				return result;
			}
			throw JsonReaderException.Create(this, "Invalid Unicode escape sequence: \\u{0}.".FormatWith(CultureInfo.InvariantCulture, new string(this._chars, this._charPos, 4)));
		}

		private char ParseUnicode()
		{
			return this.ConvertUnicode(this.EnsureChars(4, true));
		}

		private void ReadNumberIntoBuffer()
		{
			int num = this._charPos;
			for (;;)
			{
				char c = this._chars[num];
				if (c == '\0')
				{
					this._charPos = num;
					if (this._charsUsed != num)
					{
						return;
					}
					if (this.ReadData(true) == 0)
					{
						break;
					}
				}
				else
				{
					if (this.ReadNumberCharIntoBuffer(c, num))
					{
						return;
					}
					num++;
				}
			}
		}

		private bool ReadNumberCharIntoBuffer(char currentChar, int charPos)
		{
			if (currentChar <= 'X')
			{
				switch (currentChar)
				{
				case '+':
				case '-':
				case '.':
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
				case 'A':
				case 'B':
				case 'C':
				case 'D':
				case 'E':
				case 'F':
					break;
				case ',':
				case '/':
				case ':':
				case ';':
				case '<':
				case '=':
				case '>':
				case '?':
				case '@':
					goto IL_B0;
				default:
					if (currentChar != 'X')
					{
						goto IL_B0;
					}
					break;
				}
			}
			else
			{
				switch (currentChar)
				{
				case 'a':
				case 'b':
				case 'c':
				case 'd':
				case 'e':
				case 'f':
					break;
				default:
					if (currentChar != 'x')
					{
						goto IL_B0;
					}
					break;
				}
			}
			return false;
			IL_B0:
			this._charPos = charPos;
			if (char.IsWhiteSpace(currentChar) || currentChar == ',' || currentChar == '}' || currentChar == ']' || currentChar == ')' || currentChar == '/')
			{
				return true;
			}
			throw JsonReaderException.Create(this, "Unexpected character encountered while parsing number: {0}.".FormatWith(CultureInfo.InvariantCulture, currentChar));
		}

		private void ClearRecentString()
		{
			this._stringBuffer.Position = 0;
			this._stringReference = default(StringReference);
		}

		private bool ParsePostValue(bool ignoreComments)
		{
			char c;
			for (;;)
			{
				c = this._chars[this._charPos];
				if (c <= ')')
				{
					if (c <= '\r')
					{
						if (c != '\0')
						{
							switch (c)
							{
							case '\t':
								break;
							case '\n':
								this.ProcessLineFeed();
								continue;
							case '\v':
							case '\f':
								goto IL_14C;
							case '\r':
								this.ProcessCarriageReturn(false);
								continue;
							default:
								goto IL_14C;
							}
						}
						else
						{
							if (this._charsUsed != this._charPos)
							{
								this._charPos++;
								continue;
							}
							if (this.ReadData(false) == 0)
							{
								break;
							}
							continue;
						}
					}
					else if (c != ' ')
					{
						if (c != ')')
						{
							goto IL_14C;
						}
						goto IL_E2;
					}
					this._charPos++;
					continue;
				}
				if (c <= '/')
				{
					if (c == ',')
					{
						goto IL_10C;
					}
					if (c == '/')
					{
						this.ParseComment(!ignoreComments);
						if (!ignoreComments)
						{
							return true;
						}
						continue;
					}
				}
				else
				{
					if (c == ']')
					{
						goto IL_CA;
					}
					if (c == '}')
					{
						goto IL_B2;
					}
				}
				IL_14C:
				if (!char.IsWhiteSpace(c))
				{
					goto IL_167;
				}
				this._charPos++;
			}
			this._currentState = JsonReader.State.Finished;
			return false;
			IL_B2:
			this._charPos++;
			base.SetToken(JsonToken.EndObject);
			return true;
			IL_CA:
			this._charPos++;
			base.SetToken(JsonToken.EndArray);
			return true;
			IL_E2:
			this._charPos++;
			base.SetToken(JsonToken.EndConstructor);
			return true;
			IL_10C:
			this._charPos++;
			base.SetStateBasedOnCurrent();
			return false;
			IL_167:
			if (base.SupportMultipleContent && this.Depth == 0)
			{
				base.SetStateBasedOnCurrent();
				return false;
			}
			throw JsonReaderException.Create(this, "After parsing a value an unexpected character was encountered: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
		}

		private bool ParseObject()
		{
			for (;;)
			{
				char c = this._chars[this._charPos];
				if (c <= '\r')
				{
					if (c != '\0')
					{
						switch (c)
						{
						case '\t':
							break;
						case '\n':
							this.ProcessLineFeed();
							continue;
						case '\v':
						case '\f':
							goto IL_BD;
						case '\r':
							this.ProcessCarriageReturn(false);
							continue;
						default:
							goto IL_BD;
						}
					}
					else
					{
						if (this._charsUsed != this._charPos)
						{
							this._charPos++;
							continue;
						}
						if (this.ReadData(false) == 0)
						{
							break;
						}
						continue;
					}
				}
				else if (c != ' ')
				{
					if (c == '/')
					{
						goto IL_8A;
					}
					if (c != '}')
					{
						goto IL_BD;
					}
					goto IL_72;
				}
				this._charPos++;
				continue;
				IL_BD:
				if (!char.IsWhiteSpace(c))
				{
					goto IL_D8;
				}
				this._charPos++;
			}
			return false;
			IL_72:
			base.SetToken(JsonToken.EndObject);
			this._charPos++;
			return true;
			IL_8A:
			this.ParseComment(true);
			return true;
			IL_D8:
			return this.ParseProperty();
		}

		private bool ParseProperty()
		{
			char c = this._chars[this._charPos];
			char c2;
			if (c == '"' || c == '\'')
			{
				this._charPos++;
				c2 = c;
				this.ShiftBufferIfNeeded();
				this.ReadStringIntoBuffer(c2);
			}
			else
			{
				if (!this.ValidIdentifierChar(c))
				{
					throw JsonReaderException.Create(this, "Invalid property identifier character: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
				}
				c2 = '\0';
				this.ShiftBufferIfNeeded();
				this.ParseUnquotedProperty();
			}
			string text;
			if (this.PropertyNameTable != null)
			{
				text = this.PropertyNameTable.Get(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length);
				if (text == null)
				{
					text = this._stringReference.ToString();
				}
			}
			else
			{
				text = this._stringReference.ToString();
			}
			this.EatWhitespace();
			if (this._chars[this._charPos] != ':')
			{
				throw JsonReaderException.Create(this, "Invalid character after parsing property name. Expected ':' but got: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
			}
			this._charPos++;
			base.SetToken(JsonToken.PropertyName, text);
			this._quoteChar = c2;
			this.ClearRecentString();
			return true;
		}

		private bool ValidIdentifierChar(char value)
		{
			return char.IsLetterOrDigit(value) || value == '_' || value == '$';
		}

		private void ParseUnquotedProperty()
		{
			int charPos = this._charPos;
			for (;;)
			{
				char c = this._chars[this._charPos];
				if (c == '\0')
				{
					if (this._charsUsed != this._charPos)
					{
						goto IL_3B;
					}
					if (this.ReadData(true) == 0)
					{
						break;
					}
				}
				else if (this.ReadUnquotedPropertyReportIfDone(c, charPos))
				{
					return;
				}
			}
			throw JsonReaderException.Create(this, "Unexpected end while parsing unquoted property name.");
			IL_3B:
			this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos);
		}

		private bool ReadUnquotedPropertyReportIfDone(char currentChar, int initialPosition)
		{
			if (this.ValidIdentifierChar(currentChar))
			{
				this._charPos++;
				return false;
			}
			if (char.IsWhiteSpace(currentChar) || currentChar == ':')
			{
				this._stringReference = new StringReference(this._chars, initialPosition, this._charPos - initialPosition);
				return true;
			}
			throw JsonReaderException.Create(this, "Invalid JavaScript property identifier character: {0}.".FormatWith(CultureInfo.InvariantCulture, currentChar));
		}

		private bool ParseValue()
		{
			char c;
			for (;;)
			{
				c = this._chars[this._charPos];
				if (c <= 'N')
				{
					if (c <= ' ')
					{
						if (c != '\0')
						{
							switch (c)
							{
							case '\t':
								break;
							case '\n':
								this.ProcessLineFeed();
								continue;
							case '\v':
							case '\f':
								goto IL_276;
							case '\r':
								this.ProcessCarriageReturn(false);
								continue;
							default:
								if (c != ' ')
								{
									goto IL_276;
								}
								break;
							}
							this._charPos++;
							continue;
						}
						if (this._charsUsed != this._charPos)
						{
							this._charPos++;
							continue;
						}
						if (this.ReadData(false) == 0)
						{
							break;
						}
						continue;
					}
					else if (c <= '/')
					{
						if (c == '"')
						{
							goto IL_116;
						}
						switch (c)
						{
						case '\'':
							goto IL_116;
						case ')':
							goto IL_234;
						case ',':
							goto IL_22A;
						case '-':
							goto IL_1A3;
						case '/':
							goto IL_1D3;
						}
					}
					else
					{
						if (c == 'I')
						{
							goto IL_199;
						}
						if (c == 'N')
						{
							goto IL_18F;
						}
					}
				}
				else if (c <= 'f')
				{
					if (c == '[')
					{
						goto IL_1FB;
					}
					if (c == ']')
					{
						goto IL_212;
					}
					if (c == 'f')
					{
						goto IL_128;
					}
				}
				else if (c <= 't')
				{
					if (c == 'n')
					{
						goto IL_130;
					}
					if (c == 't')
					{
						goto IL_120;
					}
				}
				else
				{
					if (c == 'u')
					{
						goto IL_1DC;
					}
					if (c == '{')
					{
						goto IL_1E4;
					}
				}
				IL_276:
				if (!char.IsWhiteSpace(c))
				{
					goto IL_291;
				}
				this._charPos++;
			}
			return false;
			IL_116:
			this.ParseString(c, ReadType.Read);
			return true;
			IL_120:
			this.ParseTrue();
			return true;
			IL_128:
			this.ParseFalse();
			return true;
			IL_130:
			if (this.EnsureChars(1, true))
			{
				char c2 = this._chars[this._charPos + 1];
				if (c2 == 'u')
				{
					this.ParseNull();
				}
				else
				{
					if (c2 != 'e')
					{
						throw this.CreateUnexpectedCharacterException(this._chars[this._charPos]);
					}
					this.ParseConstructor();
				}
				return true;
			}
			this._charPos++;
			throw base.CreateUnexpectedEndException();
			IL_18F:
			this.ParseNumberNaN(ReadType.Read);
			return true;
			IL_199:
			this.ParseNumberPositiveInfinity(ReadType.Read);
			return true;
			IL_1A3:
			if (this.EnsureChars(1, true) && this._chars[this._charPos + 1] == 'I')
			{
				this.ParseNumberNegativeInfinity(ReadType.Read);
			}
			else
			{
				this.ParseNumber(ReadType.Read);
			}
			return true;
			IL_1D3:
			this.ParseComment(true);
			return true;
			IL_1DC:
			this.ParseUndefined();
			return true;
			IL_1E4:
			this._charPos++;
			base.SetToken(JsonToken.StartObject);
			return true;
			IL_1FB:
			this._charPos++;
			base.SetToken(JsonToken.StartArray);
			return true;
			IL_212:
			this._charPos++;
			base.SetToken(JsonToken.EndArray);
			return true;
			IL_22A:
			base.SetToken(JsonToken.Undefined);
			return true;
			IL_234:
			this._charPos++;
			base.SetToken(JsonToken.EndConstructor);
			return true;
			IL_291:
			if (char.IsNumber(c) || c == '-' || c == '.')
			{
				this.ParseNumber(ReadType.Read);
				return true;
			}
			throw this.CreateUnexpectedCharacterException(c);
		}

		private void ProcessLineFeed()
		{
			this._charPos++;
			this.OnNewLine(this._charPos);
		}

		private void ProcessCarriageReturn(bool append)
		{
			this._charPos++;
			this.SetNewLine(this.EnsureChars(1, append));
		}

		private void EatWhitespace()
		{
			for (;;)
			{
				char c = this._chars[this._charPos];
				if (c != '\0')
				{
					if (c != '\n')
					{
						if (c != '\r')
						{
							if (c != ' ' && !char.IsWhiteSpace(c))
							{
								return;
							}
							this._charPos++;
						}
						else
						{
							this.ProcessCarriageReturn(false);
						}
					}
					else
					{
						this.ProcessLineFeed();
					}
				}
				else if (this._charsUsed == this._charPos)
				{
					if (this.ReadData(false) == 0)
					{
						break;
					}
				}
				else
				{
					this._charPos++;
				}
			}
		}

		private void ParseConstructor()
		{
			if (!this.MatchValueWithTrailingSeparator("new"))
			{
				throw JsonReaderException.Create(this, "Unexpected content while parsing JSON.");
			}
			this.EatWhitespace();
			int charPos = this._charPos;
			char c;
			for (;;)
			{
				c = this._chars[this._charPos];
				if (c == '\0')
				{
					if (this._charsUsed != this._charPos)
					{
						goto IL_51;
					}
					if (this.ReadData(true) == 0)
					{
						break;
					}
				}
				else
				{
					if (!char.IsLetterOrDigit(c))
					{
						goto IL_83;
					}
					this._charPos++;
				}
			}
			throw JsonReaderException.Create(this, "Unexpected end while parsing constructor.");
			IL_51:
			int charPos2 = this._charPos;
			this._charPos++;
			goto IL_F5;
			IL_83:
			if (c == '\r')
			{
				charPos2 = this._charPos;
				this.ProcessCarriageReturn(true);
			}
			else if (c == '\n')
			{
				charPos2 = this._charPos;
				this.ProcessLineFeed();
			}
			else if (char.IsWhiteSpace(c))
			{
				charPos2 = this._charPos;
				this._charPos++;
			}
			else
			{
				if (c != '(')
				{
					throw JsonReaderException.Create(this, "Unexpected character while parsing constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
				}
				charPos2 = this._charPos;
			}
			IL_F5:
			this._stringReference = new StringReference(this._chars, charPos, charPos2 - charPos);
			string value = this._stringReference.ToString();
			this.EatWhitespace();
			if (this._chars[this._charPos] != '(')
			{
				throw JsonReaderException.Create(this, "Unexpected character while parsing constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
			}
			this._charPos++;
			this.ClearRecentString();
			base.SetToken(JsonToken.StartConstructor, value);
		}

		private void ParseNumber(ReadType readType)
		{
			this.ShiftBufferIfNeeded();
			char firstChar = this._chars[this._charPos];
			int charPos = this._charPos;
			this.ReadNumberIntoBuffer();
			this.ParseReadNumber(readType, firstChar, charPos);
		}

		private void ParseReadNumber(ReadType readType, char firstChar, int initialPosition)
		{
			base.SetPostValueState(true);
			this._stringReference = new StringReference(this._chars, initialPosition, this._charPos - initialPosition);
			bool flag = char.IsDigit(firstChar) && this._stringReference.Length == 1;
			bool flag2 = firstChar == '0' && this._stringReference.Length > 1 && this._stringReference.Chars[this._stringReference.StartIndex + 1] != '.' && this._stringReference.Chars[this._stringReference.StartIndex + 1] != 'e' && this._stringReference.Chars[this._stringReference.StartIndex + 1] != 'E';
			object value;
			JsonToken newToken;
			switch (readType)
			{
			case ReadType.Read:
			case ReadType.ReadAsInt64:
			{
				if (flag)
				{
					value = BoxedPrimitives.Get((long)((ulong)firstChar - 48UL));
					newToken = JsonToken.Integer;
					goto IL_622;
				}
				if (flag2)
				{
					string text = this._stringReference.ToString();
					try
					{
						value = BoxedPrimitives.Get(text.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(text, 16) : Convert.ToInt64(text, 8));
					}
					catch (Exception ex)
					{
						throw this.ThrowReaderError("Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, text), ex);
					}
					newToken = JsonToken.Integer;
					goto IL_622;
				}
				long value2;
				ParseResult parseResult = ConvertUtils.Int64TryParse(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length, out value2);
				if (parseResult == ParseResult.Success)
				{
					value = BoxedPrimitives.Get(value2);
					newToken = JsonToken.Integer;
					goto IL_622;
				}
				if (parseResult != ParseResult.Overflow)
				{
					if (this._floatParseHandling == FloatParseHandling.Decimal)
					{
						decimal value3;
						parseResult = ConvertUtils.DecimalTryParse(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length, out value3);
						if (parseResult != ParseResult.Success)
						{
							throw this.ThrowReaderError("Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()), null);
						}
						value = BoxedPrimitives.Get(value3);
					}
					else
					{
						double value4;
						if (!double.TryParse(this._stringReference.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out value4))
						{
							throw this.ThrowReaderError("Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()), null);
						}
						value = BoxedPrimitives.Get(value4);
					}
					newToken = JsonToken.Float;
					goto IL_622;
				}
				string text2 = this._stringReference.ToString();
				if (text2.Length > 380)
				{
					throw this.ThrowReaderError("JSON integer {0} is too large to parse.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()), null);
				}
				value = JsonTextReader.BigIntegerParse(text2, CultureInfo.InvariantCulture);
				newToken = JsonToken.Integer;
				goto IL_622;
			}
			case ReadType.ReadAsInt32:
				if (flag)
				{
					value = BoxedPrimitives.Get((int)(firstChar - '0'));
				}
				else
				{
					if (flag2)
					{
						string text3 = this._stringReference.ToString();
						try
						{
							value = BoxedPrimitives.Get(text3.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt32(text3, 16) : Convert.ToInt32(text3, 8));
							goto IL_27A;
						}
						catch (Exception ex2)
						{
							throw this.ThrowReaderError("Input string '{0}' is not a valid integer.".FormatWith(CultureInfo.InvariantCulture, text3), ex2);
						}
					}
					int value5;
					ParseResult parseResult2 = ConvertUtils.Int32TryParse(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length, out value5);
					if (parseResult2 == ParseResult.Success)
					{
						value = BoxedPrimitives.Get(value5);
					}
					else
					{
						if (parseResult2 == ParseResult.Overflow)
						{
							throw this.ThrowReaderError("JSON integer {0} is too large or small for an Int32.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()), null);
						}
						throw this.ThrowReaderError("Input string '{0}' is not a valid integer.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()), null);
					}
				}
				IL_27A:
				newToken = JsonToken.Integer;
				goto IL_622;
			case ReadType.ReadAsString:
			{
				string text4 = this._stringReference.ToString();
				if (flag2)
				{
					try
					{
						if (text4.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
						{
							Convert.ToInt64(text4, 16);
						}
						else
						{
							Convert.ToInt64(text4, 8);
						}
						goto IL_170;
					}
					catch (Exception ex3)
					{
						throw this.ThrowReaderError("Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, text4), ex3);
					}
				}
				double num;
				if (!double.TryParse(text4, NumberStyles.Float, CultureInfo.InvariantCulture, out num))
				{
					throw this.ThrowReaderError("Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()), null);
				}
				IL_170:
				newToken = JsonToken.String;
				value = text4;
				goto IL_622;
			}
			case ReadType.ReadAsDecimal:
				if (flag)
				{
					value = BoxedPrimitives.Get(firstChar - 48m);
				}
				else
				{
					if (flag2)
					{
						string text5 = this._stringReference.ToString();
						try
						{
							value = BoxedPrimitives.Get(Convert.ToDecimal(text5.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(text5, 16) : Convert.ToInt64(text5, 8)));
							goto IL_35F;
						}
						catch (Exception ex4)
						{
							throw this.ThrowReaderError("Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, text5), ex4);
						}
					}
					decimal value6;
					if (ConvertUtils.DecimalTryParse(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length, out value6) != ParseResult.Success)
					{
						throw this.ThrowReaderError("Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()), null);
					}
					value = BoxedPrimitives.Get(value6);
				}
				IL_35F:
				newToken = JsonToken.Float;
				goto IL_622;
			case ReadType.ReadAsDouble:
				if (flag)
				{
					value = BoxedPrimitives.Get((double)firstChar - 48.0);
				}
				else
				{
					if (flag2)
					{
						string text6 = this._stringReference.ToString();
						try
						{
							value = BoxedPrimitives.Get(Convert.ToDouble(text6.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(text6, 16) : Convert.ToInt64(text6, 8)));
							goto IL_437;
						}
						catch (Exception ex5)
						{
							throw this.ThrowReaderError("Input string '{0}' is not a valid double.".FormatWith(CultureInfo.InvariantCulture, text6), ex5);
						}
					}
					double value7;
					if (!double.TryParse(this._stringReference.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out value7))
					{
						throw this.ThrowReaderError("Input string '{0}' is not a valid double.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()), null);
					}
					value = BoxedPrimitives.Get(value7);
				}
				IL_437:
				newToken = JsonToken.Float;
				goto IL_622;
			}
			throw JsonReaderException.Create(this, "Cannot read number value as type.");
			IL_622:
			this.ClearRecentString();
			base.SetToken(newToken, value, false);
		}

		private JsonReaderException ThrowReaderError(string message, [Nullable(2)] Exception ex = null)
		{
			base.SetToken(JsonToken.Undefined, null, false);
			return JsonReaderException.Create(this, message, ex);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static object BigIntegerParse(string number, CultureInfo culture)
		{
			return BigInteger.Parse(number, culture);
		}

		private void ParseComment(bool setToken)
		{
			this._charPos++;
			if (!this.EnsureChars(1, false))
			{
				throw JsonReaderException.Create(this, "Unexpected end while parsing comment.");
			}
			bool flag;
			if (this._chars[this._charPos] == '*')
			{
				flag = false;
			}
			else
			{
				if (this._chars[this._charPos] != '/')
				{
					throw JsonReaderException.Create(this, "Error parsing comment. Expected: *, got {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
				}
				flag = true;
			}
			this._charPos++;
			int charPos = this._charPos;
			for (;;)
			{
				char c = this._chars[this._charPos];
				if (c <= '\n')
				{
					if (c != '\0')
					{
						if (c == '\n')
						{
							if (flag)
							{
								goto Block_16;
							}
							this.ProcessLineFeed();
							continue;
						}
					}
					else
					{
						if (this._charsUsed != this._charPos)
						{
							this._charPos++;
							continue;
						}
						if (this.ReadData(true) == 0)
						{
							break;
						}
						continue;
					}
				}
				else if (c != '\r')
				{
					if (c == '*')
					{
						this._charPos++;
						if (!flag && this.EnsureChars(0, true) && this._chars[this._charPos] == '/')
						{
							goto Block_14;
						}
						continue;
					}
				}
				else
				{
					if (flag)
					{
						goto Block_15;
					}
					this.ProcessCarriageReturn(true);
					continue;
				}
				this._charPos++;
			}
			if (!flag)
			{
				throw JsonReaderException.Create(this, "Unexpected end while parsing comment.");
			}
			this.EndComment(setToken, charPos, this._charPos);
			return;
			Block_14:
			this.EndComment(setToken, charPos, this._charPos - 1);
			this._charPos++;
			return;
			Block_15:
			this.EndComment(setToken, charPos, this._charPos);
			return;
			Block_16:
			this.EndComment(setToken, charPos, this._charPos);
		}

		private void EndComment(bool setToken, int initialPosition, int endPosition)
		{
			if (setToken)
			{
				base.SetToken(JsonToken.Comment, new string(this._chars, initialPosition, endPosition - initialPosition));
			}
		}

		private bool MatchValue(string value)
		{
			return this.MatchValue(this.EnsureChars(value.Length - 1, true), value);
		}

		private bool MatchValue(bool enoughChars, string value)
		{
			if (!enoughChars)
			{
				this._charPos = this._charsUsed;
				throw base.CreateUnexpectedEndException();
			}
			for (int i = 0; i < value.Length; i++)
			{
				if (this._chars[this._charPos + i] != value[i])
				{
					this._charPos += i;
					return false;
				}
			}
			this._charPos += value.Length;
			return true;
		}

		private bool MatchValueWithTrailingSeparator(string value)
		{
			return this.MatchValue(value) && (!this.EnsureChars(0, false) || this.IsSeparator(this._chars[this._charPos]) || this._chars[this._charPos] == '\0');
		}

		private bool IsSeparator(char c)
		{
			if (c <= ')')
			{
				switch (c)
				{
				case '\t':
				case '\n':
				case '\r':
					break;
				case '\v':
				case '\f':
					goto IL_8C;
				default:
					if (c != ' ')
					{
						if (c != ')')
						{
							goto IL_8C;
						}
						if (base.CurrentState == JsonReader.State.Constructor || base.CurrentState == JsonReader.State.ConstructorStart)
						{
							return true;
						}
						return false;
					}
					break;
				}
				return true;
			}
			if (c <= '/')
			{
				if (c != ',')
				{
					if (c != '/')
					{
						goto IL_8C;
					}
					if (!this.EnsureChars(1, false))
					{
						return false;
					}
					char c2 = this._chars[this._charPos + 1];
					return c2 == '*' || c2 == '/';
				}
			}
			else if (c != ']' && c != '}')
			{
				goto IL_8C;
			}
			return true;
			IL_8C:
			if (char.IsWhiteSpace(c))
			{
				return true;
			}
			return false;
		}

		private void ParseTrue()
		{
			if (this.MatchValueWithTrailingSeparator(JsonConvert.True))
			{
				base.SetToken(JsonToken.Boolean, BoxedPrimitives.BooleanTrue);
				return;
			}
			throw JsonReaderException.Create(this, "Error parsing boolean value.");
		}

		private void ParseNull()
		{
			if (this.MatchValueWithTrailingSeparator(JsonConvert.Null))
			{
				base.SetToken(JsonToken.Null);
				return;
			}
			throw JsonReaderException.Create(this, "Error parsing null value.");
		}

		private void ParseUndefined()
		{
			if (this.MatchValueWithTrailingSeparator(JsonConvert.Undefined))
			{
				base.SetToken(JsonToken.Undefined);
				return;
			}
			throw JsonReaderException.Create(this, "Error parsing undefined value.");
		}

		private void ParseFalse()
		{
			if (this.MatchValueWithTrailingSeparator(JsonConvert.False))
			{
				base.SetToken(JsonToken.Boolean, BoxedPrimitives.BooleanFalse);
				return;
			}
			throw JsonReaderException.Create(this, "Error parsing boolean value.");
		}

		private object ParseNumberNegativeInfinity(ReadType readType)
		{
			return this.ParseNumberNegativeInfinity(readType, this.MatchValueWithTrailingSeparator(JsonConvert.NegativeInfinity));
		}

		private object ParseNumberNegativeInfinity(ReadType readType, bool matched)
		{
			if (matched)
			{
				if (readType != ReadType.Read)
				{
					if (readType == ReadType.ReadAsString)
					{
						base.SetToken(JsonToken.String, JsonConvert.NegativeInfinity);
						return JsonConvert.NegativeInfinity;
					}
					if (readType != ReadType.ReadAsDouble)
					{
						goto IL_44;
					}
				}
				if (this._floatParseHandling == FloatParseHandling.Double)
				{
					base.SetToken(JsonToken.Float, BoxedPrimitives.DoubleNegativeInfinity);
					return double.NegativeInfinity;
				}
				IL_44:
				throw JsonReaderException.Create(this, "Cannot read -Infinity value.");
			}
			throw JsonReaderException.Create(this, "Error parsing -Infinity value.");
		}

		private object ParseNumberPositiveInfinity(ReadType readType)
		{
			return this.ParseNumberPositiveInfinity(readType, this.MatchValueWithTrailingSeparator(JsonConvert.PositiveInfinity));
		}

		private object ParseNumberPositiveInfinity(ReadType readType, bool matched)
		{
			if (matched)
			{
				if (readType != ReadType.Read)
				{
					if (readType == ReadType.ReadAsString)
					{
						base.SetToken(JsonToken.String, JsonConvert.PositiveInfinity);
						return JsonConvert.PositiveInfinity;
					}
					if (readType != ReadType.ReadAsDouble)
					{
						goto IL_44;
					}
				}
				if (this._floatParseHandling == FloatParseHandling.Double)
				{
					base.SetToken(JsonToken.Float, BoxedPrimitives.DoublePositiveInfinity);
					return double.PositiveInfinity;
				}
				IL_44:
				throw JsonReaderException.Create(this, "Cannot read Infinity value.");
			}
			throw JsonReaderException.Create(this, "Error parsing Infinity value.");
		}

		private object ParseNumberNaN(ReadType readType)
		{
			return this.ParseNumberNaN(readType, this.MatchValueWithTrailingSeparator(JsonConvert.NaN));
		}

		private object ParseNumberNaN(ReadType readType, bool matched)
		{
			if (matched)
			{
				if (readType != ReadType.Read)
				{
					if (readType == ReadType.ReadAsString)
					{
						base.SetToken(JsonToken.String, JsonConvert.NaN);
						return JsonConvert.NaN;
					}
					if (readType != ReadType.ReadAsDouble)
					{
						goto IL_44;
					}
				}
				if (this._floatParseHandling == FloatParseHandling.Double)
				{
					base.SetToken(JsonToken.Float, BoxedPrimitives.DoubleNaN);
					return double.NaN;
				}
				IL_44:
				throw JsonReaderException.Create(this, "Cannot read NaN value.");
			}
			throw JsonReaderException.Create(this, "Error parsing NaN value.");
		}

		public override void Close()
		{
			base.Close();
			if (this._chars != null)
			{
				BufferUtils.ReturnBuffer(this._arrayPool, this._chars);
				this._chars = null;
			}
			if (base.CloseInput)
			{
				TextReader reader = this._reader;
				if (reader != null)
				{
					reader.Close();
				}
			}
			this._stringBuffer.Clear(this._arrayPool);
		}

		public bool HasLineInfo()
		{
			return true;
		}

		public int LineNumber
		{
			get
			{
				if (base.CurrentState == JsonReader.State.Start && this.LinePosition == 0 && this.TokenType != JsonToken.Comment)
				{
					return 0;
				}
				return this._lineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				return this._charPos - this._lineStartPos;
			}
		}

		private readonly bool _safeAsync;

		private const char UnicodeReplacementChar = '�';

		private const int MaximumJavascriptIntegerCharacterLength = 380;

		private const int LargeBufferLength = 1073741823;

		private readonly TextReader _reader;

		[Nullable(2)]
		private char[] _chars;

		private int _charsUsed;

		private int _charPos;

		private int _lineStartPos;

		private int _lineNumber;

		private bool _isEndOfFile;

		private StringBuffer _stringBuffer;

		private StringReference _stringReference;

		[Nullable(2)]
		private IArrayPool<char> _arrayPool;
	}
}
