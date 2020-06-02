using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GuardStateMachine : MonoBehaviour
{
    public enum State
    {
        IDLE,
        PATROL,
        CHASE,
        SEARCH
    }
    
    [SerializeField] private float speed = 5f;

    private Pathfinding pathfinding;

    private Grid grid;

    private State state = State.IDLE;

    private Vector3 target;

    private bool playerHeard;
    private bool playerSeen;

    void Start()
    {
        pathfinding = FindObjectOfType<Pathfinding>();

        grid = FindObjectOfType<Grid>();
        
        //pathfinding.Seeker = transform;
    }
    
    void Update()
    {
        float minX = -(grid.GridWorldSize.x / 2);
        float maxX = grid.GridWorldSize.x / 2;
        float minY = -(grid.GridWorldSize.y / 2);
        float maxY = grid.GridWorldSize.y / 2;
        
        switch (state)
        {
            case State.IDLE:
                
                target = new Vector3(Random.Range(minX, maxX), transform.position.y, Random.Range(minY, maxY));

                //pathfinding.Target = target;
                
                state = State.PATROL;

                break;
            case State.PATROL:

                if (playerHeard)
                {
                    state = State.SEARCH;
                }
                else
                {
                    if (Vector3.Distance(transform.position,target) < 0.1f)
                    {
                        target = new Vector3(Random.Range(minX, maxX), transform.position.y, Random.Range(minY, maxY));
                        //pathfinding.Target = target;
                    }
                    else
                    {
                        FollowPath(grid.Path);
                    }
                }
                
                break;
            case State.CHASE:
                break;
            case State.SEARCH:
                break;
        }
    }

    private void FollowPath(List<PathfindingNode> path)
    {
        int targetWaypointIndex = 1;
        
        Vector3 targetWaypoint = path[targetWaypointIndex].WorldPosition;
        
        transform.LookAt(targetWaypoint);

        while (path.Count > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);

            if (transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % path.Count;

                targetWaypoint = path[targetWaypointIndex].WorldPosition;
            }
        }
    }
}
