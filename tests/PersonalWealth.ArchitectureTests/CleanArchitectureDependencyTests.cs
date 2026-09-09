using System.Reflection;
using System.Xml.Linq;
using Xunit;

namespace PersonalWealth.ArchitectureTests;

public sealed class CleanArchitectureDependencyTests
{
    private static readonly Assembly Api = Assembly.Load("PersonalWealth.Api");
    private static readonly Assembly Application = Assembly.Load("PersonalWealth.Application");
    private static readonly Assembly Domain = Assembly.Load("PersonalWealth.Domain");
    private static readonly Assembly Infrastructure = Assembly.Load("PersonalWealth.Infrastructure");
    private static readonly Assembly Worker = Assembly.Load("PersonalWealth.Worker");

    [Fact]
    public void Domain_has_no_project_dependencies()
    {
        AssertProjectDependencies(Domain);
    }

    [Fact]
    public void Application_depends_only_on_domain()
    {
        AssertProjectDependencies(Application, "PersonalWealth.Domain");
    }

    [Fact]
    public void Api_depends_on_application_contracts_and_infrastructure()
    {
        AssertProjectDependencies(
            Api,
            "PersonalWealth.Application",
            "PersonalWealth.Contracts",
            "PersonalWealth.Infrastructure");
    }

    [Fact]
    public void Infrastructure_depends_only_inward_on_application_and_domain()
    {
        AssertProjectDependencies(Infrastructure, "PersonalWealth.Application", "PersonalWealth.Domain");
    }

    [Fact]
    public void Worker_depends_on_application_and_infrastructure()
    {
        AssertProjectDependencies(Worker, "PersonalWealth.Application", "PersonalWealth.Infrastructure");
    }

    [Fact]
    public void Domain_has_no_forbidden_framework_or_infrastructure_dependencies()
    {
        Assert.DoesNotContain(ReferencedAssemblyNames(Domain), IsForbiddenDependency);
    }

    [Fact]
    public void Application_has_no_forbidden_framework_or_infrastructure_dependencies()
    {
        Assert.DoesNotContain(ReferencedAssemblyNames(Application), IsForbiddenDependency);
    }

    [Theory]
    [InlineData("PersonalWealth.Api")]
    [InlineData("PersonalWealth.Application")]
    [InlineData("PersonalWealth.Infrastructure")]
    [InlineData("Microsoft.EntityFrameworkCore")]
    [InlineData("Microsoft.AspNetCore.Http")]
    [InlineData("Microsoft.Data.SqlClient")]
    [InlineData("System.IO.FileSystem")]
    [InlineData("OpenAI")]
    [InlineData("Microsoft.Extensions.AI")]
    [InlineData("MassTransit")]
    public void Forbidden_dependency_names_are_rejected(string dependency)
    {
        Assert.True(IsForbiddenDependency(dependency));
    }

    private static void AssertProjectDependencies(
        Assembly assembly,
        params string[] allowedProjectDependencies)
    {
        var allowed = allowedProjectDependencies.ToHashSet(StringComparer.Ordinal);
        var projectDependencies = ProjectReferenceNames(assembly)
            .ToHashSet(StringComparer.Ordinal);

        Assert.True(
            projectDependencies.SetEquals(allowed),
            $"{assembly.GetName().Name} project dependencies were [{string.Join(", ", projectDependencies.Order())}], expected [{string.Join(", ", allowed.Order())}].");
    }

    private static IReadOnlySet<string> ProjectReferenceNames(Assembly assembly)
    {
        var projectName = assembly.GetName().Name
            ?? throw new InvalidOperationException("Assembly name is required to locate the project file.");
        var projectFile = Path.Combine(FindRepositoryRoot(), "src", projectName, $"{projectName}.csproj");

        if (!File.Exists(projectFile))
        {
            throw new FileNotFoundException($"Project file was not found for {projectName}.", projectFile);
        }

        var projectDirectory = Path.GetDirectoryName(projectFile)!;
        var document = XDocument.Load(projectFile);

        return document
            .Descendants("ProjectReference")
            .Select(reference => reference.Attribute("Include")?.Value)
            .Where(include => !string.IsNullOrWhiteSpace(include))
            .Select(include => Path.GetFullPath(Path.Combine(projectDirectory, include!)))
            .Where(File.Exists)
            .Select(path => Path.GetFileNameWithoutExtension(path))
            .Where(name => name.StartsWith("PersonalWealth.", StringComparison.Ordinal))
            .ToHashSet(StringComparer.Ordinal);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "PersonalWealth.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Repository root containing PersonalWealth.sln was not found.");
    }

    private static IReadOnlySet<string> ReferencedAssemblyNames(Assembly assembly) =>
        assembly.GetReferencedAssemblies()
            .Select(reference => reference.Name!)
            .ToHashSet(StringComparer.Ordinal);

    private static bool IsForbiddenDependency(string name) =>
        name is "PersonalWealth.Api"
            or "PersonalWealth.Application"
            or "PersonalWealth.Infrastructure"
            or "Microsoft.EntityFrameworkCore"
            or "Microsoft.Data.SqlClient"
            or "OpenAI"
            or "MassTransit"
            || name.StartsWith("Microsoft.AspNetCore.", StringComparison.Ordinal)
            || name.StartsWith("Microsoft.Extensions.", StringComparison.Ordinal)
            || name.StartsWith("System.IO.", StringComparison.Ordinal);
}
