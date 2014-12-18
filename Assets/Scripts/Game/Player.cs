using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public Hand _Hand;
    public Deck _Deck;
    public InPlay _InPlay;
    public Player _OppositePlayer;
    public Discard _Discard;

    public GenericCard StartingCharacter; //Set by main menu? GameObject?

    public int AmountLessonsInPlay = 0;
    public List<Lesson.LessonTypes> LessonTypesInPlay
    {
        get { return _lessonTypesInPlay; }
    }

    private List<Lesson.LessonTypes> _lessonTypesInPlay = new List<Lesson.LessonTypes>();

    public int ActionsAvailable { get; private set; }

    public int CreaturesInPlay { get; set; }
    public int DamagePerTurn { get; set; }

    public Player()
    {
        ActionsAvailable = 0;
    }

    public void UseAction()
    {
        ActionsAvailable--;

        if (ActionsAvailable > 0) return;
        ActionsAvailable = 0;
        //AfterTurnAction happens here
        _InPlay.Cards.ForEach(card => ((IPersistentCard) card).OnInPlayAfterTurnAction());
        _OppositePlayer.InitTurn();
    }

    public void AddAction()
    {
        ActionsAvailable++;
    }

    public void InitTurn()
    {
        //BeforeTurnAction happens here
        _InPlay.Cards.ForEach(card => ((IPersistentCard) card).OnInPlayBeforeTurnAction());

        _Deck.DrawCard();
        ActionsAvailable += 2;

        //Creatures do damage here
        _InPlay.GetCreaturesInPlay().ForEach(card => _OppositePlayer.TakeDamage(((GenericCreature) card).DamagePerTurn));
    }

    public bool CanUseAction()
    {
        return ActionsAvailable > 0;
    }

    public void DrawInitialHand()
    {
        //TODO: Needs cleanup
        for (var i = 0; i < 7; i++)
        {
            var card = _Deck.TakeTopCard();
            var cardPosition = Hand.HandCardsOffset;

            var shrinkFactor = _Hand.Cards.Count >= 12 ? 0.5f : 1f;

            cardPosition.x += _Hand.Cards.Count * Hand.Spacing * shrinkFactor;
            cardPosition.z -= _Hand.Cards.Count;

            _Hand.Cards.Add(card);
            card.transform.parent = _Hand.transform;

            Helper.AddTweenToQueue(card, cardPosition, 0.3f, 0f, GenericCard.CardStates.InHand, true, false);
        }       
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        for (var i = 0; i < amount; i++)
        {
            var card = _Deck.TakeTopCard();

            if (card == null)
            {
                //TODO: Show game over message here.
                Debug.Log("Game Over");
                break;
            }
            _Discard.Add(card);
        }
    }

    public void UpdateLessonTypesInPlay()
    {
        _lessonTypesInPlay = new List<Lesson.LessonTypes>();

        var lessons = _InPlay.Cards.FindAll(card => card is Lesson);

        foreach (Lesson card in lessons)
        {
            if (!_lessonTypesInPlay.Contains(card.LessonType))
            {
                _lessonTypesInPlay.Add(card.LessonType);
            }
        }
    }

    public void DisableAllCards()
    {
        _Deck.gameObject.layer = Helper.IgnoreRaycastLayer;
        Helper.DisableCards(_Hand.Cards);
        Helper.DisableCards(_InPlay.Cards);
    }

    public void EnableAllCards()
    {
        _Deck.gameObject.layer = Helper.DeckLayer;
        Helper.EnableCards(_Hand.Cards);
        Helper.EnableCards(_InPlay.Cards);
    }
}
