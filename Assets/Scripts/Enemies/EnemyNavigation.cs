using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    public class EnemyNavigation : MonoBehaviour
    {
        private NavMeshAgent agent;
        private Coroutine followPath;
        private NavMeshPath path;
        private WaitForSeconds pathfindInterval = new(1);

        public void Init(NavMeshAgent navMeshAgent)
        {
            path = new NavMeshPath();
            agent = navMeshAgent;
        }

        public void FollowPath(Transform target)
        {
            Stop();
            followPath = StartCoroutine(FollowingPath(target));
        }

        private IEnumerator FollowingPath(Transform target)
        {
            agent.isStopped = false;
            while (true)
            {
                yield return pathfindInterval;
                
                NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
                agent.destination = path.corners[^1];
                
                yield return null;
            }
        }

        public void Stop()
        {
            if (followPath != null)
            {
                agent.isStopped = true;
                StopCoroutine(followPath);
                followPath = null;
            }
        }
    }
}
