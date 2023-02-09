using System;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public BiomeTypes BiomeType;
    public BiomeHeights BiomeHeight;

    public float Elevation;
    public float Temperature;
    public float Rainfall;
    
    [SerializeField] private Material opaqueMaterial; // these don't belong on each instance, should be on BiomeSetup SO once
    [SerializeField] private Material transparentMaterial;

    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void ResetBiome()
    {
        meshRenderer.material = opaqueMaterial;
        meshRenderer.material.color = Color.gray;
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
    }
    
    // this should really be passed a BiomeSetup S.O. instead of these 3 floats
    public void AssignBiome(float elevation, float temperature, float rainfall)
    {
        BiomeType = rainfall switch
        {
            <= .3f when temperature > .6f => BiomeTypes.Desert,
            <= .3f when temperature <= .3f => BiomeTypes.Tundra,
            > .3f when elevation <= .3f => BiomeTypes.Water,
            > .6f when temperature <= .6f => BiomeTypes.Forest,
            > .6f when temperature > .6f => BiomeTypes.Jungle,
            _ => BiomeTypes.Grassland
        };
        
        BiomeHeight = elevation switch
        {
            > .85f => BiomeHeights.Mountain,
            > .6f => BiomeHeights.Hills,
            <= .3f => BiomeHeights.Sunken,
            _ => BiomeHeights.Flat
        };

        // assign initial colors
        meshRenderer.material.color = BiomeType switch
        {
            BiomeTypes.Grassland => new Color(.1f, 1f, .1f),
            BiomeTypes.Jungle => new Color(0f, .6f, .6f),
            BiomeTypes.Forest => new Color(0f, .7f, 0f),
            BiomeTypes.Desert => new Color(.8f, .7f, .1f),
            BiomeTypes.Tundra => new Color(.7f, .7f, .7f),
            BiomeTypes.Water => new Color(.1f, .1f, 1f),
            _ => throw new ArgumentOutOfRangeException()
        };

        // make water slightly transparent and have player sink slightly
        if (BiomeType == BiomeTypes.Water)
        {
            GetComponent<BoxCollider>().size = new Vector3(1f, .5f, 1f);
            if(transparentMaterial)
            {
                meshRenderer.material = transparentMaterial;
                meshRenderer.material.color = new Color(.1f, .1f, 1f, .99f);
            }
        }

        // adjust tile height and desaturate/darken the tile to help the player
        switch (BiomeHeight)
        {
            case BiomeHeights.Sunken:
                transform.localScale += Vector3.up * 1.5f;
                transform.localPosition += Vector3.down * .75f;
                meshRenderer.material.color = Color.Lerp(meshRenderer.material.color, Color.black, .5f); 
                break;
            case BiomeHeights.Hills:
                transform.localScale += Vector3.up * 1.5f;
                meshRenderer.material.color = Color.Lerp(meshRenderer.material.color, Color.white, .25f);
                break;
            case BiomeHeights.Mountain:
                transform.localScale += Vector3.up * 4.95f;
                meshRenderer.material.color = Color.Lerp(meshRenderer.material.color, Color.white, .5f);
                break;
        }

        Rainfall = rainfall;
        Temperature = temperature;
        Elevation = elevation;

    }
    
}
