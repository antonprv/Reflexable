// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Infrastructure.Services.StaticDataService.Interfaces;
using Code.Zenjex.Extensions.Attribute;
using Code.Zenjex.Extensions.Injector;

namespace Code.UI.DevConsole.Controllers
{
  public class ConsoleButtonController : ZenjexBehaviour
  {
    [Zenjex] private IStaticDataService _staticData;

    protected override void OnAwake()
    {
      if (!IsDevelopmentBuild())
        gameObject.SetActive(false);
    }

    private bool IsDevelopmentBuild() =>
      _staticData.BuildConfig.IsDevelopment();
  }
}
