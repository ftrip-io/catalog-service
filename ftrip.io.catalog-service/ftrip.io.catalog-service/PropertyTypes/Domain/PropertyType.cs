﻿using ftrip.io.framework.Domain;
using System;

namespace ftrip.io.catalog_service.PropertyTypes.Domain
{
    public class PropertyType : Entity<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
