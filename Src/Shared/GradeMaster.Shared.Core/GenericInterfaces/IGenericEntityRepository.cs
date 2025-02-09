﻿namespace GradeMaster.Shared.Core.GenericInterfaces;

public interface IGenericEntityRepository<T>
{
    Task<T?> GetByIdAsync(int id);

    Task<List<T>> GetAllAsync();

    Task<T> AddAsync(T t);

    Task UpdateAsync(int id, T t);

    Task DeleteByIdAsync(int id);

    Task<bool> ExistsAnyAsync();
}