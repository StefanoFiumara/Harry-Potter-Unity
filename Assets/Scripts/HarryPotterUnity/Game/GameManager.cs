using Assets.Scripts.HarryPotterUnity.Utils;
using UnityEngine;

namespace Assets.Scripts.HarryPotterUnity.Game
{
    public class GameManager : MonoBehaviour
    {
        public Player Player1, Player2;
        
        void Start()
        {
           //TODO: Instantiate Player 1 and Player 2 (with generated Decks) in Awake()
           Player1.OppositePlayer = Player2;
           Player2.OppositePlayer = Player1;
           
           Player1.DrawInitialHand();
           Player2.DrawInitialHand();
           
           Player1.InitTurn();

         
        }
    }
}