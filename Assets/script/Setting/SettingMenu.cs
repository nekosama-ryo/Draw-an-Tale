using System.Collections;
using UnityEngine;
/// <summary>
/// 設定画面の操作
/// </summary>
public class SettingMenu : MonoBehaviour
{
    //シングルトン
    static public SettingMenu Setting = default;

    //設定画面の表示速度
    private WaitForSeconds _waitTime = new WaitForSeconds(1.2f);
    //設定画面の状態
    public bool _isSetting { get; set; } = false;
    //一度のみ処理
    private bool _isOne = false;


    //オブジェクト情報
    [SerializeField, Header("選択する項目オブジェクト")]
    private GameObject[] _selectObj = default;
    [SerializeField, Header("選択する項目オブジェクトの位置（パーティクルの往復時の左の位置）")]
    private Vector3[] _selectPos = default;
    [SerializeField, Header("BGMの設定位置の番号")]
    private int _bgmSettingNumber = default;
    [SerializeField, Header("SEの設定位置の番号")]
    private int _seSettingNumber = default;
    [SerializeField, Header("もどるの設定位置の番号")]
    private int _exitNumber = default;
    [SerializeField, Header("BGMの調整オブジェクト")]
    private GameObject _bgmObj = default;
    [SerializeField, Header("SEの調整オブジェクト")]
    private GameObject _seObj = default;


    //パーティクル
    //パーティクルの移動量
    private float _particleTime = 0f;
    [SerializeField, Header("通常選択時のパーティクル")]
    private GameObject _particleObj=default;
    [SerializeField, Header("パーティクルの移動速度")]
    private float _particleSpeed = default;
    [SerializeField, Header("パーティクルの移動量")]
    private float _particleMove = default;
    [SerializeField, Header("バー選択時のパーティクルの大きさ")]
    private Vector3 _particleSize = default;

    //パーティクルの初期サイズ
    private Vector3 _particleStartSize = default;
    //往復の右座標
    private Vector3 _maxPos = default;

    //パーティクルが端まで移動したか
    private bool _isTrip = false;


    //ボリューム
    //調整状態かどうか
    private bool _isSetVolume = false;

    [SerializeField, Header("音量設定の最大値")]
    private int _maxVolume = default;

    //ボリュームバーの位置
    private Vector3 _bgmPos=default;
    private Vector3 _sePos=default;

    //調整バーの長さ
    [SerializeField, Header("調整バーの左位置")]
    private float _rodLeft = default;
    [SerializeField, Header("調整バーのサイズ")]
    private float _rodLength = default;
    //プロパティ
    private float RodLength
    {
        get { return _rodLength / _maxVolume; }
        set { _rodLength = value; }

    }

    //現在の選択位置
    private int _selectPoint = 0;
    //プロパティ
    private int SelectPoint
    {
        get { return _selectPoint; }

        set
        {
            //入ってきた値が項目数を超えた場合、初期位置に戻る。
            if (value > _selectObj.Length - 1)
            {
                value = 0;
            }

            //０以下の場合一番下の選択位置になる
            if (value < 0)
            {
                value = _selectObj.Length - 1;
            }

            //値を代入
            _selectPoint = value;
        }
    }

    //BGMの大きさ
    private float _bgmVolume = default;
    //プロパティ
    public float BgmVolume
    {
        get { return _bgmVolume/_maxVolume; }

        private set
        {
            if (value > _maxVolume)
            {
                value = _maxVolume;
            }
            if (value < 0)
            {
                value = 0;
            }

            _bgmVolume = value;
        }
    }

    //SEの大きさ
    private float _seVolume = default;
    //プロパティ
    public float SeVolume
    {
        get { return _seVolume/_maxVolume; }

        private set
        {
            if (value > _maxVolume)
            {
                value = _maxVolume;
            }
            if (value < 0)
            {
                value = 0;
            }

            _seVolume = value;
        }
    }


    //キー情報

    //選択時のキー
    #region Key
    private KeyCode _up = KeyCode.W;
    private KeyCode _down = KeyCode.S;
    private KeyCode _left = KeyCode.A;
    private KeyCode _right = KeyCode.D;
    private KeyCode _aButton = KeyCode.Space;
    private KeyCode _upArrow = KeyCode.UpArrow;
    private KeyCode _downArrow = KeyCode.DownArrow;
    private KeyCode _leftArrow = KeyCode.LeftArrow;
    private KeyCode _rightArrow = KeyCode.RightArrow;
    #endregion

    [SerializeField, Header("キーを押し続けて、連続処理するまでの時間")]
    private float _keyTime = default;
    [SerializeField, Header("連続処理の行う間隔")]
    private float _keyInterval = default;


    //キーを押し続けた時間をカウント
    private float _selectTime = 0;
    //連続処理の行う間隔をカウント
    private float _keyIntervalTime = 0;

    private void Awake()
    {
        //シングルトンのため、初期時に一瞬だけ表示させて、シングルトンを有効にする。
        gameObject.SetActive(false);

        //シングルトンの処理
        if (Setting == null)
        {
            Setting = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }

        //パーティクルの初期サイズを保存
        _particleStartSize = _particleObj.transform.localScale;

        //ボリューム調整
        _bgmVolume = _maxVolume;
        _seVolume = _maxVolume;

        //volume調整の位置を調整
        _bgmPos = _bgmObj.transform.position;
        _sePos = _seObj.transform.position;

        //バーの位置を調整
        _bgmPos.x = RodLength * _bgmVolume+_rodLeft;
        _bgmObj.transform.position = _bgmPos;
        _sePos.x = RodLength * _seVolume + _rodLeft;
        _seObj.transform.position = _sePos;
    }

    /// <summary>
    /// 設定画面の動作
    /// </summary>
    public void SettingMove()
    {
        //通常時の操作
        if(!_isSetVolume)
        {
            //上キーで行う処理
            KeyAction(_up,_upArrow, () =>
            {
                //選択位置を上に移動
                SelectPoint--;
                SelectMove();
            });

            //下キーで行う処理
            KeyAction(_down,_downArrow, () =>
            {
                //選択位置を下に移動
                SelectPoint++;
                SelectMove();
            });

            //決定
            if (Input.GetKeyDown(_aButton))
            {
                //ボリューム処理
                if(_bgmSettingNumber==SelectPoint||_seSettingNumber==SelectPoint)
                {
                    _particleObj.transform.localScale = _particleSize;
                    _isSetVolume = true;
                }

                //やめる処理
                if (_exitNumber == SelectPoint)
                {
                    _isSetting = false;
                }
            }

            //パーティクルの挙動
            ParticleMove();
        }
        else
        {
            //音量変更時の操作
            VolumeSetting();

            //決定
            if (Input.GetKeyDown(_aButton))
            {
                _particleObj.transform.localScale = _particleStartSize;
                _isSetVolume = false;
            }
        }

    }

    /// <summary>
    /// 設定画面の表示処理
    /// </summary>
    public IEnumerator StartActive()
    {
        //秒数待機
        yield return _waitTime;

        //オブジェクトを表示
        gameObject.SetActive(true);

        //選択位置を０位置に。
        SelectPoint = 0;
        //初期位置に移動
        _particleObj.transform.position = _selectPos[SelectPoint];

        //パーティクルの終点を調整
        _maxPos = _selectPos[SelectPoint];
        _maxPos.x += _particleMove;

        //一度のみ処理する
        _isOne = true;

        //終了
        yield break;
    }

    /// <summary>
    /// キーが押された際に行う処理
    /// </summary>
    /// <param name="key">入力されるキー</param>
    /// <param name="subkey">入力されるキー</param>
    /// <param name="action">行う処理</param>
    private void KeyAction(KeyCode key, KeyCode subkey, System.Action action)
    {
        //キーが押されたかどうか
        if (Input.GetKeyDown(key)|| Input.GetKeyDown(subkey))
        {
            //キーが押された際に行う処理
            action();
            //カウントのリセット
            _selectTime = 0;
            _keyIntervalTime = 0;
        }
        else if (Input.GetKey(key) || Input.GetKey(subkey))
        {
            //キーが一定時間押され続けていたら連続処理を行う
            if (_selectTime > _keyTime)
            {
                //一定間隔で連続処理を行う
                if (_keyIntervalTime > _keyInterval)
                {
                    //連続で行う処理
                    action();
                    //カウントの初期化
                    _keyIntervalTime = 0;
                }
                else
                {
                    //カウント
                    _keyIntervalTime += Time.deltaTime;
                }
            }
            else
            {
                //カウント
                _selectTime += Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// Particleの挙動
    /// </summary>
    private void ParticleMove()
    {
        _particleTime += Time.deltaTime;

        //移動処理
        if (_isTrip)
        {
            _particleObj.transform.position = Vector3.Lerp(_selectPos[SelectPoint], _maxPos, _particleTime/_particleSpeed);
        }
        else
        {
            _particleObj.transform.position = Vector3.Lerp( _maxPos, _selectPos[SelectPoint], _particleTime/_particleSpeed);
        }

        //移動しきった
        if(_particleObj.transform.position.x <= _selectPos[SelectPoint].x||_particleObj.transform.position.x>=_maxPos.x)
        {
            _isTrip = !_isTrip;
            _particleTime = 0;
        }
    }

    /// <summary>
    /// Particleの選択時の挙動
    /// </summary>
    private void SelectMove()
    {
        //移動の終点を更新
        _maxPos = _selectPos[SelectPoint];
        _maxPos.x += _particleMove;

        //値の初期化
        _particleTime = 0f;
        _isTrip = false;


        //初期位置に移動
        _particleObj.transform.position = _selectPos[SelectPoint];
    }
    
    /// <summary>
    /// 設定画面を抜ける処理
    /// </summary>
    /// <param name="action">一度のみ行う</param>
    public void ExitActive(System.Action action)
    {
        //一度のみ処理する
        if(_isOne)
        {
            gameObject.SetActive(false);
            action();
            _isOne = false;
            AudioManager.Audio.PlaySe(0);
        }
    }

    /// <summary>
    /// ボリュームの設定時の操作
    /// </summary>
    private void VolumeSetting()
    {
        //BGMの調整
        if (SelectPoint == _bgmSettingNumber)
        {
            //右キーで行う処理
            KeyAction(_right, _rightArrow, () =>
             {
            //ボリュームを変更
            BgmVolume = _bgmVolume + 1;
            });
            //左キーで行う処理
            KeyAction(_left,_leftArrow, () =>
            {
                //ボリュームを変更
                BgmVolume=_bgmVolume-1;
            });

            //音量の調整
             AudioManager.Audio.BgmVolume();

            //バーの位置を調整
            _bgmPos.x = RodLength * _bgmVolume+_rodLeft;
            _bgmObj.transform.position = _bgmPos;

            _particleObj.transform.position = _bgmPos;
        }

        //SEの調整
        if (SelectPoint == _seSettingNumber)
        {
            //右キーで行う処理
            KeyAction(_right,_rightArrow, () =>
            {
                //ボリュームを変更
                SeVolume=_seVolume+1;
            });
            //左キーで行う処理
            KeyAction(_left,_leftArrow, () =>
            {
                //ボリュームを変更
                SeVolume=_seVolume-1;
            });

            //バーの位置を調整
            _sePos.x = RodLength * _seVolume +_rodLeft;
            _seObj.transform.position = _sePos;

            _particleObj.transform.position = _sePos;
        }
    }
}
