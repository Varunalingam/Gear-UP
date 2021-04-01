using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TachometerUI : MonoBehaviour
{

    public float value;
    public RectTransform Needle;
    public float maxrotationz, minrotationz;
    public int Speedvalue;
    public Text Speed;
    public int GearValue;
    public Text Gear;
    Quaternion InitRot;

    public float Fuel;
    public Image Fuelfill;
    public Color Fuelnormal, FuelLow;

    public bool ABS, Light, ECO, Parking, Damaged;

    public GameObject ABSo, Lighto, ECOo, Parkingo, Damagedo;

    public Image Healthfill;
    public float Health;

    public Text Distance;
    public Text Coins;
    public Text Diamonds;

    public float Distancef;

    public int Distancefuel;

    public Text FDtext;

    // Start is called before the first frame update
    void Start()
    {
        InitRot = Needle.rotation;
        Fuelnormal = Fuelfill.color;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Diamonds.text = "" + FindObjectOfType<GameController2D>().CurrentDiamonds;
        Coins.text = "" + FindObjectOfType<GameController2D>().CurrentCoins;

        FDtext.text = "Next Tank in " + Distancefuel + " M";

        Distance.text = "" + Mathf.RoundToInt(Distancef * 100) + "%";
        if (Mathf.RoundToInt(Distancef * 100) > 100)
        {
            Distance.text = "100%";
        }
        else if (Mathf.RoundToInt(Distancef * 100) < 0)
        {
            Distance.text = "0%";
        }



        if (value > 1)
        {
            value = 1;
        }
        if (value < 0)
        {
            value = 0;
        }

        float rotation = ((maxrotationz - minrotationz ) * value ) + minrotationz;
        Vector3 RotationC = InitRot.eulerAngles;
        RotationC.z = rotation;
        Needle.rotation = Quaternion.Euler(RotationC);
        Speed.text = "" + Speedvalue;

        if (GearValue < 0)
        {
            Gear.text = "R";
        }
        else if (GearValue == 0)
        {
            Gear.text = "N";
        }
        else
        {
            Gear.text = "D" + GearValue;
        }

        ABSo.SetActive(ABS);
        ECOo.SetActive(ECO);
        Lighto.SetActive(Light);
        Damagedo.SetActive(Damaged);
        Parkingo.SetActive(Parking);


        Fuelfill.fillAmount = Fuel;

        if (Fuel < 0.3f)
        {
            Fuelfill.color = FuelLow;
        }
        else
        {
            Fuelfill.color = Fuelnormal;
        }

        Color Healthf = Healthfill.color;
        Healthf.a = (1f - Health) / 3;
        Healthfill.color = Healthf;
    }
}
