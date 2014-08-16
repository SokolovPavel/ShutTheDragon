using UnityEngine;
using System.Collections;

public class Control : Photon.MonoBehaviour {
	
	private Player ply;
	public Transform ballPos;
	public Rigidbody plane;
	public float maxThr;
	public int thrStep;
	public float minThr;
	public float pitchCoef = 60f;
	public float yawCoef = 45f;
	private Vector3 fwd;
	private float xSpd;
	private float ySpd;
	private float thrust;

	private float seaLevel;
	private float alt;
	private float range;
	private float curSpd;
    public GameObject camObj;
    public Camera cam;
    private float camDefFOV;
    private Vector3 camDefPos;
    private Vector3 camPos;

    //private Vector3 vertVec;


	void Awake () {
		if (photonView.isMine) {
            cam = camObj.GetComponent<Camera>();
            camDefFOV = cam.fieldOfView;
            camDefPos = transform.position - camObj.transform.position;
			Screen.lockCursor = true;
			rigidbody.isKinematic = false;
		} else {
			rigidbody.isKinematic = true;
		}
		plane = this.gameObject.rigidbody;
		ply = this.gameObject.GetComponent<Player>();
		seaLevel = this.transform.position.y;
		fwd = transform.TransformDirection (Vector3.up);

	}

	void OnGUI() {
		if (photonView.isMine) {
			GUI.Label (new Rect (20, 20, 200, 20), "THR: " + thrust);
			GUI.Label (new Rect (20, 40, 200, 20), "SPD: " + Mathf.FloorToInt (curSpd));
			GUI.Label (new Rect (20, 60, 200, 20), "ALT: " + Mathf.FloorToInt (alt));
			GUI.Label (new Rect (Screen.width / 2 - 100, Screen.height / 2 - 30, 200, 20), "Range: " + Mathf.FloorToInt (range));

           // GUI.Label(new Rect(20, 80, 200, 20), "MouseX: " + xSpd);
           // GUI.Label(new Rect(20, 100, 200, 20), "MouseY: " + ySpd);
            GUI.Label(new Rect(20, 80, 200, 20), "Angle: " + Vector3.Dot(transform.TransformDirection(Vector3.up),Vector3.up));
		}
	}

	void FixedUpdate ()
	{	
		if (photonView.isMine) {
            xSpd = Input.GetAxis("Mouse X");
            ySpd = Input.GetAxis("Mouse Y");
			curSpd = plane.velocity.magnitude;

			Screen.lockCursor = true;
			alt = transform.position.y - seaLevel;

			fwd = transform.TransformDirection (Vector3.forward);
			RaycastHit hit;
			if (Physics.Raycast (transform.position, fwd, out hit,800.0f)) {
				Debug.DrawRay (this.transform.position, fwd * 10);
				range = hit.distance;
			}

			plane.AddForce (Vector3.up* plane.mass * 9.85f );
			plane.AddRelativeForce (Vector3.forward* thrust);
			plane.AddRelativeTorque (0,  xSpd*yawCoef ,0);
			plane.AddRelativeTorque ( -ySpd*pitchCoef , 0,0);
           // plane.AddRelativeTorque(0, 0, (1 - Vector3.Dot(transform.TransformDirection(Vector3.up), Vector3.up)) * 50);

            //vertVec = 
          //  camObj.transform.position = transform.position - camDefPos +camObj.transform.TransformDirection(Vector3.back) * Mathf.Abs(curSpd * 0.1f);
         //   camObj.transform.LookAt(this.transform);


		}

	}
	

	void Update() {
		if (photonView.isMine) {
		
			if(Input.GetMouseButtonDown(0)){
				if(ply.balls>0){
				ply.photonView.RPC("ThrowBall",PhotonTargets.All);
				}
			}

		

			if(Input.GetKey("s")){
				thrust -= thrStep;
				if (thrust < -maxThr) {
					thrust = -maxThr;
				}
			}

			if(Input.GetKey("w")){
				thrust += thrStep;
				if (thrust > maxThr) {
					thrust = maxThr;
				}
			}

			if (Input.GetKey (KeyCode.LeftShift) && (Input.GetKeyDown ("a"))) {
				plane.AddRelativeForce (-10000, 0, 0);
			} else if (Input.GetKey ("a")) {
				plane.AddRelativeTorque (0, 0, 90);
				//	dampRoll = false;
			}

			if(Input.GetKey(KeyCode.LeftShift)&&(Input.GetKeyDown("d"))){
				plane.AddRelativeForce (10000, 0, 0);
			} else if (Input.GetKey ("d")) {
				//dampRoll = false;
				plane.AddRelativeTorque (0, 0, -90);
			} 

			if(Input.GetKey("x")){
				thrust = 0;

			}

			if(Input.GetKeyDown(KeyCode.Space)){
				plane.AddRelativeForce (0, 80000, 0);
			}


		}
	}
}
