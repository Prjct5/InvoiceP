using System.Collections.Generic;

namespace FurniTrack.Data.Repositories
{
    public interface IRepository<T>
    {
        T? GetById(int id);
        List<T> GetAll();
        int Insert(T entity);
        bool Update(T entity);
        bool Delete(int id);
    }
}
