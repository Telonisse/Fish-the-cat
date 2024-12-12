using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingRod : MonoBehaviour
{
    // Fishingrod cached ref
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
    private Vector3 castTarget;
    private PlayerInputs playerActions;

    // Bools for fishing 
    private bool isCasting = false;
    private bool isFishingRodActive = false;
    private bool isLineCast = false;
    

    private void Awake()
    {
        playerActions = new PlayerInputs();
    }

    // Needs to be changed to other inputs or figure out what inputs for what 
    private void OnEnable()
    {
        playerActions.Player.ToggleFishing.started += ToggleFishing;
        playerActions.Player.Fish.started += Fish;
        playerActions.Player.Retract.started += Retract;
        playerActions.Player.Enable();
    }

    private void OnDisable()
    {
        playerActions.Player.ToggleFishing.started -= ToggleFishing;
        playerActions.Player.Fish.started -= Fish;
        playerActions.Player.Retract.started += Retract;
        playerActions.Player.Disable();
    }

    private void Retract(InputAction.CallbackContext context) // right click
    {
        if (!isFishingRodActive) return;

        if (isCasting) // checks if youve casted the line 
        {
            RetractLine();
            isLineCast = false; // this bool needs to be false to be able to cast the line 
        }
    }

    private void ToggleFishing(InputAction.CallbackContext obj)
    {
        ToggleFishingRod(); // Turn on fishingrod with E (probably this one stay)
    }
    private void Fish(InputAction.CallbackContext obj) // this one is on left click maybe change/add a different input?
    {
        if (!isFishingRodActive) return; // if fishingrod not active return

        if (!isLineCast)
        {
            TryCastLine(); // Trys to cast the line 
        }
        else if (FishingSystem.Instance.isThereABite)
        {
            FishingSystem.Instance.PlayerFishing();
        }
    }
   

    // Detects when fishing rod enters fish area and gives what fish are it is
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<FishArea>(out FishArea area))
        {
            currentFishArea = area;
        }
    }

    // exits fish area and nulls the currentFishArea
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<FishArea>(out FishArea area) && currentFishArea == area)
        {
            currentFishArea = null;
        }
    }

    void Update()
    {
        if (isCasting)
        {
            UpdateLine(); // visual for line 
        }
    }

    void ToggleFishingRod() // pressed E comes here to turn on and off fishingrod
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

    void TryCastLine() // tries to cast line into water 
    {
        if (!isFishingRodActive) return;

        Vector3 castDirection = rodTip.forward; // direction
        Vector3 potentialTarget = rodTip.position + castDirection * maxCastDistance;

        if (Physics.Raycast(potentialTarget, Vector3.down, out RaycastHit hit))
        {
            if (hit.collider.CompareTag(waterTag))
            {
                // check what fish area it is 
                if (hit.collider.TryGetComponent<FishArea>(out FishArea fishArea))
                {
                    castTarget = hit.point;
                    StartCasting();
                    FishingSystem.Instance.StartFishing(fishArea.waterSource); // ref to startfishing in fishing system in correct fish area 
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

       // turn on bobbler at correct pos 
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

    void UpdateLine() // fishing lines curve shit dont look
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
            bobberInstance.transform.position = lineRenderer.GetPosition(lineSegments - 1); // bobbler at end of line 
        }
    }

    void RetractLine() // hides bobbler and retracts the line 
    {
        isCasting = false;
        lineRenderer.enabled = false;

        if (bobberInstance != null)
        {
            bobberInstance.SetActive(false);
        }
    }
}

