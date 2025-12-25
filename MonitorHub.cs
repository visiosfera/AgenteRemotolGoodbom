using Microsoft.AspNetCore.SignalR;
using System;
using System.Text.RegularExpressions;

namespace AgenteRemotoGoodbom
{
    public class MonitorHub : Hub
    {
        // Console chama isso ao conectar para se registrar
        public async Task Register(string clientId)
        {
            // Coloca essa conexão no "grupo" do clientId
            await Groups.AddToGroupAsync(Context.ConnectionId, clientId);

            // Opcional: mantém um "mapa" pra saber que está online
            ClientRegistry.MarkOnline(clientId, Context.ConnectionId);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            // Opcional: remove do registry se quiser (depende se você quer manter histórico)
            ClientRegistry.MarkOfflineByConnectionId(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        // Console chama isso para devolver a resposta de um comando
        public Task SendResponse(string clientId, string commandId, string result, bool ok = true)
        {
            CommandRpc.Complete(commandId, ok, result);
            return Task.CompletedTask;
        }
    }
}
