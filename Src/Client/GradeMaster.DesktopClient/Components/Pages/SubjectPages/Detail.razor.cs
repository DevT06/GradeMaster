﻿using BlazorBootstrap;
using GradeMaster.Client.Shared.Utility;
using GradeMaster.Common.Entities;
using GradeMaster.DataAccess.Interfaces.IRepositories;
using GradeMaster.DataAccess.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;

namespace GradeMaster.DesktopClient.Components.Pages.SubjectPages;

public partial class Detail
{
    #region Fields / Properties

    [Parameter]
    public int Id
    {
        get; set;
    }

    public Subject Subject { get; set; } = new();

    public List<Grade> Grades { get; set; } = [];

    private bool IsExpanded { get; set; } = false;

    private bool IsTruncated { get; set; } = false;

    private string ButtonText => IsExpanded ? "less" : "more";

    private string DescriptionAreaDynamicHeight => IsExpanded ? $"max-height: {_descriptionAreaExpandedHeight}px;" : "max-height: 175px;";

    private int _descriptionAreaExpandedHeight;

    private decimal _subjectAverage;

    private ConfirmDialog _dialog = default!;

    #endregion

    #region Dependency Injection

    [Inject]
    private IGradeRepository _gradeRepository
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

    [Inject]
    private NavigationManager Navigation
    {
        get; set;
    }

    [Inject]
    private IJSRuntime JSRuntime
    {
        get; set;
    }

    #endregion

    protected async override Task OnInitializedAsync()
    {
        Subject.Education = new Education(); // Initialize the Education property to avoid null reference exceptions

        var educationExists = await _subjectRepository.ExistsAsync(Id);

        if (!educationExists)
        {
            ToastService.Notify(new ToastMessage(ToastType.Info, $"This subject does no longer exist."));
            await GoBack();
            return;
        }

        await _weightRepository.GetAllAsync();
        Subject = await _subjectRepository.GetByIdDetailAsync(Id);
        await RefreshGradeData();

        // Calculate the average only after loading the education data
        CalculateSubjectAverage();
    }

    private async Task RefreshGradeData()
    {
        Grades = await _gradeRepository.GetBySubjectIdAsync(Subject.Id);
    }

    #region Description

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            IsTruncated = await JSRuntime.InvokeAsync<bool>("checkDescriptionHeight", "description-area", 175);
            await SetDescriptionAreaExpandedHeight();
            StateHasChanged();
        }
    }

    private async Task ToggleDescription()
    {
        if (!IsExpanded)
        {
            await SetDescriptionAreaExpandedHeight();
        }

        IsExpanded = !IsExpanded;

        await DynamicDescriptionHeightActive(IsExpanded);
    }

    private async Task SetDescriptionAreaExpandedHeight()
    {
        _descriptionAreaExpandedHeight = await JSRuntime.InvokeAsync<int>("getMaxDescriptionHeight", "description-text");
    }

    private async Task DynamicDescriptionHeightActive(bool isActive)
    {
        if (isActive)
        {
            await JSRuntime.InvokeVoidAsync("addDescriptionAreaEventListener");
        }
        else
        {
            await JSRuntime.InvokeVoidAsync("removeDescriptionAreaEventListener");
        }
    }

    #endregion

    #region Delete Subject

    private async Task DeleteSubjectAsync()
    {
        var options = new ConfirmDialogOptions
        {
            YesButtonColor = ButtonColor.Danger,
        };

        var confirmation = await _dialog.ShowAsync(
            title: "Are you sure you want to delete this Subject?",
            message1: $"Subject: {Subject.Name} - {Subject.Semester} of Education: {Subject.Education.Name} and all of its {Subject.Grades.Count} Grade(s) will be deleted.",
            message2: "Do you want to proceed?",
            confirmDialogOptions: options);

        if (confirmation)
        {
            try
            {
                await _subjectRepository.DeleteByIdAsync(Subject.Id);
                await GoBack(); // was await OnSubjectDeleted.InvokeAsync(Subject.Id);
                ToastService.Notify(new ToastMessage(ToastType.Success, $"Subject deleted successfully.")); // maybe add Name of deleted object
            }
            catch (Exception e)
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, $"Error deleting subject: {e.Message}"));
            }
        }
    }

    #endregion

    #region Navigation

    private async Task GoBack() => await JSRuntime.InvokeVoidAsync("window.history.back");

    private void EditSubject() => Navigation.NavigateTo($"/subjects/{Subject.Id}/edit");

    private void GoToEducation() => Navigation.NavigateTo($"/educations/{Subject.Education.Id}");

    private void GoToNewGrade(int subjectId) => Navigation.NavigateTo($"/grades/create?subjectId={subjectId}");

    #endregion

    #region Averages

    private void CalculateSubjectAverage()
    {
        // Maybe if needed add more logic here
        _subjectAverage = SubjectUtils.CalculateWeightedAverage(Grades);
    }

    #endregion

    public async ValueTask DisposeAsync()
    {
        await JSRuntime.InvokeVoidAsync("removeDescriptionAreaEventListener");
    }
}