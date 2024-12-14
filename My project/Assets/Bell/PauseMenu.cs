using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] bool paused;

    private PlayerInputs playerActions;
    private void Awake()
    {
        playerActions = new PlayerInputs();
    }

    private void OnEnable()
    {
        playerActions.Player.Pause.started += Pause;
        playerActions.Player.Enable();
    }

    private void OnDisable()
    {
        playerActions.Player.Pause.started -= Pause;
        playerActions.Player.Disable();
    }

    private void Pause(InputAction.CallbackContext context)
    {
        if (pauseMenu != null)
        {
            if (pauseMenu.activeSelf == false)
            {
                pauseMenu.SetActive(true);
                Time.timeScale = 0f;
            }
            else if (pauseMenu.activeSelf == true)
            {
                pauseMenu.SetActive(false);
                Time.timeScale = 1f;
            }
        }
        PauseGame();
    }

    public void PauseGame()
    {
        if (paused == false)
        {
            paused = true;
        }
        else if (paused == true)
        {
            paused = false;
        }
    }

    public bool IsPaused()
    {
        return paused;
    }
}
