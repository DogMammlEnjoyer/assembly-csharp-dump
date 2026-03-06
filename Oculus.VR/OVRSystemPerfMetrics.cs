using System;
using System.Text;
using OVRSimpleJSON;
using UnityEngine;

public class OVRSystemPerfMetrics
{
	public const int TcpListeningPort = 32419;

	public const int PayloadTypeMetrics = 100;

	public const int MaxBufferLength = 65536;

	public const int MaxMessageLength = 65532;

	public class PerfMetrics
	{
		public string ToJSON()
		{
			JSONObject jsonobject = new JSONObject();
			jsonobject.Add("frameCount", new JSONNumber((double)this.frameCount));
			jsonobject.Add("frameTime", new JSONNumber((double)this.frameTime));
			jsonobject.Add("deltaFrameTime", new JSONNumber((double)this.deltaFrameTime));
			if (this.appCpuTime_IsValid)
			{
				jsonobject.Add("appCpuTime", new JSONNumber((double)this.appCpuTime));
			}
			if (this.appGpuTime_IsValid)
			{
				jsonobject.Add("appGpuTime", new JSONNumber((double)this.appGpuTime));
			}
			if (this.compositorCpuTime_IsValid)
			{
				jsonobject.Add("compositorCpuTime", new JSONNumber((double)this.compositorCpuTime));
			}
			if (this.compositorGpuTime_IsValid)
			{
				jsonobject.Add("compositorGpuTime", new JSONNumber((double)this.compositorGpuTime));
			}
			if (this.compositorDroppedFrameCount_IsValid)
			{
				jsonobject.Add("compositorDroppedFrameCount", new JSONNumber((double)this.compositorDroppedFrameCount));
			}
			if (this.compositorSpaceWarpMode_IsValid)
			{
				jsonobject.Add("compositorSpaceWarpMode", new JSONNumber((double)this.compositorSpaceWarpMode));
			}
			if (this.systemGpuUtilPercentage_IsValid)
			{
				jsonobject.Add("systemGpuUtilPercentage", new JSONNumber((double)this.systemGpuUtilPercentage));
			}
			if (this.systemCpuUtilAveragePercentage_IsValid)
			{
				jsonobject.Add("systemCpuUtilAveragePercentage", new JSONNumber((double)this.systemCpuUtilAveragePercentage));
			}
			if (this.systemCpuUtilWorstPercentage_IsValid)
			{
				jsonobject.Add("systemCpuUtilWorstPercentage", new JSONNumber((double)this.systemCpuUtilWorstPercentage));
			}
			if (this.deviceCpuClockFrequencyInMHz_IsValid)
			{
				jsonobject.Add("deviceCpuClockFrequencyInMHz", new JSONNumber((double)this.deviceCpuClockFrequencyInMHz));
			}
			if (this.deviceGpuClockFrequencyInMHz_IsValid)
			{
				jsonobject.Add("deviceGpuClockFrequencyInMHz", new JSONNumber((double)this.deviceGpuClockFrequencyInMHz));
			}
			if (this.deviceCpuClockLevel_IsValid)
			{
				jsonobject.Add("deviceCpuClockLevel", new JSONNumber((double)this.deviceCpuClockLevel));
			}
			if (this.deviceGpuClockLevel_IsValid)
			{
				jsonobject.Add("deviceGpuClockLevel", new JSONNumber((double)this.deviceGpuClockLevel));
			}
			for (int i = 0; i < OVRPlugin.MAX_CPU_CORES; i++)
			{
				if (this.deviceCpuCoreUtilPercentage_IsValid[i])
				{
					jsonobject.Add("deviceCpuCore" + i.ToString() + "UtilPercentage", new JSONNumber((double)this.deviceCpuCoreUtilPercentage[i]));
				}
			}
			return jsonobject.ToString();
		}

		public bool LoadFromJSON(string json)
		{
			JSONObject jsonobject = JSONNode.Parse(json) as JSONObject;
			if (jsonobject == null)
			{
				return false;
			}
			this.frameCount = ((jsonobject["frameCount"] != null) ? jsonobject["frameCount"].AsInt : 0);
			this.frameTime = ((jsonobject["frameTime"] != null) ? jsonobject["frameTime"].AsFloat : 0f);
			this.deltaFrameTime = ((jsonobject["deltaFrameTime"] != null) ? jsonobject["deltaFrameTime"].AsFloat : 0f);
			this.appCpuTime_IsValid = (jsonobject["appCpuTime"] != null);
			this.appCpuTime = (this.appCpuTime_IsValid ? jsonobject["appCpuTime"].AsFloat : 0f);
			this.appGpuTime_IsValid = (jsonobject["appGpuTime"] != null);
			this.appGpuTime = (this.appGpuTime_IsValid ? jsonobject["appGpuTime"].AsFloat : 0f);
			this.compositorCpuTime_IsValid = (jsonobject["compositorCpuTime"] != null);
			this.compositorCpuTime = (this.compositorCpuTime_IsValid ? jsonobject["compositorCpuTime"].AsFloat : 0f);
			this.compositorGpuTime_IsValid = (jsonobject["compositorGpuTime"] != null);
			this.compositorGpuTime = (this.compositorGpuTime_IsValid ? jsonobject["compositorGpuTime"].AsFloat : 0f);
			this.compositorDroppedFrameCount_IsValid = (jsonobject["compositorDroppedFrameCount"] != null);
			this.compositorDroppedFrameCount = (this.compositorDroppedFrameCount_IsValid ? jsonobject["compositorDroppedFrameCount"].AsInt : 0);
			this.compositorSpaceWarpMode_IsValid = (jsonobject["compositorSpaceWarpMode"] != null);
			this.compositorSpaceWarpMode = (this.compositorSpaceWarpMode_IsValid ? jsonobject["compositorSpaceWarpMode"].AsInt : 0);
			this.systemGpuUtilPercentage_IsValid = (jsonobject["systemGpuUtilPercentage"] != null);
			this.systemGpuUtilPercentage = (this.systemGpuUtilPercentage_IsValid ? jsonobject["systemGpuUtilPercentage"].AsFloat : 0f);
			this.systemCpuUtilAveragePercentage_IsValid = (jsonobject["systemCpuUtilAveragePercentage"] != null);
			this.systemCpuUtilAveragePercentage = (this.systemCpuUtilAveragePercentage_IsValid ? jsonobject["systemCpuUtilAveragePercentage"].AsFloat : 0f);
			this.systemCpuUtilWorstPercentage_IsValid = (jsonobject["systemCpuUtilWorstPercentage"] != null);
			this.systemCpuUtilWorstPercentage = (this.systemCpuUtilWorstPercentage_IsValid ? jsonobject["systemCpuUtilWorstPercentage"].AsFloat : 0f);
			this.deviceCpuClockFrequencyInMHz_IsValid = (jsonobject["deviceCpuClockFrequencyInMHz"] != null);
			this.deviceCpuClockFrequencyInMHz = (this.deviceCpuClockFrequencyInMHz_IsValid ? jsonobject["deviceCpuClockFrequencyInMHz"].AsFloat : 0f);
			this.deviceGpuClockFrequencyInMHz_IsValid = (jsonobject["deviceGpuClockFrequencyInMHz"] != null);
			this.deviceGpuClockFrequencyInMHz = (this.deviceGpuClockFrequencyInMHz_IsValid ? jsonobject["deviceGpuClockFrequencyInMHz"].AsFloat : 0f);
			this.deviceCpuClockLevel_IsValid = (jsonobject["deviceCpuClockLevel"] != null);
			this.deviceCpuClockLevel = (this.deviceCpuClockLevel_IsValid ? jsonobject["deviceCpuClockLevel"].AsInt : 0);
			this.deviceGpuClockLevel_IsValid = (jsonobject["deviceGpuClockLevel"] != null);
			this.deviceGpuClockLevel = (this.deviceGpuClockLevel_IsValid ? jsonobject["deviceGpuClockLevel"].AsInt : 0);
			for (int i = 0; i < OVRPlugin.MAX_CPU_CORES; i++)
			{
				this.deviceCpuCoreUtilPercentage_IsValid[i] = (jsonobject["deviceCpuCore" + i.ToString() + "UtilPercentage"] != null);
				this.deviceCpuCoreUtilPercentage[i] = (this.deviceCpuCoreUtilPercentage_IsValid[i] ? jsonobject["deviceCpuCore" + i.ToString() + "UtilPercentage"].AsFloat : 0f);
			}
			return true;
		}

		public int frameCount;

		public float frameTime;

		public float deltaFrameTime;

		public bool appCpuTime_IsValid;

		public float appCpuTime;

		public bool appGpuTime_IsValid;

		public float appGpuTime;

		public bool compositorCpuTime_IsValid;

		public float compositorCpuTime;

		public bool compositorGpuTime_IsValid;

		public float compositorGpuTime;

		public bool compositorDroppedFrameCount_IsValid;

		public int compositorDroppedFrameCount;

		public bool compositorSpaceWarpMode_IsValid;

		public int compositorSpaceWarpMode;

		public bool systemGpuUtilPercentage_IsValid;

		public float systemGpuUtilPercentage;

		public bool systemCpuUtilAveragePercentage_IsValid;

		public float systemCpuUtilAveragePercentage;

		public bool systemCpuUtilWorstPercentage_IsValid;

		public float systemCpuUtilWorstPercentage;

		public bool deviceCpuClockFrequencyInMHz_IsValid;

		public float deviceCpuClockFrequencyInMHz;

		public bool deviceGpuClockFrequencyInMHz_IsValid;

		public float deviceGpuClockFrequencyInMHz;

		public bool deviceCpuClockLevel_IsValid;

		public int deviceCpuClockLevel;

		public bool deviceGpuClockLevel_IsValid;

		public int deviceGpuClockLevel;

		public bool[] deviceCpuCoreUtilPercentage_IsValid = new bool[OVRPlugin.MAX_CPU_CORES];

		public float[] deviceCpuCoreUtilPercentage = new float[OVRPlugin.MAX_CPU_CORES];
	}

	public class OVRSystemPerfMetricsTcpServer : MonoBehaviour
	{
		private void OnEnable()
		{
			if (OVRSystemPerfMetrics.OVRSystemPerfMetricsTcpServer.singleton != null)
			{
				Debug.LogError("Mutiple OVRSystemPerfMetricsTcpServer exists");
				return;
			}
			OVRSystemPerfMetrics.OVRSystemPerfMetricsTcpServer.singleton = this;
			if (Application.isEditor)
			{
				Application.runInBackground = true;
			}
			this.tcpServer.StartListening(this.listeningPort);
		}

		private void OnDisable()
		{
			this.tcpServer.StopListening();
			OVRSystemPerfMetrics.OVRSystemPerfMetricsTcpServer.singleton = null;
			Debug.Log("[OVRSystemPerfMetricsTcpServer] server destroyed");
		}

		private void Update()
		{
			if (this.tcpServer.HasConnectedClient())
			{
				string s = this.GatherPerfMetrics().ToJSON();
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				this.tcpServer.Broadcast(100, bytes);
			}
		}

		public OVRSystemPerfMetrics.PerfMetrics GatherPerfMetrics()
		{
			OVRSystemPerfMetrics.PerfMetrics perfMetrics = new OVRSystemPerfMetrics.PerfMetrics();
			perfMetrics.frameCount = Time.frameCount;
			perfMetrics.frameTime = Time.unscaledTime;
			perfMetrics.deltaFrameTime = Time.unscaledDeltaTime;
			float? perfMetricsFloat = OVRPlugin.GetPerfMetricsFloat(OVRPlugin.PerfMetrics.App_CpuTime_Float);
			perfMetrics.appCpuTime_IsValid = (perfMetricsFloat != null);
			perfMetrics.appCpuTime = perfMetricsFloat.GetValueOrDefault();
			perfMetricsFloat = OVRPlugin.GetPerfMetricsFloat(OVRPlugin.PerfMetrics.App_GpuTime_Float);
			perfMetrics.appGpuTime_IsValid = (perfMetricsFloat != null);
			perfMetrics.appGpuTime = perfMetricsFloat.GetValueOrDefault();
			perfMetricsFloat = OVRPlugin.GetPerfMetricsFloat(OVRPlugin.PerfMetrics.Compositor_CpuTime_Float);
			perfMetrics.compositorCpuTime_IsValid = (perfMetricsFloat != null);
			perfMetrics.compositorCpuTime = perfMetricsFloat.GetValueOrDefault();
			perfMetricsFloat = OVRPlugin.GetPerfMetricsFloat(OVRPlugin.PerfMetrics.Compositor_GpuTime_Float);
			perfMetrics.compositorGpuTime_IsValid = (perfMetricsFloat != null);
			perfMetrics.compositorGpuTime = perfMetricsFloat.GetValueOrDefault();
			int? perfMetricsInt = OVRPlugin.GetPerfMetricsInt(OVRPlugin.PerfMetrics.Compositor_DroppedFrameCount_Int);
			perfMetrics.compositorDroppedFrameCount_IsValid = (perfMetricsInt != null);
			perfMetrics.compositorDroppedFrameCount = perfMetricsInt.GetValueOrDefault();
			perfMetricsInt = OVRPlugin.GetPerfMetricsInt(OVRPlugin.PerfMetrics.Compositor_SpaceWarp_Mode_Int);
			perfMetrics.compositorSpaceWarpMode_IsValid = (perfMetricsInt != null);
			perfMetrics.compositorSpaceWarpMode = perfMetricsInt.GetValueOrDefault();
			perfMetricsFloat = OVRPlugin.GetPerfMetricsFloat(OVRPlugin.PerfMetrics.System_GpuUtilPercentage_Float);
			perfMetrics.systemGpuUtilPercentage_IsValid = (perfMetricsFloat != null);
			perfMetrics.systemGpuUtilPercentage = perfMetricsFloat.GetValueOrDefault();
			perfMetricsFloat = OVRPlugin.GetPerfMetricsFloat(OVRPlugin.PerfMetrics.System_CpuUtilAveragePercentage_Float);
			perfMetrics.systemCpuUtilAveragePercentage_IsValid = (perfMetricsFloat != null);
			perfMetrics.systemCpuUtilAveragePercentage = perfMetricsFloat.GetValueOrDefault();
			perfMetricsFloat = OVRPlugin.GetPerfMetricsFloat(OVRPlugin.PerfMetrics.System_CpuUtilWorstPercentage_Float);
			perfMetrics.systemCpuUtilWorstPercentage_IsValid = (perfMetricsFloat != null);
			perfMetrics.systemCpuUtilWorstPercentage = perfMetricsFloat.GetValueOrDefault();
			perfMetricsFloat = OVRPlugin.GetPerfMetricsFloat(OVRPlugin.PerfMetrics.Device_CpuClockFrequencyInMHz_Float);
			perfMetrics.deviceCpuClockFrequencyInMHz_IsValid = (perfMetricsFloat != null);
			perfMetrics.deviceCpuClockFrequencyInMHz = perfMetricsFloat.GetValueOrDefault();
			perfMetricsFloat = OVRPlugin.GetPerfMetricsFloat(OVRPlugin.PerfMetrics.Device_GpuClockFrequencyInMHz_Float);
			perfMetrics.deviceGpuClockFrequencyInMHz_IsValid = (perfMetricsFloat != null);
			perfMetrics.deviceGpuClockFrequencyInMHz = perfMetricsFloat.GetValueOrDefault();
			perfMetricsInt = OVRPlugin.GetPerfMetricsInt(OVRPlugin.PerfMetrics.Device_CpuClockLevel_Int);
			perfMetrics.deviceCpuClockLevel_IsValid = (perfMetricsInt != null);
			perfMetrics.deviceCpuClockLevel = perfMetricsInt.GetValueOrDefault();
			perfMetricsInt = OVRPlugin.GetPerfMetricsInt(OVRPlugin.PerfMetrics.Device_GpuClockLevel_Int);
			perfMetrics.deviceGpuClockLevel_IsValid = (perfMetricsInt != null);
			perfMetrics.deviceGpuClockLevel = perfMetricsInt.GetValueOrDefault();
			for (int i = 0; i < OVRPlugin.MAX_CPU_CORES; i++)
			{
				perfMetricsFloat = OVRPlugin.GetPerfMetricsFloat(OVRPlugin.PerfMetrics.Device_CpuCore0UtilPercentage_Float);
				perfMetrics.deviceCpuCoreUtilPercentage_IsValid[i] = (perfMetricsFloat != null);
				perfMetrics.deviceCpuCoreUtilPercentage[i] = perfMetricsFloat.GetValueOrDefault();
			}
			return perfMetrics;
		}

		public static OVRSystemPerfMetrics.OVRSystemPerfMetricsTcpServer singleton;

		private OVRNetwork.OVRNetworkTcpServer tcpServer = new OVRNetwork.OVRNetworkTcpServer();

		public int listeningPort = 32419;
	}
}
