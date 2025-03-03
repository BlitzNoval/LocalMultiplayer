using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultipleTargetCamera : MonoBehaviour
{
    public List<Transform> targets = new List<Transform>();

    public Vector3 offset;
    public float smoothTime = .5f;

    public float maxZoom = 40f;
    public float minZoom = 10f;
    public float zoomLimiter = 50f;

    private Vector3 velocity;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        UpdateTargets(); // Initial population of the targets list
    }

    void LateUpdate()
    {
        UpdateTargets(); // Continuously check for new players

        if (targets.Count == 0)
            return;

        Move();
        Zoom();
    }

    void UpdateTargets()
    {
        targets.Clear(); // Reset the list to avoid duplicates

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player1");
        foreach (GameObject player in players)
        {
            targets.Add(player.transform);
        }

        players = GameObject.FindGameObjectsWithTag("Player2");
        foreach (GameObject player in players)
        {
            targets.Add(player.transform);
        }
    }

    void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
    }

    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + offset;
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    float GetGreatestDistance()
    {
        if (targets.Count == 0) return 0f;

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 1; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.size.x;
    }

    Vector3 GetCenterPoint()
    {
        if (targets.Count == 1)
        {
            return targets[0].position;
        }

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 1; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.center;
    }
}
