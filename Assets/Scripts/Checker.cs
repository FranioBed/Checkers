using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour {

    private int posX;
    private int posY;
    public bool color;  //true means red, false white 
    public bool queen = false;


    void Start () {
		
	}
	
	public void SetPosition (int x, int y) {
        posX = x;
        posY = y;
	}

    public int[] GetPosition()
    {
        return new int [] {posX, posY };
    }
}
