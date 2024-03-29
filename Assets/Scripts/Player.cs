﻿using UnityEngine;
using System.Collections;

public class Player : Photon.MonoBehaviour {
    public int team;

	public GameObject PlyCam;
	public Control controls;
	public int balls = 5;
	public GameObject ballObj;

    public GameObject chatIcon;

    private bool typing;

    public int score;

	void Awake() {
		if(balls==0){ballObj.SetActive(false);}
		if (photonView.isMine) {
			PlyCam.gameObject.SetActive (true);
			controls.enabled = true;
		} else {
			Destroy (PlyCam);

		}

		gameObject.name = gameObject.name + photonView.viewID;
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		if (stream.isWriting) {
			stream.SendNext (transform.position);
			stream.SendNext (transform.rotation);
            stream.SendNext(typing);


		} else {
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			correctPlayerRot = (Quaternion)stream.ReceiveNext();
            typing = (bool)stream.ReceiveNext();
		}
	}

	private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
	private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

	void Update()
	{
		if (!photonView.isMine)
		{
           
			//Update remote player (smooth this, this looks good, at the cost of some accuracy)
			transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
			transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);


        }
        else
        {
            typing = InRoomChat.typing;
           // typing = InRoomChat.typing;
          //  if (typing) { chatIcon.SetActive(true); } else { chatIcon.SetActive(false); }   
        }
       // Debug.Log(typing);
        if (typing) { chatIcon.SetActive(true); } else { chatIcon.SetActive(false); }

	}

    [RPC]
    public void SetTeam(int index)
    {
        team = index;
       
            foreach (Renderer ren in gameObject.GetComponentsInChildren<Renderer>())
            {
                if (index == 0)
                {
                ren.material.color = new Color(0, 0, 1f);
                }
                else
                {
                    ren.material.color = new Color(1f, 0, 0);
                }
            }

        
    }

	[RPC]
	public void AddBall(){
		balls++;
		ballObj.SetActive(true);
	}
	
	[RPC]
	public void ThrowBall(){
		
		if(balls == 0){ballObj.SetActive(false); return;}
        balls--;
		if (photonView.isMine) {
			
			GameObject obj =   PhotonNetwork.Instantiate("Ball", ballObj.transform.position, Quaternion.identity, 0, null) as GameObject;
			obj.GetComponent<Ball>().state = Ball.BallState.Thrown;
            obj.GetComponent<Ball>().photonView.RPC("SetBallState", PhotonTargets.All, (int)Ball.BallState.Thrown);
			obj.rigidbody.velocity = this.rigidbody.velocity;
			//obj.rigidbody.AddRelativeForce(Vector3.forward * 2000);
			obj.rigidbody.velocity += transform.TransformDirection(Vector3.forward)*15;
			obj.rigidbody.AddForce(Vector3.up * 500);
            if (balls == 0)
            {
                ballObj.SetActive(false);
            }
		}
	}
	
}
