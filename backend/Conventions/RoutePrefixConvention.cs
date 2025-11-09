using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

namespace backend.Conventions;

public sealed class RoutePrefixConvention(IRouteTemplateProvider route) : IApplicationModelConvention
{
  private readonly AttributeRouteModel _routePrefix = new AttributeRouteModel(route);

  public void Apply(ApplicationModel application)
  {
    foreach (var controller in application.Controllers)
    {
      var selectorsWithAttributeRoute = controller.Selectors
        .Where(s => s.AttributeRouteModel != null)
        .ToList();

      if (selectorsWithAttributeRoute.Count > 0)
      {
        foreach (var selector in selectorsWithAttributeRoute)
        {
          selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(_routePrefix, selector.AttributeRouteModel);
        }
      }
      else
      {
        controller.Selectors.Add(new SelectorModel
        {
          AttributeRouteModel = _routePrefix
        });
      }
    }
  }
}

