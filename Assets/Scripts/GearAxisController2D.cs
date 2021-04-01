using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GearAxisController2D : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IDragHandler
{
    // Start is called before the first frame update

    float value;
    Vector3 Defaultpos;
    public RectTransform Shiftup, ShiftDown;
    public GameController2D Controller;
    public RectTransform Knob;

    PointerEventData GlobalData;

    void Start()
    {
        Defaultpos = Knob.position;


    }

    float timer;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (holdtrue)
        {
            timer += Time.fixedDeltaTime;
            if (timer > 1)
            {
                timer = 0;
                holdtrue = false;
                GlobalData.pointerDrag = null;
                GlobalData.dragging = false;
                OnPointerUp(GlobalData);
            }
        }
    }

    bool pointerinbounds;
    bool holdtrue;


    public void OnPointerDown(PointerEventData pointerEventData)
    {
        pointerinbounds = true;
        GlobalData = pointerEventData;
        if (RectTransformUtility.RectangleContainsScreenPoint(Shiftup, pointerEventData.position))
        {
            value = 1;
            holdtrue = true;
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(ShiftDown, pointerEventData.position))
        {
            value = -1;
            holdtrue = true;
        }

        Vector2 localposition = pointerEventData.position;
        localposition.x = Defaultpos.x;
        Knob.position = localposition;
    }

    public void OnDrag(PointerEventData pointerEventData)
    {
        GlobalData = pointerEventData;
        if (!RectTransformUtility.RectangleContainsScreenPoint(gameObject.GetComponent<RectTransform>(), pointerEventData.position))
            pointerinbounds = false;

        if (RectTransformUtility.RectangleContainsScreenPoint(Shiftup, pointerEventData.position))
        {
            value = 1;
            holdtrue = true;
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(ShiftDown, pointerEventData.position))
        {
            value = -1;
            holdtrue = true;
        }

        if (pointerinbounds && !(holdtrue == false && value != 0))
        {
            Vector2 localposition = pointerEventData.position;
            localposition.x = Defaultpos.x;
            Knob.position = localposition;
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
        pointerinbounds = false;
        Knob.position = Defaultpos;
        if (value != 0)
        {
            Controller.GearInput(value);
        }
        value = 0;
    }

    

}
