
using UnityEngine;

[RequireComponent(typeof(CharacterManipulator))]
public class Bot : MonoBehaviour
{
    // A humanoid bot. Uses weapons, moves around, can attack specific targets.

    public CharacterManipulator Manipulator
    {
        get
        {
            if (_charM == null)
                _charM = GetComponent<CharacterManipulator>();
            return _charM;
        }
    }
    private CharacterManipulator _charM;

    public Character Prefab;

    private void Start()
    {
        SpawnCharacter(Vector2.one * 3f);
        Manipulator.Target.Hands.EquipItem(Item.Spawn(0, Vector2.zero));
    }

    public void SpawnCharacter(Vector2 pos)
    {
        if(Manipulator.Target != null)
        {
            Debug.LogError("Cannot spawn new character, manipulator already has a target!");
            return;
        }

        Character spawned = Instantiate(Prefab);
        spawned.transform.position = pos;

        this.Manipulator.Target = spawned;
    }

    private void Update()
    {
        if (Manipulator.Target == null)
            return;

        //var c = Manipulator.Target;
        Manipulator.MovementDirection = new Vector2(1f, 1f);
    }
}