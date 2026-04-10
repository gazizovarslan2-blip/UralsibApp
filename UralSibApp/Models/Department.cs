namespace UralSibApp.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Floor { get; set; }
        public int TeamsCount { get; set; }
    }
}