using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
	internal class XRUIToolkitPokeHandler
	{
		public bool updateDepth
		{
			get
			{
				return this.m_UpdateDepth;
			}
			set
			{
				this.m_UpdateDepth = value;
			}
		}

		public XRUIToolkitPokeHandler(XRPokeInteractor interactor)
		{
			this.m_Interactor = interactor;
		}

		public void Dispose()
		{
			this.DestroyVisualizers();
		}

		public void ProcessPokeInteraction(Collider hitCollider, Transform interactableTransform, IXRInteractable interactable, bool useMultiPick, IXRPokeFilter pokeFilter = null)
		{
			UIDocument uidocument;
			if (!interactableTransform.TryGetComponent<UIDocument>(out uidocument))
			{
				return;
			}
			Transform transform = uidocument.transform;
			Vector3 position = this.m_Interactor.GetAttachTransform(null).position;
			Vector3 vector = -transform.forward;
			Plane plane = new Plane(vector, transform.position);
			float d;
			plane.Raycast(new Ray(position, -vector), out d);
			Vector3 vector2 = position - vector * d;
			Vector3 vector3 = vector2 + vector;
			this.UpdateVisualizers(position, vector2, vector3, vector, transform);
			VisualElement visualElement = this.PerformPick(uidocument, vector3, -vector, this.m_Interactor.pokeWidth, useMultiPick);
			if (visualElement != null)
			{
				XRUIToolkitHandler.UpdateInteractorHitData(this.m_Interactor, new InteractorHitData
				{
					closestPoint = vector2,
					interactorOrigin = position,
					interactorDirection = vector,
					hitDocument = uidocument
				});
				float num = 1f;
				if (pokeFilter != null)
				{
					IXRSelectInteractable ixrselectInteractable = interactable as IXRSelectInteractable;
					if (ixrselectInteractable != null)
					{
						num = pokeFilter.Process(this.m_Interactor, ixrselectInteractable, 0f);
					}
				}
				bool isUiSelectInputActive = num > 0.99f;
				if (this.m_UpdateDepth)
				{
					XRUIToolkitHandler.SetZDepthForInteractor(visualElement, this.m_Interactor, 20f * (1f - num));
				}
				XRUIToolkitHandler.HandlePointerUpdate(this.m_Interactor, vector3, Quaternion.LookRotation((vector2 - vector3).normalized), isUiSelectInputActive, false);
				return;
			}
			this.ResetPointerState();
		}

		public void ResetPointerState()
		{
			XRUIToolkitHandler.HandlePointerUpdate(this.m_Interactor, Vector3.zero, Quaternion.identity, false, true);
			if (this.m_UpdateDepth)
			{
				XRUIToolkitHandler.ClearZDepthForInteractor(this.m_Interactor);
			}
		}

		private VisualElement PerformPick(UIDocument document, Vector3 center, Vector3 direction, float radius, bool useMultiPick)
		{
			VisualElement visualElement = WorldSpaceInput.Pick3D(document, new Ray(center, direction));
			if (visualElement != null)
			{
				return visualElement;
			}
			if (useMultiPick)
			{
				return this.PerformMultiPick(document, center, direction, radius);
			}
			return null;
		}

		private VisualElement PerformMultiPick(UIDocument document, Vector3 center, Vector3 direction, float radius)
		{
			Dictionary<VisualElement, float> dictionary = new Dictionary<VisualElement, float>();
			Matrix4x4 localToWorldMatrix = document.transform.localToWorldMatrix;
			Vector3 a = localToWorldMatrix.GetColumn(0);
			Vector3 a2 = localToWorldMatrix.GetColumn(1);
			for (int i = 0; i < 4; i++)
			{
				float f = (float)i * 1.5707964f;
				Vector3 b = a * (Mathf.Cos(f) * radius) + a2 * (Mathf.Sin(f) * radius);
				Vector3 origin = center + b;
				VisualElement visualElement = WorldSpaceInput.Pick3D(document, new Ray(origin, direction));
				if (visualElement != null)
				{
					float sqrMagnitude = b.sqrMagnitude;
					float num;
					if (!dictionary.TryGetValue(visualElement, out num) || sqrMagnitude < num)
					{
						dictionary[visualElement] = sqrMagnitude;
					}
				}
			}
			VisualElement result = null;
			float num2 = float.MaxValue;
			foreach (KeyValuePair<VisualElement, float> keyValuePair in dictionary)
			{
				if (keyValuePair.Value < num2)
				{
					num2 = keyValuePair.Value;
					result = keyValuePair.Key;
				}
			}
			return result;
		}

		public void UpdateVisualizersState()
		{
			if (this.m_Interactor.debugVisualizationsEnabled && this.m_Interactor.isActiveAndEnabled)
			{
				this.CreateVisualizers();
				return;
			}
			this.DestroyVisualizers();
		}

		private void UpdateVisualizers(Vector3 pokePoint, Vector3 closestPoint, Vector3 rayOrigin, Vector3 normal, Transform parentTransform)
		{
			if (!this.m_Interactor.debugVisualizationsEnabled || !this.m_VisualizersCreated)
			{
				return;
			}
			this.m_PokePointVisualizer.position = pokePoint;
			this.m_ClosestPointVisualizer.position = closestPoint;
			this.m_RayOriginVisualizer.position = rayOrigin;
			this.m_NormalVisualizer.position = closestPoint + normal * 0.025f;
			this.m_NormalVisualizer.up = normal;
			if (this.m_VisualizersRoot.transform.parent != parentTransform)
			{
				this.m_VisualizersRoot.transform.SetParent(parentTransform, false);
			}
		}

		private void CreateVisualizers()
		{
			if (this.m_VisualizersCreated)
			{
				return;
			}
			this.m_VisualizersRoot = new GameObject("UIPokeVisualizers");
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			gameObject.name = "PokePoint";
			gameObject.transform.SetParent(this.m_VisualizersRoot.transform);
			gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
			Collider obj;
			if (gameObject.TryGetComponent<Collider>(out obj))
			{
				Object.Destroy(obj);
			}
			gameObject.GetComponent<Renderer>().material.color = Color.blue;
			this.m_PokePointVisualizer = gameObject.transform;
			GameObject gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			gameObject2.name = "ClosestPoint";
			gameObject2.transform.SetParent(this.m_VisualizersRoot.transform);
			gameObject2.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
			Collider obj2;
			if (gameObject2.TryGetComponent<Collider>(out obj2))
			{
				Object.Destroy(obj2);
			}
			gameObject2.GetComponent<Renderer>().material.color = Color.green;
			this.m_ClosestPointVisualizer = gameObject2.transform;
			GameObject gameObject3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			gameObject3.name = "RayOrigin";
			gameObject3.transform.SetParent(this.m_VisualizersRoot.transform);
			gameObject3.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
			Collider obj3;
			if (gameObject3.TryGetComponent<Collider>(out obj3))
			{
				Object.Destroy(obj3);
			}
			gameObject3.GetComponent<Renderer>().material.color = Color.yellow;
			this.m_RayOriginVisualizer = gameObject3.transform;
			GameObject gameObject4 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			gameObject4.name = "DocumentNormal";
			gameObject4.transform.SetParent(this.m_VisualizersRoot.transform);
			gameObject4.transform.localScale = new Vector3(0.005f, 0.05f, 0.005f);
			Collider obj4;
			if (gameObject4.TryGetComponent<Collider>(out obj4))
			{
				Object.Destroy(obj4);
			}
			gameObject4.GetComponent<Renderer>().material.color = Color.red;
			this.m_NormalVisualizer = gameObject4.transform;
			this.m_VisualizersCreated = true;
		}

		private void DestroyVisualizers()
		{
			if (this.m_VisualizersRoot != null)
			{
				Object.Destroy(this.m_VisualizersRoot);
				this.m_VisualizersCreated = false;
				this.m_PokePointVisualizer = null;
				this.m_ClosestPointVisualizer = null;
				this.m_RayOriginVisualizer = null;
				this.m_NormalVisualizer = null;
			}
		}

		private GameObject m_VisualizersRoot;

		private Transform m_PokePointVisualizer;

		private Transform m_ClosestPointVisualizer;

		private Transform m_RayOriginVisualizer;

		private Transform m_NormalVisualizer;

		private bool m_VisualizersCreated;

		private readonly XRPokeInteractor m_Interactor;

		private bool m_UpdateDepth;
	}
}
