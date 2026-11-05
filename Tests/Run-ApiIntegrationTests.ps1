param(
    [string]$SqlServer = 'localhost',
    [switch]$NoBuild
)

$ErrorActionPreference = 'Stop'
$previousConnection = $env:KAHRAD_TEST_SQL_SERVER
$testExitCode = 1
try {
    if ([string]::IsNullOrWhiteSpace($previousConnection)) {
        $testConnection = New-Object System.Data.SqlClient.SqlConnectionStringBuilder
        $testConnection['Data Source'] = $SqlServer
        $testConnection['Initial Catalog'] = 'master'
        $testConnection['Integrated Security'] = $true
        $testConnection['TrustServerCertificate'] = $true
        $testConnection['Connect Timeout'] = 5
        $env:KAHRAD_TEST_SQL_SERVER = $testConnection.get_ConnectionString()
    }
    $testProject = Join-Path $PSScriptRoot 'IntegrationTests/Kahrad.Pricing.Api.IntegrationTests/Kahrad.Pricing.Api.IntegrationTests.csproj'
    $testArguments = @('test', $testProject, '--verbosity', 'minimal')
    if ($NoBuild) { $testArguments += @('--no-build', '--no-restore') }
    & dotnet @testArguments
    $testExitCode = $LASTEXITCODE
}
finally {
    $env:KAHRAD_TEST_SQL_SERVER = $previousConnection
}
exit $testExitCode
