using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReSpawn : MonoBehaviour
{
    private GameObject _reSpawnCollider = default;

    [SerializeField,Header("プレイヤーオブジェクト")]
    private Transform _playerObj = default;

    [SerializeField]
    private LegCollider _legScr = default;
    private Vector3 _currentPlayerObj = default;
    private float _time;

    void Start()
    {
        _reSpawnCollider = this.gameObject;
        _playerObj = GameObject.FindGameObjectWithTag("Player").transform;
    }

    
    void Update()
    {
        if (_legScr._CanJump && _time > 0.5f)
        {
            _currentPlayerObj = _playerObj.position;
            _time = 0;
        }
        _time += Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _playerObj.transform.position = _currentPlayerObj;
        }
    }
}
