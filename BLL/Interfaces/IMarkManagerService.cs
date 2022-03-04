using System.Threading.Tasks;
using DAL.Entities;

namespace BLL.Interfaces
{
    public interface IMarkManagerService
    {
        public Task SetMark(Mark mark, int newMarkValue);
    }
}