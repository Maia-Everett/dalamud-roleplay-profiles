using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoleplayProfiles.Api
{
    public class ApiClient
    {
        private readonly RestClient restClient = new RestClient(new RestClientOptions("https://chaosarchives.org/api/rpp")
        {
            ThrowOnAnyError = true,
        });

        public async Task<Profile?> GetProfile(string name, string server)
        {
            var request = new RestRequest($"/profile/{server}/{name}");
            var response = await restClient.ExecuteAsync<Profile>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            if (response.ErrorException != null)
            {
                throw response.ErrorException;
            }

            return response.Data;
        }
    }
}
