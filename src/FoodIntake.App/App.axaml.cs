using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FoodIntake.App.ViewModels;
using FoodIntake.Data;

namespace FoodIntake.App;

public partial class App : Application
{
  public override void Initialize()
  {
    AvaloniaXamlLoader.Load(this);
  }

  public override void OnFrameworkInitializationCompleted()
  {
    var dataDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "FoodIntake");
    Directory.CreateDirectory(dataDir);
    var dbPath = Path.Combine(dataDir, "food.db");

    var services = new ServiceCollection();
    services.AddDbContextFactory<AppDbContext>(o => o.UseSqlite($"Data Source={dbPath}"));
    var provider = services.BuildServiceProvider();
    var dbContextFactory = provider.GetRequiredService<IDbContextFactory<AppDbContext>>();

    using (var db = dbContextFactory.CreateDbContext())
    {
      db.Database.Migrate();
    }

    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
      var viewModel = new ProjectsViewModel(dbContextFactory);
      desktop.MainWindow = new MainWindow { DataContext = viewModel };
      _ = viewModel.LoadProjectsAsync();
    }
  }
}
