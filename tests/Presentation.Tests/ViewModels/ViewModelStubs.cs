//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Extensions;
using BadEcho.Presentation.ViewModels;

namespace BadEcho.Presentation.Tests.ViewModels;

public sealed class ViewModelStub : ViewModel<ModelStub>
{
    public int Value
    { get; private set; }

    protected override void OnBinding(ModelStub model)
        => Value = model.Value;

    protected override void OnUnbound(ModelStub model)
        => Value = 0;
}

public sealed class ModelStub
{
    private readonly string _name;

    public ModelStub(string name, int value)
    {
        _name = name;
        Value = value;
    }

    public int Value
    { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is not ModelStub otherModel)
            return false;

        return _name == otherModel._name;
    }

    public override int GetHashCode()
        => this.GetHashCode(_name);
}
    
public sealed class FakeAncestorViewModel : ViewModel, IAncestorViewModel<ViewModelStub>
{
    public AtomicObservableCollection<ViewModelStub> Children { get; }
        = new();

    public override void Disconnect()
    { }
}