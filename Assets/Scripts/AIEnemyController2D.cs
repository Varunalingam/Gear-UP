using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEnemyController2D : MonoBehaviour
{

    public int AIMode = 0;
    public AIEnemyTriggerReceiver2D PlayerRange;
    bool moving;
    bool ActivateMoving;
    public AIEnemyTriggerReceiver2D EndPoint;
    public AIEnemyTriggerReceiver2D StartPoint;


    public int strength = 10;


    public bool ModeFirst = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool Contact = false;

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && AIMode == 0)
        {
            if (PlayerRange.Triggered.layer == 8 && FindObjectOfType<GameController2D>().PlayMode == 1)
                FindObjectOfType<GameController2D>().AddAICollectables(3, -1 * strength);
            else
                FindObjectOfType<GameController2D>().AddCollectables(3, -1 * strength);

            if (FindObjectOfType<GameController2D>().SFX == true)
                gameObject.GetComponent<AudioSource>().PlayOneShot(gameObject.GetComponent<AudioSource>().clip,1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Bg" && !Contact)
        {
            Contact = true;
            if (!ModeFirst)
            {
                if (AIMode == 1)
                {
                    gameObject.GetComponent<Rigidbody2D>().Sleep();
                    Collider2D[] a = gameObject.GetComponents<Collider2D>();
                    foreach (Collider2D asx in a)
                    {
                        asx.enabled = false;
                    }
                }
                ModeFirst = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Bg" && Contact)
        {
            Contact = false;
        }
    }

    // Update is called once per frame
    bool contact = false;
    float movetimer = 0;
    bool RunOnce = false;
    void FixedUpdate()
    {
        if (!Contact && contact)
        {
            gameObject.transform.Rotate(new Vector3(0, 0, -1), 4);
        }

        if (contact && (gameObject.GetComponent<Rigidbody2D>().velocity.magnitude == 0 || gameObject.GetComponent<Rigidbody2D>().velocity.magnitude.ToString().Contains("E-")) && moving)
        {
            gameObject.transform.Rotate(new Vector3(0, 0, 1), 2);
        }

        Collider2D[] A = new Collider2D[5];
        gameObject.GetComponent<Rigidbody2D>().GetContacts(A);
        contact = false;
        foreach (Collider2D C in A)
        {
            try
            {
                if (C.gameObject.tag == "Bg")
                {
                    contact = true;
                }
            }
            catch
            {
                break;
            }
        }

        if (AIMode == 0)
        {
            if (FindObjectOfType<GameController2D>().PlayMode == 0)
            {
                ActivateMoving = (FindObjectOfType<GameController2D>().CarHorned && PlayerRange.Trigger) && !EndPoint.Trigger;   
            }
            else
            {
                ActivateMoving = ((FindObjectOfType<GameController2D>().CarHorned || FindObjectOfType<GameController2D>().AICarHorn) && PlayerRange.Trigger) && !EndPoint.Trigger;
            }
            if (EndPoint.Trigger)
            {
                Destroy(gameObject);
            }
        }
        if (AIMode == 1)
        {
            if (StartPoint.Trigger == true && !RunOnce)
            {
                ActivateMoving = true;
                Destroy(StartPoint.gameObject);
                gameObject.GetComponent<Rigidbody2D>().WakeUp();
                Collider2D[] a = gameObject.GetComponents<Collider2D>();
                foreach (Collider2D asx in a)
                {
                    asx.enabled = true;
                }
                RunOnce = true;
            }

            if (RunOnce)
            {
                ActivateMoving = !PlayerRange.Trigger && !EndPoint.Trigger;
                if (PlayerRange.Trigger)
                {
                    if (PlayerRange.Triggered.layer == 8 && FindObjectOfType<GameController2D>().PlayMode == 1)
                        FindObjectOfType<GameController2D>().AddAICollectables(3, -1 * strength);
                    else
                        FindObjectOfType<GameController2D>().AddCollectables(3, -1 * strength);

                    if (FindObjectOfType<GameController2D>().SFX == true)
                        gameObject.GetComponent<AudioSource>().PlayOneShot(gameObject.GetComponent<AudioSource>().clip, 1f);
                }

                if (EndPoint.Trigger && FindObjectOfType<GameController2D>().PlayMode == 1)
                {
                    Destroy(gameObject);
                }
            }
        }

        if (ActivateMoving)
        {
            Movement();
            movetimer = 0;
        }
        else if (moving)
        {
            movetimer += Time.fixedDeltaTime;
            if (movetimer < 1)
            {
                Movement();
            }
            else
            {
                moving = false;
                gameObject.GetComponent<Animator>().SetBool("idle", true);
                gameObject.GetComponent<Rigidbody2D>().velocity = 0.9f * gameObject.GetComponent<Rigidbody2D>().velocity;
                gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0;
                gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
                gameObject.GetComponent<Rigidbody2D>().freezeRotation = true;
            }
        }
    }

    void Movement()
    {
        
        moving = true;

        gameObject.GetComponent<Animator>().SetBool("idle", false);      
        if (contact)
        {
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
            gameObject.GetComponent<Rigidbody2D>().freezeRotation = true;
            gameObject.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(10, 0));
        }
        else
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = 0.9f * gameObject.GetComponent<Rigidbody2D>().velocity;
            gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0;
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
            gameObject.GetComponent<Rigidbody2D>().freezeRotation = false;
        }
    }
}
