using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private GameObject MainPortrait;
    [SerializeField] private GameObject OffPortrait;
    [SerializeField] List<Sprite> BethPotrait;
    [SerializeField] List<Sprite> ErikPotrait;
    public TextMeshProUGUI PromptLabel;
    Sprite MainSprite { get => MainPortrait.transform.GetChild(0).GetComponent<Image>().sprite; set => MainPortrait.transform.GetChild(0).GetComponent<Image>().sprite = value; }
    Sprite OffSprite { get => OffPortrait.transform.GetChild(0).GetComponent<Image>().sprite; set => OffPortrait.transform.GetChild(0).GetComponent<Image>().sprite = value; }
    public static HUD Instance;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    private void Start()
    {
        GameManager.Instance.charChanged.AddListener(() => ShowHUD(GameManager.Instance.activeChar));
        BindHPChanged();


        GameManager.Instance.charsReassigned.AddListener(() => {
            if (GameManager.Instance.OtherPlayer == null) OffPortrait.SetActive(false);
            BindHPChanged();
            ShowHUD(GameManager.Instance.activeChar); });
        if (GameManager.Instance.OtherPlayer == null) OffPortrait.SetActive(false);
        
    }
    public void BindHPChanged()
    {
        GameManager.Instance.erikPC.playerData.OnHPChanged.AddListener(() => ShowHUD(GameManager.Instance.activeChar));
        GameManager.Instance.bethPC.playerData.OnHPChanged.AddListener(() => ShowHUD(GameManager.Instance.activeChar));
    }


    public void ShowHUD(PlayerCharacter character)
    {
        switch (character)
        {
            case PlayerCharacter.Beth:
                MainSprite = BethPotrait[GameManager.Instance.bethPC.playerData.HP];
                OffSprite = ErikPotrait[GameManager.Instance.erikPC.playerData.HP];
                break;
            case PlayerCharacter.Erik:
                MainSprite = ErikPotrait[GameManager.Instance.erikPC.playerData.HP];
                OffSprite = BethPotrait[GameManager.Instance.bethPC.playerData.HP];
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
