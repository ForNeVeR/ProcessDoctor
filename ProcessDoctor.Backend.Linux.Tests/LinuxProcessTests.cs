using System.Runtime.InteropServices;
using FluentAssertions;

namespace ProcessDoctor.Backend.Linux.Tests;

public sealed class LinuxProcessTests
{
    [SkippableTheory]
    [InlineData("/usr/bin/htop")]
    public void Should_extract_icon(string executablePath)
    {
        Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Linux));

        // Arrange
        var processEntry = FakeProcessEntry.Create(id: 123u, executablePath: executablePath);
        var sut = LinuxProcess.Create(processEntry);

        // Act
        using var bitmap = sut.ExtractIcon();

        // Assert
        bitmap
            .Bytes
            .Should()
            .NotBeEmpty();
    }

    [SkippableFact]
    public void Should_extract_stock_icon()
    {
        Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Linux));

        // Arrange
        var processEntry = FakeProcessEntry.Create(id: 123u);
        var sut = LinuxProcess.Create(processEntry);

        // Act
        using var bitmap = sut.ExtractIcon();

        // Assert
        bitmap
            .Bytes
            .Should()
            .NotBeEmpty();
    }
}
