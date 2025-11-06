namespace Mapper;
public interface IMapper<TIn, TOut>
{
    TOut Map(TIn input);
}
