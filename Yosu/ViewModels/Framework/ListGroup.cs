using System.Collections.Generic;

namespace Yosu.ViewModels.Framework;

public class ListGroup<T> : List<T>
{
    public string Name { get; }

    public ListGroup(string name, List<T> items)
        : base(items)
    {
        Name = name;
    }
}
