using AppraisalSystem.Application.Features.ChatUI;

namespace AppraisalSystem.Web.Services;

public class PropertyAsideService
{
    private PropertyListing? _listing;
    private bool _open;

    public event Action<PropertyListing?, bool>? OnChange;

    public bool IsOpen => _open;
    public PropertyListing? CurrentListing => _listing;

    public void Open(PropertyListing listing)
    {
        _listing = listing;
        _open = true;
        OnChange?.Invoke(_listing, _open);
    }

    public void Close()
    {
        _open = false;
        OnChange?.Invoke(_listing, _open);
    }
}