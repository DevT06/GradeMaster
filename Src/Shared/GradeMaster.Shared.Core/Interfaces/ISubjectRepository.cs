﻿using GradeMaster.Shared.Core.Entities;
using GradeMaster.Shared.Core.GenericInterfaces;

namespace GradeMaster.Shared.Core.Interfaces;

public interface ISubjectRepository : IGenericEntityRepository<Subject>
{
    Task<List<Subject>> GetByEducationIdAsync(int id);
}