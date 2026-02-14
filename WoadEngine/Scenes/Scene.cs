// --------------------------------------------------------------------------------------------
// WoadEngine
// Copyright (c) 2026 Woad Stoat Studios, All Rights Reserved.
// This source code may not be copied, modified, disclosed or distributed without permission.
//---------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using WoadEngine.ECS;
using WoadEngine.ECS.Systems;
using WoadEngine.Diagnostics;

namespace WoadEngine.Scenes;

#region Scene Base Class

/// <summary>
/// Base class for a game "scene" (state/level/screen).
///
/// Responsibilities:
/// - Owns scene-scoped content (<see cref="ContentManager"/>)
/// - Owns the ECS <see cref="World"/> for this scene (entities + components)
/// - Defines the update/draw lifecycle for derived scenes
///
/// In MonoGame terms, a scene is the unit you swap in/out during gameplay:
/// menu -> level -> pause -> game over, etc.
/// </summary>
/// <remarks>
/// Scene vs World:
/// - <see cref="World"/> is the ECS data model (entities/components/queries).
/// - <see cref="Scene"/> is orchestration: content loading, system ordering,
///   transitions, and high-level state management.
/// </remarks>
public abstract class Scene : IDisposable
{
    #region Fields / Properties
    /// <summary>
    /// Scene specific DeltaTime
    /// </summary>
    protected float DeltaTime { get; set; }
    /// <summary>
    /// Scene-local content manager.
    /// Use this for scene-owned assets so they can be unloaded cleanly on scene change.
    /// </summary>
    protected ContentManager Content { get; }

    /// <summary>
    /// ECS world for this scene. All entities/components for this scene live here.
    /// </summary>
    protected World World { get; }

    /// <summary>
    /// True once the scene has been disposed and its content unloaded.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Update systems executed during <see cref="Update(GameTime)"/>.
    /// </summary>
    protected List<ISystem> UpdateSystems { get; } = new();

    /// <summary>
    /// Render systems executed during <see cref="Draw(GameTime)"/>.
    /// </summary>
    protected List<IRenderSystem> RenderSystems { get; } = new();
    #endregion

    #region Construction

    /// <summary>
    /// Creates a scene with a scene-scoped <see cref="ContentManager"/> and a new ECS <see cref="World"/>.
    /// </summary>
    public Scene()
    {
        // Scene-local content manager allows clean unloading when the scene is removed.
        Content = new ContentManager(Core.Content.ServiceProvider)
        {
            RootDirectory = Core.Content.RootDirectory
        };

        // Each scene owns its own World by default.
        // If you later want persistent/global entities across scenes, you can introduce
        // a separate "GameWorld" owned above the scene layer.
        World = new World();
    }

    /// <summary>
    /// Finalizer in case Dispose is not called.
    /// </summary>
    ~Scene() => Dispose(false);
    #endregion

    #region Lifecycle

    /// <summary>
    /// Called once when the scene becomes active.
    /// Default behaviour calls <see cref="LoadContent"/>.
    /// </summary>
    public virtual void Initialize()
    {
        LoadContent();
        #if DEBUG
        RenderSystems.Add(new DebugColliderRenderSystem());
        #endif
    }

    /// <summary>
    /// Load scene-specific content.
    /// Override in derived scenes. Called by <see cref="Initialize"/>.
    /// </summary>
    public virtual void LoadContent() { }

    /// <summary>
    /// Unload scene-specific content.
    /// Called during disposal to release scene-local assets.
    /// </summary>
    public virtual void UnloadContent()
    {
        Content.Unload();
    }

    /// <summary>
    /// Updates the scene simulation.
    /// Default behaviour runs all registered <see cref="UpdateSystems"/>.
    /// </summary>
    /// <param name="gameTime">MonoGame timing information.</param>
    public virtual void Update(GameTime gameTime)
    {
        DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Run ECS update systems in the order they were registered.
        for (int i = 0; i < UpdateSystems.Count; i++)
        {
            UpdateSystems[i].Update(World, DeltaTime);
        }
    }

    /// <summary>
    /// Draws the scene.
    /// Default behaviour runs all registered <see cref="RenderSystems"/>.
    /// </summary>
    /// <param name="gameTime">MonoGame timing information.</param>
    public virtual void Draw(GameTime gameTime)
    {
        // Run ECS draw systems in the order they were registered.
        for (int i = 0; i < RenderSystems.Count; i++)
        {
            RenderSystems[i].Draw(World, DeltaTime);
        }
    }
    #endregion

    #region IDisposable
    /// <summary>
    /// Disposes the scene, unloading content.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Internal dispose pattern implementation.
    /// </summary>
    /// <param name="disposing">True when called from <see cref="Dispose()"/>.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        if (disposing)
        {
            // Release scene resources deterministically.
            UnloadContent();
            Content.Dispose();

            // If we later add unmanaged resources or pooled allocations to World/stores,
            // dispose them here as well (e.g., if World becomes IDisposable).
        }
        IsDisposed = true;
    }
    #endregion
}
#endregion