using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using FoodIntake.Data;
using FoodIntake.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FoodIntake.App.ViewModels;

public partial class FoodCategorisationViewModel(
    IDbContextFactory<AppDbContext> dbContextFactory
) : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory = dbContextFactory;
    private int? _projectId;
    private bool _revertingChoice;

    public ObservableCollection<Scheme> Schemes { get; } = [];
    public ObservableCollection<FoodCategoryAssignment> Foods { get; } = [];

    private List<CategoryChoice> _choices = [CategoryChoice.None];

    [ObservableProperty]
    public partial Scheme? SelectedScheme { get; set; }

    [ObservableProperty]
    public partial StatusMessage? Status { get; set; }

    private async Task ApplyChoiceAsync(FoodCategoryAssignment assignment)
    {
        Status = null;

        if (_projectId is null || SelectedScheme is null)
            return;

        int projectId = _projectId.Value;
        int schemeId = SelectedScheme.Id;

        try
        {
            await using AppDbContext db = await _dbContextFactory.CreateDbContextAsync();
            await using IDbContextTransaction transaction = await db.Database.BeginTransactionAsync();

            // Delete existing assignment
            await db.FoodCategories
                .Where(fc => fc.ProjectId == projectId
                    && fc.FoodId == assignment.FoodId
                    && fc.SchemeId == schemeId)
                .ExecuteDeleteAsync();

            if (assignment.SelectedChoice.Category is { } category)
            {
                db.FoodCategories.Add(new FoodCategory
                {
                    ProjectId = projectId,
                    FoodId = assignment.FoodId,
                    CategoryId = category.Id,
                    SchemeId = schemeId
                });
                await db.SaveChangesAsync();
            }

            await transaction.CommitAsync();

            assignment.LastSavedChoice = assignment.SelectedChoice;
        }
        catch (Exception ex)
        {
            Status = StatusMessage.Exception("Couldn't save category", ex);

            _revertingChoice = true;
            assignment.SelectedChoice = assignment.LastSavedChoice;
            _revertingChoice = false;
        }
    }

    private void OnAssignmentChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_revertingChoice)
            return;

        if (e.PropertyName != nameof(FoodCategoryAssignment.SelectedChoice))
            return;

        if (sender is not FoodCategoryAssignment assignment)
            return;

        _ = ApplyChoiceAsync(assignment);
    }

    private async Task LoadFoodsAsync()
    {
        foreach (FoodCategoryAssignment existing in Foods)
            existing.PropertyChanged -= OnAssignmentChanged;

        Foods.Clear();

        if (_projectId is null || SelectedScheme is null)
        {
            _choices = [CategoryChoice.None];
            return;
        }

        int projectId = _projectId.Value;
        int schemeId = SelectedScheme.Id;

        try
        {
            await using AppDbContext db = await _dbContextFactory.CreateDbContextAsync();

            List<Category> categories = await db.Categories
                .Where(c => c.SchemeId == schemeId)
                .OrderBy(c => c.Name)
                .ToListAsync();

            _choices = [CategoryChoice.None, .. categories.Select(c => new CategoryChoice(c))];

            List<int> categoryIds = [.. categories.Select(c => c.Id)];

            Dictionary<int, int> assigned = await db.FoodCategories
                .Where(fc => fc.ProjectId == projectId && fc.SchemeId == schemeId)
                .ToDictionaryAsync(fc => fc.FoodId, fc => fc.CategoryId);

            List<ProjectFood> foods = await db.Lines
                .Where(l => l.FoodRecord.ProjectId == projectId)
                .GroupBy(l => new { l.FoodId, l.Food.Name })
                .OrderBy(g => g.Key.Name)
                .Select(g => new ProjectFood(g.Key.FoodId, g.Key.Name, g.Count()))
                .ToListAsync();

            foreach (ProjectFood food in foods)
            {
                FoodCategoryAssignment assignment = new(food, _choices);

                if (assigned.TryGetValue(food.FoodId, out int categoryId))
                {
                    assignment.SelectedChoice = _choices
                        .First(c => c.Category?.Id == categoryId);
                }

                assignment.LastSavedChoice = assignment.SelectedChoice;
                assignment.PropertyChanged += OnAssignmentChanged;
                Foods.Add(assignment);
            }

        }
        catch (Exception ex)
        {
            Status = StatusMessage.Exception("Couldn't load foods", ex);
        }
    }


    partial void OnSelectedSchemeChanged(Scheme? value)
    {
        _ = LoadFoodsAsync();
    }

    public async Task LoadAsync(int? projectId)
    {
        _projectId = projectId;
        Status = null;

        SelectedScheme = null;
        Schemes.Clear();

        if (projectId is null)
            return;

        try
        {
            await using AppDbContext db = await _dbContextFactory.CreateDbContextAsync();

            List<Scheme> schemes = await db.ProjectSchemes
                .Where(ps => ps.ProjectId == projectId)
                .Select(ps => ps.Scheme!)
                .OrderBy(s => s.Name)
                .ToListAsync();

            Schemes.ReplaceAll(schemes);
            SelectedScheme = Schemes.Count == 1 ? Schemes[0] : null;
        }
        catch (Exception ex)
        {
            Status = StatusMessage.Exception("Couldn't load schemes", ex);
        }
    }
}
