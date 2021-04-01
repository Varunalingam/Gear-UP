using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CarManager2D : MonoBehaviour
{
    public int Carid;
    public Light2D HeadLights;

    public float MaxAcc, MinAcc;
    public float MaxSpeed, MinSpeed;
    public float MaxBrakeConst, MinBrakeConst;
    public float MaxTorque, MinTorque;
    public int MaxGear, MinGear;
    public float MaxDampaningRatio, MinDampeningRatio;
    public float MaxFrequency, MinFrequency;
    public float MaxHandelingC, MinHandelingC;
    public float MaxHandelingCoef, MinHandelingCoef;
    public float MaxFuel, MinFuel;
    public float MaxHealth, MinHealth;
    public float MaxEfficiency, MinEfficiency;

    private void Awake()
    {
        

        CarController2D Controller = GetComponent<CarController2D>();

        int Engine = PlayerPrefs.GetInt("" + Carid + "Engine", 4);
        int Brake = PlayerPrefs.GetInt("" + Carid + "Brake", 4);
        int Gear = PlayerPrefs.GetInt("" + Carid + "Gear", 4);
        int Suspension = PlayerPrefs.GetInt("" + Carid + "Suspension", 4);


        Controller.MaxAccelarationConst = (MaxAcc - MinAcc) / Engine  + MinAcc;
        Controller.MaxMotorSpeed = (MaxSpeed - MinSpeed) * 2 / (Engine + Gear) + MinSpeed;
        Controller.BrakeConst = (MaxBrakeConst - MinBrakeConst) / Brake + MinBrakeConst;
        Controller.MaxTorque = (MaxTorque - MinTorque) / Gear + MinTorque;
        Controller.Dampaning = (MaxDampaningRatio - MinDampeningRatio) / Suspension + MinDampeningRatio;
        Controller.Frequency = (MaxFrequency - MinFrequency) / Suspension + MinFrequency;
        Controller.MaxHealth = (MaxHealth - MinHealth) * 2 / (Engine + Suspension) + MinHealth;
        Controller.MaxFuel = (MaxFuel - MinFuel) * 4 / (Engine + Suspension + Brake + Gear) + MinFuel;
        Controller.HandelingConst = (MaxHandelingC - MinHandelingC) / Brake + MinHandelingC;
        Controller.HandelingCoeffiecient = (MaxHandelingCoef - MinHandelingCoef) / (5 - Brake) + MinHandelingCoef;

        Controller.FuelEfficiency = (MaxEfficiency - MinEfficiency) * 2 / (Engine + Gear);

        if (Engine < 3)
        {
            Controller.FrontWheelDrive = Controller.RearWheelDrive = true;
        }
        else if (Engine == 3)
        {
            Controller.RearWheelDrive = true;
            Controller.FrontWheelDrive = false;
        }
        else
        {
            Controller.FrontWheelDrive = true;
            Controller.RearWheelDrive = false;
        }

        if (Carid > 2)
        {
            Controller.Automatic = true;
        }
        else
        {
            Controller.Automatic = false;
        }

        if (Gear < (MaxGear - MinGear + 1))
        {
            Controller.Maxgear = (MaxGear - Gear + 1);
        }
        else
        {
            Controller.Maxgear = MinGear;
        }

        if (Brake < 3)
        {
            Controller.ExtraHandeling = true;
            
        }


        
    }
}
