using System.Collections.Concurrent;

namespace AgenteRemotoGoodbom
{
    public static class ClientRegistry
    {
        private static readonly ConcurrentDictionary<string, string> _clientToConn = new();
        private static readonly ConcurrentDictionary<string, string> _connToClient = new();

        public static void MarkOnline(string clientId, string connectionId)
        {
            _clientToConn[clientId] = connectionId;
            _connToClient[connectionId] = clientId;
        }

        public static void MarkOfflineByConnectionId(string connectionId)
        {
            if (_connToClient.TryRemove(connectionId, out var clientId))
            {
                _clientToConn.TryRemove(clientId, out _);
            }
        }

        public static bool IsOnline(string clientId) => _clientToConn.ContainsKey(clientId);
    }
}
