using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shop : MonoBehaviour
{
    [SerializeField] bool playerIsIn;
    private PlayerInputs playerActions;
    [SerializeField] string shopText;
    private void Awake()
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
        playerActions.Player.Interact.started += Shopping;
        playerActions.Player.Enable();
    }

    private void OnDisable()
    {
        playerActions.Player.Interact.started -= Shopping;
        playerActions.Player.Disable();
    }

    private void Shopping(InputAction.CallbackContext context)
    {
        if (playerIsIn)
        {
            Debug.Log("Shopping");
            //Start shop
            if (Time.timeScale == 1)
            {
                //call pause in another script??
                //Time.timeScale = 0;
                FindFirstObjectByType<DialogueSystem>().StartText(shopText);
            }
            else if (Time.timeScale == 0)
            {
                //call play in another script??
                //Time.timeScale = 1;
            }
        }
    }
}
