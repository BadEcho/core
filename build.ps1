# Builds the Bad Echo solution.

param (
	[string]$BuildIdentifier
)

function Execute([scriptblock]$command) {
	& $command
	if ($lastexitcode -ne 0) {
		throw ("Build command errored with exit code: " + $lastexitcode)
	}
}

$artifacts = ".\artifacts"

if (Test-Path $artifacts) {
	Remove-Item $artifacts -Force -Recurse
}

Execute { & dotnet clean -c Release }

if ($BuildIdentifier) {
	$properties = [Xml] (Get-Content .\Directory.build.props)
	$prereleaseIdentifier = [string] $properties.Project.PropertyGroup.PrereleaseIdentifier
	$prereleaseIdentifier = $prereleaseIdentifier.Trim()
	$versionSuffix = "$prereleaseIdentifier.$BuildIdentifier"

	Write-Output "Here is version: $versionSuffix"
	
	Execute { & dotnet build -c Release --version-suffix $versionSuffix }
	Execute { & dotnet pack -c Release -o $artifacts --no-build --version-suffix $versionSuffix }
}
else {
	Execute { & dotnet build -c Release }
	Execute { & dotnet pack -c Release -o $artifacts --no-build }
}