using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class poisonHealData : Metadata
{
    BaseUnit lastOneToHitMe;


    int attackGained = 0;
    int defenseGained = 0;


    public override void onStartFight()
    {
        //base.onStartFight();
    }

    public override void onHitBy(int attackerIndex)
    {
        /*base.onHitBy(attackerIndex);*/
        lastOneToHitMe = myEnemies[attackerIndex].myUnit.myType;
    }

    public override void onAtk(int targetIndex)
    {
        /*base.onAtk(targetIndex);*/
        lastOneToHitMe = myEnemies[targetIndex].myUnit.myType;
    }

    public override void onDeath()
    {
        /*base.onDeath();*/
        if(lastOneToHitMe.unitName == "Sai")
        {
            me.curHealth = me.maxHealth;
            me.curAtk++;
            attackGained++;
            me.curDef++;
            defenseGained++;
        }
    }

    public override void onEndFight()
    {
        /*base.onEndFight();*/
        me.curAtk -= attackGained;
        me.curDef -= defenseGained;
        attackGained = 0;
        defenseGained = 0;
    }

}
