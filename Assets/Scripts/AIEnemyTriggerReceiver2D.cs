using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEnemyTriggerReceiver2D : MonoBehaviour
{
    public string Tag = "Player";

    public bool Trigger = false;

    public Transform follower;

    public GameObject Triggered;

    public void FixedUpdate()
    {
        if (follower != null)
        {
            gameObject.transform.position = follower.position;
            gameObject.transform.rotation = follower.rotation;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Tag)
        {
            Trigger = true;
            Triggered = collision.gameObject;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == Tag)
        {
            Trigger = true;
            Triggered = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    { 
        if (other.tag == Tag)
        {
            Trigger = false;
            Triggered = null;
        }
    }
}
