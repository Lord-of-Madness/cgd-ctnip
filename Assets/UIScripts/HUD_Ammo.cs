using System;
using System.Collections.Generic;
using UnityEngine;

public class HUD_Ammo : MonoBehaviour
{
    List<GameObject> rounds;
    public int currentAmmo;
    void Start()
    {
        rounds = new();
        foreach (RectTransform bullet in GetComponentInChildren<RectTransform>())
        {
            rounds.Add(bullet.gameObject);
        }
        Reload(currentAmmo);

        //TODO bind Fire and reload to player actions
    }
    /// <summary>
    /// Adds rounds to the drum without wasting.
    /// </summary>
    /// <param name="count"></param>
    public void Reload(int count)
    {
        currentAmmo += count;
        if (currentAmmo > rounds.Count)
        {
            currentAmmo = rounds.Count;
        }
        UpdateRounds();
    }
    void UpdateRounds()
    {
        for (int i = 0; i < rounds.Count; i++)
        {
            if (i < currentAmmo)
            {
                rounds[i].SetActive(true);
            }
            else
            {
                rounds[i].SetActive(false);
            }
        }
    }
    public void FireRound()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            UpdateRounds();
        }
    }
}
