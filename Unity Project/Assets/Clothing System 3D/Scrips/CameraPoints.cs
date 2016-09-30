// ********************************
//  [class-name]
//
//  [class-description]
//
//  [project-name]
//  2016 - Larissa Redeker
//  Kittens and Elves at Work
//  http://www.keawstudio.com
//
// ********************************

using UnityEngine;
using System.Collections.Generic;
using System;

public class CameraPoints : MonoBehaviour {

	[Serializable]
	public class CamPoint {
		public Transform camAnchor;
		public float FOV = 10f;
	}

	[SerializeField]
	public List<CamPoint> cameraPoints = new List<CamPoint>();

	public Camera cam;

	public void SetPoint(int indexPoint){

		Vector3 finalPos = cameraPoints [indexPoint].camAnchor.position;

		cam.transform.position = finalPos;
		cam.fieldOfView = cameraPoints [indexPoint].FOV;

	}

}
