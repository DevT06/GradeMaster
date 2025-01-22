﻿using GradeMaster.Shared.Core.Entities;
using GradeMaster.Shared.Core.GenericInterfaces;

namespace GradeMaster.Shared.Core.Interfaces;

public interface IEducationRepository : IGenericEntityRepository<Education>
{
    Task<Education?> GetBySubjectIdAsync(int id);

    Task<List<Education>> GetBySubjectIdsAsync(List<int> ids);
    // implement custom methods here, like searching by name, etc.

    Task<List<Education>> GetByCompletedAsync(bool completed);

    Task<List<Education>> GetBySearchWithLimitAsync(string searchValue, int startIndex, int amount);

    Task<int> GetTotalCountAsync(string searchValue);
}