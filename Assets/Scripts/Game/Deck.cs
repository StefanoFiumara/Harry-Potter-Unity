using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck : MonoBehaviour {

    public List<Transform> Cards;

    public Hand _Hand;
    public Player _Player;

    public float DeckShuffleTweenTime = 0.5f;

	// Use this for initialization
	public void Start () {
        for (int i = 0; i < Cards.Count; i++)
        {
            Cards[i] = (Transform)Instantiate(Cards[i], transform.position + Vector3.back * -16f, Quaternion.Euler(new Vector3(0f, 180f, _Player.transform.rotation.eulerAngles.z)));
            Cards[i].parent = transform;
            Cards[i].position += i * Vector3.back * 0.2f;
            
            //Give the card a reference to the player so that it knows who it belongs to.
            GenericCard cardInfo = Cards[i].GetComponent<GenericCard>();
            cardInfo._Player = _Player;
        }

       // Shuffle();
	}
	
    public Transform TakeTopCard()
    {
        Transform card = Cards[Cards.Count - 1];
        Cards.RemoveAt(Cards.Count - 1);
        return card;
    }

    public void OnMouseUp()
    {
        DrawCard();
    }

    private void DrawCard()
    {
        if (_Player.ActionsAvailable <= 0) return;
        Transform card = TakeTopCard();
        _Hand.Add(card);
        card.parent = _Hand.transform;

        _Player.ActionsAvailable--; //TODO: Refactor to _Player.UseAction()
    }


    public void Shuffle()
    {
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
