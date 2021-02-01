using System.Collections;
using UnityEngine;
/// <summary>
/// 音の再生や停止をする
/// </summary>
public class AudioManager : MonoBehaviour
{
    //シングルトン
    public static AudioManager Audio = default;

    //スクリプト
    [SerializeField, Header("設定スクリプト")]
    private SettingMenu _settingScr = default;

    //音楽データ
    [SerializeField, Header("音楽")]
    private AudioClip[] _bgmClips=default;
    [SerializeField, Header("効果音")]
    private AudioClip[] _seClips = default;

    //再生用のソース
    [SerializeField,Header("BGMのソース")]
    private AudioSource _audioBGM = default;
    [SerializeField, Header("SEのソース")]
    private AudioSource _audioSE = default;
    [SerializeField, Header("SEのソース")]
    private AudioSource _audioGimmickSE = default;

    private void Awake()
    {
        //シングルトンの処理
        if (Audio == null)
        {
            Audio = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// 秒数待機後にSEを再生する
    /// </summary>
    /// <param name="Number">再生するSE番号</param>
    /// <param name="time">再生までの時間</param>
    private IEnumerator WaitPlay(int Number,float time=0)
    {
        //秒数待機
        yield return new WaitForSeconds(time);

        //効果音の音量を調整
        _audioSE.volume = _settingScr.SeVolume;
        //効果音の再生
        _audioSE.PlayOneShot(_seClips[Number]);

        //コルーチン終了
        yield break;
    }
    /// <summary>
    /// 効果音を再生する
    /// </summary>
    /// <param name="Number">再生するSE番号</param>
    /// <param name="time">再生までの時間</param>
    public void PlaySe(int Number, float time = 0)
    {
        //再生用のコルーチンを呼び出す
        StartCoroutine(WaitPlay(Number, time));
    }

    /// <summary>
    /// 効果音をループ再生する
    /// </summary>
    /// <param name="Number">再生するSE番号</param>
    public void PlayLoopSe(int Number)
    {
        //効果音の音量を調整
        _audioSE.volume = _settingScr.SeVolume;
        //効果音の再生
        _audioSE.clip = _seClips[Number];
        _audioSE.Play();
    }

    /// <summary>
    /// 効果音のループ再生を停止する
    /// </summary>
    public void StopLoopSe()
    {
        _audioSE.clip = null;
    }

    /// <summary>
    /// ギミック用の効果音を再生する
    /// </summary>
    /// <param name="Number">再生するSE番号</param>
    public void PlayGimmickSe(int Number)
    {
        //効果音の音量を調整
        _audioGimmickSE.volume = _settingScr.SeVolume;

        //効果音の再生
        _audioGimmickSE.clip = _seClips[Number];
        _audioGimmickSE.Play();
    }

    /// <summary>
    /// ギミック用の効果音を停止する
    /// </summary>
    public void StopGimmickSe()
    {
        _audioGimmickSE.clip = null;
    }

    /// <summary>
    /// 音楽を再生する
    /// </summary>
    /// <param name="Number">再生するBGM番号</param>
    public void PlayBgm(int Number)
    {
        //音楽の音量調整
        _audioBGM.volume =_settingScr.BgmVolume;

        //音楽の再生
        _audioBGM.clip = _bgmClips[Number];
        _audioBGM.Play();
    }

    /// <summary>
    /// 再生中の音楽の音量を調整する
    /// </summary>
    public void BgmVolume()
    {
        _audioBGM.volume = _settingScr.BgmVolume;
    }
}
