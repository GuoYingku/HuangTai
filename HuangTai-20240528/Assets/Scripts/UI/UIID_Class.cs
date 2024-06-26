using System;
public partial class UIID
{
    public string Name { get; private set; }
    public Type ViewType { get; private set; }
    public Type PresenterType { get; private set; }
    private UIID(string name, Type viewType, Type presenterType)
    {
        Name = name;
        ViewType = viewType;
        PresenterType = presenterType;
    }
}