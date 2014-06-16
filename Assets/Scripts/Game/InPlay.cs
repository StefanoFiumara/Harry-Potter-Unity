using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CardTypes = GenericCard.CardTypes;

public class InPlay : MonoBehaviour {

    public List<Transform> Cards;

    public Player _Player;

    void Start()
    {
        Cards = new List<Transform>();
    }

    public void Add(Transform card, CardTypes CardType)
    {
        Cards.Add(card);

        switch (CardType)
        {
            case CardTypes.LESSON:
                AnimateLessonToBoard(card);
                break;
        }
    }

    private void AnimateLessonToBoard(Transform card)
    {
        //TODO: Animate to lesson section of the board
        // x: -160, y = 6, z = 15 + number of cards this column? remember islocal!
    }
}
