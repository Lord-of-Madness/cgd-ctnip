using System.Collections.Generic;
using Unity.AI.Navigation.Samples;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerController character1;
    public PlayerController character2;

    Camera m_mainCamera;

    public bool firstCharacterActive = true;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(Instance);
    }
    void Start()
    {
        m_mainCamera = Camera.main;
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
			character1.gameObject.GetComponent<NavMeshAgent>().enabled = true;
			character1.gameObject.GetComponent<AITarget>().enabled = true;
			character1.gameObject.GetComponent<AgentLinkMover>().enabled = true;
            character1.enabled = false;
			character2.gameObject.GetComponent<NavMeshAgent>().enabled = false;
			character2.gameObject.GetComponent<AITarget>().enabled = false;
			character2.gameObject.GetComponent<AgentLinkMover>().enabled = false; 
            character2.enabled = true;
            m_mainCamera.GetComponent<FollowPlayer>().player = character2.gameObject;
        }
        else
        {
            DisableCameraFilter();
            RenderSettings.ambientSkyColor = Color.black;
			character1.gameObject.GetComponent<NavMeshAgent>().enabled = false;
			character1.gameObject.GetComponent<AITarget>().enabled = false;
			character1.gameObject.GetComponent<AgentLinkMover>().enabled = false; 
            character1.enabled = true;
			character2.gameObject.GetComponent<NavMeshAgent>().enabled = true;
			character2.gameObject.GetComponent<AITarget>().enabled = true;
			character2.gameObject.GetComponent<AgentLinkMover>().enabled = true;
			character2.enabled = false;
			m_mainCamera.GetComponent<FollowPlayer>().player = character1.gameObject;
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
