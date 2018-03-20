using AdaptiveCards;
using Newtonsoft.Json;
using Razorback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace RazorbackSample.AspNet.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly IRazorbackTemplateEngine _razorback;

        public ValuesController()
        {
            _razorback = razorback;
        }

        private static Person CreateModelInstance()
        {
            var model = new Person()
            {
                First = "Tom",
                Last = "Laird-McConnell",
                Birthday = new DateTime(1967, 5, 25)
            };
            for (int i = 0; i < 10; i++)
                model.Factoids.Add(new KeyValuePair<string, string>(i.ToString("x"), i.ToString()));
            return model;
        }

        [HttpGet]
        [Route("card")]
        public async Task<string> GetCard()
        {
            Person model = CreateModelInstance();
            var card = await this._razorback.BindXmlToObject<Person, AdaptiveCard>("Values/Card", model);

            return JsonConvert.SerializeObject(card, Formatting.Indented);
        }


    }
}
