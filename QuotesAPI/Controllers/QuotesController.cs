using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuotesAPI.Data;
using QuotesAPI.Models;

namespace QuotesAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class QuotesController : ControllerBase
	{
		private QuotesDbContext _quotesDbContext;
		public QuotesController(QuotesDbContext quotesDbContext)
		{
			_quotesDbContext = quotesDbContext;
		}

		// GET: api/Quotes
		[HttpGet]
		[ResponseCache(Duration = 30, Location = ResponseCacheLocation.Client)]
		[AllowAnonymous]
		public IActionResult Get(string sort)
		{
			IQueryable<Quote> quote;
			switch (sort)
			{
				case "desc":
					quote = _quotesDbContext.Quotes.OrderByDescending(q => q.CreatedAt);
					break;
				case "asc":
					quote = _quotesDbContext.Quotes.OrderBy(q => q.CreatedAt);
					break;
				default:
					quote = _quotesDbContext.Quotes;
					break;
			}
			return Ok(quote);

		}

		[HttpGet("[Action]")]
		public IActionResult MyQuotes(string sort)
		{
			string UserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			var quotes = _quotesDbContext.Quotes.Where(p=>p.UserId==UserId);
			return Ok(quotes);

		}
		[HttpGet("[Action]")]
		public IActionResult SearchQuote(string type)
		{
			var quote = _quotesDbContext.Quotes.Where(q => q.Type.StartsWith(type));
			return Ok(quote);
		}


		[HttpGet("[Action]")]
		public IActionResult PagingQuote(int? pageNumber, int? pageSize)
		{
			var quotes = _quotesDbContext.Quotes;
			var currentPageNumber = pageNumber ?? 1;
			var currentPageSize = pageSize ?? 1;

			return Ok(quotes.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize));
		}

		// GET: api/Quotes/5
		[HttpGet("{id}", Name = "Get")]
		public IActionResult Get(int id)
		{
			var quote = _quotesDbContext.Quotes.Find(id);
			if (quote == null)
			{
				return NotFound("Record not found...");
			}
			return Ok(quote);
		}

		// POST: api/Quotes
		[HttpPost]
		public IActionResult Post([FromBody] Quote quote)
		{
			string UserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			quote.UserId = UserId;
			_quotesDbContext.Quotes.Add(quote);
			_quotesDbContext.SaveChanges();
			return StatusCode(StatusCodes.Status201Created);

		}

		// PUT: api/Quotes/5
		[HttpPut("{id}")]
		public IActionResult Put(int id, [FromBody] Quote quote)
		{
			string UserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

			var entity = _quotesDbContext.Quotes.Find(id);
			if (entity == null)
			{
				return NotFound("Record not found...");
			}

			if (entity.UserId != UserId)
			{
				return BadRequest("Sorry you can't update this record");
			}
			entity.Title = quote.Title;
			entity.Author = quote.Author;
			entity.Description = quote.Description;
			entity.Type = quote.Type;
			entity.CreatedAt = quote.CreatedAt;

			_quotesDbContext.SaveChanges();
			return Ok("Record updated successfully...");
		}

		// DELETE: api/ApiWithActions/5
		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			string UserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			var quote = _quotesDbContext.Quotes.Find(id);
			if (quote == null)
			{
				return NotFound("Record not found...");
			}
			if (quote.UserId != UserId)
			{
				return BadRequest("Sorry you can't delete this record");
			}
			_quotesDbContext.Quotes.Remove(quote);
			_quotesDbContext.SaveChanges();
			return Ok("Quote deleted...");
		}
	}
}
