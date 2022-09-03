using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RoleplayProfiles.Api
{
    public class Profile
    {
        // Note that since RestSharp uses the web defaults for JSON deserialization,
        // explicitly setting the camelCase name for each property is unnecessary.

        public string Title { get; set; } = "";

        public string Nickname { get; set; } = "";
        public string Occupation { get; set; } = "";
    }
}
