﻿using System;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos.Table;

using Newtonsoft.Json;

using Streamstone;

namespace Example.Scenarios
{
    public class S06_Include_additional_entities : Scenario
    {
        public override async Task RunAsync()
        {
            var existent = await Stream.TryOpenAsync(Partition);
	        var stream = existent.Found ? existent.Stream : new Stream(Partition);

            Console.WriteLine("Writing to new stream along with making snapshot in partition '{0}'", 
                              stream.Partition);

            var snapshot = Include.Insert(new InventoryItemShapshot
            {
                RowKey = "SNAPSHOT",
                Name = "iPhone7",
                Count = 100 - 50 - 40,
                Version = 5
            });

            var events = new[]
            {
                Event(new InventoryItemCreated(Id, "iPhone6")),
                Event(new InventoryItemCheckedIn(Id, 100)),
                Event(new InventoryItemCheckedOut(Id, 50)),
                Event(new InventoryItemRenamed(Id, "iPhone6", "iPhone7")),
                Event(new InventoryItemCheckedOut(Id, 40), snapshot)
            };

            var result = await Stream.WriteAsync(stream, events);

            Console.WriteLine("Succesfully written to new stream.\r\nEtag: {0}, Version: {1}",
                              result.Stream.ETag, result.Stream.Version);
        }

        static EventData Event(object @event, params Include[] includes)
        {
            var id = Guid.NewGuid();

            var properties = new
            {
                Type = @event.GetType().Name,
                Data = JsonConvert.SerializeObject(@event)
            };

            return new EventData(
                            EventId.From(id), 
                            EventProperties.From(properties), 
                            EventIncludes.From(includes));
        }

        class InventoryItemShapshot : TableEntity
        {
            public string Name { get; set; }
            public int Count   { get; set; }
            public int Version { get; set; }
        }
    }
}
