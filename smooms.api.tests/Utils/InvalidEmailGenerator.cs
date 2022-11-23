using System.Collections;

namespace smooms.api.tests.Utils;

public class InvalidEmailGenerator : IEnumerable<object[]>
{
    private readonly List<object[]> _data = new()
    {
        new object[] {""},
        new object[] {"invalidEmail"},
        new object[] {"invalidEmail@"},
        new object[] {"invalidEmail@invalidDomain"},
        new object[] {"invalidEmail@invalidDomain."},
        new object[] {"invalidEmail@invalidDomain.tooLong"},
        new object[] {"invalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmailinvalidEmail@invalidDomain.tooLong"},
    };

    public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}