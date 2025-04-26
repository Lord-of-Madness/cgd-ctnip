using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] GameObject BethHUD;
    [SerializeField] GameObject ErikHUD;
    private void Start()
    {
        BethHUD.SetActive(false);
        ErikHUD.SetActive(false);
        GameManager.Instance.charChanged.AddListener(ShowCorrectHUD);
    }

    void ShowCorrectHUD()
    {
        ShowHUD(GameManager.Instance.activeChar);
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
