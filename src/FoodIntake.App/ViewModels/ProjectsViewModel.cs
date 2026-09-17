using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodIntake.Data;
using FoodIntake.Domain;
using FoodIntake.Import;
using Microsoft.EntityFrameworkCore;

namespace FoodIntake.App.ViewModels;

public partial class ProjectsViewModel(IDbContextFactory<AppDbContext> dbContextFactory) : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory = dbContextFactory;

    public ObservableCollection<Project> Projects { get; } = [];
    public ObservableCollection<TimePointListItem> TimePoints { get; } = [];

    [ObservableProperty]
    public partial TimePointListItem? SelectedTimePoint { get; set; }

    [ObservableProperty]
    public partial string NewProjectName { get; set; } = "";

    [ObservableProperty]
    public partial string NewTimePointName { get; set; } = "";

    [ObservableProperty]
    public partial Project? SelectedProject { get; set; }

    [ObservableProperty]
    public partial string? StatusMessage { get; set; }

    public async Task LoadProjectsAsync()
    {
        try
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            var projects = await db.Projects.OrderBy(p => p.Name).ToListAsync();

            Projects.Clear();
            foreach (var project in projects)
                Projects.Add(project);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Couldn't load projects: {ex.Message}";
        }
    }

    public async Task UploadAsync(string filePath)
    {
        StatusMessage = null;

        if (SelectedTimePoint is null)
        {
            StatusMessage = "Select a time point first.";
            return;
        }

        if (SelectedTimePoint.HasUpload)
        {
            StatusMessage = "This time point already has an upload. Clear it first.";
            return;
        }

        try
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            var summary = await FoodWorksImporter.ImportAsync(db, SelectedTimePoint.Id, filePath);

            SelectedTimePoint.Upload = await db.Uploads
                .SingleAsync(u => u.TimePointId == SelectedTimePoint.Id);

            StatusMessage = $"Imported {summary.LineCount} lines across {summary.RecordCount} records.";
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            StatusMessage = $"Couldn't import {ex.GetBaseException().Message}";
        }
    }

    partial void OnSelectedProjectChanged(Project? value)
    {
        _ = LoadTimePointsAsync();
    }

    private async Task LoadTimePointsAsync()
    {
        TimePoints.Clear();
        SelectedTimePoint = null;
        if (SelectedProject is null)
            return;

        try
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            var timePoints = await db.TimePoints
              .Where(t => t.ProjectId == SelectedProject.Id)
              .OrderBy(t => t.SortOrder)
              .ToListAsync();

            var uploads = await db.Uploads
                .Where(u => u.TimePoint.ProjectId == SelectedProject.Id)
                .ToDictionaryAsync(u => u.TimePointId);

            foreach (var timePoint in timePoints)
            {
                TimePoints.Add(new TimePointListItem(timePoint)
                {
                    Upload = uploads.GetValueOrDefault(timePoint.Id)
                });
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Couldn't load time points: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task AddProjectAsync()
    {
        StatusMessage = null;
        var name = NewProjectName.Trim();
        if (name.Length == 0)
            return;

        try
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();

            if (await db.Projects.AnyAsync(p => p.Name == name))
            {
                StatusMessage = $"A project named '{name}' already exists.";
                return;
            }

            var project = new Project { Name = name };
            db.Projects.Add(project);
            await db.SaveChangesAsync();

            Projects.Add(project);
            NewProjectName = "";
            SelectedProject = project;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Couldn't add project: {ex.Message}";
        }
    }

    [RelayCommand]
    public async Task ClearUploadAsync()
    {
        StatusMessage = null;
        if (SelectedTimePoint is null || !SelectedTimePoint.HasUpload)
            return;

        try
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            var timePointId = SelectedTimePoint.Id;

            await db.FoodRecords.Where(r => r.TimePointId == timePointId).ExecuteDeleteAsync();
            await db.Uploads.Where(u => u.TimePointId == timePointId).ExecuteDeleteAsync();

            SelectedTimePoint.Upload = null;
            StatusMessage = "Cleared.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Couldn't clear: {ex.Message}";
        }
    }

    public async Task<TimePointDataViewModel?> OpenSelectedTimePointAsync()
    {
        StatusMessage = null;

        if (SelectedTimePoint is null)
        {
            StatusMessage = "Select a time point first.";
            return null;
        }

        try
        {
            var viewModel = new TimePointDataViewModel(_dbContextFactory);
            await viewModel.LoadAsync(SelectedTimePoint.Id);
            return viewModel;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Couldn't open time point: {ex.Message}";
            return null;
        }
    }

    [RelayCommand]
    private async Task AddTimePointAsync()
    {
        StatusMessage = null;

        if (SelectedProject is null)
        {
            StatusMessage = "Select a project first.";
            return;
        }

        var name = NewTimePointName.Trim();
        if (name.Length == 0)
            return;

        try
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();

            if (await db.TimePoints.AnyAsync(t => t.ProjectId == SelectedProject.Id && t.Name == name))
            {
                StatusMessage = $"'{name}' already exists for this project.";
                return;
            }

            var project = await db.Projects.SingleAsync(p => p.Id == SelectedProject.Id);
            var sortOrder = await db.TimePoints.CountAsync(t => t.ProjectId == SelectedProject.Id);

            var timePoint = new TimePoint
            {
                ProjectId = project.Id,
                Project = project,
                Name = name,
                SortOrder = sortOrder
            };
            db.TimePoints.Add(timePoint);
            await db.SaveChangesAsync();

            TimePoints.Add(new TimePointListItem(timePoint));
            NewTimePointName = "";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Couldn't add time point: {ex.Message}";
        }
    }
}
