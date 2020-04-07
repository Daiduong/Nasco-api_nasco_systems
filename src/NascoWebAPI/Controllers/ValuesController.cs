using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VNPTPublishService;
using Microsoft.AspNetCore.Server.Kestrel;
using System.Runtime.CompilerServices;

namespace NascoWebAPI.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public async Task<ImportAndPublishInvResponse> Post([FromBody]ImportAndPublishInvRequestBody requestBody)
        {
            //      ImportAndPublishInvRequestBody requestBody = new ImportAndPublishInvRequestBody();
            //      requestBody.Account = "nascoadmin";
            //      requestBody.ACpass = "123456aA@";
            //      requestBody.xmlInvData = @"<Invoices>
            // <Inv>
            //          <key>dungtest1</key> 
            // <Invoice>
            // <CusCode>12356</CusCode>
            // <CusName>Thai Phan</CusName>
            // <Buyer>test</Buyer>
            // <CusAddress>355 tung thien vuong</CusAddress>
            // <CusPhone>0946669698</CusPhone>
            // <CusTaxCode>0108497732</CusTaxCode>
            // <PaymentMethod>TM</PaymentMethod>
            // <KindOfService>04</KindOfService>
            // <Extra>Khuyen mai</Extra>
            // <Products>
            // <Product>
            // <ProdName>TEST</ProdName>
            //          <Code>RV</Code>
            // <ProdUnit>cai</ProdUnit>
            // <ProdQuantity>1</ProdQuantity>
            // <ProdPrice>1000000</ProdPrice>
            // <Amount>1000000</Amount>
            // </Product>
            // </Products>
            // <Total>1000000</Total>
            // <DiscountAmount>0</DiscountAmount>
            // <VATRate>10</VATRate>
            // <VATAmount>100000</VATAmount>
            // <Amount>1100000</Amount>
            // <AmountInWords>mot trieu mot tran ngan việt nam đồng</AmountInWords>
            //          <ArisingDate>04/03/2020</ArisingDate>
            // <PaymentStatus>1</PaymentStatus>
            // </Invoice>
            // </Inv>
            //</Invoices>";
            //      requestBody.username = "nascoservice";
            //      requestBody.password = "123456aA@";
            //      requestBody.pattern = "01GTKT0/001";
            //      requestBody.serial = "NC/18E";
            //      requestBody.convert = 1;
            PublishServiceSoapClient.EndpointConfiguration endpoint = new PublishServiceSoapClient.EndpointConfiguration();
            PublishServiceSoapClient client = new PublishServiceSoapClient(endpoint);
            var result = client.ImportAndPublishInvAsync(
                requestBody.Account,
                requestBody.ACpass,
                requestBody.xmlInvData,
                requestBody.username,
                requestBody.password,
                requestBody.pattern,
                requestBody.serial,
                requestBody.convert
                ).ConfigureAwait(true).GetAwaiter().GetResult();
            return result;
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
