using UnityEngine;
using UnityEngine.AI;

public class DungeonGame_NavMeshObstacleFix : MonoBehaviour
{
    private void Start()
    {
        NavMeshObstacle obstacle = GetComponent<NavMeshObstacle>();
        if (obstacle != null)
        {
            obstacle.enabled = false;
            obstacle.enabled = true;
        }
    }
}
