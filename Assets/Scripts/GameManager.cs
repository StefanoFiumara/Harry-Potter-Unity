using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

    public Player Player1, Player2;

	// Use this for initialization
	void Start () {
        Debug.Log("Init GameManager");
	    Player1.DrawInitialHand();
        Player2.DrawInitialHand();

        Player1.InitTurn();
	}
}
