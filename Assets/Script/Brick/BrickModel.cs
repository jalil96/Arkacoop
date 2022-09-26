using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class BrickModel : MonoBehaviourPun
{
    [SerializeField] private int maxHits;
    [SerializeField] private int _hits;

    private void Start()
    {
        _hits = maxHits;
    }
    
    public void Damage(int amount)
    {
        _hits -= amount;
        if (_hits <= 0)
        {
            PhotonNetwork.Destroy(gameObject);
            return;
        }
        photonView.RPC(nameof(UpdateHits), RpcTarget.All, _hits);
    }

    [PunRPC]
    private void UpdateHits(int hits)
    {
        _hits = hits;
    }
    
}
