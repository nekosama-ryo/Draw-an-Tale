using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SepiaClear : MonoBehaviour
{

    [SerializeField, Header("WaveObject")]
    private GameObject _WaveObject=default;
    [SerializeField, Header("CepiaCamera")]
    private GameObject _SepiaCamera=default;

    Animator anim;
    SepiaColor sepiaScr;
    void Start()
    {
        anim = _WaveObject.GetComponent<Animator>();
        sepiaScr = _SepiaCamera.GetComponent<SepiaColor>();
    }

    void Update()
    {
        
    }

    IEnumerator WaveEnable()
    {
        _WaveObject.SetActive(true);
        anim.SetBool("Wave", true);
        yield return new WaitForSeconds(2);
        sepiaScr.enabled = false;
        yield return new WaitForSeconds(2);
        _WaveObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine("WaveEnable");
        }
    }
}
