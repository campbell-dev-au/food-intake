using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodIntake.Data;
using FoodIntake.Domain;
using Microsoft.EntityFrameworkCore;

namespace FoodIntake.App.ViewModels;

public partial class SchemesViewModel(
    IDbContextFactory<AppDbContext> dbContextFactory
) : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory = dbContextFactory;

    public ObservableCollection<Scheme> Schemes { get; } = [];
    public ObservableCollection<Category> Categories { get; } = [];

    [ObservableProperty]
    public partial Scheme? SelectedScheme { get; set; }

    [ObservableProperty]
    public partial string NewSchemeName { get; set; } = "";

    [ObservableProperty]
    public partial Category? SelectedCategory { get; set; }

    [ObservableProperty]
    public partial string NewCategoryName { get; set; } = "";

    [ObservableProperty]
    public partial StatusMessage? Status { get; set; }

    public async Task LoadSchemesAsync()
    {
        try
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            Schemes.ReplaceAll(await db.Schemes.OrderBy(s => s.Name).ToListAsync());
        }
        catch (Exception ex)
        {
            Status = StatusMessage.Exception("Couldn't load schemes", ex);
        }
    }

    private async Task LoadCategoriesAsync()
    {
        Categories.Clear();
        SelectedCategory = null;
        if (SelectedScheme is null)
            return;

        try
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            List<Category> dbCategories = await db.Categories
                .Where(c => c.SchemeId == SelectedScheme.Id)
                .OrderBy(c => c.Name)
                .ToListAsync();

            Categories.ReplaceAll(dbCategories);
        }
        catch (Exception ex)
        {
            Status = StatusMessage.Exception("Couldn't load categories", ex);
        }
    }

    partial void OnSelectedSchemeChanged(Scheme? value)
    {
        _ = LoadCategoriesAsync();
    }

    [RelayCommand]
    private async Task AddSchemeAsync()
    {
        Status = null;
        string name = NewSchemeName.Trim();
        if (name.Length == 0)
        {
            Status = StatusMessage.Error("Enter a name");
            return;
        }

        try
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();

            if (await db.Schemes.AnyAsync(s => s.Name == name))
            {
                Status = StatusMessage.Error($"A scheme named '{name}' already exists.");
                return;
            }

            Scheme scheme = new() { Name = name };
            db.Schemes.Add(scheme);
            await db.SaveChangesAsync();

            Schemes.Add(scheme);
            NewSchemeName = "";
            SelectedScheme = scheme;
        }
        catch (Exception ex)
        {
            Status = StatusMessage.Exception("Couldn't add scheme", ex);
        }
    }

    [RelayCommand]
    private async Task AddCategoryAsync()
    {
        Status = null;

        if (SelectedScheme is null)
        {
            Status = StatusMessage.Error("Select a scheme first.");
            return;
        }

        string name = NewCategoryName.Trim();
        if (name.Length == 0)
        {
            Status = StatusMessage.Error("Enter a name");
            return;
        }

        try
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();

            if (await db.Categories.AnyAsync(
                c => c.SchemeId == SelectedScheme.Id && c.Name == name && c.ParentId == null
            ))
            {
                Status = StatusMessage.Error($"'{name}' already exists for this scheme and level");
                return;
            }

            Scheme scheme = await db.Schemes.SingleAsync(s => s.Id == SelectedScheme.Id);
            Category category = new()
            {
                SchemeId = scheme.Id,
                Scheme = scheme,
                Name = name,
                ParentId = null, // @TODO: add sub category feature
                Parent = null
            };
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            Categories.Add(category);
            NewCategoryName = "";
            SelectedCategory = category;
        }
        catch (Exception ex)
        {
            Status = StatusMessage.Exception("Couldn't add category", ex);
        }
    }
}
