using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    public class EnemyNavigation : MonoBehaviour
    {
        private bool isMoving = false;
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

        public void FollowPath(Transform target)
        {
            //Stop();
            followPath = StartCoroutine(FollowingPath(target));
        }

        public void Stop() => isMoving = false;

        private IEnumerator FollowingPath(Transform target)
        {
            agent.isStopped = false;
            isMoving = true;

            while (isMoving)
            {
                yield return pathfindInterval;
                
                NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
                agent.destination = path.corners[^1];
                
                yield return null;
            }

            agent.isStopped = true;
            agent.speed = 0;
            path.ClearCorners();
        }
    }
}
