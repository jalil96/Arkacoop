using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;


public class PlayerData
{
    public int score;
    public string nickname;
}

public class ScoreManager : MonoBehaviour
{
    public PlayerScore playerScorePrefab;
    public GameObject playerListContainer;
    public Button mainMenuButton;

    public Text titleTxt;

    private List<PlayerScore> playerScoreList;

    private void Awake()
    {
        playerScorePrefab.gameObject.SetActive(false);
    }

    public void SetTitle(string title)
    {
        titleTxt.text = title;
    }

    public void SetPlayersPrefab(int quantityPlayer)
    {
        playerScorePrefab.gameObject.SetActive(false);
        for (int i = 0; i < quantityPlayer; i++)
        {
            PlayerScore aux = Instantiate(playerScorePrefab, playerListContainer.transform);
            aux.gameObject.SetActive(false);
            playerScoreList.Add(aux);
        }
    }

    public void SetScores(List<PlayerData> playerList)
    {
        IEnumerable orderedList =  playerList.OrderBy(o => o.score);

        List<PlayerData> auxList =  new List<PlayerData>();

        foreach (PlayerData item in orderedList)
        {
            auxList.Add(item);
        }

        RefreshList(auxList);
    }

    public void RefreshList(List<PlayerData> playerList)
    {
        for (int i = 0; i < playerScoreList.Count; i++)
        {
            if((playerList.Count() - 1) < i)
            {
                playerScoreList[i].gameObject.SetActive(false);
                continue;
            }

            playerScoreList[i].gameObject.SetActive(true);
            playerScoreList[i].positionTxt.text = $"{i + 1} - ";
            playerScoreList[i].nameTxt.text = playerList[i].nickname;
            playerScoreList[i].scoreTxt.text = playerList[i].score.ToString();
        }
    }
}
