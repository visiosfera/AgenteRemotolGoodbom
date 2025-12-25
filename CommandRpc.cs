using System.Collections.Concurrent;

namespace AgenteRemotoGoodbom
{
    public static class CommandRpc
    {
        private record Waiter(TaskCompletionSource<(bool ok, string result)> Tcs, CancellationTokenRegistration Ctr);

        private static readonly ConcurrentDictionary<string, Waiter> _waiters = new();

        public static Task<(bool ok, string result)> CreateWaiter(string commandId, TimeSpan timeout, CancellationToken ct = default)
        {
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            linkedCts.CancelAfter(timeout);

            var tcs = new TaskCompletionSource<(bool ok, string result)>(TaskCreationOptions.RunContinuationsAsynchronously);

            var ctr = linkedCts.Token.Register(() =>
            {
                if (_waiters.TryRemove(commandId, out var w))
                    w.Tcs.TrySetResult((false, "TIMEOUT aguardando resposta do console"));
            });

            _waiters[commandId] = new Waiter(tcs, ctr);
            return tcs.Task;
        }

        public static void Complete(string commandId, bool ok, string result)
        {
            if (_waiters.TryRemove(commandId, out var w))
            {
                w.Ctr.Dispose();
                w.Tcs.TrySetResult((ok, result));
            }
        }
    }
}
