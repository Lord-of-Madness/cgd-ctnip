using System;
using UnityEngine;

public class Tool : MonoBehaviour
{
    public string toolName;
    public Sprite toolIcon;
    public int maxLoadedAmmo;
    public int reloadBatch;
    public float reloadTime;
    public float actionTime;
    public AudioClip reloadSound;
    public AudioClip fireSound;
    public bool hasToAim;
    public bool infinteReloads;
    public AudioClip equipSound;
}
