﻿using BlazorBootstrap;
using GradeMaster.Client.Shared.Utility;
using GradeMaster.Common.Entities;
using GradeMaster.DataAccess.Interfaces.IRepositories;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace GradeMaster.DesktopClient.Components.Pages.EducationPages;

public partial class Detail : IAsyncDisposable
{
    #region Fields / Properties

    [Parameter]
    public int Id
    {
        get; set;
    }

    public Education Education { get; set; } = new();

    public List<Subject> Subjects { get; set; } = [];

    private bool IsExpanded { get; set; } = false;

    private bool IsTruncated { get; set; } = false;

    private string ButtonText => IsExpanded ? "less" : "...more";

    private string DescriptionAreaDynamicHeight => IsExpanded ? $"max-height: {_descriptionAreaExpandedHeight}px;" : "max-height: 175px;";

    private int _descriptionAreaExpandedHeight;

    private decimal _educationAverage;

    private ConfirmDialog _dialog = default!;

    private BlazorBootstrap.Button _copyButton = default!;

    private DotNetObjectReference<Detail>? _objRef;

    #endregion

    #region Dependency Injection

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
        var educationExists = await _educationRepository.ExistsAsync(Id);

        if (!educationExists)
        {
            ToastService.Notify(new ToastMessage(ToastType.Info, $"This education does no longer exist."));
            await GoBack();
            return;
        }

        _objRef = DotNetObjectReference.Create(this);
        await JSRuntime.InvokeVoidAsync("addPageKeybinds", "EducationDetailPage", _objRef);

        await _weightRepository.GetAllAsync();
        Education = await _educationRepository.GetByIdAsync(Id);
        await RefreshSubjectData();

        // Calculate the average only after loading the data
        CalculateEducationAverage();
    }

    private async Task RefreshSubjectData()
    {
        Subjects = await _subjectRepository.GetByEducationIdOrderedAsync(Education.Id);
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

    #region Clipboard

    private async Task CopyToClipboard()
    {
        //_copyButton.ShowLoading();
        _copyButton.Loading = true;

        var textToCopy = $"[Education with Id: {Education.Id}](/educations/{Education.Id})";
        await Clipboard.SetTextAsync(textToCopy);

        //ToastService.Notify(new ToastMessage(ToastType.Success, "Copied page link to clipboard"));

        await Task.Delay(3000);

        //_copyButton.HideLoading();
        _copyButton.Loading = false;
    }

    #endregion

    #region Delete Education

    private async Task DeleteEducationAsync()
    {
        var options = new ConfirmDialogOptions
        {
            YesButtonColor = ButtonColor.Danger,
        };

        var confirmation = await _dialog.ShowAsync(
            title: "Are you sure you want to delete this Education?",
            message1: $"Education: {Education.Name}, {Education.Subjects.Count} Subject(s) and all related Objects will be deleted.",
            message2: "Do you want to proceed?",
            confirmDialogOptions: options);

        if (confirmation)
        {
            try
            {
                await _educationRepository.DeleteByIdAsync(Education.Id);
                await GoBack(); // was await OnEducationDeleted.InvokeAsync(Education.Id);
                ToastService.Notify(new ToastMessage(ToastType.Success, $"Education deleted successfully.")); // maybe add Name of deleted object
            }
            catch (Exception e)
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, $"Error deleting education: {e.Message}"));
            }
        }
    }

    #endregion

    #region Not used

    //invoke js function to scroll to top
    //protected async override Task OnAfterRenderAsync(bool firstRender)
    //{
    //    if (firstRender)
    //    {
    //        await JSRuntime.InvokeVoidAsync("scrollToTop");
    //    }
    //}

    #endregion

    #region Navigation

    private async Task GoBack() => await JSRuntime.InvokeVoidAsync("window.history.back");

    private void EditEducation() => Navigation.NavigateTo($"/educations/{Education.Id}/edit");

    private void GoToNewSubject(int educationId) => Navigation.NavigateTo($"/subjects/create?educationId={educationId}");

    #endregion

    #region JSInvokable / Keybinds

    [JSInvokable]
    public void NavigateToEdit() => EditEducation();

    [JSInvokable]
    public void NavigateToCreate()
    {
        if (Education.Completed)
        {
            ToastService.Notify(new ToastMessage(ToastType.Info, $"Education is completed"));
            return;
        }

        GoToNewSubject(Education.Id);
    }

    [JSInvokable]
    public async Task DeleteObject() => await DeleteEducationAsync();

    [JSInvokable]
    public async Task ToggleDescriptionHeight()
    {
        if (IsTruncated)
        {
            await ToggleDescription();
            StateHasChanged();
        }
    }

    [JSInvokable]
    public async Task CopyPageUrl()
    {
        ToastService.Notify(new ToastMessage(ToastType.Success, "Copied page link to clipboard"));

        await CopyToClipboard();
    }

    #endregion

    #region Averages

    private void CalculateEducationAverage()
    {
        // Maybe if needed add more logic here
        _educationAverage = EducationUtils.CalculateEducationAverage(Subjects);
    }

    #endregion

    public async ValueTask DisposeAsync()
    {
        await JSRuntime.InvokeVoidAsync("removeDescriptionAreaEventListener");

        await JSRuntime.InvokeVoidAsync("removePageKeybinds", "EducationDetailPage");
        _objRef?.Dispose();
    }
}