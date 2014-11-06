using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck : MonoBehaviour {

    public List<Transform> Cards;

    public Hand _Hand;
    public Player _Player;

    private readonly Vector2 DECK_POSITION_OFFSET = new Vector2(-355f, -124f);

    public float DeckShuffleTweenTime = 0.5f;

	// Use this for initialization
	public void Start () {
        //instantiate cards into scene
        Vector3 cardPos = new Vector3(DECK_POSITION_OFFSET.x, DECK_POSITION_OFFSET.y, 0f);
        for (int i = 0; i < Cards.Count; i++)
        {
            Cards[i] = (Transform)Instantiate(Cards[i]);
            Cards[i].parent = transform;
            Cards[i].localPosition = cardPos + Vector3.back * -16f;
            Cards[i].rotation = Quaternion.Euler(new Vector3(0f, 180f, _Player.transform.rotation.eulerAngles.z));
            Cards[i].position += i * Vector3.back * 0.2f;
            
            //Give the card a reference to the player so that it knows who it belongs to.
            GenericCard cardInfo = Cards[i].GetComponent<GenericCard>();
            cardInfo.SetPlayer(_Player);
        }

        //Set the collider to the proper position
        if (gameObject.collider == null)
        {
            var col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(50f, 70f, 1f);
            col.center = new Vector3(DECK_POSITION_OFFSET.x, DECK_POSITION_OFFSET.y, 0f);
        }
	}
	
    public Transform TakeTopCard()
    {
        if (Cards.Count == 0) return null;

        Transform card = Cards[Cards.Count - 1];
        Cards.RemoveAt(Cards.Count - 1);
        return card;
    }

    public void OnMouseUp()
    {
        if(Cards.Count > 0) DrawCard();
    }

    private void DrawCard()
    {
        if (_Player.UseAction())
        {
            Transform card = TakeTopCard();
            _Hand.Add(card);
            card.parent = _Hand.transform;   
        }
    }


    public void Shuffle()
    {
        //TODO: Switch to Fisher-Yates shuffle
        for (int i = 0; i < Cards.Count; i++)
        {
            int random = Random.Range(i, Cards.Count);

            Transform temp = Cards[i];
            Cards[i] = Cards[random];
            Cards[random] = temp;

            float newZ = (transform.position.z + 16f) - i * 0.2f;
            
            Vector3 point1 = new Vector3(Cards[i].position.x, Cards[i].position.y + 80, Cards[i].position.z);
            Vector3 point2 = new Vector3(Cards[i].position.x, Cards[i].position.y, newZ);

            iTween.MoveTo(Cards[i].gameObject, iTween.Hash("time", DeckShuffleTweenTime, 
                                                           "path", new Vector3[] {point1, point2}, 
                                                           "easetype", iTween.EaseType.easeInOutSine, 
                                                           "delay", Random.Range(0f,1.5f))
                                                           );
        }
    }
}
