using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// すべてのゲーム処理を管理する
/// </summary>
public class GameManager : MonoBehaviour
{
    //オブジェクト
    [SerializeField, Header("プレイヤーオブジェクト")]
    private GameObject _playerObj = default;
    private Rigidbody2D _playerRb = default;
    [SerializeField, Header("ポーズ時に表示させるボタン")]
    private GameObject _pauseObj = default;
    [SerializeField, Header("ドローモード時のスプライト")]
    private Sprite _drawModeSprite = default;
    [SerializeField, Header("歩行時のスプライト")]
    private Sprite _walkModeSprite = default;

    private Image _playerModeChangeImage = default;

    //数値
    [SerializeField, Header("風の数")]
    private Wind[] _windsScr = default;
    [SerializeField, Header("水車の数")]
    private WaterWheel[] _wheelScr = default;
    [SerializeField, Header("水の数")]
    private WaterController[] _waterConScr = default;
    //風量の数値
    private float _windCollision = default;

    //スクリプト
    [SerializeField, Header("本スクリプト")]
    private Book _bookScr = default;
    [SerializeField, Header("ドアスクリプト")]
    private ClearDoor _doorScr = default;
    [SerializeField, Header("インクスクリプト")]
    private InkController _inkScr = default;
    [SerializeField, Header("インクゲージスクリプト")]
    private Inkgauge _gaugeScr = default;
    [SerializeField, Header("スプライトマスクスクリプト")]
    private Spritemask[] _spritemaskScr = default;
    //プレイヤースクリプト
    private PlayerController _playerScr = default;
    [SerializeField, Header("ポーズスクリプト")]
    private Pause _pauseScr = default;

    private void Awake()
    {
        //スクリプトの取得
        //_pauseScr = new Pause();
        _playerScr = _playerObj.GetComponent<PlayerController>();
    }

    private void Start()
    {
        //風の初期化
        foreach (Wind wind in _windsScr)
        {
            wind.SetCollison();
        }

        _playerModeChangeImage = GameObject.FindGameObjectWithTag("ModeChange").GetComponent<Image>();
    }

    private void Update()
    {
        //ポーズ処理
        _pauseScr.PauseControl(_pauseObj);
        //ポーズ時はすべての処理を行わない。
        if (Time.timeScale == 0) return;

        //プレイヤーの基本動作
        _playerScr.PlayerMove();
        //本の基本動作
        _bookScr.BookController(_inkScr._isDraw);

        if (!_playerScr._IsDrawMode)
        {
            _playerModeChangeImage.sprite = _walkModeSprite;
        }
        else if (_playerScr._IsDrawMode)
        {
            _playerModeChangeImage.sprite = _drawModeSprite;
        }

        //インクゲージの変更
        _gaugeScr.GaugeUpdate();

        foreach (Spritemask mask in _spritemaskScr)
        {
            mask.Randomalpha();
        }


        //本が閉じている際の処理
        if (Book.IsClose)
        {
            //閉じている間水を生成しない
            foreach (WaterController con in _waterConScr) con.CloseBookWater();
            //水車の動きを停止させる
            foreach (WaterWheel wheel in _wheelScr) wheel.StopWheel();
        }
        //本が開いている際の処理
        else
        {
            //水を生成
            foreach (WaterController con in _waterConScr) con.PopWater();
            //水車の基本動作
            foreach (WaterWheel wheel in _wheelScr) wheel.Wheel();
            //チュートリアルシーン判別処理
            _doorScr.IsMatchingTutorialScene();
            //通常ステージのクリア判定　※現在のシーンがチュートリアルの場合は呼び出さない
            if (_doorScr._isTutorialScenes == false)
            {
                _doorScr.ClearJudge();
            }
            //チュートリアルのクリア判定
            else if (_doorScr._isTutorialScenes == true)
            {
                _doorScr.TutorialClearJudge();
            }

            //線を描く処理
            _inkScr.Draw();

            //マスク生成
            _playerScr.PlayerSpriteMatch();
        }

        //風の処理
        foreach (Wind wind in _windsScr)
        {
            //プレイヤーが風の影響をうけるかどうか
            if (wind.CheckWind(_playerObj.transform, _playerRb))
            {
                _windCollision = wind._Vec;
                break;
            }
            else
            {
                _windCollision = 0;
            }
        }

        //プレイヤーの最終移動
        if (_playerScr._IsStop)
        {
            _playerScr.Move(_windCollision);
        }
    }
}
