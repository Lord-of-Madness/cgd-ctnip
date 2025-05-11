using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Ammo : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI PrimaryAmmo;
    [SerializeField] private TextMeshProUGUI StashedAmmo;

    bool firstUpdate = true;
    void UpdateAmmoLabel()
    {
        Debug.Log(GameManager.APD.SelectedTool.name);
        image.sprite =
            GameManager.APD.SelectedTool.toolIcon;
        PrimaryAmmo.text = 
            GameManager.APD.SelectedToolData.loadedAmmo.ToString() + "/" +
            GameManager.APD.SelectedTool.maxLoadedAmmo;
        StashedAmmo.text = GameManager.Instance.ActivePlayer.playerData.SelectedToolData.stashedAmmo.ToString();
    }
    void Start()
    {
        GameManager.Instance.bethPC.onToolUsed.AddListener(UpdateAmmoLabel);
        GameManager.Instance.erikPC.onToolUsed.AddListener(UpdateAmmoLabel);
        GameManager.Instance.bethPC.onReload.AddListener(UpdateAmmoLabel);
        GameManager.Instance.erikPC.onReload.AddListener(UpdateAmmoLabel);
        GameManager.Instance.bethPC.onToolSwitched.AddListener(UpdateAmmoLabel);
        GameManager.Instance.erikPC.onToolSwitched.AddListener(UpdateAmmoLabel);
    }

	private void Update()
	{
		if (firstUpdate)
        {
            UpdateAmmoLabel();
            firstUpdate = false;
        }
	}
}
