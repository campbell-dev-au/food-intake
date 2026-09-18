using System;

namespace FoodIntake.App.ViewModels;

public enum StatusSeverity
{
    Success,
    Error
}

public sealed record StatusMessage(string Text, StatusSeverity Severity)
{
    public static StatusMessage Success(string text) => new(text, StatusSeverity.Success);

    public static StatusMessage Error(string text) => new(text, StatusSeverity.Error);

    public static StatusMessage Exception(string text, Exception ex) => new($"{text}: {ex.Message}", StatusSeverity.Error);

    public bool IsSuccess => Severity == StatusSeverity.Success;

    public bool IsError => Severity == StatusSeverity.Error;
}
