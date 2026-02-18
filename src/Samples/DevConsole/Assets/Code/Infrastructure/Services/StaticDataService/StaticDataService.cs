// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Zenjex.Extensions.Core;

namespace Code.Infrastructure.Services.StaticDataService
{
  public class StaticDataService : IStaticDataService
  {
    public IBuildConfigSubservice BuildConfig { get; private set; }

    public StaticDataService() => 
      BuildConfig = RootContext.Resolve<IBuildConfigSubservice>();
  }
}
