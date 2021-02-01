using UnityEngine;
/// <summary>
/// プレイヤーの動作を管理する
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField, Header("移動スピード")]
    private float _moveSpeed = default;
    [SerializeField, Header("ジャンプ力")]
    private float _jumpPower = default;
    //プレイヤーの入力
    private float _horizontal = default;
    //横移動のスピード
    private float _xSpeed = 0.0f;
    //現在Drawモード中かどうか
    public bool _IsDrawMode { get; private set; } = default;
    //プレイヤーが行動可能かどうか
    public bool _IsStop { get; private set; } = true;
    //本動作中は行動不可
    public bool _CanMove = false;
    //傾斜のリミット
    private bool _islimitSlope = default;
    //初期位置を保存
    private Vector3 _startPos = default;

    //コンポーネント
    private Rigidbody2D _rb = default;
    public Animator _Anim { get; private set; } = default;


    [SerializeField, Header("左右のページオブジェクト位置")]
    private Transform[] _pages = default;
    //各ページの位置
    private Vector3[] _pagePoses = new Vector3[2];

    //カメラオブジェクト
    private GameObject _cameraObj = default;

    //ジャンプ管理のスクリプト
    private LegCollider _LegScr = default;
    public bool isJumping { get; private set; } = false;

    private SpriteRenderer playerSprite;
    [SerializeField] private Sprite moveSprite1=default;
    [SerializeField] private Sprite moveSprite2=default;
    [SerializeField] private Sprite moveSprite3=default;
    private SepiaMaskController sepiaMaskController=default;
    public bool isMatchingSprite { get; private set; }
    void Start()
    {
        //コンポーネント・オブジェクトの取得
        _rb = GetComponent<Rigidbody2D>();
        _Anim = GetComponent<Animator>();
        _LegScr = GameObject.Find("Leg").gameObject.GetComponent<LegCollider>();
        _cameraObj = GameObject.Find("0-BGCamera");

        //現在位置を保存
        _startPos = transform.position;

        //ページ位置を保存、修正。
        _pagePoses[0] = _pages[0].position;
        _pagePoses[1] = _pages[1].position;
        _pagePoses[0].z = 9.7f;
        _pagePoses[1].z = 9.7f;
        playerSprite = this.gameObject.GetComponent<SpriteRenderer>();
        sepiaMaskController = this.gameObject.GetComponent<SepiaMaskController>();
    }

    /// <summary>
    /// プレイヤーの動作
    /// </summary>
    public void PlayerMove()
    {
        //行動可能かどうか
        if (_IsStop)
        {
            //本が動作中は行動不可
            if (_CanMove == true)
            {
                //すべての位置を固定
                _rb.constraints = RigidbodyConstraints2D.FreezeAll;
                return;
            }
            else
            {
                //Z軸の調整
                transform.position = new Vector3(transform.position.x, transform.position.y, _startPos.z);

                //回転固定
                if (_rb.constraints == RigidbodyConstraints2D.FreezeAll)
                {
                    _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
            }

            //Drawモード中じゃない場合
            if (!_IsDrawMode)
            {
                //入力を保存
                _horizontal = Input.GetAxisRaw("Horizontal");

                //キーが入力されたらジャンプする
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    //ジャンプ可能かどうか
                    if (_LegScr._CanJump == true)
                    {
                        AudioManager.Audio.PlaySe(3);
                        //上に力を加える
                        _rb.velocity = transform.up * _jumpPower;

                        //ジャンプしたことを伝える
                        _LegScr._CanJump = false;
                        isJumping = true;

                        //アニメーションを変更
                        _Anim.SetBool("idle", false);
                        _Anim.SetBool("Walk", false);
                        _Anim.SetBool("Jump", true);
                    }
                }

                //左右の入力を見る
                if (_horizontal > 0) //左入力
                {
                    //スプライトの向きを変更
                    transform.localScale = new Vector3(1, 1, 1);

                    //アニメーションを変更
                    _Anim.SetBool("idle", false);
                    _Anim.SetBool("Walk", true);

                    //スピードの保存
                    _xSpeed = _moveSpeed;
                }
                else if (_horizontal < 0) //右入力
                {
                    //スプライトの向きを変更
                    transform.localScale = new Vector3(-1, 1, 1);

                    //アニメーションを変更
                    _Anim.SetBool("idle", false);
                    _Anim.SetBool("Walk", true);

                    //スピードの保存
                    _xSpeed = -_moveSpeed;
                }

            }
            else
            {
                //動作しない
                _horizontal = 0;
            }

            //未入力時
            if (_horizontal == 0)
            {
                //スピードを０に。
                _xSpeed = 0;

                //アニメーションを変更
                _Anim.SetBool("Walk", false);
                _Anim.SetBool("idle", true);
            }
        }
        else
        {
            //アニメーションの変更
            _Anim.SetBool("Walk", false);
            _Anim.SetBool("idle", true);

            //入力操作通りに力を加える。
            _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
        }
    }


    /// <summary>
    /// プレイヤーが行動不可になる。
    /// </summary>
    public void StopMove()
    {
        _IsStop = false;
    }

    /// <summary>
    /// Drawモードを切り替える
    /// </summary>
    /// <param name="drawnow">Drawモード中かどうか</param>
    public void DrawModeChange(bool drawnow)
    {
        //入力されたかどうか
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetMouseButtonDown(1))
        {
            //ジャンプ可能状態、且つDrawモード中でなければ、Drawモードを切り替える。
            if (_LegScr._CanJump && !drawnow)
            {
                _IsDrawMode = !_IsDrawMode;
                AudioManager.Audio.PlaySe(2);
            }
        }
    }

    /// <summary>
    /// フィルターを掛ける
    /// </summary>
    /// <param name="filter">フィルターオブジェクト</param>
    public void Filter(GameObject filter)
    {
        //自身の親ページを調べる
        if (transform.parent.name == "Left_Page")
        {
            //フィルターをオンにする。
            //filter.SetActive(true);
            //フィルター位置をページ位置に調整
            //filter.transform.position = _pagePoses[1];
        }
        else
        {
            //フィルターをオンにする。
            //filter.SetActive(true);
            //フィルター位置をページ位置に調整
            //filter.transform.position = _pagePoses[0];
        }
    }

    /// <summary>
    /// リミット時に滑らせる。
    /// </summary>
    /// <param name="vec"></param>
    public void Move(float vec)
    {
        //リミット状態を確認し、滑らせる。
        if (!_islimitSlope)
        {
            _rb.velocity = new Vector3(_xSpeed + vec, _rb.velocity.y, 0);
        }
        else
        {
            _rb.velocity = new Vector3(_rb.velocity.x, _rb.velocity.y, 0);
        }
    }

    /// <summary>
    /// プレイヤーが行動可能かどうか
    /// </summary>
    /// <param name="drawnow">Drawモード中かどうか</param>
    /// <returns></returns>
    public bool OneBack(bool drawnow)
    {
        //ジャン中や、Draw中は戻せない
        if (_LegScr._CanJump && !drawnow)
        {
            return true;
        }

        return false;
    }
    public void PlayerSpriteMatch()
    {
        sepiaMaskController.MaskInstantiate();
        if (moveSprite1 == playerSprite.sprite || moveSprite2 == playerSprite.sprite || moveSprite3 == playerSprite.sprite)
        {
            isMatchingSprite = true;
        }
        else
        {
            isMatchingSprite = false;
        }
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        //インクに当たった際の処理
        if (collision.gameObject.CompareTag("ink") || collision.gameObject.CompareTag("_ink"))
        {
            //接触したインクが一定角度以上の際リミットを掛ける
            if (Mathf.Abs(collision.transform.localEulerAngles.z) >= 45 && Mathf.Abs(collision.transform.localEulerAngles.z) <= 135)
            {
                _islimitSlope = true;
            }
            else if (Mathf.Abs(collision.transform.localEulerAngles.z) >= 225 && Mathf.Abs(collision.transform.localEulerAngles.z) <= 315)
            {
                _islimitSlope = true;
            }
            else
            {
                //一定角度以下の場合リミットを解除。
                _islimitSlope = false;
            }
            isJumping = false;
        }
        else if(collision.gameObject.CompareTag("Floor"))
        {
            isJumping = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //地面に接触した際にリミットを解除する。
        if (collision.gameObject.CompareTag("Floor"))
        {
            _islimitSlope = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //本が開いている場合
        if (!Book.IsClose)
        {
            //自身を当たったページの子供にする。
            if (collision.gameObject.name == "Right" || collision.gameObject.name == "Left")
            {
                transform.parent = collision.gameObject.transform;
            }
        }
    }
}
