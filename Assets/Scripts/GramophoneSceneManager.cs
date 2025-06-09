using UnityEngine;
using UnityEngine.SceneManagement;

public class GramophoneSceneManager : MonoBehaviour, SaveSystem.ISaveable
{

    [SerializeField]
    SceneLightsTurnOff lightsAffectedByGenerator;

    [SerializeField]
    WirePuzzleController wirePuzzle;

    [SerializeField]
    DoorOpen doorsToBeDisabledAfterGenFix;
    [SerializeField]
    GameObject sceneTransitionToBeEnableAfterGenFix;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip ambientMusicIntro;
    [SerializeField] AudioClip ambientMusicFinale;
    [SerializeField] TextAsset GeneratorDialogueJSON;
    [SerializeField] Transform generatorPosition;
    [SerializeField] TextAsset DoorDialogueJSON;
    [SerializeField] Transform doorPosition;
    [SerializeField] Trigger trigger;
    [SerializeField] bool cinematicMode =false;
    public bool GeneratorFixed { get; set; } = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SaveSystem.AddGeneralSaveable(this);

        if (GameManager.Instance.MansionKeyPickedUp)
        {
            if (GameManager.Instance.GramophoneGenFixed)
            {
                audioSource.clip = ambientMusicFinale;
                TurnGeneratorOn();
                audioSource.Play();
            }
            else
            {
                TurnGeneratorOff();
                audioSource.Stop();
            }
        }
        else
        {
            audioSource.clip = ambientMusicIntro;
            TurnGeneratorOn();
            audioSource.Play();
            if(cinematicMode) trigger.gameObject.SetActive(true);
        }
    }

	private void OnDestroy()
	{
        SaveSystem.RemoveGenSaveable(this);
	}

	public void TurnGeneratorOff()
    {
        //if (!GeneratorFixed) return;
        doorsToBeDisabledAfterGenFix.enabled = true;
		sceneTransitionToBeEnableAfterGenFix.SetActive(false);

		lightsAffectedByGenerator.TurnOffAllChildren();
        wirePuzzle.UnComplete();
        GeneratorFixed = false;
        GameManager.Instance.GramophoneGenFixed = false;
    }

    public void TurnGeneratorOn()
	{
        //if (GeneratorFixed) return;
        
        if (GameManager.Instance.MansionKeyPickedUp)
        {
            doorsToBeDisabledAfterGenFix.enabled = false;
			sceneTransitionToBeEnableAfterGenFix.SetActive(true);
        }
        else
        {
			doorsToBeDisabledAfterGenFix.enabled = true;
			sceneTransitionToBeEnableAfterGenFix.SetActive(false);
		}

            lightsAffectedByGenerator.TurnOnAllChildren();
        wirePuzzle.Finish();
        GeneratorFixed = true;
        GameManager.Instance.GramophoneGenFixed = true;
    }
    public void GoToBasementScene()
    {
        SceneTransitionManager.LoadNewScene("BasementScene");
    }

    public void SaveGeneric(SaveSystem.AllSavedData dataHolder)
	{
        dataHolder.gramophoneLevelData = new SaveSystem.GramophoneLevelData { generatorFixed = GeneratorFixed };
	}

	public void LoadGeneric(SaveSystem.AllSavedData data)
	{
        //First gramophone entry
        if (data.gramophoneLevelData == null) { TurnGeneratorOn(); return; }

        //State changed externally -> don't count on the saved data
        if (GameManager.Instance.GramophoneSceneExternalChange) { GameManager.Instance.GramophoneSceneExternalChange = false;  return; }

        if (data.gramophoneLevelData.generatorFixed && !GeneratorFixed) TurnGeneratorOn();
        else if (!data.gramophoneLevelData.generatorFixed && GeneratorFixed) TurnGeneratorOff();
	}

	public void SaveSceneSpecific(SaveSystem.AllSavedData dataHolder)
	{
		return; //All info in this manager should transition to all other scenes 
	}

	public void LoadSceneSpecific(SaveSystem.AllSavedData data)
	{
		return; //All info in this manager should transition to all other scenes 
	}
    public void CinematicStart()
    {
        generatorPosition.gameObject.SetActive(true);
        doorPosition.gameObject.SetActive(true);
        GameManager.Instance.ActivePlayer.unlessIFuckingWantTo = true;
        GameManager.Instance.ActivePlayer.DisablePlayerControl();
        GameManager.Instance.ActivePlayer.gameObject.GetComponent<AITarget>().target = generatorPosition;
        GameManager.Instance.ActivePlayer.StartFollowingOtherChar();


    }
    public void GeneratorReached()
    {
        Dialogue.Instance.dialogueEnded.AddListener(() =>
        {
            GameManager.Instance.ActivePlayer.gameObject.GetComponent<AITarget>().target = doorPosition;
            Dialogue.Instance.dialogueEnded.RemoveAllListeners();
        });
        Dialogue.Instance.ShowCharacterWithText(DialogueTreeNode.DeserializeTree(GeneratorDialogueJSON));
    }
    public void DoorReached()
    {
        Dialogue.Instance.dialogueEnded.AddListener(() =>
        {
            GameManager.Instance.ActivePlayer.StopFollowingOtherChar();
            GameManager.Instance.ActivePlayer.gameObject.GetComponent<AITarget>().target = GameManager.Instance.OtherPlayer.transform;
            GameManager.Instance.ActivePlayer.EnablePlayerControl();
            GameManager.Instance.ActivePlayer.unlessIFuckingWantTo = false;
            Dialogue.Instance.dialogueEnded.RemoveAllListeners();
        });
        Dialogue.Instance.ShowCharacterWithText(DialogueTreeNode.DeserializeTree(DoorDialogueJSON));
    }
}
