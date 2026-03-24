using System;

namespace SimpleCtlModelChecker.CSharp.Tests;

internal static class ExceptionExtensions
{
    public static string? InspectForMessage(this Exception ex, string messageSubstring)
        => ex.Message.Contains(messageSubstring)
            ? null
            : $"Exception message '{ex.Message}' does not contain '{messageSubstring}'";
}
