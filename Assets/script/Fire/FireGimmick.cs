using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireGimmick : MonoBehaviour
{
    [SerializeField]
    private GameObject _flame=default;
    [SerializeField]
    private GameObject _burningObj=default;
    private Fire fireScr=default;
    bool burning2=false;
    void Start()
    {
        fireScr = _flame.GetComponent<Fire>();
    }


    void Update()
    {
        if (fireScr.burning==true && burning2==true)
        {
            StartCoroutine(BurningObj());
            Debug.Log("burning");//燃やす処理(オブジェクト消すなど)
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ink"))
        {
            Debug.Log("Fire2");
            burning2 = true;
        }
    }

    IEnumerator BurningObj()
    {
        
        yield return new WaitForSeconds(2);
        _burningObj.SetActive(false);
        yield break;
    }
}
