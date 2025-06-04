using UnityEngine;

public class GramophoneSceneManager : MonoBehaviour, SaveSystem.ISaveable
{

    [SerializeField]
    SceneLightsTurnOff lightsAffectedByGenerator;

    [SerializeField]
    WirePuzzleController wirePuzzle;

    public bool GeneratorFixed { get; set; } = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SaveSystem.AddSaveable(this);

        if (GameManager.Instance.MansionKeyPickedUp)
        {
            if (GameManager.Instance.GramophoneGenFixed)
            {
                TurnGeneratorOn();
            }
            else
            {
                TurnGeneratorOff();
            }
        }
        else
        {
            TurnGeneratorOn();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TurnGeneratorOff()
    {
        //if (!GeneratorFixed) return;
        
        lightsAffectedByGenerator.TurnOffAllChildren();
        wirePuzzle.UnComplete();
        GeneratorFixed = false;
        GameManager.Instance.GramophoneGenFixed = false;
    }

    public void TurnGeneratorOn()
	{
		//if (GeneratorFixed) return;
		
        lightsAffectedByGenerator.TurnOnAllChildren();
        wirePuzzle.Finish();
        GeneratorFixed = true;
        GameManager.Instance.GramophoneGenFixed = true;
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
