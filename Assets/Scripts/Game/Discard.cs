using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Discard : MonoBehaviour {


    private List<Transform> Cards;
    public Player _Player; //not sure if needed just yet

    private readonly Vector2 DISCARD_POSITION_OFFSET = new Vector2(-355f, -124); //to be determined
    private readonly Vector2 DISCARD_SPACING = new Vector2(0, 0); //to be determined

    
	// Use this for initialization
	void Start () {
	    Cards = new List<Transform>();
	}

    public void Add(Transform card) //don't need to worry about type here, since they all go to the same place.
    {
        Cards.Add(card);


    }
}
