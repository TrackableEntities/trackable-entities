param($rootPath, $toolsPath, $package, $project)

# Set Entity.tt CustomTool to blank
foreach ($item in $project.ProjectItems){
  foreach ($item in $item.ProjectItems){
    $item.ProjectItems | ?{ $_.Name -eq "Entity.tt" } | %{
      $_.Properties | ?{ $_.Name -eq "CustomTool" } | %{ $_.Value = "" }
    }
    $item.ProjectItems | ?{ $_.Name -eq "Context.tt" } | %{
      $_.Properties | ?{ $_.Name -eq "CustomTool" } | %{ $_.Value = "" }
    }
    $item.ProjectItems | ?{ $_.Name -eq "Mapping.tt" } | %{
      $_.Properties | ?{ $_.Name -eq "CustomTool" } | %{ $_.Value = "" }
    }
  }
}
