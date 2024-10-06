using UnityEngine;

namespace SnowHorse.Utils
{
    public class Raycaster
    {
        public static RaycastResult<GameObject> FindWithTag(RaycastData data)
        {
            var result = Find<GameObject>(data);

            if (result != null && result.HitObject.CompareTag(data.FindTag)) return result;
            return null;
        }


        public static RaycastResult<T> Find<T>(RaycastData data)
        {
            data.Ray ??= new Ray(origin: data.Origin, direction: data.Direction);
            RaycastResult<GameObject> result;

            result = data.CastType switch
            {
                CastType.Cast3D => Cast3D(data),
                CastType.Cast2D => Cast2D(data),
                CastType.Mixed => Cast3D(data) ?? Cast2D(data),
                _ => null
            };

            if(result != null && typeof(T) != typeof(GameObject))
            {
                if (result.HitObject.TryGetComponent(out T obj)) return new RaycastResult<T>(obj, result.HitPoint);
                else return null;
            }

            return (RaycastResult<T>)(object)result;
        }

		private static RaycastResult<GameObject> Cast3D(RaycastData data)
		{
            Physics.Raycast((Ray)data.Ray, out RaycastHit hit, data.MaxDistance, data.LayerMask.value);
            var result = hit.collider ? new RaycastResult<GameObject>(hit.collider.gameObject, hit.point) : null;
            DebugRaycast(data, result);
            return result;
		}

		private static RaycastResult<GameObject> Cast2D(RaycastData data)
		{
            var hit = Physics2D.GetRayIntersection((Ray)data.Ray, data.MaxDistance, data.LayerMask.value);
            var result = hit.collider ? new RaycastResult<GameObject>(hit.collider.gameObject, hit.point) : null;
            DebugRaycast(data, result);
            return result;
		}

		private static void DebugRaycast(RaycastData data, RaycastResult<GameObject> result)
		{
            if(!data.Debug) return;

            var ray = (Ray)data.Ray;
            var distance = (result != null) ? Vector3.Distance((Vector3)result.HitPoint, ray.origin) : data.MaxDistance;
            Debug.DrawRay(ray.origin, ray.direction * distance, color: (result != null) ? data.HitDebugColor : data.NonHitDebugColor);
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
        public string FindTag = string.Empty;
    }

    public class RaycastResult<T>
    {
        public readonly T HitObject;
        public readonly Vector3? HitPoint;

        public RaycastResult(T hitObject = default, Vector3? hitPoint = null)
        {
            HitObject = hitObject;
            HitPoint = hitPoint;
        }
    }
}

public enum CastType
{
	Cast2D,
	Cast3D,
	Mixed
}