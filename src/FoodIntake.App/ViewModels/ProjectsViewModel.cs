using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodIntake.Data;
using FoodIntake.Domain;
using Microsoft.EntityFrameworkCore;

namespace FoodIntake.App.ViewModels;

public partial class ProjectsViewModel(IDbContextFactory<AppDbContext> dbContextFactory) : ObservableObject
{
  private readonly IDbContextFactory<AppDbContext> _dbContextFactory = dbContextFactory;

  public ObservableCollection<Project> Projects { get; } = [];
  public ObservableCollection<TimePoint> TimePoints { get; } = [];

  [ObservableProperty]
  public partial string NewProjectName { get; set; } = "";

  [ObservableProperty]
  public partial string NewTimePointName { get; set; } = "";

  [ObservableProperty]
  public partial Project? SelectedProject { get; set; }

  [ObservableProperty]
  public partial string? ErrorMessage { get; set; }

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
      ErrorMessage = $"Couldn't load projects: {ex.Message}";
    }
  }

  partial void OnSelectedProjectChanged(Project? value)
  {
    _ = LoadTimePointsAsync();
  }

  private async Task LoadTimePointsAsync()
  {
    TimePoints.Clear();
    if (SelectedProject is null)
      return;

    try
    {
      await using var db = await _dbContextFactory.CreateDbContextAsync();
      var timePoints = await db.TimePoints
        .Where(t => t.ProjectId == SelectedProject.Id)
        .OrderBy(t => t.SortOrder)
        .ToListAsync();

      foreach (var timePoint in timePoints)
        TimePoints.Add(timePoint);
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Couldn't load time points: {ex.Message}";
    }
  }

  [RelayCommand]
  private async Task AddProjectAsync()
  {
    ErrorMessage = null;
    var name = NewProjectName.Trim();
    if (name.Length == 0)
      return;

    try
    {
      await using var db = await _dbContextFactory.CreateDbContextAsync();

      if (await db.Projects.AnyAsync(p => p.Name == name))
      {
        ErrorMessage = $"A project named '{name}' already exists.";
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
      ErrorMessage = $"Couldn't add project: {ex.Message}";
    }
  }

  [RelayCommand]
  private async Task AddTimePointAsync()
  {
    ErrorMessage = null;

    if (SelectedProject is null)
    {
      ErrorMessage = "Select a project first.";
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
        ErrorMessage = $"'{name}' already exists for this project.";
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

      TimePoints.Add(timePoint);
      NewTimePointName = "";
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Couldn't add time point: {ex.Message}";
    }
  }
}
