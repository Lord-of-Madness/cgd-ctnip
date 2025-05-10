using System;
using UnityEngine;

public class Tool : MonoBehaviour
{
    public string toolName;
    public Sprite toolIcon;
    public int maxLoadedAmmo;
    public int reloadBatch;
    public float reloadTime;
    public AudioClip reloadSound;
    public AudioClip fireSound;
    public bool hasToAim;
}
