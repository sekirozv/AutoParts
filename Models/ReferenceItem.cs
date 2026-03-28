namespace GardensRu.Models
{
    public class ReferenceItem
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString() => Name;
    }
}


