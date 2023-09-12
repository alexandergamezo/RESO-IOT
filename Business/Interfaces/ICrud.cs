namespace Business.Interfaces
{
    public interface ICrud<in TModel> where TModel : class
    {
        Task AddAsync(IEnumerable<TModel> model);
    }
}
