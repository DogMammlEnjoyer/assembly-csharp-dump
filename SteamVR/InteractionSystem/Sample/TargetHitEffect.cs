using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class TargetHitEffect : MonoBehaviour
	{
		private void OnCollisionEnter(Collision collision)
		{
			if (collision.collider == this.targetCollider)
			{
				ContactPoint contactPoint = collision.contacts[0];
				float d = 1f;
				Ray ray = new Ray(contactPoint.point - -contactPoint.normal * d, -contactPoint.normal);
				RaycastHit raycastHit;
				if (collision.collider.Raycast(ray, out raycastHit, 2f) && this.colorSpawnedObject)
				{
					Color color = ((Texture2D)collision.gameObject.GetComponent<Renderer>().material.mainTexture).GetPixelBilinear(raycastHit.textureCoord.x, raycastHit.textureCoord.y);
					if (color.r > 0.7f && color.g > 0.7f && color.b < 0.7f)
					{
						color = Color.yellow;
					}
					else if (Mathf.Max(new float[]
					{
						color.r,
						color.g,
						color.b
					}) == color.r)
					{
						color = Color.red;
					}
					else if (Mathf.Max(new float[]
					{
						color.r,
						color.g,
						color.b
					}) == color.g)
					{
						color = Color.green;
					}
					else
					{
						color = Color.yellow;
					}
					color *= 15f;
					GameObject gameObject = Object.Instantiate<GameObject>(this.spawnObjectOnCollision);
					gameObject.transform.position = contactPoint.point;
					gameObject.transform.forward = ray.direction;
					foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
					{
						renderer.material.color = color;
						if (renderer.material.HasProperty("_EmissionColor"))
						{
							renderer.material.SetColor("_EmissionColor", color);
						}
					}
				}
				Debug.DrawRay(ray.origin, ray.direction, Color.cyan, 5f, true);
				if (this.destroyOnTargetCollision)
				{
					Object.Destroy(base.gameObject);
				}
			}
		}

		public Collider targetCollider;

		public GameObject spawnObjectOnCollision;

		public bool colorSpawnedObject = true;

		public bool destroyOnTargetCollision = true;
	}
}
