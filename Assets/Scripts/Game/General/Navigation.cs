using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Game.General
{
    public class Navigation : MonoBehaviour
    {
        private NavMeshAgent agent;
        private Coroutine followPath;
        private NavMeshPath path;
        private WaitForSeconds pathFindInterval;
        private bool randomizePath;

        public void Init(NavMeshAgent navMeshAgent, float updateInterval)
        {
            path = new NavMeshPath();
            agent = navMeshAgent;
            agent.speed = 0;
            pathFindInterval = new(updateInterval);
        }
        
        public void SetAgentSpeed(float speed) => agent.speed = speed;

        public void FollowPath(Transform target, bool randomizePath, float randomPathRange)
        {
            Stop();
            this.randomizePath = randomizePath;
            followPath = StartCoroutine(FollowingPath(target, randomPathRange));
        }

        public void Stop()
        {
            agent.SetDestination(agent.transform.position);
            agent.speed = 0;
            if (followPath != null) StopCoroutine(followPath);
            agent.isStopped = true;
            //agent.updatePosition = false;
            path.ClearCorners();
        }

        public void DestroyAgent()
        {
            Stop();
            Destroy(agent);
        }

        private IEnumerator FollowingPath(Transform target, float randomPathRange)
        {
            agent.Warp(agent.transform.position); //Resets agent to new position after updating position with another method
            agent.updatePosition = true;
            agent.isStopped = false;
            
            while (true)
            {
                NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);

                if (path.corners.Length > 0)
                {
                    //Use for erratic movement such as a spider colony
                    if (randomizePath && GetRandomPoint(path.corners[^1], randomPathRange, out var point))
                    {
                        agent.destination = point;
                    }
                    else
                    {
                        agent.destination = path.corners[^1];
                    }
                }
                else
                {
                    agent.isStopped = true;
                }
                yield return pathFindInterval;
            }
        }
        
        
        //Finds a random point within a radius to have some variability on agent paths
        bool GetRandomPoint(Vector3 center, float range, out Vector3 result) {
            for (int i = 0; i < 30; i++) {
                Vector3 randomPoint = center + Random.insideUnitSphere * range;
                if (NavMesh.SamplePosition(randomPoint, out var hit, 1.0f, NavMesh.AllAreas)) {
                    result = hit.position;
                    return true;
                }
            }
            result = Vector3.zero;
            return false;
        }
    }
}
