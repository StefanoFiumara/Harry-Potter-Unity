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

    public void InitTurn(float drawAnimDelay = 1f)
    {
        //Opposite player => InPlay BeforeTurnAction happens here
        _Deck.DrawCard(drawAnimDelay);
        ActionsAvailable += 2;
        //creatures do damage here
    }

    public bool CanUseAction()
    {
        return ActionsAvailable > 0;
    }

    public void DrawInitialHand()
    {
        for (int i = 0; i < 7; i++)
        {
            _Deck.DrawCard(i*0.6f);
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

            //TODO: Adjust this offset
            Vector3 cardPosition = Discard.PREVIEW_OFFSET;
            cardPosition.x += i * Hand.SPACING;
            //move to 
            iTween.MoveTo(card.gameObject, iTween.Hash("time", 0.4f,
                                                        "position", cardPosition,
                                                        "easetype", iTween.EaseType.easeInOutSine,
                                                        "delay", 0.1f
                                                        ));

            //flip
            iTween.RotateTo(card.gameObject, iTween.Hash("time", 0.2f,
                                                        "y", 0f,
                                                        "easetype", iTween.EaseType.easeInOutSine,
                                                        "delay", 0.1f
                                                        ));
            _Discard.Add(card, 0.9f);
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
