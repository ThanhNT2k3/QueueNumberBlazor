using System;

namespace QMS.Web.Services
{
    public class QmsEventService
    {
        private static int _instanceCount = 0;
        private readonly int _instanceId;

        public QmsEventService()
        {
            _instanceId = ++_instanceCount;
            Console.WriteLine($"[QmsEventService] Instance #{_instanceId} created. Total instances: {_instanceCount}");
        }

        public event Action<int, bool>? OnCounterUpdated;
        public event Action<int, int, string>? OnCounterAssigned;
        public event Action<int>? OnCounterUnassigned;

        public void TriggerCounterUpdated(int counterId, bool isActive)
        {
            Console.WriteLine($"[QmsEventService #{_instanceId}] TriggerCounterUpdated called: CounterId={counterId}, IsActive={isActive}, Subscribers={OnCounterUpdated?.GetInvocationList().Length ?? 0}");
            OnCounterUpdated?.Invoke(counterId, isActive);
        }

        public void TriggerCounterAssigned(int counterId, int userId, string userName)
        {
            Console.WriteLine($"[QmsEventService] TriggerCounterAssigned called: CounterId={counterId}, UserId={userId}, Subscribers={OnCounterAssigned?.GetInvocationList().Length ?? 0}");
            OnCounterAssigned?.Invoke(counterId, userId, userName);
        }

        public void TriggerCounterUnassigned(int counterId)
        {
            Console.WriteLine($"[QmsEventService] TriggerCounterUnassigned called: CounterId={counterId}, Subscribers={OnCounterUnassigned?.GetInvocationList().Length ?? 0}");
            OnCounterUnassigned?.Invoke(counterId);
        }
    }
}
