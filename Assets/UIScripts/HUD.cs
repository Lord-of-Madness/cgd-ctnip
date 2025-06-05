using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private GameObject MainPortrait;
    [SerializeField] private GameObject OffPortrait;
    [SerializeField] Sprite BethPotrait;
    [SerializeField] Sprite ErikPotrait;
    public TextMeshProUGUI PromptLabel;
    Sprite MainSprite { get => MainPortrait.transform.GetChild(0).GetComponent<Image>().sprite; set => MainPortrait.transform.GetChild(0).GetComponent<Image>().sprite = value; }
    Sprite OffSprite { get => OffPortrait.transform.GetChild(0).GetComponent<Image>().sprite; set => OffPortrait.transform.GetChild(0).GetComponent<Image>().sprite = value; }
    public static HUD Instance;

    public void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        GameManager.Instance.charChanged.AddListener(()=> ShowHUD(GameManager.Instance.activeChar));
        if (GameManager.Instance.OtherPlayer == null) OffPortrait.SetActive(false);
        
    }

    public void ShowHUD(PlayerCharacter character)
    {
        switch (character)
        {
            case PlayerCharacter.Beth:
                MainSprite = BethPotrait;
                OffSprite = ErikPotrait;
                break;
            case PlayerCharacter.Erik:
                MainSprite = ErikPotrait;
                OffSprite = BethPotrait;
                break;
            default:
                Debug.LogError("Unknown character");
                break;
        }
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
