using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// インクゲージの管理クラス
/// </summary>
public class Inkgauge : MonoBehaviour
{
    //ラインレンダラー
    private InkController _renderer = default;
    //インクゲージ
    private Image _gauge = default;
 
    void Start()
    {
        //コンポーネントの取得
        _renderer = GameObject.Find("draw").GetComponent<InkController>();
        _gauge = GetComponent<Image>();
    }

    /// <summary>
    /// インクゲージの状態を更新する
    /// </summary>
    public void GaugeUpdate()
    {
        //インクを消費しただけ長さを変更
        _gauge.fillAmount = _renderer._inkamount / _renderer._inkmax;
    }
}
