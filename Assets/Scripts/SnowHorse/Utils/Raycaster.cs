using UnityEngine;

namespace SnowHorse.Utils
{
    public static class Raycaster
    {
        /// <summary>
        /// Finds an object with the given tag in the raycast and returns the given component if it exists.
        /// </summary>
        /// <typeparam name="T">The type of the component to find.</typeparam>
        /// <param name="data">Raycast data.</param>
        /// <returns>A raycast result with the found object and the requested component if it exists, otherwise null.</returns>
        public static RaycastResult<T> FindWithTag<T>(RaycastData data)
        {
            var result = Find<GameObject>(data);
            if (result != null && result.HitObject.CompareTag(data.FindTag)) return GetComponent<T>(result);
            return null;
        }

        /// <summary>
        /// Finds an object in the raycast and returns the given component if it exists.
        /// </summary>
        /// <typeparam name="T">The type of the component to find.</typeparam>
        /// <param name="data">Raycast data.</param>
        /// <returns>A raycast result with the found object and the requested component if it exists, otherwise null.</returns>
        public static RaycastResult<T> Find<T>(RaycastData data)
        {
            data.Ray ??= new Ray(origin: data.Origin, direction: data.Direction);

            var result = data.CastType switch
            {
                CastType.Cast3D => Cast3D(data),
                CastType.Cast2D => Cast2D(data),
                CastType.Mixed => Cast3D(data) ?? Cast2D(data),
                _ => null
            };

            DebugRaycast(data, result);
            if (result != null) return GetComponent<T>(result);
            return null;
        }

        private static RaycastResult<T> GetComponent<T>(RaycastResult<GameObject> result)
        {
            if (typeof(T) == typeof(GameObject)) return (RaycastResult<T>)(object)result;
            if (result.HitObject.TryGetComponent(out T obj)) return new RaycastResult<T>(obj, result.HitPoint);
            return null;
        }

        private static RaycastResult<GameObject> Cast3D(RaycastData data)
        {
            Physics.Raycast((Ray)data.Ray, out RaycastHit hit, data.MaxDistance, data.LayerMask.value);
            var result = hit.collider ? new RaycastResult<GameObject>(hit.collider.gameObject, hit.point) : null;
            return result;
        }

        private static RaycastResult<GameObject> Cast2D(RaycastData data)
        {
            var hit = Physics2D.GetRayIntersection((Ray)data.Ray, data.MaxDistance, data.LayerMask.value);
            var result = hit.collider ? new RaycastResult<GameObject>(hit.collider.gameObject, hit.point) : null;
            return result;
        }

        private static void DebugRaycast(RaycastData data, RaycastResult<GameObject> result)
        {
            if (!data.Debug) return;
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

    public enum CastType
    {
        Cast2D,
        Cast3D,
        Mixed
    }
}