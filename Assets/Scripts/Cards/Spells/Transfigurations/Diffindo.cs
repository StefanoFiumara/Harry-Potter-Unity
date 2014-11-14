using UnityEngine;
using System.Collections;

public class Diffindo : GenericSpell {

    public LayerMask mask;

    public override void OnPlayAction()
    {
        //Move ALL invalid colliders to ignoreraycast layer
        _Player._Deck.gameObject.layer = Helper.IGNORE_RAYCAST_LAYER;
        _Player._OppositePlayer._Deck.gameObject.layer = Helper.IGNORE_RAYCAST_LAYER;

        Helper.DisableCards(_Player._Hand.Cards);
        Helper.DisableCards(_Player._InPlay.Cards);
        Helper.DisableCards(_Player._OppositePlayer._Hand.Cards);
        
        //place valid cards in valid layer
        foreach(var card in _Player._OppositePlayer._InPlay.Cards) {
            card.gameObject.layer = Helper.VALID_CHOICE_LAYER;
        }

        StartCoroutine(WaitForPlayerInput());
    }

    public override bool MeetsAdditionalPlayRequirements()
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
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                // Casts the ray and get the first game object hit
                if (Physics.Raycast(ray, out hit,1000f, mask))
                {
                    //Get reference to card here
                    GenericCard target = hit.transform.gameObject.GetComponent<GenericCard>();
                    
                    playerClickedValidCard = true;

                    //TODO: call separate function with Action(GenericCard chosenCard); and abstract this loop out
                    _Player._OppositePlayer._InPlay.Remove(target);
                    _Player._OppositePlayer._Discard.Add(target, 0.1f);


                    //reset the layer for all the cards
                    _Player._Deck.gameObject.layer = 0;
                    _Player._OppositePlayer._Deck.gameObject.layer = 0;
                    foreach (var card in _Player._OppositePlayer._InPlay.Cards)
                    {
                        card.gameObject.layer = Helper.CARD_LAYER;
                    }
                    Helper.EnableCards(_Player._Hand.Cards);
                    Helper.EnableCards(_Player._InPlay.Cards);
                    Helper.EnableCards(_Player._OppositePlayer._Hand.Cards);
                }
            }

            yield return null;
        }

        
    }
}
