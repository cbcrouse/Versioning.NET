# Versioning.NET ![Main Status](https://github.com/cbcrouse/Versioning.NET/workflows/Main%20Status/badge.svg?branch=main) [![Build Status](https://caseycrouse.visualstudio.com/Github/_apis/build/status/Versioning.NET/Versioning.NET-CD?branchName=main)](https://caseycrouse.visualstudio.com/Github/_build/latest?definitionId=8&branchName=main) [![NuGet Downloads](https://img.shields.io/nuget/dt/Versioning.NET)](https://www.nuget.org/stats/packages/Versioning.NET?groupby=Version) [![NuGet Version](https://img.shields.io/nuget/v/Versioning.NET)](https://www.nuget.org/packages/Versioning.NET)

A dotnet tool that automatically versions csproj files semantically with git integration.

---

## Getting Started

```powershell
dotnet tool install --global Versioning.NET
```

---

## Backlog

- [x] Define beta exit strategy (Syntax parsing in commit message [Release])
- [x] Implement beta exit strategy
- [x] Replace PowerShell git interactions with [GitLib2Sharp](https://github.com/libgit2/libgit2sharp)
- [ ] Increase code coverage to at least 80% - [issue #8](https://github.com/cbcrouse/Versioning.NET/issues/8)
- [ ] Add code coverage reporting to readme - [issue #9](https://github.com/cbcrouse/Versioning.NET/issues/9)
- [ ] Ensure project is ready for open-source contributing - [issue #11](https://github.com/cbcrouse/Versioning.NET/issues/11)
- [ ] Update the Wiki with proper documentation - [issue #10](https://github.com/cbcrouse/Versioning.NET/issues/10)

---
