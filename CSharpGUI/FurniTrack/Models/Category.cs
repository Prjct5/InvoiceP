using System;

namespace FurniTrack.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
