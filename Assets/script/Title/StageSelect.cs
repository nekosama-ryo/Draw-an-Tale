using UnityEngine;
/// <summary>
/// ステージが選択状況を持つ
/// </summary>
public class StageSelect : MonoBehaviour
{
    //選択されたステージ
    public int _SelectStage = default;
    //ステージの選択できるページ数
    public int _SelectPage = 1;

    private void Start()
    {
        //選択されたステージを引き継ぐ
        DontDestroyOnLoad(gameObject);
    }
}
