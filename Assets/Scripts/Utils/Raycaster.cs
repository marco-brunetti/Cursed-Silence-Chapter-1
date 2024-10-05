using UnityEngine;

namespace SnowHorse.Utils
{
    public class Raycaster
    {
        public static T Find<T>(RaycastData data, out Vector3 hitPoint)
        {
            if (data.Ray == null) data.Ray = new Ray(origin: data.Origin, direction: data.Direction);
            GameObject gameObject;

            switch (data.CastType)
            {
                case CastType.Cast3D:
                    gameObject = Cast3D(data, out hitPoint);
                    break;
                case CastType.Cast2D:
                    gameObject = Cast2D(data, out hitPoint);
                    break;
                case CastType.Mixed:
                default:
                    gameObject = Cast3D(data, out hitPoint);
                    if(!gameObject) gameObject = Cast2D(data, out hitPoint);
                    break;
            }

            if(gameObject)
            {
                if (typeof(T) == typeof(GameObject)) return (T)(object)gameObject;
                else if (gameObject.TryGetComponent(out T obj)) return obj;
            }

            return default;
        }

		private static GameObject Cast3D(RaycastData data, out Vector3 hitPoint)
		{
            Physics.Raycast((Ray)data.Ray, out RaycastHit hit, data.MaxDistance, data.LayerMask.value);

            hitPoint = hit.collider ? hit.point : Vector3.zero;
            DebugRaycast(data, hit: hit);

            return hit.collider ? hit.collider.gameObject : null;
		}

		private static GameObject Cast2D(RaycastData data, out Vector3 hitPoint)
		{
            var hit = Physics2D.GetRayIntersection((Ray)data.Ray, data.MaxDistance, data.LayerMask.value);

            hitPoint = hit.collider ? hit.point : Vector3.zero;
            DebugRaycast(data, hit2D: hit);

            return hit.collider ? hit.collider.gameObject : null;
		}

		private static void DebugRaycast(RaycastData data, RaycastHit? hit = null, RaycastHit2D? hit2D = null)
		{
            if(!data.Debug) return;

            var ray = (Ray)data.Ray;
            Collider objectHit = null;
            Collider2D objectHit2D = null;
            Vector3 hitPoint;


            if (hit != null)
            {
                objectHit = ((RaycastHit)hit).collider;
                hitPoint = ((RaycastHit)hit).point;
            }
            else
            {
                objectHit2D = ((RaycastHit2D)hit2D).collider;
                hitPoint = ((RaycastHit2D)hit2D).point;
            }

            var distance = (objectHit || objectHit2D) ? Vector3.Distance(hitPoint, ray.origin) : data.MaxDistance;
            Debug.DrawRay(ray.origin, ray.direction * distance, color: (objectHit || objectHit2D) ? data.HitDebugColor : data.NonHitDebugColor);
        }
	}
    
    public class RaycastData
    {
        public Ray? Ray = null;
        public Vector3 Origin = Vector3.zero;
        public Vector3 Direction = Vector3.one;
        public CastType CastType = CastType.Mixed;
        public float MaxDistance = 10000;
        public LayerMask LayerMask = -1;
        public bool Debug = false;
        public Color NonHitDebugColor = Color.red;
        public Color HitDebugColor = Color.green;
    }
}

public enum CastType
{
	Cast2D,
	Cast3D,
	Mixed
}