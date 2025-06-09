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
    public bool GeneratorFixed { get; set; } = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SaveSystem.AddSaveable(this);

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
        }
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
        SceneManager.LoadScene("BasementScene");
    }

    public void Save(SaveSystem.AllSavedData dataHolder)
	{
        dataHolder.gramophoneLevelData = new SaveSystem.GramophoneLevelData { generatorFixed = GeneratorFixed };
	}

	public void Load(SaveSystem.AllSavedData data)
	{
        if (GameManager.Instance.GramophoneSceneExternalChange) { GameManager.Instance.GramophoneSceneExternalChange = false;  return; }
        if (data.gramophoneLevelData.generatorFixed && !GeneratorFixed) TurnGeneratorOn();
        else if (!data.gramophoneLevelData.generatorFixed && GeneratorFixed) TurnGeneratorOff();
	}
}
