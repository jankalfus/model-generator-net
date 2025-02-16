﻿using Kontent.Ai.ModelGenerator.Core.Configuration;
using Xunit;

namespace Kontent.Ai.ModelGenerator.Tests.Configuration;

public class CodeGeneratorOptionsTests
{
    [Theory]
    [InlineData(StructuredModelFlags.DateTime)]
    [InlineData(StructuredModelFlags.DateTime | StructuredModelFlags.True | StructuredModelFlags.NotSet | StructuredModelFlags.RichText | StructuredModelFlags.ValidationIssue)]
    public void StructuredModelFlags_CorrectOptions(StructuredModelFlags structuredModel)
    {
        var codeGenerationOptions = new CodeGeneratorOptions
        {
            StructuredModel = structuredModel.ToString()
        };

        Assert.Equal(structuredModel, codeGenerationOptions.StructuredModelFlags);
    }

    [Fact]
    public void StructuredModelFlags_ObsoleteOption_CorrectOptions()
    {
        var codeGenerationOptions = new CodeGeneratorOptions
        {
            StructuredModel = "true"
        };

        Assert.Equal(StructuredModelFlags.True, codeGenerationOptions.StructuredModelFlags);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void StructuredModelFlags_NullOrWhiteSpace_ReturnsNotSet(string structuredModel)
    {
        var codeGenerationOptions = new CodeGeneratorOptions
        {
            StructuredModel = structuredModel
        };

        Assert.Equal(StructuredModelFlags.NotSet, codeGenerationOptions.StructuredModelFlags);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("asdasd")]
    [InlineData("true")]
    [InlineData(null)]
    public void StructuredModel_Get_ReturnsNull(string structuredModel)
    {
        var codeGenerationOptions = new CodeGeneratorOptions
        {
            StructuredModel = structuredModel
        };

        Assert.Null(codeGenerationOptions.StructuredModel);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("invalid,invalid")]
    [InlineData("invalid,")]
    [InlineData(",invalid")]
    [InlineData(",")]
    public void StructuredModelFlags_InvalidEnumValue_ReturnsValidationIssue(string structuredModel)
    {
        var codeGenerationOptions = new CodeGeneratorOptions
        {
            StructuredModel = structuredModel
        };

        Assert.Equal(StructuredModelFlags.ValidationIssue, codeGenerationOptions.StructuredModelFlags);
    }

    [Theory]
    [InlineData("invalid,DateTime", StructuredModelFlags.ValidationIssue | StructuredModelFlags.DateTime)]
    [InlineData("DateTime,invalid", StructuredModelFlags.DateTime | StructuredModelFlags.ValidationIssue)]
    public void StructuredModelFlags_InvalidEnumValueWithValid_ReturnsValidationIssueAndValidEnumValue(string structuredModel, StructuredModelFlags expected)
    {
        var codeGenerationOptions = new CodeGeneratorOptions
        {
            StructuredModel = structuredModel
        };

        Assert.Equal(expected, codeGenerationOptions.StructuredModelFlags);
    }
}
