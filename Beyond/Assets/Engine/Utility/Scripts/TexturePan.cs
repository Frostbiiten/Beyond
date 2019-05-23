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
    void FixedUpdate()
    {
        for (int i = 0; i < panMaterials.Length; i++)
        {
            panMaterials[i].mat.SetTextureOffset("_BaseColorMap", panMaterials[i].mat.GetTextureOffset("_BaseColorMap") + panMaterials[i].speed);

            if (panMaterials[i].mat.GetTextureOffset("_BaseColorMap").x > 1)
            {
                panMaterials[i].mat.SetTextureOffset("_BaseColorMap", new Vector2(0f, panMaterials[i].mat.GetTextureOffset("_BaseColorMap").y));
            }

            if (panMaterials[i].mat.GetTextureOffset("_BaseColorMap").y > 1)
            {
                panMaterials[i].mat.SetTextureOffset("_BaseColorMap", new Vector2(panMaterials[i].mat.GetTextureOffset("_BaseColorMap").x, 0f));
            }
        }
    }
}
