using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Target : MonoBehaviour, IPointerClickHandler
{
    public GameObject iceCube;
    public Sprite[] iceCubes;
    public Sprite[] targets;

    void Start()
    {
        GameController.G.restartNotify += TargetLevel;
        TargetLevel();
    }

    private void TargetLevel()
    {
        GetComponent<SpriteRenderer>().sprite = targets[Random.Range(0, targets.Length)];
        iceCube.SetActive(true);
        iceCube.GetComponent<SpriteRenderer>().sprite = iceCubes[Random.Range(0, iceCubes.Length)];
        
        if (GameController.level == 0) SetTemp(5);
        else if (GameController.level < 15) SetTemp(Random.Range(9, 16));
        else if (GameController.level < 25) SetTemp(Random.Range(9, 26));
        else SetTemp(Random.Range(9, 36));
    }

    public int temp { private set; get; }
    
    public void SetTemp(int t = 0)
    {
        temp = t;
        if (temp == 0)
        {
            iceCube.SetActive(false);
            GameController.G.TextTarget("");
            return;
        }
        
        int x = Random.Range(2, 10);
        string[] str = { $"{temp}", $"{temp-x}+{2*x}-{x}", $"{temp-x}-{x}+{2*x}",
        $"{Mathf.FloorToInt(temp/x)}*{x}+{temp%x}",
        $"{temp%x}+{Mathf.FloorToInt(temp/x)}*{x}",
        $"{Mathf.FloorToInt(temp/x)}*{2*x}-{temp-(temp%x*2)}",
        $"{temp*2-(temp%x)}-{Mathf.FloorToInt(temp/x)}*{x}",
        $"{temp*x}/{x}", $"{temp*x-(temp%2)*x}/{2*x}+{Mathf.FloorToInt(temp/2)+(temp%2)}",
        $"{Mathf.FloorToInt(temp/2)+(temp%2)}+{temp*x-(temp%2)*x}/{2*x}",
        $"{temp*x*4}/{x}-{temp*3}",
        $"{temp+Mathf.FloorToInt(temp/2)}-{temp*x-(temp%2)*x}/{2*x}" };
        
        if (GameController.level < 15) GameController.G.TextTarget(str[Random.Range(0, 1)]);
        else if (GameController.level < 25) GameController.G.TextTarget(str[Random.Range(1, 4)]);
        else if (GameController.level < 35) GameController.G.TextTarget(str[Random.Range(4, 7)]);
        else GameController.G.TextTarget(str[Random.Range(7, str.Length)]);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameController.G.phase != GamePhase.level) return;
        else
        {
            SoundManager.PlaySound("Click1");
            GameController.G.RestartLevel();
        }
    }
}
