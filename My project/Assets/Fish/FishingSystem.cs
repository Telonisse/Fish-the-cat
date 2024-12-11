using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WaterSource
{
    Ocean,
    Bay,
    Mangrove
}
public class FishingSystem : MonoBehaviour
{
    public static FishingSystem Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public List<FishData> OceanFishList;
    public List<FishData> BayFishList;
    public List<FishData> MangroveFishList;

    public bool isThereABite;
    bool hasPulled;

    public static event Action OnFishingEnd;

    public GameObject minigame;

    FishData fishBiting;

    public FishingMiniGame fishmovement;

    public bool isCasted;

    private void Update()
    {
        StartFishingMini(WaterSource.Ocean);
    }

    internal void StartFishing(WaterSource waterSource)
    {
        StartCoroutine(FishingCoroutine(waterSource));
    }

    IEnumerator FishingCoroutine(WaterSource waterSource) // Starts fishing for specfic water 
    {
        yield return new WaitForSeconds(3f); // delay for fishing make it random?
        FishData fish = CalculateBite(waterSource); // what water we're in 

        if (fish.fishName == "NoBite") // no fish but why?, wait longer? 
        {
            Debug.Log("False Fish");
            EndFishing();
        }
        else
        {
            Debug.Log(fish.fishName + "Is Biting");
            StartCoroutine(FishHooked(fish)); // handle fish being hooked 
        }
    }

    void StartFishingMini(WaterSource source) // user input for fishing ( alot needs to be changed here it doesnt make sense )
    {
        // needs to be change to new input system and new inputs conflicts with fishingrod

        if (Input.GetMouseButton(0) && !isCasted)
        {
            //hämtar vilket vatten det är i start
            FishingSystem.Instance.StartFishing(source); // starts fishing 
            isCasted = true;

        }

        if (Input.GetMouseButton(0) && isCasted)
        {
            //hämtar vilket vatten det är i start
            //FishingSystem.Instance.StartFishing(source);
            FishingSystem.Instance.PlayerFishing(); // player pulling line?


        }

        // ifall spelaren trycker 0 ska den dra upp fisken ifall den fiskar
        //if(isCasted && input.getmousebutton(0) && fishingsystem.instance.isthereabite)

        //Fishingame active ^^^ den övre ska trigga den undre
        FishingSystem.Instance.PlayerFishing(); //<<------ triggar data till minigame
        //pause = false;

        if (FishingSystem.Instance.isThereABite) // trigger if theres a bite 
        {
            //alert
            //baitReference.transform.Find("Alert").gameObject.SetActive(true);
        }

    }

    public IEnumerator FishHooked(FishData fish)
    {
        //har nappat
        isThereABite = true;

        

        //utropstecekn om att den är på kroken men spelaren har it reagerat än
        while (!hasPulled) // waiting for player to pull fishing line 
        {
            yield return null;
        }

        Debug.Log("Start MiniGame");
        fishBiting = fish; // store data 
        StartMiniGame();

    }

    private void StartMiniGame()
    {
        //minigame.SetActive(true);
        Debug.Log("Starting MiniGame"); // might just be catch fish thing for now you catched it here
        //fishmovement.SetDifficulty(fishBiting);
    }

    public void PlayerFishing()
    {
        hasPulled = true;
    }

    private void EndFishing() // resets variables 
    {
        isThereABite = false;
        hasPulled = false;

        fishBiting = null;

        OnFishingEnd?.Invoke();

        //Trigger End FishingEvent

    }

    internal void CatchFish(WaterSource waterSource) // simulates catching fish
    {
        FishData caughtFish = CalculateBite(waterSource);

        if (caughtFish != null && caughtFish.fishName != "NoBite")
        {
            Debug.Log("You caught a: " + caughtFish.fishName);

            //ShowCaughtFishUI(caughtFish);
        }
        else
        {
            Debug.Log("No fish took the bait!");
        }
    }

    //private FishData CalculateBite(WaterSource waterSource)
    //{
    //    List<FishData> availableFish = GetAvailableFish(waterSource);

    //    //total probability
    //    float totalProbability = 0f;
    //    foreach(FishData fish in availableFish)
    //    {
    //        totalProbability += fish.probability;
    //    }

    //    //Generates random number between 0 and total probability
    //    int randomValue = UnityEngine.Random.Range(0, Mathf.FloorToInt(totalProbability) + 1);
    //    Debug.Log("Random value is" + randomValue);

    //    //loop through fish for probability
    //    float cumulativeProbability = 0f;

    //    foreach(FishData fish in availableFish)
    //    {
    //        cumulativeProbability += fish.probability;
    //        if(randomValue <= cumulativeProbability)
    //        {
    //            return fish;
    //        }
    //    }

    //    //säg walla
    //    return null;

    //}

    private FishData CalculateBite(WaterSource waterSource) // gets what fish if any takes the bite 
    {
        List<FishData> availableFish = GetAvailableFish(waterSource); // gets list for water source 
        if (availableFish == null || availableFish.Count == 0)
        {
            return null;
        }

        float totalProbability = 0f;
        foreach (FishData fish in availableFish)
        {
            totalProbability += fish.probability;
        }

        // random number for random fish 
        int randomValue = UnityEngine.Random.Range(0, Mathf.FloorToInt(totalProbability) + 1);
        float cumulativeProbability = 0f;

        foreach (FishData fish in availableFish)
        {
            cumulativeProbability += fish.probability;
            if (randomValue <= cumulativeProbability)
            {
                return fish; // gets the fish that matches the random value 
            }
        }

        return null; // no fish
    }

    private List<FishData> GetAvailableFish(WaterSource waterSource) // list based on water source 
    {
        switch (waterSource)
        {
            case WaterSource.Ocean:
                return OceanFishList;
            case WaterSource.Bay:
                return BayFishList;
            case WaterSource.Mangrove:
                return MangroveFishList;

            default:
                return null;
        }
    }

    internal void EndMinigame(bool sucess) // ends minigame, idk about it delete?
    {
        minigame.gameObject.SetActive(false);

        if (sucess)
        {

            Debug.Log("Fish Caught" + fishBiting.fishName); //<-- testa koppla detta till UI, ifall funkar gör så med resterande
            EndFishing();
        }
        else
        {
            EndFishing();
        }
    }
}


