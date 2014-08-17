using UnityEngine;
using System.Collections;

public class InGame : MonoBehaviour {

	public Transform playerPrefab;
    private bool spawned;

    private GameObject ply;

	public void Awake()
	{
		// in case we started this demo with the wrong scene being active, simply load the menu scene
		if (!PhotonNetwork.connected)
		{
			Application.LoadLevel(Lobby.SceneNameMenu);
			return;
		}

		// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
		//PhotonNetwork.Instantiate(this.playerPrefab.name, transform.position, Quaternion.identity, 0);
	}

	public void OnGUI()
	{
		if (GUILayout.Button("Return to Lobby"))
		{
			PhotonNetwork.LeaveRoom();  // we will load the menu level when we successfully left the room
		}

        if (!spawned)
        {
            GUI.skin.box.fontStyle = FontStyle.Bold;
            GUI.Box(new Rect((Screen.width - 400) / 2, (Screen.height - 350) / 2, 400, 300), "Select your team");

            GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 350) / 2, 400, 300));

            GUILayout.Space(75);

            // Player name
            GUILayout.BeginHorizontal();
            GUILayout.Space(75);
            if (GUILayout.Button("BLU", GUILayout.Width(70)))
            {
                spawned = true;
                ply = PhotonNetwork.Instantiate(this.playerPrefab.name, transform.position, Quaternion.identity, 0) as GameObject;
                ply.GetComponent<Player>().photonView.RPC("SetTeam", PhotonTargets.AllBuffered, 0);

            }
            GUILayout.Space(50);
            if (GUILayout.Button("RED", GUILayout.Width(70)))
            {
                spawned = true;
               ply = PhotonNetwork.Instantiate(this.playerPrefab.name, transform.position, Quaternion.identity, 0) as GameObject;
               ply.GetComponent<Player>().photonView.RPC("SetTeam", PhotonTargets.AllBuffered, 1);

            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }


	}

	public void OnMasterClientSwitched(PhotonPlayer player)
	{
		Debug.Log("OnMasterClientSwitched: " + player);

		string message;
		InRoomChat chatComponent = GetComponent<InRoomChat>();  // if we find a InRoomChat component, we print out a short message

		if (chatComponent != null)
		{
			// to check if this client is the new master...
			if (player.isLocal)
			{
				message = "You are Master Client now.";
			}
			else
			{
				message = player.name + " is Master Client now.";
			}


			chatComponent.AddLine(message); // the Chat method is a RPC. as we don't want to send an RPC and neither create a PhotonMessageInfo, lets call AddLine()
		}
	}

	public void OnLeftRoom()
	{
		Debug.Log("OnLeftRoom (local)");

		// back to main menu        
		Application.LoadLevel(Lobby.SceneNameMenu);
	}

	public void OnDisconnectedFromPhoton()
	{
		Debug.Log("OnDisconnectedFromPhoton");

		// back to main menu        
		Application.LoadLevel(Lobby.SceneNameMenu);
	}



	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		Debug.Log("OnPhotonInstantiate " + info.sender);    // you could use this info to store this or react
       // Debug.Log(info.sender.name);
	}

	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		Debug.Log("OnPhotonPlayerConnected: " + player);
	}

	public void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		Debug.Log("OnPlayerDisconneced: " + player);
	}

	public void OnFailedToConnectToPhoton()
	{
		Debug.Log("OnFailedToConnectToPhoton");

		// back to main menu        
		Application.LoadLevel(Lobby.SceneNameMenu);
	}



}
