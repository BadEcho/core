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

function AppendCommand([string]$command, [string]$commandSuffix){
	return [ScriptBlock]::Create($command + $commandSuffix)
}

$artifacts = ".\artifacts"

if (Test-Path $artifacts) {
	Remove-Item $artifacts -Force -Recurse
}

$buildCommand =  { & dotnet build -c Release }
$packCommand = { & dotnet pack -c Release -o $artifacts --no-build }

if($BuildIdentifier) {
	$properties = [Xml] (Get-Content .\Directory.build.props)
	$prereleaseIdentifier = [string] $properties.Project.PropertyGroup.PrereleaseIdentifier
	$prereleaseIdentifier = $prereleaseIdentifier.Trim()
	
	$versionCommand = "--version-suffix $prereleaseIdentifier.$BuildIdentifier"

	$buildCommand = AppendCommand($buildCommand.ToString(), $versionCommand)
	$packCommand = AppendCommand($packCommand.ToString(), $versionCommand)
}

Execute { & dotnet clean -c Release }
Execute $buildCommand 
Execute { & dotnet test -c Release -r $artifacts --no-build -l trx --verbosity=normal }
Execute $packCommand