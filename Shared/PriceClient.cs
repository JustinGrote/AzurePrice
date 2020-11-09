using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Client.Shared.OData;
using System.Net.Http.Json;
using System;
using System.Text.RegularExpressions;

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
            List<Price> priceList = new();
            string requestUri = path + '?' + query.ToString();


            int requestItemCount;
            Uri nextPageLink = null;

            //ToDo: Add concurrent readahead support since the paging count doesn't display properly
            do
            {
                if (nextPageLink != null)
                {
                    requestUri = convertNextPageLinkToRelativeURI(nextPageLink);
                }

                var response = await http.GetFromJsonAsync<PriceResponse>(requestUri);
                priceList.AddRange(response.Prices);
                requestItemCount = response.Count;
                nextPageLink = response.NextPageLink;
            } while (
                requestItemCount == 100 && 
                Regex.IsMatch(nextPageLink.ToString(), "skip=\\d+$")
            );

            return priceList;
        }

        private String convertNextPageLinkToRelativeURI(Uri nextPageLink) {
            return path + nextPageLink.Query;
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