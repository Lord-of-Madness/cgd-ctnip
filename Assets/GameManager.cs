using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
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
}
