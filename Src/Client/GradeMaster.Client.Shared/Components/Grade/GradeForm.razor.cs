﻿using BlazorBootstrap;
using GradeMaster.Common.Entities;
using GradeMaster.Common.Enums;
using GradeMaster.DataAccess.Interfaces.IRepositories;
using GradeMaster.Logic.Interfaces.IServices;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

using Entities = GradeMaster.Common.Entities;

namespace GradeMaster.Client.Shared.Components.Grade;

public partial class GradeForm
{
    #region Fields / Properties

    [Parameter]
    public FormType FormType
    {
        get; set;
    }
    [Parameter]
    public Entities.Grade? Grade
    {
        get; set;
    }
    [Parameter]
    public EventCallback<Entities.Grade> OnSave
    {
        get; set;
    }
    [Parameter]
    public int? SubjectId
    {
        get; set;
    }

    private string FormTitle { get; set; } = "Grade Form";

    public Entities.Grade NewGrade { get; set; } = new();

    public List<Weight> Weights { get; set; } = new();

    public List<Entities.Subject> Subjects { get; set; } = new();

    private int SelectedWeightId
    {
        get => NewGrade.Weight?.Id ?? 0;
        set
        {
            if (Weights.Count != 0)
            {
                NewGrade.Weight = Weights.FirstOrDefault(e => e.Id == value)!;
            }
        }
    }

    private int SelectedSubjectId
    {
        get => NewGrade.Subject?.Id ?? 0;
        set
        {
            if (Subjects.Count != 0)
            {
                NewGrade.Subject = Subjects.FirstOrDefault(e => e.Id == value)!;
            }
        }
    }

    private EditContext? _editContext;

    #endregion

    #region Dependency Injection

    [Inject]
    private IJSRuntime JSRuntime
    {
        get; set;
    }

    [Inject]
    private NavigationManager Navigation
    {
        get; set;
    }

    [Inject]
    private IGradeService _gradeService
    {
        get; set;
    }

    [Inject]
    private IEducationRepository _educationRepository
    {
        get; set;
    }

    [Inject]
    private ISubjectRepository _subjectRepository
    {
        get; set;
    }

    [Inject]
    private IWeightRepository _weightRepository
    {
        get; set;
    }

    [Inject] protected ToastService ToastService { get; set; } = default!;

    #endregion

    protected async override Task OnInitializedAsync()
    {
        try
        {
            Weights = await _weightRepository.GetAllAsync();

            if (Weights == null || Weights.Count == 0)
            {
                throw new InvalidOperationException("No weights found.");
            }

            if (!SubjectId.HasValue)
            {

                Subjects = await _subjectRepository.GetByCompletedAsync(false);
                Subjects = Subjects.OrderByDescending(e => e.Id).ToList();
                if (FormType == FormType.Edit && Grade != null)
                {
                    // can never be null
                    var subject = await _subjectRepository.GetByGradeIdAsync(Grade.Id);

                    if (!Subjects.Exists(e => e.Id == subject.Id))
                    {
                        Subjects.Insert(0, subject);
                    }
                }
            }
            else
            {
                Subjects.Add(await _subjectRepository.GetByIdDetailAsync(SubjectId.Value));
            }

            if (Subjects == null || Subjects.Count == 0)
            {
                throw new InvalidOperationException("No subjects found.");
            }

            if (Grade == null)
            {
                // Initialize Grade for creation
                Grade = new Entities.Grade
                {
                    Date = DateOnly.FromDateTime(DateTime.Today),
                    Weight = default!,
                    Subject = SubjectId.HasValue ? Subjects.First() : default!
                };

                FormTitle = "Create New Grade";
            }
            else
            {
                FormTitle = "Edit Grade";
            }

            _gradeService.PassObjectAttributes(NewGrade, Grade);

            _editContext = new EditContext(NewGrade);
        }
        catch (Exception ex)
        {
            ToastService.Notify(new ToastMessage(ToastType.Danger, $"Error during initialization: {ex.Message}"));
            throw;
        }
    }

    #region HandleSubmit

    private async Task HandleValidSubmit()
    {
        if (NewGrade.Weight == null || NewGrade.Weight.Id == 0)
        {
            ToastService.Notify(new ToastMessage(ToastType.Warning, "Please select a valid weight."));
            return;
        }

        if (NewGrade.Subject == null || NewGrade.Subject.Id == 0)
        {
            ToastService.Notify(new ToastMessage(ToastType.Warning, "Please select a valid subject."));
            return;
        }

        if (NewGrade.Value < 1)
        {
            NewGrade.Value = 1;
            ToastService.Notify(new ToastMessage(ToastType.Warning, "Please enter a valid grade value (1 - 6)."));
            return;
        }

        if (OnSave.HasDelegate)
        {
            _gradeService.PassObjectAttributes(Grade, NewGrade, true);
            await OnSave.InvokeAsync(Grade);
        }
        else
        {
            // Optional fallback logic
            ToastService.Notify(new ToastMessage(ToastType.Danger, $"Grade could not be saved."));
        }
    }

    private void HandleInvalidSubmit()
    {
        ToastService.Notify(new ToastMessage(ToastType.Warning, $"Grade form is not valid."));
    }

    #endregion

    private async Task Cancel() => await JSRuntime.InvokeVoidAsync("window.history.back");
}