using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterCollection : ScriptableObject
{
    public Character[] character;
    public int countCharacter
    {
        get
        {
            return character.Length;
        }
    }
    public Character GetCharacter(int index)
    {
        return character[index];
    }
}
