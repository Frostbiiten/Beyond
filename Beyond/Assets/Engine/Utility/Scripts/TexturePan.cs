using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexturePan : MonoBehaviour
{
    [System.Serializable]
    public struct PanMaterial
    {
        public Material mat;
        public Vector2 speed;
    }
    public PanMaterial[] panMaterials;

    public PanMaterial[] panEmissionMaterials;

    void FixedUpdate()
    {
        for (int i = 0; i < panMaterials.Length; i++)
        {
            panMaterials[i].mat.SetTextureOffset("_BaseColorMap", panMaterials[i].mat.GetTextureOffset("_BaseColorMap") + panMaterials[i].speed);

            if (panMaterials[i].mat.GetTextureOffset("_BaseColorMap").x > 1)
            {
                panMaterials[i].mat.SetTextureOffset("_BaseColorMap", new Vector2(panMaterials[i].mat.GetTextureOffset("_BaseColorMap").x - 1, panMaterials[i].mat.GetTextureOffset("_BaseColorMap").y));
            }

            if (panMaterials[i].mat.GetTextureOffset("_BaseColorMap").y > 1)
            {
                panMaterials[i].mat.SetTextureOffset("_BaseColorMap", new Vector2(panMaterials[i].mat.GetTextureOffset("_BaseColorMap").x, panMaterials[i].mat.GetTextureOffset("_BaseColorMap").y - 1));
            }
        }

        for (int i = 0; i < panMaterials.Length; i++)
        {
            panMaterials[i].mat.SetTextureOffset("_EmissiveColorMap", panMaterials[i].mat.GetTextureOffset("_EmissiveColorMap") + panMaterials[i].speed);

            if (panMaterials[i].mat.GetTextureOffset("_EmissiveColorMap").x > 1)
            {
                panMaterials[i].mat.SetTextureOffset("_EmissiveColorMap", new Vector2(panMaterials[i].mat.GetTextureOffset("_EmissiveColorMap").x - 1, panMaterials[i].mat.GetTextureOffset("_EmissiveColorMap").y));
            }

            if (panMaterials[i].mat.GetTextureOffset("_EmissiveColorMap").y > 1)
            {
                panMaterials[i].mat.SetTextureOffset("_EmissiveColorMap", new Vector2(panMaterials[i].mat.GetTextureOffset("_EmissiveColorMap").x, panMaterials[i].mat.GetTextureOffset("_EmissiveColorMap").y - 1));
            }
        }

    }
}
