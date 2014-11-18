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
    public List<LessonTypes> LessonTypesInPlay;

    public int ActionsAvailable = 2;

    public int nCreaturesInPlay;
    public int DamagePerTurn;

    public bool IsGoingFirst;

	public void Start () {
        LessonTypesInPlay = new List<LessonTypes>(5);
	}

    public void UseAction()
    {
        ActionsAvailable--;

        if (ActionsAvailable <= 0)
        {
            ActionsAvailable = 0;
            //TODO: Player => InPlay AfterTurnAction happens here
            _OppositePlayer.InitTurn();
        }
    }

    public void InitTurn()
    {
        //Opposite player => InPlay BeforeTurnAction happens here
        _Deck.DrawCard();
        ActionsAvailable += 2;
        //creatures do damage here
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
        LessonTypesInPlay = new List<LessonTypes>();

        var lessons = _InPlay.Cards.FindAll(card => card is Lesson);

        foreach (Lesson card in lessons)
        {
            if (!LessonTypesInPlay.Contains(card.LessonType))
            {
                LessonTypesInPlay.Add(card.LessonType);
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
