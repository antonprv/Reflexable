// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Data.StaticData.Configs;
using Code.Data.StaticData.Configs.Types;
using Code.Infrastructure.Services.AssetManagement;
using Code.Infrastructure.Services.AssetManagement.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

namespace Code.Infrastructure.Services.StaticDataService.Subservices
{
  public class BuildConfigSubservice : IBuildConfigSubservice
  {
    public BuildConfiguration Current { get; private set; }

    private static GameBuildData _buildConfig;

    public BuildConfigSubservice(IAssetLoader assetLoader)
    {
      _buildConfig = assetLoader
      .Load<GameBuildData>(StaticDataPaths.BuildConfigPath);
      Current = _buildConfig.BuildConfiguration;
    }

    public bool IsDevelopment() => Current == BuildConfiguration.Development;
  }
}
