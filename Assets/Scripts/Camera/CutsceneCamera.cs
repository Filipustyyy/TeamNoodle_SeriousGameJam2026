using System;
using System.Collections;
using Camera;
using UnityEngine;

public class CutsceneCamera : MonoBehaviour
{
    [Header("Path Points")]
    [Tooltip("Drag your empty waypoint GameObjects in here, in the order the camera should visit them.")]
    [SerializeField] private CutsceneWaypoint[] waypoints;

    [Header("Settings")]
    [SerializeField] private float timeBetweenPoints = 2.5f;

    private UnityEngine.Camera _cam;
    
    private void Awake()
    {
        _cam = GetComponent<UnityEngine.Camera>();
    }
    
    private void Start() {
        StartCutscene();
    }

    public void StartCutscene()
    {
        if (waypoints == null || waypoints.Length < 2)
        {
            Debug.LogWarning("CutsceneCameraPath needs at least 2 waypoints to move!");
            return;
        }
        StartCoroutine(FollowPathRoutine());
    }
    
    private IEnumerator FollowPathRoutine()
    {
        transform.position = waypoints[0].point.position;
        _cam.orthographicSize = waypoints[0].zoom;
        for (int i = 1; i < waypoints.Length; i++)
        {
            Transform startPoint = waypoints[i - 1].point;
            Transform targetPoint = waypoints[i].point;
            
            float startZoom = waypoints[i - 1].zoom;
            float targetZoom = waypoints[i].zoom;
            
            float elapsedTime = 0f;

            while (elapsedTime < timeBetweenPoints)
            {
                elapsedTime += Time.deltaTime;
                
                float percent = Mathf.SmoothStep(0f, 1f, elapsedTime / timeBetweenPoints);
                
                transform.position = Vector3.Lerp(startPoint.position, targetPoint.position, percent);
                
                float currentZoom = Mathf.Lerp(startZoom, targetZoom, percent);
                if (_cam.orthographic) _cam.orthographicSize = currentZoom;
                else _cam.fieldOfView = currentZoom;
                
                yield return null;
            }
        }
        Debug.Log("Cutscene Finished!");
    }
    
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i].point != null && waypoints[i + 1].point != null)
            {
                Gizmos.DrawLine(waypoints[i].point.position, waypoints[i + 1].point.position);
                Gizmos.DrawSphere(waypoints[i].point.position, 0.3f);
            }
        }
        
        if (waypoints[waypoints.Length - 1].point != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(waypoints[waypoints.Length - 1].point.position, 0.3f);
        }
    }
}
