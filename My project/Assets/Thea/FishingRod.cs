using UnityEngine;
using UnityEngine.InputSystem;

public class FishingRod : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform rodTip;
    [SerializeField] private string waterTag = "Water";
    [SerializeField] private float maxCastDistance = 15f;
    [SerializeField] private int lineSegments = 30;
    [SerializeField] private MeshRenderer rodMeshRenderer;
    [SerializeField] private GameObject bobberPrefab;

    // For connecting fish scripts 
    [SerializeField] private FishingSystem fishingSystem; 
    [SerializeField] private FishArea currentFishArea;    

    private GameObject bobberInstance;
    private bool isCasting = false;
    private Vector3 castTarget;
    private bool isFishingRodActive = false;
    private bool isLineCast = false;
    private PlayerInputs playerActions;

    private void Awake()
    {
        playerActions = new PlayerInputs();
    }

    private void OnEnable()
    {
        playerActions.Player.ToggleFishing.started += ToggleFishing;
        playerActions.Player.Fish.started += Fish;
        playerActions.Player.Enable();
    }

    private void OnDisable()
    {
        playerActions.Player.ToggleFishing.started -= ToggleFishing;
        playerActions.Player.Fish.started -= Fish;
        playerActions.Player.Disable();
    }

    private void ToggleFishing(InputAction.CallbackContext obj)
    {
        ToggleFishingRod();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<FishArea>(out FishArea area))
        {
            currentFishArea = area;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<FishArea>(out FishArea area) && currentFishArea == area)
        {
            currentFishArea = null;
        }
    }

    private void Fish(InputAction.CallbackContext obj)
    {

        if (!isFishingRodActive) return;

        if (!isLineCast)
        {
            TryCastLine();
        }
        else
        {
            if (isCasting)
            {
                RetractLine();
                isLineCast = false;
            }
        }
    }

    void Update()
    {
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

        if (isFishingRodActive)
        {
            lineRenderer.enabled = false;
            lineRenderer.positionCount = 0;
            isCasting = false;
            isLineCast = false;
        }
        else
        {
            ResetLineRenderer();
            if (bobberInstance != null)
            {
                Destroy(bobberInstance);
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
                // checking for fish area script
                if (hit.collider.TryGetComponent<FishArea>(out FishArea fishArea))
                {
                    castTarget = hit.point;
                    StartCasting();
                    FishingSystem.Instance.StartFishing(fishArea.waterSource); 
                    Debug.Log($"Fishing in {fishArea.waterSource}");
                }
                else
                {
                    Debug.Log("not valid water");
                }
            }
            else
            {
                Debug.Log("You can only fish it water");
            }
        }
        else
        {
            Debug.Log("nothing valid found");
        }
    }

    void StartCasting()
    {
        isCasting = true;
        lineRenderer.enabled = true;

       
        if (bobberInstance == null && bobberPrefab != null)
        {
            bobberInstance = Instantiate(bobberPrefab);
        }

        if (bobberInstance != null)
        {
            bobberInstance.SetActive(true);
            bobberInstance.transform.position = castTarget;
        }
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

       
        if (bobberInstance != null)
        {
            bobberInstance.transform.position = lineRenderer.GetPosition(lineSegments - 1);
        }
    }

    void RetractLine()
    {
        isCasting = false;
        lineRenderer.enabled = false;

        if (bobberInstance != null)
        {
            bobberInstance.SetActive(false);
        }
    }
}

