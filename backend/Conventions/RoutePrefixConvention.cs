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
          // Skip if the route already contains "api/v" (already has the prefix)
          if (selector.AttributeRouteModel?.Template?.Contains("api/v") == true)
          {
            continue;
          }
          
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

