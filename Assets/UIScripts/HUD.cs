using UnityEngine;

public class HUD : MonoBehaviour
{
    public enum PlayerCharacter
    {
        Beth,
        Erik
    }
    [SerializeField] GameObject BethHUD;
    [SerializeField] GameObject ErikHUD;
    private void Start()
    {
        BethHUD.SetActive(false);
        ErikHUD.SetActive(false);
    }
    public void ShowHUD(PlayerCharacter character)
    {
        switch (character)
        {
            case PlayerCharacter.Beth:
                BethHUD.SetActive(true);
                ErikHUD.SetActive(false);
                break;
            case PlayerCharacter.Erik:
                ErikHUD.SetActive(true);
                BethHUD.SetActive(false);
                break;
            default:
                Debug.LogError("Unknown character");
                break;
        }
    }
}
