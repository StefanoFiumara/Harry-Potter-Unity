using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LessonTypes = Lesson.LessonTypes;
using CardStates = GenericCard.CardStates;
using CardTypes = GenericCard.CardTypes;

public class Player : MonoBehaviour {

    public Hand _Hand;
    public Deck _Deck;
    public InPlay _InPlay;
    public Player _OppositePlayer;
    public Discard _Discard;

    public GenericCard StartingCharacter; //Set by main menu? GameObject?

    public int nLessonsInPlay = 0;
    public List<LessonTypes> LessonTypesInPlay
    {
        get { return _LessonTypesInPlay; }
    }

    private List<LessonTypes> _LessonTypesInPlay = new List<LessonTypes>();

    public int ActionsAvailable 
    {
        get { return _ActionsAvailable; }
    }

    private int _ActionsAvailable = 0;

    public int nCreaturesInPlay;
    public int DamagePerTurn;

    public bool IsGoingFirst;


    public void UseAction()
    {
        _ActionsAvailable--;

        if (_ActionsAvailable <= 0)
        {
            _ActionsAvailable = 0;
            //AfterTurnAction happens here
            _InPlay.Cards.ForEach(card => (card as PersistentCard).OnInPlayAfterTurnAction());
            _OppositePlayer.InitTurn();
        }
    }

    public void AddAction()
    {
        _ActionsAvailable++;
    }

    public void InitTurn()
    {
        //BeforeTurnAction happens here
        _InPlay.Cards.ForEach(card => (card as PersistentCard).OnInPlayBeforeTurnAction());

        _Deck.DrawCard();
        _ActionsAvailable += 2;

        //Creatures do damage here
        _InPlay.GetCreaturesInPlay().ForEach(card => _OppositePlayer.TakeDamage((card as GenericCreature).DamagePerTurn));
    }

    public bool CanUseAction()
    {
        return ActionsAvailable > 0;
    }

    public void DrawInitialHand()
    {
        //TODO: Needs cleanup
        for (int i = 0; i < 7; i++)
        {
            GenericCard card = _Deck.TakeTopCard();
            Vector3 cardPosition = Hand.HAND_CARDS_OFFSET;

            float shrinkFactor = _Hand.Cards.Count >= 12 ? 0.5f : 1f;

            cardPosition.x += _Hand.Cards.Count * Hand.SPACING * shrinkFactor;
            cardPosition.z -= _Hand.Cards.Count;

            _Hand.Cards.Add(card);
            Helper.AddTweenToQueue(card, cardPosition, 0.3f, 0f, CardStates.IN_HAND, true, false);
        }

        if (IsGoingFirst)
        {
            InitTurn();
        }
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        for (int i = 0; i < amount; i++)
        {
            GenericCard card = _Deck.TakeTopCard();

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
        _LessonTypesInPlay = new List<LessonTypes>();

        var lessons = _InPlay.Cards.FindAll(card => card is Lesson);

        foreach (Lesson card in lessons)
        {
            if (!_LessonTypesInPlay.Contains(card.LessonType))
            {
                _LessonTypesInPlay.Add(card.LessonType);
            }
        }
    }

    public void DisableAllCards()
    {
        _Deck.gameObject.layer = Helper.IGNORE_RAYCAST_LAYER;
        Helper.DisableCards(_Hand.Cards);
        Helper.DisableCards(_InPlay.Cards);
    }

    public void EnableAllCards()
    {
        _Deck.gameObject.layer = Helper.DECK_LAYER;
        Helper.EnableCards(_Hand.Cards);
        Helper.EnableCards(_InPlay.Cards);
    }
}
