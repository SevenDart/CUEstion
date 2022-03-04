using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.EF;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementations
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
            var tags = await _context
                .Tags
                .Select(t => t.Name)
                .ToListAsync();
            return tags;
        }

        public async Task<Tag> FindTag(string tag)
        {
            var foundTag = await _context
                .Tags
                .FirstOrDefaultAsync(t => t.Name.ToLower() == tag.ToLower());
            return foundTag;
        }

        public async Task CreateTag(string tag)
        {
            var foundTag = await _context
                .Tags
                .FirstOrDefaultAsync(t => t.Name.ToLower() == tag.ToLower());
            
            if (foundTag != null)
            {
                throw new Exception("Such tag already exists.");
            }

            var dbTag = new Tag() { Name = tag };
            _context.Tags.Add(dbTag);

            await _context.SaveChangesAsync();
        }


        public async Task UpdateTag(string oldTag, string newTag)
        {
            var foundTag = await _context
                .Tags
                .FirstOrDefaultAsync(t => t.Name.ToLower() == oldTag.ToLower());
			
            if (foundTag == null)
            {
                throw new Exception("There is no such tag.");
            }

            foundTag.Name = newTag;

            await _context.SaveChangesAsync();
        }


        public async Task DeleteTag(string tag)
        {
            var foundTag = await _context
                .Tags
                .FirstOrDefaultAsync(t => t.Name.ToLower() == tag.ToLower());
			
            if (foundTag == null)
            {
                throw new Exception("There is no such tag.");
            }

            _context.Tags.Remove(foundTag);

            await _context.SaveChangesAsync();
        }
    }
}