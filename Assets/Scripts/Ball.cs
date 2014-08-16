using UnityEngine;
using System.Collections;

public class Ball : Photon.MonoBehaviour {
	
	private Vector3 fwd;
	private float time;
	public float triggerDist;
	public float throwTime = 2.0f;
	public int turnChance = 400;
	public float thrust = 5;
	public float maxThrust =10.0f;
	public float minThrust =4.0f;
	public BallState state;
	public enum BallState {
	FreeFlight,
	Pass,
	Thrown
	
	}

    private Vector3[] points = new Vector3[32];
    private int pointsCount;
    private GameObject pointHolder;
    private Vector3 targetPosition;

    private readonly VectorPid angularVelocityController = new VectorPid(33.7766f, 0, 0.2553191f);
    private readonly VectorPid headingController = new VectorPid(50.244681f, 0, 0.06382979f);

	
	// Use this for initialization
	void Awake () {
			if (photonView.isMine) {
			Screen.lockCursor = true;
			rigidbody.isKinematic = false;
            pointHolder = GameObject.Find("BallWaypoints");
            int i = 0;
            foreach (Transform child in pointHolder.transform)
            {
                points[i] = child.position;
               // points[i] = child;
                i++;
            }
            pointsCount = i;
            targetPosition = points[Random.Range(0, pointsCount)];
               
			
		} else {
			rigidbody.isKinematic = true;
		}

		fwd = transform.TransformDirection (Vector3.up);
		state = BallState.FreeFlight;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	if (photonView.isMine) {
		if(state == BallState.FreeFlight) {
			rigidbody.AddForce(0,rigidbody.mass * 9.84f,0);
			rigidbody.AddRelativeForce(Vector3.forward * thrust);

            var angularVelocityError = rigidbody.angularVelocity * -1;
           // Debug.DrawRay(transform.position, rigidbody.angularVelocity * 5, Color.black);

            var angularVelocityCorrection = angularVelocityController.Update(angularVelocityError, Time.deltaTime);
          //  Debug.DrawRay(transform.position, angularVelocityCorrection, Color.green);

            rigidbody.AddTorque(angularVelocityCorrection);

            var desiredHeading = targetPosition - transform.position;
         //   Debug.DrawRay(transform.position, desiredHeading, Color.magenta);

            var currentHeading = transform.forward;
        //    Debug.DrawRay(transform.position, currentHeading * 5, Color.blue);

            var headingError = Vector3.Cross(currentHeading, desiredHeading);
            var headingCorrection = headingController.Update(headingError, Time.deltaTime);

            rigidbody.AddTorque(headingCorrection);






			if(Random.Range(0, turnChance)==33) {
				//rigidbody.AddRelativeTorque(Random.Range(0, 50),Random.Range(0, 50),Random.Range(0, 50));
                targetPosition = points[Random.Range(0, pointsCount)];

				thrust = Random.Range(minThrust,maxThrust);
			}

		}
		
		if(state == BallState.Thrown) {
			time += Time.fixedDeltaTime;
			if (time>throwTime){
				state = BallState.FreeFlight;
			}
		}
	}

	}
	
		void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		if (stream.isWriting) {
			stream.SendNext (transform.position);
			stream.SendNext (transform.rotation);

		} else {
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			correctPlayerRot = (Quaternion)stream.ReceiveNext();
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
        Debug.DrawRay(transform.position, targetPosition -transform.position);
	}
	
	void DistCheck() {
	
	
	}

    [RPC]
    public void SelfDestruct()
    {
        if (photonView.isMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
