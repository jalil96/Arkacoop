using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private bool _flip;
    [SerializeField] private bool _vertical;

    public bool Flip => _flip;
    public bool Vertical => _vertical;
}