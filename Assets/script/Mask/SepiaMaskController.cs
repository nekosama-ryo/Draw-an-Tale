using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SepiaMaskController : MonoBehaviour
{

    public Stack<GameObject> _SepiaMaskStack = new Stack<GameObject>(20);
    [SerializeField, Header("マスク生成時間の間隔")] private float _instantiateTime = 0.1f;
    [SerializeField]
    private GameObject sepiaMaskObj=default;
    private Vector3 pos;
    private float _time = 0;
    [SerializeField, Header("マスクの最小サイズ")] private float maskScaleMin = 0.16f;
    [SerializeField, Header("マスクの最大サイズ")] private float maskScaleMax = 0.5f;
    private PlayerController _playerController;
    private Transform _myTransform = default;
    [SerializeField, Header("マスクの生成位置（PlayerPos+入れる値）")] private float _maskInstantiatePos = 0f;
    private bool isTurn = true;
    void Start()
    {
        _playerController = this.gameObject.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (this.transform.localScale.x == -1)
        {
            isTurn = false;
        }
        else if (this.transform.localScale.x == 1)
        {
            isTurn = true;
        }
    }

    //void Update()
    //{
    //    float maskScale = 0f;
    //    maskScale = Random.Range(maskScaleMin, maskScaleMax);
    //    pos = this.gameObject.transform.position;
    //    if (_instantiateTime < _time)
    //    {
    //        GameObject obj = Pool();
    //        obj.transform.localScale = new Vector3(maskScale, maskScale, 1);
    //        obj.SetActive(true);
    //        obj.transform.position = gameObject.transform.position;
    //        _time = 0;
    //    }
    //    else
    //    {
    //        _time += _instantiateTime.deltaTime;
    //    }
    //}

    public void MaskInstantiate()
    {
        _myTransform = this.transform;
        pos = _myTransform.position;
        float maskScale = 0f;
        maskScale = Random.Range(maskScaleMin, maskScaleMax);

        //右
        if (_playerController.isMatchingSprite == true && isTurn == true)
        {
            pos = new Vector3(pos.x + _maskInstantiatePos, pos.y, pos.z);
            GameObject obj = Pool();
            obj.transform.localScale = new Vector3(maskScale, maskScale, 1);
            obj.SetActive(true);
            obj.transform.position = pos;
            _time = 0;
        }
        //左
        else if (_playerController.isMatchingSprite == true && isTurn == false)
        {
            pos = new Vector3(pos.x + -_maskInstantiatePos, pos.y, pos.z);
            //反転した時の処理
            GameObject obj = Pool();
            obj.transform.localScale = new Vector3(maskScale, maskScale, 1);
            obj.SetActive(true);
            obj.transform.position = pos;
            _time = 0;
        }
        //ジャンプ中右
        else if (_playerController.isJumping == true && isTurn == true&&_instantiateTime<_time)
        {
            pos = new Vector3(pos.x + _maskInstantiatePos, pos.y, pos.z);
            GameObject obj = Pool();
            obj.transform.localScale = new Vector3(maskScale, maskScale, 1);
            obj.SetActive(true);
            obj.transform.position = pos;
            _time = 0;
        }
        //ジャンプ中左
        else if (_playerController.isJumping == true && isTurn == false&&_instantiateTime<_time)
        {
            pos = new Vector3(pos.x + -_maskInstantiatePos, pos.y, pos.z);
            //反転した時の処理
            GameObject obj = Pool();
            obj.transform.localScale = new Vector3(maskScale, maskScale, 1);
            obj.SetActive(true);
            obj.transform.position = pos;
            _time = 0;
        }
        else
        {
            _time += Time.deltaTime;
        }
    }

    private GameObject Pool()
    {


        //個数を確認
        if (_SepiaMaskStack.Count > 0)
        {
            return _SepiaMaskStack.Pop();
        }

        //新規生成
        return Instantiate(sepiaMaskObj, pos, transform.rotation);

    }
}
