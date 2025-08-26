namespace shorten.url.application.Models
{
    public class Domain
    {
        public List<string> List { get; set; } = new List<string>();

        public string RandomDomain
        {
            get
            {
                var random = new Random(); 
                int index = random.Next(List.Count);

                return List[index];
            }
        }
    }
}
