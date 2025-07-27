using UnityEngine;
using UnityEditor;

public class CreateCharacterAnimations
{
    [MenuItem("Tools/Create Character Animations Asset")]
    public static void CreateCharacterAnimationsAsset()
    {
        // Create the asset
        CharacterAnimationCollection newAsset = ScriptableObject.CreateInstance<CharacterAnimationCollection>();
        
        // Initialize with 4 character slots (cat, chicken, fox, lion)
        newAsset.characterAnimations = new CharacterAnimationSet[4];
        
        // Initialize each character slot
        for (int i = 0; i < 4; i++)
        {
            newAsset.characterAnimations[i] = new CharacterAnimationSet();
        }
        
        // Set character names
        newAsset.characterAnimations[0].characterName = "Cat";
        newAsset.characterAnimations[1].characterName = "Chicken";
        newAsset.characterAnimations[2].characterName = "Fox";
        newAsset.characterAnimations[3].characterName = "Lion";
        
        // Save the asset
        AssetDatabase.CreateAsset(newAsset, "Assets/CharacterAnimations.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        // Select the created asset
        Selection.activeObject = newAsset;
        
        Debug.Log("CharacterAnimations asset created at Assets/CharacterAnimations.asset");
        Debug.Log("Don't forget to assign sprites for each character including the new die sprite!");
    }
}
