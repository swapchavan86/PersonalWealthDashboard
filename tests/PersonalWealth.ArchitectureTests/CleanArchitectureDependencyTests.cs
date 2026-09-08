using System.Reflection;
using Xunit;

namespace PersonalWealth.ArchitectureTests;

public sealed class CleanArchitectureDependencyTests
{
    private static readonly Assembly Domain = typeof(PersonalWealth.DomainAssemblyMarker).Assembly;
    private static readonly Assembly Application = typeof(PersonalWealth.ApplicationAssemblyMarker).Assembly;
    private static readonly Assembly Infrastructure = typeof(PersonalWealth.InfrastructureAssemblyMarker).Assembly;

    [Fact]
    public void Domain_does_not_reference_outer_layers_or_infrastructure()
    {
        Assert.DoesNotContain(ReferencedAssemblyNames(Domain),
            name => IsForbiddenForDomain(name));
    }

    [Fact]
    public void Application_does_not_reference_outer_layers()
    {
        Assert.DoesNotContain(ReferencedAssemblyNames(Application),
            name => IsForbiddenForApplication(name));
    }

    [Fact]
    public void Infrastructure_references_only_inward_project_layers()
    {
        var references = ReferencedAssemblyNames(Infrastructure);

        Assert.Contains("PersonalWealth.Application", references);
        Assert.Contains("PersonalWealth.Domain", references);
        Assert.DoesNotContain("PersonalWealth.Api", references);
    }

    [Theory]
    [InlineData("PersonalWealth.Api")]
    [InlineData("PersonalWealth.Infrastructure")]
    [InlineData("Microsoft.EntityFrameworkCore")]
    [InlineData("Microsoft.Data.SqlClient")]
    [InlineData("System.IO.FileSystem")]
    [InlineData("OpenAI")]
    public void Domain_forbidden_dependency_catalog_is_enforced(string dependency)
    {
        Assert.True(IsForbiddenForDomain(dependency));
    }

    [Theory]
    [InlineData("PersonalWealth.Api")]
    [InlineData("PersonalWealth.Infrastructure")]
    public void Application_forbidden_dependency_catalog_is_enforced(string dependency)
    {
        Assert.True(IsForbiddenForApplication(dependency));
    }

    private static IReadOnlySet<string> ReferencedAssemblyNames(Assembly assembly) =>
        assembly.GetReferencedAssemblies()
            .Select(reference => reference.Name!)
            .ToHashSet(StringComparer.Ordinal);

    private static bool IsForbiddenForDomain(string name) =>
        name is "PersonalWealth.Api"
            or "PersonalWealth.Infrastructure"
            or "Microsoft.EntityFrameworkCore"
            or "Microsoft.Data.SqlClient"
            or "System.IO.FileSystem"
            or "OpenAI"
            or "Microsoft.Extensions.AI"
            or "Microsoft.Extensions.Hosting";

    private static bool IsForbiddenForApplication(string name) =>
        name is "PersonalWealth.Api"
            or "PersonalWealth.Infrastructure"
            or "Microsoft.EntityFrameworkCore"
            or "Microsoft.Data.SqlClient"
            or "System.IO.FileSystem"
            or "OpenAI"
            or "Microsoft.Extensions.AI";
}
