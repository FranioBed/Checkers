using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour {

    private int posX;
    private int posY;
    public bool color;  //true mean red, false white 


    // Use this for initialization
    void Start () {
		
	}
	
	void UpdatePosition (int x, int y) {
        posX = x;
        posY = y;
	}

    int[] GetPosition()
    {
        return new int [] {posX, posY };
    }
}
