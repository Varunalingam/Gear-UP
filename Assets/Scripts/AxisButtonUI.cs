using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AxisButtonUI : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IDragHandler
{

    public RectTransform JoyButton;

    public float value;
    bool holdtrue;

    bool pointerinbounds;

    public Text Displaytext;
    public List<string> ActionValue;

    public void ActionChanger(int value)
    {
        Displaytext.text = ActionValue[value];
    }
    

    Vector2 Defaultpos;

    // Start is called before the first frame update
    void Start()
    {
        Defaultpos = JoyButton.position;
    }


    bool BackgroundChanger = true;
    void OnEnable()
    {
        BackgroundChanger = true;
        Color Bg = gameObject.GetComponent<Image>().color;
        Bg.a = 0.5f;
        gameObject.GetComponent<Image>().color = Bg;
    }
    // Update is called once per frame
    void Update()
    {
        if (BackgroundChanger)
        {
            Color Bg = gameObject.GetComponent<Image>().color;
            Bg.a -= Time.deltaTime;
            gameObject.GetComponent<Image>().color = Bg;
            if (Bg.a - Time.deltaTime < 0)
            {
                Bg.a = 0f;
                gameObject.GetComponent<Image>().color = Bg;
                BackgroundChanger = false;
            }
        }

        if (holdtrue)
        {
            if (value < 1)
                value += Time.deltaTime;
            else
                value = 1;

            if (value > 0.6)
            {
                Color A = JoyButton.GetComponent<Image>().color;
                A.a = 0.7f;
                JoyButton.GetComponent<Image>().color = A;
            }
            else if (value > 0.3)
            {
                Color A = JoyButton.GetComponent<Image>().color;
                A.a = 1 - (value - 0.3f);
                JoyButton.GetComponent<Image>().color = A;
            }
            else
            {
                Color A = JoyButton.GetComponent<Image>().color;
                A.a = 1;
                JoyButton.GetComponent<Image>().color = A;
            }
                
        }
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        pointerinbounds = true;
        Vector2 localposition = pointerEventData.pressPosition;
        JoyButton.position = localposition ;
        holdtrue = true;
    }

    public void OnDrag(PointerEventData pointerEventData)
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(gameObject.GetComponent<RectTransform>(), pointerEventData.position))
            pointerinbounds = false;

        if (pointerinbounds)
        {
            Vector2 localposition = pointerEventData.position;
            JoyButton.position = localposition;
            holdtrue = true;
        }
        else
        {
            pointerEventData.pointerDrag = null;
            pointerEventData.dragging = false;
            OnPointerUp(pointerEventData);
        }
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        holdtrue = false;
        JoyButton.position = Defaultpos;
        value = 0;
        Color A = JoyButton.GetComponent<Image>().color;
        A.a = 1;
        JoyButton.GetComponent<Image>().color = A;
    }
}
