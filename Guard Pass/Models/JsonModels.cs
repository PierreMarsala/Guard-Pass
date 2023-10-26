namespace Guard_Pass.Models
{
    class AccountData
    {
        public string? name { get; set; }
        public string? user { get; set; }
        public string? password { get; set; }
        public int icon { get; set; }
        public string? imagePath { get; set; }
        public int ID { get; set; }
    }

    class Parameters
    {
        public int background { get; set; }
        public string? language { get; set; }
    }
}
