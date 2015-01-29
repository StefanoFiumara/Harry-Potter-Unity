using UnityEngine;

namespace Assets.Scripts.Game
{
    public class GameManager : MonoBehaviour
    {

        public Player Player1, Player2;

        // Use this for initialization
        void Start()
        {
            Debug.Log("Init GameManager");

            Player1.OppositePlayer = Player2;
            Player2.OppositePlayer = Player1;

            Player1.DrawInitialHand();
            Player2.DrawInitialHand();

            Player1.InitTurn();
        }
    }
}