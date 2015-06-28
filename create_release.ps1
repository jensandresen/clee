param(
	$version = $null,
    [switch]$major = $false,
    [switch]$minor = $false,
    [switch]$patch = $false
)

# change version in assembly info file
$assemblyInfoFilePath = resolve-path ".\src\Clee.Core\Properties\AssemblyInfo.cs"

if ($version -ne $null -or $major -or $minor -or $patch) {
	
    $content = get-content $assemblyInfoFilePath -raw
    $currentVersion = @{}
    $oldVersion = $null

    if ($content -match ".*\[assembly: AssemblyInformationalVersion\(`"(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)`"\)\].*") {
        $currentVersion.Major = $matches["major"]
        $currentVersion.Minor = $matches["minor"]
        $currentVersion.Patch = $matches["patch"]

        $oldVersion = "{0}.{1}.{2}" -f $currentVersion.Major,$currentVersion.Minor,$currentVersion.Patch
    }
    else {
        throw "Expected AssemblyInformationalVersion element is not defined in the AssemblyInfo.cs file"
    }

    if ($version -ne $null) {
        # nothing...use it directly
    }
    else {
        
        if ($major) {
            $currentVersion.Major = ([int]$currentVersion.Major)+1;
            $currentVersion.Minor = 0;
            $currentVersion.Patch = 0;
        }
        elseif ($minor) {
            $currentVersion.Minor = ([int]$currentVersion.Minor)+1;
            $currentVersion.Patch = 0;
        }
        elseif ($patch)  {
            $currentVersion.Patch = ([int]$currentVersion.Patch)+1;
        }

        $version = "{0}.{1}.{2}" -f $currentVersion.Major,$currentVersion.Minor,$currentVersion.Patch
    }

	$content = $content -replace "(?<before>.*)\[assembly: Assembly(?<element>File|Informational)?Version\(`".*?`"\)\](?<after>.*)","`${before}[assembly: Assembly`${element}Version(`"$version`")]`${after}"
	$content | out-file -filepath "$assemblyInfoFilePath" -encoding utf8 -force

    write-host "Changed version: $oldVersion --> $version"
}

# create nuget package
try {
	$outputDir = resolve-path ".\build"
	push-location
	cd ".\src\Clee.Core\"
	nuget pack -output "$outputDir" -build -Prop Configuration=Release
}
finally {
	pop-location
}