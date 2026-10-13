using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Linq.Expressions;

namespace MTK.DataSeeder.Infrastructure;

/// <summary>
/// Fake Mapper for CLI/Seeder - AutoMapper-ə ehtiyac yoxdur seeder-də
/// </summary>
internal sealed class FakeMapper : IMapper
{
    public IConfigurationProvider ConfigurationProvider => throw new NotImplementedException("FakeMapper: ConfigurationProvider is not available in seeder.");

    public TDestination Map<TDestination>(object source)
    {
        throw new NotImplementedException("FakeMapper: Map is not available in seeder. Use direct object creation instead.");
    }

    public TDestination Map<TSource, TDestination>(TSource source)
    {
        throw new NotImplementedException("FakeMapper: Map is not available in seeder. Use direct object creation instead.");
    }

    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        throw new NotImplementedException("FakeMapper: Map is not available in seeder. Use direct object creation instead.");
    }

    public object Map(object source, Type sourceType, Type destinationType)
    {
        throw new NotImplementedException("FakeMapper: Map is not available in seeder. Use direct object creation instead.");
    }

    public object Map(object source, object destination, Type sourceType, Type destinationType)
    {
        throw new NotImplementedException("FakeMapper: Map is not available in seeder. Use direct object creation instead.");
    }

    public TDestination Map<TDestination>(object source, Action<IMappingOperationOptions> opts)
    {
        throw new NotImplementedException("FakeMapper: Map is not available in seeder. Use direct object creation instead.");
    }

    public TDestination Map<TDestination>(object source, Action<IMappingOperationOptions<object, TDestination>> opts)
    {
        throw new NotImplementedException("FakeMapper: Map is not available in seeder. Use direct object creation instead.");
    }

    public TDestination Map<TSource, TDestination>(TSource source, Action<IMappingOperationOptions<TSource, TDestination>> opts)
    {
        throw new NotImplementedException("FakeMapper: Map is not available in seeder. Use direct object creation instead.");
    }

    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination, Action<IMappingOperationOptions<TSource, TDestination>> opts)
    {
        throw new NotImplementedException("FakeMapper: Map is not available in seeder. Use direct object creation instead.");
    }

    public object Map(object source, Type sourceType, Type destinationType, Action<IMappingOperationOptions> opts)
    {
        throw new NotImplementedException("FakeMapper: Map is not available in seeder. Use direct object creation instead.");
    }

    public object Map(object source, Type sourceType, Type destinationType, Action<IMappingOperationOptions<object, object>> opts)
    {
        throw new NotImplementedException("FakeMapper: Map is not available in seeder. Use direct object creation instead.");
    }

    public object Map(object source, object destination, Type sourceType, Type destinationType, Action<IMappingOperationOptions> opts)
    {
        throw new NotImplementedException("FakeMapper: Map is not available in seeder. Use direct object creation instead.");
    }

    public object Map(object source, object destination, Type sourceType, Type destinationType, Action<IMappingOperationOptions<object, object>> opts)
    {
        throw new NotImplementedException("FakeMapper: Map is not available in seeder. Use direct object creation instead.");
    }

    public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source, object? parameters = null, params Expression<Func<TDestination, object>>[] membersToExpand)
    {
        throw new NotImplementedException("FakeMapper: ProjectTo is not available in seeder.");
    }

    public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source, IDictionary<string, object>? parameters, params string[] membersToExpand)
    {
        throw new NotImplementedException("FakeMapper: ProjectTo is not available in seeder.");
    }

    public IQueryable ProjectTo(IQueryable source, Type destinationType, IDictionary<string, object>? parameters = null, params string[] membersToExpand)
    {
        throw new NotImplementedException("FakeMapper: ProjectTo is not available in seeder.");
    }
}