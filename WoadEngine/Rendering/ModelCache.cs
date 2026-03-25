// --------------------------------------------------------------------------------------------
// WoadEngine
// Copyright (c) 2026 Woad Stoat Studios, All Rights Reserved.
// This source code may not be copied, modified, disclosed or distributed without permission.
//---------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WoadEngine.Rendering;

public sealed class ModelCache
{
    private readonly Dictionary<string, Model> _models = new();

    public ModelCache() { }

    public Model Get(string assetName)
    {
        if (_models.TryGetValue(assetName, out var model))
            return model;

        model = Core.Content.Load<Model>(assetName);
        _models.Add(assetName, model);
        return model;
    }

    public void Clear()
    {
        _models.Clear();
    }
}