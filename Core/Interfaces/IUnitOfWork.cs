using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUnitOfWork<T> where T : class
    {
        // الحفظ فقط لانها خاصه بالتعامل مع الداتابيز
        IGenericRepository<T> Entity { get; }
        Task Save();
        Task Dispose();
    }
}
