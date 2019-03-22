using AzureTableEventSourcingTest.Domain;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureTableEventSourcingTest.Infrastructure
{
	public class AzureTableEventStore<TId, TAggregateRoot> : IEventStore<TId, TAggregateRoot>, IInitializable
        where TAggregateRoot: IAggregateRoot<TId>
	{
		private readonly CloudTable table;
		private readonly IEventPublisher eventPublisher;

		public AzureTableEventStore(CloudTableClient client, IEventPublisher eventPublisher)
		{
            if (client == null) throw new ArgumentNullException(nameof(client));

            this.table = client.GetTableReference(typeof(TAggregateRoot).Name);
            this.eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
		}

        public async Task InitializeAsync()
        {
            await table.CreateIfNotExistsAsync();
        }

        public async Task CreateStreamAsync(TId aggregateRootId, IEnumerable<IEvent> events)
		{
			await AppendToStreamAsync(aggregateRootId, VersionNumber.None, events);
		}

		public async Task<(VersionNumber, IEnumerable<IEvent>)> ReadStreamAsync(TId aggregateRootId)
		{
			var records = new List<EventRecord<TId>>();
			var query = new TableQuery<EventRecord<TId>>()
			{
				TakeCount = 1000,
				FilterString = TableQuery.GenerateFilterCondition(
					nameof(ITableEntity.PartitionKey),
					QueryComparisons.Equal,
					Convert.ToString(aggregateRootId)),
			};
			TableContinuationToken continuationToken = null;
			do
			{
				var result = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
				records.AddRange(result.Results);
				continuationToken = result.ContinuationToken;
			}
			while (continuationToken != null);

			if (records.Count == 0)
			{
				throw new StreamNotFoundException();
			}

			var version = records[records.Count - 1].VersionNumber;
			var @events = records.Select(r => r.Event);
			return (version, @events);
		}

		public async Task AppendToStreamAsync(TId aggregateRootId, VersionNumber versionNumber, IEnumerable<IEvent> events)
		{
            var batch = new TableBatchOperation();
            foreach (var @event in events)
            {
                versionNumber = versionNumber.Next;
                var record = new EventRecord<TId>(aggregateRootId, versionNumber, @event);
                batch.Add(TableOperation.Insert(record));
            }

            try
            {
                await table.ExecuteBatchAsync(batch);
            }
            catch (StorageException e) when (e.RequestInformation.HttpStatusCode == 409)
            {
                throw new ConcurrencyException();
            }

            await eventPublisher.PublishAsync(events);
        }
	}
}
