using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter2D : MonoBehaviour
{

    public string tag;

    public Transform Output;

    public Collider2D EnableC;


    bool Triggered;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == tag && FindObjectOfType<GameController2D>().CarHorned && !Triggered)
        {
            Triggered = true;
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
            Output.GetComponent<SpriteRenderer>().enabled = true;
            EnableC.enabled = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == tag && Triggered)
        {
            FindObjectOfType<GameController2D>().Player.transform.position = Output.transform.position;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
