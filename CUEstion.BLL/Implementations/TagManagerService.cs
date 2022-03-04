using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CUEstion.BLL.Interfaces;
using CUEstion.DAL.EF;
using CUEstion.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CUEstion.BLL.Implementations
{
    public class TagManagerService: ITagManagerService
    {
        private readonly ApplicationContext _context;

        public TagManagerService(ApplicationContext context)
        {
            _context = context;
        }
        
        public async Task<IEnumerable<string>> GetAllTags()
        {
            var tags = _context.Tags.Select(t => t.Name).ToListAsync();
            return await tags;
        }

        public async Task CreateTag(string tag)
        {
            var foundTag = await _context.Tags.FirstOrDefaultAsync(t => String.Equals(t.Name, tag, StringComparison.CurrentCultureIgnoreCase));
            if (foundTag == null)
            {
                var dbTag = new Tag() { Name = tag };
                _context.Tags.Add(dbTag);
            }
            else
            {
                throw new Exception("Such tag already exists.");
            }

            await _context.SaveChangesAsync();
        }


        public async Task UpdateTag(string oldTag, string newTag)
        {
            var foundTag = await _context.Tags.FirstOrDefaultAsync(t => String.Equals(t.Name, oldTag, StringComparison.CurrentCultureIgnoreCase));
			
            if (foundTag == null)
            {
                throw new Exception("There is no such tag.");
            }
            else
            {
                foundTag.Name = newTag;
            }

            await _context.SaveChangesAsync();
        }


        public async Task DeleteTag(string tag)
        {
            var foundTag = _context.Tags.FirstOrDefault(t => String.Equals(t.Name, tag, StringComparison.CurrentCultureIgnoreCase));
			
            if (foundTag == null)
            {
                throw new Exception("There is no such tag.");
            }

            _context.Tags.Remove(foundTag);

            await _context.SaveChangesAsync();
        }
    }
}