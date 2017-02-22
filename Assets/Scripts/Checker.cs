using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour {

    private int posX;
    private int posY;
    public bool color;  //true means red, false white 
    
    void Start () {
		
	}
	
	void SetPosition (int x, int y) {
        posX = x;
        posY = y;
	}

    int[] GetPosition()
    {
        return new int [] {posX, posY };
    }
}
