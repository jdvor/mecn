#!/usr/bin/env bash

# Builds tools and publishes them to ./publish directory.
# If not explicitly passed as parameter it will try to use latest git tag with semantic versioning value
# as the binary version.
# Also anything other than main or master branch produces version suffix based on a branch name.
#
# .Cli win10-x64 => publish/windows
# .Cli linux-x64 => publish/linux

version=""
suffix=""
pkgpath=""
cibuild="False"

while [[ "$#" -gt 0 ]]; do
    case $1 in
        -v|--version) version="$2"; shift ;;
        -s|--suffix) suffix="$2"; shift ;;
        -p|--packages) pkgpath="$2"; shift ;;
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
    if [ "$git_branch" != "main" ] && [ "$git_branch" != "master" ]; then
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
echo "nuget packages: $pkgpath"
echo "CI: $cibuild"
echo

csproj="tool/Mecn.Cli/Mecn.Cli.csproj"

rm -rf ./publish/windows
dotnet publish $csproj -c Release -o ./publish/windows --nologo \
    -p:VersionPrefix="$version" \
    -p:VersionSuffix="$suffix" \
    -p:RestorePackagesPath="$pkgpath" \
    -p:RunAnalyzers=False \
    -p:HostOS=Windows
if [ "$cibuild" == "True" ]; then
    zip -r "./publish/mecn-$fullversion.win-x64.zip" ./publish/windows/
fi

rm -rf ./publish/linux
dotnet publish $csproj -c Release -o ./publish/linux --nologo \
    -p:VersionPrefix="$version" \
    -p:VersionSuffix="$suffix" \
    -p:RestorePackagesPath="$pkgpath" \
    -p:RunAnalyzers=False \
    -p:HostOS=Linux
if [ "$cibuild" == "True" ]; then
    tar -czf "./publish/mecn-$fullversion.linux-x64.tar.gz" -C ./publish/linux/ .
fi
