using System;
using UnityEngine;

public static class JamUtil
{
	public static bool IsPlaying
	{
		get
		{
			return Application.isPlaying;
		}
	}

	public static void Destroy(Object obj)
	{
		Object.Destroy(obj);
	}

	public static RaycastHit ToRaycastHit(this Collision collision)
	{
		RaycastHit result;
		if (collision.ConvertToRaycast(out result))
		{
			Debug.Log(string.Format("Hit {0} [{1}]", result.collider.name, collision.contacts[0].otherCollider == result.collider), result.collider);
		}
		else
		{
			Debug.LogError(string.Format("No hit! ({0})", collision));
		}
		return result;
	}

	public static bool ConvertToRaycast(this Collision collision, out RaycastHit hit)
	{
		Vector3 point = collision.contacts[0].point;
		Vector3 normal = collision.contacts[0].normal;
		LayerMask mask = 1 << collision.gameObject.layer;
		bool result = Physics.Raycast(new Ray(point + normal * 0.01f, -normal), out hit, 0.1f, mask, QueryTriggerInteraction.Ignore);
		Debug.Log(string.Format("Match: {0}", collision.contacts[0].otherCollider == hit.collider));
		return result;
	}
}
