using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Game.General
{
    public class Navigation : MonoBehaviour
    {
        private CustomNavMeshAgent agent;
        private Coroutine followPath;
        private NavMeshPath path;
        private WaitForSeconds pathFindInterval;
        private bool randomizePath;

        public void Init(CustomNavMeshAgent navMeshAgent, float updateInterval)
        {
            path = new NavMeshPath();
            agent = navMeshAgent;
            agent.Speed = 0;
            pathFindInterval = new(updateInterval);
        }
        
        public void SetAgentSpeed(float speed) => agent.Speed = speed;

        public void FollowPath(Transform target, bool randomizePath, float randomPathRange)
        {
            Stop();
            this.randomizePath = randomizePath;
            followPath = StartCoroutine(FollowingPath(target, randomPathRange));
        }

        public void Stop(bool stopAgentCompletely = false)
        {
            agent.SetDestination(agent.transform.position);
            agent.Speed = 0;
            if (followPath != null) StopCoroutine(followPath);
            agent.isStopped = true;
            
            //Use for special attacks that require the agent from exiting the navmesh
            if(stopAgentCompletely) agent.updatePosition = false; 
            
            path.ClearCorners();
        }

        public void DestroyAgent()
        {
            Stop();
            Destroy(agent);
        }

        private IEnumerator FollowingPath(Transform target, float randomPathRange)
        {
            //Resets agent after using another movement method
            agent.Warp(agent.transform.position);
            agent.updatePosition = true;
            agent.isStopped = false;
            
            while (true)
            {
                NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);

                if (path.corners.Length > 0)
                {
                    agent.destination = randomizePath && GetRandomPoint(path.corners[^1], randomPathRange, out var point) ? point : path.corners[^1];
                }
                else
                {
                    agent.isStopped = true;
                }
                
                yield return pathFindInterval;
            }
        }
        
        //Adds variability to agent paths for erratic movement
        private bool GetRandomPoint(Vector3 center, float radius, out Vector3 result) {
            for (int i = 0; i < 30; i++) {
                Vector3 randomPoint = center + Random.insideUnitSphere * radius;
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
