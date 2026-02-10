
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;
using System.Net.NetworkInformation;
namespace SchoolManegementNew.Services
{
    public class RazorViewRenderer
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        public RazorViewRenderer(IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }
        public string RenderToString(ControllerContext context,string ViewName,object model)
        {
            var viewResult = _viewEngine.FindView(context, ViewName, false);
            if (viewResult.View == null)
            {
                throw new FileNotFoundException($"View '{ViewName}' not found.");
            }
                var viewData = new ViewDataDictionary(
            new EmptyModelMetadataProvider(),
            new ModelStateDictionary())
                {
                    Model = model
                };
                using var sw = new StringWriter();
                var viewContext = new ViewContext(
                    context,
                    viewResult.View,
                    viewData,
                    new TempDataDictionary(context.HttpContext, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );
                viewResult.View.RenderAsync(viewContext).GetAwaiter().GetResult();
                return sw.ToString();
            }
        }
    }

