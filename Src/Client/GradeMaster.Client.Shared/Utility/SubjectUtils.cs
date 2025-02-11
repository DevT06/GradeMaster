﻿using GradeMaster.Common.Entities;

namespace GradeMaster.Client.Shared.Utility;

public static class SubjectUtils
{
    /// <summary>
    /// Calculates the weighted average of the given grades.
    /// </summary>
    /// <param name="grades"></param>
    /// <returns>decimal</returns>
    public static decimal CalculateWeightedAverage(ICollection<Grade> grades)
    {
        if (grades == null || !grades.Any())
        {
            return 0;
        }

        decimal totalWeight = grades.Sum(g => g.Weight?.Value ?? 1); // Default weight to 1 if null
        if (totalWeight == 0)
        {
            return 0;
        }

        decimal weightedSum = grades.Sum(g => g.Value * (g.Weight?.Value ?? 1)); // Default weight to 1 if null
        return weightedSum / totalWeight;
    }
}