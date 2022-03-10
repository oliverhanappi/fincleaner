$ErrorActionPreference = "Stop"

Push-Location (Join-Path $PSScriptRoot "..")

Write-Output "Restoring packages..."

dotnet restore

Write-Output "Building solution..."

dotnet build --configuration Release --no-restore

Write-Output "Generating artifacts..."

if (Test-Path artifacts) {
    Remove-Item artifacts -Recurse -Force
}

dotnet publish --configuration Release --runtime win-x64 --no-self-contained --nologo --output artifacts ./src/Console/Console.csproj

Write-Output "Build succeeded."