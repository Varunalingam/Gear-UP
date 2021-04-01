using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectablesData2D : MonoBehaviour
{
    // Start is called before the first frame update


    public int Value;
    public int Type;

    public Text Text;
    public Color Colors;

    public AudioClip Clip;

    Vector3 Starts;

    int LastLayer;

    bool clicked = false;

    void Start()
    {

        if (Starts == Vector3.zero)
            Starts = gameObject.transform.position;

        Text.text = "+" + Value;
        Text.color = Colors;
        if (Type == 4)
        {
            FindObjectOfType<GameController2D>().FuelObj.Add(gameObject);
            if (Value == 100)
            {
                Text.text = "Full Tank";
            }
            else if (Value == 50)
            {
                Text.text = "Half Tank";
            }
            else
            {
                Text.text = "Quarter Tank";
            }
        }
    }

    // Update is called once per frame

    IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && clicked == false )
        {
            if (FindObjectOfType<GameController2D>().PlayMode == 1)
            {
                if (LastLayer != other.gameObject.layer)
                {
                    if (FindObjectOfType<GameController2D>().SFX == true)
                        gameObject.GetComponent<AudioSource>().PlayOneShot(Clip, 1f);

                    Text.gameObject.SetActive(true);
                    GameController2D Controller = FindObjectOfType<GameController2D>();
                    if (other.gameObject.layer == 8)
                    {
                        Controller.AddAICollectables(Type, Value);
                    }
                    else
                    {
                        Controller.AddCollectables(Type, Value);
                    }

                    float speed = 1.5f / 0.5f;
                    float t = 0;
                    while (t < 0.5f)
                    {
                        gameObject.transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
                        t += Time.deltaTime;
                        yield return new WaitForSeconds(Time.deltaTime);
                    }

                    LastLayer = other.gameObject.layer;
                    Text.gameObject.SetActive(false);
                    gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    yield return new WaitForSeconds(0.5f);
                    gameObject.transform.position = Starts;
                    gameObject.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
            else
            {
                clicked = true;

                if (FindObjectOfType<GameController2D>().SFX == true)
                    gameObject.GetComponent<AudioSource>().PlayOneShot(Clip, 1f);

                Text.gameObject.SetActive(true);
                GameController2D Controller = FindObjectOfType<GameController2D>();
                Controller.AddCollectables(Type, Value);
                float speed = 1.5f / 0.5f;
                float t = 0;
                while (t < 0.5f)
                {
                    gameObject.transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
                    t += Time.deltaTime;
                    yield return new WaitForSeconds(Time.deltaTime);
                }
                Destroy(gameObject);
            }
        }
    }
}
