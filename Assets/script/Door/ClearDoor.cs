using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// クリア時の制御
/// </summary>
public class ClearDoor : MonoBehaviour
{
    [SerializeField, Header("クリア範囲")]
    private float _clearArea = 0.5f;
    [SerializeField, Header("ドアオブジェクト")]
    private GameObject _doorObj = default;
    [SerializeField, Header("チュートリアルオブジェクト")]
    private GameObject[] _tutorialList = default;
    [SerializeField, Header("クリア後の選択オブジェ")]
    private GameObject _clearObj = default;
    [SerializeField, Header("セピアオブジェクト")]
    private GameObject[] _sepiaObj = default;
    //プレイヤーオブジェクト
    private GameObject _playerObj = default;
    //プレイヤーの位置
    private Transform _playerPos = default;
    //プレイヤーのスクリプト
    private PlayerController _playerScr = default;
    //カメラオブジェクト
    private GameObject _cameraObj = default;
    //プレイヤーがクリア位置に来ているか確認する
    private Vector2 _limit = default;
    //時間管理
    private float _time = default;
    //クリア判定
    private bool _isClear = default;
    private bool pleview = false;

    //現在のシーン
    public string _currentScene { get; private set; } = default;
    //チュートリアルシーン
    public string _tutorial { get; private set; } = default;

    //現在のシーンがチュートリアルかどうか
    public bool _isTutorialScenes { get; private set; } = false;

    private Animator _fadeAnimator = default;
    private void Awake()
    {
        _currentScene = SceneManager.GetActiveScene().name;
    }

    private void Start()
    {
        //オブジェクト・Componentの取得
        _playerObj = GameObject.Find("Player");
        _playerPos = _playerObj.transform;
        _playerScr = _playerObj.GetComponent<PlayerController>();
        _cameraObj = GameObject.Find("Main Camera");

        //string型でシーン名の代入
        if (SceneManager.GetActiveScene().name == "tutorial")
        {
            _tutorial = "tutorial";
            _fadeAnimator = GameObject.FindGameObjectWithTag("TurorialFade").GetComponent<Animator>();
        }
    }
    /// <summary>
    /// 現在のシーンがチュートリアルシーンかどうか
    /// </summary>
    public void IsMatchingTutorialScene()
    {
        if (_currentScene == _tutorial)
        {
            _isTutorialScenes = true;
        }
        else
        {
            _isTutorialScenes = false;
        }
    }
    
    /// <summary>
    /// チュートリアルオブジェクトの切り替え
    /// </summary>
    public void TutorialClearJudge()
    {
        //現在のプレイヤーの位置を参照
        _limit = new Vector2(Mathf.Abs(Mathf.Max(transform.position.x, _playerPos.position.x) - Mathf.Min(transform.position.x, _playerPos.position.x))
                            , Mathf.Abs(Mathf.Max(_playerPos.position.y, transform.position.y) - Mathf.Min(_playerPos.position.y, transform.position.y)));
        
        //プレイヤー位置がクリア範囲に入っているかどうか
        if (_limit.x <= _clearArea && _limit.y <= _clearArea || _isClear == true)
        {
            //クリア状態にする
            _isClear = true;
            
            //クリア演出
            Dooropen();
            Timercount();
            
            //プレイヤーを操作不可にする。
            _playerScr.StopMove();

            StartCoroutine(TutorialChange());
            
        }
    }
    /// <summary>
    /// ドアに近づいたかどうかのチェック
    /// </summary>
    public void ClearJudge()
    {
        //現在のプレイヤーの位置を参照
        _limit = new Vector2(Mathf.Abs(Mathf.Max(transform.position.x, _playerPos.position.x) - Mathf.Min(transform.position.x, _playerPos.position.x))
                            , Mathf.Abs(Mathf.Max(_playerPos.position.y, transform.position.y) - Mathf.Min(_playerPos.position.y, transform.position.y)));

        //プレイヤー位置がクリア範囲に入っているかどうか
        if (_limit.x <= _clearArea && _limit.y <= _clearArea || _isClear == true)
        {
            //効果音を一度だけ鳴らす
            if(!_isClear)
            {
                //効果音の再生
                AudioManager.Audio.PlaySe(10);
                //その他再生中のSEを停止
                AudioManager.Audio.StopLoopSe();
                AudioManager.Audio.StopGimmickSe();

                //セピアの非表示
                foreach(GameObject obj in _sepiaObj)
                {
                    obj.SetActive(false);
                }
            }

            //クリア状態にする
            _isClear = true;

            //クリア演出
            Dooropen();
            Timercount();

            //プレイヤーを操作不可にする。
            _playerScr.StopMove();
        }
    }

    /// <summary>
    /// ドアを開く
    /// </summary>
    void Dooropen()
    {
        _doorObj.transform.eulerAngles = new Vector3(0, Mathf.Lerp(_doorObj.transform.eulerAngles.y, 180, 0.05f), 0);
    }

    /// <summary>
    /// カメラをズームさせる
    /// </summary>
    void Cmerarmove()
    {
        _cameraObj.transform.position = Vector3.Lerp(_cameraObj.transform.position, new Vector3(_cameraObj.transform.position.x, _cameraObj.transform.position.y, 8.6f), 0.05f);
        if (_time >= 4 && !pleview)
        {
            Time.timeScale = 0;
            _clearObj.SetActive(true);
            pleview = !pleview;
        }
    }

    /// <summary>
    /// 画面が近づいた後に次に移るまでの時間処理
    /// </summary>
    void Timercount()
    {
        //時間
        _time += Time.deltaTime;

        //秒数以上経過したら、カメラをズームさせる
        if (_time >= 2)
        {
            Cmerarmove();
        }
        else
        {
            _cameraObj.transform.position = Vector3.Lerp(_cameraObj.transform.position, new Vector3(_playerPos.position.x, _playerPos.position.y, 5), 0.1f);
        }
    }

    /// <summary>
    /// クリアメニューを表示させる
    /// </summary>
    /// <param name="clearMenu">クリアメニューのオブジェクト</param>
    public void Clearmenu(GameObject clearMenu)
    {
        //秒数管理
        if (_time >= 6)
        {
            //タイムスケールを０にする
            Time.timeScale = 0;
            //メニューの表示
            clearMenu.SetActive(true);
        }
    }
    IEnumerator TutorialChange()
    {
        if (_tutorialList[0].activeSelf)
        {
            _fadeAnimator.SetBool("tutorialObjectChange", true);
            yield return new WaitForSeconds(1);
            _tutorialList[0].SetActive(false);
            _tutorialList[1].SetActive(true);
            _fadeAnimator.SetBool("tutorialObjectChange", false);
        }
        else if (_tutorialList[1].activeSelf)
        {
            _fadeAnimator.SetBool("tutorialObjectChange", true);
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene("title");
        }
        yield break;
    }

}
