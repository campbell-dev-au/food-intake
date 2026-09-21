using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    private bool _revertingAssignment;

    public ObservableCollection<Project> Projects { get; } = [];
    public ObservableCollection<TimePointListItem> TimePoints { get; } = [];
    public ObservableCollection<SchemeAssignment> SchemeAssignments { get; } = [];

    [ObservableProperty]
    public partial TimePointListItem? SelectedTimePoint { get; set; }

    [ObservableProperty]
    public partial string NewTimePointName { get; set; } = "";

    [ObservableProperty]
    public partial Project? SelectedProject { get; set; }

    [ObservableProperty]
    public partial StatusMessage? Status { get; set; }

    public async Task LoadProjectsAsync()
    {
        try
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            Projects.ReplaceAll(await db.Projects.OrderBy(p => p.Name).ToListAsync());
        }
        catch (Exception ex)
        {
            Status = StatusMessage.Error($"Couldn't load projects: {ex.Message}");
        }
    }

    public async Task UploadAsync(string filePath)
    {
        Status = null;

        if (SelectedTimePoint is null)
        {
            Status = StatusMessage.Error("Select a time point first.");
            return;
        }

        if (SelectedTimePoint.HasUpload)
        {
            Status = StatusMessage.Error("This time point already has an upload. Clear it first.");
            return;
        }

        try
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            var summary = await FoodWorksImporter.ImportAsync(db, SelectedTimePoint.Id, filePath);

            SelectedTimePoint.Upload = await db.Uploads
                .SingleAsync(u => u.TimePointId == SelectedTimePoint.Id);

            Status = StatusMessage.Success($"Imported {summary.LineCount} lines across {summary.RecordCount} records.");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            Status = StatusMessage.Error($"Couldn't import {ex.GetBaseException().Message}");
        }
    }

    partial void OnSelectedProjectChanged(Project? value)
    {
        _ = LoadTimePointsAsync();
        _ = LoadSchemeAssignmsnAsync();
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
            Status = StatusMessage.Error($"Couldn't load time points: {ex.Message}");
        }
    }

    public async Task AddProjectAsync(string name)
    {
        Status = null;
        if (name.Length == 0)
            return;

        try
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();

            if (await db.Projects.AnyAsync(p => p.Name == name))
            {
                Status = StatusMessage.Error($"A project named '{name}' already exists.");
                return;
            }

            var project = new Project { Name = name };
            db.Projects.Add(project);
            await db.SaveChangesAsync();

            Projects.Add(project);
            SelectedProject = project;
        }
        catch (Exception ex)
        {
            Status = StatusMessage.Error($"Couldn't add project: {ex.Message}");
        }
    }

    public async Task RenameProjectAsync(string name)
    {
        Status = null;

        if (SelectedProject is null)
        {
            Status = StatusMessage.Error("Select a project first.");
            return;
        }

        int projectId = SelectedProject.Id;

        try
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();

            if (await db.Projects.AnyAsync(p => p.Name == name && p.Id != projectId))
            {
                Status = StatusMessage.Error($"A project named '{name}' already exists.");
                return;
            }

            Project project = await db.Projects.SingleAsync(p => p.Id == projectId);
            project.Name = name;
            await db.SaveChangesAsync();

            await LoadProjectsAsync();
            SelectedProject = Projects.FirstOrDefault(p => p.Id == projectId);
        }
        catch (Exception ex)
        {
            Status = StatusMessage.Exception("Couldn't rename project", ex);
        }
    }

    [RelayCommand]
    public async Task ClearUploadAsync()
    {
        Status = null;
        if (SelectedTimePoint is null || !SelectedTimePoint.HasUpload)
            return;

        try
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            var timePointId = SelectedTimePoint.Id;

            await db.FoodRecords.Where(r => r.TimePointId == timePointId).ExecuteDeleteAsync();
            await db.Uploads.Where(u => u.TimePointId == timePointId).ExecuteDeleteAsync();

            SelectedTimePoint.Upload = null;
            Status = StatusMessage.Success("Cleared.");
        }
        catch (Exception ex)
        {
            Status = StatusMessage.Error($"Couldn't clear: {ex.Message}");
        }
    }

    public async Task<TimePointDataViewModel?> OpenSelectedTimePointAsync()
    {
        Status = null;

        if (SelectedTimePoint is null)
        {
            Status = StatusMessage.Error("Select a time point first.");
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
            Status = StatusMessage.Error($"Couldn't open time point: {ex.Message}");
            return null;
        }
    }

    [RelayCommand]
    private async Task AddTimePointAsync()
    {
        Status = null;

        if (SelectedProject is null)
        {
            Status = StatusMessage.Error("Select a project first.");
            return;
        }

        var name = NewTimePointName.Trim();
        if (name.Length == 0)
            return;

        try
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();

            if (await db.TimePoints.AnyAsync(
                t => t.ProjectId == SelectedProject.Id && t.Name == name
            ))
            {
                Status = StatusMessage.Error($"'{name}' already exists for this project.");
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
            Status = StatusMessage.Exception("Couldn't add time point", ex);
        }
    }

    private async Task ApplySchemeAssignmentAsync(SchemeAssignment assignment)
    {
        Status = null;

        if (SelectedProject is null)
            return;

        int projectId = SelectedProject.Id;

        try
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();

            if (assignment.IsAssigned)
            {
                db.ProjectSchemes.Add(new ProjectScheme { ProjectId = projectId, SchemeId = assignment.Id });
                await db.SaveChangesAsync();
            }
            else
            {
                await db.ProjectSchemes
                    .Where(ps => ps.ProjectId == projectId && ps.SchemeId == assignment.Id)
                    .ExecuteDeleteAsync();
            }
        }
        catch (Exception ex)
        {
            Status = StatusMessage.Exception("Couldn't update scheme assignment", ex);

            _revertingAssignment = true;
            assignment.IsAssigned = !assignment.IsAssigned;
            _revertingAssignment = false;
        }
    }

    private void OnSchemeAssignmentChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_revertingAssignment)
            return;

        if (e.PropertyName != nameof(SchemeAssignment.IsAssigned))
            return;

        if (sender is not SchemeAssignment assignment)
            return;

        _ = ApplySchemeAssignmentAsync(assignment);
    }

    private async Task LoadSchemeAssignmsnAsync()
    {
        foreach (SchemeAssignment existing in SchemeAssignments)
            existing.PropertyChanged -= OnSchemeAssignmentChanged;

        SchemeAssignments.Clear();

        if (SelectedProject is null)
            return;

        try
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();

            List<Scheme> schemes = await db.Schemes.OrderBy(s => s.Name).ToListAsync();

            HashSet<int> assignedIds = await db.ProjectSchemes
                .Where(ps => ps.ProjectId == SelectedProject.Id)
                .Select(ps => ps.SchemeId)
                .ToHashSetAsync();

            Dictionary<int, int> categoryCounts = await db.Categories
                .GroupBy(c => c.SchemeId)
                .Select(g => new { SchemeId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.SchemeId, g => g.Count);

            foreach (Scheme scheme in schemes)
            {
                SchemeAssignment assignment = new(scheme)
                {
                    IsAssigned = assignedIds.Contains(scheme.Id),
                    CategoryCount = categoryCounts.GetValueOrDefault(scheme.Id)
                };

                assignment.PropertyChanged += OnSchemeAssignmentChanged;
                SchemeAssignments.Add(assignment);
            }
        }
        catch (Exception ex)
        {
            Status = StatusMessage.Exception("Couldn't load scheme assignment", ex);
        }
    }
}
