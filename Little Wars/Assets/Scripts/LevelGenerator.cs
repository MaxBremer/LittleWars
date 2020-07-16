using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public BaseUnit[] unitList;
    public int unitOptionsCount;
    public int[] unitOptionChances;

    public BaseUnit[] SaiCounters;
    public BaseUnit[] JennCounters;

    const bool genRandChances = false;

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

        

        GenLevel ret = new GenLevel();

        //Choose number of friendlies and enemies
        int numEnemies = Random.Range(2, 7);
        difficulty -= numEnemies * enemyCountWeight;
        int numFriendlies;
        if(numEnemies > 4)
        {
            numFriendlies = Random.Range(4, 7);
        }
        else
        {
            numFriendlies = Random.Range(2, 7);
        }
        difficulty += numFriendlies * friendlyCountWeight;
        ret.enemySlotNum = numEnemies;
        ret.friendlySlotNum = numFriendlies;

        

        //Setup chances of different enemies
        //for now, its a simple and dumb system.
        if (genRandChances)
        {
            unitOptionChances = new int[unitOptionsCount];

            for (int i = 0; i < unitOptionsCount; i++)
            {
                if (unitList[i].difficultyFactor * numEnemies > difficulty)
                {
                    unitOptionChances[i] = Random.Range(1, 4);
                }
                else
                {
                    unitOptionChances[i] = Random.Range(3, 8);
                }
            }
        }

        //Setup enemy board
        //NOTE: At this point, chances of various units should be set based on difficulty remaining
        difficulty -= chooseEnemiesForCount(ret, numEnemies);



        //choose available units in market
        //First, choose how many options there are.
        int numAvailableInMarket = Random.Range(2, unitOptionsCount);
        //int numAvailableInMarket = 2;
        ret.availableInMarket = new BaseUnit[numAvailableInMarket];
        //Then, choose what the options actually are.
        List<BaseUnit> optionsRemaining = new List<BaseUnit>();
        optionsRemaining.AddRange(unitList);
        for (int i = 0; i < numAvailableInMarket; i++)
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
            ret.marketUnitChances[i] = (int)(10/ret.availableInMarket[i].difficultyFactor);
        }

        //choose number of shop slots
        int numSlotsAvailable = Random.Range(3, 14);
        difficulty += numSlotsAvailable * marketCountWeight;
        ret.numShopSlots = numSlotsAvailable;

        //If the enemy has a Jenn, the market MUST contain at least one unit capable of damaging her, e.g. it cannot be a market full of Dans.
        if(hasType("Jenn", ret.enemyUnits))
        {
            marketMustContain(ret.availableInMarket, JennCounters);
        }
        //If the enemy has a Sai, the market MUST contain either a Sai, Val, or Mark.
        if (hasType("Sai", ret.enemyUnits)){
            marketMustContain(ret.availableInMarket, SaiCounters);
        }

        //Finally, choose the starting ctrl at a value that attempts to offset remaining difficulty.
        if (difficulty < ctrlBoostOffset)
        {
            ret.startingCtrl = Random.Range(10, 30);
        }
        else
        {
            ret.startingCtrl = Random.Range(3, 10);
        }
        difficulty += ret.startingCtrl * ctrlWeight;

        //NEW METHOD FOR CTRL
        //Add exactly enough to offset.
        /*if(difficulty < ctrlBoostOffset)
        {
            ret.startingCtrl = Mathf.CeilToInt((ctrlBoostOffset - difficulty)/ctrlWeight);
        }*/

        /*Debug.Log("Post-generation, remaining difficulty was as follows.");
        Debug.Log(difficulty);
        Debug.Log("(hopefully should be close to zero)");*/

        /*Debug.Log("Units available:");
        foreach(BaseUnit unit in ret.availableInMarket)
        {
            Debug.Log(unit.unitName);
        }
        Debug.Log("Odds:");
        foreach(int x in ret.marketUnitChances)
        {
            Debug.Log("" + x);
        }*/

        return ret;
    }

    bool hasType(string typeName, bUnit[] units)
    {
        foreach(bUnit unit in units)
        {
            if(unit.myName == typeName)
            {
                return true;
            }
        }
        return false;
    }

    float chooseEnemiesForCount(GenLevel ret, int numEnemies)
    {
        float totDifficultyOfBoard = 0;

        ret.enemyUnits = new bUnit[numEnemies];
        for (int i = 0; i < numEnemies; i++)
        {
            BaseUnit typeToDeploy = chooseEnemy();
            /*GameObject tempUnitObj = Instantiate(gc.basicUnit, new Vector3(100, 100, 100), Quaternion.identity);
            tempUnitObj.GetComponent<Unit>().assignType(typeToDeploy);*/
            ret.enemyUnits[i] = new bUnit(typeToDeploy);
            totDifficultyOfBoard += ret.enemyUnits[i].myType.difficultyFactor;
        }
        return totDifficultyOfBoard;
    }

    void marketMustContain(BaseUnit[] marketOpts, BaseUnit[] typesMustHave)
    {
        bool hasIt = false;
        for(int i = 0; i < typesMustHave.Length; i++)
        {
            hasIt = hasIt || marketOpts.ToList<BaseUnit>().Contains(typesMustHave[i]);
        }

        if (!hasIt)
        {
            marketOpts[Random.Range(0, marketOpts.Length)] = typesMustHave[Random.Range(0,typesMustHave.Length)];
        }
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
