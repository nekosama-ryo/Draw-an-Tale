using System.Collections;
using UnityEngine;

/// <summary>
/// 本の動作を管理する
/// </summary>
public class Book : MonoBehaviour
{
    /// <summary>
    /// 本の状態。true→閉じている状態
    /// </summary>
    static public bool IsClose { get; private set; } = false;

    //inspectorにて変更可能情報
    [SerializeField, Header("ページオブジェクト")]
    private GameObject _leftPageObj = default;
    [SerializeField]
    private GameObject _rightPageObj = default;

    [SerializeField, Header("開閉のスピード")]
    private float _openSpeed = 0.01f;
    [SerializeField, Header("閉じてから開くまでの時間")]
    private float _openTime = default;
    [SerializeField, Header("効果音のタイミング")]
    private float _seTime = default;

    [SerializeField, Header("インク生成位置の調整")]
    private float _inkPosZ = default;

    [SerializeField, Header("プレイヤースクリプト")]
    private PlayerController _playerScr = default;
    [SerializeField, Header("プレイヤーの足場判定オブジェクト")]
    private LegCollider _legScr = default;
    [SerializeField, Header("水描画用カメラ")]
    private Camera _camera = default;
    [SerializeField, Header("通常のカメラ")]
    private GameObject _cameraObj = default;

    //ページの回転位置の情報
    private Vector3 _leftStartRot = default;
    private Vector3 _rightStartRot = default;
    private Vector3 _leftEndRot = default;
    private Vector3 _rightEndRot = default;

    //カメラの位置情報
    private Vector3 _cameraStartPos;
    private Vector3 _cameraEndPos;

    private void Start()
    {
        //ページの初期位置を設定
        _leftStartRot = _leftPageObj.transform.eulerAngles;
        _rightStartRot = _rightPageObj.transform.eulerAngles;
        //ページの回転後の位置の設定
        _leftEndRot = new Vector3(_leftStartRot.x, _leftStartRot.y - 90, _leftStartRot.z);
        _rightEndRot = new Vector3(_rightStartRot.x, _rightStartRot.y + 90, _rightStartRot.z);

        //カメラの初期位置の設定
        _cameraStartPos = _cameraObj.transform.position;
        _cameraEndPos = _cameraStartPos;
        _cameraEndPos = new Vector3(_cameraEndPos.x, _cameraEndPos.y, _cameraEndPos.z - 10);

        //開閉判定のフラグの初期値を設定
        IsClose = false;
    }

    /// <summary>
    /// 本の操作を管理
    /// </summary>
    /// <param name="drawnow">Drawモード中かどうか</param>
    public void BookController(bool drawnow)
    {
        //Tabが押されたら、本を閉じる。
        if (_legScr._CanJump && Input.GetKeyDown(KeyCode.Tab) && !IsClose && !drawnow)
        {
            //カメラのMaskから水を除外する。
            _camera.cullingMask &= ~(1 << 4);
            //プレイヤーのステータスフラグ
            _playerScr._CanMove = true;
            //本の開閉フラグ
            IsClose = true;

            //本の開閉処理
            StartCoroutine(BookControl(_openTime));
        }
    }

    /// <summary>
    /// Lerp用の時間計算用メソッド
    /// </summary>
    /// <param name="time">時間管理変数（※refです）</param>
    /// <returns>時間ごとに徐々に大きな値を返す</returns>
    private float LerpTime(ref float time)
    {
        time += Time.deltaTime;
        float lerptime = time * time * 0.7f;
        return Mathf.Min(lerptime / 1, 1);
    }

    /// <summary>
    /// 本のメイン処理
    /// </summary>
    /// <param name="waitTime">開くまでの待機時間</param>
    /// <returns>本の開閉動作を返す</returns>
    private IEnumerator BookControl(float waitTime)
    {
        //Lerp用の一時変数
        float time = 0;
        float lerpTime = 0;

        //効果音を指定時間後に再生
        AudioManager.Audio.PlaySe(6, _seTime);

        //本が閉じるまでループ
        while (lerpTime < 1)
        {
            //本の開閉位置
            lerpTime = LerpTime(ref time);
            //本の閉じる動作
            _leftPageObj.transform.eulerAngles = Vector3.Lerp(_leftStartRot, _leftEndRot, lerpTime);
            _rightPageObj.transform.eulerAngles = Vector3.Lerp(_rightStartRot, _rightEndRot, lerpTime);

            //カメラの位置を変更
            _cameraObj.transform.position = Vector3.Lerp(_cameraStartPos, _cameraEndPos, lerpTime);

            //待機時間
            yield return new WaitForSeconds(_openSpeed * 0.01f);
        }

        //動作時間をリセット
        time = 0;
        lerpTime = 0;

        //インクのコピー
        Copy(_leftPageObj, _rightPageObj, 1);
        Copy(_rightPageObj, _leftPageObj, -1);

        //本を開くまでの待機時間
        yield return new WaitForSeconds(waitTime);

        //本を開くまでループ
        while (lerpTime < 1)
        {
            //本の開閉位置
            lerpTime = LerpTime(ref time);
            //本の開く動作
            _leftPageObj.transform.eulerAngles = Vector3.Lerp(_leftEndRot, _leftStartRot, lerpTime);
            _rightPageObj.transform.eulerAngles = Vector3.Lerp(_rightEndRot, _rightStartRot, lerpTime);

            //カメラ位置を元に戻す
            _cameraObj.transform.position = Vector3.Lerp(_cameraEndPos, _cameraStartPos, lerpTime);

            //待機時間
            yield return new WaitForSeconds(_openSpeed * 0.01f);
        }

        //本を開くと、プレイヤーが動作可能になる
        _playerScr._CanMove = false;

        //水を表示する。
        _camera.cullingMask |= (1 << 4);

        //本を開くまでの連続で開くのを防止する為若干の待機時間
        yield return new WaitForSeconds(0.5f);

        //本が開いた。
        IsClose = false;

        //コルーチンを終了
        yield break;
    }

    /// <summary>
    /// 複製処理
    /// </summary>
    /// <param name="page">コピー元のページ</param>
    /// <param name="pearPage">コピー先のページ</param>
    /// <param name="pageNum">左右でー１か１を設定する。</param>
    void Copy(GameObject page, GameObject pearPage, int pageNum)
    {
        //インクコピー用
        Vector3 copyPos;

        //子オブジェクトからインクを探す。
        foreach (Transform inkObj in pearPage.transform)
        {
            if (inkObj.tag == "ink")
            {
                //コピー位置を設定
                copyPos = inkObj.transform.position;
                //位置を調整
                copyPos.x += _inkPosZ * pageNum;
                //コピー生成
                GameObject Obj = Instantiate(inkObj.gameObject, copyPos, inkObj.transform.rotation, page.transform);
                Obj.transform.localScale = inkObj.transform.localScale;
                //これ以上複製が起こらないように、タグを変更する。
                Obj.gameObject.tag = "_ink";
                inkObj.gameObject.tag = "_ink";
            }
        }
    }
}
