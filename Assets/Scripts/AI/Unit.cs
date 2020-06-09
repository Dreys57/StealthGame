using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Serialization;
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
    [SerializeField] private LayerMask playerMask;

    [SerializeField] private GameObject AStar;
    
    [SerializeField] private float speed = 5f;
    [SerializeField] private float turnSpeed = 90f;
    [SerializeField] private float viewDistance;
    [SerializeField] private float waitTime = 0.3f;
    [SerializeField] private float sphereRadius;

    [SerializeField] private int maxDistanceTargetToPlayer = 10;

    private Vector3[] path;

    private int targetIndex;

    private float viewAngle;
    private float timer  = 5f;
    private float timerValue = 5f;
    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    private Grid grid;

    private PathRequestManager requestManager;

    private GameObject player;
    private PlayerMovement playerMovement;

    private Light spotlight;

    private bool playerHeard;
    private bool playerSeen;
    private bool hasFinishedPath;

    private void Start()
    {
        grid = AStar.gameObject.GetComponent<Grid>();

        requestManager = AStar.gameObject.GetComponent<PathRequestManager>();

        startPos = new Vector3(transform.position.x, 2f, transform.position.z);

        player = GameObject.FindGameObjectWithTag("Player");

        playerMovement = player.gameObject.GetComponent<PlayerMovement>();

        spotlight = GetComponentInChildren<Light>();

        viewAngle = spotlight.spotAngle;
    }

    private void FixedUpdate()
    {
        switch (state)
        { 
            case State.IDLE:

                if (grid.FinishedLevelGeneration1)
                {
                    minX = grid.transform.position.x - (grid.GridWorldSize.x / 2) + 4;
                    maxX = grid.transform.position.x + (grid.GridWorldSize.x / 2) - 4;
                    minY = grid.transform.position.z - (grid.GridWorldSize.y / 2) + 4;
                    maxY = grid.transform.position.z + (grid.GridWorldSize.y / 2) - 4;
                    
                    target = new Vector3(Random.Range(minX, maxX), 2f, Random.Range(minY, maxY));
                    
                    StopAllCoroutines();
                    
                    path = new Vector3[0];

                    targetIndex = 0;
                    
                    PathRequestManager.RequestPath(transform.position, target, OnPathFound);

                    state = State.PATROL;
                }

                break;
            case State.PATROL:

                playerHeard = CanHearPlayer();
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
                    StopAllCoroutines();
                    
                    path = new Vector3[0];

                    targetIndex = 0;
                    
                    SwapTarget();
                    
                    hasFinishedPath = false;
                    
                    PathRequestManager.RequestPath(transform.position, target, OnPathFound);
                }
                
                break;
            case State.CHASE:
                
                playerHeard = CanHearPlayer();
                playerSeen = CanSeePlayer();

                if (player.transform.position.x > grid.GridWorldSize.x || player.transform.position.y > grid.GridWorldSize.y)
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

    private bool CanHearPlayer()
    {
        if (Physics.CheckSphere(transform.position, sphereRadius, playerMask))
        {
            if (!playerMovement.IsCrouching)
            {
                return true;
            }
        }

        return false;
    }

    private bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;

            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);

            if (angleBetweenGuardAndPlayer < viewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position, player.transform.position, viewMask))
                {
                    return true;
                }
            }
        }

        return false;
    }

    IEnumerator TurnToFaceTarget(Vector3 lookTarget)
    {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;

        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);

            transform.eulerAngles = Vector3.up * angle;

            yield return null;
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
        Vector3 currentWaypoint = new Vector3();
        
        if (path.Length == 0)
        {
            currentWaypoint = target;
        }
        else
        {
            currentWaypoint = path[0];
        }
        
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

                yield return new WaitForSeconds(waitTime);

                yield return StartCoroutine(TurnToFaceTarget(currentWaypoint));
            }
            
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);

            yield return null;
        }
    }
    
    public void OnDrawGizmos() {
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
        
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
        
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
