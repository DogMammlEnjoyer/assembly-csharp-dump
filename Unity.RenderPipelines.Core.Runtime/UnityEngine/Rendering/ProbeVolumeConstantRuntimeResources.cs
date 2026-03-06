using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering
{
	internal static class ProbeVolumeConstantRuntimeResources
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void GetRuntimeResources(ref ProbeReferenceVolume.RuntimeResources rr)
		{
			rr.SkyPrecomputedDirections = ProbeVolumeConstantRuntimeResources.m_SkySamplingDirectionsBuffer;
			rr.QualityLeakReductionData = ProbeVolumeConstantRuntimeResources.m_AntiLeakDataBuffer;
		}

		internal static void Initialize()
		{
			if (ProbeVolumeConstantRuntimeResources.m_SkySamplingDirectionsBuffer == null)
			{
				ProbeVolumeConstantRuntimeResources.k_SkyDirections = ProbeVolumeConstantRuntimeResources.GenerateSkyDirections();
				ProbeVolumeConstantRuntimeResources.m_SkySamplingDirectionsBuffer = new ComputeBuffer(ProbeVolumeConstantRuntimeResources.k_SkyDirections.Length, 12);
				ProbeVolumeConstantRuntimeResources.m_SkySamplingDirectionsBuffer.SetData(ProbeVolumeConstantRuntimeResources.k_SkyDirections);
			}
			if (ProbeVolumeConstantRuntimeResources.m_AntiLeakDataBuffer == null)
			{
				ProbeVolumeConstantRuntimeResources.m_AntiLeakDataBuffer = new ComputeBuffer(ProbeVolumeConstantRuntimeResources.k_AntiLeakData.Length, 4);
				ProbeVolumeConstantRuntimeResources.m_AntiLeakDataBuffer.SetData(ProbeVolumeConstantRuntimeResources.k_AntiLeakData);
			}
		}

		public static Vector3[] GetSkySamplingDirections()
		{
			return ProbeVolumeConstantRuntimeResources.k_SkyDirections;
		}

		internal static void Cleanup()
		{
			CoreUtils.SafeRelease(ProbeVolumeConstantRuntimeResources.m_SkySamplingDirectionsBuffer);
			ProbeVolumeConstantRuntimeResources.m_SkySamplingDirectionsBuffer = null;
			CoreUtils.SafeRelease(ProbeVolumeConstantRuntimeResources.m_AntiLeakDataBuffer);
			ProbeVolumeConstantRuntimeResources.m_AntiLeakDataBuffer = null;
		}

		private static Vector3[] GenerateSkyDirections()
		{
			Vector3[] array = new Vector3[255];
			float num = Mathf.Sqrt(255f);
			float num2 = 0f;
			float a = 0f;
			float a2 = 0f;
			for (int i = 0; i < 255; i++)
			{
				float num3 = -1f + 2f * (float)i / 254f;
				float num4 = Mathf.Acos(num3);
				if (i == 254 || i == 0)
				{
					num2 = 0f;
				}
				else
				{
					num2 += 3.6f / num * 1f / Mathf.Sqrt(1f - num3 * num3);
				}
				Vector3 vector = new Vector3(Mathf.Sin(num4) * Mathf.Cos(num2), Mathf.Sin(num4) * Mathf.Sin(num2), Mathf.Cos(num4));
				vector.Normalize();
				array[i] = vector;
				a = Mathf.Max(a, num2);
				a2 = Mathf.Max(a2, num4);
			}
			return array;
		}

		private static ComputeBuffer m_SkySamplingDirectionsBuffer = null;

		private static ComputeBuffer m_AntiLeakDataBuffer = null;

		private const int NB_SKY_PRECOMPUTED_DIRECTIONS = 255;

		private static Vector3[] k_SkyDirections = new Vector3[255];

		private static uint[] k_AntiLeakData = new uint[]
		{
			38347995U,
			38347849U,
			38347852U,
			38347851U,
			38347873U,
			38347865U,
			38322764U,
			38322763U,
			38347876U,
			38324297U,
			38347868U,
			38324299U,
			38347875U,
			38324313U,
			38322780U,
			38347867U,
			38348041U,
			38347977U,
			38408780U,
			38408779U,
			38408801U,
			38408793U,
			69517900U,
			69517899U,
			38408804U,
			38324425U,
			38408796U,
			69519435U,
			38408803U,
			69519449U,
			69517916U,
			38408795U,
			38348044U,
			38410313U,
			38347980U,
			38410315U,
			38410337U,
			38410329U,
			38322892U,
			70304331U,
			38410340U,
			70305865U,
			38410332U,
			70305867U,
			38410339U,
			70305881U,
			70304348U,
			38410331U,
			38348043U,
			38410441U,
			38408908U,
			38347979U,
			38322955U,
			38409817U,
			69518028U,
			38322891U,
			38324491U,
			70305993U,
			38409820U,
			38324427U,
			38409827U,
			26351193U,
			25564764U,
			38323915U,
			38348065U,
			38421065U,
			38421068U,
			38421067U,
			38348001U,
			38421081U,
			38312161U,
			38388299U,
			38421092U,
			75810889U,
			38421084U,
			75810891U,
			38421091U,
			75810905U,
			38388316U,
			38421083U,
			38348057U,
			38421193U,
			38312217U,
			38416971U,
			38408929U,
			38347993U,
			69507297U,
			38312153U,
			38324505U,
			75811017U,
			38416988U,
			26358347U,
			38416995U,
			38324441U,
			69583452U,
			38320345U,
			38421260U,
			75896905U,
			38421196U,
			75896907U,
			38410465U,
			75896921U,
			38388428U,
			70369867U,
			75896932U,
			70305865U,
			75896924U,
			70305867U,
			75896931U,
			70305881U,
			70369884U,
			75896923U,
			38421259U,
			75897033U,
			38417100U,
			38421195U,
			38409953U,
			38410457U,
			69583564U,
			38377689U,
			75811083U,
			70305993U,
			75896412U,
			75811019U,
			75896419U,
			70306009U,
			70107740U,
			70301913U,
			38348068U,
			38422601U,
			38422604U,
			38422603U,
			38422625U,
			38422617U,
			76595788U,
			76595787U,
			38348004U,
			38310628U,
			38422620U,
			38389835U,
			38422627U,
			38389849U,
			76595804U,
			38422619U,
			38422793U,
			38422729U,
			76681804U,
			76681803U,
			76681825U,
			76681817U,
			69517900U,
			69517899U,
			38408932U,
			38389961U,
			76681820U,
			69584971U,
			76681827U,
			69584985U,
			69517916U,
			76681819U,
			38348060U,
			38310684U,
			38422732U,
			38418507U,
			38322972U,
			38418521U,
			76595916U,
			25573451U,
			38410468U,
			70292196U,
			38347996U,
			38310620U,
			38418531U,
			70371417U,
			38322908U,
			38318812U,
			38422795U,
			38418633U,
			76681932U,
			38422731U,
			76595979U,
			76682841U,
			69518028U,
			76595915U,
			38409956U,
			70371529U,
			38408924U,
			38376156U,
			76682851U,
			70109273U,
			69518044U,
			69513948U,
			38348067U,
			38310691U,
			38312227U,
			38422091U,
			38422753U,
			38422105U,
			76585185U,
			76661323U,
			38421220U,
			75797220U,
			38422108U,
			75876427U,
			38348003U,
			38310627U,
			38312163U,
			38311651U,
			38422809U,
			38422217U,
			76585241U,
			76689995U,
			76681953U,
			38422745U,
			69507297U,
			76585177U,
			38417124U,
			75876553U,
			76690012U,
			73779275U,
			38408931U,
			38389977U,
			69507299U,
			76593369U,
			38421276U,
			75797276U,
			38422220U,
			75905099U,
			38418657U,
			75905113U,
			76661452U,
			74564171U,
			75897060U,
			70292196U,
			38421212U,
			75797212U,
			38410467U,
			70292195U,
			38388444U,
			75805404U,
			38348059U,
			38310683U,
			38312219U,
			38422219U,
			38322971U,
			38418649U,
			25467163U,
			76650713U,
			38324507U,
			26252059U,
			38417116U,
			75862748U,
			38409955U,
			70371545U,
			69583580U,
			38347995U
		};
	}
}
