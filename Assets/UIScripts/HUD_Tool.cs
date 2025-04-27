using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Ammo : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI PrimaryAmmo;
    [SerializeField] private TextMeshProUGUI StashedAmmo;
    void UpdateAmmoLabel()
    {
        PrimaryAmmo.text = 
            GameManager.Instance.ActivePlayer.playerData.SelectedToolData.loadedAmmo.ToString() + "/" +
            GameManager.Instance.ActivePlayer.playerData.SelectedTool.maxLoadedAmmo;
        StashedAmmo.text = GameManager.Instance.ActivePlayer.playerData.SelectedToolData.stashedAmmo.ToString();
    }
    void Start()
    {
        GameManager.Instance.bethPC.onToolUsed.AddListener(UpdateAmmoLabel);
        GameManager.Instance.erikPC.onToolUsed.AddListener(UpdateAmmoLabel);
        GameManager.Instance.bethPC.onReload.AddListener(UpdateAmmoLabel);
        GameManager.Instance.erikPC.onReload.AddListener(UpdateAmmoLabel);
    }
}
