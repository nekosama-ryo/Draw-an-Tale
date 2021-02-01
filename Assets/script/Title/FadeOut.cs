using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// フェードアウトを処理
/// </summary>
public class FadeOut : MonoBehaviour
{
    [SerializeField, Header("フェードアウト")]
    private Image fadeimage = default;
    [SerializeField, Header("ステージスクリプト")]
    private StageSelect Stage=default;

    //シングルトン
    public static FadeOut fadein = default;
    //フェードアウト中かどうか
    public bool _IsFadeOut { get; private set; } = default;
    //画面切り替え時のフェードイン
    private bool _isFadeIn = default;
    //フェードアウト用
    private bool _isFadeOut = default;
    private float _alfa = default;
    //色の配色
    float r = default, g = default, b = default;
    int _bgm = default;

    //チュートリアル
    public bool IsTutorial = false;

    private void Awake()
    {
        //シングルトンの処理
        if (fadein == null)
        {
            fadein = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        //α値の初期化
        _alfa = 1;
        //次回のシーンをロード
        SceneManager.sceneLoaded += SceneLoaded;
    }

    /// <summary>
    /// シーンをロードする
    /// </summary>
    /// <param name="nextscene">次回のシーン名</param>
    /// <param name="mode">ロードの仕方</param>
    void SceneLoaded(Scene nextscene, LoadSceneMode mode)
    {
        _isFadeIn = true;
    }

    private void FixedUpdate()
    {
        //フェードインが呼ばれた
        if (_isFadeIn)
        {
            //時間経過でα値を引く
            _alfa -= Time.deltaTime;
            //カラーの変更
            colorset();

            //０になったらフラグを切り替える
            if (_alfa <= 0)
            {
                _IsFadeOut = false;
                _isFadeIn = false;
            }
        }

        //フェードアウト処理
        if (_isFadeOut)
        {
            //徐々にα値を足す
            _alfa += Time.deltaTime;
            //カラーの変更
            colorset();

            //フェードアウトが完了したら
            if (_alfa >= 1)
            {
                _isFadeOut = false;
                //シーン遷移
                AudioManager.Audio.PlayBgm(_bgm);

                //チュートリアルシーンかどうか
                if(IsTutorial)
                {
                    SceneManager.LoadScene(7);
                }
                else
                {
                    SceneManager.LoadScene(Stage._SelectStage);
                }
            }
        }
    }

    /// <summary>
    /// フェードアウト処理
    /// </summary>
    /// <param name="R">赤</param>
    /// <param name="G">緑</param>
    /// <param name="B">青</param>
    /// <param name="A">透明度</param>
    /// <param name="NextSceneName">次回のシーンの名前</param>
    public void Fadeout(int R, int G, int B, int A ,int BGM)
    {
        //フェードアウト
        _IsFadeOut = true;
        _isFadeOut = true;

        //引数を記録
        r = R;
        g = G;
        b = B;
        _alfa = A;
        _bgm = BGM;
    }

    /// <summary>
    /// 色を設定
    /// </summary>
    void colorset()
    {
        fadeimage.color = new Color(r, g, b, _alfa);
    }
}
