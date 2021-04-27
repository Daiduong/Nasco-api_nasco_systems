using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VNPTPublishService;
using VNPTPortalService;
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
        [HttpPost("PostVNPT_MB")]
        public async Task<MB_VNPTPublishService.ImportAndPublishInvResponse> PostVNPT_MB([FromBody]MB_VNPTPublishService.ImportAndPublishInvRequestBody requestBody)
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

            MB_VNPTPublishService.PublishServiceSoapClient.EndpointConfiguration endpoint = new MB_VNPTPublishService.PublishServiceSoapClient.EndpointConfiguration();
            MB_VNPTPublishService.PublishServiceSoapClient client = new MB_VNPTPublishService.PublishServiceSoapClient(endpoint);
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
        [HttpPost("PostVNPT_MT")]
        public async Task<MT_VNPTPublishService.ImportAndPublishInvResponse> PostVNPT_MT([FromBody]MT_VNPTPublishService.ImportAndPublishInvRequestBody requestBody)
        {
            MT_VNPTPublishService.PublishServiceSoapClient.EndpointConfiguration endpoint = new MT_VNPTPublishService.PublishServiceSoapClient.EndpointConfiguration();
            MT_VNPTPublishService.PublishServiceSoapClient client = new MT_VNPTPublishService.PublishServiceSoapClient(endpoint);
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
        [HttpPost("PostVNPT_MT_Test")]
        public async Task<MT_TEST_VNPTPublishService.ImportAndPublishInvResponse> PostVNPT_MT_Test([FromBody]MT_TEST_VNPTPublishService.ImportAndPublishInvRequestBody requestBody)
        {
            MT_TEST_VNPTPublishService.PublishServiceSoapClient.EndpointConfiguration endpoint = new MT_TEST_VNPTPublishService.PublishServiceSoapClient.EndpointConfiguration();
            MT_TEST_VNPTPublishService.PublishServiceSoapClient client = new MT_TEST_VNPTPublishService.PublishServiceSoapClient(endpoint);
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
        [HttpPost("PostVNPT_MN")]
        public async Task<MN_VNPTPublishService.ImportAndPublishInvResponse> PostVNPT_MN([FromBody]MN_VNPTPublishService.ImportAndPublishInvRequestBody requestBody)
        {
            MN_VNPTPublishService.PublishServiceSoapClient.EndpointConfiguration endpoint = new MN_VNPTPublishService.PublishServiceSoapClient.EndpointConfiguration();
            MN_VNPTPublishService.PublishServiceSoapClient client = new MN_VNPTPublishService.PublishServiceSoapClient(endpoint);
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

        //Get Invoice
        [HttpGet("GetInvoice")]
        public async Task<getInvViewFkeyResponse> GetInvoice(string fkey, string userName, string userPass)
        {
            PortalServiceSoapClient.EndpointConfiguration endpoint = new PortalServiceSoapClient.EndpointConfiguration();
            PortalServiceSoapClient client = new PortalServiceSoapClient(endpoint);
            var result = client.getInvViewFkeyAsync(
                fkey,
                userName,
                userPass
                ).ConfigureAwait(true).GetAwaiter().GetResult();
            return result;
        }
    }
}
