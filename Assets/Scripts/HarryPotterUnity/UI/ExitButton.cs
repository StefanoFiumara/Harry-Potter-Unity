using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.UI
{
    public class ExitButton : MonoBehaviour
    {
        [UsedImplicitly]
        public void ExitGame()
        {
            //Save settings here (when we have some)
            Application.Quit();
        }
    }
}
