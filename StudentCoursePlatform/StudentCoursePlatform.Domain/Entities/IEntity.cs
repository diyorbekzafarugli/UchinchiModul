using System;
using System.Collections.Generic;
using System.Text;

namespace StudentCoursePlatform.Domain.Entities;

public interface IEntity
{
    public Guid Id { get; set; }
}
