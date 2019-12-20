using BuildingBlock.MessageQueue;

namespace RabbitMqWorker.Entities
{
    public class TestMessage : MessageBase
    {
        public TestMessage(string entityId) : base(entityId)
        {
        }
    }
}