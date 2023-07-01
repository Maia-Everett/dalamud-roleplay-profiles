namespace RoleplayProfiles.Api
{
    public class Profile
    {
        // Note that since RestSharp uses the web defaults for JSON deserialization,
        // explicitly setting the camelCase name for each property is unnecessary.

        public string Title { get; set; } = "";

        public string Nickname { get; set; } = "";
        public string Occupation { get; set; } = "";
        public string Currently { get; set; } = "";
        public string OocInfo { get; set; } = "";
        public string Pronouns { get; set; } = "";

        public string Appearance { get; set; } = "";
        public string Background { get; set; } = "";
        public string Race { get; set; } = "";
        public string Age { get; set; } = "";
        public string Birthplace { get; set; } = "";
        public string Residence { get; set; } = "";
        public string Friends { get; set; } = "";
        public string Relatives { get; set; } = "";
        public string Enemies { get; set; } = "";
        public string Loves { get; set; } = "";
        public string Hates { get; set; } = "";
        public string Motto { get; set; } = "";
        public string Motivation { get; set; } = "";
    }
}
