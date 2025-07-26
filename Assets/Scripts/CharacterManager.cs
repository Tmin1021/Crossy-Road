using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public CharacterCollection charCollection;
    public SpriteRenderer spriteRendered;
    private int selectedCharIdx = 0;
    void Start()
    {
        // Initialize by showing the first character
        renderCharacterByIdx(selectedCharIdx);
    }

    private void renderCharacterByIdx(int selectedCharIdx)
    {
        Character character = charCollection.GetCharacter(selectedCharIdx);
        spriteRendered.sprite = character.characterSprite;
    }

    public void nextChar()
    {
        selectedCharIdx++;
        if (selectedCharIdx >= charCollection.countCharacter)
            selectedCharIdx = 0;

        renderCharacterByIdx(selectedCharIdx);
    }
    
    public void previousChar()
    {
        selectedCharIdx--;
        if (selectedCharIdx < 0)
            selectedCharIdx = charCollection.countCharacter - 1;

        renderCharacterByIdx(selectedCharIdx);
    }

    public int GetSelectedCharacterIndex()
    {
        return selectedCharIdx;
    }
}
