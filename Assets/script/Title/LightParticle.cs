using UnityEngine;
/// <summary>
/// ライトのパーティクルを処理する
/// </summary>
public class LightParticle : MonoBehaviour
{
    [SerializeField, Header("パーティクルの大きさ")]
    private Vector2 minmax=default;
    //ライト
    UnityEngine.Light _me=default;
    //ランダムな値を持たせる
    private float _range=default;
    //時間を管理
    private float _time=default;
    void Start()
    {
        //ライト取得
        _me = GetComponent<UnityEngine.Light>();
        _range = _me.intensity;
    }

    void FixedUpdate()
    {
        //時間経過ごとにランダム範囲で大きさを調整
        if (_time >= 2)
        {
            _range = Random.Range(minmax.x, minmax.y);
            _time = 0;
        }

        //徐々に大きさを変える
        _me.intensity = Mathf.Lerp(_me.intensity, _range, 0.02f);
        //経過時間
        _time += Time.deltaTime;
    }
}
