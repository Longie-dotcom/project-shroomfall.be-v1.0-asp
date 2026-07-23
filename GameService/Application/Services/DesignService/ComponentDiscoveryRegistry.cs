using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Contract.Attributes;
using Contract.DTO.Abstraction;
using Contract.Enum.EntityDomain;
using Domain.Abstraction;
using Domain.DomainException;
using ResponseCode;
using System.Reflection;

namespace Application.Services.DesignService
{
    public sealed class ComponentDescriptor
    {
        public required string Id { get; init; }
        public required Type DtoType { get; init; }
        public required Type DomainType { get; init; }
        public required Type RepositoryInterface { get; init; }
        public required MethodInfo GetRepositoryMethod { get; init; }
        public required MethodInfo GetByEntityIdMethod { get; init; }
        public required EntityType[] SupportedEntityTypes { get; init; }
    }

    public class ComponentDiscoveryRegistry
    {
        #region Attributes
        private readonly List<ComponentDescriptor> components = new();
        #endregion

        #region Properties
        public IReadOnlyList<ComponentDescriptor> GetComponents() => components;
        #endregion

        public ComponentDiscoveryRegistry()
        {
            InitializeDiscovery();
        }

        #region Methods
        private void InitializeDiscovery()
        {
            var assembly = typeof(IEntityDefinitionRepository).Assembly;
            var uowType = typeof(IRelationalUoW);
            var dtoAssembly = typeof(ComponentDefinitionDTO).Assembly;

            var getRepoGenericDefinition = uowType.GetMethod(nameof(IRelationalUoW.GetRepository));
            if (getRepoGenericDefinition == null)
                return;

            // Scan for all interfaces that implement ISQLDefinitionRepository<T>
            var componentRepositoryMatches = assembly
                .GetTypes()
                .Where(t => t.IsInterface)
                .Select(t => new
                {
                    RepoInterface = t,
                    // Find if this interface inherits from ISQLDefinitionRepository<T>
                    DefinitionInterface = t.GetInterfaces()
                        .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISQLDefinitionRepository<>))
                })
                // Filter out any interfaces that don't inherit from ISQLDefinitionRepository<T>
                .Where(x => x.DefinitionInterface != null)
                .ToList();

            foreach (var match in componentRepositoryMatches)
            {
                // Extract the explicit generic argument type T (e.g., AppearanceDefinition, AIDefinition)
                var domainComponentType = match.DefinitionInterface!.GetGenericArguments()[0];

                // Strict Guard: Ensure T actually inherits from ComponentDefinition 
                // This safely ignores other system definitions like ItemDefinition or QuestDefinition
                // Resolve the DTO name using the Domain Model name safely (e.g., "AppearanceDefinition" -> "AppearanceDefinitionDTO")
                var componentName = domainComponentType.Name;
                if (!typeof(ComponentDefinition).IsAssignableFrom(domainComponentType)) 
                    continue;

                // Grab the method directly from the closed generic definition interface instance 
                var getByEntityIdMethod = match.DefinitionInterface.GetMethod("GetByEntityIdAsync", new[] { typeof(string) });
                if (getByEntityIdMethod == null) 
                    continue;

                // Resolve DTO
                var dtoType = dtoAssembly.GetTypes().FirstOrDefault(t => t.Name == $"{componentName}DTO");
                if (dtoType == null) 
                    continue;

                // Resolve attributes of DTO
                var attribute = dtoType.GetCustomAttribute<EntityComponentAttribute>();
                if (attribute == null)
                    throw new InternalException(
                        ApplicationCode.ComponentDiscoveryRegistryCode.DTOMissingAttribute,
                        $"{dtoType.Name} must be decorated with {nameof(EntityComponentAttribute)}.");

                // Create the concrete UoW lookup method call: relationalUoW.GetRepository<IAppearanceDefinitionRepository>()
                var concreteGetRepoMethod = getRepoGenericDefinition.MakeGenericMethod(match.RepoInterface);
                components.Add(new ComponentDescriptor
                {
                    Id = dtoType.Name,
                    DtoType = dtoType,
                    DomainType = domainComponentType,
                    RepositoryInterface = match.RepoInterface,
                    GetRepositoryMethod = concreteGetRepoMethod,
                    GetByEntityIdMethod = getByEntityIdMethod,
                    SupportedEntityTypes = attribute.SupportedEntityTypes
                });
            }
        }
        #endregion
    }
}