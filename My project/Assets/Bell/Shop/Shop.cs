using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shop : MonoBehaviour
{
    [SerializeField] bool playerIsIn;
    private PlayerInputs playerActions;
    [SerializeField] string shopText;
    private PauseMenu pauseMenu;
    [SerializeField] GameObject shopMenu;
    private DialogueSystem dialogueSystem;

    private void Awake()
    {
        playerActions = new PlayerInputs();
        pauseMenu = FindFirstObjectByType<PauseMenu>();
        dialogueSystem = FindFirstObjectByType<DialogueSystem>();
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
        playerActions.Player.Fish.started += ShowShopping;
        playerActions.Player.Enable();
    }

    private void OnDisable()
    {
        playerActions.Player.Interact.started -= Shopping;
        playerActions.Player.Fish.started -= ShowShopping;
        playerActions.Player.Disable();
    }

    private void Shopping(InputAction.CallbackContext context)
    {
        if (playerIsIn)
        {
            Debug.Log("Shopping");
            //Start shop
            if (pauseMenu.IsPaused() == false)
            {
                pauseMenu.PauseGame();
                dialogueSystem.StartText(shopText);
            }
            else if (pauseMenu.IsPaused() == true)
            {
                pauseMenu.PauseGame();
                shopMenu.SetActive(false);
                dialogueSystem.StopText();
            }
        }
    }

    private void ShowShopping(InputAction.CallbackContext context)
    {
        if (shopMenu.activeSelf == false && playerIsIn && pauseMenu.IsPaused() == true && dialogueSystem.IsTextDone() == true)
        {
            dialogueSystem.StopText();
            shopMenu.SetActive(true);
        }
    }
}
