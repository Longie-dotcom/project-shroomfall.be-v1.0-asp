using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Contract.DTO.Abstraction;
using Domain.Abstraction;
using System.Reflection;

namespace Application.Services.DesignService
{
    public record ComponentPipelineDescriptor(
        MethodInfo GetRepoMethod,
        MethodInfo GetByEntityIdMethod,
        Type DtoType
    );

    public class ComponentDiscoveryRegistry
    {
        #region Attributes
        private readonly List<ComponentPipelineDescriptor> pipelines = new();
        #endregion

        #region Properties
        public IReadOnlyList<ComponentPipelineDescriptor> GetPipelines() => pipelines;
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
            var getRepoGenericDefinition = uowType.GetMethod(nameof(IRelationalUoW.GetRepository));

            if (getRepoGenericDefinition == null) return;

            // Scan for all interfaces that implement ISQLDefinitionRepository<T>
            var componentRepositoryMatches = assembly.GetTypes()
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
                if (!typeof(ComponentDefinition).IsAssignableFrom(domainComponentType)) continue;

                // Grab the method directly from the closed generic definition interface instance 
                var getByEntityIdMethod = match.DefinitionInterface.GetMethod("GetByEntityIdAsync", new[] { typeof(string) });
                if (getByEntityIdMethod == null) continue;

                // Resolve the DTO name using the Domain Model name safely (e.g., "AppearanceDefinition" -> "AppearanceDefinitionDTO")
                var componentName = domainComponentType.Name;
                var dtoType = Assembly.GetAssembly(typeof(ComponentDefinitionDTO))?
                    .GetTypes()
                    .FirstOrDefault(t => t.Name == $"{componentName}DTO");

                if (dtoType == null) continue;

                // Create the concrete UoW lookup method call: relationalUoW.GetRepository<IAppearanceDefinitionRepository>()
                var concreteGetRepoMethod = getRepoGenericDefinition.MakeGenericMethod(match.RepoInterface);

                pipelines.Add(new ComponentPipelineDescriptor(
                    concreteGetRepoMethod,
                    getByEntityIdMethod,
                    dtoType
                ));
            }
        }
        #endregion
    }
}