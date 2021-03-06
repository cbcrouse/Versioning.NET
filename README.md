# Versioning.NET ![Main Status](https://github.com/cbcrouse/Versioning.NET/workflows/Main%20Status/badge.svg?branch=main) [![Build Status](https://caseycrouse.visualstudio.com/Github/_apis/build/status/Versioning.NET/Versioning.NET-CD?branchName=main)](https://caseycrouse.visualstudio.com/Github/_build/latest?definitionId=8&branchName=main) ![NuGet Downloads](https://img.shields.io/nuget/dt/Versioning.NET)

A dotnet tool that automatically versions csproj files semantically with git integration.

---

## Getting Started

```powershell
dotnet tool install --global Versioning.NET
```

## Roadmap

Build a dotnet tool that can...

### v0.0.1

- [x] Update version numbers in csproj files
- [x] Update version numbers based upon git commit messages
- [x] Push git commits and tags

### v0.0.7

- [x] Implement self-versioning in CI/CD

### Backlog

- [x] Define beta exit strategy (Syntax parsing in commit message [Release])
- [ ] Update project version and referenced versions
- [ ] Update version based on comparison of different git branches
- [ ] Support trunk based development
- [ ] Support gitflow development
- [ ] Documentation for tool

---
