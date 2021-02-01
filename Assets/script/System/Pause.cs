using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// ポーズを管理
/// </summary>
public class Pause : MonoBehaviour
{
    /// <summary>
    /// Escを押した際にポーズ状態を切り替える
    /// </summary>
    /// <param name="pauseObj">ポーズオブジェクト</param>
    public void PauseControl(GameObject pauseObj)
    {
        //対応したキーを押すとポーズ状態を切り替える
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseCancel(pauseObj);
        }
    }

    /// <summary>
    /// ポーズ状態を切り替える
    /// </summary>
    /// <param name="pauseObj">ポーズオブジェクト</param>
    public void PauseCancel(GameObject pauseObj)
    {
        //オブジェクトの状態を切り替える
        pauseObj.SetActive(!pauseObj.activeSelf);

        //タイムスケールを変更
        if (pauseObj.activeSelf)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// ステージのやり直し処理
    /// </summary>
    public void ReStart()
    {
        //タイムスケールを１にする
        Time.timeScale = 1;

        //ゲームシーンを再ロード
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// タイトルに移動
    /// </summary>
    public void TitleScene()
    {
        //タイムスケールを１にする
        Time.timeScale = 1;

        //タイトルシーンに遷移
        SceneManager.LoadScene("title");
    }

    /// <summary>
    /// 次のシーンを読み込む
    /// </summary>
    public void Next()
    {
        //ポーズを解除
        Time.timeScale = 1;
        //次回のステージに移動（シーンごとに行うので、ゲッポ処理になってる）
        GameObject.Find("SelectStageMove").GetComponent<StageSelect>()._SelectStage += 1;
        GameObject.Find("Fademanager").GetComponent<FadeOut>().Fadeout(1, 1, 1, 0, 2);
    }
}

