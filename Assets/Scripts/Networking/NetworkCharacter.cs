using UnityEngine;
using System.Collections;
using System;

public class NetworkCharacter : Photon.MonoBehaviour
{

    Vector3 realPosition = Vector3.zero;
    Quaternion realRotation = Quaternion.identity;
    Quaternion turretRotation = Quaternion.identity;
    Transform turret;

	// Use this for initialization
	void Start () {
        turret = transform.FindChild("Turret");
	}
	
	// Update is called once per frame
	void Update () {
        //if (photonView.isMine)
        //{
        //    // Do nothing - character motor is moving us
        //}
        //else
        //{
        //    transform.position = Vector3.Lerp(transform.position, realPosition, 0.2f);
        //  //  transform.position = realPosition;
        //    transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.2f);
        //    if (turret != null)
        //    {
        //        turret.rotation = Quaternion.Lerp(turret.rotation, turretRotation, 0.2f);
        //    }
        //}
	}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //// This is our player, we need to send our actual position to the network
            //stream.SendNext(transform.position);
            //stream.SendNext(transform.rotation);
            //if (turret != null)
            //{
            //    stream.SendNext(turret.rotation);
            //}
            
        }
        else
        {
            // This is someone else's player, we need to receive their position (as of a
            // few milliseconds ago, and update our version of that player.

            //realPosition = (Vector3)stream.ReceiveNext();
            //realRotation = (Quaternion)stream.ReceiveNext();
            //turretRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}