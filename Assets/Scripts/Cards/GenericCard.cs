using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GenericCard : MonoBehaviour {

    public enum CardStates
    {
        IN_DECK, IN_HAND, IN_PLAY, DISCARDED
    }

    public enum CardTypes
    {
        LESSON, CREATURE, SPELL, ITEM, LOCATION, MATCH, ADVENTURE, CHARACTER
    }
    public enum CostTypes
    {
        CARE_OF_MAGICAL_CREATURES, CHARMS, TRANSFIGURATION, POTIONS, QUIDDITCH
    }

    public CardStates State;
    public CardTypes CardType;

    protected Player _Player;

    private readonly Vector2 COLLIDER_SIZE = new Vector2(50f, 70f);

    public const int PREVIEW_LAYER = 9;
    public const int CARD_LAYER = 10;

    private GameObject FrontPlane;

    public void Start()
    {
        //Add the collider through code instead of through unity so that if it ever changes, we won't need to edit every prefab.
        if(gameObject.collider == null)
        {
            var col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(COLLIDER_SIZE.x, COLLIDER_SIZE.y, 0.2f);
        }

        FrontPlane = transform.FindChild("Front").gameObject;
    }

    public void SetPlayer(Player p)
    {
        _Player = p;
    }

    public void SwitchState(CardStates newState)
    {
        State = newState;
    }

    public void OnMouseOver()
    {
        ShowPreview();
    }

    public void OnMouseExit()
    {
        HidePreview();
    }

    private void ShowPreview()
    {
        FrontPlane.layer = PREVIEW_LAYER;
        if (State != CardStates.IN_DECK && State != CardStates.DISCARDED)
        {
            if (iTween.Count(gameObject) == 0)
            {
                Helper.PreviewCamera.transform.rotation = transform.rotation;
                Helper.PreviewCamera.transform.position = transform.position + 2 * Vector3.back;
            }
            else
            {
                HidePreview();
            }
        }
    }
    
    private void HidePreview()
    {
        FrontPlane.layer = CARD_LAYER;
        Helper.PreviewCamera.transform.position = Helper.DefaultPreviewCameraPos;
    }
    
}
