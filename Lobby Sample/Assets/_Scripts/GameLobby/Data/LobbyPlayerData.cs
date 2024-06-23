using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace GameFramework.Core.Data
{
    public class lobbyPlayerData
    {
        private string _id;
        private string _gamerTag;
        private bool _isReady;

        public string Id => _id;
        public string gamerTag => _gamerTag;
        public bool isReady
        {
            get => _isReady;
            set => _isReady = value;
        }

        public void Initialize(string id, string gamerTag)
        {
            _id = id;
            _gamerTag = gamerTag;
        }

        public void Initialize(Dictionary<string, PlayerDataObject> playerData)
        {
            UpdateState(playerData);
        }

        public void UpdateState(Dictionary<string, PlayerDataObject> playerData)
        {
            if(playerData.ContainsKey("Id")) 
            {
                _id = playerData["Id"].Value;
            } 
            if(playerData.ContainsKey("gamerTag")) 
            {
                _gamerTag = playerData["gamerTag"].Value;
            }
            if (playerData.ContainsKey("isReady"))
            {
                _isReady = playerData["isReady"].Value == "True";
            }
        }

        public Dictionary<string, string> Serialize()
        {
            return new Dictionary<string, string>
            {
                { "Id", _id },
                { "gamerTag", _gamerTag },
                { "isReady", _isReady.ToString()} // True/False not true/false
            };
        }
    }
}