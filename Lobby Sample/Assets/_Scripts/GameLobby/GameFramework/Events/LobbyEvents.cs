using Unity.Services.Lobbies.Models;

namespace GameFramework.Core.Events
{
    public static class LobbyEvents
    {
        public delegate void LobbyUpdated(Lobby lobby);
        public static LobbyUpdated onLobbyUpdated;
    }
}