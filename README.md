# Versioning.NET

![Main Status](https://github.com/cbcrouse/Versioning.NET/workflows/Main%20Status/badge.svg?branch=main) [![Build Status](https://caseycrouse.visualstudio.com/Github/_apis/build/status/Versioning.NET/Versioning.NET-CD?branchName=main)](https://caseycrouse.visualstudio.com/Github/_build/latest?definitionId=8&branchName=main) [![NuGet Downloads](https://img.shields.io/nuget/dt/Versioning.NET)](https://www.nuget.org/stats/packages/Versioning.NET?groupby=Version) [![NuGet Version](https://img.shields.io/nuget/v/Versioning.NET)](https://www.nuget.org/packages/Versioning.NET) [![codecov](https://codecov.io/gh/cbcrouse/Versioning.NET/branch/main/graph/badge.svg?token=VT14HECMQE)](https://codecov.io/gh/cbcrouse/Versioning.NET) [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Versioning.NET&metric=alert_status)](https://sonarcloud.io/dashboard?id=Versioning.NET)

## Overview

A dotnet core CLI tool that maintains the semantic version of your dotnet assemblies and git tags based on a git commit message convention. This repository and tool version are currently maintained by this tool.

---

## Objective

[Versioning.NET](https://github.com/cbcrouse/Versioning.NET) is designed to take the responsibility of maintaining the version of the code away from the developers, but still allow developers to control the outcome of the versioning automation through their commit messages. Although this tool works locally, it's intended and recommended to be utilized on CI servers where the developer can be abstracted.

---

## What makes it different?

* It's a dotnet tool which makes it easy to use, but means you don't have to install anything in your project for it to work.
* The version calculation (defined below) is a different way of controlling the version. It allows the developers to still maintain fine control over the version without actually modifying the files themselves.
* It was created using [Clean Architecture](https://github.com/cbcrouse/CleanArchitecture) which means it's built to last, regardless of which system is used to integrate with Git or other framework dependencies.

---

## How does it work?

In short, the tool will read the commits on the specified branch, parse through the commit history starting from HEAD and traversing back to the latest version tag (commonly referred to as `git height`) looking for [commit hints](https://github.com/cbcrouse/Versioning.NET/wiki/Commit-Hints) in order to determine the version increment (Major, Minor, Patch, or None), update all of the .csproj files in a single commit using the determined increment, and finally create and push a git tag to that commit representing the semantic version.

#### **Branching**

The tool currently works with a single branch. This branch will be used for maintaining the version tags and will also be used to determine which commits will be included when determining what the latest version will be. It is recommended to select a long-lived branch such as master, main, or dev, but other branches can be used for testing without impacting current versions.

[*Git Flow*](http://datasift.github.io/gitflow/IntroducingGitFlow.html) - It is possible to work with Git Flow. When release branches are cut, the version will not increase on the release branch, but each bug fix that is merged back into the versioned branch will cause the version to increment. However, because commits will be entering the release branch and not affecting the version that is deployed, this may not be a desired result.

[*GitHub Flow*](https://guides.github.com/introduction/flow/) - This flow is more appropriate for this tool as feature branches are merged into the target branch, the version increments appropriately, and the code is deployed immediately afterwards, promoting continuous release.

#### **Authorization**

In order for the tool to work with any git repository, it expects that a remote target has been [configured with credentials](https://github.com/cbcrouse/Versioning.NET/wiki/Configuring-CI-CD#configure-credentials-for-remote-target). When running the command, passing in the git repository path and the name of the configured remote target is sufficient.

#### **Version Calculation**

When the tool is looking through the commits for `hints`, it's expecting that the commits follow a [this convention](https://github.com/cbcrouse/Versioning.NET/blob/main/docs/commit_message_standards.md) in order to determine the version increment. Currently, commits that do not follow this convention are ignored (not desired and will updated to default to Patch) and will not be included when calculating the version. When commits do follow the convention, but it is not desired for the commit to affect the version, it is possible to include a hint that will tell the tool to ignore it (see [Commit Hints](https://github.com/cbcrouse/Versioning.NET/wiki/Commit-Hints)). This is useful when updating parts of the repository that are not deployed such as documentation and build scripts.

#### **Assembly Updates**

Currently, this tool only supports updating .csproj files targeting the `<Version>` and `<VersionPrefix>` elements. In the future, other ways of maintaining version info for dotnet projects such as `AssemblyInfo.cs` are expected to be supported. The .csproj files that are targeted are in the git repository and have the previously mentioned xml elements. This means that repositories with more than one project (mono-repo) is not currently supported. It is also expected to have path filtering supported in a later version so that files changed by the commit are only included when the file path is a child of a specified path filter.

#### **Version Increment Commit**

Once the version increment has been determined and the files have been updated, the tool will create a git commit with a message (not yet configurable) describing the version changes. In order to avoid circular builds, CI servers are required to support `[skip ci]` (both [Azure DevOps Pipelines](https://docs.microsoft.com/en-us/azure/devops/pipelines/repos/azure-repos-git?view=azure-devops&tabs=yaml#skipping-ci-for-individual-commits) and [GitHub Actions](https://github.blog/changelog/2021-02-08-github-actions-skip-pull-request-and-push-workflows-with-skip-ci/) support this) in the commit message to stop the version commit from triggering another build. The following is an example commit from the tool:

```bash
ci(Versioning): Increment version 0.3.1 -> 0.3.2 [skip ci] [skip hint]
```

This commit will not trigger another build, provided that the CI server supports the skip build hint, and the commit will also not be included in subsequent version increments. Once the commit exists, the commit ID will be targeted when pushing the semver git tag to remote.

---

## Example Scenario

This section will provide an example scenario to give a more clear picture of what is happening.

Assume that the following commits exist on the master branch.

```csharp
// HEAD on master
82d2d2a feat(Versioning): Changed git integration from PowerShell to LibGit2Sharp #breaking
38basd2 fix(Versioning): Fixed issue where git tag could not be found
23a32s3 feat(Versioning): Added support to modify .csproj files
828asd2 docs(README): Updated overview section [skip hint]
28s8d28 Updated some files
// Latest version tag: v1.1.5 (targeting 67ds273)
67ds273 ci(Versioning): Increment version 1.1.4 -> 1.1.5 [skip ci] [skip hint]
```

The commits above the v1.1.5 tag will be used to determine the version increment, here's what the tool will do:

1. 82d2d2a - Major is determined for increment because `#breaking` is found.
1. 38basd2 - Patch is determined for increment because `fix` is found.
1. 23a32s3 - Minor is determined for increment because `feat` is found.
1. 828asd2 - None is determined for the increment because `[skip hint]` was found.
1. 28s8d28 - None is determined for increment because no hint was found.

> In a later version, when no hint is found, the increment will default to Patch.

When all the commits have been assigned a version increment, the highest priority is used to determine what the final increment will be. The version increment that will result from the above commits is **Major**.

```csharp
// HEAD on master
// Latest version tag: v2.0.0 (targeting 87sd2s2)
87sd2s2 ci(Versioning): Increment version 1.1.5 -> 2.0.0 [skip ci] [skip hint]
82d2d2a feat(Versioning): Changed git integration from PowerShell to LibGit2Sharp #breaking
38basd2 fix(Versioning): Fixed issue where git tag could not be found
23a32s3 feat(Versioning): Added support to modify .csproj files
828asd2 docs(README): Updated overview section [skip hint]
28s8d28 Updated some files
// Latest version tag: v1.1.5 (targeting 67ds273)
67ds273 ci(Versioning): Increment version 1.1.4 -> 1.1.5 [skip ci] [skip hint]
```

Below is an excerpt from the [code](https://github.com/cbcrouse/Versioning.NET/blob/main/src/Infrastructure/Services/GitVersioningService.cs) showing how increment priority is determined.

```csharp
List<VersionIncrement> incrementsList = increments.ToList();
bool isBreaking = incrementsList.Any(x => x == VersionIncrement.Major);
bool isMinor = incrementsList.Any(x => x == VersionIncrement.Minor);
bool isPatch = incrementsList.Any(x => x == VersionIncrement.Patch);
bool isNone = incrementsList.Any(x => x == VersionIncrement.None);

return isBreaking ? VersionIncrement.Major
    : isMinor ? VersionIncrement.Minor
    : isPatch ? VersionIncrement.Patch
    : isNone ? VersionIncrement.None
    : VersionIncrement.Unknown;
```

---

## ![Download](./docs/media/download_icon.png) Getting Started

Get started by first installing the tool.

```bash
dotnet tool install --global Versioning.NET
```

Once the tool is installed, run `dotnet-version` to see the help menu.

```bash
Usage: dotnet-version [command] [options]

Options:
  -?|-h|--help                Show help information.

Commands:
  increment-version
  increment-version-with-git

Run 'dotnet-version [command] -?|-h|--help' for more information about a command.
```

For more information on the command details, see the wiki page for [Commands](https://github.com/cbcrouse/Versioning.NET/wiki/Commands).

---

## ![Puzzle](./docs/media/puzzle.png) Contributing

Want to add a feature or fix a bug? Glad to have the help! There's just a couple of things to go over before you start submitting pull requests:

* [Commit Message Standards](./docs/commit_message_standards.md)
* [Branching Strategies](./docs/branching_strategies.md)

---

## ![Law](./docs/media/law.png) Licensing

These templates are licensed under the [MIT License](./LICENSE).

---
