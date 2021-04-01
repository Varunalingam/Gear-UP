using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;


public class GameController2D : MonoBehaviour
{
    // Start is called before the first frame update

    public int PlayMode = 1;

    public Transform AIStartTransform;

    public GameObject AIPlayer;

    public bool paused = false;

    public Transform StartTransform;
    public Transform EndTransform;

    public Transform EndLE;

    public GameObject Player;
    public bool TouchInput = false;


    //UI
    public TachometerUI Tachometer;

    public AxisButtonUI Forward, Reverse;

    public int CurrentCoins;
    public int CurrentDiamonds;


    public bool CarHorned = false;


    public float time;
    public float Totaltime;

    //Time Parameters
    public Color Day;
    public Color Night;

    public List<GameObject> Clouds;

    List<GameObject> GenClouds = new List<GameObject>();

    public int GenLimit;

    public SpriteRenderer Stars;

    public Light2D GL;

    public Color Sunlight, MoonLight;

    public bool SFX = true;

    public float MusicVolume = 1f;

    [HideInInspector]
    public List<GameObject> FuelObj;

    public float TimeTaken;

    public bool LevelEnded;

    public List<GameObject> Cars;

    public bool AICarHorn;

    public int AICurrentCoins, AICurrentDiamonds;

    public List<GameObject> AICars;

    public Text LevelName, PlayModeName;

    public List<GameObject> Birds;

    public float CrowGap;
    public float Crowspeed;

    void Awake()
    {

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Application.targetFrameRate = 30;
        }
        SFX = PlayerPrefs.GetInt("SFX", 1) > 0 ? true:false;
        TouchInput = PlayerPrefs.GetInt("TI", 0) > 0 ?true:false;
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        CurrentCoins = PlayerPrefs.GetInt("Coins", 10000);
        CurrentDiamonds = PlayerPrefs.GetInt("Diamonds", 1000);

        PlayMode = PlayerPrefs.GetInt("PlayMode", 0);
        gameObject.GetComponent<SpiriteGenerator2D>().Level = PlayerPrefs.GetInt("CurrentLevel", 0);
    }

    void Start()
    {
        Vector3 Run = StartTransform.position;
        Run.y += 1;
        Run.z = gameObject.transform.position.z;
        gameObject.transform.position = Run;

        gameObject.GetComponent<SpiriteGenerator2D>().LevelGenerate();

        Player = Instantiate(Cars[PlayerPrefs.GetInt("CurrentCar",0)]);
        Player.transform.position = StartTransform.position;

        if (PlayMode == 1)
        {
            AIPlayer = Instantiate(AICars[GetComponent<SpiriteGenerator2D>().AILevelData[GetComponent<SpiriteGenerator2D>().Level].AICar]);
            AIPlayer.transform.position = AIStartTransform.position;
            LevelName.text = GetComponent<SpiriteGenerator2D>().AILevelData[GetComponent<SpiriteGenerator2D>().Level].Name;
            PlayModeName.text = "Race";
        }
        else
        {
            LevelName.text = GetComponent<SpiriteGenerator2D>().LevelData[GetComponent<SpiriteGenerator2D>().Level].Name;
            PlayModeName.text = "Story";
        }

        if (GetComponent<SpiriteGenerator2D>().Level == GetComponent<SpiriteGenerator2D>().LevelData.Count - 1 && PlayMode != 1)
            EndTransform = EndLE;

        for (int i = 0; i < GenLimit; i++)
        {
            Vector3 pos = gameObject.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(Random.Range((i)*1f/GenLimit,(i + 1)*1f/GenLimit), Random.Range(0.8f,0.95f), 1));
            pos.z = Player.transform.position.z;
            GenClouds.Add(Instantiate(Clouds[Random.Range(0, Clouds.Count)], pos, Quaternion.identity,gameObject.transform));
        }

        Vector3 Scale = new Vector3(1, 1, 1);

        float wSH = GetComponent<Camera>().orthographicSize * 2f;
        float wSW = wSH / Screen.height * Screen.width;

        Scale.x = (Screen.width > Screen.height ? wSW / Stars.bounds.size.x : wSH / Stars.bounds.size.y);
        Scale.y = Scale.x;

        Stars.GetComponent<Transform>().localScale = Scale;

        orhographicsize = gameObject.GetComponent<Camera>().orthographicSize;

    }

    public void Update()
    {




        if (TouchInput)
        {
            Player.GetComponent<CarController2D>().a = Forward.value - Reverse.value;

            if (Player.GetComponent<CarController2D>().gear > 0)
            {
                Forward.ActionChanger(0);
                Reverse.ActionChanger(1);
            }
            else if (Player.GetComponent<CarController2D>().gear < 0)
            {
                Forward.ActionChanger(1);
                Reverse.ActionChanger(0);
            }
            else
            {
                Forward.ActionChanger(0);
                Reverse.ActionChanger(0);
            }
        }
           
    }
    // Update is called once per frame

    public float LevelEndTimer = 0;
    public float orhographicsize;

    public bool LevelCompleted = false;

    public bool AIWin;

    void FixedUpdate()
    {

        if (EndTransform.GetComponent<AIEnemyTriggerReceiver2D>().Trigger)
        {
            LevelCompleted = true;
            if (EndTransform.GetComponent<AIEnemyTriggerReceiver2D>().Triggered.layer == 8)
                AIWin = true;
        }

        if (Player.GetComponent<CarController2D>().Health < 0 && !LevelEnded)
        {
            LevelEndTimer += Time.fixedDeltaTime;
            if (LevelEndTimer > 1)
            {
                gameObject.GetComponent<Camera>().orthographicSize -= orhographicsize / 4 * Time.fixedDeltaTime;
                if (LevelEndTimer > 2)
                    LevelEnded = true;
            }
        }
        else if (Player.GetComponent<CarController2D>().Fuel < 0 && !LevelEnded)
        {
            LevelEndTimer += Time.fixedDeltaTime;
            if (LevelEndTimer > 1)
            {
                gameObject.GetComponent<Camera>().orthographicSize -= orhographicsize / 4 * Time.fixedDeltaTime;
                if (LevelEndTimer > 2)
                    LevelEnded = true;
            }
        }
        else if (LevelCompleted && !LevelEnded)
        {
            LevelEndTimer += Time.fixedDeltaTime;
            gameObject.GetComponent<Camera>().orthographicSize -= orhographicsize / 4 * Time.fixedDeltaTime;
            if (LevelEndTimer > 2)
                LevelEnded = true;
        }
        else if (!LevelEnded)
        {
            gameObject.GetComponent<Camera>().orthographicSize = orhographicsize;
            LevelEndTimer = 0;
        }

        TimeTaken += Time.fixedDeltaTime;
        DayNight();

        Tachometer.Distancef = (Player.transform.position.x - StartTransform.position.x) / (EndTransform.position.x - StartTransform.position.x);
        Player.GetComponent<CarController2D>().TouchInput = TouchInput;

        if (Player != null)
        {
            Vector3 Run = Player.transform.position;
            Run.y += 1;
            Run.z = gameObject.transform.position.z;
            gameObject.transform.position = Run;
        }

        Tachometer.value = Player.GetComponent<CarController2D>().RPM;
        Tachometer.GearValue = Mathf.RoundToInt(Player.GetComponent<CarController2D>().gear);

        Tachometer.Parking = Player.GetComponent<CarController2D>().HandBreak;
        Tachometer.ABS = Player.GetComponent<CarController2D>().ExtraHandeling;
        Tachometer.ECO = Player.GetComponent<CarController2D>().ECO;
        Tachometer.Light = Player.GetComponent<CarController2D>().Lights;
        Tachometer.Damaged = Player.GetComponent<CarController2D>().Damaged;
        if (Player.GetComponent<CarController2D>().Fuel > 0)
            Tachometer.Fuel = Player.GetComponent<CarController2D>().Fuel / Player.GetComponent<CarController2D>().MaxFuel;
        else
            Tachometer.Fuel = 0;

        Tachometer.Health = Player.GetComponent<CarController2D>().Health / Player.GetComponent<CarController2D>().MaxHealth;


        if (Player.GetComponent<CarController2D>().Accelarating)
            Tachometer.Speedvalue = Mathf.RoundToInt(Player.GetComponent<Rigidbody2D>().velocity.magnitude * 10);
        else
            Tachometer.Speedvalue = 0;


        if (CarHorned)
            Player.GetComponent<CarController2D>().Horn();


        LeastDistance();
    }

    public void LeastDistance()
    {
        float dist = 0;
        for (int i = 0; i < FuelObj.Count;)
        {
            if (FuelObj[i] == null)
            {
                FuelObj.RemoveAt(i);
            }
            else
            {
                if (FuelObj[i].activeInHierarchy)
                    dist = (dist < Mathf.Abs(FuelObj[i].transform.position.x - Player.transform.position.x) && dist != 0) ? dist : Mathf.Abs(FuelObj[i].transform.position.x - Player.transform.position.x);

                i++;
            }
        }
        Tachometer.Distancefuel = Mathf.RoundToInt(dist);
    }

    float CrowTimer = 0;

    GameObject GenCrow;
    public void DayNight()
    {
        time += Time.fixedDeltaTime;
        if (CrowTimer >= 0)
            CrowTimer += Time.deltaTime;

        if (CrowTimer > CrowGap)
        {
            CrowTimer = -1;
            GenCrow = Instantiate(Birds[Random.Range(0, Birds.Count)],gameObject.transform);
            Vector3 cpos = gameObject.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(-0.05f, Random.Range(0.75f, 0.9f), 1));
            cpos.z = Player.transform.position.z;
            GenCrow.transform.position = cpos;
        }

        if (CrowTimer == -1)
        {
            GenCrow.transform.Translate(new Vector3(Crowspeed * Time.deltaTime, 0));
            GenCrow.GetComponent<AudioSource>().volume = SFX == true ? 1f : 0f;
            if (GetComponent<Camera>().WorldToViewportPoint(GenCrow.transform.position).x > 1.05f)
            {
                Destroy(GenCrow);
                CrowTimer = 0;
            }
        }

        if (time < Totaltime / 4)
        {
            gameObject.GetComponent<Camera>().backgroundColor = Color.Lerp(Day, Night, 4 * time / Totaltime);
            GL.color = Color.Lerp(Sunlight, MoonLight, 4 * time / Totaltime);
            Color A = Stars.color;
            A.a = 4 * time / Totaltime;
            A.a = Random.Range(0, 100) % 3 == 0 ? 1f * A.a : 0.5f * A.a;
            Stars.color = A;

            if (time > Totaltime/8)
            {
                Player.GetComponent<CarManager2D>().HeadLights.enabled = true;
                Tachometer.Light = true;
                if (PlayMode == 1)
                {
                    AIPlayer.GetComponent<CarManager2D>().HeadLights.enabled = true;
                }

            }

        }
        else if (time > Totaltime / 2 && time < 0.75f * Totaltime)
        {
            gameObject.GetComponent<Camera>().backgroundColor = Color.Lerp(Night, Day, 4 * (time - (0.5f * Totaltime)) / Totaltime);
            GL.color = Color.Lerp(MoonLight, Sunlight, 4 * (time - (0.5f * Totaltime)) / Totaltime);
            Color A = Stars.color;
            A.a =1 - (4 * (time - (0.5f * Totaltime)) / Totaltime);
            A.a = Random.Range(0, 100) % 3 == 0 ? 1f * A.a : 0.5f * A.a;
            Stars.color = A;

            if (time > 5 * Totaltime / 8)
            {
                Player.GetComponent<CarManager2D>().HeadLights.enabled = false;
                Tachometer.Light = false;
                if (PlayMode == 1)
                {
                    AIPlayer.GetComponent<CarManager2D>().HeadLights.enabled = true;
                }
            }

        }
        else if (time < Totaltime/2 && time > Totaltime/4)
        {
            Color A = Stars.color;
            A.a = Random.Range(0,100) % 3 == 0 ? 1f: 0.5f;
            Stars.color = A;
        }

        if (time > Totaltime)
            time = 0f;

        for (int i = 0; i < GenClouds.Count; i++)
        {
            Vector3 pos = GenClouds[i].transform.position;
            pos.x -= Time.fixedDeltaTime;
            if (gameObject.GetComponent<Camera>().WorldToViewportPoint(pos).x < -0.05f )
            {
                pos = gameObject.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(1.05f, Random.Range(0.8f, 0.95f), 1));
                pos.z = Player.transform.position.z;
                Destroy(GenClouds[i]);
                GenClouds[i] = (Instantiate(Clouds[Random.Range(0, Clouds.Count)], pos, Quaternion.identity, gameObject.transform));
            }
            else
            {
                GenClouds[i].transform.position = pos;
            }
        }

    }

    public void GearInput(float value)
    {
        if (value > 0 && Player.GetComponent<CarController2D>().gear + 1 <= Player.GetComponent<CarController2D>().Maxgear)
        {
            Player.GetComponent<CarController2D>().gear += 1;
        }
        else if (value < 0 && Player.GetComponent<CarController2D>().gear - 1 >= -1)
            Player.GetComponent<CarController2D>().gear -= 1;

        print(Player.GetComponent<CarController2D>().gear);

    }

    public void Handbrake(Image ButtonIcon)
    {
        bool value = Player.GetComponent<CarController2D>().HandBreak;

        if (value)
        {
            Player.GetComponent<CarController2D>().HandBreak = false;
            Color a = ButtonIcon.color;
            a.a = 0.5f;
            ButtonIcon.color = a;
        }
        else
        {
            Player.GetComponent<CarController2D>().HandBreak = true;
            Color a = ButtonIcon.color;
            a.a = 1f;
            ButtonIcon.color = a;
        }
    }
    public void HornDown()
    { 
        CarHorned = true;
    }

    public void HornUp()
    {
        CarHorned = false;
    }


    public void AddCollectables(int Type,int Value)
    {
        //1 : Coins
        if (Type == 1)
        {
            CurrentCoins += Value;
        }
        else if (Type == 2)
        {
            CurrentDiamonds += Value;
        }
        else if (Type == 3)
        {
            if (Value > 0)
            {
                Player.GetComponent<CarController2D>().Health = Player.GetComponent<CarController2D>().MaxHealth;
            }
            else
            {
                Player.GetComponent<CarController2D>().Health += Value;
            }
        }
        else
        {
            Debug.Log("Fuel : " + Player.GetComponent<CarController2D>().Fuel + " , " + Player.GetComponent<CarController2D>().MaxFuel + " , " + Value);


            if (Value == 100)
            {
                Player.GetComponent<CarController2D>().Fuel = Player.GetComponent<CarController2D>().MaxFuel;
            }
            else if (Value == 50)
            {
                
                if (Player.GetComponent<CarController2D>().Fuel +( Player.GetComponent<CarController2D>().MaxFuel / 2 )> Player.GetComponent<CarController2D>().MaxFuel )
                {
                    Player.GetComponent<CarController2D>().Fuel = Player.GetComponent<CarController2D>().MaxFuel;
                }
                else
                {
                    Player.GetComponent<CarController2D>().Fuel += Player.GetComponent<CarController2D>().MaxFuel / 2;
                }
            }
            else
            {
                if (Player.GetComponent<CarController2D>().Fuel + (Player.GetComponent<CarController2D>().MaxFuel / 4) > Player.GetComponent<CarController2D>().MaxFuel)
                {
                    Player.GetComponent<CarController2D>().Fuel = Player.GetComponent<CarController2D>().MaxFuel;
                }
                else
                {
                    Player.GetComponent<CarController2D>().Fuel += Player.GetComponent<CarController2D>().MaxFuel / 4;
                }
            }
        }
    }

    public void AddAICollectables(int Type, int Value)
    {
        //1 : Coins
        if (Type == 1)
        {
            AICurrentCoins += Value;
        }
        else if (Type == 2)
        {
            AICurrentDiamonds += Value;
        }
        else if (Type == 3)
        {
            if (Value > 0)
            {
                AIPlayer.GetComponent<CarController2D>().Health = AIPlayer.GetComponent<CarController2D>().MaxHealth;
            }
            else
            {
                AIPlayer.GetComponent<CarController2D>().Health += Value * Time.deltaTime;
            }
        }
        else
        {
            if (Value == 100)
            {
                AIPlayer.GetComponent<CarController2D>().Fuel = AIPlayer.GetComponent<CarController2D>().MaxFuel;
            }
            else if (Value == 50)
            {

                if (AIPlayer.GetComponent<CarController2D>().Fuel + (AIPlayer.GetComponent<CarController2D>().MaxFuel / 2) > AIPlayer.GetComponent<CarController2D>().MaxFuel)
                {
                    Player.GetComponent<CarController2D>().Fuel = Player.GetComponent<CarController2D>().MaxFuel;
                }
                else
                {
                    AIPlayer.GetComponent<CarController2D>().Fuel += AIPlayer.GetComponent<CarController2D>().MaxFuel / 2;
                }
            }
            else
            {
                if (AIPlayer.GetComponent<CarController2D>().Fuel + (AIPlayer.GetComponent<CarController2D>().MaxFuel / 4) > AIPlayer.GetComponent<CarController2D>().MaxFuel)
                {
                    AIPlayer.GetComponent<CarController2D>().Fuel = AIPlayer.GetComponent<CarController2D>().MaxFuel;
                }
                else
                {
                    AIPlayer.GetComponent<CarController2D>().Fuel += AIPlayer.GetComponent<CarController2D>().MaxFuel / 4;
                }
            }
        }
    }
}
