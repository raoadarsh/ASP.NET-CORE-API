using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace QuotesAPI.Models
{
	public class Quote
	{
		public int Id { get; set; }

		[Required]
		[StringLength(30)]
		public string Title { get; set; }

		[Required]
		[StringLength(20)]
		public string Author { get; set; }

		[Required]
		[StringLength(500)]
		public string Description { get; set; }

		[Required]
		public string Type { get; set; }

		[Required]
		[Column(TypeName = "datetime")]
		public DateTime CreatedAt { get; set; }

		[JsonIgnore]
		public string UserId { get; set; }
	}
}
