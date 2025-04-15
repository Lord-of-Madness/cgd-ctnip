using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    public bool firstCharacterActive = true;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Dialogue.Instance.ShowCharacterWithText(new DialogueLine("Hello, world!", "Player", null));
        Dialogue.Instance.ShowCharacterWithText(new List<DialogueLine>()
        {
            new ("This is a test.", "Player", null),
            new ("How are you?", "Player", null),
            new ("Goodbye!", "Player", null)
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwapCharacters()
    {
        if (firstCharacterActive) 
        { 
            EnableCameraFilter();
			RenderSettings.ambientSkyColor = Color.gray;
        }
        else
        {
            DisableCameraFilter();
            RenderSettings.ambientSkyColor = Color.black;
		}

		firstCharacterActive = !firstCharacterActive;
    }

    void DisableCameraFilter()
    {
        Camera camera = Camera.main;
        if (camera == null) return;

        Volume volume = camera.GetComponent<Volume>();

        if (volume == null) return;

        volume.enabled = false;
    }

	void EnableCameraFilter()
	{
		Camera camera = Camera.main;
		if (camera == null) return;

		Volume volume = camera.GetComponent<Volume>();

        if (volume == null) { Debug.Log("No volume found"); return; }

		volume.enabled = true;
	}
}
