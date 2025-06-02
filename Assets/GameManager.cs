using System.Collections.Generic;
using Unity.AI.Navigation.Samples;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    /// <summary>
    /// Shorthand to get active playerData
    /// </summary>
    public static PlayerData APD { get => Instance.ActivePlayer.playerData; }
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
    public PlayerController OtherPlayer { get => activeChar == PlayerCharacter.Erik ? bethPC : erikPC; }

    public PlayerCharacter activeChar = PlayerCharacter.Beth;
    bool followingOn = true;

    public InputActionsGen inputActions;

    public UnityEvent charChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)//So it can be in multiple scenes for testing, but does not appear twice
        {
            Instance.bethPC = this.bethPC;
            Instance.erikPC = this.erikPC; //These are just references set in inspector -> have to be reinserted in each scene
            Destroy(gameObject);
            return;
        }
        Directory.Delete(GlobalConstants.savePath, true);
        Instance = this;
        inputActions = new();
        DontDestroyOnLoad(Instance);
        inputActions.Player.Enable();//TODO this will have to be moved to some scene init, not GameManager
    }
    void Start()
    {
        inputActions.Player.SwapCharacters.performed += ctx => SwapCharacters();
        inputActions.Player.ToggleFollowing.performed += ctx => ToggleFollowing();
    }

    public void SwapCharacters()
    {
        if (followingOn) ActivePlayer.StartFollowingOtherChar();
        ActivePlayer.DisablePlayerControl();

        OtherPlayer.StopFollowingOtherChar();
        OtherPlayer.EnablePlayerControl();

        if (Camera.main.TryGetComponent(out FollowPlayer cameraFollowScript)) cameraFollowScript.player = OtherPlayer.gameObject;

        activeChar = activeChar == PlayerCharacter.Beth ? PlayerCharacter.Erik : PlayerCharacter.Beth;
        UpdateCameraFilterState();
        charChanged.Invoke();
    }

    void ToggleFollowing()
    {
        followingOn = !followingOn;
        if (bethPC == null || erikPC == null) return;
        if (followingOn)
        {
            OtherPlayer.StartFollowingOtherChar();
        }
        else
        {
            OtherPlayer.StopFollowingOtherChar();
        }
    }
    public void DisableCameraFilter()
    {
        Camera camera = Camera.main;
        if (camera == null) { Debug.LogWarning("No main camera found!"); return; }


        if (!camera.TryGetComponent(out Volume volume)) { Debug.Log("No volume found"); return; }

        RenderSettings.ambientSkyColor = Color.black;

        volume.enabled = false;
    }

    public void EnableCameraFilter()
    {
        Camera camera = Camera.main;
        if (camera == null) { Debug.LogWarning("No main camera found!"); return; }


        if (!camera.TryGetComponent(out Volume volume)) { Debug.Log("No volume found"); return; }

        RenderSettings.ambientSkyColor = Color.gray;

        volume.enabled = true;
    }

    public void UpdateCameraFilterState()
    {
        if (activeChar == PlayerCharacter.Erik) EnableCameraFilter();
        else DisableCameraFilter();
    }

    public static void GameOver()
    {
        Instance.inputActions.Disable();
        Time.timeScale = 0;
        GameOverScreenScript.instance.Show();
    }

    public static void GameLoad()
    {
		Instance.inputActions.Enable();
		Time.timeScale = 1;
        GameOverScreenScript.instance.Hide();
        SaveSystem.Load();
    }
}

public enum PlayerCharacter
{
    Beth,
    Erik
}