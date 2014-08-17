using UnityEngine;
using System.Collections;

public class Catch : Photon.MonoBehaviour {
	public Player ply;
    
	void Awake() {
		
	//	ply = this.transform.parent.gameObject.GetComponent<Player>();
	}
	void OnTriggerEnter(Collider col) {
        if (ply.photonView.isMine)
        {
            if (col.gameObject.tag == "Ball")
            {

                if (col.gameObject.GetComponent<Ball>().state == Ball.BallState.FreeFlight)
                {
                    col.gameObject.GetComponent<Ball>().photonView.RPC("SelfDestruct", PhotonTargets.All);
                    //PhotonNetwork.Destroy(col.gameObject);
                    ply.photonView.RPC("AddBall", PhotonTargets.All);

                    col.gameObject.GetComponent<TrailRenderer>().enabled = false;
                    col.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    col.enabled = false;
                    audio.Play();
                }
            }
        }
	}

}
