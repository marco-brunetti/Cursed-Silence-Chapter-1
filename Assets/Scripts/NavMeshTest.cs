// ShowGoldenPath
using UnityEngine;
using UnityEngine.AI;

public class NavMeshTest : MonoBehaviour
{
    // public Transform target;
    // private NavMeshPath path;
    // private float elapsed = 0.0f;
    //
    // private int currentPathIndex = 0;
    //
    // void Start()
    // {
    //     path = new NavMeshPath();
    //     elapsed = 0.0f;
    //     
    //     NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
    //     
    //     NavMeshAgent agent = GetComponent<NavMeshAgent>();
    //     agent.destination = path.corners[^1]; 
    // }
    //
    // void Update()
    // {
    //     // Update the way to the goal every second.
    //     // elapsed += Time.deltaTime;
    //     // if (elapsed > 1.0f)
    //     // {
    //     //     elapsed -= 1.0f;
    //     //     NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
    //     // }
    //     // for (int i = 0; i < path.corners.Length - 1; i++)
    //     //     Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
    //
    //     // if (path.corners.Length > 0 && currentPathIndex < path.corners.Length)
    //     // {
    //     //     if (Vector3.Distance(transform.position, path.corners[currentPathIndex]) < 0.5f)
    //     //     {
    //     //         currentPathIndex++;
    //     //     }
    //     // }
    //     
    //     //if (path.corners.Length > 0) transform.position = Vector3.MoveTowards(transform.position, path.corners[currentPathIndex], Time.deltaTime * 5.0f);
    //     
    // }
}