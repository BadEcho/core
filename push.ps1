# Pushes Bad Echo packages to a package repository.

$scriptName = $MyInvocation.MyCommand.Name
$artifacts = ".\artifacts"

if ([string]::IsNullOrEmpty($Env:PKG_API_KEY)) {
	Write-Host "${scriptName}: PKG_API_KEY has not been set; no packages will be pushed."
}
elseif ([string]::IsNullOrEmpty($Env:PKG_URL)) {
	Write-Host "${scriptName}: PKG_URL has not been set; no packages will be pushed."
}
else {
	Get-ChildItem $artifacts -Filter "*.nupkg" | ForEach-Object {
		Write-Host "$($scriptName): Pushing $($_.Name) to repository."
		dotnet nuget push $_ --source $Env:PKG_URL --api-key $Env:PKG_API_KEY
		if ($lastexitcode -ne 0) {
			throw ("Push command errored with exit code: " + $lastexitcode)
		}
	}
}