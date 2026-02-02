using System;
using System.Collections.Generic;
using System.Text;

namespace MyProject.Interfaces;

public interface IEntity
{
    Guid Id { get; set; }
    DateTime CreatedAt { get; set; }
}
