using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class ToolInvData
{
    public int loadedAmmo = 0;
    public int stashedAmmo = 20;
}
[Serializable]
public class Document
{
    public enum DocumentType
    {
        Documents,
        Codex,
        Inventory
    }
    public string name;
    public List<Page> pages;
    public DocumentType type;
    public Document(string name, List<Page> pages, DocumentType type)
    {
        this.name = name;
        if (pages == null || pages.Count == 0)
        {
            Debug.LogWarning("Document without pages");
            pages = new() { new("WHY DID YOU ADD EMPTY PAGE YOU SILLY BUGGER?") };
        }
        this.pages = pages;
        this.type = type;
    }
    public Document(string name,string text, DocumentType type)
    {
        this.name = name;
        pages = new() {new(text) };
        this.type = type;
    }
}
[Serializable]
public class Page {
    public string text;
    public Page(string text)
    {
        this.text = text;
    }
}
public class PlayerData : MonoBehaviour
{
    public enum ToolUseExcuses
    {
        OutOfAmmo,
        GottaAim,
        AllClear,
        NoToolSelected
    }

    [SerializeField] public List<Tool> toolInspectorField;
    public Dictionary<Tool, ToolInvData> toolInventory = new();
    public Tool SelectedTool => toolInspectorField[selectedToolIndex];
    int selectedToolIndex = 0;
    public ToolInvData SelectedToolData => toolInventory[SelectedTool];

    public Document.DocumentType lastTypeAdded = Document.DocumentType.Documents;
    public List<Document> Documents = new();
    public List<Document> Codex = new();
    public List<Document> Inventory = new();

    int LoadedAmmo { get => SelectedToolData.loadedAmmo; set => SelectedToolData.loadedAmmo = value; }
    int StashedAmmo { get => SelectedToolData.stashedAmmo; set => SelectedToolData.stashedAmmo = value; }
    public int MaxLoadedAmmo => SelectedTool.maxLoadedAmmo;
    public int ReloadBatch => SelectedTool.reloadBatch;

    private void Start()
    {
        if (toolInspectorField != null && toolInspectorField.Count != 0)
        {
            selectedToolIndex = 0;
        }
        foreach (var tool in toolInspectorField)
        {
            toolInventory.Add(tool, new() { loadedAmmo = tool.maxLoadedAmmo });
        }
    }

    public ToolUseExcuses CanUseTool()
    {
        if(SelectedTool == null)
        {
            Debug.Log("No tool selected");
            return ToolUseExcuses.NoToolSelected;
        }
        if(SelectedTool.hasToAim && !GameManager.Instance.ActivePlayer.aimLaserVisible)
        {
            Debug.Log("Tool has to be aimed");
            return ToolUseExcuses.GottaAim;
        }
        if (SelectedTool.maxLoadedAmmo == 0) return ToolUseExcuses.AllClear; //Melee weapon doesn't have any ammo whatsoever
        if( SelectedToolData.loadedAmmo > 0)return ToolUseExcuses.AllClear; //We have ammo to use the tool
        else return ToolUseExcuses.OutOfAmmo; //We have no ammo to use the tool
    }

    internal void Reload(int ammountToReload)
    {
        LoadedAmmo += ammountToReload;
        if(!GameManager.APD.SelectedTool.infinteReloads)
        {
            StashedAmmo -= ammountToReload;
        }
        
    }

    internal void Fire()
    {
        LoadedAmmo--;
    }
    public ToolUseExcuses TryFire()
    {
        ToolUseExcuses toolUseExcuses = CanUseTool();
        if (toolUseExcuses == ToolUseExcuses.AllClear)
        {
            Fire();
        }
        return toolUseExcuses;
    }
    /// <summary>
    /// Load no more than fits in the tool and no more than we own and no more than we can reload at one time.
    /// </summary>
    /// <returns></returns>
    internal bool TryReload()
    {
        if (SelectedTool == null)
        {
            Debug.Log("No tool selected");
            return false;
        }
        else if (!SelectedTool.infinteReloads &&StashedAmmo <= 0)
        {
            Debug.Log("No ammo to reload");
            return false;
        }
        else
        {
            int ammountToReload = math.min(math.min(StashedAmmo, ReloadBatch), MaxLoadedAmmo - LoadedAmmo);
            if(ammountToReload > 0)
            {
                Reload(ammountToReload);
                return true;
            }
            else
            {
                Debug.Log("No need to reload");
                return false;
            }
        }
    }

    internal void SwitchTool()
    {
        selectedToolIndex++;
        if (selectedToolIndex >= toolInspectorField.Count)
        {
            selectedToolIndex = 0;
        }
    }
}
