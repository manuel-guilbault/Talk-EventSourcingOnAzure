using AzureTableEventSourcingTest.Domain;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace AzureTableEventSourcingTest.Infrastructure
{
	public class EventRecord<TId>: ITableEntity
    {
        private static readonly TypeConverter aggregateRootIdTypeConverter = TypeDescriptor.GetConverter(typeof(TId));
        private static TId DeserializeAggregateRootId(string partitionKey) => (TId)aggregateRootIdTypeConverter.ConvertFromInvariantString(partitionKey);
        private static string SerializeAggregateRootId(TId aggregateRootId) => aggregateRootIdTypeConverter.ConvertToInvariantString(aggregateRootId);
        private static VersionNumber DeserializeVersionNumber(string rowKey) => new VersionNumber(int.Parse(rowKey, CultureInfo.InvariantCulture));
        private static string SerializeVersionNumber(VersionNumber versionNumber) => versionNumber.Value.ToString("D10", CultureInfo.InvariantCulture);

        private string partitionKey;
        private string rowKey;

        [Obsolete("This constructor should not be used directly, it is intended to be used only by the Microsoft.Azure.Cosmos.Table package.")]
        public EventRecord()
        {
        }

        public EventRecord(TId aggregateRootId, VersionNumber versionNumber, IEvent @event)
		{
            AggregateRootId = aggregateRootId;
			VersionNumber = versionNumber;
			Event = @event;
		}
		
        string ITableEntity.PartitionKey
        {
            get => partitionKey;
            set => partitionKey = value;
        }

        string ITableEntity.RowKey
        {
            get => rowKey;
            set => rowKey = value;
        }

        DateTimeOffset ITableEntity.Timestamp { get; set; }
        string ITableEntity.ETag { get; set; }

        public TId AggregateRootId
        {
            get => DeserializeAggregateRootId(partitionKey);
            private set => partitionKey = SerializeAggregateRootId(value);
        }

        public VersionNumber VersionNumber
        {
            get => DeserializeVersionNumber(rowKey);
            private set => rowKey = SerializeVersionNumber(value);
        }

        public IEvent Event { get; private set; }

        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            Event = EventSerializer.FromString(properties[nameof(Event)].StringValue);
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            return new Dictionary<string, EntityProperty>
            {
                [nameof(Event)] = EntityProperty.GeneratePropertyForString(EventSerializer.ToString(Event)),
            };
        }
    }
}
