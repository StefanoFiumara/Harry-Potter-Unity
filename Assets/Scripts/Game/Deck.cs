using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck : MonoBehaviour {

    public List<GenericCard> Cards;

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
            Cards[i] = (GenericCard)Instantiate(Cards[i]);
            Cards[i].transform.parent = transform;
            Cards[i].transform.localPosition = cardPos + Vector3.back * -16f;
            Cards[i].transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, _Player.transform.rotation.eulerAngles.z));
            Cards[i].transform.position += i * Vector3.back * 0.2f;

            Cards[i]._Player = _Player;
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
	
    public GenericCard TakeTopCard()
    {
        if (Cards.Count <= 0)
        {
            return null;
        }

        GenericCard card = Cards[Cards.Count - 1];
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
            GenericCard card = TakeTopCard();
            _Hand.Add(card);
            card.transform.parent = _Hand.transform;   
        }
    }

    public void Shuffle()
    {
        for (int i = Cards.Count-1; i >= 0; i--)
        {
            int random = Random.Range(0, i);

            GenericCard temp = Cards[i];
            Cards[i] = Cards[random];
            Cards[random] = temp;

            float newZ = (transform.position.z + 16f) - i * 0.2f;

            Vector3 point1 = new Vector3(Cards[i].transform.position.x, Cards[i].transform.position.y + 80, Cards[i].transform.position.z);
            Vector3 point2 = new Vector3(Cards[i].transform.position.x, Cards[i].transform.position.y, newZ);

            iTween.MoveTo(Cards[i].gameObject, iTween.Hash("time", DeckShuffleTweenTime, 
                                                           "path", new Vector3[] {point1, point2}, 
                                                           "easetype", iTween.EaseType.easeInOutSine, 
                                                           "delay", Random.Range(0f,1.5f))
                                                           );
        }
    }

    public void Disable()
    {
        gameObject.layer = Helper.IGNORE_RAYCAST_LAYER;
    }

    public void Enable()
    {
        gameObject.layer = Helper.DECK_LAYER;
    }
}
