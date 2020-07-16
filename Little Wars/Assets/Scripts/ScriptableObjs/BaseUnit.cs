using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Unit")]
public class BaseUnit : ScriptableObject
{
    public string unitName;
    public string desc;

    public float difficultyFactor;

    public int health;
    public int attack;
    public int defense;

    public int buyCost;
    public int deployCost;
    
    public Mesh mesh;
    public Material unitMat;
    public Sprite menuImage;

    public dataType myDataType;
}

//IF NEW METADATA, ADD TO THIS ENUM
public enum dataType { basic, swipe, poison, immune, dodge }
