using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiceToSnuffboxes : GenericSpell {

    private List<GenericCard> SelectedCards;

    public override void OnPlayAction()
    {
        //Move ALL invalid colliders to ignoreraycast layer
        _Player._Deck.gameObject.layer = Helper.IGNORE_RAYCAST_LAYER;
        _Player._OppositePlayer._Deck.gameObject.layer = Helper.IGNORE_RAYCAST_LAYER;

        Helper.DisableCards(_Player._Hand.Cards);
        Helper.DisableCards(_Player._OppositePlayer._Hand.Cards);
        Helper.DisableCards(_Player._InPlay.Cards);
        Helper.DisableCards(_Player._OppositePlayer._InPlay.Cards);

        var validCards = _Player._InPlay.GetCreaturesInPlay();
        validCards.AddRange(_Player._OppositePlayer._InPlay.GetCreaturesInPlay());

        foreach(var card in validCards)
        {
            card.Enable();
            card.gameObject.layer = Helper.VALID_CHOICE_LAYER;
        }

        SelectedCards = new List<GenericCard>();
        StartCoroutine(WaitForPlayerInput(SelectedCards));
    }

    public override bool MeetsAdditionalPlayRequirements()
    {
        var validCards = _Player._InPlay.GetCreaturesInPlay();
        validCards.AddRange(_Player._OppositePlayer._InPlay.GetCreaturesInPlay());

        return validCards.Count >= 2;
    }

    public override void AfterInputAction(List<GenericCard> selectedCards)
    {
        foreach(var card in selectedCards) {
            card._Player._InPlay.Remove(card);
            card._Player._Hand.Add(card, false, false);
            card.Enable();
            Helper.RotateCard(card.transform);
        }

        Helper.EnableCards(_Player._Hand.Cards);
        Helper.EnableCards(_Player._OppositePlayer._Hand.Cards);
        Helper.EnableCards(_Player._InPlay.Cards);
        Helper.EnableCards(_Player._OppositePlayer._InPlay.Cards);

        _Player._Deck.gameObject.layer = Helper.DECK_LAYER;
        _Player._OppositePlayer._Deck.gameObject.layer = Helper.DECK_LAYER;
    }
}
