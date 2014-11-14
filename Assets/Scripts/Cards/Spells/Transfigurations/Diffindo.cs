using UnityEngine;
using System.Collections;

public class Diffindo : GenericSpell {

    public const int VALID_CHOICE_LAYER = 11;
    public const int IGNORE_RAYCAST_LAYER = 2;

    public LayerMask mask;

    public override void OnPlayAction()
    {
        //TODO: fade out invalid cards

        //Move ALL invalid colliders to ignoreraycast layer
        _Player._Deck.gameObject.layer = IGNORE_RAYCAST_LAYER;
        foreach (var card in _Player._Hand.Cards)
        {
            card.gameObject.layer = IGNORE_RAYCAST_LAYER;
        }

        //place valid cards in valid layer
        foreach(var card in _Player._OppositePlayer._InPlay.Cards) {
            card.gameObject.layer = VALID_CHOICE_LAYER;
        }

        StartCoroutine(WaitForPlayerInput());
    }

    public override bool MeetsPlayRequirements()
    {
        return _Player._OppositePlayer._InPlay.Cards.Count > 0;
    }

    private IEnumerator WaitForPlayerInput()
    {
        bool playerClickedValidCard = false;

        while (!playerClickedValidCard)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Debug.Log("Checking Raycast");
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                // Casts the ray and get the first game object hit
                if (Physics.Raycast(ray, out hit,1000f, mask))
                {
                    Debug.Log("Found gameobject at layer: " + hit.transform.gameObject.layer);
                    //Get reference to card here
                    GenericCard target = hit.transform.gameObject.GetComponent<GenericCard>();
                    
                    playerClickedValidCard = true;

                    //TODO: call separate function with Action(GenericCard chosenCard); and abstract this loop out
                    _Player._OppositePlayer._InPlay.Cards.Remove(target);
                    _Player._OppositePlayer._Discard.Add(target, 0.1f);


                    //reset the layer for all the cards
                    _Player._Deck.gameObject.layer = 0;
                    foreach (var card in _Player._OppositePlayer._InPlay.Cards)
                    {
                        card.gameObject.layer = CARD_LAYER;
                    }
                    foreach (var card in _Player._Hand.Cards)
                    {
                        card.gameObject.layer = 0;
                    }

                }
            }

            yield return null;
        }

        
    }
}
