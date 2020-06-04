using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class Unit : MonoBehaviour
{

    public enum State
    {
        IDLE,
        PATROL,
        CHASE,
        SEARCH
    }

    private State state = State.IDLE;

    private Vector3 target;
    private Vector3 startPos;

    [SerializeField] private float speed = 5f;

    private Vector3[] path;

    private int targetIndex;

    private Grid grid;

    private bool playerHeard;
    private bool playerSeen;
    private bool hasFinishedPath;

    private void Start()
    {
        grid = FindObjectOfType<Grid>();

        startPos = new Vector3(transform.position.x, 2f, transform.position.z);
        
        path = new Vector3[0];
    }

    private void Update()
    {
        float minX = -(grid.GridWorldSize.x / 4);
        float maxX = grid.GridWorldSize.x / 4;
        float minY = -(grid.GridWorldSize.y / 4);
        float maxY = grid.GridWorldSize.y / 4;
        
        switch (state)
        {
            case State.IDLE:
                
                target = new Vector3(Random.Range(minX, maxX), 2f, Random.Range(minY, maxY));
                Debug.Log("idle" + target + startPos);
                PathRequestManager.RequestPath(transform.position, target, OnPathFound);

                state = State.PATROL;
                
                break;
            case State.PATROL:

                if (playerHeard)
                {
                    state = State.SEARCH;
                }
                else
                {
                    if (path.Length == 0 && hasFinishedPath)
                    {
                        Debug.Log(transform.position);
                        SwapTarget();
                        Debug.Log("patrol" + target + startPos);
                        PathRequestManager.RequestPath(transform.position, target, OnPathFound);

                        hasFinishedPath = false;
                    }
                }
                break;
            case State.CHASE:
                break;
            case State.SEARCH:
                break;
        }
    }

    void SwapTarget()
    {
        Vector3 tempPos = startPos;

        startPos = target;

        target = tempPos;
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            
            StopCoroutine(FollowPath());
            StartCoroutine(FollowPath());
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];

        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;

                if (targetIndex >= path.Length)
                {
                    targetIndex = 0;
                    
                    path = new Vector3[0];

                    hasFinishedPath = true;
                    
                    yield break;
                }

                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            
            yield return null;
        }
    }
    
    public void OnDrawGizmos() {
        if (path != null) {
            for (int i = targetIndex; i < path.Length; i ++) {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex) {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else {
                    Gizmos.DrawLine(path[i-1],path[i]);
                }
            }
        }
    }
}
