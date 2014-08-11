﻿using UnityEngine;
using System.Collections;

public class Catch : Photon.MonoBehaviour {
	public Player ply;
	void Awake() {
		
	//	ply = this.transform.parent.gameObject.GetComponent<Player>();
	}
	void OnTriggerEnter(Collider col) {
	//if(ply.photonView.isMine){
		if(col.gameObject.tag == "Ball") {
		
		if(col.gameObject.GetComponent<Ball>().state == Ball.BallState.FreeFlight){
			PhotonNetwork.Destroy(col.gameObject);
			ply.photonView.RPC("AddBall",PhotonTargets.All);
			
	//	}
			
		}
	}
	}

}