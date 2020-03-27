using UnityEngine;

[CreateAssetMenu (fileName = "CharacterData", menuName = "FinderDemo/CharacterData", order = 0)]
public class CharacterData : ScriptableObject {
    public string CharacterName;
    public float Health = 100;
    public float Mana = 200;
    public float AttackDamage = 15;
    public float AttackSpeed = .5f;
}