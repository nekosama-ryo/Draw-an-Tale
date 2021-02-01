using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SepiaMask : MonoBehaviour
{
    private SepiaMaskController _sepiaMaskController;


    void Start()
    {
        _sepiaMaskController = GameObject.FindObjectOfType<SepiaMaskController>();
    }


    void Update()
    {
        
    }

    IEnumerator SepiaMaskFalse()
    {
        yield return new WaitForSeconds(1.1f);
        gameObject.SetActive(false);
        _sepiaMaskController._SepiaMaskStack.Push(gameObject);
        yield break;
    }
    private void OnEnable()
    {
        StartCoroutine(SepiaMaskFalse());
    }
}
