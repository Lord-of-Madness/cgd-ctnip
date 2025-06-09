using System.Collections.Generic;
using Unity.AI.Navigation.Samples;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.IO;

public class GameManager : MonoBehaviour, SaveSystem.ISaveable
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


    //Scene specific information
    public bool MansionKeyPickedUp { get; set; } = false;
    public bool GramophoneGenFixed { get; set; } = true;
    public bool GramophoneSceneExternalChange { get; set; } = false;

    private void Awake()
    {

		if (Instance != null && Instance != this)//So it can be in multiple scenes for testing, but does not appear twice
        {
            Instance.bethPC = this.bethPC;
            Instance.erikPC = this.erikPC; //These are just references set in inspector -> have to be reinserted in each scene
            Destroy(gameObject);
            return;
        }
        if (Directory.Exists(GlobalConstants.savePath))
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

		//Player actions set here -> just to always use current activePlayer -> avoid referencing Players unloaded in other scene
		//inputActions.Player.Jump.performed += (ctx) => { if (isGrounded && controlledByPlayer) GiveCommandToActivePlayer(CharacterCommand.JUMP); };
		inputActions.Player.Sprint.performed += (ctx) => { GiveCommandToActivePlayer(CharacterCommand.TOGGLE_RUN); };
		inputActions.Player.Aim.started += (ctx) => { GiveCommandToActivePlayer(CharacterCommand.SHOW_LASER); };
		inputActions.Player.Aim.canceled += (ctx) => { GiveCommandToActivePlayer(CharacterCommand.HIDE_LASER); };
		inputActions.Player.Attack.performed += (ctx) => { GiveCommandToActivePlayer(CharacterCommand.ATTACK); };
		inputActions.Player.Reload.performed += (ctx) => { GiveCommandToActivePlayer(CharacterCommand.RELOAD); };
		inputActions.Player.SwapTools.performed += (ctx) => { GiveCommandToActivePlayer(CharacterCommand.SWITCH_TOOL); };
	}

    void GiveCommandToActivePlayer(CharacterCommand cmd)
    {
        switch (cmd)
        {
            case CharacterCommand.TOGGLE_RUN:
                ActivePlayer.ToggleRunningCommand();
				break;
            case CharacterCommand.SHOW_LASER:
                ActivePlayer.ShowLaserAimCommand();
                break;
            case CharacterCommand.HIDE_LASER:
                ActivePlayer.HideLaserAimCommand();
                break;
            case CharacterCommand.ATTACK:
                ActivePlayer.AttackCommand();
                break;
            case CharacterCommand.RELOAD:
                ActivePlayer.ReloadCommand();
                break;
            case CharacterCommand.SWITCH_TOOL:
                ActivePlayer.SwitchToolCommand();
                break;
            default:
                break;
        }
    }

    public void SwapCharacters()
    {
        if (OtherPlayer == null) {
            ActivePlayer.PlayNope();
            return; }
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
        Instance.inputActions.Player.Disable();
        
        //Fixes the player still rotating after death
        Instance.GiveCommandToActivePlayer(CharacterCommand.HIDE_LASER); 
        
        Time.timeScale = 0;
        GameOverScreenScript.instance.Show();
    }

    public static void GameLoad()
    {
		Instance.inputActions.Player.Enable();
		Time.timeScale = 1;
        GameOverScreenScript.instance.Hide();
        SaveSystem.LoadSceneData();
    }

	public void Save(SaveSystem.AllSavedData dataHolder)
	{
        dataHolder.gameManagerData = new SaveSystem.GameManagerData() { activePlayer = (int)activeChar };
	}

	public void Load(SaveSystem.AllSavedData data)
	{
        if (activeChar != (PlayerCharacter)data.gameManagerData.activePlayer)
            SwapCharacters();
	}
}

public enum PlayerCharacter
{
    Beth,
    Erik
}

public enum CharacterCommand
{
    TOGGLE_RUN,
    SHOW_LASER,
    HIDE_LASER,
    ATTACK,
    RELOAD,
    SWITCH_TOOL
}