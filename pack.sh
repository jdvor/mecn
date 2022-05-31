#!/usr/bin/env bash

# Builds NuGet package Mecn and copies Mecn.{version}.nupkg and .snupkg file into ./publish directory.
# If not explicitly passed as parameter it will try to use latest git tag with semantic versioning value
# as the package version.
# Also anything other than main or master branch produces version suffix based on a branch name.

version=""
strict="True"
pkgpath=""
suffix=""
cibuild="False"

while [[ "$#" -gt 0 ]]; do
    case $1 in
        -v|--version) version="$2"; shift ;;
        -s|--suffix) suffix="$2"; shift ;;
        -p|--packages) pkgpath="$2"; shift ;;
        -f|--forced) strict="False" ;;
        -c|--ci-build) cibuild="True" ;;
        *) echo "Unknown parameter passed: $1"; exit 1 ;;
    esac
    shift
done

if [ -z "$version" ]; then
    version=$(git tag | grep -P '^\d+\.\d+\.\d+(-[a-z0-9]+)?$' | sort -V | tail -1)
    if [ -z "$version" ]; then
        if [ "$cibuild" == "True" ]; then
            echo "Failed to determine package version" 1>&2
            exit 1
        else
            version="1.0.0"
        fi
    fi
fi

if [ -z "$suffix" ]; then
    git_branch=$(git rev-parse --abbrev-ref HEAD)
    if [ "$git_branch" != "main" ] && [ "$git_branch" != "master" ] && [ "$git_branch" != "HEAD" ]; then
        suffix=$(echo "$git_branch" | tr "[:upper:]" "[:lower:]" | sed s/[^a-z0-9]//g)
    fi
fi

if [ -z "$pkgpath" ]; then
    if [ "$cibuild" == "True" ]; then
        pkgpath="./packages"
    else
        pkgpath_raw=$(nuget locals global-packages -list)
        pkgpath="${pkgpath_raw/global-packages: /}"
    fi
fi

[[ -z "$suffix" ]] && fullversion=$version || fullversion="$version-$suffix"

echo "version: $fullversion"
echo "strict: $strict"
echo "nuget packages: $pkgpath"
echo "CI: $cibuild"
echo

rm -rf ./publish
csproj="src/Mecn/Mecn.csproj"
dotnet clean $csproj -c Release -v quiet --nologo
dotnet pack $csproj -c Release -o ./publish --nologo \
    -p:VersionPrefix="$version" \
    -p:VersionSuffix="$suffix" \
    -p:RestorePackagesPath="$pkgpath" \
    -p:TreatWarningsAsErrors=$strict \
    -p:RunAnalyzers=$strict \
    -p:ContinuousIntegrationBuild=$cibuild
