using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Journal : MonoBehaviour
{
    Image Backcover;
    PlayerCharacter currentCharacter;
    void Start()
    {
        Backcover = GameObject.Find("Backcover").GetComponent<Image>();
        GameObject.Find("BethPortrait").GetComponent<Button>().onClick.AddListener(() => { if(currentCharacter != PlayerCharacter.Beth) SwitchCharacter(PlayerCharacter.Beth); });
        GameObject.Find("ErikPortrait").GetComponent<Button>().onClick.AddListener(() => { if(currentCharacter != PlayerCharacter.Erik) SwitchCharacter(PlayerCharacter.Erik); });
        SwitchCharacter(PlayerCharacter.Beth);
        GameManager.Instance.inputActions.Player.Journal.performed += ctx => Show();
        GameManager.Instance.inputActions.UI.Cancel.performed += ctx => Hide();
        //GameManager.Instance.inputActions.UI.JournalExit.performed += ctx => Hide(); TODO remove from UI and make it separate perhaps
        Hide();
    }
    public void Show()
    {
        //TODO pause game
        GameManager.Instance.inputActions.Player.Disable();
        GameManager.Instance.inputActions.UI.Enable();
        gameObject.SetActive(true);
        HUD.Instance.Hide();
    }
    public void Hide()
    {
        GameManager.Instance.inputActions.Player.Enable();
        gameObject.SetActive(false);
        HUD.Instance.Show();
    }
    public void SwitchCharacter(PlayerCharacter character) {
        currentCharacter = character;
        Debug.Log("Switching CHaracter");
    }
    public void OnNotesPressed()
    {
        OnBookmarkPressed();
        Debug.Log("Dokumenty");
    }
    public void OnInventoryPressed()
    {
        OnBookmarkPressed();
        Debug.Log("Inventráè");
    }
    public void OnCodexPressed()
    {
        OnBookmarkPressed();
        Debug.Log("Ne nefixnu to typo nahoøe");
    }
    void OnBookmarkPressed()
    {
        Debug.Log("Záložkavìc");
    }
}
