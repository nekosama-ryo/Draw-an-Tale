using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// タイトル処理を管理
/// </summary>
public class TitleManager : MonoBehaviour
{
    #region Variable
    //カメラオブジェクト
    private GameObject _cameraObj = default;
    //現在のタイトル画面の位置
    [SerializeField]
    private bool[] _isScreen = new bool[3];
    private bool _first = false;

    //アニメーター
    private Animator _anim = default;
    [SerializeField, Header("本のアニメーター")]
    private Animator _bookAnim = default;
    [SerializeField, Header("ロゴのアニメーター")]
    private Animator _logoAnim = default;

    [SerializeField, Header("本のロゴ")]
    private GameObject _titleLogoObj = default, _selectLightObj = default, _selectModeObj = default, _fadeManagerObj = default;
    [SerializeField, Header("明かりのスクリプト")]
    private UnityEngine.Light _pageLightScr = default;
    [SerializeField, Header("ページごとに表示されるオブジェクト")]
    private GameObject[] Active = default, Stagecount = default;
    //テキスト
    private Text[] _text = new Text[3];

    //入力管理
    private float _horizon = default;
    private float _vertical = default;
    //現在のキー値
    private int[] _select = new int[2];
    //現在の選択位置
    private int _menuSelect = default;
    //現在のページ数
    private int _redopage = default;

    //時間
    private float _time = default;

    //イベント中かどうか
    private bool _isInputAxis = default, _isEventNow = default;
    [SerializeField, Header("モード選択のアイコン")]
    private SpriteRenderer[] _icons = new SpriteRenderer[4];

    [SerializeField, Header("選択時のパーティクル")]
    private GameObject _selectParticle = default;
    //ステージ選択のスクリプト
    private StageSelect _selectScr = default;
    //フェードアウトの管理スクリプト
    private FadeOut _fadeScr = default;


    //-------------追加部分------------------
    [SerializeField, Header("設定画面")]
    private SettingMenu _settingScr;
    #endregion

    private void Awake()
    {
        //スタート時にフェードアウトオブジェクトを生成
        if (GameObject.FindGameObjectWithTag("Fade") == null)
        {
            Instantiate(_fadeManagerObj, transform.position, Quaternion.identity);
        }

        //コンポーネントの取得
        _cameraObj = GameObject.Find("Main Camera");
        _anim = _cameraObj.GetComponent<Animator>();
        _selectScr = GameObject.Find("SelectStageMove").GetComponent<StageSelect>();
        _redopage = _selectScr._SelectPage;
        for (int i = 0; i < Stagecount.Length; i++)
        {
            _text[i] = Stagecount[i].GetComponent<Text>();
        }
        _fadeScr = GameObject.FindGameObjectWithTag("Fade").GetComponent<FadeOut>();
    }
    private void Start()
    {
        _settingScr = SettingMenu.Setting;
        _fadeScr = GameObject.Find("Fademanager").GetComponent<FadeOut>();

        //TitleBGMを鳴らす
        AudioManager.Audio.StopGimmickSe();
        AudioManager.Audio.PlayBgm(1);

        //チュートリアルフラグをオフ
        _fadeScr.IsTutorial = false;
    }

    void Update()
    {
        //イベント中以外の場合
        if (!_isEventNow)
        {
            //設定画面
            if (_settingScr._isSetting)
            {
                //設定画面の動作
                _settingScr.SettingMove();
            }
            else
            {
                //設定画面を閉じる
                _settingScr.ExitActive(() =>
                {
                    //設定を戻す処理
                    _isScreen[2] = false;
                    _isScreen[1] = false;
                    //アニメーション
                    _bookAnim.SetFloat("speed", -1);
                    _bookAnim.Play("open", 0, 0.9f);

                    //メニューの表示

                });
            }

            //キーが押された
            if(Input.anyKeyDown)
            {
                //決定かどうか
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    //決定音
                    AudioManager.Audio.PlaySe(1);
                }
                else
                {
                    //選択音
                    AudioManager.Audio.PlaySe(2);
                }
            }

            //キー入力・動作
            _input();
            //ページめくる
            Check();
        }
    }

    /// <summary>
    /// キー入力に応じて、動作を返します。
    /// </summary>
    void _input()
    {
        //入力管理
        _horizon = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");

        //どこかキーが押されたら次の画面に以降
        if (Input.anyKeyDown && !_isScreen[0])
        {
            _isScreen[0] = true;

        }

        //アニメーション再生中に行う
        if (_anim.GetFloat("speed") == 1.3f && !_isScreen[1] && !_first || _anim.GetFloat("speed") == 1.3f && _bookAnim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0 && !_isScreen[1])
        {
            //現在のシーンの物を非表示にする
            foreach (GameObject game in Active)
            {
                game.SetActive(false);
            }
            _first = true;
            _titleLogoObj.SetActive(true);
            //次のアニメーション
            _logoAnim.Play("_TitleIcon", 0, 0f);
            _logoAnim.SetFloat("speed", 1);

            //次の画面段階に進む。
            _isScreen[1] = true;
            //ロゴを表示
            _selectlogo();
        }

        //モード選択画面の処理
        if (_logoAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && _isScreen[1] && !_isScreen[2])
        {
            //入力状況に応じて、処理を行う
            if ((_horizon != 0 || _vertical != 0))
            {
                //入力情報によって目立つマークを切り替える
                switch (_horizon)
                {
                    case 1:
                        _select[0] = 1;
                        _select[1] = 1;
                        break;

                    case -1:
                        _select[0] = 0;
                        _select[1] = 0;
                        break;
                }

                //入力情報によって目立つマークを切り替える
                switch (_vertical)
                {
                    case 1:
                        _select[0] = 1;
                        _select[1] = 0;
                        break;

                    case -1:
                        _select[0] = 1;
                        _select[1] = 2;
                        break;
                }
                //マークの状況をリセット？
                if (_select[0] == 0)
                {
                    _select[1] = 0;
                }

                //選択ロゴを更新
                _selectlogo();
            }

            //キー入力がなかった場合
            if (_isInputAxis && _horizon == 0 && _vertical == 0)
            {
                _isInputAxis = false;
            }

            //メニュー画面に移行
            if (Input.GetButtonDown("Submit"))
            {
                _titleLogoObj.SetActive(false);
                MenuSelect();
            }
        }
        else if ((_bookAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 || _bookAnim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0) && _isScreen[2] && _menuSelect == 0)
        {
            //選択画面のオブジェクトを表示
            _selectModeObj.SetActive(true);

            //ステージごとに文字を描画
            for (int i = 0; i < _text.Length; i++)
            {
                _text[i].text = "stage" + (((_selectScr._SelectPage - 1) * 3) + i + 1);
            }

            //選択位置にパーティクルを移動
            _selectParticle.transform.position = new Vector3(_selectParticle.transform.position.x, 0.55f, Stagecount[_selectScr._SelectStage - ((_selectScr._SelectPage - 1) * 3) - 1].transform.position.z - 0.05f);

            //キーが入力された
            if (!_isInputAxis && (_horizon != 0 || _vertical != 0))
            {
                //ページをめくる
                switch (_horizon)
                {
                    //次のページへ
                    case 1:
                        _selectScr._SelectPage++;
                        _selectScr._SelectStage += 3;
                        break;

                    //前のページへ
                    case -1:
                        _selectScr._SelectPage--;
                        _selectScr._SelectStage -= 3;
                        break;
                }

                //ページ位置の選択
                switch (_vertical)
                {
                    //前のステージを選択
                    case 1:
                        _selectScr._SelectStage--;
                        break;

                    //次のステージを選択
                    case -1:
                        _selectScr._SelectStage++;
                        break;
                }


                //キー入力を感知
                _isInputAxis = true;

                //選択位置の限界を定義
                _selectScr._SelectPage = Mathf.Clamp(_selectScr._SelectPage, 1, 2);
                _selectScr._SelectStage = Mathf.Clamp(_selectScr._SelectStage, (_selectScr._SelectPage - 1) * 3 + 1, _selectScr._SelectPage * 3);

                //ステージの選択
                SelectStage();
            }

            //キー入力がない場合
            if (_isInputAxis && _horizon == 0 && _vertical == 0)
            {
                _isInputAxis = false;
            }

            //ゲームの開始処理
            if (Input.GetButtonDown("Submit"))
            {
                GameStart();
            }
        }

    }

    /// <summary>
    /// ページをめくるアニメーション
    /// </summary>
    void Check()
    {
        //現在のページを調べる
        if (_isScreen[0] && !_isScreen[1])
        {
            //時間経過
            _time = Mathf.Clamp(_time += Time.deltaTime, 0, 1.3f);
            _pageLightScr.intensity = Mathf.Clamp(_pageLightScr.intensity + Time.deltaTime, 0, 1);

            //経過ごとにアニメーションを再生
            _anim.SetFloat("speed", _time);
        }
    }

    /// <summary>
    /// ロゴの選択状況を管理
    /// </summary>
    void _selectlogo()
    {
        //現在の位置を調べる
        for (int _countx = 0; _countx < 2; _countx++)
        {
            for (int _county = 0; _county < 3; _county++)
            {
                //選択以外のロゴの色を暗くする。
                if (_countx == _select[0] && _county == _select[1])
                {
                    _icons[_countx + _county].color = new Color(1, 1, 1);
                }
                else
                {
                    _icons[_countx + _county].color = new Color(0.6f, 0.6f, 0.6f);
                }
            }
        }
    }

    /// <summary>
    /// メニューの選択状況を管理
    /// </summary>
    void MenuSelect()
    {
        //現在の選択位置を調べる。
        _menuSelect = _select[0] + _select[1];

        //現在の選択状況
        switch (_menuSelect)
        {
            //ステージ選択
            case 0:
                _bookAnim.SetFloat("speed", -1);
                _bookAnim.Play("continuous_page", 0, 0.9f);
                _isScreen[2] = true;
                AudioManager.Audio.PlaySe(7);
                break;

            //チュートリアル
            case 1:
                _bookAnim.Play("continuous_page", 0, 0.1f);
                _isScreen[2] = true;
                _fadeScr.IsTutorial = true;
                //フェードアウト
                _fadeScr.Fadeout(1, 1, 1, 0, 2);
                AudioManager.Audio.PlaySe(7);
                break;

            //設定
            case 2:
                _isScreen[2] = true;
                _bookAnim.SetFloat("speed", 1);
                _bookAnim.Play("open", 0, 0);                
                StartCoroutine(_settingScr.StartActive());
                _settingScr._isSetting = true;
                AudioManager.Audio.PlaySe(0);
                break;

            //ゲームの終了
            case 3:
                //UnityEditor.EditorApplication.isPlaying = false;
                Application.Quit();
                break;
        }
    }

    /// <summary>
    /// ステージの選択
    /// </summary>
    private void SelectStage()
    {
        //現在のページ数を調べる。
        if (_redopage < _selectScr._SelectPage)
        {
            //現在のページ情報を非表示にする
            _selectModeObj.SetActive(false);
            //位置調整
            _selectLightObj.transform.position = new Vector3(_selectLightObj.transform.position.x, 100, 0);

            AudioManager.Audio.PlaySe(0);

            //本を捲るアニメーションの再生
            _bookAnim.SetFloat("speed", -1);
            _bookAnim.Play("open", 0, 0.9f);
        }
        else if (_redopage > _selectScr._SelectPage)
        {
            //現在のページ情報を非表示にする
            _selectModeObj.SetActive(false);
            //位置調整
            _selectLightObj.transform.position = new Vector3(_selectLightObj.transform.position.x, 100, 0);

            AudioManager.Audio.PlaySe(0);

            //本を捲るアニメーションの再生
            _bookAnim.SetFloat("speed", 1);
            _bookAnim.Play("open", 0, 0.1f);
        }

        //現在のページを更新
        _redopage = _selectScr._SelectPage;
    }

    /// <summary>
    /// ゲーム開始
    /// </summary>
    private void GameStart()
    {
        //操作不可にする
        _isEventNow = true;
        //フェードアウト
        _fadeScr.Fadeout(1, 1, 1, 0, 2);
    }
}
