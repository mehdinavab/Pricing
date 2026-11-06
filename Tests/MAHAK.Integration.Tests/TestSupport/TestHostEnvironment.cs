using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace MAHAK.Integration.Tests.TestSupport;

internal sealed class TestHostEnvironment(string environmentName = "Development") : IHostEnvironment
{
    public string EnvironmentName { get; set; } = environmentName;
    public string ApplicationName { get; set; } = "MAHAK.Integration.Tests";
    public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
}
