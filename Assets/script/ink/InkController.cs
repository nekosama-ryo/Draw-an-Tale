using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// インク処理を制御する
/// </summary>
public class InkController : MonoBehaviour
{
    [SerializeField, Header("描画するパーティクル")]
    private GameObject _lineparticleObj = default;

    [SerializeField, Header("ラインレンダラーのコライダー")]
    private GameObject _lineColliderObj = default;

    [SerializeField, Header("インクボックス")]
    private GameObject _inkBoxObj = default;

    [SerializeField, Header("ラインの太さ")]
    private float _lineWidth = 0.5f;

    [SerializeField, Header("Z軸の調整")]
    private float _zPos = 0;
    [SerializeField]
    private Camera _raycam=default;

    //プレイヤーのオブジェクト
    private GameObject _playerObj = default;
    //プレイヤースクリプト
    private PlayerController _playerScr = default;

    //初期位置
    private Vector3 _startPos = default;
    //現在位置
    private Vector3 _pos = default;
    private Vector3 _poslen = default;

    //自身のラインレンダラー
    private LineRenderer _renderer = default;

    //ラインレンダラーの位置を保存
    private List<Vector3> _rendererList = new List<Vector3>();

    //Draw可能かどうか
    private bool _canDraw = false;
    //Draw中かどうか
    public bool _isDraw { get; private set; } = false;


    //現在インク量
    public float _inkamount { get; private set; } = 50;
    //インクの最大量
    public float _inkmax { get; private set; } = 50;

    //自身のいるページかを判定
    private RaycastHit2D _hit = default;

    private void Awake()
    {
        //コンポーネントの取得
        _renderer = gameObject.GetComponent<LineRenderer>();
    }

    private void Start()
    {
        // 線の幅
        _renderer.startWidth = 0.5f;
        _renderer.endWidth = 0.2f;

        //プレイヤーを取得
        _playerObj = GameObject.FindGameObjectWithTag("Player");

        //プレイヤースクリプトを取得
        _playerScr = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    /// <summary>
    /// インクの処理
    /// </summary>
    public void Draw()
    {
        //メイン処理
        Click();
        //プレイヤーにDraw可能状態を渡す
        _playerScr.DrawModeChange(_isDraw);
        //フィルター
        //_playerScr.Filter(_filter);
    }

    /// <summary>
    /// マウスクリック時のインクの処理
    /// </summary>
    void Click()
    {
        //現在がDrawモード中かどうか判定
        if (_playerScr._IsDrawMode)
        {
            //マウスの位置を取得・調整
            Vector3 _mouseposi = Input.mousePosition;
            _mouseposi *= 1f;
            _mouseposi.z = 20;

            //インクが残っている状態で左クリック
            if (Input.GetMouseButtonDown(0) && _inkamount > 0)
            {
                //現在のページ位置を調べる。
                CheckPage(_mouseposi);

                //タッチ位置取得
                Vector3 cameraPosition = Input.mousePosition;
                cameraPosition.z = _zPos;

                //効果音
                AudioManager.Audio.PlayLoopSe(4);

                //線を描ける状態かどうか。
                if (_canDraw)
                {
                    //数値を調整
                    _renderer.positionCount = 1;
                    _startPos = Camera.main.ScreenToWorldPoint(cameraPosition);
                    _poslen = _startPos;
                    _pos = Camera.main.ScreenToWorldPoint(cameraPosition);

                    //ラインの起点設定
                    _renderer.SetPosition(0, _startPos);
                    _isDraw = true;
                    _lineparticleObj.SetActive(true);
                }

            }
            else if (Input.GetMouseButton(0) && _canDraw && _inkamount > 0 && _isDraw)
            {
                //線を描いてる際のパーティクルの位置を調整
                _lineparticleObj.transform.position = _renderer.GetPosition(_renderer.positionCount - 1);

                //タッチ位置取得
                CheckPage(_mouseposi);

                //線を描ける状態かどうか。
                if (_canDraw)
                {
                    Vector3 cameraPosition = Input.mousePosition;
                    cameraPosition.z = _zPos;
                    _pos = Camera.main.ScreenToWorldPoint(cameraPosition);
                }
            }
            if (Input.GetMouseButtonUp(0) || !_canDraw && Input.GetMouseButton(0))
            {
                //ラインレンダラーで描いた点が１以上のときのみ描画する。
                if (_renderer.positionCount > 1)
                {
                    //インクをまとめる空オブジェクトを生成。
                    GameObject inkbox_prefab = Instantiate(_inkBoxObj, transform.position, transform.rotation) as GameObject;

                    //具現化効果音
                    AudioManager.Audio.PlaySe(5);

                    //空オブジェクトをページの子に設定
                    inkbox_prefab.transform.parent = _playerObj.transform.parent;

                    //インクを生成する
                    for (int _renderercont = 0; _renderercont < _renderer.positionCount - 1; ++_renderercont)
                    {
                        //インクオブジェクトを生成
                        GameObject ink = Instantiate(_lineColliderObj, transform.position, transform.rotation) as GameObject;

                        //空オブジェクトの子供にする
                        ink.transform.parent = inkbox_prefab.transform;

                        //ラインレンダラーと同じ形で生成する
                        ink.transform.position = (_renderer.GetPosition(_renderercont) + _renderer.GetPosition(_renderercont + 1)) / 2;
                        ink.transform.right = (_renderer.GetPosition(_renderercont + 1) - _renderer.GetPosition(_renderercont)).normalized;
                        ink.transform.localScale = new Vector3((_renderer.GetPosition(_renderercont + 1) - _renderer.GetPosition(_renderercont)).magnitude - 0.17f, _lineWidth, _lineWidth);

                    }
                }

                //変数の初期化
                _renderer.positionCount = 0;
                _pos = new Vector3(0, 0, 0);
                _rendererList.Clear();
                _isDraw = false;
                _lineparticleObj.SetActive(false);
                AudioManager.Audio.StopLoopSe();
            }
            // ラインレンダラーに座標を設定し線を描画
            if (!_rendererList.Contains(_pos))
            {
                if (Mathf.Abs(Mathf.Max(_poslen.x, _pos.x) - Mathf.Min(_poslen.x, _pos.x)) > 0.5f || Mathf.Abs(Mathf.Max(_poslen.y, _pos.y) - Mathf.Min(_poslen.y, _pos.y)) > 0.5f)
                {
                    //ラインレンダラーの現在位置を点として保存
                    _rendererList.Add(_pos);
                    _poslen = _pos;

                    //点の数を増やす
                    _renderer.positionCount += 1;
                    _renderer.SetPosition(_renderer.positionCount - 1, _pos);

                    //インク量を減らす
                    _inkamount -= 1;
                }

            }
        }
    }

    /// <summary>
    /// レイキャストからページを調べる。
    /// </summary>
    /// <param name="mousePos">マウスの位置</param>
    void CheckPage(Vector3 mousePos)//Raycast処理
    {
        //マウスの位置をカメラ基準に。
        mousePos = _raycam.ScreenToWorldPoint(mousePos);
        //レイキャストが当たったオブジェクトを拾う
        _hit = Physics2D.Raycast(mousePos, new Vector3(0, 0, 1), 30);
        //レイキャストを表示する。
        Debug.DrawRay(mousePos, new Vector3(0, 0, 30));

        //レイキャストの当たったオブジェクトをしらべる。
        if (_hit.collider != null)
        {
            //プレイヤーの親ページと、レイキャストの当たったページが同じかどうか
            if (_playerObj.transform.parent.name == _hit.transform.name)
            {
                _canDraw = true;
            }
            else
            {
                _canDraw = false;
            }
        }
    }
}

