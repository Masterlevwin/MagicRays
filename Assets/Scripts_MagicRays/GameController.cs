using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum GamePhase
{
    level,
    complete,
    pause
}

public class GameController: MonoBehaviour
{
    public static GameController G;
    public static int training = 0;
    public static bool invisResult = false;
    public GamePhase phase = GamePhase.level;
    public int result = 0;
    
    public Fader fader;
    public TMP_Text res;
    public TMP_Text sunDays;
    public GameObject trainingPref;
    public Target target;
    public TMP_Text txtTarg;
    public Image table;
    public GameObject[] vfx;
    public Sprite[] tables;
    public Sprite[] raySprites;
    public Sprite[] ranks;
    public SpriteRenderer rank;
    public delegate void CheckDelegate();
    public event CheckDelegate pullNotify;
    public event CheckDelegate cancelNotify;
    public event CheckDelegate restartNotify;

    void Awake()
    {
        if (G == null) G = this;
        else if (G == this) Destroy(gameObject);

        trainingPref = Instantiate(trainingPref, table.transform);
        LoadLevel();
    }

    public void LoadLevel()
    {
        if (PlayerPrefs.HasKey("SavedLevel")) training = PlayerPrefs.GetInt("SavedLevel");
        restartNotify?.Invoke();
        phase = GamePhase.level;
        result = 0;
        fader.FaderLevel();

        trainingPref.SetActive(false);
        table.sprite = tables[Random.Range(0, tables.Length)];
        
        if (training < 5) rank.sprite = ranks[0];
        else if (training < 15) rank.sprite = ranks[1];
        else if (training < 25) rank.sprite = ranks[2];
        else if (training < 35) rank.sprite = ranks[3];
        else if (training < 45) rank.sprite = ranks[4];
        else if (training < 55) rank.sprite = ranks[5];
        else if (training < 65) rank.sprite = ranks[6];
        else if (training < 75) rank.sprite = ranks[7];
        else rank.sprite = ranks[8];

        if (training == 0) TextView("Привет! Чтобы согреть персонажа, нажми на луч с нужным зарядом");
        if (training == 10) TextView("Некоторые лучи можно использовать два раза");
        if (training == 15) TextView("Также лучи могут понижать общий заряд");
        if (training == 20) TextView("Красные лучи поворачивают солнышко влево, а зеленые - вправо");
        if (training == 25) TextView("Нажав на голубой луч, можно выбрать - увеличить или понизить общий заряд");
        if (training == 30)
        {
            TextView("Попробуй сыграть вслепую или отключи этот режим, нажав на лупу");
            InvisResult();
        }
    }

    public void RestartLevel()
    {
        result = 0;
        cancelNotify?.Invoke();
    }
    
    public void Win()
    {
        //SoundManager.PlaySound("OrchestraEffect2");
        Instantiate(vfx[1]);
        training++;
        PlayerPrefs.SetInt("SavedLevel", training);
        PlayerPrefs.Save();
        if (training < 8)
        {
            TextView("Молодец! Понятно, как играть? Если нет, нажми на книжку, чтобы повторить обучение");
            Invoke("LoadLevel", 4f);
        } 
        else Invoke("LoadLevel", 2f);
    }

    public void Lose()
    {
        //SoundManager.PlaySound("LoseGame");
        phase = GamePhase.complete;
        Instantiate(vfx[0]);
        training--;
        PlayerPrefs.SetInt("SavedLevel", training);
        PlayerPrefs.Save();
        if (training < 8)
        {
            TextView("Посчитай внимательнее в следующий раз, или нажми на книжку, чтобы повторить обучение");
            Invoke("LoadLevel", 4f);
        } 
        else Invoke("LoadLevel", 2f);
    }
    
    void Update()
    {
        if (invisResult) res.text = $"";
        else res.text = $"{result}";

        if (phase != GamePhase.complete && result == target.temp)
        {
            phase = GamePhase.complete;
            pullNotify?.Invoke();
        }
    }

    public void InvisResult()
    {
        invisResult = !invisResult;
    }

    public void TraningNull()
    {
        training = 0;
        PlayerPrefs.SetInt("SavedLevel", training);
        PlayerPrefs.Save();
        LoadLevel();
    }

    public void TextView(string txt)
    {
        trainingPref.SetActive(true);
        trainingPref.GetComponentInChildren<TMP_Text>().text = txt;
    }

    public void TextTarget(string txt)
    {
        txtTarg.text = txt;
    }

    public static int RandomWithoutInt(int from, int to, int without = 0)
    {
        int res = Random.Range(from, to);
        if (res != without) return res;
        else return RandomWithoutInt(from, to, without);
    }
}
