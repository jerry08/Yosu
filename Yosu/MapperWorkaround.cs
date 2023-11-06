using System;
using Microsoft.Maui;

namespace Yosu.Platforms;

internal class MapperWorkaround<TVirtualView, TViewHandler>
    : PropertyMapper<TVirtualView, TViewHandler>
    where TVirtualView : IElement
    where TViewHandler : IElementHandler
{
    public MapperWorkaround(params IPropertyMapper[] chained)
        : base(chained) { }

    protected override void UpdatePropertyCore(
        string key,
        IElementHandler viewHandler,
        IElement virtualView
    )
    {
        try
        {
            base.UpdatePropertyCore(key, viewHandler, virtualView);
        }
        catch (InvalidCastException)
        {
            System.Diagnostics.Debug.WriteLine($"Somone needs to fix {key} {viewHandler}");
            // Someone didn't use interfaces everywhere for the mappers so we have to just ignore these exceptions
        }
    }
}
