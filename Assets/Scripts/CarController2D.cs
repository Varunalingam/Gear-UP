using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarController2D : MonoBehaviour
{

    public float MaxAccelarationConst = 10;
    public float MaxMotorSpeed = 1000;
    public float BrakeConst = 10;

    public WheelJoint2D FrontWheel, BackWheel;
    JointMotor2D FrontWheelMotor, BackWheelMotor;

    public float MaxTorque = 10000;
    public float Maxgear = 5;
    
    
    public bool FrontWheelDrive, RearWheelDrive;

    float AccelarationConst;
    public float gear;
    bool zeroed;
    public float a = 0;
    float AllowedMotorSpeed;

    bool checkground = true;

    public float Dampaning = 0.7f;
    public float Frequency = 10;
    public bool Automatic = false;
    public bool ExtraHandeling = false;

    public float HandelingConst = 2;
    public float HandelingCoeffiecient = 0.9f;

    public float MaxFuel = 1000f;
    public float Fuel;
    public float Health = 1000f;
    public float MaxHealth = 1000f;

    public float RPM;


    public bool HandBreak;

    public List<AudioClip> CarSounds;
    /*Car Sounds
     * 0 - Engine Start
     * 1 - Engine Noise
     * 2 - Gear Up
     * 3 - Gear Rev
     * 4 - Horn
         */

    public AudioSource CarPlayer;
    public AudioSource GearPlayer;

    public bool Accelarating;

    public bool ECO;
    public bool Damaged;
    public bool Lights;

    public bool TouchInput;

    public float FuelEfficiency;

    public bool AI;

    // Start is called before the first frame update
    void Start()
    {
        if (FindObjectOfType<GameController2D>().AIPlayer == gameObject)
            AI = true;

        Fuel = MaxFuel;
        Health = MaxHealth;
        
        CarPlayer = gameObject.GetComponent<AudioSource>();

        FrontWheelMotor = FrontWheel.motor;
        BackWheelMotor = BackWheel.motor;
        BackWheelMotor.maxMotorTorque = MaxTorque;
        FrontWheelMotor.maxMotorTorque = MaxTorque;
        JointSuspension2D Suspension = FrontWheel.suspension;
        Suspension.dampingRatio = Dampaning;
        Suspension.frequency = Frequency;
        FrontWheel.suspension = Suspension;

        Suspension = BackWheel.suspension;
        Suspension.dampingRatio = Dampaning;
        Suspension.frequency = Frequency;
        BackWheel.suspension = Suspension;
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bg")
        {
            checkground = true;
        }
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Water")
        {
            Health -= 2f;
        }

        if (other.tag == "Bg" && Vector3.Dot(transform.up, Vector3.down) > 0)
        {
            Health -= 2f;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Bg")
        {
            checkground = false;
        }
    }


    bool jumpr = true;
    bool shiftupr = true;
    bool shiftdowr = true;

    private void Update()
    {
        if (FindObjectOfType<GameController2D>().SFX == true)
        {
            GearPlayer.volume = 1f;
        }
        else
        {
            GearPlayer.volume = 0f;
        }

        if (!FindObjectOfType<GameController2D>().paused)
        {
            if (!TouchInput && !AI)
            {
                a = Input.GetAxis("Vertical");
                if (Input.GetAxis("Shift Up") > 0 && shiftupr)
                {
                    shiftupr = false;
                    if (gear < Maxgear)
                    {
                        gear += 1;
                        GearPlayer.PlayOneShot(CarSounds[2]);
                    }
                }
                else if (Input.GetAxis("Shift Down") > 0 && shiftdowr)
                {
                    shiftdowr = false;
                    if (gear == 0)
                    {
                        GearPlayer.PlayOneShot(CarSounds[3]);
                    }
                    if (gear > -1)
                    {
                        gear -= 1;
                        GearPlayer.PlayOneShot(CarSounds[2]);
                    }
                }
                else if (Input.GetAxis("Jump") > 0 && jumpr)
                {
                    if (HandBreak)
                        HandBreak = false;
                    else
                        HandBreak = true;

                    jumpr = false;
                }
                else if (Input.GetAxis("Horn") > 0)
                {
                    Horn();
                    FindObjectOfType<GameController2D>().CarHorned = true;
                }

                if (Input.GetAxis("Horn") == 0)
                {
                    FindObjectOfType<GameController2D>().CarHorned = false;
                }

                if (Input.GetAxis("Jump") == 0)
                {
                    jumpr = true;
                }

                if (Input.GetAxis("Shift Up") == 0)
                {
                    shiftupr = true;
                }

                if (Input.GetAxis("Shift Down") == 0)
                {
                    shiftdowr = true;
                }
            }

            if (AI)
                AIControls();

            if (carstartup && !waittillend)
                SoundChanger();

            if (a != 0)
            {
                Accelarating = true;
                if (!carstartup && !waittillend)
                {
                    SoundChanger();
                }
                BackWheel.connectedBody.freezeRotation = false;
                if (gameObject.GetComponent<Rigidbody2D>().velocity.magnitude < HandelingConst && zeroed)
                {
                    BackWheelMotor.motorSpeed = 0;
                    FrontWheelMotor.motorSpeed = 0;
                    zeroed = false;
                }
                else if (zeroed && gear == 0)
                {
                    if (ExtraHandeling && checkground)
                    {
                        gameObject.GetComponent<Rigidbody2D>().velocity = gameObject.GetComponent<Rigidbody2D>().velocity * HandelingCoeffiecient;

                        if (FrontWheel.motor.motorSpeed / a > 0)
                        {
                            FrontWheelMotor.motorSpeed -= (BrakeConst * a);
                            if (MaxMotorSpeed * (gear - 1) / Maxgear > -1 * FrontWheelMotor.motorSpeed && gear != -1)
                                gear -= 1;
                        }
                        if (BackWheel.motor.motorSpeed / a > 0)
                        {
                            BackWheelMotor.motorSpeed -= (BrakeConst * a);
                            if (MaxMotorSpeed * (gear - 1) / Maxgear > -1 * BackWheelMotor.motorSpeed && gear != -1)
                                gear -= 1;
                        }
                        FrontWheel.motor = FrontWheelMotor;
                        BackWheel.motor = BackWheelMotor;

                        FrontWheel.useMotor = false;
                        BackWheel.useMotor = false;

                    }
                    else
                    {
                        FrontWheel.useMotor = false;
                        FrontWheel.connectedBody.freezeRotation = true;
                        BackWheel.useMotor = false;
                        BackWheel.connectedBody.freezeRotation = true;
                    }
                }
                else
                {
                    BackWheel.useMotor = RearWheelDrive;
                    FrontWheel.useMotor = FrontWheelDrive;

                    FrontWheel.connectedBody.freezeRotation = false;
                    BackWheel.connectedBody.freezeRotation = false;

                    if ((FrontWheel.motor.motorSpeed / a > 0) || (BackWheel.motor.motorSpeed / a > 0) || (Fuel < 0))
                    {
                        if (ExtraHandeling && checkground)
                        {
                            gameObject.GetComponent<Rigidbody2D>().velocity = gameObject.GetComponent<Rigidbody2D>().velocity * HandelingCoeffiecient;
                        }

                        if (FrontWheel.motor.motorSpeed / a > 0)
                        {
                            FrontWheelMotor.motorSpeed -= (BrakeConst * a);
                            if (MaxMotorSpeed * (gear - 1) / Maxgear > -1 * FrontWheelMotor.motorSpeed && gear != -1)
                                gear -= 1;
                        }
                        if (BackWheel.motor.motorSpeed / a > 0)
                        {
                            BackWheelMotor.motorSpeed -= (BrakeConst * a);
                            if (MaxMotorSpeed * (gear - 1) / Maxgear > -1 * BackWheelMotor.motorSpeed && gear != -1)
                                gear -= 1;
                        }

                        FrontWheel.motor = FrontWheelMotor;
                        BackWheel.motor = BackWheelMotor;

                        if (ExtraHandeling || (Fuel - (AccelarationConst / MaxAccelarationConst) < 0))
                        {
                            FrontWheel.useMotor = false;
                            BackWheel.useMotor = false;
                        }
                        zeroed = false;

                        if ((Fuel - (AccelarationConst / MaxAccelarationConst) < 0))
                        {
                            Accelarating = false;
                            BackWheel.connectedBody.freezeRotation = false;

                            if (checkground)
                                zeroed = true;

                            if (Mathf.Abs(BackWheelMotor.motorSpeed) > Mathf.Abs(BrakeConst))
                            {
                                BackWheelMotor.motorSpeed -= AccelarationConst * Mathf.Abs(BackWheelMotor.motorSpeed) / BackWheelMotor.motorSpeed;
                                BackWheel.motor = BackWheelMotor;
                                if (MaxMotorSpeed * (gear - 1) / Maxgear > -1 * BackWheelMotor.motorSpeed && gear != -1)
                                    gear -= 1;
                            }
                            else
                            {
                                gear = 0;
                            }

                            if (Mathf.Abs(FrontWheelMotor.motorSpeed) > Mathf.Abs(BrakeConst))
                            {
                                FrontWheelMotor.motorSpeed -= AccelarationConst * Mathf.Abs(FrontWheelMotor.motorSpeed) / FrontWheelMotor.motorSpeed;
                                FrontWheel.motor = FrontWheelMotor;
                                if (MaxMotorSpeed * (gear - 1) / Maxgear > -1 * FrontWheelMotor.motorSpeed && gear != -1)
                                    gear -= 1;
                            }
                            else
                            {
                                gear = 0;
                            }

                            BackWheel.useMotor = false;
                            FrontWheel.useMotor = false;

                            ECO = false;
                        }
                    }
                    else
                    {
                        if (gear == 0 || gear / a < 0)
                        {
                            if (a > 0)
                            {
                                gear = 1;
                                GearPlayer.PlayOneShot(CarSounds[2]);
                            }
                            else
                            {
                                gear = -1;
                                GearPlayer.PlayOneShot(CarSounds[3]);
                            }
                        }

                        if (gear >= 0)
                            AllowedMotorSpeed = (gear / Maxgear) * MaxMotorSpeed;
                        else
                            AllowedMotorSpeed = MaxMotorSpeed / 2;


                        if (-1 * BackWheelMotor.motorSpeed > 0.8 * AllowedMotorSpeed && gear < Maxgear && gear != -1)
                        {
                            if (Automatic)
                                gear += 1;
                            else
                                Health -= Mathf.Abs(RPM - Mathf.Abs(gear / Maxgear));

                        }
                        AllowedMotorSpeed = (gear / Maxgear) * MaxMotorSpeed;
                        if (-1 * FrontWheelMotor.motorSpeed > 0.8 * AllowedMotorSpeed && gear < Maxgear && Automatic && gear != -1)
                        {
                            if (Automatic)
                                gear += 1;
                            else
                                Health -= Mathf.Abs(RPM - Mathf.Abs(gear / Maxgear));
                        }

                        if (gear >= 0)
                            AllowedMotorSpeed = (gear / Maxgear) * MaxMotorSpeed;
                        else
                            AllowedMotorSpeed = MaxMotorSpeed / 2;

                        if (gear > Maxgear)
                            AccelarationConst = Mathf.Abs(((Maxgear - (gear - 1)) / Maxgear) * MaxAccelarationConst);
                        else
                            AccelarationConst = MaxAccelarationConst;


                        if (Mathf.Abs(BackWheelMotor.motorSpeed - (AccelarationConst * a)) < Mathf.Abs(AllowedMotorSpeed))
                            BackWheelMotor.motorSpeed -= (AccelarationConst * a);
                        else
                            BackWheelMotor.motorSpeed = Mathf.Abs(AllowedMotorSpeed) * (a / Mathf.Abs(a)) * -1;


                        if (Mathf.Abs(FrontWheelMotor.motorSpeed - (AccelarationConst * a)) < Mathf.Abs(AllowedMotorSpeed))
                            FrontWheelMotor.motorSpeed -= (AccelarationConst * a);
                        else
                            FrontWheelMotor.motorSpeed = Mathf.Abs(AllowedMotorSpeed) * (a / Mathf.Abs(a)) * -1;

                        
                        Fuel -= Mathf.Abs(RPM - Mathf.Abs(gear / Maxgear));
                        if (Mathf.Abs(RPM - Mathf.Abs(gear / Maxgear)) < 0.3f)
                        {
                            ECO = true;
                        }
                        else
                        {
                            ECO = false;
                            Health -= Mathf.Abs(RPM - Mathf.Abs(gear / Maxgear));
                        }

                        if (RPM > 0.9f)
                        {
                            ECO = false;
                            Health -= FuelEfficiency *Time.deltaTime*0.5f;
                            Fuel -= FuelEfficiency * Time.deltaTime * 2.5f;
                        }

                        BackWheel.motor = BackWheelMotor;
                        FrontWheel.motor = FrontWheelMotor;
                        zeroed = false;
                    }
                }
            }
            else
            {
                Accelarating = false;
                BackWheel.connectedBody.freezeRotation = false;

                if (checkground)
                    zeroed = true;

                if (Mathf.Abs(BackWheelMotor.motorSpeed) > Mathf.Abs(BrakeConst))
                {
                    BackWheelMotor.motorSpeed -= AccelarationConst * Mathf.Abs(BackWheelMotor.motorSpeed) / BackWheelMotor.motorSpeed;
                    BackWheel.motor = BackWheelMotor;
                    if (MaxMotorSpeed * (gear - 1) / Maxgear > -1 * BackWheelMotor.motorSpeed && gear != -1)
                        gear -= 1;
                }
                else
                {
                    gear = 0;
                }

                if (Mathf.Abs(FrontWheelMotor.motorSpeed) > Mathf.Abs(BrakeConst))
                {
                    FrontWheelMotor.motorSpeed -= AccelarationConst * Mathf.Abs(FrontWheelMotor.motorSpeed) / FrontWheelMotor.motorSpeed;
                    FrontWheel.motor = FrontWheelMotor;
                    if (MaxMotorSpeed * (gear - 1) / Maxgear > -1 * FrontWheelMotor.motorSpeed && gear != -1)
                        gear -= 1;
                }
                else
                {
                    gear = 0;
                }

                if (carstartup)
                {
                    Fuel -= FuelEfficiency * Time.deltaTime * 2.5f;
                }


                BackWheel.useMotor = false;
                FrontWheel.useMotor = false;

                ECO = false;
            }

            if (HandBreak)
            {
                BackWheel.connectedBody.freezeRotation = true;
                BackWheel.useMotor = false;
                FrontWheel.useMotor = false;
            }
        }
        else
        {
            CarPlayer.volume = 0f;
        }
    }
    bool carstartup = false;
    bool waittillend = false;

    public void AIControls()
    {
        Automatic = true;
        a = Random.Range(0f, 1f);
       
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "enemyz" && AI)
        {
            Horn();
            FindObjectOfType<GameController2D>().AICarHorn = true;
        }
        else
        {
            FindObjectOfType<GameController2D>().AICarHorn = false;
        }
    }

    public void SoundChanger()
    {
        if (FindObjectOfType<GameController2D>().SFX == true)
        {
            CarPlayer.volume = 1f;
        }
        else
        {
            CarPlayer.volume = 0f;
        }


        if (waittillend && !CarPlayer.isPlaying)
        {
            CarPlayer.clip = CarSounds[1];
            CarPlayer.loop = true;
            CarPlayer.Play();
            if (AllowedMotorSpeed != 0 && CarPlayer.clip == CarSounds[1])
                CarPlayer.pitch = Mathf.Abs(BackWheelMotor.motorSpeed / AllowedMotorSpeed);
            waittillend = false;
        }

        if (!carstartup)
        {
            CarPlayer.clip = null;
            CarPlayer.PlayOneShot(CarSounds[0]);
            CarPlayer.clip = null;
            carstartup = true;
            waittillend = true;
        }
        else
        {
            if (AllowedMotorSpeed != 0 && CarPlayer.clip == CarSounds[1] && Mathf.Abs(BackWheelMotor.motorSpeed / AllowedMotorSpeed) > 0.2f)
                CarPlayer.pitch = Mathf.Abs(BackWheelMotor.motorSpeed / AllowedMotorSpeed);
            else
                CarPlayer.pitch = 0.2f;
        }
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        if (waittillend)
        {
            if (!CarPlayer.isPlaying)
            {
                SoundChanger();
            }
        }

        if (AllowedMotorSpeed != 0)
            RPM = Mathf.Abs(BackWheelMotor.motorSpeed / AllowedMotorSpeed);
        else
            RPM = 0;

    }


    public void Horn()
    {
        GearPlayer.clip = CarSounds[4];
        GearPlayer.Play();
    }
}
