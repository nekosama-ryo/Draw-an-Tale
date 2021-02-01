using UnityEngine;
/// <summary>
/// プレイヤーの地面接触判定を管理
/// </summary>
public class LegCollider : MonoBehaviour
{
    //ジャンプ可能かどうか
    public bool _CanJump = true;
    //プレイヤーのスクリプト
    private PlayerController _playerScr = default;

    private void Start()
    {
        //コンポーネントの取得
        _playerScr = GameObject.Find("Player").gameObject.GetComponent<PlayerController>();
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        //地面、インクに触れたらジャンプ可能状態にする。
        if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("ink") || collision.gameObject.CompareTag("_ink"))
        {
            //ジャンプ可能にする。
            _CanJump = true;
            //プレイヤーアニメーションのジャンプを中止する。
            _playerScr._Anim.SetBool("Jump", false);
        }
    }
    public void OnCollisionExit2D(Collision2D collision)
    {
        //地面、インクから離れたらジャンプ不可能状態にする。
        if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("ink") || collision.gameObject.CompareTag("_ink"))
        {
            //ジャンプ不可能にする。
            _CanJump = false;
            //プレイヤーアニメーションのジャンプをする。
            _playerScr._Anim.SetBool("Jump", true);
        }
    }
}