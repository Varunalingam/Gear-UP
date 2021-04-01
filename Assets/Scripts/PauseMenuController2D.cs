using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class PauseMenuController2D : MonoBehaviour
{

    public GameObject PauseMenu;
    public Button Starts;

    public Sprite ToggleOn, ToggleOff;

    public Image TI, SFX;

    public GameObject TouchInput;

    public Slider Volume;

    public GameObject LevelEndCall;

    public Text LEheadline;
    public Text LEGolds;
    public Text LEDias;
    public Text LETime;

    public Color Red;

    public Button StartL;
    public Button RestartL;

    public GameObject EndObject;

    // Start is called before the first frame update
    void Start()
    {
        PauseMenu.SetActive(FindObjectOfType<GameController2D>().paused);
        TouchInput.SetActive(FindObjectOfType<GameController2D>().TouchInput);
        if (FindObjectOfType<GameController2D>().TouchInput)
        {
            TI.sprite = ToggleOn;
        }
        else
        {
            TI.sprite = ToggleOff;
        }

        if (FindObjectOfType<GameController2D>().SFX)
        {
            SFX.sprite = ToggleOn;
        }
        else
        {
            SFX.sprite = ToggleOff;
        }

        Volume.value = FindObjectOfType<GameController2D>().MusicVolume;

        FindObjectOfType<GameController2D>().gameObject.GetComponent<AudioSource>().volume = Volume.value * 0.2f;
    }

    bool LevelEndedCalled = false;
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7)) && !FindObjectOfType<GameController2D>().LevelEnded)
        {
            Pause();
        }

        if (FindObjectOfType<GameController2D>().LevelEnded && !LevelEndedCalled)
        {
            LevelEndedCalled = true;
            LevelEndCall.SetActive(true);
            Time.timeScale = 0f;
            FindObjectOfType<GameController2D>().paused = true;

            LETime.text = TimeConvert(FindObjectOfType<GameController2D>().TimeTaken);

            if (FindObjectOfType<GameController2D>().LevelCompleted)
            {
                if (FindObjectOfType<GameController2D>().PlayMode == 1)
                {
                    if (FindObjectOfType<GameController2D>().AIWin)
                    {
                        StartL.interactable = false;
                        Navigation RL = RestartL.navigation;
                        RL.selectOnLeft = null;
                        RestartL.navigation = RL;
                        RestartL.Select();

                        LEheadline.text = "Level LOST";

                        LEheadline.color = Red;
                        LEDias.color = Red;
                        LEGolds.color = Red;

                        LEGolds.text = "-" + FindObjectOfType<GameController2D>().AICurrentCoins;
                        LEDias.text = "-" + FindObjectOfType<GameController2D>().AICurrentDiamonds;
                        PlayerPrefs.SetInt("Coins", FindObjectOfType<GameController2D>().CurrentCoins - FindObjectOfType<GameController2D>().AICurrentCoins > 0 ? FindObjectOfType<GameController2D>().CurrentCoins - FindObjectOfType<GameController2D>().AICurrentCoins : 0);
                        PlayerPrefs.SetInt("Diamonds", FindObjectOfType<GameController2D>().CurrentDiamonds - FindObjectOfType<GameController2D>().AICurrentDiamonds > 0 ? FindObjectOfType<GameController2D>().CurrentDiamonds - FindObjectOfType<GameController2D>().AICurrentDiamonds : 0);
                    }
                    else
                    {

                        if (FindObjectOfType<GameController2D>().GetComponent<SpiriteGenerator2D>().Level == FindObjectOfType<GameController2D>().GetComponent<SpiriteGenerator2D>().AILevelData.Count)
                        {

                        }
                        else if (FindObjectOfType<GameController2D>().GetComponent<SpiriteGenerator2D>().Level == PlayerPrefs.GetInt("RMLevel",0))
                        {
                            PlayerPrefs.SetInt("RMLevel", FindObjectOfType<GameController2D>().GetComponent<SpiriteGenerator2D>().Level + 1);
                        }

                        StartL.Select();

                        LEheadline.text = "Level Won";
                        LEGolds.text = "+" + (int)((FindObjectOfType<GameController2D>().CurrentCoins - PlayerPrefs.GetInt("Coins", 10000)) + FindObjectOfType<GameController2D>().AICurrentCoins);
                        LEDias.text = "+" + (int)((FindObjectOfType<GameController2D>().CurrentDiamonds - PlayerPrefs.GetInt("Diamonds", 1000)) + FindObjectOfType<GameController2D>().AICurrentDiamonds);
                        PlayerPrefs.SetInt("Coins", FindObjectOfType<GameController2D>().CurrentCoins + FindObjectOfType<GameController2D>().AICurrentCoins);
                        PlayerPrefs.SetInt("Diamonds", FindObjectOfType<GameController2D>().CurrentDiamonds + FindObjectOfType<GameController2D>().AICurrentDiamonds);
                    }
                }
                else
                {
                    if (FindObjectOfType<GameController2D>().GetComponent<SpiriteGenerator2D>().Level == FindObjectOfType<GameController2D>().GetComponent<SpiriteGenerator2D>().LevelData.Count)
                    {

                    }
                    else if (FindObjectOfType<GameController2D>().GetComponent<SpiriteGenerator2D>().Level == PlayerPrefs.GetInt("SMLevel", 0))
                    {
                        PlayerPrefs.SetInt("SMLevel", FindObjectOfType<GameController2D>().GetComponent<SpiriteGenerator2D>().Level + 1);
                    }

                    StartL.Select();
                    LEheadline.text = "Level Complete";
                    LEGolds.text = "+" + (int)((FindObjectOfType<GameController2D>().CurrentCoins - PlayerPrefs.GetInt("Coins", 10000)));
                    LEDias.text = "+" + (int)((FindObjectOfType<GameController2D>().CurrentDiamonds - PlayerPrefs.GetInt("Diamonds", 1000)));
                    PlayerPrefs.SetInt("Coins", FindObjectOfType<GameController2D>().CurrentCoins);
                    PlayerPrefs.SetInt("Diamonds", FindObjectOfType<GameController2D>().CurrentDiamonds);
                }
            }
            else if (FindObjectOfType<GameController2D>().Player.GetComponent<CarController2D>().Health < 0)
            {
                StartL.interactable = false;
                Navigation RL = RestartL.navigation;
                RL.selectOnLeft = null;
                RestartL.navigation = RL;
                RestartL.Select();
                if (FindObjectOfType<GameController2D>().PlayMode == 1)
                {
                    LEheadline.text = "Race Over! You Died!";
                    LEGolds.text = "-" + (int)(((FindObjectOfType<GameController2D>().CurrentCoins - PlayerPrefs.GetInt("Coins", 10000)) / 2) + FindObjectOfType<GameController2D>().AICurrentCoins);
                    LEDias.text = "-" + (int)(((FindObjectOfType<GameController2D>().CurrentDiamonds - PlayerPrefs.GetInt("Diamonds", 1000)) / 2) + FindObjectOfType<GameController2D>().AICurrentDiamonds);

                    LEheadline.color = Red;
                    LEDias.color = Red;
                    LEGolds.color = Red;

                    PlayerPrefs.SetInt("Coins", FindObjectOfType<GameController2D>().CurrentCoins - ((FindObjectOfType<GameController2D>().CurrentCoins - PlayerPrefs.GetInt("Coins", 10000)) / 2) - FindObjectOfType<GameController2D>().AICurrentCoins > 0 ? FindObjectOfType<GameController2D>().CurrentCoins - ((FindObjectOfType<GameController2D>().CurrentCoins - PlayerPrefs.GetInt("Coins", 10000)) / 2) - FindObjectOfType<GameController2D>().AICurrentCoins : 0);
                    PlayerPrefs.SetInt("Diamonds", FindObjectOfType<GameController2D>().CurrentDiamonds - ((FindObjectOfType<GameController2D>().CurrentDiamonds - PlayerPrefs.GetInt("Diamonds", 1000)) / 2) - FindObjectOfType<GameController2D>().AICurrentDiamonds > 0 ? FindObjectOfType<GameController2D>().CurrentDiamonds - ((FindObjectOfType<GameController2D>().CurrentDiamonds - PlayerPrefs.GetInt("Diamonds", 1000)) / 2) - FindObjectOfType<GameController2D>().AICurrentDiamonds : 0);
                }
                else
                {
                    LEheadline.text = "Game Over! You Died!";
                    LEGolds.text = "-" + (int)((FindObjectOfType<GameController2D>().CurrentCoins - PlayerPrefs.GetInt("Coins", 10000)) / 2);
                    LEDias.text = "-" + (int)((FindObjectOfType<GameController2D>().CurrentDiamonds - PlayerPrefs.GetInt("Diamonds", 1000)) / 2);

                    LEheadline.color = Red;
                    LEDias.color = Red;
                    LEGolds.color = Red;

                    PlayerPrefs.SetInt("Coins", FindObjectOfType<GameController2D>().CurrentCoins - (int)((FindObjectOfType<GameController2D>().CurrentCoins - PlayerPrefs.GetInt("Coins", 10000)) / 2));
                    PlayerPrefs.SetInt("Diamonds", FindObjectOfType<GameController2D>().CurrentDiamonds - (int)((FindObjectOfType<GameController2D>().CurrentDiamonds - PlayerPrefs.GetInt("Diamonds", 1000)) / 2));
                }
            }
            else if (FindObjectOfType<GameController2D>().Player.GetComponent<CarController2D>().Fuel < 0)
            {
                StartL.interactable = false;
                Navigation RL = RestartL.navigation;
                RL.selectOnLeft = null;
                RestartL.navigation = RL;
                RestartL.Select();
                if (FindObjectOfType<GameController2D>().PlayMode == 1)
                {
                    LEheadline.text = "Race Over! You Didn't Fill the Tank!";
                    LEGolds.text = "-" + (int)(((FindObjectOfType<GameController2D>().CurrentCoins - PlayerPrefs.GetInt("Coins", 10000)) / 3) + FindObjectOfType<GameController2D>().AICurrentCoins);
                    LEDias.text = "-" + (int)(((FindObjectOfType<GameController2D>().CurrentDiamonds - PlayerPrefs.GetInt("Diamonds", 1000)) / 3) + FindObjectOfType<GameController2D>().AICurrentDiamonds);

                    LEheadline.color = Red;
                    LEDias.color = Red;
                    LEGolds.color = Red;

                    PlayerPrefs.SetInt("Coins", FindObjectOfType<GameController2D>().CurrentCoins - ((FindObjectOfType<GameController2D>().CurrentCoins - PlayerPrefs.GetInt("Coins", 10000)) / 3) - FindObjectOfType<GameController2D>().AICurrentCoins > 0 ? FindObjectOfType<GameController2D>().CurrentCoins - ((FindObjectOfType<GameController2D>().CurrentCoins - PlayerPrefs.GetInt("Coins", 10000)) / 3) - FindObjectOfType<GameController2D>().AICurrentCoins : 0);
                    PlayerPrefs.SetInt("Diamonds", FindObjectOfType<GameController2D>().CurrentDiamonds - ((FindObjectOfType<GameController2D>().CurrentDiamonds - PlayerPrefs.GetInt("Diamonds", 1000)) / 3) - FindObjectOfType<GameController2D>().AICurrentDiamonds > 0 ? FindObjectOfType<GameController2D>().CurrentDiamonds - ((FindObjectOfType<GameController2D>().CurrentDiamonds - PlayerPrefs.GetInt("Diamonds", 1000)) / 3) - FindObjectOfType<GameController2D>().AICurrentDiamonds : 0);
                }
                else
                {
                    LEheadline.text = "Game Over! You Lost Gasoline!";
                    LEGolds.text = "-" + (int)((FindObjectOfType<GameController2D>().CurrentCoins - PlayerPrefs.GetInt("Coins", 10000)) / 3);
                    LEDias.text = "-" + (int)((FindObjectOfType<GameController2D>().CurrentDiamonds - PlayerPrefs.GetInt("Diamonds", 1000)) / 3);
                    PlayerPrefs.SetInt("Coins", FindObjectOfType<GameController2D>().CurrentCoins - (int)((FindObjectOfType<GameController2D>().CurrentCoins - PlayerPrefs.GetInt("Coins", 10000)) / 3));
                    PlayerPrefs.SetInt("Diamonds", FindObjectOfType<GameController2D>().CurrentDiamonds - (int)((FindObjectOfType<GameController2D>().CurrentDiamonds - PlayerPrefs.GetInt("Diamonds", 1000)) / 3));

                    LEheadline.color = Red;
                    LEDias.color = Red;
                    LEGolds.color = Red;
                }
            }

            PlayerPrefs.Save();

        }

    }

    public string TimeConvert(float seconds)
    {
        int sec = Mathf.RoundToInt(seconds);
        return  sec/60 > 0 ? "" + (sec/60) + " mins : " + (sec % 60) + " secs" : ""  + sec + " secs";
    }


    public void Pause()
    {
        if (FindObjectOfType<GameController2D>().paused)
        {
            Time.timeScale = 1f;
            FindObjectOfType<GameController2D>().paused = false;
            PauseMenu.SetActive(false);
        }
        else
        {
            Time.timeScale = 0f;
            FindObjectOfType<GameController2D>().paused = true;
            PauseMenu.SetActive(true);
            Starts.Select();
        }
    }

    public void ToggleButton(Image toggle)
    {
        if (toggle.sprite == ToggleOn)
        {
            toggle.sprite = ToggleOff;
            if (toggle.gameObject.name == "TI")
            {
                FindObjectOfType<GameController2D>().TouchInput = false;
                PlayerPrefs.SetInt("TI", 0);

                TouchInput.SetActive(false);
            }
            else if (toggle.gameObject.name == "SFX")
            {
                FindObjectOfType<GameController2D>().SFX = false;
                PlayerPrefs.SetInt("SFX", 0);
            }
            PlayerPrefs.Save();
        }
        else
        {
            toggle.sprite = ToggleOn;
            if (toggle.gameObject.name == "TI")
            {
                FindObjectOfType<GameController2D>().TouchInput = true;
                TouchInput.SetActive(true);
                PlayerPrefs.SetInt("TI", 1);
            }
            else if (toggle.gameObject.name == "SFX")
            {
                FindObjectOfType<GameController2D>().SFX = true;
                PlayerPrefs.SetInt("SFX", 1);
            }
            PlayerPrefs.Save();
        }
    }

    public void VolumeSlider()
    {
        FindObjectOfType<GameController2D>().gameObject.GetComponent<AudioSource>().volume = Volume.value * 0.2f;
        PlayerPrefs.SetFloat("MusicVolume", Volume.value);
        PlayerPrefs.Save();
    }

    public void ChangeScene(int id)
    {
        if (id == 0)
        {
            if (!LevelEndedCalled)
            {
                StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex));
            }
            else
            {
                StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex));
            }
        }
        else if (id == 1)
        {
            if (!LevelEndedCalled)
            {
                StartCoroutine(LoadScene(1));
            }
            else
            {
                StartCoroutine(LoadScene(1));
            }
        }
        else if (id == 2)
        {
            print(FindObjectOfType<GameController2D>().GetComponent<SpiriteGenerator2D>().Level);
            if (FindObjectOfType<GameController2D>().GetComponent<SpiriteGenerator2D>().Level == (FindObjectOfType<GameController2D>().PlayMode == 0 ? FindObjectOfType<GameController2D>().GetComponent<SpiriteGenerator2D>().LevelData.Count : FindObjectOfType<GameController2D>().GetComponent<SpiriteGenerator2D>().AILevelData.Count))
            {
                print("t");
                PlayerPrefs.SetInt("CurrentLevel", FindObjectOfType<GameController2D>().GetComponent<SpiriteGenerator2D>().Level);
            }
            else
            {
                PlayerPrefs.SetInt("CurrentLevel", FindObjectOfType<GameController2D>().GetComponent<SpiriteGenerator2D>().Level + 1);
            }
            PlayerPrefs.Save();
            print(PlayerPrefs.GetInt("CurrentLevel",0));
            StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex));

        }
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
            if (timer > 2f)
            {
                EndObject.GetComponent<Animator>().SetBool("End", true);
                if (timer > 2.3f)
                {
                    Time.timeScale = 1f;
                    A.allowSceneActivation = true;
                }
            }
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
    }

}
