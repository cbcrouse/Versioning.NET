# Versioning.NET ![Main Status](https://github.com/cbcrouse/Versioning.NET/workflows/Main%20Status/badge.svg?branch=main) [![Build Status](https://caseycrouse.visualstudio.com/Github/_apis/build/status/Versioning.NET/Versioning.NET-CD?branchName=main)](https://caseycrouse.visualstudio.com/Github/_build/latest?definitionId=8&branchName=main) [![NuGet Downloads](https://img.shields.io/nuget/dt/Versioning.NET)](https://www.nuget.org/stats/packages/Versioning.NET?groupby=Version) [![NuGet Version](https://img.shields.io/nuget/v/Versioning.NET)](https://www.nuget.org/packages/Versioning.NET)

A dotnet tool that automatically versions csproj files semantically with git integration.

---

## Getting Started

```powershell
dotnet tool install --global Versioning.NET
```

---

## Changelog

### v0.2.1

Minor Changes

* Implemented Beta Exit Strategy

### v0.2.0

Major Changes

* Changed IGitService.GetBranchTipId to support remote target
* Removed Revision support from IncrementVersionWithGitIntegrationCommand

Minor Changes

* Fixed an issue finding the branch tip ID in GetIncrementFromCommitHintsHandler
* Fixed an issue pushing tags to remote from IncrementVersionWithGitIntegrationHandler

### v0.1.2

Minor Changes

* Fixed [issue #2](https://github.com/cbcrouse/Versioning.NET/issues/2) where tags were not pushing to remote
* Added test coverage for pushing remote branches and tags
* Increased test coverage on IncrementVersionWithGitIntegrationHandler

### v0.1.1

Minor Changes

* AppStartupOrchestrator spacing updates to test self-versioning still works.

### v0.1.0

Major Changes

* Replaced PowerShell-Git integration with LibGit2Sharp
* GetIncrementFromCommitHintsQuery now requires a branch name instead of a git log revision. Using a git log revision leaked implementation details.
* IGitVersioningService.GetCommitVersionInfo now accepts GitCommit objects instead of strings.
* IPowerShellService/PowerShellService were removed.
* GitService.cs was removed.

Minor Changes

* Several new methods added to the IGitService.cs.
* Added new extensions for GitCommit.
* GetIncrementFromCommitHintsHandler now utilizes new filtering functionality from GitService.
* Extension methods were added for LibGit2Sharp's Commit object.
* Added VersionIncrement extensions
* Tests were heavily updated and GitSetup was reworked to remove dependency on PowerShellService.

### v0.0.9

Minor Changes

* Extracted Increment lowering functionality from IncrementAssemblyVersionhandler

### v0.0.8

Minor Changes

* Increased test coverage for GitCommitVersionInfo.cs

### v0.0.7

Minor Changes

* Implement self-versioning in CI/CD

### v0.0.1

* Update version numbers in csproj files
* Update version numbers based upon git commit messages
* Push git commits and tags

---

## Backlog

- [x] Define beta exit strategy (Syntax parsing in commit message [Release])
- [x] Implement beta exit strategy
- [x] Replace PowerShell git interactions with [GitLib2Sharp](https://github.com/libgit2/libgit2sharp)
- [ ] Update project version and referenced versions
- [ ] Update version based on comparison of different git branches
- [ ] Support trunk based development
- [ ] Support gitflow development
- [ ] Documentation for tool

---
