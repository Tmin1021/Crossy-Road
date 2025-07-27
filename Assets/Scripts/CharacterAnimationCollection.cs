using UnityEngine;

[System.Serializable]
public class CharacterAnimationSet
{
    public string characterName;
    public Sprite idleSprite;
    public Sprite backSprite;
    public Sprite frontSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;
    public Sprite dieSprite;
}

[CreateAssetMenu(fileName = "CharacterAnimations", menuName = "Game/Character Animation Collection")]
public class CharacterAnimationCollection : ScriptableObject
{
    public CharacterAnimationSet[] characterAnimations;
    
    public CharacterAnimationSet GetCharacterAnimationSet(int index)
    {
        if (index >= 0 && index < characterAnimations.Length)
        {
            return characterAnimations[index];
        }
        return null;
    }
    
    public CharacterAnimationSet GetCharacterAnimationSet(string characterName)
    {
        foreach (var animSet in characterAnimations)
        {
            if (animSet.characterName.Equals(characterName, System.StringComparison.OrdinalIgnoreCase))
            {
                return animSet;
            }
        }
        return null;
    }
}
