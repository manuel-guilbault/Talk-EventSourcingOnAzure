using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureTableEventSourcingTest.Domain
{
	public interface IEventStore<TId, TAggregateRoot>
		where TAggregateRoot: IAggregateRoot<TId>
	{
		Task CreateStreamAsync(TId aggregateRootId, IEnumerable<IEvent> events);
		Task<(VersionTag version, IEnumerable<IEvent> events)> ReadStreamAsync(TId aggregateRootId);
		Task AppendToStreamAsync(TId aggregateRootId, VersionTag version, IEnumerable<IEvent> events);
	}
	
	public static class EventStoreExtensions
	{
		public static async Task Update<TId, TAggregateRoot>(
			this IEventStore<TId, TAggregateRoot> store,
			TId id,
			Func<IEnumerable<IEvent>, TAggregateRoot> hydrate,
			Func<TAggregateRoot, IEnumerable<IEvent>> mutate)
			where TAggregateRoot: IAggregateRoot<TId>
		{
			var (version, pastEvents) = await store.ReadStreamAsync(id);
			var aggregateRoot = hydrate(pastEvents);
			var newEvents = mutate(aggregateRoot);
			await store.AppendToStreamAsync(id, version, newEvents);
		}
	}
}
