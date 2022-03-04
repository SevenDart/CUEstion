using System;
using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.EF;
using DAL.Entities;

namespace BLL.Implementations
{
    public class MarkManagerService: IMarkManagerService
    {
        private readonly ApplicationContext _context;

        public MarkManagerService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task SetMark(Mark mark, int newMarkValue)
        {
            if (mark != null && mark.MarkValue == newMarkValue)
            {
                throw new Exception("This user have already voted such.");
            }

            mark!.MarkValue += newMarkValue;
            
            var user = await _context.Users.FindAsync(mark.MarkValue);
            user.Rate += newMarkValue;

            await _context.SaveChangesAsync();
        }
    }
}