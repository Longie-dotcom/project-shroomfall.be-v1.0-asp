using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Contract.DTO.Domain.Definition;
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
            Console.WriteLine("[ComponentDiscovery] ==================================================");
            Console.WriteLine("[ComponentDiscovery] Starting automated component pipeline engine scan...");
            Console.WriteLine("[ComponentDiscovery] ==================================================");

            var assembly = typeof(IEntityDefinitionRepository).Assembly;
            var uowType = typeof(IRelationalUoW);
            var getRepoGenericDefinition = uowType.GetMethod(nameof(IRelationalUoW.GetRepository));

            if (getRepoGenericDefinition == null)
            {
                Console.WriteLine("[ComponentDiscovery] CRITICAL ERROR: Could not find IRelationalUoW.GetRepository generic method definition.");
                return;
            }

            // Scan for all interfaces that implement ISQLDefinitionRepository<T>
            var componentRepositoryMatches = assembly.GetTypes()
                .Where(t => t.IsInterface)
                .Select(t => new
                {
                    RepoInterface = t,
                    DefinitionInterface = t.GetInterfaces()
                        .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISQLDefinitionRepository<>))
                })
                .Where(x => x.DefinitionInterface != null)
                .ToList();

            Console.WriteLine($"[ComponentDiscovery] Detected {componentRepositoryMatches.Count} interfaces inheriting from ISQLDefinitionRepository<>");

            foreach (var match in componentRepositoryMatches)
            {
                var domainComponentType = match.DefinitionInterface!.GetGenericArguments()[0];

                // Strict Guard: Ensure T actually inherits from ComponentDefinition 
                if (!typeof(ComponentDefinition).IsAssignableFrom(domainComponentType))
                {
                    Console.WriteLine($"[ComponentDiscovery] [SKIPPED] {match.RepoInterface.Name} -> Domain type '{domainComponentType.Name}' does not inherit from ComponentDefinition.");
                    continue;
                }

                var getByEntityIdMethod = match.DefinitionInterface.GetMethod("GetByEntityIdAsync", new[] { typeof(string) });
                if (getByEntityIdMethod == null)
                {
                    Console.WriteLine($"[ComponentDiscovery] [ERROR] {match.RepoInterface.Name} -> Method 'GetByEntityIdAsync' could not be resolved from definition layout.");
                    continue;
                }

                var componentName = domainComponentType.Name;
                var expectedDtoName = $"{componentName}DTO";

                var dtoType = Assembly.GetAssembly(typeof(ComponentDefinitionDTO))?
                    .GetTypes()
                    .FirstOrDefault(t => t.Name == expectedDtoName);

                if (dtoType == null)
                {
                    Console.WriteLine($"[ComponentDiscovery] [ERROR] {match.RepoInterface.Name} -> Missing counterpart target DTO class named '{expectedDtoName}'.");
                    continue;
                }

                var concreteGetRepoMethod = getRepoGenericDefinition.MakeGenericMethod(match.RepoInterface);

                pipelines.Add(new ComponentPipelineDescriptor(
                    concreteGetRepoMethod,
                    getByEntityIdMethod,
                    dtoType
                ));

                Console.WriteLine($"[ComponentDiscovery] [SUCCESS] Registered: {match.RepoInterface.Name} -> Bound to domain element: {componentName} -> Transpiling to: {expectedDtoName}");
            }

            Console.WriteLine("[ComponentDiscovery] ==================================================");
            Console.WriteLine($"[ComponentDiscovery] Engine initialization complete. Total active pipelines populated: {pipelines.Count}");
            Console.WriteLine("[ComponentDiscovery] ==================================================");
        }
        #endregion
    }
}