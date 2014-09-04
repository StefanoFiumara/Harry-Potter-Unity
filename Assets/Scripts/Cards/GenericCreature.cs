using UnityEngine;
using System.Collections;

public class Creature : GenericCard {

	// Use this for initialization

    public CostTypes CostType;

    public int CostAmount;

    public int DamagePerTurn;
    public int Health;

	public new void Start ()
    {
        base.Start();
        Debug.Log("Creature Start");
	}

    public override void BeforeTurnAction()
    {

    }

    public override void AfterTurnAction()
    {

    }


}
