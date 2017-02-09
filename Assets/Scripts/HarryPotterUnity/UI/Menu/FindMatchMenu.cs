using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace HarryPotterUnity.UI.Menu
{
    public class FindMatchMenu : BaseMenu
    {
        private List<Toggle> _lessonSelectButtons;
        private SubMenuManager _subMenuManager;
        private ErrorPanel _errorPanel;

        private Button _startMatchmakingButton;
        private Button _cancelMatchmakingButton;

        private Text _findMatchStatus;

        protected override void Awake()
        {
            base.Awake();

            this._lessonSelectButtons = FindObjectsOfType<Toggle>().ToList();

            this._subMenuManager = FindObjectOfType<SubMenuManager>();
            this._errorPanel = FindObjectOfType<ErrorPanel>();

            var allButtons = FindObjectsOfType<Button>().ToList();
            this._startMatchmakingButton = allButtons.Find(b => b.name.Contains("StartMatchmakingButton"));
            this._cancelMatchmakingButton = allButtons.Find(b => b.name.Contains("BackToMainMenuButton"));

            this._findMatchStatus = FindObjectsOfType<Text>().First(t => t.name.Contains("FindMatchStatus"));
        }

        [UsedImplicitly]
        public void StartMatchmaking()
        {
            var selectedLessons = this.GetSelectedLessons();
            
            if (selectedLessons.Count == 2 || selectedLessons.Count == 3)
            {
                var lessonBytes = selectedLessons.Select(n => (byte)n).ToArray();

                var selected = new Hashtable { { "lessons", lessonBytes } };
                PhotonNetwork.player.SetCustomProperties(selected);

                PhotonNetwork.JoinRandomRoom();

                this._startMatchmakingButton.interactable = false;
                this._cancelMatchmakingButton.interactable = false;

                this._lessonSelectButtons.ForEach(t => t.interactable = false);
            }
            else
            {
                this._errorPanel.Title = "Error!";
                this._errorPanel.Message = "You must choose two or three lesson types to enter matchmaking!";
                this._subMenuManager.ShowMenu(this._errorPanel);
            }
        }

        [UsedImplicitly]
        public void OnJoinedRoom()
        {
            this._cancelMatchmakingButton.interactable = true;
            this._findMatchStatus.GetComponent<Animator>().SetBool("IsFindingMatch", true);
        }

        [UsedImplicitly]
        public void OnLeftRoom()
        {
            this._findMatchStatus.GetComponent<Animator>().SetBool("IsFindingMatch", false);
        }
        

        private List<LessonTypes> GetSelectedLessons()
        {
            var selectedLessons = this._lessonSelectButtons
                .Where(t => t.isOn)
                .Select( t => (LessonTypes) Enum.Parse( typeof(LessonTypes), t.name) )
                .ToList();
            
            return selectedLessons;
        }

        [UsedImplicitly]
        public void CancelMatchmaking()
        {
            if (PhotonNetwork.inRoom)
            {
                PhotonNetwork.LeaveRoom();
                NetworkManager.ConnectToPhotonLobby();
            }
        }

        public override void OnShowMenu()
        {
            this._startMatchmakingButton.interactable = true;
            this._lessonSelectButtons.ForEach(t => t.interactable = true);
        }
    }
}
