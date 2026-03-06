using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Rendering
{
	public sealed class ShaderDebugPrintManager
	{
		private int DebugValueTypeToElemSize(ShaderDebugPrintManager.DebugValueType type)
		{
			switch (type)
			{
			case ShaderDebugPrintManager.DebugValueType.TypeUint:
			case ShaderDebugPrintManager.DebugValueType.TypeInt:
			case ShaderDebugPrintManager.DebugValueType.TypeFloat:
			case ShaderDebugPrintManager.DebugValueType.TypeBool:
				return 1;
			case ShaderDebugPrintManager.DebugValueType.TypeUint2:
			case ShaderDebugPrintManager.DebugValueType.TypeInt2:
			case ShaderDebugPrintManager.DebugValueType.TypeFloat2:
				return 2;
			case ShaderDebugPrintManager.DebugValueType.TypeUint3:
			case ShaderDebugPrintManager.DebugValueType.TypeInt3:
			case ShaderDebugPrintManager.DebugValueType.TypeFloat3:
				return 3;
			case ShaderDebugPrintManager.DebugValueType.TypeUint4:
			case ShaderDebugPrintManager.DebugValueType.TypeInt4:
			case ShaderDebugPrintManager.DebugValueType.TypeFloat4:
				return 4;
			default:
				return 0;
			}
		}

		private ShaderDebugPrintManager()
		{
			for (int i = 0; i < 4; i++)
			{
				this.m_OutputBuffers.Add(new GraphicsBuffer(GraphicsBuffer.Target.Structured, 16384, 4));
				this.m_ReadbackRequests.Add(default(AsyncGPUReadbackRequest));
			}
			this.m_BufferReadCompleteAction = new Action<AsyncGPUReadbackRequest>(this.BufferReadComplete);
			this.m_OutputAction = new Action<string>(this.DefaultOutput);
		}

		public static ShaderDebugPrintManager instance
		{
			get
			{
				return ShaderDebugPrintManager.s_Instance;
			}
		}

		public void SetShaderDebugPrintInputConstants(CommandBuffer cmd, ShaderDebugPrintInput input)
		{
			Vector4 value = new Vector4(input.pos.x, input.pos.y, (float)(input.leftDown ? 1 : 0), (float)(input.rightDown ? 1 : 0));
			cmd.SetGlobalVector(ShaderDebugPrintManager.m_ShaderPropertyIDInputMouse, value);
			cmd.SetGlobalInt(ShaderDebugPrintManager.m_ShaderPropertyIDInputFrame, this.m_FrameCounter);
		}

		public void SetShaderDebugPrintBindings(CommandBuffer cmd)
		{
			int index = this.m_FrameCounter % 4;
			if (!this.m_ReadbackRequests[index].done)
			{
				this.m_ReadbackRequests[index].WaitForCompletion();
			}
			cmd.SetGlobalBuffer(ShaderDebugPrintManager.m_shaderDebugOutputData, this.m_OutputBuffers[index]);
			this.ClearShaderDebugPrintBuffer();
		}

		private void ClearShaderDebugPrintBuffer()
		{
			if (!this.m_FrameCleared)
			{
				int index = this.m_FrameCounter % 4;
				NativeArray<uint> data = new NativeArray<uint>(1, Allocator.Temp, NativeArrayOptions.ClearMemory);
				data[0] = 0U;
				this.m_OutputBuffers[index].SetData<uint>(data, 0, 0, 1);
				this.m_FrameCleared = true;
			}
		}

		private unsafe void BufferReadComplete(AsyncGPUReadbackRequest request)
		{
			using (new ProfilingScope(ShaderDebugPrintManager.Profiling.BufferReadComplete))
			{
				if (!request.hasError)
				{
					NativeArray<uint> data = request.GetData<uint>(0);
					uint num = data[0];
					if (num >= 16384U)
					{
						num = 16384U;
						Debug.LogWarning("Debug Shader Print Buffer Full!");
					}
					string text = "";
					if (num > 0U)
					{
						text = text + "Frame #" + this.m_FrameCounter.ToString() + ": ";
					}
					uint* unsafePtr = (uint*)data.GetUnsafePtr<uint>();
					int num2 = 1;
					while ((long)num2 < (long)((ulong)num))
					{
						ShaderDebugPrintManager.DebugValueType type = (ShaderDebugPrintManager.DebugValueType)(data[num2] & 15U);
						if ((data[num2] & 128U) == 128U && (long)(num2 + 1) < (long)((ulong)num))
						{
							uint num3 = data[num2 + 1];
							num2++;
							for (int i = 0; i < 4; i++)
							{
								char c = (char)(num3 & 255U);
								if (c != '\0')
								{
									text += c.ToString();
									num3 >>= 8;
								}
							}
							text += " ";
						}
						int num4 = this.DebugValueTypeToElemSize(type);
						if ((long)(num2 + num4) > (long)((ulong)num))
						{
							break;
						}
						num2++;
						switch (type)
						{
						case ShaderDebugPrintManager.DebugValueType.TypeUint:
							text += string.Format("{0}u", data[num2]);
							break;
						case ShaderDebugPrintManager.DebugValueType.TypeInt:
						{
							int num5 = (int)unsafePtr[num2];
							text += num5.ToString();
							break;
						}
						case ShaderDebugPrintManager.DebugValueType.TypeFloat:
						{
							float num6 = *(float*)(unsafePtr + num2);
							text += string.Format("{0}f", num6);
							break;
						}
						case ShaderDebugPrintManager.DebugValueType.TypeUint2:
						{
							uint* ptr = unsafePtr + num2;
							text += string.Format("uint2({0}, {1})", *ptr, ptr[1]);
							break;
						}
						case ShaderDebugPrintManager.DebugValueType.TypeInt2:
						{
							int* ptr2 = (int*)(unsafePtr + num2);
							text += string.Format("int2({0}, {1})", *ptr2, ptr2[1]);
							break;
						}
						case ShaderDebugPrintManager.DebugValueType.TypeFloat2:
						{
							float* ptr3 = (float*)(unsafePtr + num2);
							text += string.Format("float2({0}, {1})", *ptr3, ptr3[1]);
							break;
						}
						case ShaderDebugPrintManager.DebugValueType.TypeUint3:
						{
							uint* ptr4 = unsafePtr + num2;
							text += string.Format("uint3({0}, {1}, {2})", *ptr4, ptr4[1], ptr4[2]);
							break;
						}
						case ShaderDebugPrintManager.DebugValueType.TypeInt3:
						{
							int* ptr5 = (int*)(unsafePtr + num2);
							text += string.Format("int3({0}, {1}, {2})", *ptr5, ptr5[1], ptr5[2]);
							break;
						}
						case ShaderDebugPrintManager.DebugValueType.TypeFloat3:
						{
							float* ptr6 = (float*)(unsafePtr + num2);
							text += string.Format("float3({0}, {1}, {2})", *ptr6, ptr6[1], ptr6[2]);
							break;
						}
						case ShaderDebugPrintManager.DebugValueType.TypeUint4:
						{
							uint* ptr7 = unsafePtr + num2;
							text += string.Format("uint4({0}, {1}, {2}, {3})", new object[]
							{
								*ptr7,
								ptr7[1],
								ptr7[2],
								ptr7[3]
							});
							break;
						}
						case ShaderDebugPrintManager.DebugValueType.TypeInt4:
						{
							int* ptr8 = (int*)(unsafePtr + num2);
							text += string.Format("int4({0}, {1}, {2}, {3})", new object[]
							{
								*ptr8,
								ptr8[1],
								ptr8[2],
								ptr8[3]
							});
							break;
						}
						case ShaderDebugPrintManager.DebugValueType.TypeFloat4:
						{
							float* ptr9 = (float*)(unsafePtr + num2);
							text += string.Format("float4({0}, {1}, {2}, {3})", new object[]
							{
								*ptr9,
								ptr9[1],
								ptr9[2],
								ptr9[3]
							});
							break;
						}
						case ShaderDebugPrintManager.DebugValueType.TypeBool:
							text += ((data[num2] == 0U) ? "False" : "True");
							break;
						default:
							num2 = (int)num;
							break;
						}
						num2 += num4;
						text += " ";
					}
					if (num > 0U)
					{
						this.m_OutputLine = text;
						this.m_OutputAction(text);
					}
				}
				else
				{
					this.m_OutputLine = "Error at read back!";
					this.m_OutputAction("Error at read back!");
				}
			}
		}

		public void EndFrame()
		{
			int index = this.m_FrameCounter % 4;
			this.m_ReadbackRequests[index] = AsyncGPUReadback.Request(this.m_OutputBuffers[index], this.m_BufferReadCompleteAction);
			this.m_FrameCounter++;
			this.m_FrameCleared = false;
		}

		public void PrintImmediate()
		{
			int index = this.m_FrameCounter % 4;
			AsyncGPUReadbackRequest obj = AsyncGPUReadback.Request(this.m_OutputBuffers[index], null);
			obj.WaitForCompletion();
			this.m_BufferReadCompleteAction(obj);
			this.m_FrameCounter++;
			this.m_FrameCleared = false;
		}

		public string outputLine
		{
			get
			{
				return this.m_OutputLine;
			}
		}

		public Action<string> outputAction
		{
			set
			{
				this.m_OutputAction = value;
			}
		}

		public void DefaultOutput(string line)
		{
			Debug.Log(line);
		}

		private static readonly ShaderDebugPrintManager s_Instance = new ShaderDebugPrintManager();

		private const int k_FramesInFlight = 4;

		private const int k_MaxBufferElements = 16384;

		private List<GraphicsBuffer> m_OutputBuffers = new List<GraphicsBuffer>();

		private List<AsyncGPUReadbackRequest> m_ReadbackRequests = new List<AsyncGPUReadbackRequest>();

		private Action<AsyncGPUReadbackRequest> m_BufferReadCompleteAction;

		private int m_FrameCounter;

		private bool m_FrameCleared;

		private string m_OutputLine = "";

		private Action<string> m_OutputAction;

		private static readonly int m_ShaderPropertyIDInputMouse = Shader.PropertyToID("_ShaderDebugPrintInputMouse");

		private static readonly int m_ShaderPropertyIDInputFrame = Shader.PropertyToID("_ShaderDebugPrintInputFrame");

		private static readonly int m_shaderDebugOutputData = Shader.PropertyToID("shaderDebugOutputData");

		private const uint k_TypeHasTag = 128U;

		private static class Profiling
		{
			public static readonly ProfilingSampler BufferReadComplete = new ProfilingSampler("ShaderDebugPrintManager.BufferReadComplete");
		}

		private enum DebugValueType
		{
			TypeUint = 1,
			TypeInt,
			TypeFloat,
			TypeUint2,
			TypeInt2,
			TypeFloat2,
			TypeUint3,
			TypeInt3,
			TypeFloat3,
			TypeUint4,
			TypeInt4,
			TypeFloat4,
			TypeBool
		}
	}
}
