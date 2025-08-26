﻿using System;

namespace shorten.url.domain
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
