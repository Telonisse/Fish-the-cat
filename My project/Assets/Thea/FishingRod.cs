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
        if (Input.GetMouseButtonDown(0)) 
        {
            TryCastLine();
        }

        if (Input.GetMouseButtonDown(1) && isCasting) 
        {
            RetractLine();
        }

        if (isCasting)
        {
            UpdateLine(); 
        }
    }

    void TryCastLine()
    {
        Vector3 castDirection = rodTip.forward;
        Vector3 potentialTarget = rodTip.position + castDirection * maxCastDistance;

        if (Physics.Raycast(potentialTarget, Vector3.down, out RaycastHit hit))
        {
            if (hit.collider.CompareTag(waterTag))
            {
                castTarget = hit.point; 
                StartCasting();
            }
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
            float t = (float)i / (lineSegments - 1); 
            Vector3 point = Vector3.Lerp(start, castTarget, t); 
            point.y -= Mathf.Sin(t * Mathf.PI) * direction.magnitude * 0.2f; 
            lineRenderer.SetPosition(i, point);
        }
    }

    void RetractLine()
    {
        isCasting = false;
        lineRenderer.enabled = false;
    }
}

