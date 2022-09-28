using UnityEngine;

public class WallController : MonoBehaviour
{
    [SerializeField] private WallModel _wallModel;

    public void Activate(bool activate)
    {
        _wallModel.Activate(activate);
    }
}