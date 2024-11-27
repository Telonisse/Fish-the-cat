using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static Unity.VisualScripting.Member;

public class FishingMiniGame : MonoBehaviour
{
 
    [Tooltip("Important Transforms")]
    [SerializeField] Transform topPiv;
    [SerializeField] Transform bottomPiv;
    [SerializeField] Transform fish;
    [SerializeField] Transform hook;


    [Tooltip("Fish Movement")]
    [SerializeField] float timeMult = 3.0f; //fishMovement fishdata
    [SerializeField] float smoothMotion = 1.0f; // fishSpeed fishdata

    [Tooltip("Hook")]
    [SerializeField] float hookSize = 0.1f;
    [SerializeField] float hookPower = 0.5f;
    [SerializeField] float hookPullPW = 0.01f;
    [SerializeField] float hookGravityPW = 0.005f;
    [SerializeField] float hookProgressDegradationPW = 0.1f;
    [SerializeField] SpriteRenderer hookSprite;

    [SerializeField] Transform ProgressBarContainer;

    [SerializeField] float failtTime = 10.0f;

    GameObject baitReference;

    float fishPos;
    float fishDir;
    float fishSpeed;
    float fishTime;

    float hookPos;
    float hookProgress;
    float hookPullVel;

    bool pause = false;
    bool isCasted;
    bool isPulling;

    FishArea fishArea;

    private void Start()
    {
        Resize();
        fishArea = FindObjectOfType<FishArea>();
        WaterSource source = fishArea.waterSource;


        //behöver hitta source någonstans?
        StartFishingMini(source);
    }

   

    private void Update()
    {


        if (pause) { return; }

        Fish();
        Hook();
        ProgressCheck();

    }

    internal void SetDifficulty(FishData fishBiting) // behöver hjälp med denna ifall man kan bara sätta in värdena istället eller ifall det är lättare o göra en switch
    {
        switch(fishBiting.fishDifficulty)
        {
            case 1:
                timeMult = 1;
                return;
            case 2:
                timeMult = 2;
                return;
            case 3:
                timeMult = 3;
                return;
        }
    }

    void Resize()
    {
        Bounds b = hookSprite.bounds;
        float ySize = b.size.y;
        Vector3 ls = hook.localScale;
        float distance = Vector3.Distance(topPiv.position, bottomPiv.position);
        ls.y = (distance / ySize * hookSize);
        hook.localScale = ls;
    }

    void Fish()
    {
        fishTime -= Time.deltaTime;

        if (fishTime < 0)
        {
            fishTime = UnityEngine.Random.value * timeMult;

            fishDir = UnityEngine.Random.value;

            Debug.Log("Changing Direction");

        }

        fishPos = Mathf.SmoothDamp(fishPos, fishDir, ref fishSpeed, smoothMotion);
        fish.position = Vector3.Lerp(bottomPiv.position, topPiv.position, fishPos);
    }

    void Hook()
    {
        if(Input.GetMouseButton(0))
        {
            hookPullVel += hookPullPW * Time.deltaTime;
        }

        hookPullVel -= hookGravityPW * Time.deltaTime;

        hookPos += hookPullVel;

        if(hookPos - hookSize/2 <= 0f && hookPullVel < 0f)
        {
            hookPullVel = 0f;
        }

        if(hookPos + hookSize/2 >= 1f && hookPullVel > 0f)
        {
            hookPullVel = 0f;
        }

        hookPos = Mathf.Clamp(hookPos, hookSize/2, 1 - hookSize/2);
        hook.position = Vector3.Lerp(bottomPiv.position,topPiv.position, hookPos);
    }

    void StartFishingMini(WaterSource source)
    {


        if (Input.GetMouseButton(0) && !isCasted && isPulling)
        {
            //hämtar vilket vatten det är i start
            FishingSystem.Instance.StartFishing(source);

        }

             // ifall spelaren trycker 0 ska den dra upp fisken ifall den fiskar
             // if(isCatsed && input.getmousebutton(0) && fishingsystem.instance.isthereabite)

            //Fishingame active ^^^ den övre ska trigga den undre
            //FishingSystem.Instance.PlayerFishing(); <<------ triggar data till minigame
            //pause = false;

        if (FishingSystem.Instance.isThereABite)
        {
            //alert
            baitReference.transform.Find("Alert").gameObject.SetActive(true);  
        }

    }
    void ProgressCheck()
    {
        Vector3 ls = ProgressBarContainer.localScale;
        ls.y = hookProgress;
        ProgressBarContainer.localScale = ls;


        float min = hookPos - hookSize / 2;
        float max = hookPos + hookSize / 2;

        if (min < fishPos && fishPos < max)
        {
            hookProgress += hookPower * Time.deltaTime;
        }
        else
        {
            hookProgress -= hookProgressDegradationPW * Time.deltaTime;

            failtTime -= Time.deltaTime;
            if(failtTime < 0f)
            {
                Lose();
            }

        }

        if (hookProgress >= 1f)
        {
            Win();
        }

        hookProgress = Mathf.Clamp(hookProgress, 0f, 1f);
    }

    void Win()
    {

        FishingSystem.Instance.EndMinigame(true); //UI Popup kopplas i fishingsystem (kanske)

        pause = true;
        Debug.Log("You Catched Fish!");

        hookProgress = 0f;  
        
    }

    void Lose()
    {
        FishingSystem.Instance.EndMinigame(false);

        pause = true;
        Debug.Log("Womp Womp, You Lost The Fish");

        hookProgress = 0f;
    }

    private void OnEnable()
    {
        FishingSystem.OnFishingEnd += HandleFishinEnd;
    }

    private void OnDestroy()
    {
        FishingSystem.OnFishingEnd -= HandleFishinEnd;
    }

    public void HandleFishinEnd()
    {
        //invokar att bait är i vattnet?+
        Destroy(baitReference); //kolla här , i tutorial så vill man ta död på bait, det ska inte jag så behöver it detta fr
    }

}
