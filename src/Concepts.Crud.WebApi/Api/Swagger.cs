using System.Reflection;
using Concepts.Crud.WebApi.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Concepts.Crud.WebApi.Api;

public class SwaggerController : ControllerBase
{
    [HttpGet]
    [Route("/swagger/oapi3.json")]
    public ActionResult Oapi3()
    {
        var r = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream(Names.Activity_SpecificationJson)
                ?? throw new InvalidOperationException("Could not find Activity_Specification.json in resources");

        return File(r, "application/json");
    }
}