---
uid: example_basic_setup
title: Basic Setup (Console)
---

# Getting started: Basic Setup (Console)

In this tutorial, we will create a .NET Core console app that performs basic data access against a SQLite database using Entity Framework Core.

[Download or view the completed app.](https://github.com/dogguts/aeon/tree/master/samples/Basics/Setup)

> [!NOTE]
> To keep things simple, we'll create a Console Application

## Create a new project

# [.NET Core CLI](#tab/netcore-cli)
```DOS
dotnet new console -o BasicSetup
cd BasicSetup
```
# [Visual Studio](#tab/visual-studio)
- Open Visual Studio
- Click **Create a new project**
- Select **Console App (.NET Core)** with the **C#** tag and click Next
- Enter BasicSetup for the name and click Create

***

## Install packages
To install Aeon Repository, you install; 
- the packages for the EF Core database provider(s) you want to target. 
- the "Aeon.Core.Repository" package
This tutorial uses SQLite because it runs on all platforms that .NET Core supports.

# [.NET Core CLI](#tab/netcore-cli)
```DOS
dotnet add package Aeon.Core.Repository
dotnet add package Microsoft.EntityFrameworkCore.Sqlite

```
# [Visual Studio](#tab/visual-studio)
- Tools > NuGet Package Manager > Package Manager Console

- Run the following commands:

```ps
Install-Package Aeon.Core.Repository
Install-Package Microsoft.EntityFrameworkCore.Sqlite
```
Or you can just right-click the project and select Manage NuGet Packages

***

## Create the model
Define a context class and entity classes that make up the model.
> [!NOTE]
> Creating a DbContext, Models and Data-seed is no different from creating those with just EF Core.
 
# [.NET Core CLI](#tab/netcore-cli)
- In the project directory, create Model.cs with the following code
# [Visual Studio](#tab/visual-studio)
- Right-click on the project and select **Add > Class**
- Enter **Model.cs** as the name and click **Add**
- Replace the contents of the file with the following code
***
 
 [!code-csharp[Main](../../../samples/01.Basics/01.Setup/Model.cs)]

## Our actual program
- Open Program.cs and replace the contents with the following code:
 [!code-csharp[Main](../../../samples/01.Basics/01.Setup/Program.cs)]