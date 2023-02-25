using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.SimpleSpinner;
using UnityEngine.EventSystems;

public enum Days: byte
{
    Утро,
    День,
    Вечер,
    Ночь
}

public class Sunny : MonoBehaviour, IPointerClickHandler
{
    public int rots { private set; get; }
    public RaySunny prefab;
    public Transform mutePrefab;
    public Days now = Days.Утро;
    public SimpleSpinner spinner;
    public Sprite[] dayTimes;
    private SpriteRenderer dayTime;
    private List<RaySunny> rays;
    private List<SimpleSpinner> spinners;

    void Start()
    {
        GameController.G.pullNotify += MoveToTarget;
        GameController.G.cancelNotify += Cancel;
        GameController.G.restartNotify += SunLevel;

        dayTime = transform.parent.GetComponent<SpriteRenderer>();

        if (rays == null) rays = new List<RaySunny>();
        if (spinners == null) spinners = new List<SimpleSpinner>();

        Vector3 centerSunny = transform.position;
        for (int i = 0; i < 12; i++)
        {
            int a = i * 30;
            Vector3 pos = CreateCircle(centerSunny, 1.25f, a);
            RaySunny raySunny = Instantiate(prefab, pos, Quaternion.identity);
            raySunny.transform.SetParent(transform);
            rays.Add(raySunny);
            if (i != 0 && i != 1 && i != 11) Instantiate(mutePrefab, pos, Quaternion.identity, transform.parent);
            else 
            {
                spinner = Instantiate(spinner, pos, Quaternion.identity, transform.parent);
                spinners.Add(spinner);
            }
        }
        SunLevel();
    }

    private void SunLevel()
    {
        StartCoroutine(Rotate(90f));
        Now();
    }

    private Vector3 CreateCircle(Vector3 centerSunny, float radius, int a)
    {
        Vector3 pos;
        pos.x = centerSunny.x + radius * Mathf.Sin(a* Mathf.Deg2Rad);
        pos.y = centerSunny.y + radius * Mathf.Cos(a* Mathf.Deg2Rad); 
        pos.z = centerSunny.z;
        return pos;
    }

    public void DestroyCopy()
    {
        foreach(RaySunny rS in GetComponentsInChildren<RaySunny>()) if (rS._isCopy) Destroy(rS.gameObject);
        GameController.G.phase = GamePhase.level;
    }

    public Days Now(int r = 0)
    {
        if (r < 0) r = 0;
        switch (r%4)
        {
            case 0:
                dayTime.sprite = dayTimes[0];
                dayTime.color = Color.yellow;
                now = Days.Утро;
                break;
            case 1:
                dayTime.sprite = dayTimes[1];
                dayTime.color = Color.white;
                now = Days.День; 
                break;
            case 2:
                dayTime.sprite = dayTimes[2];
                dayTime.color = Color.white;
                now = Days.Вечер; 
                break;
            case 3:
                dayTime.sprite = dayTimes[3];
                dayTime.color = Color.white;
                now = Days.Ночь; 
                break;
        }
        
        rots = r;
        GameController.G.sunDays.text = now.ToString();
        return now;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
      if (GameController.G.phase != GamePhase.level) return;
      else RotateSunny();
    }
    
    public void RotateSunny(float angle = 90f)
    {
        if (rots == 3 && angle > 0) GameController.G.Lose();
        else
        {
            if (angle < 0) Now(--rots);
            else Now(++rots);
            StartCoroutine(Rotate(angle));
        }

        if (GameController.training < 15 && rots == 1) GameController.G.TextView("Цель игры - успеть согреть персонажа за сутки");
        else if (GameController.training < 25 && rots == 2) GameController.G.TextView("Чтобы поменять уровень, нажми на кнопку со стрелочками");
        else if (GameController.training < 25 && rots == 3) GameController.G.TextView("Нажми на персонажа, чтобы переиграть именно этот уровень");
    }

    private IEnumerator Rotate(float angle)
    {
        if (GameController.G.phase != GamePhase.complete) GameController.G.phase = GamePhase.pause;
        float duration = 0.5f;
        float startTime = Time.time;

        Quaternion startRotation = transform.localRotation;
        transform.Rotate(transform.forward, angle);
        Quaternion endRotation = transform.localRotation;

        Quaternion startRay = Quaternion.identity;
        Quaternion endRay = Quaternion.identity;
        foreach (RaySunny rS in rays)
        {
            startRay = rS.transform.localRotation;
            rS.transform.Rotate(transform.forward, -angle);
            endRay = rS.transform.localRotation;
        } 

        while (true)
        {
            float step = (Time.time - startTime) / duration;
            if (step >= 1) break;
            transform.localRotation = Quaternion.Slerp(startRotation, endRotation, step);
            foreach (RaySunny rS in rays) rS.transform.localRotation = Quaternion.Slerp(startRay, endRay, step);
            foreach (SimpleSpinner s in spinners) s.gameObject.SetActive(false);
            yield return null;
        }
        transform.localRotation = endRotation;
        foreach (RaySunny rS in rays) rS.transform.localRotation = endRay;
        foreach (SimpleSpinner s in spinners) s.gameObject.SetActive(true);
        if (GameController.G.phase != GamePhase.complete) GameController.G.phase = GamePhase.level;
    }

    private void Cancel()
    {
        StartCoroutine(Rotate(180f));
        Now();
    }

    private void MoveToTarget()
    {
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        float step = 5f * Time.deltaTime;
        GameObject trail = Instantiate(GameController.G.vfx[2], transform.position, Quaternion.identity);
        while (Vector2.Distance(trail.transform.position, GameController.G.target.transform.position) > float.Epsilon)
        {
            trail.transform.position = Vector2.MoveTowards(trail.transform.position, GameController.G.target.transform.position, step);
            yield return null;
        }
        Destroy(trail.gameObject);
        GameController.G.target.SetTemp();
        GameController.G.Win();
    }
}
