using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 風の動きを制御する
/// </summary>
public class Wind : MonoBehaviour
{
    [SerializeField,Header("風の始点、終点")]
    private Vector2[] _windPos = new Vector2[2];

    [SerializeField,Header("右向きの風")]
    private bool _isRight=default;

    [SerializeField, Header("風の範囲")]
    private float _hitX=default;
    [SerializeField]
    private float _hitY=default;

    //力の量
    [SerializeField,Header("風の強さ")]
    private float _force = default;
    public float _Vec { get; private set; }

    private void Start()
    {
        //風の効果音を再生
        AudioManager.Audio.PlayGimmickSe(9);
    }

    /// <summary>
    /// 風の当たり位置を設定
    /// </summary>
    public void SetCollison()
    {
        //現在の位置を起点に設定
        _windPos[0] = transform.position;

        //風の向きによって、終点を設定
        if (!_isRight)
        {
            _windPos[1] = new Vector2(_windPos[0].x - _hitX, _windPos[0].y - _hitY);
            
        }
        else
        {
            _windPos[1] = new Vector2(_windPos[0].x + _hitX, _windPos[0].y - _hitY);
        }
    }

    /// <summary>
    /// 風の影響を受けているか調べる
    /// </summary>
    /// <param name="pos">位置</param>
    /// <param name="rb">リジッドボディ</param>
    /// <returns>影響を受けているか</returns>
    public bool CheckWind(Transform pos, Rigidbody2D rb)
    {
        //どちらのページか
        if (!_isRight)
        {
            //影響を受けていたら、逆向きに力を加える
            if ((_windPos[0].x > pos.position.x && _windPos[1].x < pos.position.x) && _windPos[0].y > pos.position.y && _windPos[1].y < pos.position.y)
            {
                _Vec = -_force;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            //影響を受けていたら、逆向きに力を加える
            if (_windPos[0].x < pos.position.x && _windPos[1].x > pos.position.x && _windPos[0].y > pos.position.y && _windPos[1].y < pos.position.y)
            {
                _Vec = _force;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
