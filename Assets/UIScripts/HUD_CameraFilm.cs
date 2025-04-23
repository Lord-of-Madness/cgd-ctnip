using System.Collections.Generic;
using UnityEngine;

public class HUD_CameraFilm : MonoBehaviour
{
    List<GameObject> flashCharges;
    int currentFilmLength;
    public int maxFilmLength;
    [SerializeField] GameObject flashChargePrefab;
    Transform FilmSlotBar;
    void Start()
    {
        flashCharges = new();
        FilmSlotBar = transform.GetChild(0);
        currentFilmLength = maxFilmLength;
        for (int i=0; i < maxFilmLength; i++)
        {
            flashCharges.Add(Instantiate(flashChargePrefab, FilmSlotBar));
        }
        Reload(currentFilmLength);

        //TODO bind Fire and reload to player actions
    }
    /// <summary>
    /// Adds rounds to the drum without wasting.
    /// </summary>
    /// <param name="count"></param>
    public void Reload(int count)
    {
        currentFilmLength += count;
        if (currentFilmLength > flashCharges.Count)
        {
            currentFilmLength = flashCharges.Count;
        }
        UpdateFilm();
    }
    void UpdateFilm()
    {
        for (int i = 0; i < flashCharges.Count; i++)
        {
            if (i < currentFilmLength)
            {
                flashCharges[i].SetActive(true);
            }
            else
            {
                flashCharges[i].SetActive(false);
            }
        }
    }
    public void FireFlash()
    {
        if (currentFilmLength > 0)
        {
            currentFilmLength--;
            UpdateFilm();
        }
    }
}
