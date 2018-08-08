
using UnityEngine;
using UnityEngine.Networking;

public class CharacterManipulator : NetworkBehaviour
{
    public Character Target
    {
        get
        {
            return _target;
        }
        set
        {
            if (value == _target)
                return;

            if (value == null)
            {
                _target.AssignManipulator(null);
            }
            else
            {
                if(_target != null)
                    _target.AssignManipulator(null);
                value.AssignManipulator(this);
            }
            _target = value;
        }
    }
    [SerializeField]
    private Character _target;
}
