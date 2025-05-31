using System.Collections.Generic;
using Unity.AI.Navigation.Samples;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    /// <summary>
    /// Shorthand to get active playerData
    /// </summary>
    public static PlayerData APD { get=>Instance.ActivePlayer.playerData; }
    static Dictionary<string, string> sgd;
    public static Dictionary<string, string> SpeakerGlobalData
    {
        get
        {
            if (sgd == null)
            {
                sgd = new Dictionary<string, string>();
                string json = Resources.Load<TextAsset>("Dialogues/_SPEAKERS").text;
                Debug.Log(json);
                SpeakerGlobalSettings speakerGlobalSettings = JsonUtility.FromJson<SpeakerGlobalSettings>(json);
                
                foreach (var pair in speakerGlobalSettings.speakers)
                {
                    sgd[pair.Speaker] = pair.Hex;
                }
            }
            return sgd;
        }
        set
        {
            sgd = value;
        }
    }

    public PlayerController bethPC;
    public PlayerController erikPC;
    public PlayerController ActivePlayer { get => activeChar == PlayerCharacter.Beth ? bethPC : erikPC; }

    public PlayerCharacter activeChar = PlayerCharacter.Beth;
    bool followingOn = true;

    public InputActionsGen inputActions;

    public UnityEvent charChanged;

    private void Awake()
    {
        inputActions = new();
        inputActions.Player.Enable();
        Instance = this;
        DontDestroyOnLoad(Instance);
    }
    void Start()
    {
        inputActions.Player.SwapCharacters.performed += ctx => SwapCharacters();
        inputActions.Player.ToggleFollowing.performed += ctx => ToggleFollowing();
    }

	public void SwapCharacters()
    {
        var cameraFollowScript = Camera.main.GetComponent<FollowPlayer>();
         
        if (activeChar == PlayerCharacter.Beth) 
        {
            //Debug.Log("Switching from Beth to Erik");
            EnableCameraFilter();

            if (followingOn) bethPC.StartFollowingOtherChar();
            bethPC.DisablePlayerControl();

			erikPC.StopFollowingOtherChar();
            erikPC.EnablePlayerControl();

            if (cameraFollowScript != null) cameraFollowScript.player = erikPC.gameObject;
		    activeChar = PlayerCharacter.Erik;
        }
		else
        {
			//Debug.Log("Switching from Erik to Beth");
			DisableCameraFilter();

            bethPC.StopFollowingOtherChar();
            bethPC.EnablePlayerControl();

            if(followingOn) erikPC.StartFollowingOtherChar();
			erikPC.DisablePlayerControl();

			if (cameraFollowScript != null) cameraFollowScript.player = bethPC.gameObject;
			activeChar = PlayerCharacter.Beth;
		}
        charChanged.Invoke();
    }

    void ToggleFollowing()
    {
        followingOn = !followingOn;
        if (bethPC == null || erikPC == null) return;
        if (followingOn)
        {
            if (activeChar == PlayerCharacter.Beth)
                erikPC.StartFollowingOtherChar();
            else
                bethPC.StartFollowingOtherChar();
        }
        else
        {
			if (activeChar == PlayerCharacter.Beth)
				erikPC.StopFollowingOtherChar();
			else
				bethPC.StopFollowingOtherChar();
		}
    }
    public void DisableCameraFilter()
    {
        Camera camera = Camera.main;
		if (camera == null) { Debug.LogWarning("No main camera found!"); return; }

		
		if (!camera.TryGetComponent<Volume>(out var volume)) { Debug.Log("No volume found"); return; }

		RenderSettings.ambientSkyColor = Color.black;

		volume.enabled = false;
    }

	public void EnableCameraFilter()
	{
		Camera camera = Camera.main;
        if (camera == null) { Debug.LogWarning("No main camera found!"); return; }

		
        if (!camera.TryGetComponent<Volume>(out var volume)) { Debug.Log("No volume found"); return; }

		RenderSettings.ambientSkyColor = Color.gray;

		volume.enabled = true;
	}

    public void UpdateCameraFilterState()
    {
        if (activeChar == PlayerCharacter.Erik) EnableCameraFilter();
        else DisableCameraFilter();
    }
}

public enum PlayerCharacter
{
	Beth,
	Erik
}