using UnityEngine;

public class FishingRod : MonoBehaviour
{
    public LineRenderer lineRenderer;   
    public Transform rodTip;           
    public string waterTag = "Water"; 
    public float maxCastDistance = 15f; 
    public int lineSegments = 30;       

    private bool isCasting = false;
    private Vector3 castTarget;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left-click to cast
        {
            TryCastLine();
        }

        if (Input.GetMouseButtonDown(1) && isCasting) // Right-click to retract
        {
            RetractLine();
        }

        if (isCasting)
        {
            UpdateLine(); // Continuously update the line while casting
        }
    }

    void TryCastLine()
    {
        // Calculate the forward direction from the rod tip
        Vector3 castDirection = rodTip.forward;
        Vector3 potentialTarget = rodTip.position + castDirection * maxCastDistance;

        // Raycast downwards from the potential target
        if (Physics.Raycast(potentialTarget, Vector3.down, out RaycastHit hit))
        {
            if (hit.collider.CompareTag(waterTag))
            {
                castTarget = hit.point; // End the line exactly at the ground point
                StartCasting();
            }
        }
        else
        {
            Debug.Log("No valid ground detected within range.");
        }
    }

    void StartCasting()
    {
        isCasting = true;
        lineRenderer.enabled = true;
    }

    void UpdateLine()
    {
        lineRenderer.positionCount = lineSegments;

        Vector3 start = rodTip.position;
        Vector3 direction = castTarget - start;

        for (int i = 0; i < lineSegments; i++)
        {
            float t = (float)i / (lineSegments - 1); // Normalize between 0 and 1
            Vector3 point = Vector3.Lerp(start, castTarget, t); // Linear interpolation
            point.y -= Mathf.Sin(t * Mathf.PI) * direction.magnitude * 0.2f; // Add curve for gravity
            lineRenderer.SetPosition(i, point);
        }
    }

    void RetractLine()
    {
        isCasting = false;
        lineRenderer.enabled = false;
    }
}

