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

    public Player _Player;

    private readonly Vector2 COLLIDER_SIZE = new Vector2(50f, 70f);

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

        gameObject.layer = Helper.CARD_LAYER;

        FrontPlane = transform.FindChild("Front").gameObject;
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
        FrontPlane.layer = Helper.PREVIEW_LAYER;
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
        FrontPlane.layer = Helper.CARD_LAYER;
        Helper.PreviewCamera.transform.position = Helper.DefaultPreviewCameraPos;
    }

    public void Disable()
    {
        gameObject.layer = Helper.IGNORE_RAYCAST_LAYER;
        FrontPlane.renderer.material.color = new Color(0.35f, 0.35f, 0.35f);
    }

    public void Enable()
    {
        gameObject.layer = Helper.CARD_LAYER;
        FrontPlane.renderer.material.color = Color.white;
    }

    public void SetSelected()
    {
        gameObject.layer = Helper.CARD_LAYER;
        FrontPlane.renderer.material.color = Color.yellow;
    }
    
}
