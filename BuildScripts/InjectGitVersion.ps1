# InjectGitVersion.ps1
#
# Set the version in the projects AssemblyInfo.cs file
#

$gitTag = "1.0"
$gitCommitComment = git log -1 --pretty=%B;
$gitVersion = git describe --long --always;
$gitDateTime = git log -n 1 --pretty=%cD;
$gitCount = git rev-list --all --count;

$gitCommitComment = $gitCommitComment -replace '"', '\"'

# Define file variables
$assemblyFile = $args[0] + "\Properties\AssemblyInfo.cs";
$templateFile =  $args[0] + "\Properties\AssemblyInfo_template.cs";

# Read template file, overwrite place holders with git version info
$newAssemblyContent = Get-Content $templateFile |
    %{$_ -replace '\$FILEVERSION\$', ($gitTag + "." + $gitCount) } |
    %{$_ -replace '\$INFOVERSION\$', ($gitTag + "." + $gitCount + "-" + $gitVersion + "|" + $gitDateTime + "|" + $gitCommitComment) };

# Write AssemblyInfo.cs file only if there are changes
If (-not (Test-Path $assemblyFile) -or ((Compare-Object (Get-Content $assemblyFile) $newAssemblyContent))) {
    echo "Injecting Git Version Info to AssemblyInfo.cs"
    $newAssemblyContent > $assemblyFile;       
}