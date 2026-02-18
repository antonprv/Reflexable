// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.AssetManagement;
using Code.Infrastructure.Services.AssetManagement.Interfaces;
using Code.Infrastructure.Services.DevConsole;
using Code.Infrastructure.Services.Input;
using Code.Infrastructure.Services.Input.Interfaces;
using Code.Infrastructure.Services.StaticDataService;
using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Infrastructure.Services.StaticDataService.Subservices;
using Code.Infrastructure.Services.Time;
using Code.Zenjex.Extensions.Core;
using Reflex.Core;
using System.Collections;
using UnityEngine;

namespace Code.Infrastructure.Installer
{

  [DefaultExecutionOrder(-250)]
  public class GameInstaller : ProjectRootInstaller
  {
    public override IEnumerator InstallGameInstanceRoutine() { yield return null; }
    public override void LaunchGame() {}

    public override void InstallBindings(ContainerBuilder builder)
    {
      BindAssetLoaderService(builder);
      BindInputService(builder);
      BindStaticDataService(builder);
      BindTimeService(builder);
      BindDevConsoleService(builder);
    }

    private static void BindInputService(ContainerBuilder builder) => 
      builder.Bind<IInputService>().To<InputService>().AsSingle();

    private static void BindDevConsoleService(ContainerBuilder builder) =>
      builder.Bind<IDevConsole>().To<DevConsoleService>().AsSingle();

    private static void BindStaticDataService(ContainerBuilder builder)
    {
      builder.Bind<IBuildConfigSubservice>().To<BuildConfigSubservice>().AsSingle();
      builder.Bind<IStaticDataService>().To<StaticDataService>().AsSingle();
    }

    private static void BindAssetLoaderService(ContainerBuilder builder) =>
      builder.Bind<IAssetLoader>().To<AssetLoader>().AsSingle();

    private void BindTimeService(ContainerBuilder builder) =>
      builder.Bind<ITimeService>().To<UnityTimeService>().AsSingle();
  }
}
