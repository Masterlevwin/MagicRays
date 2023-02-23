using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum Skills : byte
{
    none,
    doubleUse,
    rotateRight,
    rotateLeft,
    reverseTemp
}

public class RaySunny : MonoBehaviour, IPointerClickHandler
{
    public int t { private set; get; }
    public bool act { private set; get; }
    public Skills skill = Skills.none;
    public SpriteRenderer[] spriteRenderers;
    private Sunny sunny;
    private Skills startSkill;
    private int startTemp;
    
    private void OnEnable()
    {
        SetSortOrder(0);
        GameController.G.cancelNotify += Cancel;
        GameController.G.restartNotify += RayLevel;
        RayLevel();
    }

    private void RayLevel()
    {
        ActiveButton(true);
        if (GameController.training < 25)
        {
            if (GameController.training == 0) SetTemperature(Random.Range(4, 6));
            else if (GameController.training < 5) SetTemperature(Random.Range(2, 9));
            else if (GameController.training < 10) SetTemperature(Random.Range(1, 10));
            else if (GameController.training < 15)
            {
                SetTemperature(Random.Range(1, 10));
                skill = (Skills)Random.Range(0, System.Enum.GetValues(typeof(Skills)).Length - 3);
            }
            else if (GameController.training < 20)
            {
                SetTemperature(GameController.RandomWithoutInt(-5, 10, 0));
                skill = (Skills)Random.Range(0, System.Enum.GetValues(typeof(Skills)).Length - 3);
            }
            else if (GameController.training < 25)
            {
                SetTemperature(GameController.RandomWithoutInt(-5, 10, 0));
                skill = (Skills)Random.Range(0, System.Enum.GetValues(typeof(Skills)).Length - 1);
            }
        }
        else
        { 
            SetTemperature(GameController.RandomWithoutInt(-9, 10, 0));
            skill = (Skills)Random.Range(0, System.Enum.GetValues(typeof(Skills)).Length);
        }
        startSkill = SetSkill(skill);
        startTemp = t;
    }

    public Skills SetSkill(Skills sk = Skills.none)
    {
        switch (sk)
        {
            case Skills.doubleUse:
                spriteRenderers[1].sprite = GameController.G.raySprites[15];
                spriteRenderers[4].sprite = GameController.G.raySprites[10];
                break;
            case Skills.rotateRight:
                spriteRenderers[1].sprite = GameController.G.raySprites[16];
                spriteRenderers[4].sprite = GameController.G.raySprites[11];
                break;
            case Skills.rotateLeft:
                spriteRenderers[1].sprite = GameController.G.raySprites[17];
                spriteRenderers[4].sprite = GameController.G.raySprites[12];
                break;
            case Skills.reverseTemp:
                spriteRenderers[1].sprite = GameController.G.raySprites[18];
                spriteRenderers[4].sprite = GameController.G.raySprites[13];
                break;
            default:
                spriteRenderers[1].sprite = GameController.G.raySprites[19];
                spriteRenderers[4].sprite = GameController.G.raySprites[14];
                skill = Skills.none;
                break;
        }
        skill = sk;
        return skill;
    }

    private void SkillEffect(Skills sE)
    {
        switch (sE)
        {
            case Skills.rotateRight:
                if (sunny.rots < 3) sunny.RotateSunny();
                break;
            case Skills.rotateLeft:
                sunny.RotateSunny(-90f);
                break;
            case Skills.reverseTemp:
                GameController.G.result -= t;
                ChangeTemp();
                break;
            default:
                break;
        }
        if (sE != Skills.doubleUse) ActiveButton(false);
        SetSkill();
    }
    
    public void SetTemperature(int setTemp = 0)
    {
        t = setTemp;
        spriteRenderers[2].sprite = GameController.G.raySprites[t];
        if (t < 0) spriteRenderers[3].sprite = GameController.G.raySprites[20];
    }

    public void ActiveButton(bool b)
    {
        act = b;
        SetTemperature();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameController.G.phase != GamePhase.level) if (!_isCopy || !act) return;
        sunny = transform.parent.GetComponent<Sunny>();
        GameController.G.result += t;
        SkillEffect(skill); 
        if (_isCopy) sunny.DestroyCopy();
        if (GameController.training < 10) GameController.G.TextView("Если не хватило заряда, нажми на солнышко");
    }

    private void Cancel()
    { 
        if (!_isCopy)
        {
            ActiveButton(true);
            SetTemperature(startTemp);
            skill = SetSkill(startSkill);
        } 
    }

    public bool _isCopy = false;
    RaySunny left;
    RaySunny right;

    private void ChangeTemp()
    {
        GameController.G.phase = GamePhase.pause;
        
        left = Instantiate(sunny.prefab, new Vector3(transform.position.x - 0.5f, transform.position.y + 0.5f, 0f), Quaternion.identity, transform.parent);
        left.transform.localScale = transform.localScale * 1.2f;
        left.SetTemperature(-t);
        left.SetSkill(Skills.rotateLeft);
        left._isCopy = true;
        sunny.InactiveRays();
        right = Instantiate(sunny.prefab, new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, 0f), Quaternion.identity, transform.parent);
        right.transform.localScale = transform.localScale * 1.2f;
        right.SetTemperature(t);
        right.SetSkill(Skills.rotateRight);
        right._isCopy = true;
    }
    
    public void PopulateSpriteRenderers()
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }
    }

    public void SetSortOrder(int sOrd)
    {
        PopulateSpriteRenderers();

        foreach (SpriteRenderer tSR in spriteRenderers)
        {
            if (tSR.gameObject == gameObject)
            {
                tSR.sortingOrder = sOrd;
                continue;
            }

            switch (tSR.gameObject.name)
            {
                case "bg":
                    tSR.sortingOrder = sOrd + 1;
                    break;
                case "number":
                    tSR.sortingOrder = sOrd + 2;
                    break;
                case "minus":
                    tSR.sortingOrder = sOrd + 3;
                    break;
                case "skill":
                    tSR.sortingOrder = sOrd + 4;
                    break;
        }
    }

    public void SetSortingLayerName(string tSLN)
    {
        PopulateSpriteRenderers();

        foreach (SpriteRenderer tSR in spriteRenderers)
        {
            tSR.sortingLayerName = tSLN;
        }
    }
}
