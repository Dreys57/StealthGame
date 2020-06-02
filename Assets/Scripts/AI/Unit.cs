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

    [SerializeField] private float speed = 5f;

    private Vector3[] path;

    private int targetIndex;

    private Grid grid;

    private bool playerHeard;
    private bool playerSeen;

    private void Start()
    {
        grid = FindObjectOfType<Grid>();
    }

    private void Update()
    {
        float minX = -(grid.GridWorldSize.x / 2);
        float maxX = grid.GridWorldSize.x / 2;
        float minY = -(grid.GridWorldSize.y / 2);
        float maxY = grid.GridWorldSize.y / 2;
        
        switch (state)
        {
            case State.IDLE:
                
                target = new Vector3(Random.Range(minX, maxX), transform.position.y, Random.Range(minY, maxY));

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
                    }
                    
                    PathRequestManager.RequestPath(transform.position, target, OnPathFound);
                }
                break;
            case State.CHASE:
                break;
            case State.SEARCH:
                break;
        }
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
                    yield break;
                }

                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);

            yield return null;
        }
    }
}
