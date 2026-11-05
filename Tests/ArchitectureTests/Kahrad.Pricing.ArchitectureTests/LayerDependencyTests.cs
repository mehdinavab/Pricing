using System.Xml.Linq;
using Xunit;
namespace Kahrad.Pricing.ArchitectureTests;
public sealed class LayerDependencyTests
{
    private static string Root
    {
        get
        {
            var folder = new DirectoryInfo(AppContext.BaseDirectory);
            while (folder is not null && !File.Exists(Path.Combine(folder.FullName,"Kahrad.Pricing.slnx"))) folder = folder.Parent;
            return folder?.FullName ?? throw new InvalidOperationException("Solution root not found.");
        }
    }
    private static string[] Projects => Directory.GetFiles(Path.Combine(Root,"Src"),"*.csproj",SearchOption.AllDirectories);
    private static string[] References(string path) => XDocument.Load(path).Descendants("ProjectReference")
        .Select(x=>Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path)!,x.Attribute("Include")!.Value))).ToArray();

    [Fact]
    public void Domain_and_application_do_not_reference_outer_project_layers()
    {
        foreach(var project in Projects)
        {
            var name = Path.GetFileNameWithoutExtension(project);
            if (name.EndsWith(".Domain") || name.EndsWith(".Application"))
                Assert.DoesNotContain(References(project), p=>p.Contains(".Infrastructure",StringComparison.OrdinalIgnoreCase) || p.Contains("\\Hosts\\",StringComparison.OrdinalIgnoreCase));
            if (name.EndsWith(".Domain")) Assert.DoesNotContain(References(project),p=>p.Contains(".Application",StringComparison.OrdinalIgnoreCase));
        }
    }
    [Fact]
    public void Pricing_engine_has_no_persistence_http_cache_or_job_dependencies()
    {
        var engine = Projects.Single(x=>Path.GetFileName(x)=="Kahrad.Pricing.Pricing.Engine.csproj");
        var dependencies = XDocument.Load(engine).Descendants().Where(x=>x.Name.LocalName is "ProjectReference" or "PackageReference" or "FrameworkReference")
            .Select(x=>x.Attribute("Include")?.Value ?? "").ToArray();
        Assert.DoesNotContain(dependencies,p=>new[]{"Infrastructure","EntityFramework","SqlClient","Redis","Caching","Hangfire","AspNetCore","AutoMapper"}.Any(x=>p.Contains(x,StringComparison.OrdinalIgnoreCase)));
    }
    [Fact]
    public void Project_graph_has_no_cycles()
    {
        var completed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var active = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        void Visit(string project)
        {
            if(completed.Contains(project)) return;
            Assert.True(active.Add(project),"Circular project reference: "+project);
            foreach(var reference in References(project)) Visit(reference);
            active.Remove(project); completed.Add(project);
        }
        foreach(var project in Projects) Visit(project);
    }
    [Fact]
    public void Package_versions_are_centralized_and_prohibited_patterns_are_absent()
    {
        foreach(var project in Projects)
        {
            var document = XDocument.Load(project);
            foreach(var package in document.Descendants("PackageReference"))
            {
                Assert.Null(package.Attribute("Version"));
                Assert.DoesNotContain(package.Attribute("Include")!.Value,new[]{"MediatR","MassTransit","RabbitMQ.Client","Confluent.Kafka"});
            }
        }
    }
}
