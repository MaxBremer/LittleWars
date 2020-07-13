using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public BaseUnit[] unitList;
    public int unitOptionsCount;
    public int[] unitOptionChances;

    GameController gc;

    const float enemyCountWeight = 1.5f;
    const float friendlyCountWeight = 1.5f;
    const float marketCountWeight = .7f;
    const float ctrlWeight = .5f;
    const float ctrlBoostOffset = 10f;
    

    // Start is called before the first frame update
    void Start()
    {
        gc = gameObject.GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GenLevel generateLevel(float difficulty)
    {
        //This function works off the premise that we're trying to get the difficulty
        //float as close to zero as possible. Adding difficult things will SUBTRACT from said
        //float, while making things easier will ADD to the float.
        //I will make this better in the future I promise.

        unitOptionChances = new int[unitOptionsCount];

        GenLevel ret = new GenLevel();

        //Choose number of friendlies and enemies
        int numEnemies = Random.Range(2, 7);
        difficulty -= numEnemies * enemyCountWeight;
        int numFriendlies = Random.Range(2, 7);
        difficulty += numFriendlies * friendlyCountWeight;
        ret.enemySlotNum = numEnemies;
        ret.friendlySlotNum = numFriendlies;

        //choose available units in market
        //First, choose how many options there are.
        int numAvailableInMarket = Random.Range(1, unitOptionsCount);
        ret.availableInMarket = new BaseUnit[numAvailableInMarket];
        //Then, choose what the options actually are.
        List<BaseUnit> optionsRemaining = new List<BaseUnit>();
        optionsRemaining.AddRange(unitList);
        for(int i = 0; i < numAvailableInMarket; i++)
        {
            BaseUnit choice = optionsRemaining[Random.Range(0, optionsRemaining.Count)];
            ret.availableInMarket[i] = choice;
            optionsRemaining.Remove(choice);
            difficulty += choice.difficultyFactor;
        }
        //Finally, choose the shop chance for each unit
        //Could be done in the above loop, but I like splitting them up for clarity.
        ret.marketUnitChances = new int[numAvailableInMarket];
        for(int i = 0; i < numAvailableInMarket; i++)
        {
            //TODO: CHANGE.
            ret.marketUnitChances[i] = 1;
        }

        //choose number of shop slots
        int numSlotsAvailable = Random.Range(2, 14);
        difficulty += numSlotsAvailable * marketCountWeight;
        ret.numShopSlots = numSlotsAvailable;

        //Setup chances of different enemies
        //for now, its a simple and dumb system.
        for(int i = 0; i < unitOptionsCount; i++)
        {
            if(unitList[i].difficultyFactor * numEnemies > difficulty)
            {
                unitOptionChances[i] = Random.Range(1, 4);
            }
            else
            {
                unitOptionChances[i] = Random.Range(3, 8);
            }
        }

        //Setup enemy board
        //NOTE: At this point, chances of various units should be set based on difficulty remaining
        difficulty -= chooseEnemiesForCount(ret, numEnemies);

        //Finally, choose the starting ctrl at a value that attempts to offset remaining difficulty.
        if (difficulty < ctrlBoostOffset)
        {
            ret.startingCtrl = Random.Range(10, 30);
        }
        else
        {
            ret.startingCtrl = Random.Range(0, 8);
        }
        difficulty += ret.startingCtrl * ctrlWeight;

        //NEW METHOD FOR CTRL
        //Add exactly enough to offset.
        /*if(difficulty < ctrlBoostOffset)
        {
            ret.startingCtrl = Mathf.CeilToInt((ctrlBoostOffset - difficulty)/ctrlWeight);
        }*/

        Debug.Log("Post-generation, remaining difficulty was as follows.");
        Debug.Log(difficulty);
        Debug.Log("(hopefully should be close to zero)");

        return ret;
    }

    float chooseEnemiesForCount(GenLevel ret, int numEnemies)
    {
        float totDifficultyOfBoard = 0;

        ret.enemyUnits = new Unit[numEnemies];
        for (int i = 0; i < numEnemies; i++)
        {
            BaseUnit typeToDeploy = chooseEnemy();
            GameObject tempUnitObj = Instantiate(gc.basicUnit, new Vector3(100, 100, 100), Quaternion.identity);
            tempUnitObj.GetComponent<Unit>().assignType(typeToDeploy);
            ret.enemyUnits[i] = tempUnitObj.GetComponent<Unit>();
            totDifficultyOfBoard += ret.enemyUnits[i].unitType.difficultyFactor;
        }
        return totDifficultyOfBoard;
    }

    BaseUnit chooseEnemy()
    {
        int chanceTot = 0;
        foreach(int opt in unitOptionChances)
        {
            chanceTot += opt;
        }
        int choice = Random.Range(0, chanceTot);
        int tempTot = 0;
        for(int i = 0; i < unitOptionsCount; i++)
        {
            tempTot += unitOptionChances[i];
            if(choice < tempTot)
            {
                return unitList[i];
            }
        }
        Debug.LogError("Generator did not choose an enemy somehow.");
        return null;
    }
}
