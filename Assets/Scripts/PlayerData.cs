using UnityEngine;

[System.Serializable]
public class Character
{
    public string characterName;
    public Sprite characterSprite;
    public RuntimeAnimatorController animatorController;
    
    [Header("Unlock System")]
    public bool isUnlockedByDefault = false; 
    public int unlockCost = 100;              
}
