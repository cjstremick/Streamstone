﻿using System.Collections.Generic;

using Azure;
using Azure.Data.Tables;

using NUnit.Framework;

using System;

namespace Streamstone.Scenarios
{
    [TestFixture]
    public class Custom_stream_properties
    {
        [Test]
        public void When_passing_property_with_reserved_name()
        {
            var reserved = new Dictionary<string, object>
            {
                { nameof(ITableEntity.PartitionKey), "MyPartitionKey" },
                { nameof(ITableEntity.RowKey), "MyRowKey" },
                { nameof(ITableEntity.Timestamp), DateTimeOffset.UtcNow },
                { nameof(ITableEntity.ETag), new ETag() },
                { "odata.etag", new ETag() }
            };

            var properties = StreamProperties.From(reserved);

            Assert.That(properties.Count, Is.EqualTo(0), 
                "Should skip all properties with reserved names, such as RowKey, Id, etc");
        }
    }
}