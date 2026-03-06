using System;
using UnityEngine;

namespace Oculus.Interaction.Demo
{
	public class BasicPBRGlobals : MonoBehaviour
	{
		private void Update()
		{
			this.UpateShaderGlobals();
		}

		private void UpateShaderGlobals()
		{
			Light mainlight = this._mainlight;
			bool flag = mainlight && mainlight.isActiveAndEnabled;
			Shader.SetGlobalVector("_BasicPBRLightDir", flag ? mainlight.transform.forward : Vector3.down);
			Shader.SetGlobalColor("_BasicPBRLightColor", flag ? mainlight.color : Color.black);
		}

		[SerializeField]
		private Light _mainlight;
	}
}
