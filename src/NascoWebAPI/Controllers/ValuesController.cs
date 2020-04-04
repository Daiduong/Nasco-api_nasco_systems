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
        public async Task<ImportAndPublishInvResponse> Post()
        {
            ImportAndPublishInvRequestBody requestBody = new ImportAndPublishInvRequestBody();
            requestBody.Account = "nascoservice";
            requestBody.ACpass = "123456aA@";
            requestBody.xmlInvData = @"<xmlInvData>
      	    <Invoices>
			    <Inv>
			    <Invoice>
			    <CusCode>12356</CusCode>
			    <CusName>Thai Phan</CusName>
			    <Buyer>test</Buyer>
			    <CusAddress>355 tung thien vuong</CusAddress>
			    <CusPhone>000000000</CusPhone>
			    <CusTaxCode>12345678</CusTaxCode>
			    <PaymentMethod>Phương thức thanh toán</PaymentMethod>
			    <KindOfService>Tháng hóa đơn</KindOfService>
			    <Products>
			    <Product>
			    <ProdName>TEST</ProdName>
			    <ProdUnit>Đơn vị tính</ProdUnit>
			    <ProdQuantity>1</ProdQuantity>
			    <ProdPrice>123</ProdPrice>
			    <Amount>123</Amount>
			    </Product>
			    </Products>
			    <Total>0</Total>
			    <DiscountAmount>0</DiscountAmount>
			    <VATRate>10</VATRate>
			    <VATAmount>1000</VATAmount>
			    <Amount>0</Amount>
			    <AmountInWords>vai</AmountInWords>
			    <Extra>Các nội dung mở rộng</Extra>
			    <ArisingDate>04/03/2020</ArisingDate>
			    <PaymentStatus>1</PaymentStatus>
			    </Invoice>
			    </Inv>
		    </Invoices>
          </xmlInvData>";
            requestBody.username = "nascoservice";
            requestBody.password = "123456aA@";
            requestBody.pattern = "01GTKT0/001";
            requestBody.serial = "NC/18E";
            requestBody.convert = 0;
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
