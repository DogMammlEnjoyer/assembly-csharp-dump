using System;
using System.Runtime.CompilerServices;

namespace K4os.Compression.LZ4.Engine
{
	internal static class LLxx
	{
		private static NotImplementedException AlgorithmNotImplemented(string action)
		{
			return new NotImplementedException(string.Format("Algorithm {0} not implemented for {1}", LL.Algorithm, action));
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public unsafe static int LZ4_decompress_safe(byte* source, byte* target, int sourceLength, int targetLength)
		{
			Algorithm algorithm = LL.Algorithm;
			int result;
			if (algorithm != Algorithm.X32)
			{
				if (algorithm != Algorithm.X64)
				{
					throw LLxx.AlgorithmNotImplemented("LZ4_decompress_safe");
				}
				result = LL64.LZ4_decompress_safe(source, target, sourceLength, targetLength);
			}
			else
			{
				result = LL32.LZ4_decompress_safe(source, target, sourceLength, targetLength);
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public unsafe static int LZ4_decompress_safe_partial(byte* source, byte* target, int sourceLength, int targetLength)
		{
			Algorithm algorithm = LL.Algorithm;
			int result;
			if (algorithm != Algorithm.X32)
			{
				if (algorithm != Algorithm.X64)
				{
					throw LLxx.AlgorithmNotImplemented("LZ4_decompress_safe_partial");
				}
				result = LL64.LZ4_decompress_safe_partial(source, target, sourceLength, targetLength, targetLength);
			}
			else
			{
				result = LL32.LZ4_decompress_safe_partial(source, target, sourceLength, targetLength, targetLength);
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public unsafe static int LZ4_decompress_safe_usingDict(byte* source, byte* target, int sourceLength, int targetLength, byte* dictionary, int dictionaryLength)
		{
			Algorithm algorithm = LL.Algorithm;
			int result;
			if (algorithm != Algorithm.X32)
			{
				if (algorithm != Algorithm.X64)
				{
					throw LLxx.AlgorithmNotImplemented("LZ4_decompress_safe_usingDict");
				}
				result = LL64.LZ4_decompress_safe_usingDict(source, target, sourceLength, targetLength, dictionary, dictionaryLength);
			}
			else
			{
				result = LL32.LZ4_decompress_safe_usingDict(source, target, sourceLength, targetLength, dictionary, dictionaryLength);
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public unsafe static int LZ4_decompress_safe_continue(LL.LZ4_streamDecode_t* context, byte* source, byte* target, int sourceLength, int targetLength)
		{
			Algorithm algorithm = LL.Algorithm;
			int result;
			if (algorithm != Algorithm.X32)
			{
				if (algorithm != Algorithm.X64)
				{
					throw LLxx.AlgorithmNotImplemented("LZ4_decompress_safe_continue");
				}
				result = LL64.LZ4_decompress_safe_continue(context, source, target, sourceLength, targetLength);
			}
			else
			{
				result = LL32.LZ4_decompress_safe_continue(context, source, target, sourceLength, targetLength);
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public unsafe static int LZ4_compress_fast(byte* source, byte* target, int sourceLength, int targetLength, int acceleration)
		{
			Algorithm algorithm = LL.Algorithm;
			int result;
			if (algorithm != Algorithm.X32)
			{
				if (algorithm != Algorithm.X64)
				{
					throw LLxx.AlgorithmNotImplemented("LZ4_compress_fast");
				}
				result = LL64.LZ4_compress_fast(source, target, sourceLength, targetLength, acceleration);
			}
			else
			{
				result = LL32.LZ4_compress_fast(source, target, sourceLength, targetLength, acceleration);
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public unsafe static int LZ4_compress_fast_continue(LL.LZ4_stream_t* context, byte* source, byte* target, int sourceLength, int targetLength, int acceleration)
		{
			Algorithm algorithm = LL.Algorithm;
			int result;
			if (algorithm != Algorithm.X32)
			{
				if (algorithm != Algorithm.X64)
				{
					throw LLxx.AlgorithmNotImplemented("LZ4_compress_fast_continue");
				}
				result = LL64.LZ4_compress_fast_continue(context, source, target, sourceLength, targetLength, acceleration);
			}
			else
			{
				result = LL32.LZ4_compress_fast_continue(context, source, target, sourceLength, targetLength, acceleration);
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public unsafe static int LZ4_compress_HC(byte* source, byte* target, int sourceLength, int targetLength, int level)
		{
			Algorithm algorithm = LL.Algorithm;
			int result;
			if (algorithm != Algorithm.X32)
			{
				if (algorithm != Algorithm.X64)
				{
					throw LLxx.AlgorithmNotImplemented("LZ4_compress_HC");
				}
				result = LL64.LZ4_compress_HC(source, target, sourceLength, targetLength, level);
			}
			else
			{
				result = LL32.LZ4_compress_HC(source, target, sourceLength, targetLength, level);
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public unsafe static int LZ4_compress_HC_continue(LL.LZ4_streamHC_t* context, byte* source, byte* target, int sourceLength, int targetLength)
		{
			Algorithm algorithm = LL.Algorithm;
			int result;
			if (algorithm != Algorithm.X32)
			{
				if (algorithm != Algorithm.X64)
				{
					throw LLxx.AlgorithmNotImplemented("LZ4_compress_HC_continue");
				}
				result = LL64.LZ4_compress_HC_continue(context, source, target, sourceLength, targetLength);
			}
			else
			{
				result = LL32.LZ4_compress_HC_continue(context, source, target, sourceLength, targetLength);
			}
			return result;
		}
	}
}
