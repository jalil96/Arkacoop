using Photon.Pun;
using UnityEngine;

public class CharacterModel : MonoBehaviourPun
{
    [SerializeField] private float _speed;

    public void Move(Vector2 direction)
    {
        direction = direction.normalized * _speed;
        transform.Translate(direction.x, direction.y, transform.position.z);
    }

}
