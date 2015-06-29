$here = (Get-Item $MyInvocation.MyCommand.Path).Directory
#$here | Get-Member | Write-Host
$solutoinDir = $here.Parent.FullName
$modulePath = Join-Path $solutoinDir 'Format-Razor-Cmdlet\bin\Debug\Format-Razor.dll'

Import-Module $modulePath

Describe "Format-Razor parameters validation" {

    $pwd = (pwd)
    
    It "Model object cannot be null" {
        { Format-Razor -Model $null -Template "aa" } | Should Throw
    }

    It "Template cannot be null" {
        $pwd = (pwd)
        { Format-Razor -Model $pwd -Template $null } | Should Throw
    }

    It "Template cannot be empty" {
        { Format-Razor -Model $pwd -Template "" } | Should Throw
    }

    It "TemplatePath cannot be empty" {
        { Format-Razor -Model $pwd -TemplatePath "" } | Should Throw
    }

    It "TemplatePath cannot be null" {
        { Format-Razor -Model $pwd -TemplatePath $null } | Should Throw
    }

    It "TemplatePath and Template are exclusive paramsets" {
        { Format-Razor -Model $pwd -TemplatePath "templatePath" -Template "Template"} | Should Throw
    }

    It "TemplatePath must exist" {
        { Format-Razor -Model $pwd -TemplatePath "templatePath" } | Should Throw
    }

    It "Array Model passed, array result returned" {
        $model = (1,2,3,4,5)
        $result = ($model | Format-Razor -Template "@Model-@Model")
        
        $result  | Should BeExactly ("1-1", "2-2", "3-3", "4-4", "5-5")
        $result.Length | Should BeExactly 5
    }

    It "String model passed, string result returned"{
        $model = "Element"
        $result = ($model | Format-Razor -Template "@Model.Length")
        $result | Should BeExactly 7
    }
}