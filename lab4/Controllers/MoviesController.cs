using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab4.Data;
using Lab4.Models;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Lab4.ViewModels;

namespace Lab4.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Produces("application/json")]
	public class MoviesController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;

		public MoviesController(ApplicationDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		/// <summary>
		/// Retrieves a list of movies filtered by the interval when they were added, ordered descendingly by release year.
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// GET /api/Movies/filter/2011-02-10T12:10:00_2022-01-01
		/// </remarks>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <response code="200">The filtered movies.</response>
		[HttpGet]
		[Route("filter/{startDate}_{endDate}")]
		public ActionResult<IEnumerable<MovieViewModel>> FilterMovies(string startDate, string endDate)
		{
			var startDateDt = DateTime.Parse(startDate);
			var endDateDt   = DateTime.Parse(endDate);

			var movies = _context.Movies.Where(m => m.AddedAt >= startDateDt && m.AddedAt <= endDateDt)
				.OrderByDescending(m => m.ReleaseYear).ToList();

			return _mapper.Map<List<Movie>, List<MovieViewModel>>(movies);
		}

		/// <summary>
		/// Retrieves a list of movies filtered by the interval when they were added, ordered descendingly by release year.
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// GET /api/Movies?startDate=1997-12-31T23:59:00&endDate=2002-01-01
		/// </remarks>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <response code="200">The filtered movies.</response>
		[HttpGet]

		public async Task<ActionResult<IEnumerable<MovieViewModel>>> GetMovies(string? startDate, string? endDate)
		{
			// the first movie ever was made in 1888, so we can use this as a default first value
			var startDateDt = startDate == null ? DateTime.Parse("1888-01-01") : DateTime.Parse(startDate);
			var endDateDt = endDate == null ? DateTime.Now : DateTime.Parse(endDate);

			var movies = await _context.Movies
				.Where(m => m.AddedAt >= startDateDt && m.AddedAt <= endDateDt)
				.OrderByDescending(m => m.ReleaseYear).ToListAsync();

			return _mapper.Map<List<Movie>, List<MovieViewModel>>(movies);
		}

		/// <summary>
		/// Retrieves a movie by ID, including its comments.
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// GET api/Movies/5/Comments
		/// </remarks>
		/// <param name="id">The movie ID</param>
		/// <response code="200">The movie.</response>
		/// <response code="404">If the movie is not found.</response>
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpGet("{id}/Comments")]
		public async Task<ActionResult<MovieWithCommentsViewModel>> GetCommentsForMovieAsync(int id)
		{
			if (!MovieExists(id))
			{
				return NotFound();
			}

			var movie = await _context.Movies.Where(m => m.Id == id).FirstOrDefaultAsync();
			var comments = await _context.Comments.Where(c => c.MovieId == id).ToListAsync();

			var result = _mapper.Map<MovieWithCommentsViewModel>(movie);
			result.Comments = _mapper.Map<List<Comment>, List<CommentViewModel>>(comments);

			return result;
		}

		/// <summary>
		/// Retrieves a movie by ID.
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// GET api/Movies/5
		/// </remarks>
		/// <param name="id">The movie ID</param>
		/// <response code="200">The movie.</response>
		/// <response code="404">If the movie is not found.</response>
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpGet("{id}")]
		public async Task<ActionResult<MovieViewModel>> GetMovie(int id)
		{
			var movie = await _context.Movies.FindAsync(id);

			if (movie == null)
			{
				return NotFound();
			}

			return _mapper.Map<MovieViewModel>(movie);
		}

		/// <summary>
		/// Updates a movie.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// PUT /api/Movies/5
		/// {
		///		"id": 5
		///    "title": "Title",
		///    "description": "Description!",
		///    "genre": "Comedy",
		///    "durationMinutes": 20,
		///    "releaseYear": 2021,
		///    "director": "Some Director",
		///    "addedAt": "2021-08-10",
		///    "rating": 2,
		///    "watched": true
		/// }
		///
		/// </remarks>
		/// <param name="id">The movie ID</param>
		/// <param name="movie">The movie body.</param>
		/// <response code="204">If the item was successfully added.</response>
		/// <response code="400">If the ID in the URL doesn't match the one in the body.</response>
		/// <response code="404">If the item is not found.</response>
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpPut("{id}")]
		[Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
		public async Task<IActionResult> PutMovie(int id, MovieViewModel movie)
		{
			if (id != movie.Id)
			{
				return BadRequest();
			}

			_context.Entry(_mapper.Map<Movie>(movie)).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!MovieExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		/// <summary>
		/// Updates a movie comment.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// PUT: api/Movies/1/Comments/2
		/// {
		///    "text": "some comment",
		///    "important": false,
		///    "movieId": 3,
		/// }
		///
		/// </remarks>
		/// <param name="commentId">The comment ID</param>
		/// <param name="comment">The comment body</param>
		/// <response code="204">If the item was successfully added.</response>
		/// <response code="400">If the ID in the URL doesn't match the one in the body.</response>
		/// <response code="404">If the item is not found.</response>
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpPut("{id}/Comments/{commentId}")]
		public async Task<IActionResult> PutComment(int commentId, CommentViewModel comment)
		{
			if (commentId != comment.Id)
			{
				return BadRequest();
			}

			_context.Entry(_mapper.Map<Comment>(comment)).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!CommentExists(commentId))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		// POST: api/Movies
		/// <summary>
		/// Creates a movie.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// POST /api/Movies
		/// {
		///    "title": "Title",
		///    "description": "Description!",
		///    "genre": "Comedy",
		///    "durationMinutes": 20,
		///    "releaseYear": 2021,
		///    "director": "Some Director",
		///    "addedAt": "2021-08-10",
		///    "rating": 2,
		///    "watched": true
		/// }
		///
		/// </remarks>
		/// <param name="movie"></param>
		/// <response code="201">Returns the newly created item</response>
		/// <response code="400">If the item is null or the rating is not a value between 1 and 10.</response>
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpPost]
		[Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
		public async Task<ActionResult<Movie>> PostMovie(MovieViewModel movie)
		{
			_context.Movies.Add(_mapper.Map<Movie>(movie));
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetMovie", new { id = movie.Id }, movie);
		}

		/// <summary>
		/// Creates a movie comment.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// POST /api/Movies/3/Comments
		/// {
		///    "text": "some comment",
		///    "important": false,
		///    "movieId": 3,
		/// }
		///
		/// </remarks>
		/// <param name="id">The movie ID</param>
		/// <param name="comment">The comment body</param>
		/// <response code="200">If the item was successfully added.</response>
		/// <response code="404">If movie is not found.</response>  
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpPost("{id}/Comments")]
		public IActionResult PostCommentForMovie(int id, CommentViewModel comment)
		{
			var movie = _context.Movies
				.Where(m => m.Id == id)
				.Include(m => m.Comments).FirstOrDefault();

			if (movie == null)
			{
				return NotFound();
			}

			movie.Comments.Add(_mapper.Map<Comment>(comment));
			_context.Entry(movie).State = EntityState.Modified;
			_context.SaveChanges();

			return Ok();
		}

		// DELETE: api/Movies/5
		/// <summary>
		/// Deletes a movie.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// DELETE api/Movies/1
		///
		/// </remarks>
		/// <param name="id"></param>
		/// <response code="204">No content if successful.</response>
		/// <response code="404">If the movie doesn't exist.</response>  
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpDelete("{id}")]
		[Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
		public async Task<IActionResult> DeleteMovie(int id)
		{
			var movie = await _context.Movies.FindAsync(id);
			if (movie == null)
			{
				return NotFound();
			}

			_context.Movies.Remove(movie);
			await _context.SaveChangesAsync();

			return NoContent();
		}


		// DELETE: api/Movies/1/Comments/5
		/// <summary>
		/// Deletes a movie comment.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// DELETE api/Movies/1/Comments/5
		///
		/// </remarks>
		/// <param name="commentId"></param>
		/// <response code="204">No content if successful.</response>
		/// <response code="404">If the comment doesn't exist.</response>  
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpDelete("{id}/Comments/{commentId}")]
		[Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
		public async Task<IActionResult> DeleteComment(int commentId)
		{
			var comment = await _context.Comments.FindAsync(commentId);
			if (comment == null)
			{
				return NotFound();
			}

			_context.Comments.Remove(comment);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool MovieExists(int id)
		{
			return _context.Movies.Any(e => e.Id == id);
		}

		private bool CommentExists(int id)
		{
			return _context.Comments.Any(e => e.Id == id);
		}
	}
}
