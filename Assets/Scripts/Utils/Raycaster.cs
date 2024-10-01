using UnityEngine;
using static UnityEngine.UI.Image;
using UnityEngine.Rendering;

namespace SnowHorse.Utils
{
    public class Raycaster
    {
        public static T Cast<T>(Ray ray, out Vector3 hitPoint, CastType castType = CastType.mixed, float maxDistance = Mathf.Infinity, LayerMask layerMask = new LayerMask(), bool debugMode = false, Color debugColor = new Color(), Color hitDebugColor = new Color())
        {
            var o = Cast(ray, out hitPoint, castType, maxDistance, layerMask, debugMode, debugColor, hitDebugColor);

            if (o && o.TryGetComponent(out T obj)) return obj;
            else return default;
        }

        public static GameObject Cast(Vector3 origin, Vector3 direction, out Vector3 hitPoint, CastType castType = CastType.mixed, float maxDistance = Mathf.Infinity, LayerMask layerMask = new LayerMask(), bool debugMode = false, Color debugColor = new Color(), Color hitDebugColor = new Color())
        {
            return Cast(new Ray(origin, direction), out hitPoint, castType, maxDistance, layerMask, debugMode, debugColor, hitDebugColor);
        }

        public static GameObject Cast(Ray ray, out Vector3 hitPoint, CastType castType = CastType.mixed, float maxDistance = Mathf.Infinity, LayerMask layerMask = new LayerMask(), bool debugMode = false, Color debugColor = new Color(), Color hitDebugColor = new Color())
        {
			if (layerMask.value == 0) layerMask = -1;
            GameObject objectHit;

            switch (castType)
            {
                case CastType.cast3D:
                    objectHit = Cast3D(ray, maxDistance, layerMask, out hitPoint);
                    break;
                case CastType.cast2D:
                    objectHit = Cast2D(ray, maxDistance, layerMask, out hitPoint);
                    break;
                default:
                case CastType.mixed:
                    objectHit = Cast3D(ray, maxDistance, layerMask, out hitPoint);
                    if(!objectHit) objectHit = Cast2D(ray, maxDistance, layerMask, out hitPoint);
                    break;
            }

			if(debugMode) DebugRaycast(debugColor, hitDebugColor, ray, maxDistance, objectHit, hitPoint);
            return objectHit;
        }

		private static GameObject Cast3D(Ray ray, float maxDistance, LayerMask layerMask, out Vector3 hitPoint)
		{
            hitPoint = Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask) ? hit.point : Vector3.zero;
			return hit.collider ? hit.collider.gameObject : null;
		}

		private static GameObject Cast2D(Ray ray, float maxDistance, LayerMask layerMask, out Vector3 hitPoint)
		{
            var hit = Physics2D.GetRayIntersection(ray, maxDistance, layerMask);
            hitPoint = hit.collider ? hit.point : Vector3.zero;
            return hit.collider ? hit.collider.gameObject : null;
		}

		private static void DebugRaycast(Color nonHitColor, Color hitColor, Ray ray, float maxDistance, GameObject objectHit, Vector3 hitPoint)
		{
            var nonHitCol = nonHitColor.a == 0 ? Color.red : nonHitColor;
            var hitCol = hitColor.a == 0 ? Color.green : hitColor;
            var color = objectHit ? hitCol : nonHitCol;

            var distance = objectHit ? Vector3.Distance(hitPoint, ray.origin) : maxDistance == Mathf.Infinity ? 10000 : maxDistance;
            Debug.DrawRay(ray.origin, ray.direction * distance, color);
        }
	}
}

public enum CastType
{
	cast2D,
	cast3D,
	mixed
}