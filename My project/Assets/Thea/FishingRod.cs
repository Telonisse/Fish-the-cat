using UnityEngine;

public class FishingRod : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform rodTip;
    [SerializeField] private string waterTag = "Water";
    [SerializeField] private float maxCastDistance = 15f;      
    [SerializeField] private int lineSegments = 30;
    [SerializeField] private MeshRenderer rodMeshRenderer;

    [SerializeField] private GameObject bobberPrefab; 
    private GameObject currentBobber;

    private bool isCasting = false;          
    private Vector3 castTarget;              
    private bool isFishingRodActive = true;  

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleFishingRod();
        }

        if (!isFishingRodActive) return; 

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

    void ToggleFishingRod()
    {
        isFishingRodActive = !isFishingRodActive; 

        if (rodMeshRenderer != null)
        {
            rodMeshRenderer.enabled = isFishingRodActive;
        }

        if (lineRenderer != null)
        {
            if (isFishingRodActive)
            {
                lineRenderer.gameObject.SetActive(true);
            }
            else
            {
                ResetLineRenderer(); 
            }
        }
    }

    void ResetLineRenderer()
    {
        lineRenderer.positionCount = 0;
        isCasting = false;
    }

    void TryCastLine()
    {
        if (!isFishingRodActive) return;

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

        if (currentBobber == null && bobberPrefab != null)
        {
            currentBobber = Instantiate(bobberPrefab, castTarget, Quaternion.identity);
            PositionBobber();
        }
    }

    void PositionBobber()
    {
        if (currentBobber != null)
        {
            Vector3 position = castTarget;
            position.y += 0.2f; 
            currentBobber.transform.position = position;
        }
    }

    void UpdateLine()
    {
        if (currentBobber == null) return;

        lineRenderer.positionCount = lineSegments;

        Vector3 start = rodTip.position;
        Vector3 end = currentBobber.transform.position;
        Vector3 direction = end - start;

        for (int i = 0; i < lineSegments; i++)
        {
            float t = (float)i / (lineSegments - 1);
            Vector3 point = Vector3.Lerp(start, end, t);
            point.y -= Mathf.Sin(t * Mathf.PI) * direction.magnitude * 0.2f;
            lineRenderer.SetPosition(i, point);
        }
    }

    void RetractLine()
    {
        isCasting = false;
        lineRenderer.enabled = false;

        if (currentBobber != null)
        {
            Destroy(currentBobber);
            currentBobber = null;
        }
    }
}

