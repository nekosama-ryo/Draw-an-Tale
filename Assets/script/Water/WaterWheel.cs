using UnityEngine;
/// <summary>
/// 水車の動作を管理
/// </summary>
public class WaterWheel : MonoBehaviour
{
    [SerializeField, Header("自身のRigidbody2D")]
    private Rigidbody2D _rb=default;
    [SerializeField, Header("動かしたい仕掛けオブジェクト")]
    private GameObject _Obj=default;
    [SerializeField, Header("最終的に動く合計量")]
    private Vector3 _endPos = default;
    [SerializeField, Header("必要力量")]
    private float _power=default;
    [SerializeField, Header("回転方向")]
    private bool _left=default;

    [SerializeField, Header("クリアマスクのスクリプト")]
    private ClearMask _clearMaskScr = default;
    [SerializeField, Header("呼び出したいマスク番号")]
    private int _maskNumber = default;

    //回転方向
    private int _direction = 1;
    //合計回転量
    private float _totalRot=default;
    //初期位置
    private Vector3 _startPos=default;

    void Start()
    {
        //スタート位置を記録
        _startPos = _Obj.transform.position;

        //方向取得
        if(_left)
        {
            _direction = -1;
        }

        //動かすオブジェクトの終点位置を設定
        _endPos = new Vector3(_Obj.transform.position.x + _endPos.x, _Obj.transform.position.y + _endPos.y, _Obj.transform.position.z + _endPos.z);
    }

    /// <summary>
    /// 水車の動作
    /// </summary>
    public void Wheel()
    {
        //本が開いた際に固定を解除する。
        if (_rb.constraints==RigidbodyConstraints2D.FreezeAll)
        {
            _rb.constraints = RigidbodyConstraints2D.FreezePosition;
        }

        //自身に力が加わった場合
        if (_rb.angularVelocity!=0)
        {
            //回転量を足していく。
            _totalRot = Mathf.Clamp(_totalRot + Mathf.Round(_rb.angularVelocity) * _direction, 0, _power);
            //回転量に応じてギミックが動く
            _Obj.transform.position = Vector3.Lerp(_startPos, _endPos, _totalRot / _power);
        }

        //終点についたら色付けを行う
        if(_Obj.transform.position==_endPos)
        {
            _clearMaskScr.GimmickClear(_maskNumber);
        }
    }

    /// <summary>
    /// 水車の動作を停止させる。
    /// </summary>
    public void StopWheel()
    {
        //固定する。
        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
}
