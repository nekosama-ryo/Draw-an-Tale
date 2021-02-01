using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public bool burning = false;
    
    void Start()
    {
        
    }

    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ink") || collision.gameObject.CompareTag("_ink"))
        {
            Debug.Log("Fire");
            burning = true;
        }
    }

    //public void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("ink") | collision.gameObject.CompareTag("_ink") | collision.gameObject.CompareTag("Metaball_liquid"))
    //    {
    //        Debug.Log("Fire");
    //    }
    //}
}
