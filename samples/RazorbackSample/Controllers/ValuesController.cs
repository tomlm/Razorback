﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AdaptiveCards;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Razorback;

namespace AdaptiveCardRazorSample.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IRazorbackTemplateEngine _razorback;

        public ValuesController(IRazorbackTemplateEngine razorback)
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

        [HttpGet]
        public async Task<string> Get()
        {
            Person model = CreateModelInstance();
            return await this._razorback.BindToText<Person>("Values/Text", model);
        }


    }
}
