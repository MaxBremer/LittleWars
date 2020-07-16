using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bUnit
{
    public string nickName;
    public string myName;

    public AttackRule myRule;
    public Metadata myData;

    public BaseUnit myType;
    public bool isFriendly;

    public Mesh myMesh;
    public Material myMat;

    public int curHealth;
    public int maxHealth;
    public int curAtk;
    public int curDef;

    public int curBuyCost;
    public int curDeployCost;

    public int timesBuffed;

    public bUnit() { }
    public bUnit(BaseUnit unitType)
    {
        timesBuffed = 0;

        myType = unitType;

        myName = unitType.unitName;
        myRule = AttackRule.random;

        curHealth = unitType.health;
        maxHealth = curHealth;
        curAtk = unitType.attack;
        curDef = unitType.defense;

        curBuyCost = unitType.buyCost;
        curDeployCost = unitType.deployCost;

        myMesh = unitType.mesh;
        myMat = unitType.unitMat;

        //idleAnim = unitType.idleAnimation;

        switch (unitType.myDataType)
        {
            case dataType.basic:
                myData = new Metadata();
                break;
            case dataType.poison:
                myData = new poisonData();
                break;
            case dataType.swipe:
                myData = new bobData();
                break;
            case dataType.immune:
                myData = new poisonHealData();
                break;
            case dataType.dodge:
                myData = new dodgeData();
                break;
            default:
                Debug.Log("ERROR: unrecognized datatype from BaseUnit");
                break;
        }
    }

    public int chooseTarget(bUnit[] opts)
    {
        switch (myRule)
        {
            case AttackRule.random:
                return Random.Range(0, opts.Length);
            default:
                Debug.Log("ERROR: Unrecognized attack rule.");
                return -1;
        }
    }

    public void setHealth(int val)
    {
        curHealth = val;
        //refreshText();
        //checkDeath();
    }

    public void atkDamage(int atk)
    {
        curHealth = curHealth - Mathf.Max(atk - curDef, 0);
        //refreshText();
        //checkDeath();
    }

    public bool isDead()
    {
        return curHealth <= 0;
    }
}

public enum AttackRule { random, leftToRight, leftMost, rightMost }
