﻿using GradeMaster.Common.Entities;

namespace GradeMaster.Client.Shared.Utility;

/// <summary>
/// Education utility class.
/// </summary>
public static class EducationUtils
{
    /// <summary>
    /// Calculates the weighted average of the given grades.
    /// </summary>
    /// <param name="grades"></param>
    /// <returns>calculated average as decimal</returns>
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

    /// <summary>
    /// Calculates the Education average with the given subjects.
    /// </summary>
    /// <param name="subjects"></param>
    /// <returns>calculated education average as decimal</returns>
    public static decimal CalculateEducationAverage(List<Subject> subjects)
    {
        if (subjects == null || !subjects.Any())
        {
            return 0;
        }

        var subjectAverages = subjects
            .Where(s => s.Grades != null && s.Grades.Any())
            .Select(s => CalculateWeightedAverage(s.Grades));

        if (!subjectAverages.Any())
        {
            return 0;
        }

        return subjectAverages.Average();
    }

    ///// <summary>
    ///// Returns the completion state of the given boolean.
    ///// </summary>
    ///// <param name="completed"></param>
    ///// <returns>Completion state as a string</returns>
    //public static string CompletionState(bool completed)
    //{
    //    return completed ? "Completed" : "In Progress";
    //}
}