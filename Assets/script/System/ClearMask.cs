using System.Collections;
using UnityEngine;

/// <summary>
/// ギミックのクリアに対し色を付ける
/// </summary>
public class ClearMask : MonoBehaviour
{
    [SerializeField, Header("マスク")]
    private GameObject _maskObj = default;
    [SerializeField, Header("マスクを作動させたいギミックオブジェクト")]
    private GameObject[] _gimmickObjs = default;
    [SerializeField, Header("マスクのサイズ")]
    private Vector3[] _maskSize = default;
    [SerializeField, Header("最大サイズになる時間")]
    private float _sizeTime = default;

    //一回でも生成されたら以降生成を行わないように。
    private bool _isCreate = true;
    //ループ時に0.1秒待機する
    private WaitForSeconds wait = new WaitForSeconds(0.1f);

    /// <summary>
    /// ギミックをクリアした際に呼び出す。
    /// </summary>
    /// <param name="number">作動させたいマスクの指標番号</param>
    public void GimmickClear(int number)
    {
        if(_isCreate)
        {
            //マスクをギミックオブジェクトの位置と子として生成
            GameObject mask = Instantiate(_maskObj, _gimmickObjs[number].transform.position
                , _gimmickObjs[number].transform.rotation, transform);
            //表示
            mask.SetActive(true);
            //マスクの大きさを調整
            StartCoroutine(BigSize(_maskSize[number], mask));
            //生成を停止する
            _isCreate = false;
        }
    }

    /// <summary>
    /// サイズを徐々に大きくする
    /// </summary>
    /// <param name="size">最大サイズ</param>
    /// <param name="mask">マスクオブジェクト</param>
    private IEnumerator BigSize(Vector3 size,GameObject mask)
    {
        //初期化
        Vector3 maskSize = Vector3.zero;
        mask.transform.localScale = maskSize;
        //効果音
        AudioManager.Audio.PlaySe(11);

        while (size.x >= mask.transform.localScale.x)
        {
            //徐々に大きさを大きくする
            mask.transform.localScale = maskSize;
            maskSize.x += size.x/_sizeTime/10;
            maskSize.y += size.y/_sizeTime/10;
            maskSize.z += size.z/_sizeTime/10;

            //秒数待機
            yield return wait;
        }
        //終了
        yield break;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player" && _isCreate)
        {
            //マスクをギミックオブジェクトの位置と子として生成
            GameObject mask = Instantiate(_maskObj, transform.position, transform.rotation, transform);
            //表示
            mask.SetActive(true);
            //マスクの大きさを調整
            StartCoroutine(BigSize(_maskSize[0], mask));
            //生成を停止する
            _isCreate = false;
        }
    }
}
