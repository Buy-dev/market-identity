namespace Market.Identity.Application.Infrastructure.Mappers;

public interface IMapWith<in TSource, out TResult>
{
    public TResult Map(TSource source);
}