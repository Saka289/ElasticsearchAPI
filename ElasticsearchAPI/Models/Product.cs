using System.ComponentModel.DataAnnotations;

namespace ElasticsearchAPI.Models;

public class Product
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public decimal? Price { get; set; }
}