$ErrorActionPreference = "Stop"

Push-Location (Join-Path $PSScriptRoot ../src/Core/Configuration)

Write-Output "Restoring tools..."

dotnet tool restore

Write-Output "Generating configuration serializer..."

dotnet xscgen "--namespace=|Configuration.xsd=FinCleaner.Configuration.Schema" --netCore Configuration.xsd

Write-Output "Finished successfully."

Pop-Location