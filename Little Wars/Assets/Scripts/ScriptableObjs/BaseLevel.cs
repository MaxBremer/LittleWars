using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level")]
public class BaseLevel : ScriptableObject
{
    public int levelNum;
    public string levelName;
    public string levelDesc;
    public int startingCtrl;


    public int friendlySlotNum;
    public int enemySlotNum;

    public BaseUnit[] enemyUnits;

    //SIMPLE market randomizer system, update later.
    public BaseUnit[] marketUnits;
    public int[] marketChances;
    public int numMarketSlots;
}
