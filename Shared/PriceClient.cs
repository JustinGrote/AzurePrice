using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Client.Shared.OData;
using System.Net.Http.Json;

namespace Client.Shared
{
    public class PriceClient
    {
        //Reused Blazor WASM client
        protected readonly HttpClient http;
        
        //Path the the api
        public readonly string path = "/api/prices";
        public PriceClient(HttpClient httpclient)
        {
            http = httpclient;
        }

        public async Task<List<Price>> GetPrices(ODataQuery<Price> query)
        {
            string requestUri = path + '?' + query.ToString();
            var response = await http.GetFromJsonAsync<PriceResponse>(requestUri);
            return response.Prices;
        }
        
        public async Task<List<Price>> GetSpotPrices(ODataQuery<Price> query)
        {
            string spotVMFilter = " and endswith(skuName,'Spot') eq true and endswith(productName,'Windows') eq false";
            query.Filter += spotVMFilter;
            query.OrderBy = "unitPrice";
            return await GetPrices(query);
        }
    }
}