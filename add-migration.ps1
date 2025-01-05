Param(
    [Parameter(Mandatory=$True)]
    [string]$Name
)
Push-Location
cd src/GtKasse.Core
dotnet ef migrations add $Name --startup-project ../GtKasse.Ui/
Pop-Location