// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using System.Threading.Tasks;

using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;

// Created by Anton Piruev in 2025. Any direct commercial use of derivative work is strictly prohibited.

namespace Code.Infrastructure.Services.StaticDataService.Interfaces
{
  public interface IStaticDataService
  {
    public IBuildConfigSubservice BuildConfig { get; }
  }
}
