using Android.Views;
using AndroidX.AppCompat.Widget;
using Google.Android.Material.TextField;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Yosu.Platforms;

namespace Yosu.Handlers;

public class MaterialEntryHandler : ViewHandler<Entry, TextInputLayout>, IEntryHandler
{
    public static IPropertyMapper<IEntry, MaterialEntryHandler> Mapper = new MapperWorkaround<
        IEntry,
        MaterialEntryHandler
    >(EntryHandler.Mapper)
    {
        // Place holder maps to a different property
        [nameof(IEntry.Placeholder)] = MapPlaceHolder,
        [nameof(IEntry.Text)] = MapText,
    };

    private static void MapText(MaterialEntryHandler arg1, IEntry arg2)
    {
        arg1.PlatformView.EditText.Text = arg2.Text;
    }

    private static void MapPlaceHolder(MaterialEntryHandler arg1, IEntry arg2)
    {
        if (arg2 is IPlaceholder ph)
            arg1.PlatformView.Hint = ph.Placeholder;
    }

    public MaterialEntryHandler()
        : base(Mapper, null) { }

    IEntry IEntryHandler.VirtualView => base.VirtualView as IEntry;

    AppCompatEditText IEntryHandler.PlatformView => _editText;

    AppCompatEditText _editText;

    protected override TextInputLayout CreatePlatformView()
    {
        var layoutInflater = MauiContext.Services.GetService<LayoutInflater>();
        var view =
            layoutInflater.Inflate(Yosu.Resource.Layout.materialentry, null) as TextInputLayout;

        _editText = view.FindViewById<TextInputEditText>(Resource.Id.materialentry_entry);

        return view;
    }
}
