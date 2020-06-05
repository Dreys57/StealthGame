using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    
    [SerializeField] private LayerMask viewMask;
    
    [SerializeField] private float speed = 5f;
    [SerializeField] private float viewDistance;

    [SerializeField] private int maxDistanceTargetToPlayer = 10;

    private Vector3[] path;

    private int targetIndex;

    private float viewAngle;
    private float timer  = 5f;
    private float timerValue = 5f;

    private Grid grid;

    private Transform player;

    private Light spotlight;

    private bool playerHeard;
    private bool playerSeen;
    private bool hasFinishedPath;

    private void Start()
    {
        grid = FindObjectOfType<Grid>();

        startPos = new Vector3(transform.position.x, 2f, transform.position.z);
        
        path = new Vector3[0];
        
        player = GameObject.FindGameObjectWithTag("Player").transform;

        spotlight = GetComponentInChildren<Light>();

        viewAngle = spotlight.spotAngle;
    }

    private void FixedUpdate()
    {
        
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
                PathRequestManager.RequestPath(transform.position, target, OnPathFound);

                state = State.PATROL;
                
                break;
            case State.PATROL:

                playerSeen = CanSeePlayer();

                if (playerHeard)
                {
                    StopAllCoroutines();
                    
                    path = new Vector3[0];

                    targetIndex = 0;

                    target = player.transform.position;
                    
                    PathRequestManager.RequestPath(transform.position, target, OnPathFound);

                    state = State.SEARCH;
                }
                
                if(playerSeen)
                {
                    spotlight.color = Color.red;
                    
                    StopAllCoroutines();
                    
                    path = new Vector3[0];

                    targetIndex = 0;

                    target = player.transform.position;
                    
                    PathRequestManager.RequestPath(transform.position, target, OnPathFound);

                    state = State.CHASE;
                }
                
                if (path.Length == 0 && hasFinishedPath)
                {
                    SwapTarget();
                        
                    PathRequestManager.RequestPath(transform.position, target, OnPathFound);

                    hasFinishedPath = false;
                }
                break;
            case State.CHASE:
                
                playerSeen = CanSeePlayer();
                
                if (Vector3.Distance(target, player.transform.position) > maxDistanceTargetToPlayer)
                {
                    StopAllCoroutines();
                    
                    path = new Vector3[0];

                    targetIndex = 0;

                    target = player.transform.position;

                    PathRequestManager.RequestPath(transform.position, target, OnPathFound);
                }

                if (!playerSeen)
                {
                    StopAllCoroutines();
                    
                    path = new Vector3[0];

                    targetIndex = 0;

                    target = player.transform.position;
                    
                    PathRequestManager.RequestPath(transform.position, target, OnPathFound);

                    state = State.SEARCH;
                }

                break;
            case State.SEARCH:
                
                playerSeen = CanSeePlayer();
                
                if (hasFinishedPath)
                {
                    timer -= 1 * Time.deltaTime;
                }
                
                if (timer <= 0 && !playerSeen)
                {
                    StopAllCoroutines();
                    
                    path = new Vector3[0];

                    targetIndex = 0;

                    timer = timerValue;

                    playerHeard = false;
                    
                    target = new Vector3(Random.Range(minX, maxX), 2f, Random.Range(minY, maxY));
                    
                    PathRequestManager.RequestPath(transform.position, target, OnPathFound);

                    state = State.PATROL;
                }
                else if (timer <= 0 && playerSeen)
                {
                    StopAllCoroutines();
                    
                    path = new Vector3[0];

                    targetIndex = 0;
                    
                    timer = timerValue;

                    playerHeard = false;

                    target = player.transform.position;
                    
                    PathRequestManager.RequestPath(transform.position, target, OnPathFound);

                    state = State.CHASE;
                }
                
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerHeard = true;
        }
    }

    bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;

            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);

            if (angleBetweenGuardAndPlayer < viewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position, player.position, viewMask))
                {
                    return true;
                }
            }
        }

        return false;
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

            yield return new WaitForEndOfFrame();

            yield return null;
        }
    }
    
    public void OnDrawGizmos() {
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
        
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
