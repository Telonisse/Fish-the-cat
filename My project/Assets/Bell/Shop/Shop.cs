using UnityEngine;
using UnityEngine.InputSystem;

public class Shop : MonoBehaviour
{
    [SerializeField] bool playerIsIn;
    private PlayerInputs playerActions;

    private void Start()
    {
        playerActions = new PlayerInputs();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            playerIsIn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerIsIn = false;
        }
    }

    private void OnEnable()
    {
        playerActions.Player.Jump.started += Shopping;
        playerActions.Player.Enable();
    }

    private void OnDisable()
    {
        playerActions.Player.Jump.started -= Shopping;
        playerActions.Player.Disable();
    }

    private void Shopping(InputAction.CallbackContext obj)
    {
        if (playerIsIn)
        {
            Debug.Log("Shopping");
        }
    }
}
