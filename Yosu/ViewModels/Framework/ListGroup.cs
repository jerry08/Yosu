using System.Collections.Generic;

namespace Yosu.ViewModels.Framework;

public class ListGroup<T>(string name, List<T> items) : List<T>(items)
{
    public string Name { get; } = name;
}
