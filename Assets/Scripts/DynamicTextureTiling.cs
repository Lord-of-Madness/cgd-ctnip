using UnityEngine;

[ExecuteInEditMode]
public class DynamicTextureTiling : MonoBehaviour
{
    // Reference to the original material with the texture
     Material originalMaterial;
    [SerializeField] float density = 1;

    void Start()
    {
        // Ensure we have a material
        originalMaterial = GetComponent<Renderer>().sharedMaterial;

        // Create a new material instance for this object
        Material materialInstance = new(originalMaterial);

        // Apply the new material to the object
        GetComponent<Renderer>().material = materialInstance;

        // Get the initial scale of the object
        Vector3 initialScale = transform.localScale;

        // Set the texture tiling based on the initial scale
        SetTextureTiling(materialInstance, initialScale);
    }

    void Update()
    {
        // Adjust texture tiling based on the current scale
        SetTextureTiling(GetComponent<Renderer>().sharedMaterial, transform.localScale);
    }

    void SetTextureTiling(Material material, Vector3 scale)
    {

        // Calculate tiling based on the scale
        Vector2 tiling = new(scale.x, scale.y);
        if (scale.y==1) tiling = new(scale.x, scale.z);


        // Apply tiling to the material
        material.mainTextureScale = tiling*density;
    }
}
