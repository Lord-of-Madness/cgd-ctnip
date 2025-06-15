using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }
        gameObject.SetActive(false);
        
    }
    public void Start()
    {
        GameManager.Instance.inputActions.PauseMenu.UnPause.performed += ctx => Hide();
    }
    public void Show()
    {
        GameManager.Instance.inputActions.Player.Disable();
        GameManager.Instance.inputActions.PauseMenu.UnPause.Enable();
        gameObject.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }
    public void Hide()
    {
        GameManager.Instance.inputActions.Player.Enable();
        GameManager.Instance.inputActions.PauseMenu.UnPause.Disable();
        gameObject.SetActive(false);
        Time.timeScale = 1f; // Resume the game
    }
}
