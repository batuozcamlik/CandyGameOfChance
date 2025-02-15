using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMatchData", menuName = "ScriptableObjects/MatchData", order = 1)]

public class CandyScribleObject : ScriptableObject
{
    public string tag;
    public float oneMatch;
    public float twoMatch;
    public float threeMatch;
    public Sprite candySprite;
}
