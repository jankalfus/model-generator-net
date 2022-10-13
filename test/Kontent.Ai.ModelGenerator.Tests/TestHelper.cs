﻿using System;
using System.Collections.Generic;
using Kontent.Ai.Management.Models.Shared;
using System.Linq;
using Kontent.Ai.Management.Models.Types;
using Kontent.Ai.Management.Models.Types.Elements;
using Newtonsoft.Json.Linq;

namespace Kontent.Ai.ModelGenerator.Tests;

internal static class TestHelper
{
    public static ElementMetadataBase GenerateElementMetadataBase(Guid elementId, string elementCodename, ElementMetadataType type = ElementMetadataType.Text) =>
        JObject.FromObject(new
        {
            Id = elementId,
            Codename = elementCodename,
            type
        }).ToObject<ElementMetadataBase>();

    public static ElementMetadataBase GenerateGuidelinesElement(Guid elementId, string elementCodename) =>
        JObject.FromObject(new
        {
            Id = elementId,
            Codename = elementCodename,
            type = ElementMetadataType.Guidelines,
            guidelines = "guidelines"

        }).ToObject<ElementMetadataBase>();

    public static LinkedItemsElementMetadataModel GenerateLinkedItemsElement(string elementId, string elementCodename, LimitModel limitModel, IEnumerable<Guid> allowedTypesIds)
    {
        var element = (LinkedItemsElementMetadataModel)GenerateElementMetadataBase(Guid.Parse(elementId), elementCodename, ElementMetadataType.LinkedItems);

        element.AllowedTypes = allowedTypesIds.Select(Reference.ById);
        element.ItemCountLimit = limitModel;

        return element;
    }
}
