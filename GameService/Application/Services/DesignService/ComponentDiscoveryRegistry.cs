using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Contract.DTO.Domain.Definition;
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
            // Scan for repository interfaces ending with "DefinitionRepository"
            var repositoryInterfaces = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsInterface && t.Name.EndsWith("DefinitionRepository") && t.Name != nameof(IEntityDefinitionRepository))
                .ToList();

            var uowType = typeof(IRelationalUoW);
            var getRepoGenericDefinition = uowType.GetMethod(nameof(IRelationalUoW.GetRepository));

            if (getRepoGenericDefinition == null) return;

            foreach (var repoInterface in repositoryInterfaces)
            {
                var getByEntityIdMethod = repoInterface.GetMethod("GetByEntityIdAsync", new[] { typeof(string) });
                if (getByEntityIdMethod == null) continue;

                // Extract core domain name (e.g., "AI", "Collision")
                var componentName = repoInterface.Name[1..^10]; // Strips leading 'I' and trailing 'Repository'

                var dtoType = Assembly.GetAssembly(typeof(ComponentDefinitionDTO))?
                    .GetTypes()
                    .FirstOrDefault(t => t.Name == $"{componentName}DTO");

                if (dtoType == null) continue;

                var concreteGetRepoMethod = getRepoGenericDefinition.MakeGenericMethod(repoInterface);

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