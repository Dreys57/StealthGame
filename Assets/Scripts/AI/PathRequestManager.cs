using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;

        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 start_, Vector3 end_, Action<Vector3[], bool> callback_)
        {
            pathStart = start_;
            pathEnd = end_;
            callback = callback_;
        }
    }

    private Queue<PathRequest> pathRequests = new Queue<PathRequest>();

    private PathRequest currentPathRequest;

    private static PathRequestManager instance;

    private Pathfinding pathfinding;

    private bool isPorcessingPath;

    private void Awake()
    {
        instance = this;

        pathfinding = GetComponent<Pathfinding>();
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        
        instance.pathRequests.Enqueue(newRequest);
        
        instance.TryProcessNext();
    }

    private void TryProcessNext()
    {
        if (!isPorcessingPath && pathRequests.Count > 0)
        {
            currentPathRequest = pathRequests.Dequeue();

            isPorcessingPath = true;

            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool sucess)
    {
        currentPathRequest.callback(path, sucess);
        
        Debug.Log("ici");
        
        isPorcessingPath = false;
        
        TryProcessNext();
    }
}
