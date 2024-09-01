# dotnet-playground
Its just collection of very basic dotnet examples. Can be used as a tutorial for newbees.

## Chapters
- [x] 01 Dependency injection
- [x] 02 Application configuration
- [x] 03 Logging
- [x] 04 Asynchronous tasks
- [ ] 05 Background services
- [ ] 06 Unit tests

## Cheat sheet

Create empty dotnet solution
```bash
dotnet new sln -n <solution_name> -o <solution_dir>
```

Enter to solution directory
```bash
cd <solution_dir>
```

Create new console project
```bash
dotnet new console -n <project_name>
```

Add existing project to existing solution
```bash
dotnet sln add <project_name>
```

Build and run current project
```bash
dotnet run --project <project_name>
```
