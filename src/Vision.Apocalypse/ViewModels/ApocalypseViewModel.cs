﻿//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Fenestra.ViewModels;
using BadEcho.Odin;

namespace BadEcho.Omnified.Vision.Apocalypse.ViewModels;

/// <summary>
/// Provides a view model that displays events generated by the Apocalypse system in an Omnified game.
/// </summary>
internal sealed class ApocalypseViewModel : PolymorphicCollectionViewModel<ApocalypseEvent, IApocalypseEventViewModel>
{
    private double _effectMessageMaxWidth;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApocalypseViewModel"/> class.
    /// </summary>
    public ApocalypseViewModel()
        : base(new CollectionViewModelOptions { AsyncBatchBindings = true, Capacity = 3 },
               new PresortedInsertionStrategy<IApocalypseEventViewModel, DateTime>(vm => vm.Timestamp, true))
    {   // TODO: change capacity to configured max messages.
        // Events from Player Apocalypse die rolls.
        RegisterDerivation<ExtraDamageEvent, PlayerApocalypseEventViewModel<ExtraDamageEvent>>();
        RegisterDerivation<TeleportitisEvent, PlayerApocalypseEventViewModel<TeleportitisEvent>>();
        RegisterDerivation<RiskOfMurderEvent, RiskOfMurderEventViewModel>();
        RegisterDerivation<OrgasmEvent, PlayerApocalypseEventViewModel<OrgasmEvent>>();

        // Fatalis events.
        RegisterDerivation<FatalisDeathEvent, ApocalypseEventViewModel<FatalisDeathEvent>>();
        RegisterDerivation<FatalisCuredEvent, ApocalypseEventViewModel<FatalisCuredEvent>>();

        // Events from Enemy Apocalypse die rolls.
        RegisterDerivation<EnemyApocalypseEvent, ApocalypseEventViewModel<EnemyApocalypseEvent>>();
    }

    /// <inheritdoc/>
    public override IApocalypseEventViewModel CreateChild(ApocalypseEvent model)
    {
        var viewModel = base.CreateChild(model);
            
        viewModel.EffectMessageMaxWidth = _effectMessageMaxWidth;

        return viewModel;
    }

    /// <summary>
    /// Provides the provided Apocalypse module configuration to this Apocalypse root view model instance.
    /// </summary>
    /// <param name="configuration">The Apocalypse module configuration to apply to this view model.</param>
    public void ApplyConfiguration(ApocalypseModuleConfiguration configuration)
    {
        Require.NotNull(configuration, nameof(configuration));

        _effectMessageMaxWidth = configuration.EffectMessageMaxWidth;
    }
}