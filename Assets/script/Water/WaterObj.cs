using System.Collections;
using UnityEngine;
/// <summary>
/// 水本体の処理
/// </summary>
public class WaterObj:MonoBehaviour
{
    //ーーーコンポーネントーーー
    private WaterController _waterScr;
    private Rigidbody2D _rb;
    private TrailRenderer _tr;

    //水が消えるまでの経過時間
    private float _time = 0;

    private void Awake()
    {
        //コンポーネント
        _rb = GetComponent<Rigidbody2D>();
        _tr = GetComponent<TrailRenderer>();
        //外部コンポーネント
        _waterScr = gameObject.transform.parent.GetComponent<WaterController>();
    }

    private void OnEnable()
    {
        //オブジェクトのパラメータ設定
        gameObject.transform.position = _waterScr._GeneratePos;
        gameObject.transform.localScale = _waterScr._GenerateSize;
        //水の数を設定
        _waterScr._WaterAllCnt += 1;
        //トレイルレンダラーをリセット
        _tr.Clear();
        //力をリセット
        _rb.velocity = Vector3.zero;
        _time = 0;
    }

    private void Update()
    {
        //水の動作
        StartCoroutine(WaterMove());
    }

    /// <summary>
    /// 水の動作を処理する
    /// </summary>
    /// <returns>数が多いので非同期処理</returns>
    private IEnumerator WaterMove()
    {
        //本を閉じた際に動作を停止する。
        if (Book.IsClose)
        {
            //動作停止
            _rb.constraints = RigidbodyConstraints2D.FreezePosition;
        }
        else
        {
            //回転率のリセットと固定
            transform.eulerAngles = Vector3.zero;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        //水に力が加わっていなければ一定秒数で消滅する。
        if (Mathf.Abs(_rb.velocity.y) <= 2 && Mathf.Abs(_rb.velocity.x) <= 2)
        {
            //時間が経ったら消える。
            if (_time > _waterScr._DisappearTime)
            {
                //徐々に大きさを小さく変え、最終的になくなる。
                if (transform.localScale.y > 0)
                {
                    transform.localScale = new Vector2(transform.localScale.x - Time.deltaTime*2, transform.localScale.y - Time.deltaTime*2);
                }
                else
                {
                    //無くなった際に非表示にする
                    gameObject.SetActive(false);
                    //プールに戻す。
                    _waterScr._WaterStack.Push(gameObject);
                }
            }
            else
            {
                //経過時間ごとに時間を足す
                _time += Time.deltaTime;
            }
        }

        //コルーチンを終わる
        yield break;
    }

    private void OnBecameInvisible()
    {
        //水の数を減らす
        _waterScr._WaterAllCnt -= 1;

        //非表示
        gameObject.SetActive(false);
        //プールに戻す。
        _waterScr._WaterStack.Push(gameObject);
    }
}
