using UnityEngine;

// --- On déplace ces définitions ici ---
// Elles n'ont plus besoin d'être dans DialogueManager.cs
public enum Speaker
{
    Player,
    PlayerThought,
    PlayerCinematic,
    PlayerTwo,
    Entity,
    Narrator
}

[System.Serializable]
public class DialogueLine
{
    public Speaker speaker;
    [TextArea(3, 5)]
    public string sentence;
}
// -----------------------------------------


// [CreateAssetMenu] est la partie magique.
// Il ajoutera une nouvelle option dans le menu "Create" d'Unity.
[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/New Dialogue")]
public class Dialogue : ScriptableObject
{
    // Ce ScriptableObject contient simplement notre ancien tableau
    public DialogueLine[] lines;
}