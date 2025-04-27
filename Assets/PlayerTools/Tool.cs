using UnityEngine;

public class Tool : MonoBehaviour
{
    public string toolName;
    [SerializeField] private Sprite toolIcon;
    public int maxLoadedAmmo;
    public int reloadBatch;
}
