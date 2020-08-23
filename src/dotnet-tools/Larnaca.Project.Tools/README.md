
# Larnaca Framework 🌴 Project Tools

Nuget contains the dotnet project tool required for Larnaca Framework 🌴

This tool is used by the Larnaca Build Tools

It is a cli tool that handles all the tasks for working with Larnaca Framework 🌴 projects.

To install the tool:

```cmd
dotnet tool install Larnaca.Project.Tools
```

To run the tool to compile all the templates:

```cmd
larnaca-project-tools --templates D:\\Dev\\lca\\src\\lab\\sandbox.larnaca.project\\templates\\dal.config.lca.tt --larnacaFiles C:\\Users\\lev\\.nuget\\packages\\test.lca\\1.0.3\\contentFiles\\any\\any\\obj\\results.json
```

To run the tool to install/update the templates:

```cmd
larnaca-project-tools install-templates --csproj D:\\Dev\\lca\\src\\lab\\sandbox.larnaca.project\\sandbox.larnaca.project.csproj
```

To run the tool to create a nuget:

```cmd
larnaca-project-tools package --resultFile results.json --packageId Test.LCA --version 1.0.3 --authors "Larnaca Framework 🌴" --owners "Larnaca Framework 🌴" --description "Larnaca Framework 🌴 test nuget"
```