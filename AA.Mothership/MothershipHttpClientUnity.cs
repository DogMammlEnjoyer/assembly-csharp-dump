using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class MothershipHttpClientUnity : MothershipSendHTTPRequestDelegateWrapper
{
	public MothershipHttpClientUnity(MothershipClientApiClient client, bool isRequestLoggingEnabled)
	{
		this.swigCMemOwn = false;
		this.client = client;
		this.isRequestLoggingEnabled = isRequestLoggingEnabled;
	}

	public override bool SendRequest(MothershipHTTPRequest request)
	{
		UnityWebRequest unityWebRequest = new UnityWebRequest(request.Path, request.Verb.ToString());
		byte[] data = null;
		if (request.Verb == MothershipHTTPVerbs.POST)
		{
			data = new UTF8Encoding().GetBytes(request.Body);
		}
		if (this.isRequestLoggingEnabled)
		{
			Debug.Log(string.Format("Mothership request body: {0}", request.Body));
		}
		unityWebRequest.uploadHandler = new UploadHandlerRaw(data);
		unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
		foreach (MothershipHttpHeader mothershipHttpHeader in request.RequestHeaders)
		{
			if (this.isRequestLoggingEnabled)
			{
				Debug.Log(string.Format("Mothership request header: {0} {1})", mothershipHttpHeader.Name, mothershipHttpHeader.Value));
			}
			unityWebRequest.SetRequestHeader(mothershipHttpHeader.Name, mothershipHttpHeader.Value);
		}
		unityWebRequest.timeout = 5;
		MothershipHttpRunner.instance.SendRequest(unityWebRequest, request, delegate(MothershipHTTPResponse Response)
		{
			if (this.isRequestLoggingEnabled)
			{
				Debug.Log(string.Format("Mothership: Request to {0} status {1}", request.Path, Response.statusCode));
			}
			this.client.ReceiveHttpResponse(Response);
		});
		return true;
	}

	private MothershipClientApiClient client;

	private bool isRequestLoggingEnabled;
}
