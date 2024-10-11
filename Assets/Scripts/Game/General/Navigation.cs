using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    public class Navigation : MonoBehaviour
    {
        private NavMeshAgent agent;
        private Coroutine followPath;
        private NavMeshPath path;
        private WaitForSeconds pathfindInterval = new(1);

        public void Init(NavMeshAgent navMeshAgent)
        {
            path = new NavMeshPath();
            agent = navMeshAgent;
            agent.speed = 0;
        }
        
        public void SetAgentSpeed(float speed) => agent.speed = speed;

        public void FollowPath(Transform target)
        {
            Stop();
            followPath = StartCoroutine(FollowingPath(target));
        }

        public void Stop()
        {
            if (followPath != null) StopCoroutine(followPath);
            agent.isStopped = true;
            agent.speed = 0;
            path.ClearCorners();
        }

        private IEnumerator FollowingPath(Transform target)
        {
            agent.isStopped = false;

            while (true)
            {
                NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
                agent.destination = path.corners[^1];
                yield return pathfindInterval;
            }
        }
    }
}
