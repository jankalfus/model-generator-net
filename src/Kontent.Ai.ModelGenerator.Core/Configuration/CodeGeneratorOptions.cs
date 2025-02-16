﻿using System;
using System.Linq;
using Kontent.Ai.Delivery.Abstractions;
using Kontent.Ai.Management.Configuration;

namespace Kontent.Ai.ModelGenerator.Core.Configuration;

public class CodeGeneratorOptions
{
    private const char StructuredModelSeparator = ',';
    private const bool DefaultGeneratePartials = true;
    private const bool DefaultWithTypeProvider = true;
    private const bool DefaultManagementApi = false;
    private const string DefaultFileNameSuffix = "Generated";
    private const StructuredModelFlags DefaultStructuredModelFlags = StructuredModelFlags.NotSet;

    /// <summary>
    /// Delivery Client configuration.
    /// </summary>
    public DeliveryOptions DeliveryOptions { get; set; }

    /// <summary>
    /// Management Client configuration.
    /// </summary>
    public ManagementOptions ManagementOptions { get; set; }

    /// <summary>
    /// Namespace name of the generated classes
    /// </summary>
    public string Namespace { get; set; }

    /// <summary>
    /// Output directory for the generated files
    /// </summary>
    public string OutputDir { get; set; }

    /// <summary>
    /// Optionally add suffix to the generated files
    /// </summary>
    public string FileNameSuffix { get; set; } = DefaultFileNameSuffix;

    /// <summary>
    /// Optionally generate partial classes for user customization
    /// </summary>
    public bool GeneratePartials { get; set; } = DefaultGeneratePartials;

    /// <summary>
    /// Indicates whether the CustomTypeProvider class should be generated
    /// </summary>
    public bool WithTypeProvider { get; set; } = DefaultWithTypeProvider;

    /// <summary>
    /// Indicates whether the classes should be generated with types that represent structured data model
    /// </summary>
    public string StructuredModel
    {
        get => null;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                StructuredModelFlags = StructuredModelFlags.NotSet;
                return;
            }

            var splitStructuredModels = value.Split(StructuredModelSeparator);

            if (!splitStructuredModels.Any())
            {
                StructuredModelFlags = StructuredModelFlags.NotSet;
                return;
            }

            StructuredModelFlags = splitStructuredModels
                .Select(structuredModel =>
                    Enum.TryParse<StructuredModelFlags>(structuredModel, true, out var parsed)
                        ? parsed
                        : StructuredModelFlags.ValidationIssue)
                .Aggregate((result, next) => result | next);
        }
    }

    public StructuredModelFlags StructuredModelFlags { get; private set; } = DefaultStructuredModelFlags;

    /// <summary>
    /// Indicates whether the classes should be generated for CM API SDK
    /// </summary>
    public bool ManagementApi { get; set; } = DefaultManagementApi;

    /// <summary>
    /// Indicates whether a base class should be created and all output classes should derive from it using a partial class
    /// </summary>
    public string BaseClass { get; set; }
}
