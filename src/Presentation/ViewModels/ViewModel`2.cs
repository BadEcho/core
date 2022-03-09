//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Presentation.ViewModels;

/// <summary>
/// Provides a base view abstraction that automates communication between a view and bound
/// <typeparamref name="TModelImpl"/>-typed data and preserves assignment compatibility with other providers of
/// <typeparamref name="TModel"/>-typed data.
/// </summary>
/// <typeparam name="TModel"></typeparam>
/// <typeparam name="TModelImpl"></typeparam>
public abstract class ViewModel<TModel,TModelImpl> : ViewModel<TModelImpl>, IModelProvider<TModel>
    where TModelImpl : TModel
{
    TModel? IModelProvider<TModel>.ActiveModel
        => ActiveModel;
}