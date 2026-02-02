using System;
using System.Collections.Generic;
using System.Text;

namespace MyProejct.Entities;

public interface IEntity
{
    Guid Id { get; set; }
    DateTime CreatedAt { get; set; }
}
