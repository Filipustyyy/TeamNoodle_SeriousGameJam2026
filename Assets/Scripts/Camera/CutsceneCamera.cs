using System;
using System.Collections;
using Camera;
using UnityEngine;

public class CutsceneCamera : MonoBehaviour
{
    [Header("Path Points")]
    [Tooltip("Set the size, drag in your waypoints, and check the boxes where images should change.")]
    [SerializeField] private CutsceneWaypoint[] waypoints;

    [Header("Settings")]
    [SerializeField] private float timeBetweenPoints = 2.5f;
    
    [Tooltip("Next image after this waypoint?")]
    [SerializeField] private bool nextImage = false;

    public static Action OnNextImageRequested;

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
        // 1. Snap to the first point
        transform.position = waypoints[0].point.position;

        // 2. Travel the path
        for (int i = 1; i < waypoints.Length; i++)
        {
            Transform startPoint = waypoints[i - 1].point;
            Transform targetPoint = waypoints[i].point;
            
            float elapsedTime = 0f;

            while (elapsedTime < timeBetweenPoints)
            {
                elapsedTime += Time.deltaTime;
                float percent = Mathf.SmoothStep(0f, 1f, elapsedTime / timeBetweenPoints);

                transform.position = Vector3.Lerp(startPoint.position, targetPoint.position, percent);

                yield return null;
            }

            // Snap perfectly to the target at the end of the movement just in case
            transform.position = targetPoint.position;

            // --- THE TRIGGER ---
            // If the checkbox for this specific waypoint is checked, fire the event!
            if (waypoints[i].triggerNextImage)
            {
                OnNextImageRequested?.Invoke();
            }
        }
        
        Debug.Log("Camera Path Finished!");
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
