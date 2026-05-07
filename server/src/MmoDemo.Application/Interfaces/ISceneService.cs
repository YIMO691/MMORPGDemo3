using MmoDemo.Contracts;

namespace MmoDemo.Application;

public interface ISceneService
{
    EnterCityResponse EnterCity(EnterCityRequest request);
}
