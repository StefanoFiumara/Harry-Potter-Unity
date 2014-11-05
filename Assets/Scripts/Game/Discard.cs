using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Discard : MonoBehaviour {


    private List<Transform> Cards;
    public Player _Player; //not sure if needed just yet

    private readonly Vector2 DISCARD_POSITION_OFFSET = new Vector2(-355f, -204f); //to be determined

    
	// Use this for initialization
	void Start () {
	    Cards = new List<Transform>();

        if (gameObject.collider == null)
        {
            var col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(50f, 70f, 1f);
            col.center = new Vector3(DISCARD_POSITION_OFFSET.x, DISCARD_POSITION_OFFSET.y, 0f);
        }
	}

    //TODO: Test this function
    public void Add(Transform card) 
    {
        Cards.Add(card);
        Vector3 cardPos = new Vector3(DISCARD_POSITION_OFFSET.x, DISCARD_POSITION_OFFSET.y, -16f);
        cardPos.z +=  Cards.Count * 0.2f;
        Helper.TweenCardToPosition(card, cardPos, GenericCard.CardStates.DISCARDED, 0.5f, false);
    }

    //TODO: OnMouseUp: View cards in pile
}
