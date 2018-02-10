using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingOrigin : MonoBehaviour {

    private const float maxDist = 10000;

	void Update () {
        Vector3 cameraPosition = transform.position;

	    if (cameraPosition.sqrMagnitude > maxDist * maxDist) {
            transform.position -= cameraPosition;

            GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");

            for (int i = 0; i < planets.Length; i++) {
                planets[i].GetComponent<PlanetController>().UpdatePositions(cameraPosition);
            }
        }	
	}
}
