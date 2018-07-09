$versionInfo = Get-Content -Path "$PSScriptRoot/../VersionInfo.json" -Encoding UTF8 | ConvertFrom-Json
$projectFiles = Get-ChildItem -Path "$PSScriptRoot/../" -Recurse -Filter *.csproj

$buildNumber = 0
if ($env:BUILD_NUMBER) {
	$buildNumber = $env:BUILD_NUMBER
}

$gitCommit = 'NotBuiltByCI'
if ($env:GIT_COMMIT) {
	$gitCommit = $env:GIT_COMMIT
}

$versionTag = 'Version'
$informationalVersionTag = 'InformationalVersion'
$copyrightTag = 'Copyright'

foreach ($projectFile in $projectFiles) {
	$newContent = New-Object System.Text.StringBuilder
    
	foreach ($line in Get-Content $projectFile.FullName) {
		if ($line -Match "^[ ,\t]*<$versionTag>((?!<[\/]{0, 1}$versionTag>).)*<\/$versionTag>[ ,\t]*$") {
			$line = $line -replace "<$versionTag>.*<\/$versionTag>", "<$versionTag>$($versionInfo.Version).$buildNumber</$versionTag>"
		}
		
		if ($line -Match "^[ ,\t]*<$informationalVersionTag>((?!<[\/]{0, 1}$informationalVersionTag>).)*<\/$informationalVersionTag>[ ,\t]*$") {
			$line = $line -replace "<$informationalVersionTag>.*<\/$informationalVersionTag>", "<$informationalVersionTag>$($versionInfo.Version).$buildNumber-$gitCommit</$informationalVersionTag>"
		}
		
		if ($line -Match "^[ ,\t]*<$copyrightTag>((?!<[\/]{0, 1}$copyrightTag>).)*<\/$copyrightTag>[ ,\t]*$") {
			$line = $line -replace ' [0-9]{4} ', " $($versionInfo.CopyrightYear) "
		}
		
        [Void]$newContent.AppendLine($line)
    }
	
	Set-Content -Path $projectFile.FullName -Value $newContent.ToString().TrimEnd() -Encoding UTF8
}