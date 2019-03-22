using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureTableEventSourcingTest.Domain;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace AzureTableEventSourcingTest.Infrastructure
{
	public class AzureServiceBusEventPublisher : IEventPublisher
	{
		private readonly MessageSender messageSender;

		public AzureServiceBusEventPublisher(MessageSender messageSender)
		{
			this.messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
		}

		public async Task PublishAsync(IEnumerable<IEvent> events)
		{
			if (events == null) throw new ArgumentNullException(nameof(events));

			var messages = events.Select(ToMessage).ToList();
			await messageSender.SendAsync(messages);
		}

		private Message ToMessage(IEvent @event)
		{
			var body = EventSerializer.ToBytes(@event);
			var message = new Message(body);
			return message;
		}
	}
}
