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
public class PlayerData : MonoBehaviour
{
    [SerializeField] List<Tool> toolInspectorField;
    public Dictionary<Tool, ToolInvData> toolInventory = new();
    public Tool SelectedTool;
    public ToolInvData SelectedToolData => toolInventory[SelectedTool];

    int LoadedAmmo { get => SelectedToolData.loadedAmmo; set => SelectedToolData.loadedAmmo = value; }
    int StashedAmmo { get => SelectedToolData.stashedAmmo; set => SelectedToolData.stashedAmmo = value; }
    public int MaxLoadedAmmo => SelectedTool.maxLoadedAmmo;
    public int ReloadBatch => SelectedTool.reloadBatch;

    private void Start()
    {
        if(toolInspectorField != null && toolInspectorField.Count != 0)
        {
            SelectedTool = toolInspectorField[0];
        }
        foreach (var tool in toolInspectorField)
        {
            toolInventory.Add(tool, new());
        }
    }

    public bool CanUseTool()
    {
        if(SelectedTool == null)
        {
            Debug.Log("No tool selected");
            return false;
        }
        return SelectedToolData.loadedAmmo > 0;
    }

    internal void Reload(int ammountToReload)
    {
        LoadedAmmo += ammountToReload;
        StashedAmmo -= ammountToReload;
    }

    internal void Fire()
    {
        LoadedAmmo--;
    }
    public bool TryFire()
    {
        if (CanUseTool())
        {
            Fire();
            return true;
        }
        else
        {
            Debug.Log("No ammo");
            return false;
        }
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
        else if (StashedAmmo <= 0)
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
}
