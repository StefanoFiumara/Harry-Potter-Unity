using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HarryPotterUnity.UI
{
    public class ErrorPanel : Menu
    {
        private Text _title;
        private Text _message;

        public string Title
        {
            get { return _title.text; }
            set { _title.text = value; }
        }

        public string Message
        {
            get { return _message.text; }
            set { _message.text = value; }
        }

        protected override void Awake()
        {
            base.Awake();
            var textComponents = GetComponentsInChildren<Text>();
            _title = textComponents.First(t => t.name.Contains("Title"));
            _message = textComponents.First(t => t.name.Contains("Message"));
        }
    }
}