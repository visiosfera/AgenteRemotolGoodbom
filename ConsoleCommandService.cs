using Microsoft.AspNetCore.SignalR;

namespace AgenteRemotoGoodbom
{
    public class ConsoleCommandService
    {
        private readonly IHubContext<MonitorHub> _hub;

        public ConsoleCommandService(IHubContext<MonitorHub> hub)
        {
            _hub = hub;
        }

        public async Task<(bool ok, string result)> SendAsync(
            string clientId,
            string payload,
            TimeSpan timeout,
            CancellationToken ct = default)
        {
            if (!ClientRegistry.IsOnline(clientId))
                return (false, $"Console '{clientId}' está OFFLINE (não registrado no hub).");

            var commandId = Guid.NewGuid().ToString("N");

            // cria waiter antes de enviar (pra não perder resposta rápida)
            var waitTask = CommandRpc.CreateWaiter(commandId, timeout, ct);

            await _hub.Clients.Group(clientId)
                .SendAsync("ExecuteCommand", commandId, payload, ct);

            return await waitTask;
        }
    }
}
