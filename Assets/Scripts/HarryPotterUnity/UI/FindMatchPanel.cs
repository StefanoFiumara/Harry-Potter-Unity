using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace HarryPotterUnity.UI
{
    public class FindMatchPanel : MonoBehaviour
    {
        private List<Toggle> _lessonSelectButtons;
        private SubMenuManager _subMenuManager;
        private ErrorPanel _errorPanel;

        private Button _startMatchmakingButton;
        private Button _cancelMatchmakingButton;

        private Text _findMatchStatus;

        private void Awake()
        {
            _lessonSelectButtons = FindObjectsOfType<Toggle>().ToList();
            
            _subMenuManager = FindObjectOfType<SubMenuManager>();
            _errorPanel = FindObjectOfType<ErrorPanel>();

            var allButtons = FindObjectsOfType<Button>().ToList();
            _startMatchmakingButton = allButtons.Find(b => b.name.Contains("StartMatchmakingButton"));
            _cancelMatchmakingButton = allButtons.Find(b => b.name.Contains("BackToMainMenuButton"));

            _findMatchStatus = FindObjectsOfType<Text>().Single(t => t.name.Contains("FindMatchStatus"));
        }

        [UsedImplicitly]
        public void StartMatchmaking()
        {
            var selectedLessons = GetSelectedLessons();
            
            if (selectedLessons.Count == 2 || selectedLessons.Count == 3)
            {
                var lessonBytes = selectedLessons.Select(n => (byte)n).ToArray();

                var selected = new Hashtable { { "lessons", lessonBytes } };
                PhotonNetwork.player.SetCustomProperties(selected);

                PhotonNetwork.JoinRandomRoom();

                _startMatchmakingButton.interactable = false;
                _cancelMatchmakingButton.interactable = false;

                _lessonSelectButtons.ForEach(t => t.interactable = false);
            }
            else
            {
                _errorPanel.Title = "Error!";
                _errorPanel.Message = "You must choose two or three lesson types to enter matchmaking!";
                _subMenuManager.ShowMenu(_errorPanel);
            }
        }

        [UsedImplicitly]
        public void OnJoinedRoom()
        {
            _cancelMatchmakingButton.interactable = true;
            _findMatchStatus.GetComponent<Animator>().SetBool("IsFindingMatch", true);
        }

        [UsedImplicitly]
        public void OnLeftRoom()
        {
            _findMatchStatus.GetComponent<Animator>().SetBool("IsFindingMatch", false);
        }
        

        private List<LessonTypes> GetSelectedLessons()
        {
            var selectedLessons = _lessonSelectButtons
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

            _startMatchmakingButton.interactable = true;
            _lessonSelectButtons.ForEach(t => t.interactable = true);
        }
    }
}
