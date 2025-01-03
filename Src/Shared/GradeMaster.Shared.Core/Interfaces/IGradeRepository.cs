﻿using GradeMaster.Shared.Core.Entities;
using GradeMaster.Shared.Core.GenericInterfaces;

namespace GradeMaster.Shared.Core.Interfaces;

public interface IGradeRepository : IGenericEntityRepository<Grade>
{
    Task<List<Grade>> GetBySubjectIdsAsync(List<int> id);
}