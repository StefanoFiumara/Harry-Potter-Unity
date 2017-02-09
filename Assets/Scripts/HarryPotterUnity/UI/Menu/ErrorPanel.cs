using System.Linq;
using UnityEngine.UI;

namespace HarryPotterUnity.UI.Menu
{
    public class ErrorPanel : BaseMenu
    {
        private Text _title;
        private Text _message;

        public string Title
        {
            get { return this._title.text; }
            set { this._title.text = value; }
        }

        public string Message
        {
            get { return this._message.text; }
            set { this._message.text = value; }
        }

        protected override void Awake()
        {
            base.Awake();
            var textComponents = this.GetComponentsInChildren<Text>();
            this._title = textComponents.First(t => t.name.Contains("Title"));
            this._message = textComponents.First(t => t.name.Contains("Message"));
        }
    }
}