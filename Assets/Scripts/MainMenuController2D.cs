using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class MainMenuController2D : MonoBehaviour
{
    public EventSystem E;

    int playmode,SelectedCar;

    public Sprite ToggleOn, ToggleOff;

    public Image TI, SFX;

    public Slider Volume;

    public Button Play, Controls, Options, Quit , PlayPage, OptionsPage;

    public GameObject PlayP, OptionsP, ControlsP;

    public Text CoinsT, DiamondsT;

    public int Coins, Diamonds;

    public List<string> LevelsS, LevelsR;
    public List<string> Cars;

    public GameObject MM, LevelSelector, CarSelector, Upgrades;

    public int CurrentLevel, MaxLevel;

    public Button LSP, CSP, CUP;

    public GameObject Error;

    public Text LST, CST;

    public GameObject CarPrice;
    public Text CSSelectText;

    public List<Sprite> CarSprite;

    public Image CarImage;

    public List<Text> UPCost;
    public List<Text> DownCost;
    public List<Image> PartLevel;
    public List<Sprite> AvailLevel;

    string[] Parts = { "Engine", "Brake", "Gear", "Suspension" };

    public Text CUHeadText;

    public float SFXVolume;

    public AudioClip CashSound;

    public List<string> Description;

    public Text Des;

    public GameObject EndObject;

    public Animator Bg;

    void Awake()
    {
        for (int i = 0; i < Description.Count;i++)
        {
            Description[i] = Description[i].Replace("\\n", "\n");
        }

        if (Application.platform == RuntimePlatform.Android||Application.platform == RuntimePlatform.IPhonePlayer)
        {
            PlayerPrefs.SetInt("TI", 1);
        }

        PlayerPrefs.SetInt("Car_0", 1);
        PlayerPrefs.Save();

        Coins = PlayerPrefs.GetInt("Coins",10000);
        Diamonds = PlayerPrefs.GetInt("Diamonds",1000);

        CoinsT.text = "" + Coins;
        DiamondsT.text = "" + Diamonds;


        if (PlayerPrefs.GetInt("TI", 0) > 0)
        {
            TI.sprite = ToggleOn;
        }
        else
        {
            TI.sprite = ToggleOff;
        }

        if (PlayerPrefs.GetInt("SFX", 1) > 0)
        {
            SFX.sprite = ToggleOn;
            SFXVolume = 1f;
        }
        else
        {
            SFX.sprite = ToggleOff;
            SFXVolume = 0f;
        }

        Volume.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        GetComponent<AudioSource>().volume = 0.5f * Volume.value;

        LevelSelector.SetActive(false);
        CarSelector.SetActive(false);
        Upgrades.SetActive(false);

        OptionsP.SetActive(false);
        ControlsP.SetActive(false);

        StartCoroutine(WaitForFrame());

    }

    IEnumerator WaitForFrame()
    {
        while(!Bg.GetCurrentAnimatorStateInfo(0).IsTag("End"))
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        MM.SetActive(true);
        PlayP.SetActive(true);
        PlayPage.Select();
    }
    
    // Start is called before the first frame update

    public void Buttons(string name)
    {
        Error.GetComponentInChildren<Text>().text = "";
        Error.SetActive(false);
        if (name == "Play")
        {
            PlayP.SetActive(true);
            OptionsP.SetActive(false);
            ControlsP.SetActive(false);
            Navigation PlayN = Play.navigation;
            Navigation OptionsN = Options.navigation;
            Navigation ControlsN = Controls.navigation;
            Navigation QuitN = Quit.navigation;

            PlayN.selectOnDown = OptionsN.selectOnDown = ControlsN.selectOnDown = QuitN.selectOnDown = PlayPage;

            Play.navigation = PlayN;
            Options.navigation = OptionsN;
            Controls.navigation = ControlsN;
            Quit.navigation = QuitN;

            PlayPage.Select();

        }
        else if (name == "Options")
        {
            PlayP.SetActive(false);
            OptionsP.SetActive(true);
            ControlsP.SetActive(false);
            Navigation PlayN = Play.navigation;
            Navigation OptionsN = Options.navigation;
            Navigation ControlsN = Controls.navigation;
            Navigation QuitN = Quit.navigation;

            PlayN.selectOnDown = OptionsN.selectOnDown = ControlsN.selectOnDown = QuitN.selectOnDown = OptionsPage;

            Play.navigation = PlayN;
            Options.navigation = OptionsN;
            Controls.navigation = ControlsN;
            Quit.navigation = QuitN;
            OptionsPage.Select();
        }
        else if (name == "Controls" || name == "Quit")
        {
            Navigation PlayN = Play.navigation;
            Navigation OptionsN = Options.navigation;
            Navigation ControlsN = Controls.navigation;
            Navigation QuitN = Quit.navigation;

            PlayN.selectOnDown = OptionsN.selectOnDown = ControlsN.selectOnDown = QuitN.selectOnDown = null;

            Play.navigation = PlayN;
            Options.navigation = OptionsN;
            Controls.navigation = ControlsN;
            Quit.navigation = QuitN;
            if (name == "Controls")
            {
                PlayP.SetActive(false);
                OptionsP.SetActive(false);
                ControlsP.SetActive(true);
            }
            else
            {
                StartCoroutine(LoadScene(3));
            }
        }
        else if (name == "Play_Story")
        {
            MM.SetActive(false);
            LevelSelector.SetActive(true);
            LSP.Select();
            playmode = 0;
            MaxLevel = PlayerPrefs.GetInt("SMLevel", 0);
            CurrentLevel = MaxLevel;
            LST.text = "Level " + (CurrentLevel + 1) + " : " + LevelsS[CurrentLevel];
        }
        else if (name == "Play_Race")
        {
            MM.SetActive(false);
            LevelSelector.SetActive(true);
            LSP.Select();
            playmode = 1;
            MaxLevel = PlayerPrefs.GetInt("RMLevel", 0);
            CurrentLevel = MaxLevel;
            LST.text = "Level " + (CurrentLevel + 1) + " : " + LevelsR[CurrentLevel];
        }
        else if (name == "Select_LS")
        {
            CarSelector.SetActive(true);
            LevelSelector.SetActive(false);
            CSP.Select();

            CarImage.sprite = CarSprite[SelectedCar];

            SelectedCar = PlayerPrefs.GetInt("CurrentCar", 0);
            CarPrice.SetActive(false);
            CST.text = Cars[SelectedCar];

            Des.text = "Description : \n" + Description[SelectedCar];
        }
        else if (name == "Back_LS")
        {
            MM.SetActive(true);
            LevelSelector.SetActive(false);
            PlayPage.Select();
        }
        else if (name == "Select_CS")
        {
            if (CSSelectText.text == "Buy")
            {
                if (int.Parse(CarPrice.GetComponentInChildren<Text>().text) > Diamonds)
                {
                    Error.SetActive(true);
                    Error.GetComponentInChildren<Text>().text = "Not Enough Diamonds";
                }
                else
                {
                    GetComponent<AudioSource>().PlayOneShot(CashSound, SFXVolume);
                    Diamonds = Diamonds - int.Parse(CarPrice.GetComponentInChildren<Text>().text);
                    PlayerPrefs.SetInt("Diamonds", Diamonds);
                    PlayerPrefs.SetInt("Car_" + SelectedCar, 1);
                    PlayerPrefs.SetInt(SelectedCar + "Engine", 4);
                    PlayerPrefs.SetInt(SelectedCar + "Brake", 4);
                    PlayerPrefs.SetInt(SelectedCar + "Gear", 4);
                    PlayerPrefs.SetInt(SelectedCar + "Suspension", 4);
                    PlayerPrefs.Save();
                    DiamondsT.text = "" + Diamonds;
                    CSSelectText.text = "Select Car";
                    CarPrice.SetActive(false);
                }
            }
            else
            {
                CarSelector.SetActive(false);
                Upgrades.SetActive(true);
                CUP.Select();

                CUHeadText.text = Cars[SelectedCar];

                for (int i = 0; i < 4; i++)
                {
                    PartLevel[i].sprite = AvailLevel[4 - PlayerPrefs.GetInt("" + SelectedCar + Parts[i], 4)];
                    UPCost[i].text = PlayerPrefs.GetInt("" + SelectedCar + Parts[i], 4) != 1 ? "" + ((5 - PlayerPrefs.GetInt("" + SelectedCar + Parts[i], 4)) * 6000) : "";
                    DownCost[i].text = PlayerPrefs.GetInt("" + SelectedCar + Parts[i], 4) != 4 ? "" + ((5 - PlayerPrefs.GetInt("" + SelectedCar + Parts[i], 4)) * 5000) : "";
                }

            }
        }
        else if (name == "Back_CS")
        {
            LevelSelector.SetActive(true);
            CarSelector.SetActive(false);
            LSP.Select();
        }
        else if (name == "Select_CU")
        {
            PlayerPrefs.SetInt("PlayMode", playmode);
            PlayerPrefs.SetInt("CurrentLevel", CurrentLevel);
            PlayerPrefs.SetInt("CurrentCar", SelectedCar);
            PlayerPrefs.Save();

            E.SetSelectedGameObject(null);

            StartCoroutine(LoadScene(2));

        }
        else if (name == "Back_CU")
        {
            CarSelector.SetActive(true);
            Upgrades.SetActive(false);
            CSP.Select();
        }
        else if (name == "R_LS")
        {
            if (CurrentLevel + 1 > MaxLevel)
            {
                Error.SetActive(true);
                Error.GetComponentInChildren<Text>().text = "No More Levels Unlocked";
            }
            else
            {
                CurrentLevel += 1;
            }
            if (playmode == 0)
                LST.text = "Level " + (CurrentLevel + 1) + " : " + LevelsS[CurrentLevel];
            else
                LST.text = "Level " + (CurrentLevel + 1) + " : " + LevelsR[CurrentLevel];
        }
        else if (name == "L_LS")
        {
            if (CurrentLevel - 1 < 0)
            {
                Error.SetActive(true);
                Error.GetComponentInChildren<Text>().text = "Level 1 is the Base Level";
            }
            else
            {
                CurrentLevel -= 1;
            }
            if (playmode == 0)
                LST.text = "Level " + (CurrentLevel + 1) + " : " + LevelsS[CurrentLevel];
            else
                LST.text = "Level " + (CurrentLevel + 1) + " : " + LevelsR[CurrentLevel];
        }
        else if (name == "R_CS")
        {
            if (SelectedCar + 1 > Cars.Count - 1)
            {
                SelectedCar = 0;
            }
            else
            {
                SelectedCar += 1;
            }

            if (PlayerPrefs.GetInt("Car_" + SelectedCar, 0) == 0)
            {
                CarPrice.SetActive(true);
                CarPrice.GetComponentInChildren<Text>().text = "" + ((SelectedCar + 1) * 2500);
                CSSelectText.text = "Buy";
            }
            else
            {
                CarPrice.SetActive(false);
                CSSelectText.text = "Select Car";
            }

            CarImage.sprite = CarSprite[SelectedCar];
            CST.text = Cars[SelectedCar];
            Des.text = "Description : \n" + Description[SelectedCar];

        }
        else if (name == "L_CS")
        {
            if (SelectedCar - 1 < 0)
            {
                SelectedCar = Cars.Count - 1;
            }
            else
            {
                SelectedCar -= 1;
            }

            if (PlayerPrefs.GetInt("Car_" + SelectedCar, 0) == 0)
            {
                CarPrice.SetActive(true);
                CarPrice.GetComponentInChildren<Text>().text = "" + ((SelectedCar + 1) * 2500);
                CSSelectText.text = "Buy";
            }
            else
            {
                CarPrice.SetActive(false);
                CSSelectText.text = "Select Car";
            }

            CarImage.sprite = CarSprite[SelectedCar];
            CST.text = Cars[SelectedCar];
            Des.text = "Description : \n" + Description[SelectedCar];
        }
        else if (name == "DLS")
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }


    public void UpgradeD(string name)
    {
        int i = int.Parse(name.Substring(2));
        if (name.Contains("U_"))
        {
            if (UPCost[i].text == "")
            {
                Error.SetActive(true);
                Error.GetComponentInChildren<Text>().text = "This part is Upgraded to Max Level";
            }
            else if (Coins > int.Parse(UPCost[i].text))
            {
                GetComponent<AudioSource>().PlayOneShot(CashSound, SFXVolume);
                PartLevel[i].sprite = AvailLevel[AvailLevel.IndexOf(PartLevel[i].sprite) + 1];
                PlayerPrefs.SetInt("" + SelectedCar + Parts[i], PlayerPrefs.GetInt("" + SelectedCar + Parts[i], 4) - 1);
                Coins -= int.Parse(UPCost[i].text);
                PlayerPrefs.SetInt("Coins", Coins);
                PlayerPrefs.Save();
                CoinsT.text = "" + Coins;
                DownCost[i].text = "" + ((5 - PlayerPrefs.GetInt("" + SelectedCar + Parts[i],4))* 5000);
                UPCost[i].text = PlayerPrefs.GetInt("" + SelectedCar + Parts[i], 4) != 1 ? "" + ((5 - PlayerPrefs.GetInt("" + SelectedCar + Parts[i], 4)) * 6000)  : "";
            }
            else
            {
                Error.SetActive(true);
                Error.GetComponentInChildren<Text>().text = "Not Enough Coins";
            }
        }
        else
        {
            if (DownCost[i].text == "")
            {
                Error.SetActive(true);
                Error.GetComponentInChildren<Text>().text = "This part is Upgraded to Max Level";
            }
            else if (Coins > int.Parse(DownCost[i].text))
            {
                GetComponent<AudioSource>().PlayOneShot(CashSound, SFXVolume);
                PartLevel[i].sprite = AvailLevel[AvailLevel.IndexOf(PartLevel[i].sprite) - 1];
                PlayerPrefs.SetInt("" + SelectedCar + Parts[i], PlayerPrefs.GetInt("" + SelectedCar + Parts[i], 4) + 1);
                Coins -= int.Parse(DownCost[i].text);
                PlayerPrefs.SetInt("Coins", Coins);
                PlayerPrefs.Save();
                CoinsT.text = "" + Coins;
                UPCost[i].text = "" + ((5 - PlayerPrefs.GetInt("" + SelectedCar + Parts[i], 4)) * 6000);
                DownCost[i].text = PlayerPrefs.GetInt("" + SelectedCar + Parts[i], 4) != 4 ? "" + ((5 - PlayerPrefs.GetInt("" + SelectedCar + Parts[i], 4)) * 5000) : "";
            }
            else
            {
                Error.SetActive(true);
                Error.GetComponentInChildren<Text>().text = "Not Enough Coins";
            }
        }

    }

    public void Toggle(Image toggle)
    {
        if (toggle.sprite == ToggleOn)
        {
            toggle.sprite = ToggleOff;
            if (toggle.gameObject.name == "TI")
            {
                PlayerPrefs.SetInt("TI", 0);
            }
            else if (toggle.gameObject.name == "SFX")
            {
                PlayerPrefs.SetInt("SFX", 0);
                SFXVolume = 0f;
            }
            PlayerPrefs.Save();
        }
        else
        {
            toggle.sprite = ToggleOn;
            if (toggle.gameObject.name == "TI")
            {
                PlayerPrefs.SetInt("TI", 1);
            }
            else if (toggle.gameObject.name == "SFX")
            {
                PlayerPrefs.SetInt("SFX", 1);
                SFXVolume = 1f;
            }
            PlayerPrefs.Save();
        }
    }

    public void Slider()
    {
        PlayerPrefs.SetFloat("MusicVolume", Volume.value);
        PlayerPrefs.Save();
        GetComponent<AudioSource>().volume = Volume.value * 0.5f;
    }

    public IEnumerator LoadScene(int buildindex)
    {
        EndObject.SetActive(true);
        AsyncOperation A = SceneManager.LoadSceneAsync(buildindex);
        A.allowSceneActivation = false;
        float timer = 0f;
        while (!A.isDone)
        {
            timer += Time.unscaledDeltaTime;
            if (timer > 1.5f)
            {
                EndObject.GetComponent<Animator>().SetBool("End", true);
                if (timer > 1.8f)
                {
                    Time.timeScale = 1f;
                    A.allowSceneActivation = true;
                }
            }
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
    }
}
