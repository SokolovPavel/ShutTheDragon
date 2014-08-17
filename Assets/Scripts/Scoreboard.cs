using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Scoreboard : Photon.MonoBehaviour {
    public static List<string> Players = new List<string>();
    public static List<int> Scores = new List<int>();
    public static List<GameObject> Labels = new List<GameObject>();
    public GameObject Label;

	// Use this for initialization
	void Awake () {
	    foreach(GameObject ply in GameObject.FindGameObjectsWithTag("Player"))
        {
             Players.Add(ply.GetComponent<Player>().photonView.owner.name);
             int index = Players.IndexOf(ply.GetComponent<Player>().photonView.owner.name);
             Scores.Add(0);
             Labels.Add(Instantiate(Label, this.transform.position + new Vector3(0.55f,4- index*1.2f, -7.5f), Quaternion.Euler(0,-90,0)) as GameObject);
             Labels[index].GetComponent<TextMesh>().text = Players[index] + "   " + Scores[index];

        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        Players.Add(player.name);
       int index = Players.IndexOf(player.name);
        Scores.Add(0);

        Labels.Add(Instantiate(Label, this.transform.position + new Vector3(0.55f,4- index*1.2f, -7.5f), Quaternion.Euler(0, -90, 0)) as GameObject);
        Labels[index].GetComponent<TextMesh>().text = Players[index] + "   " + Scores[index];

    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i] == player.name)
            {
                Players.RemoveAt(i);
                Scores.RemoveAt(i);
                Labels.RemoveAt(i);
            }
        }
    }


    public static void AddScore(string plyName)
    {
        for(int i=0;i<Players.Count;i++)
        {
            if (Players[i] == plyName)
            {
                Scores[i]++;
                Labels[i].GetComponent<TextMesh>().text = Players[i] + "   " + Scores[i];
            }
        }
    }

}
