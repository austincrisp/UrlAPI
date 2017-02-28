using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using UrlAPI.Models;

namespace UrlAPI.Controllers
{
    public class BookmarkController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Bookmark
        public IQueryable<Bookmark> GetBookmarks()
        {
            var userId = User.Identity.GetUserId();

            return db.Bookmarks.Where(b => b.MakePublic == true || b.OwnerId == userId);
        }

        // GET: api/Bookmark/5
        [ResponseType(typeof(Bookmark))]
        public IHttpActionResult GetBookmark(int id)
        {
            var userId = User.Identity.GetUserId();
            Bookmark bookmark = db.Bookmarks.Where(b => b.Id == id && (b.MakePublic == true || b.OwnerId == userId)).FirstOrDefault();
            if (bookmark == null)
            {
                return NotFound();
            }

            return Ok(bookmark);
        }

        // PUT: api/Bookmark/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutBookmark(int id, Bookmark bookmark)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != bookmark.Id)
            {
                return BadRequest();
            }

            db.Entry(bookmark).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookmarkExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Bookmark
        [ResponseType(typeof(Bookmark))]
        public IHttpActionResult PostBookmark(Bookmark bookmark)
        {
            if (ModelState.IsValid)
            {
                string output;
                byte[] byteData = Encoding.ASCII.GetBytes(bookmark.LongUrl);
                Stream inputStream = new MemoryStream(byteData);

                using (SHA256 shaM = new SHA256Managed())
                {
                    var result = shaM.ComputeHash(inputStream);
                    output = BitConverter.ToString(result);
                }
                bookmark.ShortUrl = output.Replace("-", "").Substring(0, 5);
            }

            bookmark.OwnerId = User.Identity.GetUserId();
            db.Bookmarks.Add(bookmark);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = bookmark.Id }, bookmark);
        }

        // DELETE: api/Bookmark/5
        [ResponseType(typeof(Bookmark))]
        public IHttpActionResult DeleteBookmark(int id)
        {
            Bookmark bookmark = db.Bookmarks.Find(id);
            if (bookmark == null)
            {
                return NotFound();
            }

            db.Bookmarks.Remove(bookmark);
            db.SaveChanges();

            return Ok(bookmark);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BookmarkExists(int id)
        {
            return db.Bookmarks.Count(e => e.Id == id) > 0;
        }

        public IHttpActionResult DetailBookmark(string ShortUrl)
        {
            var viewPosts = db.Bookmarks.Where(b => b.ShortUrl == ShortUrl).FirstOrDefault();

            return Ok(viewPosts.LongUrl);
        }
    }
}