using UnityEngine;
/// <summary>
/// セピア色に変えるクラス
/// </summary>
public class SepiaColor : MonoBehaviour
{
    //セピア色のmaterial
    public Material sepia = default;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, sepia);
    }
}
