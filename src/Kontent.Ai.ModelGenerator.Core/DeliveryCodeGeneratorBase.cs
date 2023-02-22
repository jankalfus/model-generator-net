using Kontent.Ai.ModelGenerator.Core.Configuration;
using Kontent.Ai.ModelGenerator.Core.Generators.Class;
using Kontent.Ai.ModelGenerator.Core.Generators;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Kontent.Ai.ModelGenerator.Core;

public abstract class DeliveryCodeGeneratorBase : CodeGeneratorBase
{
    protected DeliveryCodeGeneratorBase(IOptions<CodeGeneratorOptions> options, IOutputProvider outputProvider) : base(options, outputProvider)
    {
    }

    public new async Task<int> RunAsync()
    {
        await base.RunAsync();

        if (Options.WithTypeProvider)
        {
            await GenerateTypeProvider();
        }

        return 0;
    }

    protected override Task<ICollection<ClassCodeGenerator>> GetClassCodeGenerators() =>
        Task.FromResult<ICollection<ClassCodeGenerator>>(new List<ClassCodeGenerator>
            {
                new ContentItemClassCodeGenerator(Options.Namespace)
            });

    private async Task GenerateTypeProvider()
    {
        var classCodeGenerators = await GetClassCodeGenerators();

        if (!classCodeGenerators.Any())
        {
            Console.WriteLine(NoContentTypeAvailableMessage);
            return;
        }

        var typeProviderCodeGenerator = new TypeProviderCodeGenerator(Options.Namespace);

        foreach (var codeGenerator in classCodeGenerators)
        {
            var className = codeGenerator is ContentItemClassCodeGenerator ? codeGenerator.ClassDefinition.Codename : codeGenerator.ClassDefinition.ClassName;
            typeProviderCodeGenerator.AddContentType(codeGenerator.ClassDefinition.Codename, className);
        }

        var typeProviderCode = typeProviderCodeGenerator.GenerateCode();
        WriteToOutputProvider(typeProviderCode, TypeProviderCodeGenerator.ClassName, true);
    }
}
