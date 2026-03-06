using System;
using System.Buffers;
using System.Buffers.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using VYaml.Internal;

namespace VYaml.Emitter
{
	[NullableContext(1)]
	[Nullable(0)]
	public ref struct Utf8YamlEmitter
	{
		private unsafe EmitState CurrentState
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				ExpandBuffer<EmitState> expandBuffer = this.stateStack;
				return *expandBuffer[expandBuffer.Length - 1];
			}
		}

		private unsafe EmitState PreviousState
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				ExpandBuffer<EmitState> expandBuffer = this.stateStack;
				return *expandBuffer[expandBuffer.Length - 2];
			}
		}

		private bool IsFirstElement
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.currentElementCount <= 0;
			}
		}

		public Utf8YamlEmitter(IBufferWriter<byte> writer, [Nullable(2)] YamlEmitOptions options = null)
		{
			this.writer = writer;
			this.options = (options ?? YamlEmitOptions.Default);
			this.currentIndentLevel = 0;
			ExpandBuffer<char> expandBuffer;
			if ((expandBuffer = Utf8YamlEmitter.stringBufferStatic) == null)
			{
				expandBuffer = (Utf8YamlEmitter.stringBufferStatic = new ExpandBuffer<char>(1024));
			}
			this.stringBuffer = expandBuffer;
			this.stringBuffer.Clear();
			ExpandBuffer<EmitState> expandBuffer2;
			if ((expandBuffer2 = Utf8YamlEmitter.stateBufferStatic) == null)
			{
				expandBuffer2 = (Utf8YamlEmitter.stateBufferStatic = new ExpandBuffer<EmitState>(16));
			}
			this.stateStack = expandBuffer2;
			this.stateStack.Clear();
			ExpandBuffer<int> expandBuffer3;
			if ((expandBuffer3 = Utf8YamlEmitter.elementCountBufferSTatic) == null)
			{
				expandBuffer3 = (Utf8YamlEmitter.elementCountBufferSTatic = new ExpandBuffer<int>(16));
			}
			this.elementCountStack = expandBuffer3;
			this.elementCountStack.Clear();
			this.stateStack.Add(EmitState.None);
			this.currentElementCount = 0;
			this.tagStack = new ExpandBuffer<string>(4);
		}

		internal readonly IBufferWriter<byte> GetWriter()
		{
			return this.writer;
		}

		public unsafe void BeginSequence(SequenceStyle style = SequenceStyle.Block)
		{
			if (style == SequenceStyle.Block)
			{
				switch (this.CurrentState)
				{
				case EmitState.BlockSequenceEntry:
					this.WriteBlockSequenceEntryHeader();
					break;
				case EmitState.BlockMappingKey:
					throw new YamlEmitterException("To start block-sequence in the mapping key is not supported.");
				case EmitState.FlowSequenceEntry:
					throw new YamlEmitterException("To start block-sequence in the flow-sequence is not supported.");
				}
				this.PushState(EmitState.BlockSequenceEntry);
				return;
			}
			if (style != SequenceStyle.Flow)
			{
				throw new ArgumentOutOfRangeException("style", style, null);
			}
			switch (this.CurrentState)
			{
			case EmitState.BlockSequenceEntry:
			{
				Span<byte> span = this.writer.GetSpan(this.currentIndentLevel * this.options.IndentWidth + Utf8YamlEmitter.BlockSequenceEntryHeader.Length + 1);
				int count = 0;
				this.WriteBlockSequenceEntryHeader(span, ref count);
				*span[count++] = 91;
				this.writer.Advance(count);
				goto IL_180;
			}
			case EmitState.BlockMappingKey:
				throw new YamlEmitterException("To start flow-sequence in the mapping key is not supported.");
			case EmitState.FlowSequenceEntry:
			{
				Span<byte> span2 = this.writer.GetSpan(Utf8YamlEmitter.FlowSequenceSeparator.Length + 1);
				int num = 0;
				if (this.currentElementCount > 0)
				{
					Utf8YamlEmitter.FlowSequenceSeparator.CopyTo(span2);
					num += Utf8YamlEmitter.FlowSequenceSeparator.Length;
				}
				*span2[num++] = 91;
				this.writer.Advance(num);
				goto IL_180;
			}
			}
			Span<byte> span3 = this.writer.GetSpan(this.GetTagLength() + 2);
			int count2 = 0;
			if (this.TryWriteTag(span3, ref count2))
			{
				*span3[count2++] = 32;
			}
			*span3[count2++] = 91;
			this.writer.Advance(count2);
			IL_180:
			this.PushState(EmitState.FlowSequenceEntry);
		}

		public unsafe void EndSequence()
		{
			EmitState currentState = this.CurrentState;
			if (currentState != EmitState.BlockSequenceEntry)
			{
				if (currentState != EmitState.FlowSequenceEntry)
				{
					throw new YamlEmitterException(string.Format("Current state is not sequence: {0}", this.CurrentState));
				}
				this.PopState();
				bool flag = false;
				switch (this.CurrentState)
				{
				case EmitState.BlockSequenceEntry:
					flag = true;
					this.currentElementCount++;
					break;
				case EmitState.BlockMappingValue:
					this.ReplaceCurrentState(EmitState.BlockMappingKey);
					flag = true;
					this.currentElementCount++;
					break;
				case EmitState.FlowSequenceEntry:
					this.currentElementCount++;
					break;
				case EmitState.FlowMappingValue:
					this.ReplaceCurrentState(EmitState.FlowMappingKey);
					this.currentElementCount++;
					break;
				}
				int num = 1;
				if (flag)
				{
					num++;
				}
				int count = 0;
				Span<byte> span = this.writer.GetSpan(num);
				*span[count++] = 93;
				if (flag)
				{
					*span[count++] = 10;
				}
				this.writer.Advance(count);
				return;
			}
			else
			{
				bool flag2 = this.currentElementCount <= 0;
				this.PopState();
				if (flag2)
				{
					EmitState currentState2 = this.CurrentState;
					bool lineBreak = currentState2 == EmitState.BlockSequenceEntry || currentState2 == EmitState.BlockMappingValue;
					this.WriteRaw(Utf8YamlEmitter.FlowSequenceEmpty, false, lineBreak);
				}
				switch (this.CurrentState)
				{
				case EmitState.BlockSequenceEntry:
					if (!flag2)
					{
						this.DecreaseIndent();
					}
					this.currentElementCount++;
					return;
				case EmitState.BlockMappingKey:
					throw new YamlEmitterException("Complex key is not supported.");
				case EmitState.BlockMappingValue:
					this.ReplaceCurrentState(EmitState.BlockMappingKey);
					this.currentElementCount++;
					return;
				case EmitState.FlowSequenceEntry:
					this.currentElementCount++;
					return;
				default:
					return;
				}
			}
		}

		public unsafe void BeginMapping(MappingStyle style = MappingStyle.Block)
		{
			if (style == MappingStyle.Block)
			{
				switch (this.CurrentState)
				{
				case EmitState.BlockSequenceEntry:
					this.WriteBlockSequenceEntryHeader();
					break;
				case EmitState.BlockMappingKey:
					throw new YamlEmitterException("To start block-mapping in the mapping key is not supported.");
				case EmitState.FlowSequenceEntry:
					throw new YamlEmitterException("Cannot start block-mapping in the flow-sequence");
				}
				this.PushState(EmitState.BlockMappingKey);
				return;
			}
			if (style != MappingStyle.Flow)
			{
				throw new ArgumentOutOfRangeException("style", style, null);
			}
			switch (this.CurrentState)
			{
			case EmitState.BlockSequenceEntry:
			{
				Span<byte> span = this.writer.GetSpan(this.currentIndentLevel * this.options.IndentWidth + Utf8YamlEmitter.BlockSequenceEntryHeader.Length + Utf8YamlEmitter.FlowMappingHeader.Length + this.GetTagLength() + 1);
				int count = 0;
				this.WriteBlockSequenceEntryHeader(span, ref count);
				if (this.TryWriteTag(span, ref count))
				{
					*span[count++] = 32;
				}
				*span[count++] = 123;
				this.writer.Advance(count);
				goto IL_1AC;
			}
			case EmitState.BlockMappingKey:
				throw new YamlEmitterException("To start flow-mapping in the mapping key is not supported.");
			case EmitState.FlowSequenceEntry:
			{
				Span<byte> span2 = this.writer.GetSpan(Utf8YamlEmitter.FlowSequenceSeparator.Length + Utf8YamlEmitter.FlowMappingHeader.Length);
				int num = 0;
				if (!this.IsFirstElement)
				{
					Utf8YamlEmitter.FlowSequenceSeparator.CopyTo(span2);
					num += Utf8YamlEmitter.FlowSequenceSeparator.Length;
				}
				*span2[num++] = 123;
				this.writer.Advance(num);
				goto IL_1AC;
			}
			}
			Span<byte> span3 = this.writer.GetSpan(this.GetTagLength() + 2);
			int count2 = 0;
			if (this.TryWriteTag(span3, ref count2))
			{
				*span3[count2++] = 32;
			}
			*span3[count2++] = 123;
			this.writer.Advance(count2);
			IL_1AC:
			this.PushState(EmitState.FlowMappingKey);
		}

		public unsafe void EndMapping()
		{
			EmitState currentState = this.CurrentState;
			if (currentState != EmitState.BlockMappingKey)
			{
				if (currentState != EmitState.FlowMappingKey)
				{
					throw new YamlEmitterException(string.Format("Invalid mapping end: {0}", this.CurrentState));
				}
				bool flag = this.currentElementCount <= 0;
				this.PopState();
				bool flag2 = false;
				switch (this.CurrentState)
				{
				case EmitState.BlockSequenceEntry:
					flag2 = true;
					this.currentElementCount++;
					break;
				case EmitState.BlockMappingValue:
					this.ReplaceCurrentState(EmitState.BlockMappingKey);
					flag2 = true;
					this.currentElementCount++;
					break;
				case EmitState.FlowSequenceEntry:
					this.currentElementCount++;
					break;
				case EmitState.FlowMappingValue:
					this.ReplaceCurrentState(EmitState.FlowMappingKey);
					this.currentElementCount++;
					break;
				}
				int num = Utf8YamlEmitter.FlowMappingFooter.Length;
				if (flag2)
				{
					num++;
				}
				int count = 0;
				Span<byte> span = this.writer.GetSpan(num);
				if (!flag)
				{
					*span[count++] = 32;
				}
				*span[count++] = 125;
				if (flag2)
				{
					*span[count++] = 10;
				}
				this.writer.Advance(count);
				return;
			}
			else
			{
				bool flag3 = this.currentElementCount <= 0;
				this.PopState();
				EmitState currentState2;
				if (flag3)
				{
					currentState2 = this.CurrentState;
					bool lineBreak = currentState2 == EmitState.BlockSequenceEntry || currentState2 == EmitState.BlockMappingValue;
					string str;
					if (this.tagStack.TryPop(out str))
					{
						byte[] bytes = StringEncoding.Utf8.GetBytes(str + " ");
						this.WriteRaw(bytes, Utf8YamlEmitter.FlowMappingEmpty, false, lineBreak);
					}
					else
					{
						this.WriteRaw(Utf8YamlEmitter.FlowMappingEmpty, false, lineBreak);
					}
				}
				currentState2 = this.CurrentState;
				if (currentState2 == EmitState.BlockSequenceEntry)
				{
					if (!flag3)
					{
						this.DecreaseIndent();
					}
					this.currentElementCount++;
					return;
				}
				if (currentState2 != EmitState.BlockMappingValue)
				{
					return;
				}
				if (!flag3)
				{
					this.DecreaseIndent();
				}
				this.ReplaceCurrentState(EmitState.BlockMappingKey);
				this.currentElementCount++;
				return;
			}
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void WriteRaw(ReadOnlySpan<byte> value, bool indent, bool lineBreak)
		{
			int num = value.Length + (indent ? (this.currentIndentLevel * this.options.IndentWidth) : 0) + (lineBreak ? 1 : 0);
			int num2 = 0;
			Span<byte> span = this.writer.GetSpan(num);
			if (indent)
			{
				this.WriteIndent(span, ref num2, -1);
			}
			Span<byte> span2 = span;
			int num3 = num2;
			value.CopyTo(span2.Slice(num3, span2.Length - num3));
			if (lineBreak)
			{
				*span[num - 1] = 10;
			}
			this.writer.Advance(num);
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void WriteRaw(ReadOnlySpan<byte> value1, ReadOnlySpan<byte> value2, bool indent, bool lineBreak)
		{
			int num = value1.Length + value2.Length + (indent ? (this.currentIndentLevel * this.options.IndentWidth) : 0) + (lineBreak ? 1 : 0);
			int num2 = 0;
			Span<byte> span = this.writer.GetSpan(num);
			if (indent)
			{
				this.WriteIndent(span, ref num2, -1);
			}
			Span<byte> span2 = span;
			int num3 = num2;
			value1.CopyTo(span2.Slice(num3, span2.Length - num3));
			num2 += value1.Length;
			span2 = span;
			num3 = num2;
			value2.CopyTo(span2.Slice(num3, span2.Length - num3));
			if (lineBreak)
			{
				*span[num - 1] = 10;
			}
			this.writer.Advance(num);
		}

		public void Tag(string value)
		{
			this.tagStack.Add(value);
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteScalar(ReadOnlySpan<byte> value)
		{
			int num = 0;
			Span<byte> span = this.writer.GetSpan(this.CalculateMaxScalarBufferLength(value.Length));
			this.BeginScalar(span, ref num);
			Span<byte> span2 = span;
			int num2 = num;
			value.CopyTo(span2.Slice(num2, span2.Length - num2));
			num += value.Length;
			this.EndScalar(span, ref num);
			this.writer.Advance(num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteNull()
		{
			this.WriteScalar(YamlCodes.Null0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteBool(bool value)
		{
			this.WriteScalar(value ? YamlCodes.True0 : YamlCodes.False0);
		}

		public void WriteInt32(int value)
		{
			int num = 0;
			Span<byte> span = this.writer.GetSpan(this.CalculateMaxScalarBufferLength(11));
			this.BeginScalar(span, ref num);
			Span<byte> span2 = span;
			int num2 = num;
			int num3;
			if (!Utf8Formatter.TryFormat(value, span2.Slice(num2, span2.Length - num2), out num3, default(StandardFormat)))
			{
				throw new YamlEmitterException(string.Format("Failed to emit : {0}", value));
			}
			num += num3;
			this.EndScalar(span, ref num);
			this.writer.Advance(num);
		}

		public void WriteUInt32(uint value)
		{
			int num = 0;
			Span<byte> span = this.writer.GetSpan(this.CalculateMaxScalarBufferLength(10));
			this.BeginScalar(span, ref num);
			Span<byte> span2 = span;
			int num2 = num;
			int num3;
			if (!Utf8Formatter.TryFormat(value, span2.Slice(num2, span2.Length - num2), out num3, default(StandardFormat)))
			{
				throw new YamlEmitterException(string.Format("Failed to emit : {0}", value));
			}
			num += num3;
			this.EndScalar(span, ref num);
			this.writer.Advance(num);
		}

		public void WriteInt64(long value)
		{
			int num = 0;
			Span<byte> span = this.writer.GetSpan(this.CalculateMaxScalarBufferLength(20));
			this.BeginScalar(span, ref num);
			Span<byte> span2 = span;
			int num2 = num;
			int num3;
			if (!Utf8Formatter.TryFormat(value, span2.Slice(num2, span2.Length - num2), out num3, default(StandardFormat)))
			{
				throw new YamlEmitterException(string.Format("Failed to emit : {0}", value));
			}
			num += num3;
			this.EndScalar(span, ref num);
			this.writer.Advance(num);
		}

		public void WriteUInt64(ulong value)
		{
			int num = 0;
			Span<byte> span = this.writer.GetSpan(this.CalculateMaxScalarBufferLength(20));
			this.BeginScalar(span, ref num);
			Span<byte> span2 = span;
			int num2 = num;
			int num3;
			if (!Utf8Formatter.TryFormat(value, span2.Slice(num2, span2.Length - num2), out num3, default(StandardFormat)))
			{
				throw new YamlEmitterException(string.Format("Failed to emit : {0}", value));
			}
			num += num3;
			this.EndScalar(span, ref num);
			this.writer.Advance(num);
		}

		public void WriteFloat(float value)
		{
			int num = 0;
			Span<byte> span = this.writer.GetSpan(this.CalculateMaxScalarBufferLength(12));
			this.BeginScalar(span, ref num);
			Span<byte> span2 = span;
			int num2 = num;
			int num3;
			if (!Utf8Formatter.TryFormat(value, span2.Slice(num2, span2.Length - num2), out num3, default(StandardFormat)))
			{
				throw new YamlEmitterException(string.Format("Failed to emit : {0}", value));
			}
			num += num3;
			this.EndScalar(span, ref num);
			this.writer.Advance(num);
		}

		public void WriteDouble(double value)
		{
			int num = 0;
			Span<byte> span = this.writer.GetSpan(this.CalculateMaxScalarBufferLength(17));
			this.BeginScalar(span, ref num);
			Span<byte> span2 = span;
			int num2 = num;
			int num3;
			if (!Utf8Formatter.TryFormat(value, span2.Slice(num2, span2.Length - num2), out num3, default(StandardFormat)))
			{
				throw new YamlEmitterException(string.Format("Failed to emit : {0}", value));
			}
			num += num3;
			this.EndScalar(span, ref num);
			this.writer.Advance(num);
		}

		public void WriteString(string value, ScalarStyle style = ScalarStyle.Any)
		{
			if (style == ScalarStyle.Any)
			{
				style = EmitStringAnalyzer.Analyze(value).SuggestScalarStyle();
			}
			switch (style)
			{
			case ScalarStyle.Plain:
				this.WritePlainScalar(value);
				return;
			case ScalarStyle.SingleQuoted:
				this.WriteQuotedScalar(value, false);
				return;
			case ScalarStyle.DoubleQuoted:
				this.WriteQuotedScalar(value, true);
				return;
			case ScalarStyle.Literal:
				this.WriteLiteralScalar(value);
				return;
			case ScalarStyle.Folded:
				throw new NotSupportedException();
			default:
				throw new ArgumentOutOfRangeException("style", style, null);
			}
		}

		private void WritePlainScalar(string value)
		{
			int maxByteCount = StringEncoding.Utf8.GetMaxByteCount(value.Length);
			Span<byte> span = this.writer.GetSpan(this.CalculateMaxScalarBufferLength(maxByteCount));
			int num = 0;
			this.BeginScalar(span, ref num);
			int num2 = num;
			Encoding utf = StringEncoding.Utf8;
			ReadOnlySpan<char> chars = value;
			Span<byte> span2 = span;
			int num3 = num;
			num = num2 + utf.GetBytes(chars, span2.Slice(num3, span2.Length - num3));
			this.EndScalar(span, ref num);
			this.writer.Advance(num);
		}

		private void WriteLiteralScalar(string value)
		{
			int indentCharCount = (this.currentIndentLevel + 1) * this.options.IndentWidth;
			StringBuilder stringBuilder = EmitStringAnalyzer.BuildLiteralScalar(value, indentCharCount);
			Span<char> span = this.stringBuffer.AsSpan(stringBuilder.Length);
			stringBuilder.CopyTo(0, span, stringBuilder.Length);
			EmitState currentState = this.CurrentState;
			if (currentState == EmitState.BlockMappingValue || currentState == EmitState.BlockSequenceEntry)
			{
				Span<char> span2 = span;
				span = span2.Slice(0, span2.Length - 1);
			}
			int maxByteCount = StringEncoding.Utf8.GetMaxByteCount(span.Length);
			int num = 0;
			Span<byte> span3 = this.writer.GetSpan(this.CalculateMaxScalarBufferLength(maxByteCount));
			this.BeginScalar(span3, ref num);
			int num2 = num;
			Encoding utf = StringEncoding.Utf8;
			ReadOnlySpan<char> chars = span;
			Span<byte> span4 = span3;
			int num3 = num;
			num = num2 + utf.GetBytes(chars, span4.Slice(num3, span4.Length - num3));
			this.EndScalar(span3, ref num);
			this.writer.Advance(num);
		}

		private void WriteQuotedScalar(string value, bool doubleQuote = true)
		{
			StringBuilder stringBuilder = EmitStringAnalyzer.BuildQuotedScalar(value, doubleQuote);
			Span<char> span = this.stringBuffer.AsSpan(stringBuilder.Length);
			stringBuilder.CopyTo(0, span, stringBuilder.Length);
			int maxByteCount = StringEncoding.Utf8.GetMaxByteCount(span.Length);
			int num = 0;
			Span<byte> span2 = this.writer.GetSpan(this.CalculateMaxScalarBufferLength(maxByteCount));
			this.BeginScalar(span2, ref num);
			int num2 = num;
			Encoding utf = StringEncoding.Utf8;
			ReadOnlySpan<char> chars = span;
			Span<byte> span3 = span2;
			int num3 = num;
			num = num2 + utf.GetBytes(chars, span3.Slice(num3, span3.Length - num3));
			this.EndScalar(span2, ref num);
			this.writer.Advance(num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void WriteRaw1(byte value)
		{
			*this.writer.GetSpan(1)[0] = value;
			this.writer.Advance(1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void WriteBlockSequenceEntryHeader()
		{
			Span<byte> span = this.writer.GetSpan(Utf8YamlEmitter.BlockSequenceEntryHeader.Length + this.currentIndentLevel * this.options.IndentWidth + 2);
			int count = 0;
			this.WriteBlockSequenceEntryHeader(span, ref count);
			this.writer.Advance(count);
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void WriteBlockSequenceEntryHeader(Span<byte> output, ref int offset)
		{
			int num;
			if (this.IsFirstElement)
			{
				EmitState previousState = this.PreviousState;
				if (previousState != EmitState.BlockSequenceEntry)
				{
					if (previousState == EmitState.BlockMappingValue)
					{
						num = offset;
						offset = num + 1;
						*output[num] = 10;
					}
				}
				else
				{
					num = offset;
					offset = num + 1;
					*output[num] = 10;
					this.IncreaseIndent();
				}
			}
			this.WriteIndent(output, ref offset, -1);
			byte[] blockSequenceEntryHeader = Utf8YamlEmitter.BlockSequenceEntryHeader;
			Span<byte> span = output;
			num = offset;
			blockSequenceEntryHeader.CopyTo(span.Slice(num, span.Length - num));
			offset += Utf8YamlEmitter.BlockSequenceEntryHeader.Length;
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void WriteIndent(Span<byte> output, ref int offset, int forceWidth = -1)
		{
			int num;
			if (forceWidth > -1)
			{
				if (forceWidth <= 0)
				{
					return;
				}
				num = forceWidth;
			}
			else
			{
				if (this.currentIndentLevel <= 0)
				{
					return;
				}
				num = this.currentIndentLevel * this.options.IndentWidth;
			}
			if (num > Utf8YamlEmitter.whiteSpaces.Length)
			{
				Utf8YamlEmitter.whiteSpaces = Enumerable.Repeat<byte>(32, num * 2).ToArray<byte>();
			}
			Span<byte> span = Utf8YamlEmitter.whiteSpaces.AsSpan(0, num);
			Span<byte> span2 = output;
			int num2 = offset;
			span.CopyTo(span2.Slice(num2, span2.Length - num2));
			offset += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int CalculateMaxScalarBufferLength(int length)
		{
			return length + (this.currentIndentLevel + 1) * this.options.IndentWidth + 3 + this.GetTagLength();
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void BeginScalar(Span<byte> output, ref int offset)
		{
			switch (this.CurrentState)
			{
			case EmitState.None:
			case EmitState.FlowMappingValue:
				if (this.TryWriteTag(output, ref offset))
				{
					int num = offset;
					offset = num + 1;
					*output[num] = 32;
					return;
				}
				break;
			case EmitState.BlockSequenceEntry:
				this.WriteBlockSequenceEntryHeader(output, ref offset);
				if (this.TryWriteTag(output, ref offset))
				{
					int num = offset;
					offset = num + 1;
					*output[num] = 32;
					return;
				}
				break;
			case EmitState.BlockMappingKey:
			{
				if (!this.IsFirstElement)
				{
					this.WriteIndent(output, ref offset, -1);
					return;
				}
				EmitState previousState = this.PreviousState;
				if (previousState != EmitState.BlockSequenceEntry)
				{
					if (previousState != EmitState.BlockMappingValue)
					{
						this.WriteIndent(output, ref offset, -1);
					}
					else
					{
						this.IncreaseIndent();
						this.TryWriteTag(output, ref offset);
						int num = offset;
						offset = num + 1;
						*output[num] = 10;
						this.WriteIndent(output, ref offset, -1);
					}
				}
				else
				{
					this.IncreaseIndent();
					string value;
					if (this.tagStack.TryPop(out value))
					{
						int num2 = offset;
						Encoding utf = StringEncoding.Utf8;
						ReadOnlySpan<char> chars = value;
						Span<byte> span = output;
						int num = offset;
						offset = num2 + utf.GetBytes(chars, span.Slice(num, span.Length - num));
						num = offset;
						offset = num + 1;
						*output[num] = 10;
						this.WriteIndent(output, ref offset, -1);
					}
					else
					{
						this.WriteIndent(output, ref offset, this.options.IndentWidth - 2);
					}
				}
				if (this.TryWriteTag(output, ref offset))
				{
					int num = offset;
					offset = num + 1;
					*output[num] = 10;
					this.WriteIndent(output, ref offset, -1);
					return;
				}
				break;
			}
			case EmitState.BlockMappingValue:
				if (this.TryWriteTag(output, ref offset))
				{
					int num = offset;
					offset = num + 1;
					*output[num] = 32;
					return;
				}
				break;
			case EmitState.FlowSequenceEntry:
				if (!this.IsFirstElement)
				{
					byte[] flowSequenceSeparator = Utf8YamlEmitter.FlowSequenceSeparator;
					Span<byte> span = output;
					int num = offset;
					flowSequenceSeparator.CopyTo(span.Slice(num, span.Length - num));
					offset += Utf8YamlEmitter.FlowSequenceSeparator.Length;
				}
				if (this.TryWriteTag(output, ref offset))
				{
					int num = offset;
					offset = num + 1;
					*output[num] = 32;
					return;
				}
				break;
			case EmitState.FlowMappingKey:
			{
				int num;
				if (this.IsFirstElement)
				{
					num = offset;
					offset = num + 1;
					*output[num] = 32;
					return;
				}
				byte[] flowSequenceSeparator2 = Utf8YamlEmitter.FlowSequenceSeparator;
				Span<byte> span = output;
				num = offset;
				flowSequenceSeparator2.CopyTo(span.Slice(num, span.Length - num));
				offset += Utf8YamlEmitter.FlowSequenceSeparator.Length;
				return;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void EndScalar(Span<byte> output, ref int offset)
		{
			switch (this.CurrentState)
			{
			case EmitState.None:
				return;
			case EmitState.BlockSequenceEntry:
			{
				int num = offset;
				offset = num + 1;
				*output[num] = 10;
				this.currentElementCount++;
				return;
			}
			case EmitState.BlockMappingKey:
			{
				byte[] mappingKeyFooter = Utf8YamlEmitter.MappingKeyFooter;
				Span<byte> span = output;
				int num = offset;
				mappingKeyFooter.CopyTo(span.Slice(num, span.Length - num));
				offset += Utf8YamlEmitter.MappingKeyFooter.Length;
				this.ReplaceCurrentState(EmitState.BlockMappingValue);
				return;
			}
			case EmitState.BlockMappingValue:
			{
				int num = offset;
				offset = num + 1;
				*output[num] = 10;
				this.ReplaceCurrentState(EmitState.BlockMappingKey);
				this.currentElementCount++;
				return;
			}
			case EmitState.FlowSequenceEntry:
				this.currentElementCount++;
				return;
			case EmitState.FlowMappingKey:
			{
				byte[] mappingKeyFooter2 = Utf8YamlEmitter.MappingKeyFooter;
				Span<byte> span = output;
				int num = offset;
				mappingKeyFooter2.CopyTo(span.Slice(num, span.Length - num));
				offset += Utf8YamlEmitter.MappingKeyFooter.Length;
				this.ReplaceCurrentState(EmitState.FlowMappingValue);
				return;
			}
			case EmitState.FlowMappingValue:
				this.ReplaceCurrentState(EmitState.FlowMappingKey);
				this.currentElementCount++;
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void ReplaceCurrentState(EmitState newState)
		{
			ExpandBuffer<EmitState> expandBuffer = this.stateStack;
			*expandBuffer[expandBuffer.Length - 1] = newState;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void PushState(EmitState state)
		{
			this.stateStack.Add(state);
			this.elementCountStack.Add(this.currentElementCount);
			this.currentElementCount = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void PopState()
		{
			this.stateStack.Pop();
			this.currentElementCount = ((this.elementCountStack.Length > 0) ? (*this.elementCountStack.Pop()) : 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void IncreaseIndent()
		{
			this.currentIndentLevel++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DecreaseIndent()
		{
			if (this.currentIndentLevel > 0)
			{
				this.currentIndentLevel--;
			}
		}

		[NullableContext(0)]
		private bool TryWriteTag(Span<byte> output, ref int offset)
		{
			string value;
			if (this.tagStack.TryPop(out value))
			{
				int num = offset;
				Encoding utf = StringEncoding.Utf8;
				ReadOnlySpan<char> chars = value;
				Span<byte> span = output;
				int num2 = offset;
				offset = num + utf.GetBytes(chars, span.Slice(num2, span.Length - num2));
				return true;
			}
			return false;
		}

		private unsafe int GetTagLength()
		{
			if (this.tagStack.Length <= 0)
			{
				return 0;
			}
			return StringEncoding.Utf8.GetMaxByteCount(this.tagStack.Peek()->Length);
		}

		private static byte[] whiteSpaces = new byte[]
		{
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32,
			32
		};

		private static readonly byte[] BlockSequenceEntryHeader = new byte[]
		{
			45,
			32
		};

		private static readonly byte[] FlowSequenceEmpty = new byte[]
		{
			91,
			93
		};

		private static readonly byte[] FlowSequenceSeparator = new byte[]
		{
			44,
			32
		};

		private static readonly byte[] MappingKeyFooter = new byte[]
		{
			58,
			32
		};

		private static readonly byte[] FlowMappingHeader = new byte[]
		{
			123,
			32
		};

		private static readonly byte[] FlowMappingFooter = new byte[]
		{
			32,
			125
		};

		private static readonly byte[] FlowMappingEmpty = new byte[]
		{
			123,
			125
		};

		[Nullable(2)]
		[ThreadStatic]
		private static ExpandBuffer<char> stringBufferStatic;

		[Nullable(2)]
		[ThreadStatic]
		private static ExpandBuffer<EmitState> stateBufferStatic;

		[Nullable(2)]
		[ThreadStatic]
		private static ExpandBuffer<int> elementCountBufferSTatic;

		[Nullable(new byte[]
		{
			2,
			1
		})]
		[ThreadStatic]
		private static ExpandBuffer<string> tagBufferStatic;

		private readonly IBufferWriter<byte> writer;

		private readonly YamlEmitOptions options;

		private readonly ExpandBuffer<char> stringBuffer;

		private readonly ExpandBuffer<EmitState> stateStack;

		private readonly ExpandBuffer<int> elementCountStack;

		private readonly ExpandBuffer<string> tagStack;

		private int currentIndentLevel;

		private int currentElementCount;
	}
}
