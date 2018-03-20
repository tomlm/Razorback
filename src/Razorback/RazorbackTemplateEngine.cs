using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Razorback
{
    public interface IRazorbackTemplateEngine
    {
        /// <summary>
        /// render text view as string
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<string> BindToText<ModelT>(string viewName, ModelT model);

        /// <summary>
        /// Render Xml view as object of type ResultT
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultT> BindXmlToObject<ModelT, ResultT>(string viewName, ModelT model);
    }

    public class RazorbackTemplateEngine  : IRazorbackTemplateEngine
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public RazorbackTemplateEngine(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Bind Model to text
        /// </summary>
        /// <typeparam name="ModelT"></typeparam>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> BindToText<ModelT>(string viewName, ModelT model)
        {
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            using (var sw = new StringWriter())
            {
                var viewResult = _razorViewEngine.FindView(actionContext, viewName, false);
                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
                }
                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };
                var viewContext = new ViewContext(actionContext, viewResult.View, viewDictionary, new TempDataDictionary(actionContext.HttpContext, _tempDataProvider), sw, new HtmlHelperOptions());
                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }

        static Dictionary<Type, XmlSerializer> serializers = new Dictionary<Type, XmlSerializer>();

        /// <summary>
        /// Bind Object to model
        /// </summary>
        /// <typeparam name="ModelT"></typeparam>
        /// <typeparam name="ResultT"></typeparam>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResultT> BindXmlToObject<ModelT, ResultT>(string viewName, ModelT model)
        {
            var xml = await BindToText(viewName, model);
            if (!serializers.TryGetValue(typeof(ResultT), out XmlSerializer serializer))
            {
                serializer = new XmlSerializer(typeof(ResultT));
                serializers[typeof(ResultT)] = serializer;
            }

            var result = (ResultT)serializer.Deserialize(new StringReader(xml));
            return result;
        }
    }
}
