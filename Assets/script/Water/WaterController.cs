using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 水を管理
/// </summary>
public class WaterController : MonoBehaviour
{
    //inspectorで管理
    [SerializeField, Header("水オブジェクト")] 
    private GameObject _waterObj = default;
    [SerializeField, Header("水の発生間隔")] 
    private float _generateTime=default;
    [SerializeField, Header("水の最大沸き数")] 
    private int _generateMax =default;
    [SerializeField, Header("発生位置の調整Z軸")] 
    private float _generatePosZ = 10;

    //水オブジェクトに渡す値
    [Header("発生サイズ")] 
    public Vector3 _GenerateSize =default;
    [Header("水消滅までの時間")] 
    public float _DisappearTime = default;
    //水の生成位置
    public Vector3 _GeneratePos { get; private set; } = default;
    //水の合計沸き数
    public int _WaterAllCnt{ get; set; }=default;

    //沸き管理用
    //沸き時間
    private float _time = 999;      
    //沸き可能かどうか
    private bool _isGenerate = true;

    //プール用のスタック
    public Stack<GameObject> _WaterStack = new Stack<GameObject>(175);

    void Start()
    {
        //発生位置を調整
        _GeneratePos = new Vector3(transform.position.x, transform.position.y, _generatePosZ);
        //水の効果音を再生
        AudioManager.Audio.PlayGimmickSe(8);
    }

    /// <summary>
    /// 本が閉じている際の水の処理
    /// </summary>
    public void CloseBookWater()
    {
        //生成を一時中断
        _isGenerate = false;
        //指定秒数後に再生成
        StartCoroutine(WaitTime());
    }

    /// <summary>
    /// 水を間隔毎に生成する。
    /// </summary>
    public void PopWater()
    {
        //沸き数が指定数を超えたら沸かなくなる
        if (_WaterAllCnt < _generateMax &&_isGenerate)
        {
            //指定間隔毎に沸く。
            if (_time>_generateTime)
            {
                _time = 0;
                //水を生成
                GameObject obj = Pool();
                //水を表示
                obj.SetActive(true);
            }
            else
            {
                _time += Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// プール処理
    /// </summary>
    /// <returns>プールから取り出したオブジェクト</returns>
    private GameObject Pool()
    {
        //個数を確認
        if(_WaterStack.Count>0)
        {
            return _WaterStack.Pop();
        }

        //新規生成
        return Instantiate(_waterObj,transform);
    }

    /// <summary>
    /// 秒数待機
    /// </summary>
    /// <returns>指定秒数後に生成可能にする</returns>
    private IEnumerator WaitTime()
    {
        //本が開くまで処理停止
        yield return new WaitUntil(() => !Book.IsClose);
        //指定秒数待機
        yield return new WaitForSeconds(0.05f);

        //生成可能にする
        _isGenerate = true;

        //コルーチンの終了
        yield break;
    }
}
