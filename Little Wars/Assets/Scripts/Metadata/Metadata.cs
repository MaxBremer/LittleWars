using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metadata
{
    public Slot[] myFriends;
    public Slot[] myEnemies;
    public bUnit me;
    public int myIndex;

    public Metadata() { }

    public void passInData(Slot[] myFriendst, Slot[] myEnemiest, bUnit met, int index)
    {
        myFriends = myFriendst;
        myEnemies = myEnemiest;
        me = met;
        myIndex = index;
    }

    public virtual void onAtk(int targetIndex)
    {
        
    }

    public virtual void onHitBy(int attackerIndex)
    {

    }

    public virtual void onDeath()
    {

    }

    public virtual void onStartFight()
    {

    }
}
