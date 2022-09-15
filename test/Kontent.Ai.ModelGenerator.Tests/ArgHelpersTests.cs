﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kontent.Ai.Delivery.Abstractions;
using Kontent.Ai.Management.Configuration;
using Kontent.Ai.ModelGenerator.Core.Configuration;
using Kontent.Ai.ModelGenerator.Options;
using Xunit;

namespace Kontent.Ai.ModelGenerator.Tests;

public class ArgHelpersTests
{
    private static IEnumerable<string> GeneralOptionArgs => ToLower(new List<string>
        {
            nameof(CodeGeneratorOptions.Namespace),
            nameof(CodeGeneratorOptions.OutputDir),
            nameof(CodeGeneratorOptions.FileNameSuffix),
            nameof(CodeGeneratorOptions.GeneratePartials),
            nameof(CodeGeneratorOptions.BaseClass)
        });

    private static IDictionary<string, string> ExpectedManagementMappings => new Dictionary<string, string>
        {
            { "-n", nameof(CodeGeneratorOptions.Namespace) },
            { "-o", nameof(CodeGeneratorOptions.OutputDir) },
            { "-f", nameof(CodeGeneratorOptions.FileNameSuffix) },
            { "-g", nameof(CodeGeneratorOptions.GeneratePartials) },
            { "-b", nameof(CodeGeneratorOptions.BaseClass) },
            { "-p", $"{nameof(ManagementOptions)}:{nameof(ManagementOptions.ProjectId)}" },
            { "--projectid", $"{nameof(ManagementOptions)}:{nameof(ManagementOptions.ProjectId)}" },
            { "-m", nameof(CodeGeneratorOptions.ManagementApi) },
            { "-k", $"{nameof(ManagementOptions)}:{nameof(ManagementOptions.ApiKey)}" },
            { "--apikey", $"{nameof(ManagementOptions)}:{nameof(ManagementOptions.ApiKey)}" }
        };

    private static IDictionary<string, string> ExpectedExtendedDeliveryMappings => new Dictionary<string, string>
        {
            { "-n", nameof(CodeGeneratorOptions.Namespace) },
            { "-o", nameof(CodeGeneratorOptions.OutputDir) },
            { "-f", nameof(CodeGeneratorOptions.FileNameSuffix) },
            { "-g", nameof(CodeGeneratorOptions.GeneratePartials) },
            { "-s", nameof(CodeGeneratorOptions.StructuredModel) },
            { "-b", nameof(CodeGeneratorOptions.BaseClass) },
            { "-p", $"{nameof(ManagementOptions)}:{nameof(ManagementOptions.ProjectId)}" },
            {"--projectid", $"{nameof(ManagementOptions)}:{nameof(ManagementOptions.ProjectId)}" },
            { "-t", nameof(CodeGeneratorOptions.WithTypeProvider) },
            { "-k", $"{nameof(ManagementOptions)}:{nameof(ManagementOptions.ApiKey)}" },
            { "--apikey", $"{nameof(ManagementOptions)}:{nameof(ManagementOptions.ApiKey)}" },
            { "-e", nameof(CodeGeneratorOptions.ExtendedDeliverModels) },
            { "-r", nameof(CodeGeneratorOptions.ExtendedDeliverPreviewModels) }
        };

    private static IDictionary<string, string> ExpectedDeliveryMappings => new Dictionary<string, string>
    {
        { "-n", nameof(CodeGeneratorOptions.Namespace) },
        { "-o", nameof(CodeGeneratorOptions.OutputDir) },
        { "-f", nameof(CodeGeneratorOptions.FileNameSuffix) },
        { "-g", nameof(CodeGeneratorOptions.GeneratePartials) },
        { "-s", nameof(CodeGeneratorOptions.StructuredModel) },
        { "-b", nameof(CodeGeneratorOptions.BaseClass) },
        { "-p", $"{nameof(DeliveryOptions)}:{nameof(DeliveryOptions.ProjectId)}" },
        { "--projectid", $"{nameof(DeliveryOptions)}:{nameof(DeliveryOptions.ProjectId)}" },
        { "-t", nameof(CodeGeneratorOptions.WithTypeProvider) }
    };

    [Fact]
    public void GetSwitchMappings_MissingMapiSwitch_ReturnsDeliveryMappings()
    {
        var result = ArgHelpers.GetSwitchMappings(new string[]
        {
                "-p",
                Guid.NewGuid().ToString()
        });

        Assert.Equal(ExpectedDeliveryMappings, result.Mappings);
        Assert.Equal(DesiredModelsType.Delivery, result.DesiredModelsType);
    }

    [Fact]
    public void GetSwitchMappings_MapiSwitchIsFalse_ReturnsDeliveryMappings()
    {
        var result = ArgHelpers.GetSwitchMappings(new string[]
        {
                "-p",
                Guid.NewGuid().ToString(),
                "-m",
                "false"
        });

        Assert.Equal(ExpectedDeliveryMappings, result.Mappings);
        Assert.Equal(DesiredModelsType.Delivery, result.DesiredModelsType);
    }

    [Fact]
    public void GetSwitchMappings_MapiSwitchIsTrue_ReturnsManagementMappings()
    {
        var result = ArgHelpers.GetSwitchMappings(new string[]
        {
                "-p",
                Guid.NewGuid().ToString(),
                "-m",
                "true"
        });

        Assert.Equal(ExpectedManagementMappings, result.Mappings);
        Assert.Equal(DesiredModelsType.Management, result.DesiredModelsType);
    }

    [Fact]
    public void GetSwitchMappings_ExtendedDeliveryIsTrue_ReturnsManagementMappings()
    {
        var result = ArgHelpers.GetSwitchMappings(new string[]
        {
            "-p",
            Guid.NewGuid().ToString(),
            "-e",
            "true"
        });

        Assert.Equal(ExpectedExtendedDeliveryMappings, result.Mappings);
        Assert.Equal(DesiredModelsType.ExtendedDelivery, result.DesiredModelsType);
    }

    [Fact]
    public void GetSwitchMappings_ExtendedDeliveryPreviewIsTrue_ReturnsManagementMappings()
    {
        var result = ArgHelpers.GetSwitchMappings(new string[]
        {
            "-p",
            Guid.NewGuid().ToString(),
            "-r",
            "true"
        });

        Assert.Equal(ExpectedExtendedDeliveryMappings, result.Mappings);
        Assert.Equal(DesiredModelsType.ExtendedDelivery, result.DesiredModelsType);
    }

    [Fact]
    public void GetSwitchMappings_ExtendedDeliveryAndPreviewIsTrue_ReturnsManagementMappings()
    {
        var result = ArgHelpers.GetSwitchMappings(new string[]
        {
            "-p",
            Guid.NewGuid().ToString(),
            "-e",
            "true",
            "-r",
            "true"
        });

        Assert.Equal(ExpectedExtendedDeliveryMappings, result.Mappings);
        Assert.Equal(DesiredModelsType.ExtendedDelivery, result.DesiredModelsType);
    }

    [Fact]
    public void ContainsContainsValidArgs_SupportedDeliveryOptions_ReturnsTrue()
    {
        var args = AppendValuesToArgs(ExpectedDeliveryMappings)
            .Concat(AppendValuesToArgs(ToLower(new List<string>
            {
                    nameof(CodeGeneratorOptions.StructuredModel),
                    nameof(CodeGeneratorOptions.WithTypeProvider)
            })))
            .Concat(AppendValuesToArgs(GeneralOptionArgs))
            .Concat(AppendValuesToArgs(typeof(DeliveryOptions)))
            .ToArray();

        var result = ArgHelpers.ContainsValidArgs(args);

        Assert.True(result);
    }

    [Theory]
    [InlineData("-x")]
    [InlineData("--projectidX")]
    [InlineData("--DeliveryOptionsX:UseSecureAccess")]
    [InlineData("--DeliveryOptions:UseSecureAccessX")]
    public void ContainsContainsValidArgs_UnsupportedDeliveryOptions_ReturnsFalse(string arg)
    {
        var args = new[]
        {
            arg,
            "arg_value"
        };
        var result = ArgHelpers.ContainsValidArgs(args);

        Assert.False(result);
    }

    [Fact]
    public void ContainsContainsValidArgs_SupportedManagementOptions_ReturnsTrue()
    {
        var args = AppendValuesToArgs(ExpectedManagementMappings)
            .Concat(AppendValuesToArgs(ToLower(new List<string>
            {
                nameof(CodeGeneratorOptions.ManagementApi)
            })))
            .Concat(AppendValuesToArgs(GeneralOptionArgs))
            .Concat(AppendValuesToArgs(typeof(ManagementOptions)))
            .ToArray();

        var result = ArgHelpers.ContainsValidArgs(args);

        Assert.True(result);
    }

    [Theory]
    [InlineData("-x")]
    [InlineData("--contentmanagementapi")]
    [InlineData("--managementapiX")]
    [InlineData("--ManagementOptions:ApiKeyX")]
    [InlineData("--ManagementOptionsX:ApiKey")]
    public void ContainsContainsValidArgs_UnsupportedManagementOptions_ReturnsFalse(string arg)
    {
        var args = new[]
        {
            arg,
            "arg_value"
        };
        var result = ArgHelpers.ContainsValidArgs(args);

        Assert.False(result);
    }

    [Fact]
    public void ContainsContainsValidArgs_SupportedExtendedDeliveryOptions_ReturnsTrue()
    {
        var args = AppendValuesToArgs(ExpectedExtendedDeliveryMappings)
            .Concat(AppendValuesToArgs(ToLower(new List<string>
            {
                nameof(CodeGeneratorOptions.StructuredModel),
                nameof(CodeGeneratorOptions.WithTypeProvider)
            })))
            .Concat(AppendValuesToArgs(GeneralOptionArgs))
            .Concat(AppendValuesToArgs(typeof(ManagementOptions)))
            .ToArray();

        var result = ArgHelpers.ContainsValidArgs(args);

        Assert.True(result);
    }

    [Theory]
    [InlineData("-x")]
    [InlineData("--contentmanagementapi")]
    [InlineData("--managementapix")]
    [InlineData("--ManagementOptions:ApiKeyX")]
    [InlineData("--ManagementOptionsX:ApiKey")]
    [InlineData("--DeliveryOptionsX:UseSecureAccess")]
    [InlineData("--DeliveryOptions:UseSecureAccessX")]
    public void ContainsContainsValidArgs_UnsupportedExtendedDeliveryOptions_ReturnsFalse(string arg)
    {
        var args = new[]
        {
            arg,
            "arg_value"
        };

        var result = ArgHelpers.ContainsValidArgs(args);

        Assert.False(result);
    }

    [Fact]
    public void GetProgramOptionsData_ManagementApi_ReturnsManagementProgramOptionsData()
    {
        var result = ArgHelpers.GetUsedSdkInfo(DesiredModelsType.Management);

        Assert.Equal("management-sdk-net", result.Name);
        Assert.Equal(Assembly.GetAssembly(typeof(ManagementOptions)).GetName().Version.ToString(3), result.Version);
    }

    [Fact]
    public void GetProgramOptionsData_DeliveryApi_ReturnsDeliveryProgramOptionsData()
    {
        var result = ArgHelpers.GetUsedSdkInfo(DesiredModelsType.Delivery);

        Assert.Equal("delivery-sdk-net", result.Name);
        Assert.Equal(Assembly.GetAssembly(typeof(DeliveryOptions)).GetName().Version.ToString(3), result.Version);
    }

    [Fact]
    public void GetProgramOptionsData_ExtendedDeliveryApi_ReturnsExtendedDeliveryProgramOptionsData()
    {
        var result = ArgHelpers.GetUsedSdkInfo(DesiredModelsType.ExtendedDelivery);

        Assert.Equal("delivery-sdk-net", result.Name);
        Assert.Equal(Assembly.GetAssembly(typeof(DeliveryOptions)).GetName().Version.ToString(3), result.Version);
    }

    private static IEnumerable<string> AppendValuesToArgs(IDictionary<string, string> mappings) => AppendValuesToArgs(mappings.Keys);

    private static IEnumerable<string> AppendValuesToArgs(Type type) =>
        AppendValuesToArgs(type.GetProperties().Select(p => $"{type.Name}:{p.Name}"));

    private static IEnumerable<string> ToLower(IEnumerable<string> args) => args.Select(a => a.ToLower());

    private static IEnumerable<string> AppendValuesToArgs(IEnumerable<string> args)
    {
        var argsWithValue = new List<string>();
        foreach (var arg in args)
        {
            argsWithValue.Add(arg.StartsWith('-') ? arg : $"--{arg}");
            argsWithValue.Add("arg_value");
        }
        return argsWithValue;
    }
}
